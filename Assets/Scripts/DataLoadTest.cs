using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UGS;

public class DataLoadTest : MonoBehaviour
{
    // void Awake()
    // {
    //     UnityGoogleSheet.LoadAllData(); // 모든 데이터 로드
    // }

    void ScriptDataListTest() {
        // ScriptDataList 가져오기
        var scriptDataList = DefaultTable.ScriptData.ScriptDataList;

        if (scriptDataList != null && scriptDataList.Count > 0)
        {
            Debug.Log("ScriptDataList contains the following items:");

            // ScriptDataList의 모든 데이터를 출력
            foreach (var data in scriptDataList)
            {
                Debug.Log($"Index: {data.index}, Name: {data.name}, Talk: {data.talk}");
            }
        }
        else
        {
            Debug.LogError("ScriptDataList is null or empty.");
        }
    }

    void ScriptDataMapTest() {
        var dataFromMap = DefaultTable.ScriptData.ScriptDataMap[0];
        Debug.Log($"dataFromMap: Index = {dataFromMap.index}, Name = {dataFromMap.name}, Talk = {dataFromMap.talk}");
    }
}
