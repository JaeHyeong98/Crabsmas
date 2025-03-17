using Main;
using UnityEditor;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public bool canControl;
    public GameObject playerPrefab;

    private void Awake()
    {
        GSC.main = this;
        canControl = false;
    }

    public void GameClear()
    {
        canControl = false;
        GSC.uiController.GameResult(true);
    }

    public void GameOver()
    {
        canControl = false;
        GSC.uiController.GameResult(false);
    }

    public void PlayerReset()
    {
        Destroy(GSC.playerController.player.gameObject);

        // ������ �ν��Ͻ� ���� (������ ȯ��)
        GameObject player = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;

        // ������ ������ ��ġ ����
        player.transform.position = new Vector3(0, 0, 0);

        GSC.playerController.player = player.transform.Find("Body").GetComponent<Body>();
        GSC.playerController.player.Init();
    }
}
