using System.Collections.Generic;

namespace StateMachine
{
    public enum StateType
    {
        None,
    }

    public enum TransitionTrigger
    {
        None,
    }

    public abstract class State<T>
    {
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnUpdate(float deltaTime);
    }

    public class StateTransition
    {
        public StateType To { get; private set; }
        public TransitionTrigger Trigger { get; private set; }
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

        public void AddStateData(StateType state, State<T> instance)
        {
            if (_stateData.ContainsKey(state)) { _stateData[state] = instance; }
            else { _stateData.Add(state, instance); }
        }

        public void RegisterTransition(StateType from, params StateTransition[] to)
        {
            if (!_transitionData.ContainsKey(from)) { _transitionData.Add(from, new List<StateTransition>()); }

            foreach (var transition in to)
            {
                if (_transitionData[from].Find(transitionData => transitionData.To == transition.To) != null)
                {
                    //遷移情報の重複が見つかった場合
                    continue;
                }
                _transitionData[from].Add(transition);
            }
        }

        public void ExecuteTrigger(TransitionTrigger trigger)
        {
            var transitionData = _transitionData[_currentStateType];
            foreach (var transition in transitionData)
            {
                if (transition.Trigger == trigger)
                {
                    OnChangeState(transition.To);
                    break;
                }
            }
        }

        public void OnChangeState(StateType to)
        {
            _currentState?.OnExit();

            if (!GetState(to)) { return; }

            _currentStateType = to;
            _currentState = _stateData[to];
            _currentState.OnEnter();
        }

        /// <summary> 指定されたStateが登録されているか </summary>
        private bool GetState(StateType state)
        {
            return _stateData.ContainsKey(state);
        }

        public void OnUpdate(float deltaTime)
        {
            _currentState.OnUpdate(deltaTime);
        }
    }
}
