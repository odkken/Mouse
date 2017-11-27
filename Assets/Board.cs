using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Interfaces;
using MoreLinq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets
{
    [RequireComponent(typeof(AudioSource))]
    public class Board : MonoBehaviour, IBoard
    {
        public int Level;
        public AudioClip CompletedClip;
        // Use this for initialization
        void Start()
        {
            LoadFromFile(true);
        }

        private bool completed = false;
        // Update is called once per frame
        void Update()
        {
            if (!completed)
            {
                var blocks = GetComponentsInChildren<IBlock>();
                if (blocks.Where(a => a.Type != BlockType.Walkable).All(a => a.Activated))
                {
                    completed = true;
                    foreach (var block in blocks)
                    {
                        block.Clickable = false;
                    }
                    GetComponent<AudioSource>().PlayOneShot(CompletedClip);
                    StartCoroutine(LoadNextLevel());
                }
            }
        }

        IEnumerator LoadNextLevel()
        {
            yield return new WaitForSeconds(1);
            Level++;
            LoadFromFile(false);
            completed = false;
        }

        public float BlockSpacing = 3;



        public void LoadFromFile(bool inEditor)
        {
            var childrenToDestroy = Enumerable.Range(0, transform.childCount).Select(a => transform.GetChild(a)).ToList();
            foreach (var block in childrenToDestroy)
            {
                if (inEditor)
                    DestroyImmediate(block.gameObject);
                else
                    Destroy(block.gameObject);
            }
            Blocks = new List<GameObject>();

            var lines = File.ReadAllLines("Assets/Levels.txt").ToList();
            for (int i = 0; i < Level; i++)
            {
                lines = lines.SkipUntil(a => a.Contains("//")).ToList();
            }
            var linesForThisLevel = lines.TakeUntil(a => a.Contains("//")).ToList();
            BlockInfos = BoardParser.ExtractBlockTypes(linesForThisLevel);


            foreach (var blockRow in BlockInfos.GroupBy(a => a.Y))
            {
                foreach (var blockLocation in blockRow.OrderBy(a => a.X))
                {
                    GameObject newBlock;
                    switch (blockLocation.Type)
                    {
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
                        case BlockType.Walkable:
                            newBlock = Instantiate(WalkableBlockPrefab);
                            var player = Instantiate(PlayerPrefab);
                            var playerComp = player.GetComponent<Player>();
                            playerComp.Board = this;
                            playerComp.Coord = new Vector2(blockLocation.X, blockLocation.Y);
                            playerComp.MoveToAction = (player1, destination) =>
                            {
                                var blockComps = Blocks.Select(a => a.GetComponent<IBlock>());
                                if (blockComps.Any(a => a.Coord == destination))
                                {
                                    player1.Coord = destination;
                                    player1.transform.position = new Vector3(destination.x, -destination.y, player1.transform.position.z) * BlockSpacing;
                                    var destBlock = blockComps.Single(a => a.Coord == destination);
                                    destBlock.ToggleActivationWithEffect(!destBlock.Activated);
                                }
                            };
                            player.transform.position = new Vector3(blockLocation.X, -blockLocation.Y, 0) * BlockSpacing;
                            player.transform.parent = transform;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    var blockComponent = newBlock.GetComponent<Block>();
                    blockComponent.Board = this;
                    blockComponent.X = blockLocation.X;
                    blockComponent.Y = blockLocation.Y;
                    blockComponent.Clickable = blockLocation.Clickable;
                    newBlock.transform.position = new Vector3(blockLocation.X, -blockLocation.Y, 0) * BlockSpacing;
                    newBlock.transform.parent = transform;
                    Blocks.Add(newBlock);
                }
            }
            Camera.main.transform.position = new Vector3((float)BlockInfos.Average(a => a.X) * BlockSpacing, (float)BlockInfos.Average(a => a.Y) * -BlockSpacing, CameraZ);
        }

        public GameObject PlayerPrefab;

        public float CameraZ;

        public GameObject WalkableBlockPrefab;
        public GameObject NormalBlockPrefab;
        public GameObject RowInvertBlockPrefab;
        public GameObject ColumnInvertBlockPrefab;
        public GameObject RowColumnInvertBlockPrefab;
        public GameObject PreviousInvertBlockPrefab;
        public GameObject SameInvertBlockPrefab;

        public List<GameObject> Blocks = new List<GameObject>();
        public List<BlockLocation> BlockInfos;
        private IBlock _lastClickedBlock;

        public List<IBlock> GetBlocksOnSameRow(IBlock asBlock)
        {
            return Blocks.Select(a => a.GetComponent<IBlock>()).Where(a => a != asBlock && a.Y == asBlock.Y).ToList();
        }

        public List<IBlock> GetBlocksOnSameColumn(IBlock asBlock)
        {
            return Blocks.Select(a => a.GetComponent<IBlock>()).Where(a => a != asBlock && a.X == asBlock.X).ToList();
        }

        public IBlock GetLastActivatedBlock()
        {
            return _lastClickedBlock;
        }

        public void AlertClick(IBlock block)
        {
            _lastClickedBlock = block;
        }

        public List<IBlock> GetAdjacentBlocks(IBlock block)
        {
            return Blocks.Select(a => a.GetComponent<IBlock>()).Where(a => (a.Coord - block.Coord).sqrMagnitude == 1).ToList();
        }
    }
}
