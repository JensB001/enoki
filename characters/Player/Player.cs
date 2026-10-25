using Godot;

public partial class Player : CharacterBody2D
{
    [Export] MovementComponent Movement { get; set; }
    public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).

    public Vector2 ScreenSize; // Size of the game window.
    public AnimatedSprite2D animatedSprite2D;

    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;

        Movement ??= new MovementComponent();
        AddChild(Movement);
        Movement.Body = this;
		
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        // Reads WASD movement using the configured move_* actions
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Movement.Move(inputDir, Speed);

        // Handle Shooting input
        if (Input.IsActionJustPressed("shoot"))
        {
            Shoot();
        }

        // Animation Logic
        if (Movement.Body.Velocity.X != 0)
        {
            animatedSprite2D.Animation = "default";
            animatedSprite2D.FlipV = false;
            animatedSprite2D.FlipH = Movement.Body.Velocity.X < 0;
            animatedSprite2D.Play();
        }
        else if (Movement.Body.Velocity.Y != 0)
        {
            animatedSprite2D.Animation = "default";
            animatedSprite2D.Play();
        }
        else
        {
            animatedSprite2D.Stop();
        }
    }

    private void Shoot()
    {
        // Place your projectile instantiation or shoot logic here
        GD.Print("Shoot action triggered!");
    }
}