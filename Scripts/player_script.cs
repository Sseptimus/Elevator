using Godot;
using System;

public partial class player_script : Area2D
{
	float speed = 400;
	AnimationPlayer attack_anim;
	String facing;
	Sprite2D sprite;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		attack_anim = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
		//sprite = GetNode<Sprite2D>("Sprite2D");
		
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 velocity = Vector2.Zero;
		if(Input.IsActionPressed("ui_up")){
			velocity.Y -= 1;
			facing = "Up";
		}if(Input.IsActionPressed("ui_down")){
			velocity.Y += 1;
			facing = "Down";
		}if(Input.IsActionPressed("ui_left")){
			velocity.X -= 1;
			facing = "Left";
		}if(Input.IsActionPressed("ui_right")){
			velocity.X += 1;
			facing = "Right";
		}
		velocity = velocity.Normalized() * speed;
		velocity.X *= (float)delta;
		velocity.Y *= (float)delta;
		Position += velocity;
		if(Input.IsActionPressed("Attack")){
			attack_anim.Play($"Player_melee_{facing}");
		}
	}
	public void _on_attack_hitbox_container_area_entered(Area2D area){
		if(area.Name == "Enemy"){
			area.QueueFree();
		}
	public void attack(){

	}
	}
}
