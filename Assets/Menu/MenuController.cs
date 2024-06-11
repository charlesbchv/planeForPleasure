using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("Réglages du volume")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;

    [Header("Réglages de gameplay")]
    [SerializeField] private TMP_Text controllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;
    [SerializeField] private int defaultSen = 4;
    public int mainControllerSen = 4;

    [Header("Réglages des toggles")]
    [SerializeField] private Toggle invertYToggle = null;

    [Header("Réglages graphiques")]
    [SerializeField] private TMP_Text brightnessTextValue = null;
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private float defaultBrightness = 1.0f;

    [Header("UI de sélection d'avion")]
    public Image planeDisplayImage;
    public TMP_Text planeNameText;
    private int currentPlaneIndex = 0;
    private List<GameObject> planeModels;

    [Space(10)]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullScreenToggle;

    private int _qualityLevel;
    private bool _isFullScreen;
    private float _brightnessLevel;

    [Header("Confirmation")]
    [SerializeField] private GameObject comfirmationPrompt = null;

    [Header("Niveaux à charger")]
    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;

    [Header("Dropdowns de résolution")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    [Header("Panneau de pause")]
    [SerializeField] private GameObject pausePanel = null;
    private bool isPaused = false;

    private void Start()
    {
        LoadPlaneModels();
        UpdatePlaneDisplay();
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMainMenu();
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f; // Met le jeu en pause
        }
        else
        {
            Time.timeScale = 1f; // Reprend le jeu
        }
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Assurez-vous que le temps reprend normalement
        SceneManager.LoadScene("MainMenu"); // Remplacez "MainMenu" par le nom réel de votre scène de menu principal
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void NewGameDialogYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }

    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void setVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }

    public void volumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = Mathf.RoundToInt(sensitivity);
        controllerSenTextValue.text = sensitivity.ToString("0");
    }

    public void GameplayApply()
    {
        if (invertYToggle.isOn)
        {
            PlayerPrefs.SetInt("masterInvertY", 1);
        }
        else
        {
            PlayerPrefs.SetInt("masterInvertY", 0);
        }

        PlayerPrefs.SetFloat("masterSen", mainControllerSen);
        StartCoroutine(ConfirmationBox());
    }

    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
        brightnessTextValue.text = brightness.ToString("0.0");
    }

    public void SetFullScreen(bool isFullScreen)
    {
        _isFullScreen = isFullScreen;
    }

    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }

    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        PlayerPrefs.SetInt("masterFullScreen", (_isFullScreen ? 1 : 0));
        Screen.fullScreen = _isFullScreen;

        StartCoroutine(ConfirmationBox());
    }

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Graphics")
        {
            brightnessSlider.value = defaultBrightness;
            brightnessTextValue.text = defaultBrightness.ToString("0.0");

            qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);

            fullScreenToggle.isOn = false;
            Screen.fullScreen = false;

            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            resolutionDropdown.value = resolutions.Length;
            GraphicsApply();
        }

        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            volumeApply();
        }

        if (MenuType == "Gameplay")
        {
            controllerSenTextValue.text = defaultSen.ToString("0");
            controllerSenSlider.value = defaultSen;
            mainControllerSen = defaultSen;
            invertYToggle.isOn = false;
            GameplayApply();
        }
    }

    public IEnumerator ConfirmationBox()
    {
        comfirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        comfirmationPrompt.SetActive(false);
    }

    private void LoadPlaneModels()
    {
        // Initialiser la liste pour stocker les modèles chargés
        planeModels = new List<GameObject>();
        // Charger tous les modèles situés dans le dossier "Resources/Models/Plane"
        GameObject[] loadedModels = Resources.LoadAll<GameObject>("Models/Plane");

        // Ajouter chaque modèle chargé à la liste planeModels
        foreach (GameObject model in loadedModels)
        {
            planeModels.Add(model);
            Debug.Log("Model loaded: " + model.name); // Ajoutez cette ligne
        }
    }

    public void NextPlane()
    {
        if (planeModels.Count == 0) return;
        currentPlaneIndex = (currentPlaneIndex + 1) % planeModels.Count;
        UpdatePlaneDisplay();
    }

    public void PreviousPlane()
    {
        if (planeModels.Count == 0) return;
        currentPlaneIndex--;
        if (currentPlaneIndex < 0) currentPlaneIndex = planeModels.Count - 1;
        UpdatePlaneDisplay();
    }

    private void UpdatePlaneDisplay()
    {
        if (planeModels.Count > 0)
        {
            Debug.Log("Updating display for model: " + planeModels[currentPlaneIndex].name); // Ajoutez cette ligne
            // Supposons que vous avez une méthode pour créer un instantané du modèle
            Sprite planeSprite = CreateSpriteFromModel(planeModels[currentPlaneIndex]);
            planeDisplayImage.sprite = planeSprite;
            planeNameText.text = planeModels[currentPlaneIndex].name;

            // Test avec une image de test
            // Sprite testSprite = Resources.Load<Sprite>("TestImage");
            // planeDisplayImage.sprite = testSprite;
        }
    }

    private Sprite CreateSpriteFromModel(GameObject model)
    {
        // Créer une caméra et rendre le modèle sur un RenderTexture
        // Convertir le RenderTexture en Texture2D, puis en Sprite
        // C'est un exemple simplifié; implémentez la logique de rendu réelle
        RenderTexture rt = new RenderTexture(256, 256, 24);
        Camera tempCamera = new GameObject().AddComponent<Camera>();
        tempCamera.targetTexture = rt;
        tempCamera.transform.position = model.transform.position + new Vector3(0, 0, -10);
        tempCamera.transform.LookAt(model.transform);

        Texture2D texture = new Texture2D(256, 256, TextureFormat.RGB24, false);
        tempCamera.Render();
        RenderTexture.active = rt;
        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        Destroy(tempCamera.gameObject);
        Debug.Log("Sprite created from model: " + model.name); // Ajoutez cette ligne
        return sprite;
    }
}
