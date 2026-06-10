using System.Collections.Generic;
using JebbyJump.Analytics;
using JebbyJump.Progression;
using JebbyJump.Rewards;
using JebbyJump.Wardrobe;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    // Text-only wardrobe panel. Lists the fixed outfit catalog with
    // locked / unlocked / equipped state derived from total Stars (Stars
    // are read-only here and never consumed). Equipping persists only the
    // equipped outfit id (WardrobeStore). No art/sprite/animation; no
    // gameplay effect. Rows are built programmatically (no prefab).
    public class WardrobePanelController : MonoBehaviour
    {
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private RectTransform _rowContainer;
        [SerializeField] private TextMeshProUGUI _previewLabel;
        [SerializeField] private TextMeshProUGUI _stateLabel;
        [SerializeField] private Button _equipButton;
        [SerializeField] private Button _backButton;
        // Provides the level count for StarRewardStore.GetTotalStars on the
        // Main Menu (where there is no LevelSessionController).
        [SerializeField] private LevelCatalog _catalog;

        private readonly List<RowEntry> _rows = new List<RowEntry>();
        private string _selectedId;
        private bool _building;

        private struct RowEntry
        {
            public string Id;
            public Button Button;
            public TextMeshProUGUI Label;
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

            int totalStars = TotalStars;
            string equippedId = WardrobeUnlockService.NormalizeEquippedId(
                WardrobeStore.GetEquippedOutfitId(), totalStars);

            _building = true;
            foreach (var def in WardrobeCatalog.Outfits)
            {
                var row = CreateRow(def);
                _rows.Add(row);
            }
            _selectedId = equippedId;
            _building = false;

            RefreshLabels(totalStars, equippedId);
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

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);
            var label = labelGo.AddComponent<TextMeshProUGUI>();
            label.fontSize = 26;
            label.alignment = TextAlignmentOptions.Left;
            label.color = Color.white;
            var lrt = labelGo.GetComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = new Vector2(16f, 0f);
            lrt.offsetMax = new Vector2(-16f, 0f);

            string id = def.Id;
            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => OnRowClicked(id));

            return new RowEntry { Id = id, Button = btn, Label = label };
        }

        private void OnRowClicked(string id)
        {
            if (_building || id == _selectedId) return;
            _selectedId = id;

            var def = WardrobeCatalog.GetById(id);
            int totalStars = TotalStars;
            if (def != null)
            {
                AnalyticsService.Track(AnalyticsEvents.CosmeticPreviewed,
                    AnalyticsParam.Of(AnalyticsParams.CosmeticId, def.Id),
                    AnalyticsParam.Of(AnalyticsParams.CosmeticCategory,
                        def.Category.ToString()),
                    AnalyticsParam.Of(AnalyticsParams.RequiredStars, def.RequiredStars),
                    AnalyticsParam.Of(AnalyticsParams.CurrentStars, totalStars));
            }

            // Normalize like Rebuild does, so a known-but-locked stored id
            // (e.g. after a dev Stars reset) still displays as default.
            string equippedId = WardrobeUnlockService.NormalizeEquippedId(
                WardrobeStore.GetEquippedOutfitId(), totalStars);
            RefreshLabels(totalStars, equippedId);
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
            RefreshLabels(totalStars, newId);
        }

        private void RefreshLabels(int totalStars, string equippedId)
        {
            for (int i = 0; i < _rows.Count; i++)
            {
                var def = WardrobeCatalog.GetById(_rows[i].Id);
                if (def == null || _rows[i].Label == null) continue;
                var state = WardrobeUnlockService.GetState(
                    def, equippedId, totalStars);
                _rows[i].Label.text =
                    def.DisplayName + "  -  " + StateText(def, state);
            }

            var selected = WardrobeCatalog.GetById(_selectedId);
            if (_previewLabel != null)
            {
                _previewLabel.text = selected != null
                    ? "Selected: " + selected.DisplayName
                    : "Selected: --";
            }
            if (_stateLabel != null && selected != null)
            {
                var sel = WardrobeUnlockService.GetState(
                    selected, equippedId, totalStars);
                _stateLabel.text = StateText(selected, sel);
            }

            if (_equipButton != null)
            {
                bool unlocked = selected != null
                    && WardrobeUnlockService.IsUnlocked(selected, totalStars);
                bool already = selected != null && selected.Id == equippedId;
                _equipButton.interactable = unlocked && !already;
            }
        }

        private static string StateText(
            CosmeticItemDefinition def, WardrobeItemState state)
        {
            switch (state)
            {
                case WardrobeItemState.Equipped: return "Equipped";
                case WardrobeItemState.Unlocked: return "Unlocked";
                default: return "Locked (" + def.RequiredStars + " Stars)";
            }
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
