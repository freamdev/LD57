using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToPlay : MonoBehaviour
{
    public void StartGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }
}
