using Godot;
using System;
using System.Collections;
public partial class dumb_enemy_script : CharacterBody2D
{
	// defining variables
	CharacterBody2D player;
	AnimationPlayer enemy_anim;
	TextureProgressBar healthbar;
	TextureProgressBar health;
	private NavigationAgent2D _navigationAgent;
	private float _movementSpeed = 200.0f;
	CollisionShape2D collider;
	bool knockback = false;
	AnimatedSprite2D sprite;
	AnimationPlayer animator;
	Timer hurt_timer;
	Boolean grouped;
	Area2D groupee;
	Random rnd;
	public Vector2 MovementTarget
	{
		get { return _navigationAgent.TargetPosition; }
		set { _navigationAgent.TargetPosition = value; }
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		/// initiate default values
		rnd = new Random();
		player = GetTree().Root.GetNode<CharacterBody2D>($"Main/Player");
		enemy_anim = GetNode<AnimationPlayer>("WeaponAnimation");
		healthbar = player.GetNode<TextureProgressBar>("Visuals_Container/Health_Bar_Container/Health_Bar");
		health = GetNode<TextureProgressBar>("Health_Bar_Container/Health_Bar");
		health.MaxValue = 100 * Upgrades.EnemyHealthMultiplier;
		health.Value = 100 * Upgrades.EnemyHealthMultiplier;
		_navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		collider = GetNode<CollisionShape2D>("CollisionShape2D");
		sprite = GetNode<AnimatedSprite2D>("AnimatedBaseSprite");
		animator = GetNode<AnimationPlayer>("AnimationPlayer");
		hurt_timer = GetTree().Root.GetNode<Timer>("Main/Player/Hurt_Timer");
		//sets clothes to random colours
		GetNode<AnimatedSprite2D>("AnimatedShirtSprite").Modulate = new Color((float)rnd.Next(0, 100) / 100, (float)rnd.Next(0, 100) / 100, (float)rnd.Next(0, 100) / 100);
		GetNode<AnimatedSprite2D>("AnimatedPantsSprite").Modulate = new Color((float)rnd.Next(0, 100) / 100, (float)rnd.Next(0, 100) / 100, (float)rnd.Next(0, 100) / 100);

		// These values need to be adjusted for the actor's speed
		// and the navigation layout.
		_navigationAgent.PathDesiredDistance = 4.0f;
		_navigationAgent.TargetDesiredDistance = 4.0f;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		ActorSetup();
		if (_navigationAgent.IsNavigationFinished())
		{
			return;
		}
		//calculating velocity to next navigation node
		Vector2 currentAgentPosition = GlobalTransform.Origin;
		Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();
		health.GetParent<Node2D>().GlobalRotation = 0;
		Vector2 newVelocity = Vector2.Zero;
		if ((nextPathPosition - currentAgentPosition).Normalized().X != 0 && (nextPathPosition - currentAgentPosition).Normalized().Y != 0)
		{
			newVelocity = (nextPathPosition - currentAgentPosition).Normalized();
		}
		newVelocity *= _movementSpeed;
		if (Position.DistanceTo(player.Position) <= 70)
		{
			newVelocity = Vector2.Zero;
		}
		if (knockback) // moves backwards when hit by player
		{
			newVelocity = -newVelocity * 1.5f;
		}
		//calculating which 4 directional animation to play while walking and attacking
		Vector2 look_position = player.GlobalPosition - GlobalPosition;
		float angle = look_position.Angle();
		float look_direction = Mathf.Round(angle / (Mathf.Pi / 2)) * (Mathf.Pi / 2);
		Vector2 aim_direction;
		aim_direction.X = MathF.Round(Mathf.Cos(look_direction));
		aim_direction.Y = MathF.Round(MathF.Sin(look_direction));
		if (aim_direction.X == -0)
		{
			aim_direction.X = 0;
		}
		if (aim_direction.Y == -0)
		{
			aim_direction.Y = 0;
		}
		animator.Play($"{aim_direction.X} {aim_direction.Y}");
		if (Position.DistanceTo(MovementTarget) <= 100)
		{
			newVelocity = Vector2.Zero;
			if (enemy_anim.CurrentAnimation == "")
			{
				enemy_anim.Play($"Enemy Attack {aim_direction.X} {aim_direction.Y}");
			}

		}
		Velocity = newVelocity;
		MoveAndSlide();
		Area2D AttackHitbox = GetNode<Area2D>("Hitbox_container");
		AttackHitbox.LookAt(player.Position);
		//remove node on death
		if (health.Value <= 0)
		{
			QueueFree();
		}
	}
	public void _on_hitbox_container_area_entered(Area2D area)
	{
		if (area.Name == "Player_hitbox_container")
		{
			//deals damage to player when player enters attack hitbox
			hit();
		}
	}
	void hit()
	{
		healthbar.Value -= 3 * Upgrades.EnemyDamageMultiplier;
		hurt_timer.Start();

	}
	private async void ActorSetup()
	{
		// Wait for the first physics frame so the NavigationServer can sync.
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

		// Now that the navigation map is no longer empty, set the movement target.
		MovementTarget = player.Position;
	}
	public void _on_health_bar_value_changed(float value)
	{
		//sets how long to be knocked back for
		knockback_delay();
	}
	private async void knockback_delay()
	{
		knockback = true;
		await ToSignal(GetTree().CreateTimer(0.05), "timeout");
		knockback = false;
	}
}
