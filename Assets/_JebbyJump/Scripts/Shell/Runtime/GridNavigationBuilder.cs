namespace JebbyJump.Shell
{
    public enum GridDir { Up, Down, Left, Right }

    // Pure grid navigation: given a flat index into a left-to-right,
    // top-to-bottom grid of `count` items in `columns` columns, returns the
    // neighbour index in a direction, or -1 for "no move" (edge / out of a
    // partial last row). Used to wire explicit keyboard/gamepad navigation on
    // the Level Select cards (which the controller applies to Selectables).
    public static class GridNavigationBuilder
    {
        public static int Neighbor(int index, int count, int columns, GridDir dir)
        {
            if (index < 0 || index >= count || columns <= 0) return -1;
            int col = index % columns;
            switch (dir)
            {
                case GridDir.Left:
                    return col > 0 ? index - 1 : -1;
                case GridDir.Right:
                    return (col < columns - 1 && index + 1 < count)
                        ? index + 1 : -1;
                case GridDir.Up:
                    return index - columns >= 0 ? index - columns : -1;
                case GridDir.Down:
                    return index + columns < count ? index + columns : -1;
                default:
                    return -1;
            }
        }
    }
}
