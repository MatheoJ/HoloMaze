using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class resetScene : MonoBehaviour
{

    public void ReloadCurrentScene()
    {
        // Obtenir le nom de la sc�ne active
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Recharger la sc�ne
        SceneManager.LoadScene(currentSceneName);
    }
}
