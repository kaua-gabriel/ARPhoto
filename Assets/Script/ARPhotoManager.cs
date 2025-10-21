using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System;
using TMPro;

public class ARPhotoManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject photoPreviewPanel;
    public Image photoPreviewImage;
    public Button saveButton;
    public Button deleteButton;
    public TMP_Text dateTimeText;



    private Texture2D capturedTexture;
    private string folderPath;

    private void Start()
    {
        folderPath = Path.Combine(Application.persistentDataPath, "ARPhotos");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        photoPreviewPanel.SetActive(false);
        saveButton.onClick.AddListener(SavePhoto);
        deleteButton.onClick.AddListener(DeletePhoto);
    }

    private void Update()
    {
        // Atualiza a data/hora no Canvas constantemente
        dateTimeText.text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }

    public void TakePhoto()
    {
        StartCoroutine(CapturePhoto());
    }

    private IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame(); // espera frame final

        int width = Screen.width;
        int height = Screen.height;

        capturedTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        capturedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        capturedTexture.Apply();

        // A data/hora já está visível no Canvas, então não precisa adicionar manualmente
        photoPreviewImage.sprite = Sprite.Create(capturedTexture,
            new Rect(0, 0, capturedTexture.width, capturedTexture.height),
            new Vector2(0.5f, 0.5f));

        photoPreviewPanel.SetActive(true);
    }

    private void SavePhoto()
    {
        string fileName = $"Photo_{DateTime.Now:yyyyMMdd_HHmmss}.png";
        string filePath = Path.Combine(folderPath, fileName);

        byte[] bytes = capturedTexture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        Debug.Log($"📸 Foto salva em: {filePath}");
        photoPreviewPanel.SetActive(false);
    }

    private void DeletePhoto()
    {
        Debug.Log("Foto descartada");
        photoPreviewPanel.SetActive(false);
        Destroy(capturedTexture);
    }
}
