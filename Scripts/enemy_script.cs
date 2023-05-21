using Godot;
using System;

public partial class enemy_script : CharacterBody2D
{
	CharacterBody2D player;
	AnimationPlayer enemy_anim;
	TextureProgressBar healthbar;
	TextureProgressBar health;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetTree().Root.GetNode<CharacterBody2D>($"Main/Player");
		enemy_anim = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
		healthbar = player.GetNode<TextureProgressBar>("Health_Bar_Container/Health_Bar");
		health = GetNode<TextureProgressBar>("Health_Bar_Container/Health_Bar");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 current = Vector2.Zero;
		LookAt(player.Position);
		health.GetParent<Node2D>().GlobalRotationDegrees = 0;
		Vector2 direction = GlobalPosition.DirectionTo(player.Position);
		Vector2 reverse_direction = GlobalPosition.DirectionTo(-player.Position);
		if(Position.DistanceTo(player.Position)<=130){
			enemy_anim.Play("Enemy_melee");
		}
		if(Position.DistanceTo(player.Position)<=50){
			velocity = -reverse_direction*2000;
		}else{
			velocity = direction*20000;
		}
		velocity.X = velocity.X * (float)delta;
		velocity.Y = velocity.Y * (float)delta;
		Velocity = velocity;
		MoveAndSlide();
		
	}
	public void _on_hitbox_container_area_entered(Area2D area){
		if(area.Name == "Player_hitbox_container"){
			healthbar.Value -= 50;
		}
	}
}
