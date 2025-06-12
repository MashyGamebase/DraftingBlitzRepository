using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlaylist : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject howToPlayCanvas;

    [Header("Playlist Items")]
    public List<PlaylistItem> playlist = new List<PlaylistItem>();

    [Header("Display Components")]
    public Image imageDisplay;             // UI Image for Sprites
    public RawImage videoDisplay;          // UI RawImage with RenderTexture
    public VideoPlayer videoPlayer;        // Plays videos

    [Header("UI Buttons")]
    public Button nextButton;
    public Button previousButton;
    public Button startButton;

    private int currentIndex = 0;

    void Start()
    {
        nextButton.onClick.AddListener(OnNextPressed);
        previousButton.onClick.AddListener(OnPreviousPressed);
        startButton.onClick.AddListener(OnStartPressed);

        PlayCurrent();
    }

    void PlayCurrent()
    {
        if (playlist.Count == 0) return;

        PlaylistItem current = playlist[currentIndex];

        // Hide all first
        imageDisplay.gameObject.SetActive(false);
        videoDisplay.gameObject.SetActive(false);
        videoPlayer.Stop();

        if (current.type == PlaylistItem.ItemType.Image && current.image != null)
        {
            imageDisplay.gameObject.SetActive(true);
            imageDisplay.sprite = current.image;
        }
        else if (current.type == PlaylistItem.ItemType.Video && current.video != null)
        {
            videoDisplay.gameObject.SetActive(true);
            videoPlayer.clip = current.video;
            videoPlayer.Play();
        }

        UpdateButtonStates();
    }

    public void CheckIfViewed()
    {
        bool viewed = PlayerPrefs.GetInt("HowToPlay") == 1 ? true : false;

        if(!viewed)
        {
            howToPlayCanvas.gameObject.SetActive(true);
            PlayerPrefs.SetInt("HowToPlay", 1);
            PlayerPrefs.Save();
        }
    }

    void OnNextPressed()
    {
        if (currentIndex < playlist.Count - 1)
        {
            currentIndex++;
            PlayCurrent();
        }
    }

    void OnPreviousPressed()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            PlayCurrent();
        }
    }

    void OnStartPressed()
    {
        Debug.Log("Start pressed. Add logic here.");
        // You can reset or load a new scene, etc.
    }

    public void OnResetPressed()
    {
        currentIndex = 0;
        PlayCurrent();
    }

    void UpdateButtonStates()
    {
        bool isLast = currentIndex == playlist.Count - 1;

        nextButton.gameObject.SetActive(!isLast);
        startButton.gameObject.SetActive(isLast);

        previousButton.interactable = currentIndex > 0;
    }
}

[System.Serializable]
public class PlaylistItem
{
    public enum ItemType { Image, Video }

    public ItemType type;
    public Sprite image;          // For images
    public VideoClip video;       // For videos
}