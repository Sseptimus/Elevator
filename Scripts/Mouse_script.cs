using Godot;
using System;
using static Godot.Input;
public partial class Mouse_script : Sprite2D
{
	Vector2 currentPosition;
	CharacterBody2D player;
	public bool waiting = false;
	TextEdit text;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = MouseModeEnum.Captured;
		player = GetTree().Root.GetNode<CharacterBody2D>("Main/Player");
		text = GetTree().Root.GetNode<TextEdit>("Main/TextEdit");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(GlobalPosition.DistanceTo(player.GlobalPosition)<=40){
			Visible = false;
		}
		else{
			Visible = true;
		}
		if(!waiting){
			currentPosition.X = Mathf.MoveToward(Position.X,0,25);
			currentPosition.Y = Mathf.MoveToward(Position.Y,0,25);
		}
		if(Position.DistanceTo(Vector2.Zero) > 100){
			currentPosition = Position.Normalized()*98;
		}
		
		Position = currentPosition;

	}
	public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseMotion mouseMotion){
			try{
				if(GetTree().Root.GetNode<Timer>("Main/timer") != null){
					GetTree().Root.GetNode<Timer>("Main/timer").QueueFree();
					waiting = false;
				}
			}catch (Exception){}
			currentPosition += mouseMotion.Relative;
			if(!waiting){
			mouse_delay();
			}
		}
    }
	async void mouse_delay(){
		waiting = true;
		await ToSignal(GetTree().CreateTimer(1),"timeout");
		waiting = false;
		
	}
}
