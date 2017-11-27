using System;
using System.Collections;
using System.Linq;
using Assets.Interfaces;
using UnityEngine;

namespace Assets
{
    public enum BlockType
    {
        Normal,
        RowInvert,
        ColumnInvert,
        RowAndColumnInvert,
        PreviousInvert,
        SameInvert,
        Walkable
    }

    [ExecuteInEditMode]
    public class Block : MonoBehaviour, IBlock
    {
        public Material OnMaterial;
        public Material OffMaterial;
        public AudioClip ClickDownClip;
        public AudioClip ClickUpClip;

        public bool Clickable
        {
            get { return Type != BlockType.Walkable && _clickable; }
            set
            {
                _clickable = value;
                if (Type != BlockType.Walkable)
                    transform.Find("frame").GetComponentInChildren<Renderer>().material = _clickable ? OnMaterial : OffMaterial;
            }
        }

        public BlockType Type => BlockType;
        public IBoard Board { get; set; }

        public BlockType BlockType;

        // Use this for initialization
        void Start()
        {
            var source = GetComponent<AudioSource>();
            _block = transform.Find("block");
            _deactivatedBlockPos = Vector3.zero;
            _activatedBlockPos = _deactivatedBlockPos + Vector3.forward * activatedDepth;
            var lastClickTime = Time.time;
            if (Type != BlockType.Walkable)
                GetComponentInChildren<IClickable>().OnClicked += () =>
                {

                    if (Clickable && Time.time - lastClickTime > 0.1f)
                    {
                        lastClickTime = Time.time;
                        Activated = !Activated;
                        source.PlayOneShot(Activated ? ClickDownClip : ClickUpClip);
                        ToggleActivationWithEffect(Activated);
                        Board.AlertClick(this);
                        StartCoroutine(Move(Activated));
                    }
                };
        }

        void SetMaterial(bool activated)
        {
            transform.Find("block").Find("default").GetComponent<Renderer>().material = activated ? OnMaterial : OffMaterial;
        }

        private float activatedDepth = 0.2f;
        private float activationTime = .15f;
        private Transform _block;
        private Vector3 _deactivatedBlockPos;
        private Vector3 _activatedBlockPos;

        private bool _moving;
        private bool _activated;
        private bool _clickable;

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

            if (!_moving && BlockType != BlockType.Walkable)
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
            Activated = activated;
            switch (BlockType)
            {
                case BlockType.RowInvert:
                    Board.GetBlocksOnSameRow(this).ForEach(a => a.ToggleActivationWithoutEffect(!a.Activated));
                    break;
                case BlockType.ColumnInvert:
                    Board.GetBlocksOnSameColumn(this).ForEach(a => a.ToggleActivationWithoutEffect(!a.Activated));
                    break;
                case BlockType.RowAndColumnInvert:
                    Board.GetBlocksOnSameColumn(this).ForEach(a => a.ToggleActivationWithoutEffect(!a.Activated));
                    Board.GetBlocksOnSameRow(this).ForEach(a => a.ToggleActivationWithoutEffect(!a.Activated));
                    break;
                case BlockType.PreviousInvert:
                    {
                        var lastBlock = Board.GetLastActivatedBlock();
                        if (lastBlock != null && lastBlock != this)
                            lastBlock.ToggleActivationWithoutEffect(!lastBlock.Activated);
                    }
                    break;
                case BlockType.SameInvert:
                    break;
            }
        }

        public bool Activated
        {
            get { return _activated; }
            set
            {
                _activated = value;



            }
        }

        public void Undo()
        {
            throw new System.NotImplementedException();
        }

        public Vector2Int Coord => new Vector2Int(X, Y);
        public int X { get; set; }
        public int Y { get; set; }

        public void Disable()
        {
            throw new System.NotImplementedException();
        }

    }
}