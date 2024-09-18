using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour {
    public Tile Tile;
    public Node Node;
    public Block MergingBlock;
    public bool Merging;
    public Vector2 Pos => transform.position;
    public TileColor Color => Tile.Color;
    [SerializeField] private TextMeshPro _text;
    [SerializeField] public GameObject Visual;
    public void Init(Tile type) {
        Tile = type;
        // _text.text = type.Color.ToString();
        _text.text = "";
        Visual.GetComponent<SpriteRenderer>().sprite = type.Renderer.sprite;
        Visual.GetComponent<SpriteRenderer>().transform.localScale = Vector3.one * 0.2f;
    }

    public void SetBlock(Node node) {
        if (Node != null) Node.OccupiedBlock = null;
        Node = node;
        Node.OccupiedBlock = this;
    }

    public void MergeBlock(Block blockToMergeWith) {
        // Set the block we are merging with
        MergingBlock = blockToMergeWith;

        // Set current node as unoccupied to allow blocks to use it
        Node.OccupiedBlock = null;

        // Set the base block as merging, so it does not get used twice.
        // blockToMergeWith.Merging = true;

        // Pour pourvoir merge plusieurs blocs en même temps
        blockToMergeWith.Merging = false;
    }

    public Block Clone() {
        var block = Instantiate(this);
        block.Init(Tile);
        return block;
    }

    public bool CanMerge(TileColor value) => value == Tile.Color && !Merging && MergingBlock == null;
}
