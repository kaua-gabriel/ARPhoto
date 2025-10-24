using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System.IO;

public class ARPhotoManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject photoHUDPanel;       // CanvasHUD
    public GameObject photoPreviewPanel;   // CanvasPreview
    public RawImage photoPreviewImage;
    public Button saveButton;
    public Button deleteButton;
    public TMP_Text feedbackText;          // Texto TMP

    [Header("Config")]
    public int maxPhotosPerSession = 5;
    public int maxPhotoSizeMB = 5;

    private Texture2D capturedTexture;
    private int photosSent = 0;
    private bool isCapturing = false;

    private void Start()
    {
        // Inicializa painéis
        photoHUDPanel.SetActive(true);
        photoPreviewPanel.SetActive(false);
        feedbackText.gameObject.SetActive(false);

        saveButton.onClick.AddListener(OnSaveClicked);
        deleteButton.onClick.AddListener(OnDeleteClicked);
    }

    public void TakePhoto()
    {
        if (isCapturing) return;

        if (photosSent >= maxPhotosPerSession)
        {
            ShowFeedback(" Limite de 5 fotos atingido!");
            return;
        }

        StartCoroutine(CapturePhotoFlow());
    }

    private IEnumerator CapturePhotoFlow()
    {
        isCapturing = true;

        // Oculta HUD e mostra feedback
        photoHUDPanel.SetActive(false);
        feedbackText.text = "📸 Tirando foto...";
        feedbackText.gameObject.SetActive(true);

        // Espera o frame terminar de desenhar
        yield return new WaitForEndOfFrame();

        // Captura a tela
        int width = Screen.width;
        int height = Screen.height;
        capturedTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        capturedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        capturedTexture.Apply();

        // Aplica na preview
        photoPreviewImage.texture = capturedTexture;
        photoPreviewPanel.SetActive(true);

        // Oculta feedback
        feedbackText.gameObject.SetActive(false);

        isCapturing = false;
    }

    private void OnSaveClicked()
    {
        if (capturedTexture == null) return;

        byte[] pngData = capturedTexture.EncodeToPNG();
        float sizeMB = pngData.Length / (1024f * 1024f);

        if (sizeMB > maxPhotoSizeMB)
        {
            ShowFeedback(" Foto muito grande!");
            return;
        }

        // 🔹 Envio para Webhook.site (teste)
        StartCoroutine(SendPhotoToWebhook(pngData));

        // 🔹 Envio para PHP original (se quiser ativar depois)
        // StartCoroutine(UploadPhoto(pngData));
    }

    private void OnDeleteClicked()
    {
        ResetPhotoState();
        ShowFeedback(" Foto descartada");
    }

    private IEnumerator UploadPhoto(byte[] imageData)
    {
        string fileName = "foto_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";

        WWWForm form = new WWWForm();
        form.AddBinaryData("photo", imageData, fileName, "image/png");

        // 🔹 URL de teste — troque depois pelo seu PHP
        using (UnityWebRequest www = UnityWebRequest.Post("https://seudominio.com/upload_photo.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success && www.downloadHandler.text.Contains("OK"))
            {
                photosSent++;
                ShowFeedback(" Foto enviada com sucesso!");
            }
            else
            {
                ShowFeedback(" Erro ao enviar foto!");
            }
        }

        ResetPhotoState();
    }

    // 🔹 Método de teste para Webhook.site
    private IEnumerator SendPhotoToWebhook(byte[] imageData)
    {
        string fileName = "foto_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";

        WWWForm form = new WWWForm();
        form.AddBinaryData("photo", imageData, fileName, "image/png");

        // 🔹 Troque pela URL do seu Webhook.site
        using (UnityWebRequest www = UnityWebRequest.Post("https://webhook.site/1c21a47a-2ba1-4e5b-93e8-17cec0912da7", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                photosSent++;
                ShowFeedback(" Foto enviada para teste!");
            }
            else
            {
                ShowFeedback(" Erro ao enviar para teste!");
            }
        }

        ResetPhotoState();
    }

    private void ResetPhotoState()
    {
        if (capturedTexture != null)
            Destroy(capturedTexture);

        capturedTexture = null;
        photoPreviewImage.texture = null;

        photoPreviewPanel.SetActive(false);
        photoHUDPanel.SetActive(true);
    }

    private void ShowFeedback(string message)
    {
        StopCoroutine(nameof(HideFeedback));
        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);
        StartCoroutine(HideFeedback());
    }

    private IEnumerator HideFeedback()
    {
        yield return new WaitForSeconds(2.5f);
        feedbackText.gameObject.SetActive(false);
    }
}
