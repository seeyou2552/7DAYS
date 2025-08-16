using System;
using System.Collections.Generic;
using UnityEngine;

public interface IForm
{
    public Type Type { get; }
    public abstract void CreateForm();
}

public class BaseBuildingForm<T> : ScriptableObject, IForm where T : class
{
    // 건물 데이터들을 넣어둘 데이터 리스트
    [SerializeField] protected List<T> dataList = new List<T>();
    // 해당 타입의 ID : 데이터 1:1 매칭이 되게 만든 딕셔너리
    public Dictionary<int, T> DataDic { get; protected set; } = new Dictionary<int, T>();
 
    public Type Type { get; private set; }
    public virtual void CreateForm() => Type = GetType();

    // ID로부터 데이터를 찾아 반환
    public virtual T GetDataByID(int id)
    {
        if (DataDic.TryGetValue(id, out T value))
            return value;

        Debug.LogError($"ID {id}를 찾을 수 없습니다.");
        return null;
    }
}