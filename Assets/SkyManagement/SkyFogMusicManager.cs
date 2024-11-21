using UnityEngine;
using System;
using UnityEngine.Audio;
//using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

//[ExecuteAlways] // M : to remove after testing. Allows to test without running
public class SkyFogMusicManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Material proceduralSkybox;
    [SerializeField] private Light sunDirectionalLight;
    [SerializeField] private Light moonDirectionalLight;
    [SerializeField] private GameObject moon;
    [SerializeField] private Transform moonOrbitCenterPoint;
    [SerializeField] private ParticleSystem starsParticleSystem;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Skybox Settings")]
    [SerializeField] private Color nightSkyColor = new Color(205f / 255f, 111f / 255f, 0f / 255f, 1f); // Converted to 0-1 range
    [SerializeField] private Color sunriseSkyColor = new Color(64f / 255f, 135f / 255f, 51f / 255f, 1f);
    [SerializeField] private Color daySkyColor = Color.black;
    [SerializeField] private Color sunsetSkyColor = new Color(74f / 255f, 82f / 255f, 75f / 255f, 1f);
    [SerializeField] private float nightExposure = 0f;
    [SerializeField] private float sunriseExposure = 0.9f;
    [SerializeField] private float dayExposure = 0.9f;
    [SerializeField] private float sunsetExposure = 0.7f;


    [Header("Fog Settings")]
    [SerializeField] private Color nightFogColor = new Color(0f, 0f, 0f, 1f);
    [SerializeField] private Color sunriseFogColor = new Color(255f / 255f, 135f / 255f, 102f / 255f, 1f);
    [SerializeField] private Color dayFogColor = Color.white;
    [SerializeField] private Color sunsetFogColor = new Color(255f / 255f, 128f / 255f, 76f / 255f, 1f);
    [SerializeField] private float nightFogDensity = 0.09f;
    [SerializeField] private float sunriseFogDensity = 0.04f;
    [SerializeField] private float dayFogDensity = 0.025f;
    [SerializeField] private float sunsetFogDensity = 0.04f;

    [Header("Sun Directional Light Settings")]
    [SerializeField] private Color nightLightColor = new Color(78f / 255f, 78f / 255f, 112f / 255f, 1f);
    [SerializeField] private Color sunriseLightColor = new Color(255f / 255f, 135f / 255f, 102f / 255f, 1f);
    [SerializeField] private Color dayLightColor = Color.white;
    [SerializeField] private Color sunsetLightColor = new Color(255f / 255f, 128f / 255f, 76f / 255f, 1f);
    private float nightLightIntensity = 0f;
    [SerializeField] private float sunriseLightIntensity = 0.6f;
    [SerializeField] private float dayLightIntensity = 1.0f;
    [SerializeField] private float sunsetLightIntensity = 0.4f;
    [SerializeField] private float daySunSize = 0.4f;

    [Header("Moon Directional Light Settings")]
    [SerializeField] private Color moonLightColor = new Color(158f / 255f, 177f / 255f, 234f / 255f, 1f);
    [SerializeField] private float moonLightIntensity = 0.8f;

    [Header("Moon Orbit Settings")]
    [SerializeField] private float moonOrbitVRadius = 1200f; // Vertical radius
    [SerializeField] private float moonOrbitHRadius = 2000f; // Horizontal radius
    [SerializeField] private float angleMaxSupplement = 65f; // used to make the moon appear higher in the sky when the sun is not very low

    [Header("Thresholds")]
    [SerializeField] private float nightToSunriseThresh = 10f;
    [SerializeField] private float sunriseToDayThresh = 50f;
    [SerializeField] private float dayToSunsetThresh = 130f;
    [SerializeField] private float sunsetToNightThresh = 170f;
    [SerializeField] private float blendRange = 15f;

    [Header("Lighting Environment parameters")]
    [SerializeField] private float dayIntensityMultiplier = 1f;
    [SerializeField] private float nightIntensityMultiplier = 0.3f;


    [Header("Background Music parameters")]
    [SerializeField] private float duringDay_dayMusicVolume = 0f;
    [SerializeField] private float duringDay_nightMusicVolume = -80f;
    [SerializeField] private float duringNight_dayMusicVolume = -60f;
    [SerializeField] private float duringNight_nightMusicVolume = 0f;

    // GameManager
    private GameManager gm;

    // Background Musics
    private float dayMusicVolume;
    private float nightMusicVolume;


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
    private float ambientLightIntensity;
    private Color fogColor;
    private float fogDensity;






    private void Start()
    {
        // Instantiating the GameManager
        gm = GameManager.Instance;
        if (gm == null) new Exception("GameManager not found in the script DynamicSkyFogManager.cs");

        // Making sure the references are provided
        if (proceduralSkybox == null || sunDirectionalLight == null || moonDirectionalLight == null
            || moon == null || moonOrbitCenterPoint == null || starsParticleSystem == null)
            throw new System.Exception("A reference has not been provided to the DynamicSkyboxController script");

        // Initializing the skybox
        RenderSettings.skybox = proceduralSkybox;

        // Initializing the sunDirectionalLight and the moonDirectionalLight rotations
        // M : not useful anymore
        //sunStartAngle = gm.GetSunAngle();
        //sunDirectionalLight.transform.rotation = Quaternion.Euler(sunStartAngle, 0, 0);
        //moonDirectionalLight.transform.rotation = Quaternion.Euler(-sunStartAngle, 0, 0);



        // Initializing the moon position at -90°
        // M : not useful anymore

        //float z = moonOrbitHRadius * Mathf.Cos(-Mathf.PI / 2); // Angle in radians
        //float y = moonOrbitVRadius * Mathf.Sin(-Mathf.PI / 2);
        //moon.transform.position = moonOrbitCenterPoint.position + new Vector3(0, y, z);

        //Initializing skybox and sunDirectionallight parameters to DayTime
        //M : not useful anymore
       //skyColor = daySkyColor;
       // exposure = dayExposure;
       // lightColor = dayLightColor;
       // lightIntensity = dayLightIntensity;

        // Initializing moonDirectionalLight parameters
        moonDirectionalLight.enabled = false;
        moonDirectionalLight.color = moonLightColor; // Will never change
        moonDirectionalLight.intensity = moonLightIntensity; // Will never change

        // Initializing other parameters
        semiBlendRange = blendRange / 2;
        itIsNight = true;

        // Setting the sky & fog
        float startSunAngle = gm.GetSunAngle();
        UpdateSkyFog(startSunAngle);
        UpdateMoon();
    }






    private void Update()
    {
       
    }








    //============= FUNCTIONS USED FOR THE SKY AND FOG MANAGEMENT =============




    // Function making all the updates of the sky (skybox, sunlight; moonlight, moon position) according to the sunAngle
    public void UpdateSkyFog(float newSunAngle)
    {

        //============= ANGLES' CALCULATIONS =============

        // NOT USEFUL ANYMORE
        // Calculate the sun angle based on the sunDirectionalLight orientation (from -180° to 180°)
        //Vector3 sunDirection = -sunDirectionalLight.transform.forward;
        //Vector3 worldZ = Vector3.forward;
        //Vector3 worldX = Vector3.right; ;
        //sunAngle = Vector3.SignedAngle(-worldZ, sunDirection, worldX); // Rotation around world x axis



        //============= UPDATING THE SUN ANGLE =============
        sunAngle = newSunAngle; // M : A REMETTRE
        sunDirectionalLight.transform.rotation = Quaternion.Euler(sunAngle, 0, 0); // Set the new Sun Directional Light angle


        //============= UPDATING THE MOON LIGHT, POSITION & ORIENTATION  =============
        UpdateMoon();

        //Debug.Log("::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::sunAngle = " + sunAngle);
        //Debug.Log("::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::moonAngle = " + moonAngle);


        //============= DAYTIMES' LOGIC =============
        // Night
        if (sunAngle <= nightToSunriseThresh - semiBlendRange || sunsetToNightThresh + semiBlendRange < sunAngle)
        {
            //Debug.Log("=  Night  =");
            StoreProceduralSettings(nightSkyColor, nightExposure, nightLightColor, nightLightIntensity);

            shouldEnableStars = true; // Stars enabled

            ambientLightIntensity = nightIntensityMultiplier; // very weak ambient light for nightTime (Lighting > Environment > IntensityMultiplier)

            // Fog
            fogColor = nightFogColor;
            fogDensity = nightFogDensity;

            // Music 
            dayMusicVolume = duringNight_dayMusicVolume;
            nightMusicVolume = duringNight_nightMusicVolume;
        }



        // Night - SunRise Transition
        else if (nightToSunriseThresh - semiBlendRange < sunAngle && sunAngle <= nightToSunriseThresh + semiBlendRange)
        {
            //Debug.Log("== Night to Sunrise ==");
            StoreProceduralTransitionSettings(nightToSunriseThresh - semiBlendRange, nightToSunriseThresh + semiBlendRange, sunAngle,
                            nightSkyColor, sunriseSkyColor, nightExposure, sunriseExposure, nightLightColor, sunriseLightColor,
                            nightLightIntensity, sunriseLightIntensity);

            shouldEnableStars = true; // Stars enabled 	-> as the skybox gradually lightens, the stars seem to progressively disappear

            // Smooth transition from very weak ambient light (nightTime) to normal ambient light (dayTime)
            ambientLightIntensity = CalculateFloatInterpolation(nightToSunriseThresh - semiBlendRange, nightToSunriseThresh + semiBlendRange,
                            sunAngle, nightIntensityMultiplier, dayIntensityMultiplier);

            // Fog smooth transition
            fogColor = CalculateColorInterpolation(nightToSunriseThresh - semiBlendRange, nightToSunriseThresh + semiBlendRange, sunAngle, nightFogColor, sunriseFogColor);
            fogDensity = CalculateFloatInterpolation(nightToSunriseThresh - semiBlendRange, nightToSunriseThresh + semiBlendRange, sunAngle, nightFogDensity, sunriseFogDensity);


            // Music smooth transition from nighttime to daytime
            dayMusicVolume = CalculateFloatInterpolation(nightToSunriseThresh - semiBlendRange, nightToSunriseThresh + semiBlendRange, sunAngle,
                            duringNight_dayMusicVolume, duringDay_dayMusicVolume);
            nightMusicVolume = CalculateFloatInterpolation(nightToSunriseThresh - semiBlendRange, nightToSunriseThresh + semiBlendRange, sunAngle,
                            duringNight_nightMusicVolume, duringDay_nightMusicVolume);
        }



        // Sunrise
        else if (nightToSunriseThresh + semiBlendRange < sunAngle && sunAngle <= sunriseToDayThresh - semiBlendRange)
        {
            //Debug.Log("=  Sunrise  =");
            StoreProceduralSettings(sunriseSkyColor, sunriseExposure, sunriseLightColor, sunriseLightIntensity);

            shouldEnableStars = false; // Stars disabled

            ambientLightIntensity = dayIntensityMultiplier; // normal ambient light for dayTime

            // Fog
            fogColor = sunriseFogColor;
            fogDensity = sunriseFogDensity;

            // Music 
            dayMusicVolume = duringDay_dayMusicVolume;
            nightMusicVolume = duringDay_nightMusicVolume;
        }



        // Sunrise - Day transition
        else if (sunriseToDayThresh - semiBlendRange < sunAngle && sunAngle <= sunriseToDayThresh + semiBlendRange)
        {
            //Debug.Log("== Sunrise to Day ==");
            StoreProceduralTransitionSettings(sunriseToDayThresh - semiBlendRange, sunriseToDayThresh + semiBlendRange, sunAngle,
                            sunriseSkyColor, daySkyColor, sunriseExposure, dayExposure, sunriseLightColor, dayLightColor,
                            sunriseLightIntensity, dayLightIntensity);

            shouldEnableStars = false; // Stars disabled

            ambientLightIntensity = dayIntensityMultiplier; // normal ambient light for dayTime

            // Fog smooth transition
            fogColor = CalculateColorInterpolation(sunriseToDayThresh - semiBlendRange, sunriseToDayThresh + semiBlendRange, sunAngle, sunriseFogColor, dayFogColor);
            fogDensity = CalculateFloatInterpolation(sunriseToDayThresh - semiBlendRange, sunriseToDayThresh + semiBlendRange, sunAngle, sunriseFogDensity, dayFogDensity);

            // Music 
            dayMusicVolume = duringDay_dayMusicVolume;
            nightMusicVolume = duringDay_nightMusicVolume;
        }



        // Day
        else if (sunriseToDayThresh + semiBlendRange < sunAngle && sunAngle <= dayToSunsetThresh - semiBlendRange)
        {
            //Debug.Log("=  Day  =");
            StoreProceduralSettings(daySkyColor, dayExposure, dayLightColor, dayLightIntensity);

            shouldEnableStars = false; // Stars disabled

            ambientLightIntensity = dayIntensityMultiplier; // normal ambient light for dayTime

            // Fog
            fogColor = dayFogColor;
            fogDensity = dayFogDensity;

            // Music 
            dayMusicVolume = duringDay_dayMusicVolume;
            nightMusicVolume = duringDay_nightMusicVolume;
        }



        // Day - Sunset transition
        else if (dayToSunsetThresh - semiBlendRange < sunAngle && sunAngle <= dayToSunsetThresh + semiBlendRange)
        {
            //Debug.Log("== Day to Sunset ==");
            StoreProceduralTransitionSettings(dayToSunsetThresh - semiBlendRange, dayToSunsetThresh + semiBlendRange, sunAngle,
                            daySkyColor, sunsetSkyColor, dayExposure, sunsetExposure, dayLightColor, sunsetLightColor,
                            dayLightIntensity, sunsetLightIntensity);

            shouldEnableStars = false; // Stars disabled

            ambientLightIntensity = dayIntensityMultiplier; // normal ambient light for dayTime

            // Fog smooth transition
            fogColor = CalculateColorInterpolation(dayToSunsetThresh - semiBlendRange, dayToSunsetThresh + semiBlendRange, sunAngle, dayFogColor, sunsetFogColor);
            fogDensity = CalculateFloatInterpolation(dayToSunsetThresh - semiBlendRange, dayToSunsetThresh + semiBlendRange, sunAngle, dayFogDensity, sunsetFogDensity);

            // Music 
            dayMusicVolume = duringDay_dayMusicVolume;
            nightMusicVolume = duringDay_nightMusicVolume;
        }



        // Sunset
        else if (dayToSunsetThresh + semiBlendRange < sunAngle && sunAngle <= sunsetToNightThresh - semiBlendRange)
        {
            //Debug.Log("=  Sunset  =");
            StoreProceduralSettings(sunsetSkyColor, sunsetExposure, sunsetLightColor, sunsetLightIntensity);

            shouldEnableStars = false; // Stars disabled

            ambientLightIntensity = dayIntensityMultiplier; // normal ambient light for dayTime

            // Fog
            fogColor = sunsetFogColor;
            fogDensity = sunsetFogDensity;

            // Music 
            dayMusicVolume = duringDay_dayMusicVolume;
            nightMusicVolume = duringDay_nightMusicVolume;
        }



        // Sunset - Night transition
        else if (sunsetToNightThresh - semiBlendRange < sunAngle && sunAngle <= sunsetToNightThresh + semiBlendRange)
        {
            //Debug.Log("== Sunset to Night ==");

            StoreProceduralTransitionSettings(sunsetToNightThresh - semiBlendRange, sunsetToNightThresh + semiBlendRange, sunAngle,
                            sunsetSkyColor, nightSkyColor, sunsetExposure, nightExposure, sunsetLightColor, nightLightColor,
                            sunsetLightIntensity, nightLightIntensity);

            shouldEnableStars = true; // Stars enabled 	-> as the skybox gradually darkens, the stars seem to progressively appear

            // Smooth transition from normal ambient light (dayTime) to very weak ambient light (nightTime)
            ambientLightIntensity = CalculateFloatInterpolation(sunsetToNightThresh - semiBlendRange, sunsetToNightThresh + semiBlendRange,
                            sunAngle, dayIntensityMultiplier, nightIntensityMultiplier);

            // Fog smooth transition
            fogColor = CalculateColorInterpolation(sunsetToNightThresh - semiBlendRange, sunsetToNightThresh + semiBlendRange, sunAngle, sunsetFogColor, nightFogColor);
            fogDensity = CalculateFloatInterpolation(sunsetToNightThresh - semiBlendRange, sunsetToNightThresh + semiBlendRange, sunAngle, sunsetFogDensity, nightFogDensity);


            // Music smooth transition from daytime to nighttime
            dayMusicVolume = CalculateFloatInterpolation(nightToSunriseThresh - semiBlendRange, nightToSunriseThresh + semiBlendRange, sunAngle,
                            duringDay_dayMusicVolume, duringNight_dayMusicVolume);
            nightMusicVolume = CalculateFloatInterpolation(nightToSunriseThresh - semiBlendRange, nightToSunriseThresh + semiBlendRange, sunAngle,
                            duringDay_nightMusicVolume, duringNight_nightMusicVolume);
        }



        //============= SKY'S UPDATES BASED ON THE DAYTIMES' LOGIC =============
        // Update the skybox and the directional lights with the stored settings
        UpdateSkyboxAndLights(proceduralSkybox, skyColor, exposure, lightColor, lightIntensity);

        // Update the stars Particle System
        UpdateStarsActivation(shouldEnableStars);

        // Update ambient light intensity
        RenderSettings.ambientIntensity = ambientLightIntensity;

        // Update Fog
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
        //Debug.Log("_________________RenderSettings.fogColor = " + RenderSettings.fogColor);
        //Debug.Log("_________________RenderSettings.fogDensity = " + RenderSettings.fogDensity);

        // Update Music
        UpdateVolumes(dayMusicVolume, nightMusicVolume);




        //============= SEND INFORMATION TO GAME MANAGER =============
        gm.SetItIsNight(itIsNight);

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
        //Debug.Log("In ApplyTransition : startThreshold = " + startThreshold + "	endThreshold = " + endThreshold);

        // Calculate blend factor
        float factor = Mathf.InverseLerp(startThreshold, endThreshold, sunAngle);

        // Interpolate skybox and light properties
        skyColor = Color.Lerp(startSkyColor, endSkyColor, factor);
        exposure = Mathf.Lerp(startExposure, endExposure, factor);
        lightColor = Color.Lerp(startLightColor, endLightColor, factor);
        lightIntensity = Mathf.Lerp(startLightIntensity, endLightIntensity, factor);

        //Debug.Log($"Transition factor: {factor}, Sky Color: {skyColor}, Exposure: {exposure}");
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
                
                gm.SetItIsNight(itIsNight);// Update the itIsNight Value of the game manager

                sunDirectionalLight.enabled = false; // Disabling sun
                moonDirectionalLight.enabled = true; // Enabling moon
                skybox.SetFloat("_SunSize", 0); // Hiding the default "moon"
                RenderSettings.ambientIntensity = nightIntensityMultiplier;
            }
            // Moon settings don't change

        }
        else if (0 < sunAngle || sunAngle < 180) // Sun under the horizon - moon over the horizon (opposites)
        {
            if (itIsNight) // If itIsNight variable is still set to true, it has to be updated
            {
                itIsNight = false;
                gm.SetItIsNight(itIsNight);// Update the itIsNight Value of the game manager

                sunDirectionalLight.enabled = true; // Enabling sun
                moonDirectionalLight.enabled = false; // Disabling moon
                skybox.SetFloat("_SunSize", daySunSize); // Setting the sun size

            }

            // Update sun settings
            sunDirectionalLight.color = lightColor;
            sunDirectionalLight.intensity = lightIntensity;
        }
        Debug.Log("::::::::::::::::::::::::::::::::::::::::::::::ItisNight =" + itIsNight);

        //Debug.Log($"Updating Sun Light - Current Angle: {sunAngle}, Light Enabled: {sunDirectionalLight.enabled}");
        //Debug.Log($"Updating Moon Light - Current Angle: {moonAngle}, Light Enabled: {moonDirectionalLight.enabled}");
    }





    private void UpdateStarsActivation(bool playSystem)
    {
        if (playSystem && !starsParticleSystem.isPlaying) starsParticleSystem.Play();
        else if (!playSystem && starsParticleSystem.isPlaying) starsParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }






    // This function updates the moonDirectional Light orientation and the moon's (gameObject) position/rotation, depending on the sunAngle
    private void UpdateMoon()
    {

        // ========== MOON GAME OBJECT =========
        // Calculate the moon angle based on the sun angle
        //moonAngle = sunAngle < 0 ? sunAngle + 180 : sunAngle - 180; 

        // Gradual Angle supplement is here to make the moon visible when the sun is under the horizon but still close to it (= what happens at night in the game)
        if (-13 < sunAngle && sunAngle < 0)
        {
            float factor = Mathf.InverseLerp(0, -13, sunAngle); // Calculate factor
            float interpolatedSupplement = Mathf.Lerp(0, angleMaxSupplement, factor); // Interpolation
            moonAngle = sunAngle + 180 - interpolatedSupplement;
        }

        else if (-180 < sunAngle && sunAngle < -167)
        {
            //moonAngle = sunAngle + 180 + angleMaxSupplement;
            float factor = Mathf.InverseLerp(-180, -167, sunAngle); // Calculate factor
            float interpolatedSupplement = Mathf.Lerp(0, angleMaxSupplement, factor); // Interpolation
            moonAngle = sunAngle + 180 + interpolatedSupplement;
        }
        else
        {
            moonAngle = sunAngle < 0 ? 90 : -90; // Basic behavior (moon and sun opposites)
        }

        // Convert the moonAngle (degrees) in radians
        float theta = moonAngle * Mathf.Deg2Rad;

        // Calculate the new position on the ellipse ("-" found by experimenting)
        float z = moonOrbitHRadius * Mathf.Cos(-theta);
        float y = moonOrbitVRadius * Mathf.Sin(-theta);
        moon.transform.position = moonOrbitCenterPoint.position + new Vector3(0, -y, -z);


        // M : PROBLEM : IT FLIPS AT 90° OR IT ROTATES IN THE WRONG DIRECTION
        // Make the moon always face the center of the orbit
        Vector3 directionToCenter = moonOrbitCenterPoint.position - moon.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToCenter);
        //targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, moonYRotation, targetRotation.eulerAngles.z); // with this no flip but it rotates in the wrong direction before -90° and after 90°
        moon.transform.rotation = targetRotation;


        // ========== MOON DIRECTIONAL LIGHT ==========
        // apply the moonAngle on the moonDirectionalLight
        moonDirectionalLight.transform.rotation = Quaternion.Euler(moonAngle, 0, 0);

    }






    private float CalculateFloatInterpolation(float startThreshold, float endThreshold, float sunAngle, float startFloat, float endFloat)
    {
        float factor = Mathf.InverseLerp(startThreshold, endThreshold, sunAngle); // Calculate blend factor
        return Mathf.Lerp(startFloat, endFloat, factor); // Interpolation
    }

    private Color CalculateColorInterpolation(float startThreshold, float endThreshold, float sunAngle, Color startColor, Color endColor)
    {
        float factor = Mathf.InverseLerp(startThreshold, endThreshold, sunAngle); // Calculate blend factor
        return Color.Lerp(startColor, endColor, factor); // Interpolation
    }



    // Function to manage the background musics volumes 
    private void UpdateVolumes(float volumeDayMusic, float volumeNightMusic)
    {
        audioMixer.SetFloat("DayMusicVolume", volumeDayMusic);
        audioMixer.SetFloat("NightMusicVolume", volumeNightMusic);
    }
}

