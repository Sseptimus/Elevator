using Godot;
using System;

public partial class menu : Node2D
{
	Sprite2D Shaft1;
	Sprite2D Shaft2;
	Sprite2D Shaft3;
	Vector2 DefaultVector;
	Vector2 StartingPos;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Shaft1 = GetNode<Sprite2D>("Shaft1");
		Shaft2 = GetNode<Sprite2D>("Shaft2");
		Shaft3 = GetNode<Sprite2D>("Shaft3");
		StartingPos.Y= -1077;
		StartingPos.X = Shaft1.Position.X;
		AnimatedSprite2D Waves = GetNode<AnimatedSprite2D>("Waves");
		Waves.Play("Waves");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		DefaultVector.Y = 3;
		if(Shaft1.Position.Y<1620){
			Shaft1.Position += DefaultVector;
		}else{
			Shaft1.Position = StartingPos;
		}
		if(Shaft2.Position.Y<1620){
			Shaft2.Position += DefaultVector;
		}else{
			Shaft2.Position = StartingPos;
		}
		if(Shaft3.Position.Y<1620){
			Shaft3.Position += DefaultVector;
		}else{
			Shaft3.Position = StartingPos;
		}
		
	}
	private void _on_button_pressed(){
		GetTree().ChangeSceneToFile("res://Scenes/main.tscn");

	}
}
