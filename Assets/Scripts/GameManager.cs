using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] protected Field fieldPrefab;
    [SerializeField] private Canvas canvas;
    [SerializeField, Range(10, 10_000)] private int minValue;
    [SerializeField, Range(99, 99_999)] private int maxValue;
    [SerializeField, Range(0, 10)] private float delayBetweenExamples;
    
    private Field field;
    private Server server;
    
    private void Start()
    {
        server ??= new Server();
        field = Instantiate(fieldPrefab, canvas.transform);
        field.Complete += OnClick;
        field.NeedUpdate += UpdateStatistic;
        SetExample(false);
    }
    
    private async void OnClick(GameModel model, int answer)
    {
        var time = (DateTime.UtcNow - model.Time).Seconds;
        
        if (answer == model.Answer)
        {
            field.CorrectAnswer();
            
            await server.ClaimAnswer(true, time);
        }
        else
        {
            field.IncorrectAnswer();
            
            await server.ClaimAnswer(false, time);
        }

        SetExample(true);
    }

    private async void UpdateStatistic()
    {
        var answers = await server.GetAnswers();

        if (!answers.data.Any())
        {
            field.UpdateStatistic(0, 0);
            return;
        }
        
        var winRate = 0;
        var avgTime = 0;

        foreach (var answer in answers.data)
        {
            avgTime += answer.Time;

            if (answer.Solved)
                winRate++;
        }
        
        avgTime /= answers.data.Length;
        winRate = winRate * 100 / answers.data.Length;
        
        field.UpdateStatistic(winRate, avgTime);
    }

    private async void SetExample(bool withDelay)
    {
        if (withDelay)
            await Task.Delay((int)(delayBetweenExamples * 1_000));
            
        if (field)
            field.SetExample(Generator.GetExample(minValue, maxValue + 1));
    }
    
    private void OnDestroy()
    {
        if (field)
            field.Complete -= OnClick;
    }
}
