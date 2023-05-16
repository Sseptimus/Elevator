using Godot;
using System;

public partial class player_script : Area2D
{
	float speed = 400;
	AnimationPlayer attack_anim;
	String facing;
	Sprite2D sprite;
	TextureProgressBar health;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		attack_anim = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
		health = GetNode<TextureProgressBar>("Health_Bar_Container/Health_Bar");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 velocity = Vector2.Zero;
		if(Input.IsActionPressed("ui_up")){
			velocity.Y -= 1;
			RotationDegrees = -90;
		}if(Input.IsActionPressed("ui_down")){
			velocity.Y += 1;
			RotationDegrees = 90;
		}if(Input.IsActionPressed("ui_left")){
			velocity.X -= 1;
			RotationDegrees = 180;
		}if(Input.IsActionPressed("ui_right")){
			velocity.X += 1;
			RotationDegrees = 0;
		}
		velocity = velocity.Normalized() * speed;
		velocity.X *= (float)delta;
		velocity.Y *= (float)delta;
		Position += velocity;
		if(Input.IsActionPressed("Attack")){
			attack_anim.Play("Player_melee");
		}
		if(health.Value<=0){
			QueueFree();
			GetTree().Quit();
		}
		health.GetParent<Node2D>().GlobalRotationDegrees = 0;
	}
	public void _on_attack_hitbox_container_area_entered(Area2D area){
		if(area.Name == "Enemy"){
			GetTree().Root.GetNode<TextureProgressBar>($"Movement_Test/{area.Name}/Health_Bar_Container/Health_Bar").Value -= 10;
		}
	}
}
