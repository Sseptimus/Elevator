using Godot;
using System;

public partial class enemy_script : Area2D
{
	Area2D player;
	AnimationPlayer enemy_melee;
	TextureProgressBar healthbar;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetTree().Root.GetNode<Area2D>("Movement_Test/Player");
		enemy_melee = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
		healthbar = player.GetChild<TextureProgressBar>(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 current = Vector2.Zero;
		LookAt(player.Position);
		MoveLocalX(200*(float)delta);
		if(Position.DistanceTo(player.Position)<=300){
			enemy_melee.Play("Enemy_melee");
		}
	}
	public void _on_hitbox_container_area_entered(Area2D area){
		if(area.Name == "Player"){
			healthbar.Value -= 50;
		}
	}
}
