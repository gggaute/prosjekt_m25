using UnityEngine;
using UnityEngine.SceneManagement;

public class ARSceneManager : MonoBehaviour
{
    public void exitAR() { 
        SceneManager.LoadScene("Hub");
    }
}
