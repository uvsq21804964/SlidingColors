using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GG.Infrastructure.Utils.Swipe;

public class SwipeActions : MonoBehaviour
{
    [SerializeField] private SwipeListener _swipeListener;

    private void OnEnable() {
        _swipeListener.OnSwipe.AddListener(OnSwipe);
    }

    private void OnSwipe(string swipe) {
        switch(swipe) {
            case "Up":
                GameManager.Instance.Shift(Vector2.up);
                break;
            case "Down":
                GameManager.Instance.Shift(Vector2.down);
                break;
            case "Left":
                GameManager.Instance.Shift(Vector2.left);
                break;
            case "Right":
                GameManager.Instance.Shift(Vector2.right);
                break;
        }
    }

    private void OnDisable() {
        _swipeListener.OnSwipe.RemoveListener(OnSwipe);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
