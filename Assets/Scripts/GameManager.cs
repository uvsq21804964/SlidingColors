using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    static public GameManager Instance;
    private int _width;
    private int _height;
    public static int Level;
    public static string _solution;
    [SerializeField] private Node _nodePrefab;
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private SpriteRenderer _boardPrefab;
    [SerializeField] private List<Tile> _tileTypes;
    [SerializeField] private float _travelTime = 0.1f;
    [SerializeField] private GameObject _mergeEffectPrefab;
    [SerializeField] private FloatingText _floatingTextPrefab;
    [SerializeField] public static int _round;



    public static event Action OnGameStarted;
    public static event Action OnGameEnded;

    [SerializeField] private AudioClip[] _moveClips;
    [SerializeField] private AudioClip[] _matchClips;
    [SerializeField] private AudioSource _source;
    [SerializeField] private GameObject _muteImage;
    private bool _music = true;

    private List<Node> _nodes;
    private List<Block> _blocks;
    private List<List<Block>> _blockHistory = new List<List<Block>>();
    public GameState _state;
    private string _fileName;
    private readonly System.Random random = new System.Random();


    private Tile GetBlockTypeByValue(TileColor value) => _tileTypes.First(t => t.Color == value);

    void Awake()
    {
        // Application.targetFrameRate = 120;

        Instance = this;
    }

    void Start() {
       Level = PlayerPrefs.GetInt("Level", 1);
       PlayerPrefs.SetInt("Level", Level);
       PlayerPrefs.Save();
       _music = PlayerPrefs.GetInt("Music", 1) == 1;
       _muteImage.SetActive(!_music);
       _fileName = "Assets/Scripts/Levels/Level" + Level + ".txt";
       GetSizeGrid();
       GenerateGrid();
       GenerateBlocks();
       StartGame();
    }

    private void ChangeState(GameState newState) {
        _state = newState;

        switch (newState) {
            case GameState.WaitingInput:
                break;
            case GameState.Moving:
                break;
            case GameState.MainMenu:
                break;
            case GameState.Win:
                Win();
                break;
            case GameState.Lose:
                GameOver();
                break;
            case GameState.GenerateLevel:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    public void Win()
    {
        PlayerPrefs.SetInt("Help", 0);
        LevelManager.Instance.IncrementLevel();
        _state = GameState.Win;
        OnGameEnded?.Invoke();
    }

    public void GameOver()
    {
        _state = GameState.Lose;
        OnGameEnded?.Invoke();
    }

    // void Update() {
    //     if (_state != GameState.WaitingInput) return;
    //     if (Input.touchCount > 0) {
    //         Touch touch = Input.GetTouch(0); // Prend le premier touché (vous pouvez adapter en fonction de votre besoin)
    //         if (touch.phase == TouchPhase.Began) {
    //             Vector2 touchStartPosition = touch.position;
    //             Vector2 touchDelta = touch.deltaPosition;
    //             if (Mathf.Abs(touchDelta.x) > Mathf.Abs(touchDelta.y)) {
    //                 if (touchDelta.x > 0) {
    //                     Shift(Vector2.right);
    //                 } else {
    //                     Shift(Vector2.left);
    //                 }
    //             } else {
    //                 if (touchDelta.y > 0) {
    //                     Shift(Vector2.up);
    //                 } else {
    //                     Shift(Vector2.down);
    //                 }
    //             }
    //         }
    //     }
    // }

    void GetSizeGrid() {
        string data;
        
        List<string> lines = new List<string>();

        StreamReader reader = new StreamReader(_fileName);
        data = reader.ReadLine();
        string[] valeurs = data.Split(' ');
        char width = valeurs[1][0];
        char height = valeurs[0][0];

        _width =  (int)(width - '0');
        _height =  (int)(height - '0');
        
        reader?.Close();
    }

    void GenerateGrid() {
        _round = 0;
        _nodes = new List<Node>();
        _blocks = new List<Block>();
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                var node = Instantiate(_nodePrefab, new Vector2(x, y), Quaternion.identity);
                _nodes.Add(node);
            }
        }

        var center = new Vector2((float) _width /2 - 0.5f,(float) _height / 2 -0.5f);

        var board = Instantiate(_boardPrefab, center, Quaternion.identity);
        board.size = new Vector2(_width,_height);

        Camera.main.transform.position = new Vector3(center.x,center.y,-10);
        Camera.main.orthographicSize = Mathf.Max(_width,_height) * 1.25f;
    }

    void GenerateBlocks() {
        var freeNodes = _nodes.ToList();
        string data;
        
        List<string> lines = new();

        StreamReader reader = new(_fileName);
        data = reader.ReadLine();

        do {
            data = reader.ReadLine();
            lines.Add(data);
        } while (data != null);

        _solution = lines[lines.Count - 3];
        Debug.Log("Solution " + _solution);

        reader?.Close();

        foreach (var node in freeNodes) {
            string value = lines[(int) node.Pos.y].Split(' ')[(int) node.Pos.x];
            
            switch (value){
            case "Mur":
                SpawnWall(node);
                break;
            case "Green":
                SpawnBlock(node, TileColor.Green);
                break;
            case "Red":
                SpawnBlock(node, TileColor.Red);
                break;
            case "Yellow":
                SpawnBlock(node, TileColor.Yellow);
                break;
            case "Purple":
                SpawnBlock(node, TileColor.Purple);
                break;
            case "Orange":
                SpawnBlock(node, TileColor.Orange);
                break;
            case "Cyan":
                SpawnBlock(node, TileColor.Turquoise);
                break;
            case "Pink":
                SpawnBlock(node, TileColor.Pink);
                break;
            case "Blue":
                SpawnBlock(node, TileColor.Blue);
                break;

            }
        }
    }

    public void StartGame() {
        ChangeState(GameState.WaitingInput);
        // AddBlockHistory();
        OnGameStarted?.Invoke();
    }

    // private void AddBlockHistory()
    // {
    //     List<Block> _blocksNew = new List<Block>();
    //     foreach (var block in _blocks)
    //     {
    //         _blocksNew.Add(block.Clone());
    //     }
    //     _blockHistory.Add(_blocksNew.ToList());
    // }

    // public void Undo() {
    //     if (_blockHistory.Count > 1) {
    //         var lastRound = _blockHistory[_blockHistory.Count - 2];
    //         foreach (var block in _blocks) {
    //             RemoveBlock(block);
    //         }

    //         foreach (var block in lastRound) {
    //             SpawnBlock(block.Node, block.Color);
    //         }

    //         _blockHistory.RemoveAt(_blockHistory.Count - 1);
    //         _round--;
    //     }
    // }

    void SpawnBlock(Node node, TileColor value) {
        var block = Instantiate(_blockPrefab, node.Pos, Quaternion.identity);
        block.Init(GetBlockTypeByValue(value));
        block.SetBlock(node);
        _blocks.Add(block);
    }

    void SpawnWall(Node node) {
        var block = Instantiate(_blockPrefab, node.Pos, Quaternion.identity);
        block.Init(GetBlockTypeByValue(TileColor.Wall));
        block.SetBlock(node);
    }

    public void Shift(Vector2 dir) {
        if (_state != GameState.WaitingInput) return;
        ChangeState(GameState.Moving);

        bool moved = false;

        var orderedBlocks = _blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
        if (dir == Vector2.right || dir == Vector2.up) orderedBlocks.Reverse();

        foreach (var block in orderedBlocks) {
            var next = block.Node;
            do {
                block.SetBlock(next);

                var possibleNode = GetNodeAtPosition(next.Pos + dir);
                if (possibleNode != null) {
                    // We know a node is present
                    // If it's possible to merge, set merge
                    if (possibleNode.OccupiedBlock != null && possibleNode.OccupiedBlock.CanMerge(block.Color)) {
                        block.MergeBlock(possibleNode.OccupiedBlock);
                    }
                    // Otherwise, can we move to this spot?
                    else if (possibleNode.OccupiedBlock == null) {
                        next = possibleNode;
                        moved = true;
                    }

                    // None hit? End do while loop
                }
                

            } while (next != block.Node);
        }


        var sequence = DOTween.Sequence();

        foreach (var block in orderedBlocks) {
            var movePoint = block.MergingBlock != null ? block.MergingBlock.Node.Pos : block.Node.Pos;

            sequence.Insert(0, block.transform.DOMove(movePoint, _travelTime).SetEase(Ease.InQuad));
        }

        sequence.OnComplete(() => {
            var mergeBlocks = orderedBlocks.Where(b => b.MergingBlock != null).ToList();
            foreach (var block in mergeBlocks) {

                MergeBlocks(block.MergingBlock,block);
            }
            if(_music && mergeBlocks.Any()) _source.PlayOneShot(_matchClips[random.Next(0, _matchClips.Length)], 0.2f);
            
            int nb = (from x in _blocks select x.Tile.Color).Distinct().Count();

            if(_blocks.Count == nb) {
                Debug.Log("Win");
                ChangeState(GameState.Win);
            } else if (_round >= _solution.Length) {
                Debug.Log("Lose");
                ChangeState(GameState.Lose);
            } else {
                ChangeState(GameState.WaitingInput);
            }
            
        });

        if (moved) {
            // AddBlockHistory();
            _round++;
            if(_music) _source.PlayOneShot(_moveClips[random.Next(0,_moveClips.Length)],0.2f);
        } 
    }

    void MergeBlocks(Block baseBlock, Block mergingBlock) {
        var newValue = baseBlock.Color;

        Instantiate(_mergeEffectPrefab, baseBlock.Pos, Quaternion.identity);
        var sequence = DOTween.Sequence();

        sequence.Insert(0, baseBlock.transform.DORotate(new Vector3(0, 0, 360 * 2), 0.3f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear));
        sequence.Insert(0, baseBlock.transform.DOScale(1.5f, 0.15f).SetEase(Ease.OutQuad).OnComplete(() => {
            baseBlock.transform.DOScale(Vector3.one, 0.15f) // Retour à la taille (1, 1, 1)
            .SetEase(Ease.InQuad);
        }));
        
        Instantiate(_floatingTextPrefab, baseBlock.Pos, Quaternion.identity).Init(newValue);

        RemoveBlock(mergingBlock);
    }

    void RemoveBlock(Block block) {
        _blocks.Remove(block);
        Destroy(block.gameObject);
    }

    Node GetNodeAtPosition(Vector2 pos) {
        return _nodes.FirstOrDefault(n => n.Pos == pos);
    }
   
   public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ToggleMusic()
    {
        _music = !_music;
        PlayerPrefs.SetInt("Music", _music ? 1 : 0);
        PlayerPrefs.Save();
        _muteImage.SetActive(!_music);
    }

    
    public struct BlockType {
        public TileColor Color;
        public SpriteRenderer Sprite;
    }

    public enum GameState {
        GenerateLevel,
        MainMenu,
        WaitingInput,
        Moving,
        Win,
        Lose
    }
}   