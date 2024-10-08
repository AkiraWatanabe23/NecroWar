using System;
using System.Collections.Generic;

namespace StateMachine
{
    public enum StateType
    {
        None,
        GameStart,
        GameFinish
    }

    public enum TransitionTrigger
    {
        None,
        InputEscape
    }

    /// <summary> 各ステートで実行する関数 </summary>
    public abstract class State<T>
    {
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnUpdate(float deltaTime);
    }

    /// <summary> ステートの遷移データ </summary>
    public class StateTransition
    {
        /// <summary> 遷移先 </summary>
        public StateType To { get; private set; }
        /// <summary> 遷移条件 </summary>
        public TransitionTrigger Trigger { get; private set; }
        public Func<bool> TransitionTrigger { get; private set; }

        public StateTransition(StateType to, TransitionTrigger trigger, Func<bool> transitionTrigger)
        {
            To = to;
            Trigger = trigger;
            TransitionTrigger = transitionTrigger;
        }
    }

    public class StateMachine<T>
    {
        private StateType _currentStateType = StateType.None;
        private State<T> _currentState = default;
        /// <summary> State名とインスタンスのデータ群 </summary>
        private Dictionary<StateType, State<T>> _stateData = default;
        /// <summary> 各State同士の遷移情報 </summary>
        private Dictionary<StateType, List<StateTransition>> _transitionData = default;

        public StateMachine(StateType initialState)
        {
            _stateData = new();
            _transitionData = new();

            OnChangeState(initialState);
        }

        #region Register
        /// <summary> ステートの登録 </summary>
        /// <param name="state"> ステート名 </param>
        /// <param name="instance"> 対象ステートのインスタンス </param>
        public void AddStateData(StateType state, State<T> instance)
        {
            if (_stateData.ContainsKey(state)) { _stateData[state] = instance; }
            else { _stateData.Add(state, instance); }
        }

        /// <summary> 遷移データの登録 </summary>
        /// <param name="from"> 遷移元 </param>
        /// <param name="to"> 遷移先 </param>
        public void RegisterTransition(StateType from, params StateTransition[] to)
        {
            if (!_transitionData.ContainsKey(from)) { _transitionData.Add(from, new List<StateTransition>()); }

            foreach (var transition in to)
            {
                if (_transitionData[from].Find(transitionData => transitionData.To == transition.To) != null)
                {
                    //遷移情報の重複が見つかった場合は無視
                    continue;
                }
                _transitionData[from].Add(transition);
            }
        }
        #endregion

        #region Execute
        /// <summary> 現在のステートの実行部 </summary>
        public void OnUpdate(float deltaTime)
        {
            _currentState.OnUpdate(deltaTime);
            //遷移条件を満たすものがあれば遷移する
            foreach (var transition in _transitionData.Values)
            {
                foreach (var data in transition)
                {
                    if (data.TransitionTrigger()) { ExecuteTrigger(data.Trigger); }
                }
            }
        }

        /// <summary> 遷移条件の判定 </summary>
        /// <param name="trigger"> 遷移条件 </param>
        private void ExecuteTrigger(TransitionTrigger trigger)
        {
            var transitionData = _transitionData[_currentStateType];
            //現在のステートに登録している遷移データ内で、あてはまるものがあれば遷移する
            foreach (var transition in transitionData)
            {
                if (transition.Trigger == trigger)
                {
                    OnChangeState(transition.To);
                    break;
                }
            }
        }

        /// <summary> ステートの遷移 </summary>
        /// <param name="to"> 遷移先 </param>
        private void OnChangeState(StateType to)
        {
            _currentState?.OnExit();

            if (!GetState(to)) { return; }

            _currentStateType = to;
            _currentState = _stateData[to];
            _currentState.OnEnter();
        }

        /// <summary> 指定されたStateが登録されているか </summary>
        private bool GetState(StateType state) => _stateData.ContainsKey(state);
        #endregion
    }
}
