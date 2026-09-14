using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RulesPanel : MonoBehaviour
{
    [SerializeField] private GameSettingsSo gameSettings;

    [Header("Match Mode Buttons")]
    [SerializeField] private Button roundsModeButton;
    [SerializeField] private Button timedModeButton;

    [Header("Options Panels")]
    [SerializeField] private GameObject roundsMatchPanel;
    [SerializeField] private GameObject timedMatchPanel;

    [Header("Preset Buttons")]
    [SerializeField] private Button[] roundsPresetButtons;
    [SerializeField] private Button[] timedPresetButtons;

    [SerializeField] private TMP_Text confirmationText;

    [Header("Selection Color")]
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color unselectedColor = new Color(.7f, .7f, .7f);

    [Header("Play Button")]
    [SerializeField] private Button playButton;

    public event Action<MatchMode, int> OnRulesConfirmed;

    private MatchMode selectedMode;
    private int selectedRoundsIndex;
    private int selectedTimedIndex;

    private void Awake()
    {
        roundsModeButton.onClick.AddListener(SelectRoundsMode);
        timedModeButton.onClick.AddListener(SelectTimedMode);

        for (int i = 0; i < roundsPresetButtons.Length; i++)
        {
            int index = i;
            roundsPresetButtons[i].onClick.AddListener(() => SelectRoundsPreset(index));
        }

        for (int i = 0; i < timedPresetButtons.Length; i++)
        {
            int index = i;
            timedPresetButtons[i].onClick.AddListener(() => SelectTimedPreset(index));
        }

        playButton.onClick.AddListener(Confirm);
    }

    private void OnDestroy()
    {
        roundsModeButton.onClick.RemoveListener(SelectRoundsMode);
        timedModeButton.onClick.RemoveListener(SelectTimedMode);
        for (int i = 0; i < roundsPresetButtons.Length; i++)
        {
            roundsPresetButtons[i].onClick.RemoveAllListeners();
        }

        for (int i = 0; i < timedPresetButtons.Length; i++)
        {
            int index = i;
            timedPresetButtons[i].onClick.RemoveAllListeners();
        }

        playButton.onClick.RemoveAllListeners();
    }

    public void Open()
    {
        gameObject.SetActive(true);

        // Defaults al abrir: siempre hay algo elegido, asi Play nunca queda en estado invalido
        selectedRoundsIndex = 1;
        selectedTimedIndex = 1;
        SelectRoundsMode();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    // Los cuatro metodos de abajo se enganchan al OnClick de cada boton en el Inspector
    public void SelectRoundsMode()
    {
        selectedMode = MatchMode.Rounds;

        roundsMatchPanel.SetActive(true);
        timedMatchPanel.SetActive(false);

        RefreshSelection();
    }

    public void SelectTimedMode()
    {
        selectedMode = MatchMode.Timed;

        roundsMatchPanel.SetActive(false);
        timedMatchPanel.SetActive(true);

        RefreshSelection();
    }

    public void SelectRoundsPreset(int index)
    {
        selectedRoundsIndex = index;
        RefreshSelection();
    }

    public void SelectTimedPreset(int index)
    {
        selectedTimedIndex = index;
        RefreshSelection();
    }

    public void Confirm()
    {
        int index = selectedMode == MatchMode.Rounds ? selectedRoundsIndex : selectedTimedIndex;

        Close();
        OnRulesConfirmed?.Invoke(selectedMode, index);
    }

    private void RefreshSelection()
    {
        bool isRounds = selectedMode == MatchMode.Rounds;

        SetButtonHighlight(roundsModeButton, isRounds);
        SetButtonHighlight(timedModeButton, !isRounds);

        HighlightPresets(roundsPresetButtons, selectedRoundsIndex);
        HighlightPresets(timedPresetButtons, selectedTimedIndex);

        UpdateConfirmationText();
    }

    private void HighlightPresets(Button[] buttons, int selectedIndex)
    {
        for (int i = 0; i < buttons.Length; i++)
            SetButtonHighlight(buttons[i], i == selectedIndex);
    }

    private void SetButtonHighlight(Button button, bool isSelected)
    {
        button.targetGraphic.color = isSelected ? selectedColor : unselectedColor;
    }

    private void UpdateConfirmationText()
    {
        if (selectedMode == MatchMode.Rounds)
        {
            int bestOf = gameSettings.GetBestOfPresets()[selectedRoundsIndex];
            int roundsToWin = MatchSettings.CalculateRoundsToWin(bestOf);

            confirmationText.text = roundsToWin == 1 ? "First to 1 goal" : $"First to {roundsToWin} goals";
        }
        else
        {
            float duration = gameSettings.GetDurationPresets()[selectedTimedIndex];

            confirmationText.text = $"{duration} seconds match";
        }
    }
}