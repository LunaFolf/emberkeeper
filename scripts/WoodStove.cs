using Godot;
using System;

public partial class WoodStove : Node2D
{
    [Export] public int MaxCoalLevel = 5;
    [Export] public AnimatedSprite2D Sprite;
    [Export] public Timer Timer;
    [Export] public Label CoalCountLabel;

    [Export] public Area2D Area2D;
    [Export] public AnimationPlayer AnimationPlayer;

    private Area2D _area;
    private int _coalLevel;

    private bool _playerClose;

    public int CoalLevel
    {
        get => _coalLevel;

        set
        {
            _coalLevel = Math.Clamp(value, 0, 5);

            if (_coalLevel <= 0) Sprite.Play("off");
            else Sprite.Play("on");

            CoalCountLabel.Text = _coalLevel.ToString();
        }
    }

    public bool IsWarm => CoalLevel > 0;

    public override void _Ready()
    {
        Timer.Timeout += OnTimerTimeout;
        CoalLevel = MaxCoalLevel;
        Area2D.BodyEntered += OnBodyEntered;
        Area2D.BodyExited += OnBodyExited;
    }

    public void Interact(Player player)
    {
        if (player.CoalCount <= 0) return;
        if (CoalLevel >= MaxCoalLevel) return;

        CoalLevel++;
        player.CoalCount--;
        Timer.Start(); // Restart timer, so new coal doesn't burn out immediately
    }

    private void OnTimerTimeout()
    {
        CoalLevel--;
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
