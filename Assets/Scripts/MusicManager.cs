using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    private AudioSource audioSource;
    
    [SerializeField] private AudioClip defaultMusic;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private string sliderSceneName = "LoadingScene"; // Имя сцены, где находится слайдер
    [SerializeField] private float volumeStep = 0.1f; // Шаг изменения громкости для клавиш
    private const string VolumeKey = "MusicVolume";
    private bool isSliderFound = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSource();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void InitializeAudioSource()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = defaultMusic;
        
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.5f);
        audioSource.volume = savedVolume;
        
        if (defaultMusic != null)
        {
            audioSource.Play();
        }
    }

    void Update()
    {
        // Обработка нажатий клавиш + и -
        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.Equals))
        {
            SetVolume(audioSource.volume + volumeStep);
        }
        else if (Input.GetKeyDown(KeyCode.Minus))
        {
            SetVolume(audioSource.volume - volumeStep);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Проверяем, если слайдер уже найден, пропускаем
        if (isSliderFound) return;

        // Проверяем, является ли загруженная сцена той, где находится слайдер
        if (scene.name == sliderSceneName)
        {
            GameObject sliderObj = GameObject.Find("Slider_Dyn_InputField");
            if (sliderObj != null)
            {
                volumeSlider = sliderObj.GetComponent<Slider>();
                if (volumeSlider != null)
                {
                    isSliderFound = true;
                    volumeSlider.value = audioSource.volume;
                    volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
                    Debug.Log("Slider found and linked in scene: " + scene.name);
                }
            }
        }
    }

    void OnVolumeChanged(float value)
    {
        audioSource.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }

    // Публичный метод для ручной привязки слайдера из другой сцены
    public void BindSlider(Slider slider)
    {
        if (slider != null)
        {
            volumeSlider = slider;
            isSliderFound = true;
            volumeSlider.value = audioSource.volume;
            volumeSlider.onValueChanged.RemoveAllListeners(); // Удаляем старые слушатели
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            Debug.Log("Slider manually bound to MusicManager.");
        }
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
        
        if (volumeSlider != null)
        {
            volumeSlider.value = volume;
        }
    }

    public float GetVolume()
    {
        return audioSource.volume;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (volumeSlider != null)
            {
                volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
            }
        }
    }
}