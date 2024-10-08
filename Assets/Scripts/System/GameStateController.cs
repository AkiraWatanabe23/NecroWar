using StateMachine;
using System.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum DevelopEnv
{
    Local,
    Build,
    OnLine,
}

public class GameStateController : SingletonMonoBehaviour<GameStateController>
{
    private StateMachine<GameStateController> _stateMachine = default;
    private GameStateUpdater _gameUpdater = default;

    protected override bool DontDestroyOnLoad => false;

    private IEnumerator Start()
    {
        yield return Initialize();
        OnInitialized();
    }

    /// <summary> 初期設定（データの読み込み等） </summary>
    private IEnumerator Initialize()
    {
        Debug.Log("Initialize Start");

        if (!TryGetComponent(out _gameUpdater)) { _gameUpdater = gameObject.AddComponent<GameStateUpdater>(); }
        _gameUpdater.SetGameStateController(this);
        _gameUpdater.enabled = false;

        yield return StateMachineInitialize();
    }

    /// <summary> 初期設定終了時に呼び出す </summary>
    private void OnInitialized()
    {
        _gameUpdater.enabled = true;
    }

    private IEnumerator StateMachineInitialize()
    {
        //_stateMachine = new();
        //各ステートのインスタンス定義
        _stateMachine.AddStateData(StateType.GameStart, new GameStartState());
        _stateMachine.AddStateData(StateType.GameFinish, new GameFinishState());

        //todo : 遷移情報の設定
        _stateMachine.RegisterTransition(
            StateType.GameStart,
            new StateTransition(StateType.GameFinish, TransitionTrigger.InputEscape, () => Input.GetKeyDown(KeyCode.Escape)));

        yield return null;
    }

    public void OnUpdate(float deltaTime)
    {
        _stateMachine?.OnUpdate(deltaTime);
    }
}
