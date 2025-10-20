using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class ARPhotoManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject photoPreviewPanel; // Painel que aparece com a foto
    public Image photoPreviewImage;      // Imagem UI que mostra a foto
    public Button saveButton;
    public Button deleteButton;

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

    public void TakePhoto()
    {
        StartCoroutine(CapturePhoto());
    }

    private IEnumerator CapturePhoto()
    {
        // Espera o final do frame para capturar a tela
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;

        capturedTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        capturedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        capturedTexture.Apply();

        // Mostra a foto na tela
        photoPreviewImage.sprite = Sprite.Create(capturedTexture, new Rect(0, 0, capturedTexture.width, capturedTexture.height), new Vector2(0.5f, 0.5f));
        photoPreviewPanel.SetActive(true);
    }

    private void SavePhoto()
    {
        string fileName = $"Photo_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        string filePath = Path.Combine(folderPath, fileName);

        byte[] bytes = capturedTexture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        Debug.Log($"Foto salva em: {filePath}");
        photoPreviewPanel.SetActive(false);
    }

    private void DeletePhoto()
    {
        Debug.Log("Foto descartada");
        photoPreviewPanel.SetActive(false);
        Destroy(capturedTexture);
    }
}
