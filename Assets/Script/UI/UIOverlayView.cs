using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOverlayView : MonoBehaviour
{
    [Header("Panels / Screens")]
    [SerializeField] private GameObject _startScreen;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;

    [Header("HP Bar UI")]
    [SerializeField] private Image _hpFillImage;   
    [SerializeField] private TextMeshProUGUI _hpText; 
    [SerializeField] private Button _restartButton;

    public Button RestartButton => _restartButton;

    public void ShowStartScreen()
    {
        _startScreen.SetActive(true);
        _winPanel.SetActive(false);
        _losePanel.SetActive(false);
    }

    public void ShowGameplay()
    {
        _startScreen.SetActive(false);
        _winPanel.SetActive(false);
        _losePanel.SetActive(false);
    }

    public void ShowWin()
    {
        _winPanel.SetActive(true);
    }

    public void ShowLose()
    {
        _losePanel.SetActive(true);
    }

    public void UpdateHP(float currentHp, float maxHp)
    {
        float fillRatio = Mathf.Clamp01(currentHp / maxHp);

        if (_hpFillImage != null)
        {
            _hpFillImage.fillAmount = fillRatio;
        }

        if (_hpText != null)
        {
            int currentInt = Mathf.Max(0, Mathf.CeilToInt(currentHp));
            int maxInt = Mathf.RoundToInt(maxHp);
            _hpText.text = $"{currentInt} / {maxInt}";
        }
    }
}