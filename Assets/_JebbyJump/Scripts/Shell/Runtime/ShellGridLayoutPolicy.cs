using UnityEngine;

namespace JebbyJump.Shell
{
    // A grid of equal cells (Level Select cards) with a header (title) and
    // footer (Back). For a vertically scrolling grid only the WIDTH must fit;
    // FitsFull also requires the height to fit without scrolling. Canvas
    // reference units.
    public readonly struct ShellGridSpec
    {
        public readonly int Count;
        public readonly int Columns;
        public readonly Vector2 Cell;
        public readonly float Spacing;
        public readonly float Padding;
        public readonly float HeaderHeight;
        public readonly float FooterHeight;

        public ShellGridSpec(
            int count, int columns, Vector2 cell, float spacing,
            float padding, float headerHeight, float footerHeight)
        {
            Count = count;
            Columns = columns;
            Cell = cell;
            Spacing = spacing;
            Padding = padding;
            HeaderHeight = headerHeight;
            FooterHeight = footerHeight;
        }

        public int Rows => Columns > 0 ? (Count + Columns - 1) / Columns : 0;

        public float RequiredWidth =>
            2f * Padding + Columns * Cell.x
            + Mathf.Max(0, Columns - 1) * Spacing;

        public float RequiredHeight =>
            2f * Padding + HeaderHeight + FooterHeight
            + Rows * Cell.y + Mathf.Max(0, Rows - 1) * Spacing;
    }

    public static class ShellGridLayoutPolicy
    {
        private const float Eps = 0.5f;

        // Scrolling grid: the row of columns must fit horizontally; the header
        // (title) + footer (Back) + at least one card row must fit vertically.
        public static bool FitsScrolling(Vector2 content, ShellGridSpec spec)
            => content.x > 0f && content.y > 0f
               && spec.RequiredWidth <= content.x + Eps
               && (2f * spec.Padding + spec.HeaderHeight + spec.FooterHeight
                   + spec.Cell.y) <= content.y + Eps;

        public static bool FitsFull(Vector2 content, ShellGridSpec spec)
            => content.x > 0f && content.y > 0f
               && spec.RequiredWidth <= content.x + Eps
               && spec.RequiredHeight <= content.y + Eps;
    }
}
