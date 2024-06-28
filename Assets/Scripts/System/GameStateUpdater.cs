using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateUpdater : MonoBehaviour
{
    private GameStateController _controller = default;

    public void SetGameStateController(GameStateController controller)
    {
        _controller = controller;
    }

    private void Update()
    {
        _controller.OnUpdate(Time.deltaTime);
    }
}
