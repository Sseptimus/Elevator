using Godot;
using System;
using System.Collections.Generic;
using System.Collections;

public partial class background_script : Control
{
	PointLight2D Light1;
	PointLight2D Light2;
	bool isFlickering = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Light1 = GetNode<PointLight2D>("PointLight2D");
		Light2 = GetNode<PointLight2D>("PointLight2D2");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Light1.GlobalRotationDegrees += 100 * (float)delta;
		if(!isFlickering){
			FlickeringLight();
		}
	}
	async void FlickeringLight(){
		isFlickering = true;
		Light2.Visible = false;
		Random rnd = new Random();
		float timeDelay = rnd.Next(1,15);
		timeDelay = timeDelay/10;
		await ToSignal(GetTree().CreateTimer(timeDelay),"timeout");
		Light2.Visible = true;
		timeDelay = rnd.Next(1,15);
		timeDelay = timeDelay/10;
		await ToSignal(GetTree().CreateTimer(timeDelay),"timeout");
		isFlickering = false;
	}
}
