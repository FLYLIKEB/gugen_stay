using GoogleSheet.Protocol.v2.Req;
using System.Collections;
using System.Collections.Generic;
using UGS;
using UnityEngine;

public class ReadMeScene : MonoBehaviour
{
    public GameObject wait;
    public GameObject step3_hide_btn;
    public GameObject step3_succesfulltText;

    public GameObject clickMe;
    
    public void OnClickCopy()
    {
        wait.SetActive(true);
        UnityPlayerWebRequest.Instance.CopyExample(new CopyExampleReqModel(), null, (x) => {
            wait.SetActive(false);
            Application.OpenURL($"https://drive.google.com/drive/u/0/folders/{x}");
            UGSettingObjectWrapper.GoogleFolderID = x.createdFolderId;
            step3_hide_btn?.SetActive(true);
            step3_succesfulltText.SetActive(true);
            clickMe.SetActive(false);
        });
    }
}
