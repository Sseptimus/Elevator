using Godot;
using System;

public partial class Door_script : Node2D
{
	int objects_inside = 0;
	StaticBody2D left_door;
	StaticBody2D right_door;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		left_door = GetNode<StaticBody2D>("Left_Door_Container");
		right_door = GetNode<StaticBody2D>("Right_Door_Container");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 currentPositionLeft = Vector2.Zero;
		Vector2 currentPositionRight = Vector2.Zero;
		if(objects_inside == 0){
			currentPositionLeft.X = Mathf.MoveToward(left_door.Position.X,0,1);
			currentPositionRight.X = Mathf.MoveToward(right_door.Position.X,321,1);
		}else if(objects_inside != 0){
			currentPositionLeft.X = Mathf.MoveToward(left_door.Position.X,-323,1);
			currentPositionRight.X = Mathf.MoveToward(right_door.Position.X,644,1);
		}
		left_door.Position = currentPositionLeft;
		right_door.Position = currentPositionRight;

	}
	public void _on_area_2d_area_entered(Area2D area){
		if(area.Name == "Enemy_hitbox_container"){
			objects_inside += 1;
		}
	}
	public void _on_area_2d_area_exited(Area2D area){
		if(area.Name == "Enemy_hitbox_container"){
			objects_inside -= 1;
		}
	}
}
