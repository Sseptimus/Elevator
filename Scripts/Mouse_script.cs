using Godot;
using System;
using static Godot.Input;
public partial class Mouse_script : Sprite2D
{
	Vector2 currentPosition;
	CharacterBody2D player;
	bool waiting = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = MouseModeEnum.Captured;
		player = GetTree().Root.GetNode<CharacterBody2D>("Main/Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Position.DistanceTo(player.Position)<=40){
			Visible = false;
		}
		else{
			Visible = true;
		}
		if(!waiting){
			currentPosition = player.Position;
		}
		Position = currentPosition;
	}
	public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseMotion mouseMotion){
			currentPosition += mouseMotion.Relative;
			mouse_delay();
		}
    }
	async void mouse_delay(){
		waiting = true;
		await ToSignal(GetTree().CreateTimer(1),"timeout");
		waiting = false;
	}
}
