using Godot;
using System;
using System.Collections.Generic;
public partial class main_script : Node2D
{
	int current_level = 1;
	public List<CharacterBody2D> enemies = new List<CharacterBody2D>();
	Timer level_timer;
	PackedScene dumb_enemy;
	PackedScene smart_enemy;
	Vector2 starting_pos;
	CharacterBody2D newEnemy;
	TextEdit level_display;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dumb_enemy = (PackedScene)ResourceLoader.Load("res://Scenes/dumb_enemy_melee.tscn");
		smart_enemy = (PackedScene)ResourceLoader.Load("res://Scenes/smart_enemy_melee.tscn");
		level_timer = GetNode<Timer>("Level_timer");
		level_display = GetNode<TextEdit>("Level_Display");
		level_display.Text = "G";
		starting_pos.X = 960;
		starting_pos.Y = -66;
		level_switch();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionPressed("ui_cancel")){
			GetTree().Quit();
		}
	}
	public async void level_switch(){
		level_timer.Start(current_level*5);
		await ToSignal(level_timer,"timeout");
		current_level += 1;
		level_display.Text = (current_level-1).ToString();
		spawn_enemies();
		level_switch();
	}
	public void spawn_enemies(){
		
		for(int i = 0; i < current_level-1; i++){
			CharacterBody2D newEnemy = (CharacterBody2D)dumb_enemy.Instantiate();
			newEnemy.Position = starting_pos;
			enemies.Add(newEnemy);
			AddChild(newEnemy);
			
			
		}
		for(int i =0; i<current_level-2; i++){
			CharacterBody2D newEnemy = (CharacterBody2D)smart_enemy.Instantiate();
			newEnemy.Position = starting_pos;
			enemies.Add(smart_enemy.Instantiate<CharacterBody2D>());
			AddChild(newEnemy);
		}
	}
}
