using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScript : MonoBehaviour
{
    public RawImage blackScreen;
    public Color blackScreenCol;
    // Start is called before the first frame update
    void Start()
    {
        blackScreen = transform.Find("BlackScreen").GetComponent<RawImage>();
        blackScreenCol = new Color(0, 0, 0, 1);
        StartCoroutine(blackScreenOff(0.01f));
    }

    private void Update()
    {
        blackScreen.color = blackScreenCol;
    }

    IEnumerator blackScreenOff(float speed)
    {
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForFixedUpdate();
            blackScreenCol.a -= speed;
        }
        blackScreenCol.a = 0;
    }

    IEnumerator GameStart(float speed)
    {
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForFixedUpdate();
            blackScreenCol.a += speed;
        }
        blackScreenCol.a = 1;
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void GameStartButton()
    {
        StartCoroutine(GameStart(0.01f));
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
