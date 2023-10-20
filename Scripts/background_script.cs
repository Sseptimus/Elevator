using Godot;
using System;
using System.Collections.Generic;
using System.Collections;

public partial class background_script : Control
{
	//declaring variables
	PointLight2D OverHeadLight;
	bool isFlickering = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//seting variables
		OverHeadLight = GetNode<PointLight2D>("PointLight2D2");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!isFlickering)
		{
			FlickeringLight();
		}
	}
	async void FlickeringLight()
	{// Flickers the main overhead light
		isFlickering = true;
		OverHeadLight.Visible = false;
		Random rnd = new Random();
		float timeDelay = rnd.Next(1, 15);
		timeDelay = timeDelay / 10;
		await ToSignal(GetTree().CreateTimer(timeDelay), "timeout");
		OverHeadLight.Visible = true;
		timeDelay = rnd.Next(1, 15);
		timeDelay = timeDelay / 10;
		await ToSignal(GetTree().CreateTimer(timeDelay), "timeout");
		isFlickering = false;
	}
}
