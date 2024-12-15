using Godot;
using System;

public partial class CoalPile : Node2D
{
    [Export] public Area2D Area2D;
    [Export] public AnimationPlayer AnimationPlayer;

    public override void _Ready()
    {
        Area2D.BodyEntered += OnBodyEntered;
        Area2D.BodyExited += OnBodyExited;
    }

    public void Interact(Player player)
    {
        player.CoalCount++;
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
