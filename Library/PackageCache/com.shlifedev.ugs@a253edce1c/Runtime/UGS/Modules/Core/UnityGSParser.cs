#if UNITY_2017_1_OR_NEWER || UNITY_BUILD  
using GoogleSheet;
using GoogleSheet.IO;
using GoogleSheet.IO.Generator;
using GoogleSheet.Protocol.v2.Res;
using System.Linq;
using UnityEngine;
using System;

namespace UGS.IO
{
    /*
        백업 플
     */
    public class UnityGSParser : IParser
    {
        public void ParseSheet(ReadSpreadSheetResult sheetJsonData, bool generateCs, bool generateJson, IFIleWriter writer)
        {
            try
            {
                Debug.Log("[UGS] 스프레드시트 파싱 시작: " + (sheetJsonData != null ? sheetJsonData.spreadSheetName : "null"));
                
                ReadSpreadSheetResult getTableResult = sheetJsonData;
                if (generateJson)
                {
                    Debug.Log("[UGS] JSON 생성 중...");
                    var result = GenerateData(getTableResult);
                    writer?.WriteData(getTableResult.spreadSheetName, result);
                }

                Debug.Log("[UGS] 시트 수: " + (getTableResult.jsonObject != null ? getTableResult.jsonObject.Count : 0));
                Debug.Log("[UGS] 시트 ID 수: " + (getTableResult.sheetIDList != null ? getTableResult.sheetIDList.Count : 0));
                
                int count = 0;
                foreach (var sheet in getTableResult.jsonObject)
                {
                    Debug.Log("[UGS] 처리 중인 시트: " + sheet.Key + ", Index: " + count);
                    
                    string[] sheetInfoTypes = null;
                    string[] sheetInfoNames = null;
                    bool[] isEnum = null;
                    ///generate json data 
                    if (generateCs)
                    {
                        Debug.Log("[UGS] 시트 " + sheet.Key + "의 필드 수: " + sheet.Value.Count());
                        
                        try
                        {
                            sheetInfoTypes = new string[sheet.Value.Count()];
                            sheetInfoNames = new string[sheet.Value.Count()];
                            isEnum = new bool[sheet.Value.Count()];
                            int i = 0;
                            foreach (var type in sheet.Value)
                            {
                                try
                                {
                                    Debug.Log("[UGS] 필드 처리 중: " + type.Key + ", Index: " + i);
                                    var id = type.Key;
                                    
                                    // 디버그: 필드 형식 확인
                                    Debug.Log("[UGS] 필드 ID: " + id);
                                    
                                    var split = id.Replace(" ", null).Split(':');
                                    
                                    // 디버그: 분할 결과 확인
                                    Debug.Log("[UGS] 분할 결과 길이: " + split.Length);
                                    for (int s = 0; s < split.Length; s++)
                                    {
                                        Debug.Log("[UGS] 분할[" + s + "]: " + split[s]);
                                    }
                                    
                                    // 분할 결과가 2개 미만인 경우 오류 처리
                                    if (split.Length < 2)
                                    {
                                        Debug.LogError("[UGS] 오류: 필드 형식이 잘못되었습니다. 형식은 '이름:타입'이어야 합니다. 필드: " + id);
                                        throw new FormatException("필드 형식이 잘못되었습니다: " + id);
                                    }
                                    
                                    sheetInfoTypes[i] = split[1];
                                    sheetInfoNames[i] = split[0];

                                    if (split[1].Contains("Enum<"))
                                    {
                                        isEnum[i] = true;
                                    }
                                    else
                                    {
                                        isEnum[i] = false;
                                    }

                                    i++;
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogError("[UGS] 필드 처리 중 오류 발생: " + ex.Message + "\n" + ex.StackTrace);
                                    Debug.LogError("[UGS] 문제가 발생한 필드: " + type.Key + ", Index: " + i);
                                    throw;
                                }
                            }
                            
                            Debug.Log("[UGS] SheetInfo 생성 중...");
                            SheetInfo info = new SheetInfo();
                            
                            Debug.Log("[UGS] count: " + count + ", sheetIDList.Count: " + getTableResult.sheetIDList.Count);
                            
                            // 인덱스 범위 검사
                            if (count >= getTableResult.sheetIDList.Count)
                            {
                                Debug.LogError("[UGS] 오류: 시트 ID 인덱스 범위를 벗어났습니다. count: " + count + ", 시트 ID 수: " + getTableResult.sheetIDList.Count);
                                throw new IndexOutOfRangeException("시트 ID 인덱스 범위를 벗어났습니다.");
                            }
                            
                            info.spreadSheetID = getTableResult.spreadSheetID;
                            info.sheetID = getTableResult.sheetIDList[count];
                            info.sheetFileName = getTableResult.spreadSheetName;
                            info.sheetName = sheet.Key;
                            info.sheetTypes = sheetInfoTypes;
                            info.sheetVariableNames = sheetInfoNames;
                            info.isEnumChecks = isEnum;

                            var result = GenerateCS(info);
                            writer?.WriteCS(info.sheetFileName + "." + info.sheetName, result);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("[UGS] 시트 처리 중 오류 발생: " + ex.Message + "\n" + ex.StackTrace);
                            Debug.LogError("[UGS] 문제가 발생한 시트: " + sheet.Key + ", Index: " + count);
                            throw;
                        }
                    }
                    count++;
                }
                
                Debug.Log("[UGS] 스프레드시트 파싱 완료");
            }
            catch (Exception ex)
            {
                Debug.LogError("[UGS] 전체 파싱 과정에서 오류 발생: " + ex.Message + "\n" + ex.StackTrace);
                throw;
            }
        }
        
        private string GenerateData(ReadSpreadSheetResult tableResult)
        {
            try
            {
                Debug.Log("[UGS] 데이터 생성 시작");
                DataGenerator dataGen = new DataGenerator(tableResult);
                var result = dataGen.Generate();
                Debug.Log("[UGS] 데이터 생성 완료");
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError("[UGS] 데이터 생성 중 오류 발생: " + ex.Message + "\n" + ex.StackTrace);
                throw;
            }
        }

        private string GenerateCS(SheetInfo info)
        {
            try
            {
                Debug.Log("[UGS] C# 코드 생성 시작: " + info.sheetName);
                CodeGeneratorUnityEngine sheetGenerator = new CodeGeneratorUnityEngine(info);
                var result = sheetGenerator.Generate();
                Debug.Log("[UGS] C# 코드 생성 완료: " + info.sheetName);
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError("[UGS] C# 코드 생성 중 오류 발생: " + ex.Message + "\n" + ex.StackTrace);
                throw;
            }
        }
    }
}


#endif