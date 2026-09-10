using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSettings : MonoBehaviour
{
    [SerializeField] private PlayerData.ID playerId;

    [SerializeField] private TMP_Text playerText;
    [Header("Speed")]
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private Slider speedSlider;
    [Header("Width")]
    [SerializeField] private TMP_Text widthText;
    [SerializeField] private Slider widthSlider;
    [Header("Color")]
    [SerializeField] private Button colorButton;
    private Image colorButtonImage;

    private static readonly Color[] colorOptions =
    {
        Color.red, Color.blue, Color.green, Color.yellow, Color.cyan, Color.magenta, Color.white
    };

    private void Awake()
    {
        playerText.text = playerId == PlayerData.ID.One ? "Player 1" : "Player 2";

        colorButtonImage = colorButton.GetComponent<Image>();

        speedSlider.onValueChanged.AddListener(OnSpeedChanged);
        widthSlider.onValueChanged.AddListener(OnWidthChanged);
        colorButton.onClick.AddListener(RandomizeColor);
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    private void OnDestroy()
    {
        speedSlider.onValueChanged.RemoveListener(OnSpeedChanged);
        widthSlider.onValueChanged.RemoveListener(OnWidthChanged);
        colorButton.onClick.RemoveListener(RandomizeColor);
    }

    private void RefreshUI()
    {
        float speed = playerId == PlayerData.ID.One ? PlayerData.playerOneSpeed : PlayerData.playerTwoSpeed;
        float width = playerId == PlayerData.ID.One ? PlayerData.playerOneWidth : PlayerData.playerTwoWidth;
        Color color = playerId == PlayerData.ID.One ? PlayerData.playerOneColor : PlayerData.playerTwoColor;

        speedSlider.SetValueWithoutNotify(speed);
        speedText.text = "Speed: " + speed.ToString("0.0");

        widthSlider.SetValueWithoutNotify(width);
        widthText.text = "Width: " + width.ToString("0.0");

        colorButtonImage.color = color;
    }

    // Los sliders modifican los valores en PlayerData en lugar de tocar directamente los Players
    private void OnSpeedChanged(float value)
    {
        PlayerData.SetSpeed(playerId, value);
        speedText.text = "Speed: " + value.ToString("0.0");
    }

    private void OnWidthChanged(float value)
    {
        PlayerData.SetWidth(playerId, value);
        widthText.text = "Width: " + value.ToString("0.0");
    }

    private void RandomizeColor()
    {
        Color newColor = colorOptions[Random.Range(0, colorOptions.Length)];
        PlayerData.SetColor(playerId, newColor);
        colorButtonImage.color = newColor;
    }

}