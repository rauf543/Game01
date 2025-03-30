using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages loading indicators and error messages across the UI
/// </summary>
public class LoadingUIManager : MonoBehaviour
{
    [Header("Loading Indicator")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private GameObject loadingSpinner;
    
    [Header("Error Display")]
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private Button errorCloseButton;

    private void Start()
    {
        // Ensure everything is hidden on start
        HideLoading();
        HideError();

        // Set up error close button
        if (errorCloseButton != null)
        {
            errorCloseButton.onClick.AddListener(HideError);
        }
    }

    /// <summary>
    /// Shows the loading indicator with optional custom text
    /// </summary>
    /// <param name="message">Custom loading message (defaults to "Loading...")</param>
    public void ShowLoading(string message = "Loading...")
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        if (loadingText != null)
        {
            loadingText.text = message;
        }

        if (loadingSpinner != null)
        {
            loadingSpinner.SetActive(true);
        }
    }

    /// <summary>
    /// Hides the loading indicator
    /// </summary>
    public void HideLoading()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Shows an error message
    /// </summary>
    /// <param name="message">Error message to display</param>
    public void ShowError(string message)
    {
        // Hide loading indicator if it's visible
        HideLoading();

        if (errorPanel != null)
        {
            errorPanel.SetActive(true);
        }

        if (errorText != null)
        {
            errorText.text = message;
        }
    }

    /// <summary>
    /// Hides the error panel
    /// </summary>
    public void HideError()
    {
        if (errorPanel != null)
        {
            errorPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (errorCloseButton != null)
        {
            errorCloseButton.onClick.RemoveListener(HideError);
        }
    }
}