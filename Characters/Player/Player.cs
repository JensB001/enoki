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
		// animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Movement.Move(Input.GetVector("move_left", "move_right", "move_up", "move_down"), Speed);

		// if (velocity.X != 0)
		// {
		// 	animatedSprite2D.Animation = "default";
		// 	animatedSprite2D.FlipV = false;
		// 	// See the note below about the following boolean assignment.
		// 	animatedSprite2D.FlipH = velocity.X > 0;
		// }
		// else if (velocity.Y != 0)
		// {
		// 	animatedSprite2D.Animation = "default";
		// }
	}
}
