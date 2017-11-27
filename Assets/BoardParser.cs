using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class BlockLocation
    {
        public bool Clickable { get; }

        public BlockLocation(BlockType type, int x, int y, bool clickable)
        {
            Clickable = clickable;
            Type = type;
            X = x;
            Y = y;
        }

        public BlockType Type { get; }
        public int X { get; }
        public int Y { get; }

        public override string ToString()
        {
            return $"({X},{Y}): {Type}";
        }
    }

    public static class BoardParser
    {

        static readonly Dictionary<char, BlockType> LetterLookup = new Dictionary<char, BlockType>
        {
            { 'n', BlockType.Normal},
            { 'w', BlockType.Walkable},
            { 'r', BlockType.RowInvert},
            { 'c', BlockType.ColumnInvert},
            { 'x', BlockType.RowAndColumnInvert},
            { 'p', BlockType.PreviousInvert},
            { 's', BlockType.SameInvert}
        };

        public static List<BlockLocation> ExtractBlockTypes(List<string> boardLines)
        {
            var blocks = new List<BlockLocation>();
            var y = 0;
            foreach (var boardLine in boardLines)
            {
                var x = 0;

                foreach (var c in boardLine)
                {
                    if (char.IsLetter(c))
                    {
                        var type = LetterLookup[char.ToLower(c)];
                        blocks.Add(new BlockLocation(type, x, y, char.IsUpper(c)));
                    }
                    x++;
                }
                y++;
            }
            return blocks;
        }
    }
}
