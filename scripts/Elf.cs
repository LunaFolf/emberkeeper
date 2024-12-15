using Godot;
using System;

public partial class Elf : Node2D
{
    [Export] public int MaxColdLevel = 10;
    [Export] public WoodStove NearestWoodStove;

    private int _coldLevel = 0;
    private bool _playerClose;

    public int ColdLevel
    {
        get => _coldLevel;
        set
        {
            _coldLevel = value;
            ColdLevelLabel.Text = _coldLevel > 0 ? _coldLevel.ToString() : "";
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
    [Export] public Timer InteractTimer;
    [Export] public Area2D Area2D;
    [Export] public AnimationPlayer AnimationPlayer;

    [Export] public AnimatedSprite2D Sprite;

    // TODO: Remove Debug labels
    [Export] public Label ColdLevelLabel;

    public override void _Ready()
    {
        ColdTimer.Timeout += OnColdTimerTimeout;
        WorkTimer.Timeout += OnWorkTimerTimeout;
        Area2D.BodyEntered += OnBodyEntered;
        Area2D.BodyExited += OnBodyExited;

        StartColdTimer();
    }

    public void Interact(Player player)
    {
        if (ColdLevel < MaxColdLevel) return;

        ColdLevel = GD.RandRange(0, MaxColdLevel -1);
        Sprite.Play("working");
    }

    private void StartColdTimer()
    {
        var timerLength = GD.RandRange(1, 5);
        ColdTimer.Start(timerLength);
    }

    private void OnColdTimerTimeout()
    {
        if (NearestWoodStove == null || !NearestWoodStove.IsWarm)
        {
            if (ColdLevel < MaxColdLevel)
            {
                ColdLevel++;
            }

            if (ColdLevel == MaxColdLevel)
            {
                GD.Print($"{Name} has frozen!");
                Sprite.Play("frozen");
            }
        }
        else if (NearestWoodStove != null)
        {
            if (ColdLevel < MaxColdLevel && ColdLevel > 0)
            {
                ColdLevel--;
            } else if (ColdLevel == MaxColdLevel)
            {
                GD.Print($"{Name} is frozen! can't warm up till de-iced!");
            }
        }

        StartColdTimer();
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

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player)
        {
            AnimationPlayer.Play("blinky");
            body.Call("InRange", this);
        }
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is Player)
        {
            AnimationPlayer.Stop();
            body.Call("OutOfRange", this);
        }
    }
}
