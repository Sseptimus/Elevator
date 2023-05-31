using Godot;
using System;
using System.Collections.Generic;
public partial class main_script : Node2D
{
	int current_level = 0;
	public List<CharacterBody2D> enemies;
	Timer level_timer;
	PackedScene dumb_enemy;
	PackedScene smart_enemy;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PackedScene dumb_enemy = (PackedScene)ResourceLoader.Load("res://Scenes/dumb_enemy_melee.tscn");
		PackedScene smart_enemy = (PackedScene)ResourceLoader.Load("res://Scenes/smart_enemy_melee.tscn");
		level_timer = GetNode<Timer>("Level_timer");
		level_switch();
		//enemies.Add((CharacterBody2D)dumb_enemy.Instantiate());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionPressed("ui_cancel")){
			GetTree().Quit();
		}
	}
	public async void level_switch(){
		level_timer.Start();
		await ToSignal(level_timer,"timeout");
		current_level += 1;
		spawn_enemies();
		level_switch();
	}
	void spawn_enemies(){
		for(int i = 0; i < current_level; i++){
			enemies.Add(dumb_enemy.Instantiate<CharacterBody2D>());
		}
		for(int i =0; i<current_level-1; i++){
			enemies.Add(smart_enemy.Instantiate<CharacterBody2D>());
		}
	}
}
