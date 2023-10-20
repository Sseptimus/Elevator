using Godot;
using System;
using System.Collections.Generic;
using static Godot.Input;
public partial class main_script : Node2D
{
	//declaring variables 
	public List<CharacterBody2D> enemies = new List<CharacterBody2D>();
	Timer level_timer;
	PackedScene dumb_enemy;
	PackedScene smart_enemy;
	Vector2 starting_pos;
	CharacterBody2D newEnemy;
	Label level_display;
	Label pause;
	CanvasLayer perkSelector;
	Vector2 pos2;
	Label menu;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//setting variabels
		GameManager.CurrentLevel = 0;
		dumb_enemy = (PackedScene)ResourceLoader.Load("res://Scenes/dumb_enemy_melee.tscn");
		smart_enemy = (PackedScene)ResourceLoader.Load("res://Scenes/smart_enemy_melee.tscn");
		level_timer = GetNode<Timer>("Level_timer");
		pause = GetTree().Root.GetNode<Label>("Main/PauseLayer/Pause");
		level_display = GetNode<Label>("Background/Label");
		perkSelector = GetNode<CanvasLayer>("PerkSelector");
		menu = GetNode<Label>("Menu");
		Input.MouseMode = MouseModeEnum.Visible;
		level_display.Text = "G";
		starting_pos.X = 850;
		starting_pos.Y = -66;
		pos2.X = 1151;
		pos2.Y = 901;
		GetTree().Paused = true;
		Upgrades.PlayerHealthMultiplier = 1;
		Upgrades.PlayerDamageMultiplier = 1;
		Upgrades.EnemyDamageMultiplier = 1;
		Upgrades.EnemyHealthMultiplier = 1;
		Upgrades.PlayerSpeedMultiplier = 1;
		perkSelector.Visible = true;
		GameManager.PlayerUpgrades = new List<UpgradeOption>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//checks for player pause input then either pauses or unpauses the scene
		if (Input.IsActionJustPressed("ui_cancel") && !GameManager.dead && !perkSelector.Visible)
		{
			if (!GetTree().Paused)
			{
				GetTree().Paused = true;
				Input.MouseMode = MouseModeEnum.Visible;
				pause.Visible = true;
			}
			else
			{
				GetTree().Paused = false;
				Input.MouseMode = MouseModeEnum.Captured;
				pause.Visible = false;
			}
		}

	}
	public async void level_switch()
	{
		//updates current level and resets timer between level
		spawn_enemies();
		level_timer.Start(GameManager.CurrentLevel + 1 * 5);
		await ToSignal(level_timer, "timeout");
		GameManager.CurrentLevel += 1;
		level_display.Text = GameManager.CurrentLevel.ToString();
		level_switch();
	}
	public void spawn_enemies()
	{
		//spanws enemies, calculating spawn location
		if (GameManager.CurrentLevel != 0)
		{
			for (int i = 0; i <= GameManager.CurrentLevel; i++)
			{
				CharacterBody2D newEnemy = (CharacterBody2D)dumb_enemy.Instantiate();
				Vector2 Spawn_pos = starting_pos;
				if (i % 2 == 0)
				{
					Spawn_pos.X += 200;
				}
				Spawn_pos.Y += i * -150;
				newEnemy.Position = Spawn_pos;
				enemies.Add(newEnemy);
				AddChild(newEnemy);
				newEnemy = (CharacterBody2D)dumb_enemy.Instantiate();
				Spawn_pos = starting_pos;
				if (i + 1 % 2 == 0)
				{
					Spawn_pos.X += 200;
				}
				Spawn_pos.Y += i + 1 * -150;
				newEnemy.Position = Spawn_pos;
				enemies.Add(newEnemy);
				AddChild(newEnemy);

			}
			for (int i = 0; i <= GameManager.CurrentLevel - 2; i++)
			{
				CharacterBody2D newEnemy = (CharacterBody2D)smart_enemy.Instantiate();
				Vector2 Spawn_pos = starting_pos;
				if (i % 2 == 0)
				{
					Spawn_pos.X += 200;
				}
				Spawn_pos.Y += i * -150 * (-100 * (GameManager.CurrentLevel * 2));
				newEnemy.Position = Spawn_pos;
				enemies.Add(newEnemy);
				AddChild(newEnemy);
			}
			try
			{
			}
			finally
			{

			}
		}

	}
	private void _on_quit_button_pressed()
	{
		//closes game when quit button pressed
		GetTree().Quit();
	}
	private void _on_perk_selector_visibility_changed()
	{
		if (!perkSelector.Visible)
		{
			//begins game after perk select
			level_switch();
		}
	}
	private void _on_button_pressed()
	{
		//restarts game on button press
		GetTree().ReloadCurrentScene();
	}
}
public class Upgrades
{
	// setting up upgrade modifiers
	public static double PlayerDamageMultiplier { get; set; } = 1;
	public static double PlayerHealthMultiplier { get; set; } = 1;
	public static double PlayerSpeedMultiplier { get; set; } = 1;
	public static double EnemyDamageMultiplier { get; set; } = 1;
	public static double EnemyHealthMultiplier { get; set; } = 1;

}
public class GameManager
{
	//sets up global variabels
	public static bool dead { get; set; } = false;
	public static int CurrentLevel { get; set; } = 0;
	public static List<UpgradeOption> PlayerUpgrades { get; set; } = new List<UpgradeOption>();
	public static int DamageDealt { get; set; } = 0;
	public static DateTime StartTime { get; set; }
	public static int Kills { get; set; } = 0;

}



