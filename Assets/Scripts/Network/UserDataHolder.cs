using Data;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class UserDataHolder : MonoBehaviour
{
    [SubclassSelector]
    [SerializeReference]
    private IDataBase[] _userDatas = default;

    [Header("For EditorDebug")]
    [Tooltip("接続開始時に保存しているIDがあった場合破棄するか")]
    [SerializeField]
    private bool _isResetDataInitialized = false;
    [Tooltip("実行終了時にIDを保存するか")]
    [SerializeField]
    private bool _isSaveIDOnClosed = false;

    [Header("For BuildDebug")]
    [Tooltip("接続開始時に保存しているIDがあった場合破棄するか")]
    [SerializeField]
    private Toggle _resetDataInitializedToggle = default;
    [Tooltip("実行終了時にIDを保存するか")]
    [SerializeField]
    private Toggle _saveIDClosedToggle = default;

    protected bool IsResetDataOnInitialized
    {
        get
        {
            if (_resetDataInitializedToggle != null) { return _resetDataInitializedToggle.isOn; }
            else { return _isResetDataInitialized; }
        }
    }
    protected bool IsSaveData
    {
        get
        {
            if (_saveIDClosedToggle != null) { return _saveIDClosedToggle.isOn; }
            else { return _isSaveIDOnClosed; }
        }
    }

    public IDataBase[] UserDatas => _userDatas;
    public string ID
    {
        get
        {
            if (IsHoldID()) { return PlayerPrefs.GetString("UserID"); }
            else { return ""; }
        }
    }

    public void Initalize()
    {
        if (IsResetDataOnInitialized) { PlayerPrefs.DeleteAll(); }
    }

    #region Data Update
    public void OnUpdateDataInfo(AbstractData data)
    {
        for (int i = 0; i < _userDatas.Length; i++)
        {
            if (_userDatas[i].GetType() != data.GetType()) { continue; }
            _userDatas[i] = data;
        }
    }

    public void OnUpdateID(string id)
    {
        foreach (var userData in UserDatas)
        {
            var dataElement = (AbstractData)userData;
            dataElement.UserID = id;
        }
        PlayerPrefs.SetString("UserID", id);
    }

    /// <summary> Neme という変数があるクラスをすべて検索し、更新をかける </summary>
    /// <param name="name">  </param>
    public void OnUpdateName(string name)
    {
        foreach (var userData in UserDatas)
        {
            var dataElement = (AbstractData)userData;
            var type = dataElement.GetType();

            var nameField = type.GetField("Name", BindingFlags.Public | BindingFlags.Instance);
            nameField?.SetValue(userData, name);
        }
    }

    public void OnUpdateScore(int score)
    {
        foreach (var userData in UserDatas)
        {
            var dataElement = (AbstractData)userData;
            var type = dataElement.GetType();
            Debug.Log(type.ToString());

            var scoreField = type.GetField("Score", BindingFlags.Public | BindingFlags.Instance);
            scoreField?.SetValue(userData, score);
        }
    }
    #endregion

    /// <summary> IDのデータを保持しているか </summary>
    public bool IsHoldID()
    {
        var isHoldID = PlayerPrefs.HasKey("UserID");
        if (isHoldID) { OnUpdateID(PlayerPrefs.GetString("UserID")); }

        return isHoldID;
    }

    /// <summary> データの削除を行うか </summary>
    public bool IsDataSaveOnClosed()
    {
        if (PlayerPrefs.HasKey("UserID") && IsSaveData) { PlayerPrefs.Save(); }

        return IsSaveData;
    }

    public void DeleteID() => PlayerPrefs.DeleteKey("UserID");
}
