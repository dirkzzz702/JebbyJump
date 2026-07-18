using System.Collections.Generic;
using JebbyJump.Analytics;
using JebbyJump.Progression;
using JebbyJump.Rewards;
using JebbyJump.Settings;
using JebbyJump.Wardrobe;
using JebbyJump.Wardrobe.Visual;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    // Wardrobe panel: a scrollable, data-driven list of the fixed outfit
    // catalog with locked / unlocked / equipped state derived from total
    // Stars (read-only; never consumed). Each row shows a UI-only idle
    // thumbnail (P15) + name + state, a "New" badge (P16), and a structural
    // selection marker (P20).
    //
    // P16 unlock ceremony: newly unlocked, unacknowledged outfits present one
    // at a time in a focus-trapping overlay. Acknowledgement is local and is
    // NOT ownership. Closing mid-ceremony does not acknowledge.
    //
    // P20 accessibility/mobile (landscape): a safe-area-fitted content root +
    // responsive region layout (with a compact variant for short screens),
    // deterministic keyboard/gamepad navigation + focus management (incl. a
    // real ceremony focus trap and scroll-the-focused-row-into-view), and a
    // Reduce Motion setting that freezes the preview to Idle. All new wiring is
    // null-safe; gameplay/reward/migration/analytics semantics are unchanged.
    public class WardrobePanelController : MonoBehaviour
    {
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private RectTransform _rowContainer;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private TextMeshProUGUI _previewLabel;
        [SerializeField] private TextMeshProUGUI _stateLabel;
        [SerializeField] private Image _selectedPreviewImage;
        [SerializeField] private Button _equipButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private LevelCatalog _catalog;
        [SerializeField] private WardrobePreviewLibrary _previewLibrary;

        // P20 responsive layout regions (optional/null-safe). Positioned by the
        // responsive layout calculator inside the safe-area-fitted root.
        [SerializeField] private RectTransform _safeAreaRoot;
        [SerializeField] private RectTransform _headerRegion;
        [SerializeField] private RectTransform _listRegion;
        [SerializeField] private RectTransform _previewRegion;
        [SerializeField] private RectTransform _actionRegion;

        // P16 unlock-ceremony overlay (optional/null-safe).
        [SerializeField] private GameObject _ceremonyOverlay;
        [SerializeField] private TextMeshProUGUI _ceremonyTitle;
        [SerializeField] private TextMeshProUGUI _ceremonyOutfitName;
        [SerializeField] private TextMeshProUGUI _ceremonyMessage;
        [SerializeField] private Image _ceremonyPreviewImage;
        [SerializeField] private Button _ceremonyEquipButton;
        [SerializeField] private Button _ceremonyContinueButton;

        private const float LockedPreviewAlpha = 0.4f;
        private const string CeremonySource = "unlock_ceremony";
        private const string WardrobeSource = "wardrobe";

        private readonly List<RowEntry> _rows = new List<RowEntry>();
        private string _selectedId;
        private bool _building;
        private WardrobeCeremonyPresenter _ceremony;

        // P18 in-panel preview carousel (UI-only; drives _selectedPreviewImage).
        private readonly WardrobePreviewPlayer _previewPlayer =
            new WardrobePreviewPlayer();
        private string _previewOutfitId;
        private bool _previewLocked;

        // P20 state.
        private bool _reduceMotion;
        private bool _motionSubscribed;
        private Vector2 _lastContentSize = new Vector2(-1f, -1f);
        private GameObject _opener;
        private int _lastFocusedRowIndex = -1;

        private struct RowEntry
        {
            public string Id;
            public Button Button;
            public TextMeshProUGUI Label;
            public Image Preview;
            public TextMeshProUGUI NewBadge;
            public Image SelectionMark;
        }

        private void Awake()
        {
            if (_panelRoot != null) _panelRoot.SetActive(false);
            if (_ceremonyOverlay != null) _ceremonyOverlay.SetActive(false);
            if (_equipButton != null)
                _equipButton.onClick.AddListener(OnEquipClicked);
            if (_backButton != null)
                _backButton.onClick.AddListener(Close);
            if (_ceremonyContinueButton != null)
                _ceremonyContinueButton.onClick.AddListener(OnCeremonyContinue);
            if (_ceremonyEquipButton != null)
                _ceremonyEquipButton.onClick.AddListener(OnCeremonyEquipNow);
            ConfigureCeremonyNavigation();
        }

        private void OnDestroy()
        {
            if (_equipButton != null)
                _equipButton.onClick.RemoveListener(OnEquipClicked);
            if (_backButton != null)
                _backButton.onClick.RemoveListener(Close);
            if (_ceremonyContinueButton != null)
                _ceremonyContinueButton.onClick.RemoveListener(OnCeremonyContinue);
            if (_ceremonyEquipButton != null)
                _ceremonyEquipButton.onClick.RemoveListener(OnCeremonyEquipNow);
            ClearRows();
        }

        // Preview state + the Reduce Motion subscription are cleared whenever the
        // panel is disabled (covers Close and any external deactivation).
        private void OnDisable()
        {
            ClearSelectedPreview();
            UnsubscribeMotion();
        }

        public void Open()
        {
            // Defensive repeat of the Main-Menu-init migration; idempotent and
            // raises no event/analytics. Ensures the equipped id is normalized
            // (e.g. now-locked after a Stars change) before the panel builds.
            WardrobePersistenceMigrator.MigrateIfNeeded(TotalStars);

            _opener = EventSystem.current != null
                ? EventSystem.current.currentSelectedGameObject : null;
            _reduceMotion = AccessibilitySettingsStore.ReduceMotion;
            SubscribeMotion();

            AnalyticsService.Track(AnalyticsEvents.WardrobeOpened);
            Rebuild();
            StartCeremonyQueue();
            if (_panelRoot != null) _panelRoot.SetActive(true);
            _lastContentSize = new Vector2(-1f, -1f); // re-apply layout next frame
            SetInitialFocus();
        }

        // Closing never acknowledges an in-progress ceremony; unacknowledged
        // outfits simply re-queue on the next Open().
        public void Close()
        {
            ClearSelectedPreview();
            if (_panelRoot != null) _panelRoot.SetActive(false);
            RestoreOpenerFocus();
        }

        private void Update()
        {
            if (_panelRoot == null || !_panelRoot.activeSelf) return;

            MaybeApplyLayout();

            // Real focus trap: while a ceremony is up, focus cannot leave its
            // controls (the underlying rows/Equip/Back are also non-interactable).
            if (_ceremony != null && _ceremony.IsActive)
            {
                EnforceCeremonyFocus();
                return;
            }

            ScrollFocusedRowIntoView();

            if (_reduceMotion) return; // static Idle; no pose cycling
            if (!_previewPlayer.HasFrames) return;
            _previewPlayer.Tick(Time.unscaledDeltaTime);
            ApplyCurrentPreviewFrame();
        }

        private int TotalStars =>
            _catalog != null ? StarRewardStore.GetTotalStars(_catalog.Count) : 0;

        // ---- responsive layout (P20) ----

        private void MaybeApplyLayout()
        {
            if (_safeAreaRoot == null) return;
            Vector2 size = _safeAreaRoot.rect.size;
            if (size.x <= 0f || size.y <= 0f) return;
            if (Mathf.Approximately(size.x, _lastContentSize.x)
                && Mathf.Approximately(size.y, _lastContentSize.y)) return;

            _lastContentSize = size;
            var layout = WardrobeResponsiveLayout.Compute(size);
            ApplyRegion(_headerRegion, layout.Header);
            ApplyRegion(_listRegion, layout.List);
            ApplyRegion(_previewRegion, layout.Preview);
            ApplyRegion(_actionRegion, layout.Actions);
        }

        private static void ApplyRegion(RectTransform rt, Rect r)
        {
            if (rt == null) return;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.zero;
            rt.pivot = Vector2.zero;
            rt.anchoredPosition = new Vector2(r.x, r.y);
            rt.sizeDelta = new Vector2(r.width, r.height);
        }

        // ---- focus / navigation (P20) ----

        private void SetInitialFocus()
        {
            if (_ceremony != null && _ceremony.IsActive) { FocusCeremony(); return; }
            var target = WardrobeFocusResolver.ResolvePanelFocus(
                _rows.Count, EquippedRowIndex());
            if (target == WardrobeFocusTarget.EquippedRow)
                SelectRow(EquippedRowIndex());
            else if (target == WardrobeFocusTarget.FirstRow)
                SelectRow(0);
        }

        private void SelectRow(int index)
        {
            if (index < 0 || index >= _rows.Count) return;
            var btn = _rows[index].Button;
            if (btn != null && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(btn.gameObject);
        }

        private int EquippedRowIndex()
        {
            string eq = WardrobePersistenceMigrator.GetEffectiveOutfitId(TotalStars);
            for (int i = 0; i < _rows.Count; i++)
                if (_rows[i].Id == eq) return i;
            return -1;
        }

        private void RestoreOpenerFocus()
        {
            if (EventSystem.current == null) return;
            EventSystem.current.SetSelectedGameObject(
                _opener != null && _opener.activeInHierarchy ? _opener : null);
        }

        private void EnforceCeremonyFocus()
        {
            if (EventSystem.current == null) return;
            var sel = EventSystem.current.currentSelectedGameObject;
            bool onCeremony =
                (_ceremonyEquipButton != null
                    && sel == _ceremonyEquipButton.gameObject)
                || (_ceremonyContinueButton != null
                    && sel == _ceremonyContinueButton.gameObject);
            if (!onCeremony) FocusCeremony();
        }

        private void FocusCeremony()
        {
            if (EventSystem.current == null) return;
            bool equipEnabled = _ceremonyEquipButton != null
                && _ceremonyEquipButton.interactable;
            var t = WardrobeFocusResolver.ResolveCeremonyFocus(equipEnabled);
            GameObject go =
                (t == WardrobeFocusTarget.CeremonyEquip
                    && _ceremonyEquipButton != null)
                ? _ceremonyEquipButton.gameObject
                : (_ceremonyContinueButton != null
                    ? _ceremonyContinueButton.gameObject : null);
            if (go != null) EventSystem.current.SetSelectedGameObject(go);
        }

        private void ScrollFocusedRowIntoView()
        {
            if (EventSystem.current == null) return;
            var sel = EventSystem.current.currentSelectedGameObject;
            if (sel == null) return;
            int idx = RowIndexOf(sel);
            if (idx < 0 || idx == _lastFocusedRowIndex) return;
            _lastFocusedRowIndex = idx;

            var sr = ResolveScrollRect();
            if (sr == null || sr.content == null || sr.viewport == null) return;
            float itemTop = WardrobeLayoutMetrics.ListPadding
                + idx * (WardrobeLayoutMetrics.RowMinHeight
                    + WardrobeLayoutMetrics.RowSpacing);
            sr.verticalNormalizedPosition =
                ScrollIntoViewCalculator.ComputeVerticalNormalized(
                    sr.content.rect.height, sr.viewport.rect.height,
                    itemTop, WardrobeLayoutMetrics.RowMinHeight,
                    sr.verticalNormalizedPosition);
        }

        private int RowIndexOf(GameObject go)
        {
            for (int i = 0; i < _rows.Count; i++)
                if (_rows[i].Button != null && _rows[i].Button.gameObject == go)
                    return i;
            return -1;
        }

        private ScrollRect ResolveScrollRect()
        {
            if (_scrollRect != null) return _scrollRect;
            if (_rowContainer != null)
                _scrollRect = _rowContainer.GetComponentInParent<ScrollRect>();
            return _scrollRect;
        }

        private void ConfigureNavigation()
        {
            for (int i = 0; i < _rows.Count; i++)
            {
                var btn = _rows[i].Button;
                if (btn == null) continue;
                var nav = new Navigation { mode = Navigation.Mode.Explicit };
                nav.selectOnUp = i > 0 ? _rows[i - 1].Button : null;
                nav.selectOnDown = i < _rows.Count - 1
                    ? _rows[i + 1].Button : _equipButton;
                btn.navigation = nav;
            }

            Button lastRow = _rows.Count > 0 ? _rows[_rows.Count - 1].Button : null;
            if (_equipButton != null)
                _equipButton.navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = lastRow, selectOnRight = _backButton,
                };
            if (_backButton != null)
                _backButton.navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = lastRow, selectOnLeft = _equipButton,
                };
        }

        private void ConfigureCeremonyNavigation()
        {
            if (_ceremonyEquipButton != null)
                _ceremonyEquipButton.navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnRight = _ceremonyContinueButton,
                };
            if (_ceremonyContinueButton != null)
                _ceremonyContinueButton.navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = _ceremonyEquipButton,
                };
        }

        private void SetRowsInteractable(bool value)
        {
            for (int i = 0; i < _rows.Count; i++)
                if (_rows[i].Button != null) _rows[i].Button.interactable = value;
        }

        // ---- reduce motion (P20; cached + event-driven) ----

        private void SubscribeMotion()
        {
            if (_motionSubscribed) return;
            AccessibilitySettingsStore.ReduceMotionChanged += OnReduceMotionChanged;
            _motionSubscribed = true;
        }

        private void UnsubscribeMotion()
        {
            if (!_motionSubscribed) return;
            AccessibilitySettingsStore.ReduceMotionChanged -= OnReduceMotionChanged;
            _motionSubscribed = false;
        }

        private void OnReduceMotionChanged(bool value)
        {
            _reduceMotion = value;
            if (_previewOutfitId != null)
                _previewPlayer.SetFrames(WardrobePreviewSequenceBuilder.Build(
                    _previewOutfitId, _previewLibrary,
                    includeHurt: false, reduceMotion: _reduceMotion));
            ApplyCurrentPreviewFrame();
        }

        // ---- build / refresh ----

        private void Rebuild()
        {
            ClearRows();
            if (_rowContainer == null) return;

            // Read-only effective id: an unsupported FUTURE save shows Classic in
            // memory without rewriting the save (Open's MigrateIfNeeded is a
            // no-op under a future schema); a supported save is lock-normalized.
            string equippedId =
                WardrobePersistenceMigrator.GetEffectiveOutfitId(TotalStars);

            _building = true;
            foreach (var def in WardrobeCatalog.Outfits)
                _rows.Add(CreateRow(def));
            _selectedId = equippedId;
            _building = false;

            ConfigureNavigation();
            _lastFocusedRowIndex = -1;
            Refresh();
        }

        private RowEntry CreateRow(CosmeticItemDefinition def)
        {
            var go = new GameObject(
                "OutfitRow_" + def.Id,
                typeof(RectTransform), typeof(CanvasRenderer),
                typeof(Image), typeof(Button), typeof(LayoutElement));
            go.transform.SetParent(_rowContainer, false);
            go.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f, 0.9f);
            go.GetComponent<LayoutElement>().minHeight =
                WardrobeLayoutMetrics.RowMinHeight; // P20 touch target

            // Structural selection marker (P20): a left accent bar shown only on
            // the selected row - a non-color-only cue. Non-raycast.
            var barGo = new GameObject(
                "SelectionBar",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            barGo.transform.SetParent(go.transform, false);
            var bar = barGo.GetComponent<Image>();
            bar.color = new Color(1f, 0.9f, 0.3f, 1f);
            bar.raycastTarget = false;
            bar.enabled = false;
            var barRT = barGo.GetComponent<RectTransform>();
            barRT.anchorMin = new Vector2(0f, 0f);
            barRT.anchorMax = new Vector2(0f, 1f);
            barRT.pivot = new Vector2(0f, 0.5f);
            barRT.sizeDelta = new Vector2(8f, 0f);
            barRT.anchoredPosition = Vector2.zero;

            var previewGo = new GameObject(
                "Preview",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            previewGo.transform.SetParent(go.transform, false);
            var previewImg = previewGo.GetComponent<Image>();
            previewImg.preserveAspect = true;
            previewImg.raycastTarget = false;
            previewImg.enabled = false;
            var prt = previewGo.GetComponent<RectTransform>();
            prt.anchorMin = new Vector2(0f, 0.5f);
            prt.anchorMax = new Vector2(0f, 0.5f);
            prt.pivot = new Vector2(0f, 0.5f);
            prt.anchoredPosition = new Vector2(20f, 0f);
            prt.sizeDelta = new Vector2(64f, 72f);

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);
            var label = labelGo.AddComponent<TextMeshProUGUI>();
            label.fontSize = 26;
            label.alignment = TextAlignmentOptions.Left;
            // Warm off-white per the 2026-07-18 typography system (matches
            // the scene-side SoftBody colour; badge below is already gold).
            label.color = new Color(0.95f, 0.93f, 0.88f);
            label.raycastTarget = false;
            label.enableWordWrapping = false;
            label.overflowMode = TextOverflowModes.Ellipsis;
            var lrt = labelGo.GetComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = new Vector2(96f, 0f);  // clear the thumbnail
            lrt.offsetMax = new Vector2(-96f, 0f); // leave room for the New badge

            // "New" badge (P16), hidden unless the row model says IsNew.
            var badgeGo = new GameObject("NewBadge", typeof(RectTransform));
            badgeGo.transform.SetParent(go.transform, false);
            var badge = badgeGo.AddComponent<TextMeshProUGUI>();
            badge.text = "New";
            badge.fontSize = 20;
            badge.fontStyle = FontStyles.Bold;
            badge.alignment = TextAlignmentOptions.MidlineRight;
            badge.color = new Color(1f, 0.9f, 0.3f, 1f);
            badge.raycastTarget = false;
            badge.enabled = false;
            var nrt = badgeGo.GetComponent<RectTransform>();
            nrt.anchorMin = new Vector2(1f, 0.5f);
            nrt.anchorMax = new Vector2(1f, 0.5f);
            nrt.pivot = new Vector2(1f, 0.5f);
            nrt.anchoredPosition = new Vector2(-12f, 0f);
            nrt.sizeDelta = new Vector2(72f, 36f);

            string id = def.Id;
            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => OnRowClicked(id));

            return new RowEntry
            {
                Id = id, Button = btn, Label = label,
                Preview = previewImg, NewBadge = badge, SelectionMark = bar,
            };
        }

        private void OnRowClicked(string id)
        {
            if (_building || id == _selectedId) return;
            _selectedId = id;

            var def = WardrobeCatalog.GetById(id);
            if (def != null)
            {
                AnalyticsService.Track(AnalyticsEvents.CosmeticPreviewed,
                    AnalyticsParam.Of(AnalyticsParams.CosmeticId, def.Id),
                    AnalyticsParam.Of(AnalyticsParams.CosmeticCategory,
                        def.Category.ToString()),
                    AnalyticsParam.Of(AnalyticsParams.RequiredStars, def.RequiredStars),
                    AnalyticsParam.Of(AnalyticsParams.CurrentStars, TotalStars));
            }

            Refresh();
        }

        private void OnEquipClicked()
        {
            var def = WardrobeCatalog.GetById(_selectedId);
            if (def == null) return;
            int totalStars = TotalStars;

            // Single validated equip path; a successful equip publishes the
            // change event (which live-updates any active player in this scene)
            // and the panel emits the canonical cosmetic_equipped once.
            var result = WardrobeEquipService.TryEquip(def.Id, totalStars);
            switch (result)
            {
                case WardrobeEquipResult.Success:
                    AnalyticsService.Track(AnalyticsEvents.CosmeticEquipped,
                        AnalyticsParam.Of(AnalyticsParams.CosmeticId, def.Id),
                        AnalyticsParam.Of(AnalyticsParams.CosmeticCategory,
                            def.Category.ToString()),
                        AnalyticsParam.Of(AnalyticsParams.IsOwned, true),
                        AnalyticsParam.Of(AnalyticsParams.Source, WardrobeSource));
                    Refresh();
                    break;
                case WardrobeEquipResult.Locked:
                    // Defensive: the Equip button is disabled for locked
                    // selections, so this normally cannot fire.
                    AnalyticsService.Track(AnalyticsEvents.CosmeticUnlockFailed,
                        AnalyticsParam.Of(AnalyticsParams.CosmeticId, def.Id),
                        AnalyticsParam.Of(AnalyticsParams.RequiredStars, def.RequiredStars),
                        AnalyticsParam.Of(AnalyticsParams.CurrentStars, totalStars));
                    break;
                // AlreadyEquipped / UnknownOutfit: no write, no event, no-op.
            }
        }

        private void Refresh()
        {
            var models = WardrobeRowModelBuilder.Build(
                WardrobePersistenceMigrator.GetEffectiveOutfitId(TotalStars),
                TotalStars, _previewLibrary,
                WardrobeUnlockAcknowledgementStore.IsAcknowledged);

            bool ceremonyActive = _ceremony != null && _ceremony.IsActive;
            for (int i = 0; i < _rows.Count; i++)
            {
                if (!TryGetModel(models, _rows[i].Id, out var m)) continue;
                if (_rows[i].Label != null)
                    _rows[i].Label.text = m.DisplayName + "  -  " + m.StateText;
                ApplyPreview(_rows[i].Preview, m);
                if (_rows[i].NewBadge != null) _rows[i].NewBadge.enabled = m.IsNew;
                if (_rows[i].SelectionMark != null)
                    _rows[i].SelectionMark.enabled = _rows[i].Id == _selectedId;
            }

            bool hasSelected = TryGetModel(models, _selectedId, out var selected);
            if (_previewLabel != null)
                _previewLabel.text = hasSelected
                    ? "Selected: " + selected.DisplayName
                    : "Selected: --";
            if (_stateLabel != null)
                _stateLabel.text = hasSelected ? selected.StateText : string.Empty;
            RefreshSelectedPreview(hasSelected, selected);
            if (_equipButton != null)
                _equipButton.interactable =
                    hasSelected && selected.CanEquip && !ceremonyActive;
        }

        // Rebuilds the preview carousel when the selected outfit changes (resets
        // to the first pose); otherwise just refreshes the locked dim. Honours
        // Reduce Motion (Idle-only). Animated frame driven by Update().
        private void RefreshSelectedPreview(
            bool hasSelected, WardrobeOutfitRowModel selected)
        {
            if (_selectedPreviewImage == null) return;
            if (!hasSelected) { ClearSelectedPreview(); return; }

            _previewLocked = !selected.IsUnlocked;
            if (selected.OutfitId != _previewOutfitId)
            {
                _previewOutfitId = selected.OutfitId;
                _previewPlayer.SetFrames(WardrobePreviewSequenceBuilder.Build(
                    selected.OutfitId, _previewLibrary,
                    includeHurt: false, reduceMotion: _reduceMotion));
            }
            ApplyCurrentPreviewFrame();
        }

        private void ApplyCurrentPreviewFrame()
        {
            if (_selectedPreviewImage == null) return;
            var sprite = _previewPlayer.HasFrames
                ? _previewPlayer.Current.Sprite : null;
            if (sprite == null) { HidePreview(_selectedPreviewImage); return; }
            _selectedPreviewImage.sprite = sprite;
            _selectedPreviewImage.enabled = true;
            _selectedPreviewImage.color = new Color(
                1f, 1f, 1f, _previewLocked ? LockedPreviewAlpha : 1f);
        }

        private void ClearSelectedPreview()
        {
            _previewOutfitId = null;
            _previewLocked = false;
            _previewPlayer.Clear();
            HidePreview(_selectedPreviewImage);
        }

        // ---- P16 unlock ceremony ----

        private void StartCeremonyQueue()
        {
            var items = WardrobeNewUnlockService.GetNewlyUnlocked(
                TotalStars, WardrobeUnlockAcknowledgementStore.IsAcknowledged);
            _ceremony = new WardrobeCeremonyPresenter(
                items,
                WardrobeUnlockAcknowledgementStore.MarkAcknowledged,
                TryEquipFromCeremony);

            if (_ceremony.IsActive) ShowCurrentCeremony();
            else HideCeremony();
        }

        // Injected equip path for the ceremony, routed through the shared
        // WardrobeEquipService. Success emits cosmetic_equipped
        // (source=unlock_ceremony) and publishes the change event. Returns true
        // for Success OR AlreadyEquipped; Locked/Unknown return false.
        private bool TryEquipFromCeremony(string id)
        {
            var def = WardrobeCatalog.GetById(id);
            if (def == null) return false;

            var result = WardrobeEquipService.TryEquip(id, TotalStars);
            if (result == WardrobeEquipResult.Success)
            {
                AnalyticsService.Track(AnalyticsEvents.CosmeticEquipped,
                    AnalyticsParam.Of(AnalyticsParams.CosmeticId, def.Id),
                    AnalyticsParam.Of(AnalyticsParams.CosmeticCategory,
                        def.Category.ToString()),
                    AnalyticsParam.Of(AnalyticsParams.IsOwned, true),
                    AnalyticsParam.Of(AnalyticsParams.Source, CeremonySource));
            }
            return result == WardrobeEquipResult.Success
                || result == WardrobeEquipResult.AlreadyEquipped;
        }

        private void ShowCurrentCeremony()
        {
            var def = _ceremony.Current;
            if (def == null) { HideCeremony(); return; }

            if (_ceremonyOverlay != null) _ceremonyOverlay.SetActive(true);
            // Focus trap: the underlying rows + Equip + Back are made
            // non-interactable so navigation cannot escape the ceremony, and
            // Update() re-asserts ceremony focus. Restored when it ends.
            SetRowsInteractable(false);
            if (_equipButton != null) _equipButton.interactable = false;
            if (_backButton != null) _backButton.interactable = false;

            if (_ceremonyTitle != null)
                _ceremonyTitle.text = "New Outfit Unlocked!";
            if (_ceremonyOutfitName != null)
                _ceremonyOutfitName.text = def.DisplayName;
            if (_ceremonyMessage != null)
                _ceremonyMessage.text = "You reached " + def.RequiredStars + " Stars!";

            Sprite preview = null;
            if (_previewLibrary != null)
                _previewLibrary.TryGetPreview(def.Id, out preview);
            if (_ceremonyPreviewImage != null)
            {
                if (preview != null)
                {
                    _ceremonyPreviewImage.sprite = preview;
                    _ceremonyPreviewImage.enabled = true;
                    _ceremonyPreviewImage.color = Color.white;
                }
                else { HidePreview(_ceremonyPreviewImage); }
            }
            if (_ceremonyEquipButton != null)
                _ceremonyEquipButton.interactable =
                    WardrobeUnlockService.IsUnlocked(def, TotalStars);

            FocusCeremony();

            AnalyticsService.Track(AnalyticsEvents.CosmeticUnlockPresented,
                AnalyticsParam.Of(AnalyticsParams.CosmeticId, def.Id),
                AnalyticsParam.Of(AnalyticsParams.CosmeticCategory,
                    def.Category.ToString()),
                AnalyticsParam.Of(AnalyticsParams.RequiredStars, def.RequiredStars),
                AnalyticsParam.Of(AnalyticsParams.CurrentStars, TotalStars),
                AnalyticsParam.Of(AnalyticsParams.Source, CeremonySource),
                AnalyticsParam.Of(AnalyticsParams.QueuePosition, _ceremony.Position),
                AnalyticsParam.Of(AnalyticsParams.QueueCount, _ceremony.Count));
        }

        private void HideCeremony()
        {
            if (_ceremonyOverlay != null) _ceremonyOverlay.SetActive(false);
            if (_backButton != null) _backButton.interactable = true;
        }

        private void OnCeremonyContinue()
        {
            if (_ceremony == null || !_ceremony.IsActive) return;
            var def = _ceremony.Current;
            int pos = _ceremony.Position, count = _ceremony.Count;

            _ceremony.Continue(); // acknowledges
            EmitAcknowledged(def, pos, count, "continue");
            AfterCeremonyAdvance();
        }

        private void OnCeremonyEquipNow()
        {
            if (_ceremony == null || !_ceremony.IsActive) return;
            var def = _ceremony.Current;
            int pos = _ceremony.Position, count = _ceremony.Count;

            // EquipNow emits cosmetic_equipped (source=unlock_ceremony) via
            // TryEquipFromCeremony on success, then acknowledges + advances.
            if (!_ceremony.EquipNow()) return; // failed equip: no ack/advance
            EmitAcknowledged(def, pos, count, "equip");
            Refresh(); // reflect the new equipped state in the rows beneath
            AfterCeremonyAdvance();
        }

        private static void EmitAcknowledged(
            CosmeticItemDefinition def, int pos, int count, string action)
        {
            AnalyticsService.Track(AnalyticsEvents.CosmeticUnlockAcknowledged,
                AnalyticsParam.Of(AnalyticsParams.CosmeticId, def.Id),
                AnalyticsParam.Of(AnalyticsParams.CosmeticCategory,
                    def.Category.ToString()),
                AnalyticsParam.Of(AnalyticsParams.RequiredStars, def.RequiredStars),
                AnalyticsParam.Of(AnalyticsParams.AcknowledgementAction, action),
                AnalyticsParam.Of(AnalyticsParams.QueuePosition, pos),
                AnalyticsParam.Of(AnalyticsParams.QueueCount, count));
        }

        private void AfterCeremonyAdvance()
        {
            if (_ceremony.IsActive)
            {
                ShowCurrentCeremony();
            }
            else
            {
                HideCeremony();
                Rebuild();        // refresh New badges + equipped + re-enable rows
                SetInitialFocus(); // return focus to the panel list
            }
        }

        // ---- helpers ----

        private static bool TryGetModel(
            IReadOnlyList<WardrobeOutfitRowModel> models, string id,
            out WardrobeOutfitRowModel model)
        {
            if (!string.IsNullOrEmpty(id))
            {
                for (int i = 0; i < models.Count; i++)
                {
                    if (models[i].OutfitId == id)
                    {
                        model = models[i];
                        return true;
                    }
                }
            }
            model = default;
            return false;
        }

        private static void ApplyPreview(Image img, WardrobeOutfitRowModel model)
        {
            if (img == null) return;
            if (model.PreviewSprite == null) { HidePreview(img); return; }
            img.sprite = model.PreviewSprite;
            img.enabled = true;
            float a = model.IsUnlocked ? 1f : LockedPreviewAlpha;
            img.color = new Color(1f, 1f, 1f, a);
        }

        private static void HidePreview(Image img)
        {
            if (img == null) return;
            img.sprite = null;
            img.enabled = false;
        }

        private void ClearRows()
        {
            for (int i = 0; i < _rows.Count; i++)
            {
                if (_rows[i].Button != null)
                {
                    _rows[i].Button.onClick.RemoveAllListeners();
                    Destroy(_rows[i].Button.gameObject);
                }
            }
            _rows.Clear();
        }
    }
}
