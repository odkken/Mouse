using UnityEngine;

namespace Assets
{
    public interface IBlock
    {
        void ToggleActivationWithoutEffect(bool activated);
        void ToggleActivationWithEffect(bool activated);
        bool Activated { get;}
        bool IsGoal { get; }
        Vector2Int Coord { get; }
        int X { get; }
        int Y { get; }
    }
}