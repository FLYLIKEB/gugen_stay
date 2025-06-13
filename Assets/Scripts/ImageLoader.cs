using UnityEngine;
using UnityEngine.UI;
using UGS; // Unity Google Sheets 네임스페이스

public class ImageLoader : MonoBehaviour
{
    public Image targetImage; // UI Image 컴포넌트

    public void LoadAndDisplayImage(string characterName)
    {
        // NPCData에서 캐릭터 이름에 해당하는 데이터 찾기 (key 또는 displayName으로 검색)
        var npcData = DefaultTable.NPCData.GetList().Find(npc => 
            npc.key == characterName || npc.displayName == characterName);
        Debug.Log($"npcData: {npcData}");

        if (npcData != null)
        {
            // npc_image 필드에서 이미지 키 가져오기 (key 사용)
            string imageName = npcData.key;

            // Resources 폴더에서 Sprite 로드
            Sprite loadedSprite = Resources.Load<Sprite>($"Images/{imageName}");
            
            if (loadedSprite != null)
            {
                targetImage.sprite = loadedSprite; // UI Image에 스프라이트 설정
                Debug.Log($"Successfully loaded image for character '{characterName}' using key '{imageName}'");
            }
            else
            {
                Debug.LogError($"Image with name '{imageName}' not found in Resources/Images.");
            }
        }
        else
        {
            Debug.LogError($"Character with name '{characterName}' not found in NPCData.");
        }
    }
}
