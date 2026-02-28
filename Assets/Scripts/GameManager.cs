using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] protected Field fieldPrefab;
    [SerializeField] private Canvas canvas;
    [SerializeField] private int minValue;
    [SerializeField] private int maxValue;
    [SerializeField] private int exampleAmount;
    [SerializeField] private int winRateToCheck;
    [SerializeField, Range(0, 10)] private float delayBetweenExamples;
    
    private Field field;
    private Server server;
    private int allAmount;
    private int rightAmount;
    private bool feedbackExist;
    
    private int WinRate => rightAmount * 100 / allAmount;
    
    private void Start()
    {
        server ??= new Server();
        field = Instantiate(fieldPrefab, canvas.transform);
        field.Complete += OnClick;
        field.NeedUpdate += UpdateStatistic;
        field.Feedback += SendFeedback;
        SetExample(false);
        UpdateStatistic();
        CheckIfFeedbackExist();
    }
    
    private async void SetExample(bool withDelay)
    {
        if (withDelay)
            await Task.Delay((int)(delayBetweenExamples * 1_000));
            
        if (field)
            field.SetExample(Generator.GetExample(minValue, maxValue + 1));
    }
    
    private async void OnClick(GameModel model, int answer)
    {
        var time = (DateTime.UtcNow - model.Time).Seconds;
        allAmount++;
        
        if (answer == model.Answer)
        {
            field.CorrectAnswer();
            rightAmount++;
            
            await server.ClaimAnswer(true, time);
        }
        else
        {
            field.IncorrectAnswer();
            
            await server.ClaimAnswer(false, time);
        }
        
        if (feedbackExist)
            SetExample(true);
        else
        {
            if (allAmount < exampleAmount)
                SetExample(true);
            else
            {
                if (WinRate < winRateToCheck)
                    SetExample(true);
                else
                {
                    await Task.Delay((int)(delayBetweenExamples * 1_000));
                    
                    field.ShowFeedback();
                }
            }
        }
    }

    private async void UpdateStatistic()
    {
        var answers = await server.GetAnswers();

        if (!answers.data.Any())
        {
            field.UpdateStatistic(0, 0);
            return;
        }
        
        allAmount = answers.data.Length;
        rightAmount = 0;
        var avgTime = 0;
        
        foreach (var answer in answers.data)
        {
            avgTime += answer.Time;

            if (answer.Solved)
                rightAmount++;
        }
        
        field.UpdateStatistic(WinRate, avgTime / allAmount);
    }

    private async void SendFeedback(string feedback)
    {
        feedbackExist = await server.SendFeedback(feedback);
        
        SetExample(true);
    }

    private async void CheckIfFeedbackExist()
    {
        feedbackExist = await server.IsFeedbackExist();
    }
    
    private void OnDestroy()
    {
        if (field)
            field.Complete -= OnClick;
    }
}
