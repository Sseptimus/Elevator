using Godot;
using System;

public partial class enemy_script : Area2D
{
	Area2D player;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetTree().Root.GetNode<Area2D>("Movement_Test/Player");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 current = Vector2.Zero;
		LookAt(player.Position);
		MoveLocalX(200*(float)delta);
		
	}
	public void _on_area_entered(Area2D area){
		if(area.Name == "attack_hitbox"){
			QueueFree();
		}
	}
}
