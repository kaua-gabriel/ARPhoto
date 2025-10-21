using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;

public class ARPhotoManager : MonoBehaviour
{
    [Header("HUD")]
    public GameObject HUDPanel;            // HUDPanel
    public Button TakePhotoButton;         // TakePhoto
    public TMP_Text dateTimeText;          // HUData/dateTimeText

    [Header("Preview")]
    public GameObject photoPreviewPanel;   // photoPreviewPanel
    public Image photoPreviewImage;        // photoPreviewImage
    public TMP_Text previewDateTimeText;   // pode reutilizar dateTimeText ou novo TMP_Text
    public Button saveButton;              // saveButton
    public Button deleteButton;            // deleteButton

    [Header("Feedback")]
    public TMP_Text messageText;           // opcional, para mensagens rápidas

    private Texture2D capturedTexture;

    private void Start()
    {
        HUDPanel.SetActive(true);
        photoPreviewPanel.SetActive(false);
        if (messageText != null)
            messageText.gameObject.SetActive(false);

        TakePhotoButton.onClick.AddListener(TakePhoto);
        saveButton.onClick.AddListener(SavePhotoToGallery);
        deleteButton.onClick.AddListener(DeletePhoto);
    }

    private void Update()
    {
        if (dateTimeText != null)
            dateTimeText.text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }

    public void TakePhoto()
    {
        StartCoroutine(CapturePhotoCoroutine());
    }

    private IEnumerator CapturePhotoCoroutine()
    {
        HUDPanel.SetActive(false);
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        capturedTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        capturedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        capturedTexture.Apply();

        HUDPanel.SetActive(true);

        photoPreviewImage.sprite = Sprite.Create(capturedTexture, new Rect(0, 0, capturedTexture.width, capturedTexture.height), new Vector2(0.5f, 0.5f));
        previewDateTimeText.text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

        photoPreviewPanel.SetActive(true);
        photoPreviewPanel.transform.SetAsLastSibling();
    }

    private void SavePhotoToGallery()
    {
        if (capturedTexture == null)
        {
            ShowMessage("Nenhuma foto para salvar!");
            return;
        }

        NativeGallery.SaveImageToGallery(capturedTexture, "ARPhotos", $"Photo_{DateTime.Now:yyyyMMdd_HHmmss}.png",
            (success, path) =>
            {
                if (success)
                    ShowMessage("📸 Foto salva na galeria!");
                else
                    ShowMessage("❌ Erro ao salvar foto!");
            });

        ClosePreview();
    }

    private void DeletePhoto()
    {
        if (capturedTexture != null)
        {
            Destroy(capturedTexture);
            capturedTexture = null;
        }

        ShowMessage("🗑 Foto descartada");
        ClosePreview();
    }

    private void ClosePreview()
    {
        photoPreviewPanel.SetActive(false);
        photoPreviewImage.sprite = null;
        HUDPanel.SetActive(true);
    }

    private void ShowMessage(string msg)
    {
        if (messageText == null) return;

        messageText.text = msg;
        messageText.gameObject.SetActive(true);

        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), 2f);
    }

    private void HideMessage()
    {
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }
}
