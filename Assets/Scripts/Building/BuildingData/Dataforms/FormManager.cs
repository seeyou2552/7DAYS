using System.Collections.Generic;
using UnityEngine;
using System;

public class FormManager : Singleton<FormManager>
{
    [SerializeField] List<ScriptableObject> formList = new List<ScriptableObject>();

    private Dictionary<Type, IForm> formDic = new Dictionary<Type, IForm>();

    void Awake()
    {
        foreach (var formObj in formList)
        {
            if (formObj is IForm form)
            {
                form.CreateForm();
                formDic[form.Type] = form;
            }
        }
    }

    // 등록된 데이터 테이블을 가져오는 함수 
    public T GetForm<T>() where T : class
    {
        return formDic[typeof(T)] as T;
    }

#if UNITY_EDITOR
    public void AutoAssign()
    {
        // 기존의 등록 데이터를 없애고
        formList.Clear();

        // 경로로부터 필터(스크립터블 오브젝트)만 찾아 GUID 배열로 반환
        string[] guids =
            UnityEditor.AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets/Scripts/Building/BuildingData" });

        foreach (string guid in guids)
        {
            // guid로부터 해당 파일의 경로
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            // 경로를 따라 해당 타입 불러오기
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

            // IForm 인터페이스를 포함하고 있다면 추가
            if (asset is IForm)
            {
                formList.Add(asset);
            }
        }

        // 에디터에서 해당 스크립트가 포함된 오브젝트의 변경 사항 저장
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
