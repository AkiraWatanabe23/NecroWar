using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    public class ConnectorView : MonoBehaviour
    {
        [SerializeField]
        private Text _userIDText = default;
        [SerializeField]
        private Text _accessResultText = default;
        [SerializeField]
        private Text _portText = default;
        [SerializeField]
        private Text _serverAddress = default;
        [SerializeField]
        private InputField _nameInput = default;
        [SerializeField]
        private InputField _scoreInput = default;

        [field: SerializeField]
        public Button ConnectionStartButton { get; private set; }
        [field: SerializeField]
        public Button NameApply { get; private set; }
        [field: SerializeField]
        public Button ScoreApply { get; private set; }
        [field: SerializeField]
        public Button GetNameButton { get; private set; }
        [field: SerializeField]
        public Button GetScoreButton { get; private set; }
        [field: SerializeField]
        public Button GetRankingButton { get; private set; }
        [field: SerializeField]
        public Button CloseButton { get; private set; }

        public string Name => _nameInput.text;
        public string Score => _scoreInput.text;

        public void OnUpdateIDText(string message) => _userIDText.text = message;
        public void OnUpdateAccessResultText(string message) => _accessResultText.text = message;
        public void OnUpdatePortText(string message) => _portText.text = message;
        public void OnUpdateServerAddressText(string message) => _serverAddress.text = message;
    }
}
