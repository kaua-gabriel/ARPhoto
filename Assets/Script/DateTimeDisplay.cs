using UnityEngine;
using TMPro;
using System;

public class DateTimeDisplay : MonoBehaviour
{
    public TMP_Text dateTimeText; // assign no Inspector

    void Update()
    {
        dateTimeText.text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }
}
