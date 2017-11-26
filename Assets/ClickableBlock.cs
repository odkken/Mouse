using System;
using UnityEngine;

namespace Assets
{
    public class ClickableBlock : MonoBehaviour, IClickable
    {
        void OnMouseDown()
        {
            OnClicked?.Invoke();
        }

        public event Action OnClicked;
    }
}
