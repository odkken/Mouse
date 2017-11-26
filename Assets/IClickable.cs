using System;

namespace Assets
{
    public interface IClickable
    {
        event Action OnClicked;
    }
}