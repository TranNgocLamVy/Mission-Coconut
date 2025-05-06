using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Animator animator; 

    public void PlayMultiplayer()
    {
        StartCoroutine(LoadScene("Multiplayer"));
    }

    public void ExitGame()
    {
        Debug.Log("ExitGame() called");
        Application.Quit();
    }

    IEnumerator LoadScene(string sceneName)
    {
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }
}
