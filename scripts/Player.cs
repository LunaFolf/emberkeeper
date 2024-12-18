using Godot;
using System;

public partial class Player : CharacterBody2D
{

    private Vector2 _zoomMin = new Vector2(2.0f, 2.0f);
    private Vector2 _zoomMax = new Vector2(4.0f, 4.0f);
    private Vector2 _zoomIncrement = new Vector2(0.2f, 0.2f);

    private bool _playerIsInteracting = false;

    private int _coalCount = 0;
    private string _floorSound;

    [Export] public float Speed = 170f;

    [Export] public Timer InteractTimer;
    [Export] public AnimatedSprite2D Sprite;
    [Export] public Label CoalCountLabel;
    [Export] public AudioStreamPlayer2D StepSoundPlayer;
    [Export] public Timer StepTimer;

    [Export] public TileMapLayer FloorLayer;

    public int CoalCount
    {
        get => _coalCount;
        set
        {
            _coalCount = value;
            if (CoalCountLabel != null) CoalCountLabel.Text = _coalCount > 0 ? _coalCount.ToString() : "";
        }
    }

    public Node InteractTarget;

    public override void _Ready()
    {
        InteractTimer.Timeout += OnInteractTimerTimeout;
        StepTimer.Timeout += OnStepTimerTimeout;
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Velocity;

        if (!_playerIsInteracting)
        {
            var directionHorizontal = Input.GetAxis("move_left", "move_right");
            var directionVertical = Input.GetAxis("move_up", "move_down");

            if (directionHorizontal != 0) velocity.X = Speed * directionHorizontal;
            else velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);

            if (directionVertical != 0) velocity.Y = Speed * directionVertical;
            else velocity.Y = Mathf.MoveToward(velocity.Y, 0, Speed);
        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);
            velocity.Y = Mathf.MoveToward(velocity.Y, 0, Speed);
        }

        Velocity = velocity;

        MoveAndSlide();
    }

    public override void _Process(double delta)
    {
        var camera = GetNode<Camera2D>("Camera2D");

        if (Input.IsActionJustPressed("scroll_down") && camera.Zoom > _zoomMin)
        {
            camera.Zoom -= _zoomIncrement;
        }
        else if (Input.IsActionJustPressed("scroll_up") && camera.Zoom < _zoomMax)
        {
            camera.Zoom += _zoomIncrement;
        }

        if (Input.IsActionJustPressed("interact") && InteractTimer.TimeLeft <= 0)
        {
            _playerIsInteracting = true;
            Sprite.Play("working");
            InteractTimer.Start();
        }

        if (FloorLayer != null)
        {
            var cell = FloorLayer.LocalToMap(Position);
            var cellData = FloorLayer.GetCellTileData(cell);
            var floorSound = cellData?.GetCustomData("Sound");

            if (floorSound != null && _floorSound != (string)floorSound)
            {
                _floorSound = (string)floorSound;
                var floorSoundStream = GD.Load<AudioStream>($"res://assets/sounds/step_{_floorSound}.wav");
                if (floorSoundStream != null) StepSoundPlayer.Stream = floorSoundStream;
            }
        }
    }

    private void OnStepTimerTimeout()
    {
        GD.Print(Velocity);
        if (Velocity.IsZeroApprox()) return;

        StepSoundPlayer.PitchScale = (float)GD.RandRange(0.9f, 1.1f);
        StepSoundPlayer.Play();
    }

    private void OnInteractTimerTimeout()
    {
        _playerIsInteracting = false;
        Sprite.Play("default");
        InteractTimer.Stop();

        GD.Print(InteractTarget);

        InteractTarget?.Call("Interact", this);
    }

    public void InRange(Node body)
    {
        InteractTarget = body;
    }

    public void OutOfRange(Node body)
    {
        if (InteractTarget == body) InteractTarget = null;
    }
}
