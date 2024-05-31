using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    /// <summary> 使用されたプールを保存するためのDictionary </summary>
    private readonly Dictionary<int, List<GameObject>> _poolDict = new();

    /// <summary> 指定したオブジェクトを表示する（無かったら生成する） </summary>
    public GameObject SpawnObject(GameObject prefab)
    {
        //引数に指定されたプレハブのハッシュ値を取得
        int hash = prefab.GetHashCode();
        //新たに表示したいプレハブが既に登録されていたらまずそのList内から探す
        if (_poolDict.ContainsKey(hash))
        {
            //まだ非表示のオブジェクトがないか探す
            var targetPool = _poolDict[hash];
            foreach (var pool in targetPool)
            {
                if (!pool.activeSelf)
                {
                    pool.SetActive(true);
                    return pool;
                }
            }

            //全部表示済みだったら新規に生成し、そのオブジェクトを返す
            var go = Object.Instantiate(prefab);

            targetPool.Add(go);
            go.SetActive(true);

            return go;
        }

        //Dictionaryに登録されてなかった場合新規に作成し、Dictionaryに登録する
        var createObj = Object.Instantiate(prefab);
        var poolList = new List<GameObject>() { createObj };
        _poolDict.Add(hash, poolList);
        createObj.SetActive(true);

        return createObj;
    }

    /// <summary> 指定したオブジェクトを非表示にする </summary>
    public void RemoveObject(GameObject go) => go.SetActive(false);

    /// <summary> 指定されたオブジェクトのハッシュ値を取得する（プールに存在しなかったら登録する） </summary>
    public int GetHashCode(GameObject prefab)
    {
        int hash = prefab.GetHashCode();
        if (!_poolDict.ContainsKey(hash))
        {
            var poolList = new List<GameObject>() { prefab };
            _poolDict.Add(hash, poolList);
        }
        return hash;
    }
}