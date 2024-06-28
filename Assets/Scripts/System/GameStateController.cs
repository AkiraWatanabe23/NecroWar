using StateMachine;
using System.Collections;
using UnityEngine;

public class GameStateController : MonoBehaviour
{
    private StateMachine<GameStateController> _stateMachine = default;
    private GameStateUpdater _gameUpdater = default;

    private IEnumerator Start()
    {
        yield return Initialize();
        OnInitialized();
    }

    /// <summary> 初期設定（データの読み込み等） </summary>
    private IEnumerator Initialize()
    {
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
        yield return null;
    }

    public void OnUpdate(float deltaTime)
    {
        _stateMachine.OnUpdate(deltaTime);
    }
}
