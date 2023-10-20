using Godot;
using System;
using static Godot.Input;
public partial class Mouse_script : Sprite2D
{
	//defining variables
	Vector2 currentPosition;
	CharacterBody2D player;
	public bool waiting = false;
	TextEdit text;
	bool active_keyboard = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetParent<CharacterBody2D>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//faces mouse icon away from player
		GlobalRotation = player.GetAngleTo(GlobalPosition) + (Mathf.Pi / 2);
		//checks if a controller is being used
		if (IsActionPressed("Active_Controller"))
		{
			active_keyboard = false;
		}
		//hides and shows mouse based distance to player object
		if (GlobalPosition.DistanceTo(player.GlobalPosition) <= 40)
		{
			Visible = false;
		}
		else
		{
			Visible = true;
		}
		//clamps mouse to player
		if (currentPosition.DistanceTo(Vector2.Zero) > 60)
		{
			currentPosition = currentPosition.Normalized() * 58;
		}

		Position = currentPosition;

	}
	public override void _Input(InputEvent @event)
	{
		//moves mouse based on mouse/trackpad input
		if (@event is InputEventMouseMotion mouseMotion)
		{
			active_keyboard = true;
			if (GetTree().Root.HasNode("Main/Timer"))
			{
				GetTree().Root.GetNode<Timer>("Main/timer").QueueFree();
			}
			currentPosition += mouseMotion.Relative;
		}
		//moves mouse based on controller input
		if (@event.IsAction("Active_Controller"))
		{
			if (GetTree().Root.GetNode<Timer>("Main/timer") != null)
			{
				GetTree().Root.GetNode<Timer>("Main/timer").QueueFree();
			}
			var direction = Input.GetVector("view_left", "view_right", "view_up", "view_down");
			currentPosition += direction * 50;
		}
	}
}
