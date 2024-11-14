using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[ExecuteAlways] // M : to remove after testing. Allows to test without running
public class DynamicSkyboxController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Material proceduralSkybox;
    [SerializeField] private Light sunDirectionalLight;
    [SerializeField] private Light moonDirectionalLight;
    [SerializeField] private GameObject moon;
    [SerializeField] private Transform moonOrbitCenterPoint;
    [SerializeField] private ParticleSystem starsParticleSystem;
    
    [Header("Skybox Settings")]
    [SerializeField] private Color nightSkyColor = new Color(205f / 255f, 111f / 255f, 0f / 255f, 1f); // Converted to 0-1 range
    [SerializeField] private Color sunriseSkyColor = new Color(64f / 255f, 135f / 255f, 51f / 255f, 1f);
    [SerializeField] private Color daySkyColor = Color.black;
    [SerializeField] private Color sunsetSkyColor = new Color(74f / 255f, 82f / 255f, 75f / 255f, 1f);
    [SerializeField] private float nightExposure = 0.4f;
    [SerializeField] private float sunriseExposure = 0.9f;
    [SerializeField] private float dayExposure = 0.9f;
    [SerializeField] private float sunsetExposure = 0.7f;
    [SerializeField] private float daySunSize = 0.064f;

    [Header("Sun Directional Light Settings")]
    [SerializeField] private Color nightLightColor = new Color(78f / 255f, 78f / 255f, 112f / 255f, 1f); 
    [SerializeField] private Color sunriseLightColor = new Color(255f / 255f, 135f / 255f, 102f / 255f, 1f);
    [SerializeField] private Color dayLightColor = Color.white;
    [SerializeField] private Color sunsetLightColor = new Color(255f / 255f, 128f / 255f, 76f / 255f, 1f);
    [SerializeField] private float nightLightIntensity = 0.1f;
    [SerializeField] private float sunriseLightIntensity = 0.6f;
    [SerializeField] private float dayLightIntensity = 1.0f;
    [SerializeField] private float sunsetLightIntensity = 0.4f;

    [Header("Moon Directional Light Settings")]
    [SerializeField] private Color moonLightColor = new Color(158f / 255f, 177f / 255f, 234f / 255f, 1f);
    [SerializeField] private float moonLightIntensity = 0.8f;

    [Header("Moon Orbit Settings")]
    [SerializeField] private float moonOrbitVRadius = 400f; // Vertical radius
    [SerializeField] private float moonOrbitHRadius = 600f; // Horizontal radius


    [Header("Thresholds")]
    [SerializeField] private float nightToSunriseThresh = 10f;
    [SerializeField] private float sunriseToDayThresh = 50f;
    [SerializeField] private float dayToSunsetThresh = 130f;
    [SerializeField] private float sunsetToNightThresh = 170f;
    [SerializeField] private float blendRange = 15f;

    // Skybox and directional light parameters
    private Color skyColor;
    private float exposure;
    private Color lightColor;
    private float lightIntensity;

    // Stars
    private bool shouldEnableStars;

    // Other
    private float semiBlendRange;
    private float sunAngle;
    private float moonAngle;
    private bool itIsNight;

    [Header("Tests")]
    [SerializeField] private float moonYRotation = 0;
    [SerializeField] private float moonZRotation = 0;






    private void Start()
    {
        // Making sure Directional Lights and Skybox are provided
        if (proceduralSkybox == null || sunDirectionalLight == null || moonDirectionalLight == null
            || moon == null || moonOrbitCenterPoint == null || starsParticleSystem == null)
            throw new System.Exception("A reference has not been provided to the DynamicSkyboxController script");

        // Initializing the skybox
        RenderSettings.skybox = proceduralSkybox;

        // Initializing the sunDirectionalLight and the moonDirectionalLight to midday position
        sunDirectionalLight.transform.rotation = Quaternion.Euler(90, 0, 0);
        moonDirectionalLight.transform.rotation = Quaternion.Euler(-90, 0, 0);
        itIsNight = true;

        // Initializing the moon position at -90°
        float z = moonOrbitHRadius * Mathf.Cos(-Mathf.PI / 2); // Angle in radians
        float y = moonOrbitVRadius * Mathf.Sin(-Mathf.PI / 2);
        moon.transform.position = moonOrbitCenterPoint.position + new Vector3(0, y, z);

        // Initializing skybox and sunDirectionallight parameters to DayTime
        skyColor = daySkyColor;
        exposure = dayExposure;
        lightColor = dayLightColor;
        lightIntensity = dayLightIntensity;

        // Initializing moonDirectionalLight parameters
        moonDirectionalLight.enabled = false;
        moonDirectionalLight.color = moonLightColor; // Will never change
        moonDirectionalLight.intensity = moonLightIntensity; // Will never change

        // Initializing other parameters
        semiBlendRange = blendRange / 2;
    }






    private void Update()
    {
        UpdateSky();
    }



    // Function making all the updates of the sky (skybox, sunlight; moonlight, moon position) according to the sunDirectionalLight orientation
    private void UpdateSky()
    {

        /////////////// ANGLES' CALCULATIONS ///////////////
        // Calculate the sun angle based on the sunDirectionalLight orientation (from -180° to 180°)
        Vector3 sunDirection = -sunDirectionalLight.transform.forward;
        Vector3 worldZ = Vector3.forward;
        Vector3 worldX = Vector3.right; ;
        sunAngle = Vector3.SignedAngle(-worldZ, sunDirection, worldX);

        // Calculate the moon angle based on the sun angle (they are opposites)
        moonAngle = sunAngle < 0 ? sunAngle + 180 : sunAngle - 180;

        // apply the moonAngle on the moonDirectionalLight
        moonDirectionalLight.transform.rotation = Quaternion.Euler(moonAngle, 0, 0);
        Debug.Log("::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::sunAngle = " + sunAngle);
        Debug.Log("::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::moonAngle = " + moonAngle);



        /////////////// UPDATING THE MOON POSITION ///////////////
        UpdateMoonPosition();



        /////////////// DAYTIMES' LOGIC ///////////////
        // Night
        if (sunAngle <= nightToSunriseThresh - semiBlendRange || sunsetToNightThresh + semiBlendRange < sunAngle)
        {
            Debug.Log("=  Night  =");
            StoreProceduralSettings(nightSkyColor, nightExposure, nightLightColor, nightLightIntensity);

            shouldEnableStars = true; // Stars enabled
        }

        // Night - SunRise Transition
        else if (nightToSunriseThresh - semiBlendRange < sunAngle && sunAngle <= nightToSunriseThresh + semiBlendRange)
        {
            Debug.Log("== Night to Sunrise ==");
            StoreProceduralTransitionSettings(nightToSunriseThresh - semiBlendRange, nightToSunriseThresh + semiBlendRange, sunAngle,
                            nightSkyColor, sunriseSkyColor, nightExposure, sunriseExposure, nightLightColor, sunriseLightColor,
                            nightLightIntensity, sunriseLightIntensity);

            shouldEnableStars = true; // Stars enabled     -> as the skybox gradually lightens, the stars seem to progressively disappear
        }

        // Sunrise
        else if (nightToSunriseThresh + semiBlendRange < sunAngle && sunAngle <= sunriseToDayThresh - semiBlendRange)
        {
            Debug.Log("=  Sunrise  =");
            StoreProceduralSettings(sunriseSkyColor, sunriseExposure, sunriseLightColor, sunriseLightIntensity);

            shouldEnableStars = false; // Stars disabled
        }

        // Sunrise - Day transition
        else if (sunriseToDayThresh - semiBlendRange < sunAngle && sunAngle <= sunriseToDayThresh + semiBlendRange)
        {
            Debug.Log("== Sunrise to Day ==");
            StoreProceduralTransitionSettings(sunriseToDayThresh - semiBlendRange, sunriseToDayThresh + semiBlendRange, sunAngle,
                            sunriseSkyColor, daySkyColor, sunriseExposure, dayExposure, sunriseLightColor, dayLightColor,
                            sunriseLightIntensity, dayLightIntensity);

            shouldEnableStars = false; // Stars disabled

        }

        // Day
        else if (sunriseToDayThresh + semiBlendRange < sunAngle && sunAngle <= dayToSunsetThresh - semiBlendRange)
        {
            Debug.Log("=  Day  =");
            StoreProceduralSettings(daySkyColor, dayExposure,dayLightColor, dayLightIntensity);

            shouldEnableStars = false; // Stars disabled

        }

        // Day - Sunset transition
        else if (dayToSunsetThresh - semiBlendRange < sunAngle && sunAngle <= dayToSunsetThresh + semiBlendRange)
        {
            Debug.Log("== Day to Sunset ==");
            StoreProceduralTransitionSettings(dayToSunsetThresh - semiBlendRange, dayToSunsetThresh + semiBlendRange, sunAngle,
                            daySkyColor, sunsetSkyColor, dayExposure, sunsetExposure, dayLightColor, sunsetLightColor,
                            dayLightIntensity, sunsetLightIntensity);

            shouldEnableStars = false; // Stars disabled
        }

        // Sunset
        else if (dayToSunsetThresh + semiBlendRange < sunAngle && sunAngle <= sunsetToNightThresh - semiBlendRange)
        {
            Debug.Log("=  Sunset  =");
            StoreProceduralSettings(sunsetSkyColor, sunsetExposure, sunsetLightColor, sunsetLightIntensity);

            shouldEnableStars = false; // Stars disabled
        }

        // Sunset - Night transition
        else if (sunsetToNightThresh - semiBlendRange < sunAngle && sunAngle <= sunsetToNightThresh + semiBlendRange)
        {
            Debug.Log("== Sunset to Night ==");

            StoreProceduralTransitionSettings(sunsetToNightThresh - semiBlendRange, sunsetToNightThresh + semiBlendRange, sunAngle,
                            sunsetSkyColor, nightSkyColor, sunsetExposure, nightExposure, sunsetLightColor, nightLightColor,
                            sunsetLightIntensity, nightLightIntensity);
            
            shouldEnableStars = true; // Stars enabled     -> as the skybox gradually darkens, the stars seem to progressively appear
        }



        /////////////// SKY'S UPDATES BASED ON THE DAYTIMES' LOGIC ///////////////
        // Update the skybox and the directional lights with the stored settings
        UpdateSkyboxAndLights(proceduralSkybox, skyColor, exposure, lightColor, lightIntensity);

        // Update the stars Particle System
        UpdateStarsActivation(shouldEnableStars);
    }





    private void StoreProceduralSettings(Color aSkyColor, float anExposure, Color aLightColor, float aLightIntensity)
    {
        skyColor = aSkyColor;
        exposure = anExposure;
        lightColor = aLightColor;
        lightIntensity = aLightIntensity;
    }


    private void StoreProceduralTransitionSettings(float startThreshold, float endThreshold, float sunAngle, Color startSkyColor,
                                 Color endSkyColor, float startExposure, float endExposure, Color startLightColor,
                                 Color endLightColor, float startLightIntensity, float endLightIntensity)
    {
        Debug.Log("In ApplyTransition : startThreshold = " + startThreshold + "    endThreshold = " + endThreshold);
        
        // Calculate blend factor
        float factor = Mathf.InverseLerp(startThreshold, endThreshold, sunAngle);

        // Interpolate skybox and light properties
        skyColor = Color.Lerp(startSkyColor, endSkyColor, factor);
        exposure = Mathf.Lerp(startExposure, endExposure, factor);
        lightColor = Color.Lerp(startLightColor, endLightColor, factor);
        lightIntensity = Mathf.Lerp(startLightIntensity, endLightIntensity, factor);

        Debug.Log($"Transition factor: {factor}, Sky Color: {skyColor}, Exposure: {exposure}");
    }




    private void UpdateSkyboxAndLights(Material skybox, Color skyColor, float exposure, Color lightColor, float lightIntensity)
    {
        ///// Apply settings to skybox material /////
        skybox.SetColor("_SkyTint", skyColor); 
        skybox.SetFloat("_Exposure", exposure);


        ///// Enable/disable & apply settings to directional lights /////
        // The sun and the moon directional lights are never enabled at the same time

        if ((sunAngle < 0 || 180 < sunAngle)) // Sun over the horizon - moon under the horizon (opposites)
        {
            if (!itIsNight) // If itIsNight variable is still set to false, it has to be updated
            {
                itIsNight = true;
                sunDirectionalLight.enabled = false; // Disabling sun
                moonDirectionalLight.enabled = true; // Enabling moon
                skybox.SetFloat("_SunSize", 0); // Hiding the default "moon"
            }
            // Moon settings don't change
            
        }
        else if (0 < sunAngle || sunAngle < 180) // Sun under the horizon - moon over the horizon (opposites)
        {
            if (itIsNight) // If itIsNight variable is still set to true, it has to be updated
            {
                itIsNight = false;
                sunDirectionalLight.enabled = true; // Enabling sun
                moonDirectionalLight.enabled = false; // Disabling moon
                skybox.SetFloat("_SunSize", daySunSize); // Setting the sun size
            }

            // Update sun settings
            sunDirectionalLight.color = lightColor;
            sunDirectionalLight.intensity = lightIntensity;
        }


        Debug.Log($"Updating Sun Light - Current Angle: {sunAngle}, Light Enabled: {sunDirectionalLight.enabled}");
        Debug.Log($"Updating Moon Light - Current Angle: {moonAngle}, Light Enabled: {moonDirectionalLight.enabled}");
    }





    private void UpdateStarsActivation(bool playSystem)
    {
        if (playSystem && !starsParticleSystem.isPlaying) starsParticleSystem.Play();
        else if (!playSystem && starsParticleSystem.isPlaying) starsParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }





    private void UpdateMoonPosition()
    {
        float theta = moonAngle * Mathf.Deg2Rad; // Convert the moonAngle (degrees) in radians

        // Calculate the new position on the ellipse ("-" found by experimenting)
        float z = moonOrbitHRadius * Mathf.Cos(-theta);
        float y = moonOrbitVRadius * Mathf.Sin(-theta);
        moon.transform.position = moonOrbitCenterPoint.position + new Vector3(0, -y, -z);



        // M : PROBLEM : IT FLIPS AT 90° OR IT ROTATES IN THE WRONG DIRECTION
        // Make the moon always face the center of the orbit
        Vector3 directionToCenter = moonOrbitCenterPoint.position - moon.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToCenter);
        targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, moonYRotation, targetRotation.eulerAngles.z); // with this no flip but it rotates in the wrong direction before -90° and after 90°
        moon.transform.rotation = targetRotation;
        Debug.Log("======================================moon rotations : " + moon.transform.rotation); 





    }
}


