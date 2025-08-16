using System.Collections.Generic;
using UnityEngine;
public class GeneratorManager : Singleton<GeneratorManager>
{
    // 건물 배치 상태 여부
    bool isBuildingState = false;
    // 필드에 설치된 발전기들
    List<Generator> generators = new List<Generator>();

    private void Awake()
    {
        // 시작 시 필드에 이미 배치된 발전기들을 추가
        Generator[] generatorsExisting = FindObjectsOfType<Generator>(true);
        foreach (Generator generator in generatorsExisting)
        {
            generators.Add(generator);
        }
    }

    // 발전기 건설/파괴할 때, 발전기 리스트에 추가/제거
    public void BuildGenerator(Generator generator)
    {
        generators.Add(generator);
        // 건물 배치 상태라면 지어진 발전기의 영역 켜짐
        if(isBuildingState)
            generator.EnableGeneratorZone();
    }
    public void DestroyGenerator(Generator generator) => generators.Remove(generator);

    // 설치 시작/종료 시 호출, 발전기 전력 공급 영역 모두 표시/숨김
    public void StartConstruct()
    {
        isBuildingState = true;
        foreach (Generator generator in generators)
        {
            generator.EnableGeneratorZone();
        }
    }
    public void EndConstruct()
    {
        isBuildingState = false;
        foreach (Generator generator in generators)
        {
            generator.DisableGeneratorZone();
        }
    }
}
