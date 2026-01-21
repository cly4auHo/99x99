using System;
using UnityEngine;
using Random = System.Random;

public static class Generator
{
    private static Random random = new Random();
    
    public static GameModel GetExample(in int minValue, in int maxValue)
    {
        var model = new GameModel
        {
            Factor = random.Next(minValue, maxValue),
            Multiplier = random.Next(minValue, maxValue),
            Time = DateTime.UtcNow
        };

        model.Answer = model.Factor * model.Multiplier;
        
        return model;
    }
}
