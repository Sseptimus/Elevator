using Godot;
using System;

public partial class enemy_script : Area2D
{
	CharacterBody2D player;
	AnimationPlayer enemy_melee;
	TextureProgressBar healthbar;
	TextureProgressBar health;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetTree().Root.GetNode<CharacterBody2D>("Movement_Test/Player");
		enemy_melee = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
		healthbar = player.GetNode<TextureProgressBar>("Health_Bar_Container/Health_Bar");
		health = GetNode<TextureProgressBar>("Health_Bar_Container/Health_Bar");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 current = Vector2.Zero;
		LookAt(player.Position);
		health.GetParent<Node2D>().GlobalRotationDegrees = 0;
		
		if(Position.DistanceTo(player.Position)<=130){
			enemy_melee.Play("Enemy_melee");
		}
		if(Position.DistanceTo(player.Position)<=75){
			MoveLocalX(-400*(float)delta);
		}else{
			MoveLocalX(200*(float)delta);
		}
			

	}
	public void _on_hitbox_container_area_entered(Area2D area){
		if(area.Name == "Player_hitbox_container"){
			healthbar.Value -= 50;
		}
	}
}
