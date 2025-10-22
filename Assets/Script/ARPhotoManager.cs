using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using TMPro;
using System;

public class ARPhotoManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject photoHUDPanel;      // painel principal (HUD da câmera)
    public GameObject photoPreviewPanel;  // painel de prévia
    public RawImage photoPreviewImage;
    public Button saveButton;
    public Button deleteButton;
    public TMP_Text dateTimeText;

    private Texture2D capturedTexture;
    private string folderPath;

    private void Start()
    {
        // Cria pasta de fotos internas
        folderPath = Path.Combine(Application.persistentDataPath, "ARPhotos");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        // Inicializa os painéis
        photoPreviewPanel.SetActive(false);
        photoHUDPanel.SetActive(true);

        // Liga os botões
        if (saveButton != null)
            saveButton.onClick.AddListener(SavePhoto);
        if (deleteButton != null)
            deleteButton.onClick.AddListener(DeletePhoto);
    }

    private void Update()
    {
        // Atualiza data/hora em tempo real
        if (dateTimeText != null)
            dateTimeText.text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }

    // Tira a foto
    public void TakePhoto()
    {
        StartCoroutine(CapturePhoto());
    }

    private IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;

        capturedTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        capturedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        capturedTexture.Apply();

        photoPreviewImage.texture = capturedTexture;
        photoHUDPanel.SetActive(false);
        photoPreviewPanel.SetActive(true);
    }

    // Salva e volta pro HUD
    public void SavePhoto()
    {
        if (capturedTexture == null) return;

        string fileName = $"Photo_{DateTime.Now:yyyyMMdd_HHmmss}.png";
        string filePath = Path.Combine(folderPath, fileName);

        File.WriteAllBytes(filePath, capturedTexture.EncodeToPNG());
        Debug.Log($"📸 Foto salva em: {filePath}");

#if UNITY_ANDROID || UNITY_IOS
        // Envia pra galeria (opcional)
        NativeGallery.SaveImageToGallery(filePath, "AR Photos", fileName);
        Debug.Log("✅ Foto enviada para a galeria");
#endif

        // Limpa preview e volta
        ReturnToCameraHUD("✅ Foto salva");
    }

    // Exclui e volta pro HUD
    public void DeletePhoto()
    {
        if (capturedTexture != null)
            Destroy(capturedTexture);

        ReturnToCameraHUD("❌ Foto descartada");
    }

    // ---- Função auxiliar para retornar à câmera ----
    private void ReturnToCameraHUD(string logMsg)
    {
        capturedTexture = null;
        photoPreviewImage.texture = null;

        photoPreviewPanel.SetActive(false);
        photoHUDPanel.SetActive(true);

        Debug.Log(logMsg);
    }
}
