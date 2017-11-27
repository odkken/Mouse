using UnityEngine;

namespace Assets
{
    public interface IBlock
    {
        void ToggleActivationWithoutEffect(bool activated);
        void ToggleActivationWithEffect(bool activated);
        bool Activated { get; }
        Vector2Int Coord { get; }
        int X { get; set; }
        int Y { get; set; }
        bool Clickable { get; set; }
        BlockType Type { get; }
    }
}