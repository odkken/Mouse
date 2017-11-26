using System.Collections;
using Assets.Interfaces;
using UnityEngine;

namespace Assets
{
    public enum BlockType
    {
        Goal,
        Normal,
        RowInvert,
        ColumnInvert,
        RowAndColumnInvert,
        PreviousInvert,
        SameInvert
    }

    [ExecuteInEditMode]
    public class Block : MonoBehaviour, IBlock
    {
        public Material OnMaterial;
        public Material OffMaterial;
        public AudioClip ClickDownClip;
        public AudioClip ClickUpClip;
        public bool Clickable;
        public IBoard Board { get; set; }

        public BlockType Type;

        // Use this for initialization
        void Start()
        {
            var source = GetComponent<AudioSource>();
            _block = transform.Find("block");
            _deactivatedBlockPos = _block.localPosition;
            _activatedBlockPos = _deactivatedBlockPos + Vector3.forward * activatedDepth;
            var lastClickTime = Time.time;
            GetComponentInChildren<IClickable>().OnClicked += () =>
            {

                if (Clickable && Time.time - lastClickTime > 0.1f)
                {
                    lastClickTime = Time.time;
                    Activated = !Activated;
                    source.PlayOneShot(Activated ? ClickDownClip : ClickUpClip);
                    StartCoroutine(Move(Activated));
                }
            };
        }

        void SetMaterial(bool activated)
        {
            transform.Find("block").Find("default").GetComponent<Renderer>().material = activated ? OnMaterial : OffMaterial;
        }

        private float activatedDepth = 0.2f;
        private float activationTime = .05f;
        private Transform _block;
        private Vector3 _deactivatedBlockPos;
        private Vector3 _activatedBlockPos;

        private bool _moving;
        IEnumerator Move(bool activated)
        {
            var t0 = Time.time;
            _moving = true;
            while (true)
            {
                var t = (Time.time - t0) / activationTime;
                _block.localPosition = Vector3.Lerp(_activatedBlockPos, _deactivatedBlockPos, activated ? 1 - t : t);
                if (t > 0.5)
                {
                    SetMaterial(activated);
                }
                if (t > 1)
                    break;
                yield return new WaitForEndOfFrame();
            }
            _moving = false;
        }
        
        void Update()
        {
            if (!_moving)
            {
                _block.localPosition = Activated ? _activatedBlockPos : _deactivatedBlockPos;
                SetMaterial(Activated);
            }
        }

        public void ToggleActivationWithoutEffect(bool activated)
        {
            Activated = activated;
        }

        public void ToggleActivationWithEffect(bool activated)
        {
            throw new System.NotImplementedException();
        }

        public bool Activated { get; set; }

        public bool IsGoal { get; set; }

        public void Undo()
        {
            throw new System.NotImplementedException();
        }

        public Vector2Int Coord { get; }
        public int X { get; }
        public int Y { get; }
        public void Disable()
        {
            throw new System.NotImplementedException();
        }

    }
}