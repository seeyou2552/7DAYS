using UnityEngine;
using System.Collections.Generic;

public class BuildingBluePrint : MonoBehaviour
{
    // 건물 배치 여부
    bool isPlaceOver = false;
    public bool SetPlaceOver { set {  isPlaceOver = value; } }

    // 건물 청사진의 충돌 판정을 위해 리지드바디 필요
    Rigidbody rb;
    Collider col;
    // 건물 오브젝트의 매시랜더러들과 머티리얼 색상 (여럿이 조합되어 있는 경우가 있어 변경)
    MeshRenderer[] meshRenderers;
    Color[,] originColors;
    readonly float tranparency = 0.5f; // 건물 청사진의 투명도

    public List<Collider> colliders = new List<Collider>();

    // 레이어 명칭
    readonly string groundLayerName = "Ground", // 땅 
                    generatorZoneLayerName = "GeneratorZone"; // 발전기 작용 영역
    // 명칭으로부터 변환한 레이어 넘버
    int groundLayer, generatorZoneLayer;

    // 해당 건물이 발전기를 요구하는지 여부
    bool isNeedGenerator;

    // 건설 가능 여부
    public bool CanConstruct {  get; private set; }

    private void Awake()
    {
        groundLayer = LayerMask.NameToLayer(groundLayerName);
        generatorZoneLayer = LayerMask.NameToLayer(generatorZoneLayerName);
    }

    private void OnEnable()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true; // 중력 영향을 받지 않도록

        // 반투명하게 표시
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        MatColorChange(tranparency);

        // 콜라이더를 트리거로 설정 >> 트리거 이벤트로 겹친 물체 판정
        col = GetComponent<Collider>();
        if (col)
            col.isTrigger = true;

        // 해당 건물이 발전기를 필요로 하는 것이라면, 이를 기억
        if (TryGetComponent(out IBuildingRequireEnegy buildingRequireEnegy))
        {
            isNeedGenerator = true;
            ChangeNotConstructable(); // 발전기 존이 필요한 건물은 초기에 건설 불가로
        }
        else
        {
            isNeedGenerator = false;
            ChangeToConstructable(); // 발전기 존이 필요하지 않은 건물은 초기에 건설 가능하게
        }
    }

    // 건물 설치 완료 때 호출하여 청사진 기능 해제 및 제거
    public void BuildComplete()
    {
        // 반투명 해제
        MatColorChange(1);
        // 트리거를 다시 콜라이더로 원복하여 충돌 판정 생기게
        if (col)
            col.isTrigger = false;

        // 설치가 끝났을 때 불필요한 컴포넌트들 제거
        Destroy(rb);
        Destroy(this);
    }

    // 겹치는지 여부 판정 및 색상 변화
    private void OnTriggerEnter(Collider other)
    {
        if (isPlaceOver)
            return;

        // 발전기 존과 땅이 아닌 것에 들어갔다면
        if (other.gameObject.layer != generatorZoneLayer && other.gameObject.layer != groundLayer)
        {
            // 건설 불가 판정
            ChangeNotConstructable();
            // 영역 내 콜라이더를 등록
            colliders.Add(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isPlaceOver)
            return;

        // 발전기를 필요로 하는 건물이고
        if (isNeedGenerator)
        {
            // 트리거 내 걸리는 다른 충돌체가 없고 발전기 영역 내라면
            if (colliders.Count.Equals(0) && other.gameObject.layer == generatorZoneLayer)
            {
                // 건설 가능
                ChangeToConstructable();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPlaceOver)
            return;

        // 발전기 존과 땅이 아닌 것이 나갔다면
        if (other.gameObject.layer != generatorZoneLayer && other.gameObject.layer != groundLayer)
        {
            // 트리거에서 나간 콜라이더는 리스트에서 제거
            colliders.Remove(other);
            // 발전기가 필요한 건물이 아니고, 트리거 내 콜라이더가 0개라면 설치 가능한 상태로
            if (!isNeedGenerator && colliders.Count.Equals(0))
            {
                ChangeToConstructable();
            }
        }

        // 발전기가 필요한 건물이 발전기 영역 내에서 나가면 설치 불가
        if(isNeedGenerator && other.gameObject.layer == generatorZoneLayer)
        {
            ChangeNotConstructable();
        }
    }

    // 머티리얼 색상, 투명도 변화
    void MatColorChange(float transparency)
    {
        if (meshRenderers != null)
        {
            // 모든 매시랜더러들의 머티리얼에 대해
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                for(int j=0; j< meshRenderers[i].materials.Length; j++)
                {
                    // 투명도를 바꾸고
                    Color colorTransparent = meshRenderers[i].materials[j].color;
                    colorTransparent.a = transparency;
                    meshRenderers[i].materials[j].color = colorTransparent;

                    // 색상 기억
                    if (originColors == null)
                        originColors = new Color[meshRenderers.Length, meshRenderers[i].materials.Length];
                    originColors[i, j] = colorTransparent;
                }
            }
        }
    }

    // 건설 불가 판정 및 표시
    void ChangeNotConstructable()
    {
        CanConstruct = false;
        if (meshRenderers != null)
        {
            // 모든 매시랜더러들의 머티리얼에 대해
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                for (int j = 0; j < meshRenderers[i].materials.Length; j++)
                {
                    // 건설 불가 색상으로
                    meshRenderers[i].materials[j].color *= Color.red;
                }
            }
        }
    }
    // 건설 가능 판정 및 표시
    void ChangeToConstructable()
    {
        CanConstruct = true;
        if (meshRenderers != null)
        {
            // 모든 매시랜더러들의 머티리얼에 대해
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                for (int j = 0; j < meshRenderers[i].materials.Length; j++)
                {
                    // 투명도를 바꾸고
                    Color colorTransparent = meshRenderers[i].materials[j].color;
                    colorTransparent.a = tranparency;
                    meshRenderers[i].materials[j].color = colorTransparent;
                    // 건설 가능 색상으로 원복
                    meshRenderers[i].materials[j].color = originColors[i, j];
                }
            }
        }
    }
}
