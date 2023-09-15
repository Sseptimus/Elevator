using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;


public partial class perk : Label
{
	UpgradeOption GlassCannon;
	List<UpgradeOption> upgrades = new List<UpgradeOption>();
	Random rnd;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		upgrades.Add(GlassCannon = new UpgradeOption(0.00001,10000,0.00001,1, optionImage: "res://Assets/Img/Option 1.png"));
		rnd = new Random();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	public void _on_visibility_changed(){
		GD.Print("a");
		if(Visible){
			UpgradeOption option1 = upgrades[rnd.Next(upgrades.Count)];
			GetNode<TextureButton>("TextureButton1").TextureNormal = (CompressedTexture2D)ResourceLoader.Load("res://.godot/imported/Option 1.png-8ad48309397fc021c613f79bb65072f9.ctex");
			GetNode<TextureButton>("TextureButton2").TextureNormal = (CompressedTexture2D)ResourceLoader.Load("res://.godot/imported/Option 1.png-8ad48309397fc021c613f79bb65072f9.ctex");
			GetNode<TextureButton>("TextureButton3").TextureNormal = (CompressedTexture2D)ResourceLoader.Load("res://.godot/imported/Option 1.png-8ad48309397fc021c613f79bb65072f9.ctex");
		}
	}
	private void _on_texture_button1_pressed(){
		
	}
}
public class UpgradeOption{
	public double PlayerHealthMultiplier;
	public double PlayerDamageMultiplier;
	public double EnemyHealthMultiplier;
	public double EnemyDamageMultiplier;
	public string OptionImage;
	public string OptionThumbnail;
	public UpgradeOption([Optional]double playerHealth,[Optional]double playerDamage,[Optional] double enemyHealth,[Optional]double enemyDamage,[Optional] string optionImage,[Optional] string thumbImage){
		PlayerHealthMultiplier = playerHealth;
		PlayerDamageMultiplier = playerDamage;
		EnemyHealthMultiplier = enemyHealth;
		EnemyDamageMultiplier = enemyDamage;
		OptionImage = optionImage;
		OptionThumbnail = thumbImage;
	}
}
