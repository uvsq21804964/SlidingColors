// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using DG.Tweening;
// using TMPro;
// using UnityEngine;

// public class GameManagerGenerateur : MonoBehaviour {

//     static public GameManagerGenerateur Instance;
//     private int _width;
//     private int _height;
//     public static int Level;
//     public static string _solution;
//     [SerializeField] private Node _nodePrefab;
//     [SerializeField] private Block _blockPrefab;
//     [SerializeField] private SpriteRenderer _boardPrefab;
//     [SerializeField] private List<Tile> _tileTypes;
//     [SerializeField] private float _travelTime = 0.1f;
//     [SerializeField] private GameObject _mergeEffectPrefab;
//     [SerializeField] private FloatingText _floatingTextPrefab;
//     [SerializeField] private RoundText _roundTextPrefab;
//     [SerializeField] public static int _round;

//     public static event Action OnGameStarted;
//     public static event Action OnGameEnded;

//     [SerializeField] private GameObject _winScreen, _loseScreen, _winScreenText;
//     [SerializeField] private AudioClip[] _moveClips;
//     [SerializeField] private AudioClip[] _matchClips;
//     [SerializeField] private AudioSource _source;

//     private List<Node> _nodes;
//     private List<Block> _blocks;
//     private GameState _state;
//     private string _fileName;
//     private readonly System.Random random = new System.Random();

//     private Tile GetBlockTypeByValue(TileColor value) => _tileTypes.First(t => t.Color == value);

//     private static Game game;

//     void Awake()
//     {
//         Application.targetFrameRate = 120;

//         Instance = this;
//     }

//     static public void FindSolution(int nRows, int nCols) {
//         game = new(nRows, nCols);
//         Game gameClone = game.Clone();
//         Analyze analyze = new(gameClone);
//         analyze.Run();
//     }

//     void Start() {
//         // int nRows = 6;
//         // int nCols = 6;
//         // int probaRows = random.Next(0, 100);
//         // int probaCols = random.Next(0, 100);
//         // if(probaRows < 50) {
//         //     nRows = 4;
//         // } else if(probaRows < 80) {
//         //     nRows = 5;
//         // }
//         // if(probaCols < 20) {
//         //     nCols = 3;
//         // } else if(probaCols < 50) {
//         //     nCols = 4;
//         // } else if(probaCols < 80) {
//         //     nCols = 5;
//         // }
//         // FindSolution(nRows, nCols);
//        Level = PlayerPrefs.GetInt("Level", 1);
//        _fileName = "Assets/Scripts/Levels/Level" + Level + ".txt";
//        ChangeState(GameState.GenerateLevel);
//        ChangeState(GameState.MainMenu);
//     }

//     private void ChangeState(GameState newState) {
//         _state = newState;

//         switch (newState) {
//             case GameState.GenerateLevel:
//                 GetSizeGrid();
//                 GenerateGrid();
//                 GenerateBlocks();
//                 Instantiate(_roundTextPrefab, new Vector2((float) _width /2 - 0.5f,(float) - 2f), Quaternion.identity).Init();
//                 break;
//             case GameState.WaitingInput:
//                 break;
//             case GameState.Moving:
//                 break;
//             case GameState.MainMenu:
//                 break;
//             case GameState.Win:
//                 LevelManager.Instance.IncrementLevel();
//                 _winScreen.SetActive(true);
//                 Invoke(nameof(DelayedWinScreenText),1.5f);
//                 break;
//             case GameState.Lose:
//                 _loseScreen.SetActive(true);
//                 break;
//             default:
//                 throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
//         }
//     }

//     void DelayedWinScreenText() {
//         _winScreenText.SetActive(true);
//     }

//     void Update() {
//         if(_state != GameState.WaitingInput) return;

//         if(Input.GetKeyDown(KeyCode.LeftArrow)) Shift(Vector2.left);
//         if(Input.GetKeyDown(KeyCode.RightArrow)) Shift(Vector2.right);
//         if(Input.GetKeyDown(KeyCode.UpArrow)) Shift(Vector2.up);
//         if(Input.GetKeyDown(KeyCode.DownArrow)) Shift(Vector2.down);
//     }

//     // void GetSizeGrid() {
//     //     string data;
        
//     //     List<string> lines = new List<string>();

//     //     StreamReader reader = new StreamReader(_fileName);
//     //     data = reader.ReadLine();

//     //     do {
//     //         lines.Add(data);
//     //         data = reader.ReadLine();
//     //     } while (data != null);

//     //     if (reader != null) {
//     //         reader.Close();
//     //     }

//     //     _width = lines[0].Length;
//     //     _height = lines.Count;
//     // }

//     void GetSizeGrid() {
//         string data;
        
//         List<string> lines = new List<string>();

//         StreamReader reader = new StreamReader(_fileName);
//         data = reader.ReadLine();
//         string[] valeurs = data.Split(' ');
//         char width = valeurs[1][0];
//         char height = valeurs[0][0];

//         _width =  (int)(width - '0');
//         _height =  (int)(height - '0');
        
//         reader?.Close();
//     }

//     void GenerateGrid() {
//         _round = 0;
//         _nodes = new List<Node>();
//         _blocks = new List<Block>();
//         for (int x = 0; x < _width; x++) {
//             for (int y = 0; y < _height; y++) {
//                 var node = Instantiate(_nodePrefab, new Vector2(x, y), Quaternion.identity);
//                 _nodes.Add(node);
//             }
//         }

//         var center = new Vector2((float) _width /2 - 0.5f,(float) _height / 2 -0.5f);

//         var board = Instantiate(_boardPrefab, center, Quaternion.identity);
//         board.size = new Vector2(_width,_height);

//         Camera.main.transform.position = new Vector3(center.x,center.y,-10);
//         Camera.main.orthographicSize = Mathf.Max(_width,_height) * 1.25f;
//     }

//     void GenerateBlocks() {
//         var freeNodes = _nodes.ToList();
//         string data;
        
//         List<string> lines = new();

//         StreamReader reader = new(_fileName);
//         data = reader.ReadLine();

//         do {
//             data = reader.ReadLine();
//             lines.Add(data);
//         } while (data != null);

//         _solution = lines[lines.Count - 3];
//         Debug.Log("Solution " + _solution);

//         reader?.Close();

//         foreach (var node in freeNodes) {
//             string value = lines[(int) node.Pos.y].Split(' ')[(int) node.Pos.x];
            
//             switch (value){
//             case "Mur":
//                 SpawnWall(node);
//                 break;
//             case "Green":
//                 SpawnBlock(node, TileColor.Green);
//                 break;
//             case "Red":
//                 SpawnBlock(node, TileColor.Red);
//                 break;
//             case "Yellow":
//                 SpawnBlock(node, TileColor.Yellow);
//                 break;
//             case "Purple":
//                 SpawnBlock(node, TileColor.Purple);
//                 break;
//             case "Orange":
//                 SpawnBlock(node, TileColor.Orange);
//                 break;
//             case "Cyan":
//                 SpawnBlock(node, TileColor.Turquoise);
//                 break;
//             case "Pink":
//                 SpawnBlock(node, TileColor.Pink);
//                 break;
//             case "Blue":
//                 SpawnBlock(node, TileColor.Blue);
//                 break;

//             }
//         }
//     }

//     public void StartGame() {
//         ChangeState(GameState.WaitingInput);
//     }

//     void SpawnBlock(Node node, TileColor value) {
//         var block = Instantiate(_blockPrefab, node.Pos, Quaternion.identity);
//         block.Init(GetBlockTypeByValue(value));
//         block.SetBlock(node);
//         _blocks.Add(block);
//     }

//     void SpawnWall(Node node) {
//         var block = Instantiate(_blockPrefab, node.Pos, Quaternion.identity);
//         block.Init(GetBlockTypeByValue(TileColor.Wall));
//         block.SetBlock(node);
//     }

//     void Shift(Vector2 dir) {
//         ChangeState(GameState.Moving);

//         bool moved = false;

//         var orderedBlocks = _blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
//         if (dir == Vector2.right || dir == Vector2.up) orderedBlocks.Reverse();

//         foreach (var block in orderedBlocks) {
//             var next = block.Node;
//             do {
//                 block.SetBlock(next);

//                 var possibleNode = GetNodeAtPosition(next.Pos + dir);
//                 if (possibleNode != null) {
//                     // We know a node is present
//                     // If it's possible to merge, set merge
//                     if (possibleNode.OccupiedBlock != null && possibleNode.OccupiedBlock.CanMerge(block.Color)) {
//                         block.MergeBlock(possibleNode.OccupiedBlock);
//                     }
//                     // Otherwise, can we move to this spot?
//                     else if (possibleNode.OccupiedBlock == null) {
//                         next = possibleNode;
//                         moved = true;
//                     }

//                     // None hit? End do while loop
//                 }
                

//             } while (next != block.Node);
//         }


//         var sequence = DOTween.Sequence();

//         foreach (var block in orderedBlocks) {
//             var movePoint = block.MergingBlock != null ? block.MergingBlock.Node.Pos : block.Node.Pos;

//             sequence.Insert(0, block.transform.DOMove(movePoint, _travelTime).SetEase(Ease.InQuad));
//         }

//         sequence.OnComplete(() => {
//             var mergeBlocks = orderedBlocks.Where(b => b.MergingBlock != null).ToList();
//             foreach (var block in mergeBlocks) {
//                 MergeBlocks(block.MergingBlock,block);
//             }
//             if(mergeBlocks.Any()) _source.PlayOneShot(_matchClips[random.Next(0, _matchClips.Length)], 0.2f);
            
//             int nb = (from x in _blocks select x.Tile.Color).Distinct().Count();

//             if(_blocks.Count == nb) {
//                 ChangeState(GameState.Win);
//             } else if (_round >= _solution.Length) {
//                 ChangeState(GameState.Lose);
//             } else {
//                 ChangeState(GameState.WaitingInput);
//             }
            
//         });

//         if (moved) {
//             _round++;
//             _source.PlayOneShot(_moveClips[random.Next(0,_moveClips.Length)],0.2f);
//         } 
//     }

//     void MergeBlocks(Block baseBlock, Block mergingBlock) {
//         var newValue = baseBlock.Color;

//         Instantiate(_mergeEffectPrefab, baseBlock.Pos, Quaternion.identity);
//         Instantiate(_floatingTextPrefab, baseBlock.Pos, Quaternion.identity).Init(newValue);

//         SpawnBlock(baseBlock.Node, newValue);

//         RemoveBlock(baseBlock);
//         RemoveBlock(mergingBlock);
//     }

//     void RemoveBlock(Block block) {
//         _blocks.Remove(block);
//         Destroy(block.gameObject);
//     }

//     Node GetNodeAtPosition(Vector2 pos) {
//         return _nodes.FirstOrDefault(n => n.Pos == pos);
//     }
   
// }

// // [Serializable]
// // public struct BlockType {
// //     public TileColor Color;
// //     public SpriteRenderer Sprite;
// // }

// // public enum GameState {
// //     GenerateLevel,
// //     MainMenu,
// //     WaitingInput,
// //     Moving,
// //     Win,
// //     Lose
// // }