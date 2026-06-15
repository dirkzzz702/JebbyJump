using System.Collections.Generic;
using JebbyJump.Shell;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    // Application-layer focus/navigation helpers shared by the shell panels
    // (MainMenu, Level Select, Settings, Pause, Result, Game Over). Pure
    // decisions come from JebbyJump.Shell.*; this maps them onto Selectables +
    // the EventSystem. Not unit-tested directly (Assembly-CSharp) - covered by
    // the pure helper tests + scene-integrity. P21 accessibility hardening.
    public static class ShellFocusUtil
    {
        public static void Select(GameObject go)
        {
            if (EventSystem.current == null) return;
            EventSystem.current.SetSelectedGameObject(
                go != null && go.activeInHierarchy ? go : null);
        }

        public static void Select(Selectable s)
            => Select(s != null ? s.gameObject : null);

        // Explicit vertical navigation over items (nulls skipped). The last
        // item's Down goes to `after` (e.g. Back), and `after` Up returns to it.
        public static void BuildVerticalNavigation(
            IList<Selectable> items, Selectable after = null)
        {
            var live = new List<Selectable>();
            if (items != null)
                foreach (var s in items) if (s != null) live.Add(s);

            for (int i = 0; i < live.Count; i++)
            {
                var nav = new Navigation { mode = Navigation.Mode.Explicit };
                nav.selectOnUp = i > 0 ? live[i - 1] : null;
                nav.selectOnDown = i < live.Count - 1 ? live[i + 1] : after;
                live[i].navigation = nav;
            }
            if (after != null)
            {
                var nav = new Navigation { mode = Navigation.Mode.Explicit };
                nav.selectOnUp = live.Count > 0 ? live[live.Count - 1] : null;
                after.navigation = nav;
            }
        }

        // Explicit grid navigation (true up/down/left/right incl. partial last
        // row, via GridNavigationBuilder). Bottom-row Down -> `after` (Back).
        public static void BuildGridNavigation(
            IList<Selectable> cards, int columns, Selectable after = null)
        {
            int n = cards != null ? cards.Count : 0;
            for (int i = 0; i < n; i++)
            {
                var s = cards[i];
                if (s == null) continue;
                var nav = new Navigation { mode = Navigation.Mode.Explicit };
                nav.selectOnLeft = At(cards,
                    GridNavigationBuilder.Neighbor(i, n, columns, GridDir.Left));
                nav.selectOnRight = At(cards,
                    GridNavigationBuilder.Neighbor(i, n, columns, GridDir.Right));
                nav.selectOnUp = At(cards,
                    GridNavigationBuilder.Neighbor(i, n, columns, GridDir.Up));
                int down = GridNavigationBuilder.Neighbor(i, n, columns, GridDir.Down);
                nav.selectOnDown = down >= 0 ? cards[down] : after;
                s.navigation = nav;
            }
            if (after != null)
            {
                var nav = new Navigation { mode = Navigation.Mode.Explicit };
                nav.selectOnUp = n > 0 ? cards[n - 1] : null;
                after.navigation = nav;
            }
        }

        // Modal focus trap: if focus has drifted outside `allowed`, snap it back
        // to `fallback`. Combined with a raycast-blocking backdrop (pointer) and
        // an explicit-nav island, this blocks both pointer and navigation access
        // to underlying controls without modifying them.
        public static void ReassertWithin(
            IReadOnlyList<GameObject> allowed, GameObject fallback)
        {
            if (EventSystem.current == null) return;
            var sel = EventSystem.current.currentSelectedGameObject;
            if (sel != null && allowed != null)
                for (int i = 0; i < allowed.Count; i++)
                    if (allowed[i] == sel) return;
            Select(fallback);
        }

        private static Selectable At(IList<Selectable> list, int i)
            => i >= 0 && i < list.Count ? list[i] : null;
    }
}
