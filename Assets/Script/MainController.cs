using Main;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public bool canControl;
    public GameObject playerPrefab;
    public Transform deathLegs;

    private void Awake()
    {
        GSC.main = this;
        canControl = false;

        if (PlayerPrefs.GetInt("fullScreen") == 1)
            Screen.fullScreen = true;
    }

    public void GameClear()
    {
        canControl = false;
        GSC.uiController.GameResult(true);
        PlayerReset();
    }

    public void GameOver()
    {
        canControl = false;
        GSC.uiController.GameResult(false);
        PlayerReset();
    }

    public void PlayerReset()
    {
        Destroy(GSC.playerController.player.transform.parent.gameObject);

        if(deathLegs.childCount > 0)
        {
            for (int i = deathLegs.childCount-1; i >= 0; i--)
            {
                Destroy(deathLegs.GetChild(i).gameObject);
            }
        }

        // ������ �ν��Ͻ� ���� (������ ȯ��)
        GameObject player = Instantiate(playerPrefab); 
        //PrefabUtility.UnpackPrefabInstance(player, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        player.transform.SetParent(transform);
        player.transform.SetSiblingIndex(5);
        // ������ ������ ��ġ ����
        player.transform.position = new Vector3(0, 0, 0);

        GSC.playerController.player = player.transform.Find("Body").GetComponent<Body>();
        GSC.cameraController.SetTarget(GSC.playerController.player.transform.Find("CamTarget"));
        GSC.playerController.player.Init();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // ���ø����̼� ����
#endif
    }
}
