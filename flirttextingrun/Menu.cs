using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Choose(bool girl)
    {
        PlayerPrefs.SetInt("forgirls", girl ? 1 : 0);
        
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
