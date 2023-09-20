using Godot;
using System;
using System.Collections.Generic;
using static Godot.Input;
public partial class main_script : Node2D
{
	public List<CharacterBody2D> enemies = new List<CharacterBody2D>();
	Timer level_timer;
	PackedScene dumb_enemy;
	PackedScene smart_enemy;
	Vector2 starting_pos;
	CharacterBody2D newEnemy;
	Label level_display;
	CanvasLayer pause;
	CanvasLayer perkSelector;
	Vector2 pos2;
	Label menu;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameManager.CurrentLevel = 0;
		dumb_enemy = (PackedScene)ResourceLoader.Load("res://Scenes/dumb_enemy_melee.tscn");
		smart_enemy = (PackedScene)ResourceLoader.Load("res://Scenes/smart_enemy_melee.tscn");
		level_timer = GetNode<Timer>("Level_timer");
		pause = GetNode<CanvasLayer>("PauseLayer");
		level_display = GetNode<Label>("Background/Label");
		perkSelector = GetNode<CanvasLayer>("PerkSelector");
		menu = GetNode<Label>("Menu");
		Input.MouseMode = MouseModeEnum.Visible;
		level_display.Text = "G";
		starting_pos.X = 960;
		starting_pos.Y = -66;
		pos2.X = 1151;
		pos2.Y = 901;
		GetTree().Paused = true;
		Upgrades.PlayerHealthMultiplier = 1;
		Upgrades.PlayerDamageMultiplier = 1;
		Upgrades.EnemyDamageMultiplier = 1;
		Upgrades.EnemyHealthMultiplier = 1;
		Upgrades.PlayerSpeedMultiplier = 1;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	/*public override async void _Process(double delta)
	{
		if(enemies.Count > 30){
			GetTree().Paused = true;
			Label pause_label = GetNode<Label>("Pause");
			pause_label.Text = "The elevator is too heavy";
			pause_label.Visible = true;
			await ToSignal(GetTree().CreateTimer(5),"timeout");
			GetTree().Quit();
		}
		
	}*/
	public async void level_switch(){
		spawn_enemies();
		level_timer.Start(GameManager.CurrentLevel+1*5);
		await ToSignal(level_timer,"timeout");
		GameManager.CurrentLevel += 1;
		level_display.Text = GameManager.CurrentLevel.ToString();
		level_switch();
	}
	public void spawn_enemies(){
		if(GameManager.CurrentLevel != 0){
		for(int i = 0; i <= GameManager.CurrentLevel; i++){
			CharacterBody2D newEnemy = (CharacterBody2D)dumb_enemy.Instantiate();
			Vector2 Spawn_pos = starting_pos;
			starting_pos.X += i * 20;
			starting_pos.Y += (float)((double)(i/10))*10;
			newEnemy.Position = starting_pos;
			enemies.Add(newEnemy);
			AddChild(newEnemy);
			newEnemy = (CharacterBody2D)dumb_enemy.Instantiate();
			Spawn_pos = starting_pos;
			starting_pos.X += i * 20;
			starting_pos.Y += (float)((double)(i/10))*10;
			newEnemy.Position = starting_pos;
			enemies.Add(newEnemy);
			AddChild(newEnemy);
			
		}
		for(int i =0; i<=GameManager.CurrentLevel-2; i++){
			CharacterBody2D newEnemy = (CharacterBody2D)smart_enemy.Instantiate();
			Vector2 Spawn_pos = starting_pos;
			starting_pos.X += i * 10;
			starting_pos.Y += (float)((double)(i/10))*10-100;
			newEnemy.Position = starting_pos;
			enemies.Add(newEnemy);
			AddChild(newEnemy);
		}
		}
	}
	private void _on_quit_button_pressed()
	{
		GetTree().Quit();
	}
	private void _on_button_pressed()
	{
		menu.Visible = false;
		perkSelector.Visible = true;
	}
	private void _on_perk_selector_visibility_changed(){
		if(!perkSelector.Visible){
			level_switch();
		}
	}
	public override void _Input(InputEvent @event){
		if(@event.IsActionPressed("ui_cancel")){
			if(!GetTree().Paused){
				GetTree().Paused = true;
				Input.MouseMode = MouseModeEnum.Visible;
				pause.Visible = true;
			}else if(GetTree().Paused){
				GetTree().Paused = false;
				Input.MouseMode = MouseModeEnum.Captured;
				pause.Visible = false;
			}
		}
	}
}
public class Upgrades{
	public static double PlayerDamageMultiplier {get; set;} = 1;
	public static double PlayerHealthMultiplier {get; set;} = 1;
	public static double PlayerSpeedMultiplier{get; set;} = 1;
	public static double EnemyDamageMultiplier {get; set;} = 1;
	public static double EnemyHealthMultiplier {get; set;} = 1;
	
}
public class GameManager{
	public static int CurrentLevel {get; set;} = 0;
	public static List<UpgradeOption> PlayerUpgrades {get; set;} = new List<UpgradeOption>();
}



