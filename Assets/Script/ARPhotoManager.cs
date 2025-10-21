using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;

public class ARPhotoManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject photoUI;             // CanvasFoto/PhotoUI
    public Button takePhotoButton;         // CanvasFoto/PhotoUI/TakePhoto
    public GameObject photoPreviewPanel;   // CanvasFoto/PhotoUI/photoPreviewPanel
    public Image photoPreviewImage;        // CanvasFoto/PhotoUI/photoPreviewPanel/photoPreviewImage
    public Button saveButton;              // CanvasFoto/PhotoUI/photoPreviewPanel/saveButton
    public Button deleteButton;            // CanvasFoto/PhotoUI/photoPreviewPanel/deleteButton
    public TMP_Text dateTimeText;          // HUData/dateTimeText

    private Texture2D capturedTexture;

    private void Start()
    {
        photoPreviewPanel.SetActive(false);

        takePhotoButton.onClick.AddListener(TakePhoto);
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
        // Esconde a UI antes de capturar
        photoUI.SetActive(false);
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        capturedTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        capturedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        capturedTexture.Apply();

        // Restaura a UI e mostra o preview
        photoUI.SetActive(true);
        photoPreviewImage.sprite = Sprite.Create(
            capturedTexture,
            new Rect(0, 0, capturedTexture.width, capturedTexture.height),
            new Vector2(0.5f, 0.5f));

        photoPreviewPanel.SetActive(true);
        photoPreviewPanel.transform.SetAsLastSibling(); // garante que o preview fica por cima
    }

    private void SavePhotoToGallery()
    {
        if (capturedTexture == null) return;

        NativeGallery.SaveImageToGallery(
            capturedTexture,
            "ARPhotos",
            $"Photo_{DateTime.Now:yyyyMMdd_HHmmss}.png",
            (success, path) =>
            {
                Debug.Log(success ? $"📸 Foto salva em: {path}" : "❌ Erro ao salvar foto!");
            });

        photoPreviewPanel.SetActive(false);
    }

    private void DeletePhoto()
    {
        if (capturedTexture != null)
        {
            Destroy(capturedTexture);
            capturedTexture = null;
        }

        photoPreviewImage.sprite = null;
        photoPreviewPanel.SetActive(false);
    }
}
