using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    static public GameController Instance;

    [Header("Components")]
    [SerializeField] PlayerControl _player;
    [SerializeField] MineMachine _mineMachine;
    [SerializeField] GridGenerator _gridGenerator;

    public PlayerControl Player { get { return _player; } }
    public MineMachine MineMachine { get { return _player.MineMachine; } }
    public GridGenerator GridGenerator { get { return _gridGenerator; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Debug.Log("<b>Start!</b>");
    }
}
