using System.Collections.Generic;
using JebbyJump.Analytics;
using JebbyJump.Progression;
using JebbyJump.Rewards;
using JebbyJump.Wardrobe;
using JebbyJump.Wardrobe.Visual;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    // Wardrobe panel: a scrollable, data-driven list of the fixed outfit
    // catalog with locked / unlocked / equipped state derived from total
    // Stars (read-only; never consumed). Each row shows a UI-only idle
    // thumbnail (from WardrobePreviewLibrary; locked rows dimmed, missing
    // previews hidden) plus name + state; a larger preview shows the selected
    // outfit. Per-row view data comes from the pure WardrobeRowModelBuilder.
    // Equipping persists only the equipped outfit id (WardrobeStore); the
    // gameplay appearance applies at the next player spawn / scene load (no
    // live mid-scene re-sync). No gameplay effect. Rows are built
    // programmatically (no row prefab).
    public class WardrobePanelController : MonoBehaviour
    {
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private RectTransform _rowContainer;
        [SerializeField] private TextMeshProUGUI _previewLabel;
        [SerializeField] private TextMeshProUGUI _stateLabel;
        // Optional larger preview of the currently-selected outfit.
        [SerializeField] private Image _selectedPreviewImage;
        [SerializeField] private Button _equipButton;
        [SerializeField] private Button _backButton;
        // Provides the level count for StarRewardStore.GetTotalStars on the
        // Main Menu (where there is no LevelSessionController).
        [SerializeField] private LevelCatalog _catalog;
        // UI-only outfit thumbnails; optional/null-safe (missing -> no image).
        [SerializeField] private WardrobePreviewLibrary _previewLibrary;

        private const float LockedPreviewAlpha = 0.4f;

        private readonly List<RowEntry> _rows = new List<RowEntry>();
        private string _selectedId;
        private bool _building;

        private struct RowEntry
        {
            public string Id;
            public Button Button;
            public TextMeshProUGUI Label;
            public Image Preview;
        }

        private void Awake()
        {
            if (_panelRoot != null) _panelRoot.SetActive(false);
            if (_equipButton != null)
                _equipButton.onClick.AddListener(OnEquipClicked);
            if (_backButton != null)
                _backButton.onClick.AddListener(Close);
        }

        private void OnDestroy()
        {
            if (_equipButton != null)
                _equipButton.onClick.RemoveListener(OnEquipClicked);
            if (_backButton != null)
                _backButton.onClick.RemoveListener(Close);
            ClearRows();
        }

        public void Open()
        {
            AnalyticsService.Track(AnalyticsEvents.WardrobeOpened);
            Rebuild();
            if (_panelRoot != null) _panelRoot.SetActive(true);
        }

        public void Close()
        {
            if (_panelRoot != null) _panelRoot.SetActive(false);
        }

        private int TotalStars =>
            _catalog != null ? StarRewardStore.GetTotalStars(_catalog.Count) : 0;

        private void Rebuild()
        {
            ClearRows();
            if (_rowContainer == null) return;

            string equippedId = WardrobeUnlockService.NormalizeEquippedId(
                WardrobeStore.GetEquippedOutfitId(), TotalStars);

            _building = true;
            foreach (var def in WardrobeCatalog.Outfits)
                _rows.Add(CreateRow(def));
            _selectedId = equippedId;
            _building = false;

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
            go.GetComponent<LayoutElement>().minHeight = 64f;

            // UI-only thumbnail slot on the left (clicks pass through to the
            // row button). Hidden until a sprite is assigned in Refresh.
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
            prt.anchoredPosition = new Vector2(8f, 0f);
            prt.sizeDelta = new Vector2(52f, 56f);

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);
            var label = labelGo.AddComponent<TextMeshProUGUI>();
            label.fontSize = 26;
            label.alignment = TextAlignmentOptions.Left;
            label.color = Color.white;
            label.raycastTarget = false; // row button owns the clicks
            var lrt = labelGo.GetComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = new Vector2(72f, 0f); // clear the thumbnail
            lrt.offsetMax = new Vector2(-16f, 0f);

            string id = def.Id;
            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => OnRowClicked(id));

            return new RowEntry
            { Id = id, Button = btn, Label = label, Preview = previewImg };
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

            if (!WardrobeUnlockService.IsUnlocked(def, totalStars))
            {
                // Defensive: the Equip button is disabled for locked
                // selections, so this normally cannot fire.
                AnalyticsService.Track(AnalyticsEvents.CosmeticUnlockFailed,
                    AnalyticsParam.Of(AnalyticsParams.CosmeticId, def.Id),
                    AnalyticsParam.Of(AnalyticsParams.RequiredStars, def.RequiredStars),
                    AnalyticsParam.Of(AnalyticsParams.CurrentStars, totalStars));
                return;
            }

            string newId = WardrobeStore.SetEquippedOutfitId(def.Id);
            AnalyticsService.Track(AnalyticsEvents.CosmeticEquipped,
                AnalyticsParam.Of(AnalyticsParams.CosmeticId, newId),
                AnalyticsParam.Of(AnalyticsParams.CosmeticCategory,
                    def.Category.ToString()),
                AnalyticsParam.Of(AnalyticsParams.IsOwned, true));
            Refresh();
        }

        // Renders every row + the selected-outfit details from the pure
        // builder (single source of truth). Never emits analytics.
        private void Refresh()
        {
            var models = WardrobeRowModelBuilder.Build(
                WardrobeStore.GetEquippedOutfitId(), TotalStars, _previewLibrary);

            for (int i = 0; i < _rows.Count; i++)
            {
                if (!TryGetModel(models, _rows[i].Id, out var m)) continue;
                if (_rows[i].Label != null)
                    _rows[i].Label.text = m.DisplayName + "  -  " + m.StateText;
                ApplyPreview(_rows[i].Preview, m);
            }

            bool hasSelected = TryGetModel(models, _selectedId, out var selected);
            if (_previewLabel != null)
                _previewLabel.text = hasSelected
                    ? "Selected: " + selected.DisplayName
                    : "Selected: --";
            if (_stateLabel != null)
                _stateLabel.text = hasSelected ? selected.StateText : string.Empty;
            if (_selectedPreviewImage != null)
            {
                if (hasSelected) ApplyPreview(_selectedPreviewImage, selected);
                else HidePreview(_selectedPreviewImage);
            }
            if (_equipButton != null)
                _equipButton.interactable = hasSelected && selected.CanEquip;
        }

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

        // Shows the thumbnail (dimmed when locked) or hides it when missing.
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
