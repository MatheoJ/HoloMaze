using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Sky & Weather management")]
    [SerializeField] private float sunAngle = 90.0f;


    // Scripts references
    private DynamicSkyFogManager skyFogManager;
    private MiniSunController miniSunController;

    private float scale = 0.1f; // Scale of the MiniMap  // M : it would be get the scale from the GM in the other scripts (instead of setting it manually)
    private bool itIsNight;




    // =============== MAKING GAMEMANAGER A SINGLETON ===============
    // Making sure there is only one GameManager

    private static GameManager _instance; // Stores the reference to the single instance of GameManager
                                          // used for checking if the instance already exists

    public static GameManager Instance // Getter : public static property that allows other scripts to access the singleton instance
    {                               // encapsulates access to the singleton, making it the only way for external scripts to get the instance
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
                if (_instance == null)
                {
                    Debug.LogError("GameManager not found in the scene");
                }
            }
            return _instance;
        }
    }




    private void Awake()
    {
        // ==== Ensuring there is only one instance of GameManager ====
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else if (_instance != this) // If another instance exists, destroy this one
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        // Instantiating the scripts' references
        skyFogManager = FindFirstObjectByType<DynamicSkyFogManager>();
        if (skyFogManager == null) throw new Exception("The script DynamicskyFogManager has not been found.");

        miniSunController = FindFirstObjectByType<MiniSunController>();
        if (miniSunController == null) throw new Exception("The script MiniSunController has not been found.");

    }






    void Update()
    {

    }





    public void ChangeSunAngle(float newAngle)
    {
        if (newAngle < -180 || 180 < newAngle) Debug.LogError("The function ChangeSunAngle() only accepts a value between -180 and 180. Here, newAngle = " + newAngle);
        sunAngle = newAngle;
        Debug.Log("Gamemanager : avant updateskyfog");
        skyFogManager.UpdateSkyFog(sunAngle);
    }



    //=============== GETTERS & SETTERS ===============
    public float GetSunAngle()
    {
        return sunAngle;
    }

    private void SetSunAngle(float newAngle)
    {
        sunAngle = newAngle;
    }


    public float GetItIsNight()
    {
        return itIsNight;
    }

    public void SetItIsNight(bool isNight)
    {
        itIsNight = isNight;
    }

}
