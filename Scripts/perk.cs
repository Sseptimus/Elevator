using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using static Godot.Input;

public partial class perk : CanvasLayer
{
	UpgradeOption GlassCannon;
	UpgradeOption Nothing;
	UpgradeOption option1;
	UpgradeOption option2;
	UpgradeOption option3;
	List<UpgradeOption> upgrades = new List<UpgradeOption>();
	Random rnd;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		upgrades = new List<UpgradeOption>();
		upgrades.Add(GlassCannon = new UpgradeOption(1,100,1,10000, optionImage: "res://Assets/Img/Option_Thumbnails/Glass_Cannon_Thumbnail.png"));
		upgrades.Add(Nothing = new UpgradeOption(1,1,1,1,optionImage: "res://Assets/Img/Option 1.png"));
		rnd = new Random();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	public void _on_visibility_changed(){
		if(Visible){
			option1 = upgrades[rnd.Next(upgrades.Count)];
			option2 = upgrades[rnd.Next(upgrades.Count)];
			option3 = upgrades[rnd.Next(upgrades.Count)];
			GetNode<TextureButton>("TextureButton1").TextureNormal = (CompressedTexture2D)ResourceLoader.Load($"{option1.OptionImage}");
			GetNode<TextureButton>("TextureButton2").TextureNormal = (CompressedTexture2D)ResourceLoader.Load($"{option2.OptionImage}");
			GetNode<TextureButton>("TextureButton3").TextureNormal = (CompressedTexture2D)ResourceLoader.Load($"{option3.OptionImage}");
		}
	}
	public void _on_texture_button_1_pressed(){
		GameManager.PlayerUpgrades.Add(option1);
		UpgradeMultipliers();
		Visible = false;
		GetTree().Paused = false;
		Input.MouseMode = MouseModeEnum.Captured;
	}
	public void _on_texture_button_2_pressed(){
		GameManager.PlayerUpgrades.Add(option2);
		UpgradeMultipliers();
		Visible = false;
		GetTree().Paused = false;
		Input.MouseMode = MouseModeEnum.Captured;
	}
	public void _on_texture_button_3_pressed(){
		GameManager.PlayerUpgrades.Add(option3);
		UpgradeMultipliers();
		Visible = false;
		GetTree().Paused = false;
		Input.MouseMode = MouseModeEnum.Captured;
	}
	void UpgradeMultipliers(){
	if((int)GameManager.PlayerUpgrades[GameManager.PlayerUpgrades.Count-1].PlayerDamageMultiplier != 1){
		Upgrades.PlayerDamageMultiplier = GameManager.PlayerUpgrades[GameManager.PlayerUpgrades.Count-1].PlayerDamageMultiplier;
	}
	if(GameManager.PlayerUpgrades[GameManager.PlayerUpgrades.Count-1].PlayerHealthMultiplier != 1){
		Upgrades.PlayerHealthMultiplier = GameManager.PlayerUpgrades[GameManager.PlayerUpgrades.Count-1].PlayerHealthMultiplier;
	}
	if(GameManager.PlayerUpgrades[GameManager.PlayerUpgrades.Count-1].EnemyDamageMultiplier != 1){
		Upgrades.EnemyDamageMultiplier = GameManager.PlayerUpgrades[GameManager.PlayerUpgrades.Count-1].EnemyDamageMultiplier;
	}
	if(GameManager.PlayerUpgrades[GameManager.PlayerUpgrades.Count-1].EnemyHealthMultiplier != 0){
		Upgrades.EnemyHealthMultiplier = GameManager.PlayerUpgrades[GameManager.PlayerUpgrades.Count-1].EnemyHealthMultiplier;
	}
}
}

public class UpgradeOption{
	public double PlayerHealthMultiplier;
	public double PlayerDamageMultiplier;
	public double EnemyHealthMultiplier;
	public double EnemyDamageMultiplier;
	public double PlayerSpeedMultiplier;
	public string OptionImage;
	public string OptionThumbnail;
	public UpgradeOption([Optional]double playerHealth,[Optional]double playerDamage,[Optional] double enemyHealth,[Optional]double enemyDamage,[Optional]double playerSpeed, [Optional] string optionImage,[Optional] string thumbImage){
		PlayerHealthMultiplier = playerHealth;
		PlayerDamageMultiplier = playerDamage;
		EnemyHealthMultiplier = enemyHealth;
		EnemyDamageMultiplier = enemyDamage;
		PlayerSpeedMultiplier = playerSpeed;
		OptionImage = optionImage;
		OptionThumbnail = thumbImage;
	}
}
