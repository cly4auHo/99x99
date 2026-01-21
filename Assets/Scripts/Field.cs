using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Field : MonoBehaviour
{
    public Action<GameModel, int> Complete;
    public Action NeedUpdate;
    
    [SerializeField] private GameObject gameField;
    [SerializeField] private TextMeshProUGUI example;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI answerType;
    
    [Header("Statistic")]
    [SerializeField] private StatisticButton statisticButton;
    [SerializeField] private GameObject statisticPanel;
    [SerializeField] private TextMeshProUGUI winRate;
    [SerializeField] private TextMeshProUGUI avgTime;
    
    private GameModel gameModel;
    private bool check;
    private bool updated;
    
    public void SetExample(GameModel model)
    {
        check = false;
        updated = false;
        gameModel = model;
        answerType.text = null;
        example.text = $"{model.Factor} x {model.Multiplier} =";
    }
    
    public void CorrectAnswer()
    {
        answerType.text = "Correct!";
        inputField.text = null;
    }

    public void IncorrectAnswer()
    {
        answerType.text = $"Wrong! Right answer is {gameModel.Answer}";
        inputField.text = null;
    }

    public void UpdateStatistic(in int win, in int time)
    {
        updated = true;
        winRate.text = $"Win Rate: {win}%";
        avgTime.text = $"Average Time: {time}s";
    }
    
    private void Start()
    {
        button.onClick.AddListener(OnClick);
        statisticButton.Hold += OnHold;
        statisticButton.Release += OnRelease;
        SetState(true);
    }

    private void SetState(in bool state)
    {
        gameField.SetActive(state);
        statisticPanel.SetActive(!state);
    }
    
    private void OnClick()
    {
        if (check)
            return;

        check = true;
        Complete?.Invoke(gameModel, int.TryParse(inputField.text, out var answer) ? answer : -1);
    }

    private void OnHold()
    {
        SetState(false);
        
        if (!updated)
            NeedUpdate?.Invoke();
    }

    private void OnRelease()
    {
        SetState(true);
    }
    
    private void OnDestroy()
    {
        if (button)
            button.onClick.RemoveListener(OnClick);
    }
}
