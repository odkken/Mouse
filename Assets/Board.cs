using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Interfaces;
using MoreLinq;
using UnityEngine;

namespace Assets
{
    public class Board : MonoBehaviour, IBoard
    {
        public int Level;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public float BlockSpacing = 3;

        public void LoadFromFile()
        {
            foreach (var block in transform.GetComponentsInChildren<Block>().ToList())
            {
                DestroyImmediate(block.gameObject);
            }
            Blocks = new List<GameObject>();

            var lines = File.ReadAllLines("Assets/Levels.txt").ToList();
            for (int i = 0; i < Level; i++)
            {
                lines = lines.SkipUntil(a => a.Contains("//")).ToList();
            }
            var linesForThisLevel = lines.TakeUntil(a => a.Contains("//")).ToList();
            var blocks = BoardParser.ExtractBlockTypes(linesForThisLevel);


            foreach (var blockRow in blocks.GroupBy(a => a.Y))
            {
                foreach (var blockLocation in blockRow.OrderBy(a => a.X))
                {
                    GameObject newBlock;
                    switch (blockLocation.Type)
                    {
                        case BlockType.Goal:
                            newBlock = Instantiate(GoalBlockPrefab);
                            break;
                        case BlockType.Normal:
                            newBlock = Instantiate(NormalBlockPrefab);
                            break;
                        case BlockType.RowInvert:
                            newBlock = Instantiate(RowInvertBlockPrefab);
                            break;
                        case BlockType.ColumnInvert:
                            newBlock = Instantiate(ColumnInvertBlockPrefab);
                            break;
                        case BlockType.RowAndColumnInvert:
                            newBlock = Instantiate(RowColumnInvertBlockPrefab);
                            break;
                        case BlockType.PreviousInvert:
                            newBlock = Instantiate(PreviousInvertBlockPrefab);
                            break;
                        case BlockType.SameInvert:
                            newBlock = Instantiate(SameInvertBlockPrefab);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    var blockComponent = newBlock.GetComponent<IBlock>();
                    blockComponent.ToggleActivationWithoutEffect(blockLocation.Activated);
                    newBlock.transform.position = new Vector3(blockLocation.X, blockLocation.Y, 0) * BlockSpacing;
                    newBlock.transform.parent = transform;
                    Blocks.Add(newBlock);
                }
            }
            Camera.main.transform.position = new Vector3((float) blocks.Average(a=> a.X) * BlockSpacing, (float) blocks.Average(a=> a.Y) * BlockSpacing, CameraZ);
        }

        public float CameraZ;

        public GameObject GoalBlockPrefab;
        public GameObject NormalBlockPrefab;
        public GameObject RowInvertBlockPrefab;
        public GameObject ColumnInvertBlockPrefab;
        public GameObject RowColumnInvertBlockPrefab;
        public GameObject PreviousInvertBlockPrefab;
        public GameObject SameInvertBlockPrefab;

        private List<GameObject> Blocks = new List<GameObject>();

        public List<IBlock> GetBlocksOnSameRow(IBlock asBlock)
        {
            throw new System.NotImplementedException();
        }

        public List<IBlock> GetBlocksOnSameColumn(IBlock asBlock)
        {
            throw new System.NotImplementedException();
        }

        public List<IBlock> GetLastActivatedBlock(IBlock block)
        {
            throw new System.NotImplementedException();
        }
    }
}
