using UnityEngine;

public class CallConstruct : MonoBehaviour
{
    [SerializeField] Transform[] buildings;
    [SerializeField] BuildManager buildManager;

    [SerializeField] ItemData[] ResourcesForCheat;
    int buildingLength;

    private void Start()
    {
        buildingLength = buildings.Length;
    }

    public void Cheat()
    {
        foreach (var resource in ResourcesForCheat)
        {
            InventoryManager.instance.AddResource(resource, 100);
        }
        Debug.Log("치트 사용");
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    int keyInt = key - KeyCode.F1; // F1부터 시작되게끔
                    if ((keyInt >= 0 && keyInt < 12) && keyInt < buildingLength) // F1~F12, 배열의 최대 크기를 넘지 않게
                    {
                        StartConstruct(keyInt);
                    }
                }
            }
        }

        // !!! 자원 치트
        if (Input.GetKeyDown(KeyCode.P))
            Cheat();
    }

    public void StartConstruct(int index)
    {
        // 건설하려는 건물 데이터 등록
        buildManager.SetBuilding(buildings[index]);
        // 건설 도중에 다른 키를 눌러도 이미 빌드매니저가 활성화된 상태라 추가로 프리팹을 생성하지 않음 >> 중복 입력 방지 테스트 완
        buildManager.gameObject.SetActive(true); 
    }
}
