using Godot;
using System;
using static Godot.Input;
public partial class Mouse_script : Sprite2D
{
	Vector2 currentPosition;
	CharacterBody2D player;
	public bool waiting = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = MouseModeEnum.Captured;
		player = GetTree().Root.GetNode<CharacterBody2D>("Main/Player");
		Position = player.Position;
		
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
			currentPosition.X = Mathf.MoveToward(Position.X,player.Position.X,25);
			currentPosition.Y = Mathf.MoveToward(Position.Y,player.Position.Y,25);
		}
		if(Mathf.Abs(currentPosition.X-player.Position.X) >= 100){
			currentPosition.X = player.Position.X + (100 * (currentPosition.X / - currentPosition.X));
		}
		if(Mathf.Abs(currentPosition.Y-player.Position.Y)>=100){
			currentPosition.Y = player.Position.Y + (100 * (currentPosition.Y / - currentPosition.Y));
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
			}catch (NullReferenceException){}
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
