using Godot;
using System;

public partial class Elf : Node2D
{
    [Export] public int MaxColdLevel = 10;

    private int _coldLevel = 0;

    public int ColdLevel
    {
        get => _coldLevel;
        set
        {
            _coldLevel = value;
            ColdLevelLabel.Text = _coldLevel.ToString();
            Sprite.SpeedScale = 1 - ((float)_coldLevel / MaxColdLevel);

            GD.Print(
                $"{Name} is {_coldLevel} / {MaxColdLevel} ({(float)_coldLevel / MaxColdLevel}) cold."
                );
        }
    }

    private int _workProgress = 0;

    [Export] public int WorkSpeed = 25;
    [Export] public Timer ColdTimer;
    [Export] public Timer WorkTimer;

    [Export] public AnimatedSprite2D Sprite;

    // TODO: Remove Debug labels
    [Export] public Label ColdLevelLabel;

    public override void _Ready()
    {
        ColdTimer.Timeout += OnColdTimerTimeout;
        WorkTimer.Timeout += OnWorkTimerTimeout;
        StartColdTimer();
    }

    private void StartColdTimer()
    {
        var timerLength = GD.RandRange(5, 10);
        ColdTimer.Start(timerLength);
    }

    private void OnColdTimerTimeout()
    {
        if (ColdLevel < MaxColdLevel)
        {
            ColdLevel++;
            StartColdTimer();
        }

        if (ColdLevel == MaxColdLevel)
        {
            GD.Print($"{Name} has frozen!");
            // TODO: Change animation sprite to frozen elf
        }
    }

    private void OnWorkTimerTimeout()
    {
        var progress = WorkSpeed / (ColdLevel + 1);
        _workProgress = Math.Clamp(_workProgress + progress, 0, 100);

        if (_workProgress >= 100)
        {
            _workProgress = 0;
        }
    }
}
