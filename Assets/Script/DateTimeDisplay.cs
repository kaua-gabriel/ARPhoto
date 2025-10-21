using UnityEngine;
using UnityEngine.UI;
using System;

public class DateTimeDisplay : MonoBehaviour
{
    public Text dateTimeText; // assign no Inspector

    void Update()
    {
        // Atualiza a cada frame
        dateTimeText.text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }
}
