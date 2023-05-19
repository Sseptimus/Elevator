using Godot;
using System;

public partial class player_script : CharacterBody2D
{
	[Export]
	public float Speed = 300.0f;
	
	AnimationPlayer anim;
	String facing;
	Sprite2D sprite;
	TextureProgressBar health;
	Vector2 ScreenSize;
	bool attack_waiting = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		anim = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
		health = GetNode<TextureProgressBar>("Health_Bar_Container/Health_Bar");
		ScreenSize = GetViewportRect().Size;
	}

	public override void _PhysicsProcess(double delta)
	{
		
		Vector2 velocity = Velocity;
		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity = direction * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(velocity.Y,0,Speed);
		}
		switch(direction.Y){
			case 1:
			RotationDegrees = 90;
			break;
			case -1:
			RotationDegrees = -90;
			break;
		}switch(direction.X){
			case 1:
			RotationDegrees = 0;
			break;
			case -1:
			RotationDegrees = 180;
			break;
		}
		
		if(Input.IsActionPressed("Attack")){
			if(!attack_waiting && anim.CurrentAnimation == ""){
				anim.Play("Player_melee");
				attack_delay();
			}
			
		}
		if(Input.IsActionPressed("Block")){
			if(anim.CurrentAnimation == ""){
				anim.Play("Player_block");
			}
		}
		if(Input.IsActionPressed("Dash")){
			if(anim.CurrentAnimation == ""){
				anim.Play("Player_dash");
			}
		}
		Velocity = velocity;
		MoveAndSlide();
			
		
		if(health.Value<=0){
			QueueFree();
			GetTree().Quit();
		}
		health.GetParent<Node2D>().GlobalRotationDegrees = 0;
	}
	async void attack_delay(){
		attack_waiting = true;
		await ToSignal(GetTree().CreateTimer(3), "timeout");
		attack_waiting = false;	
	}	
	public void _on_attack_hitbox_container_area_entered(Area2D area){
		if(area.Name == "Enemy"){
			GetTree().Root.GetNode<TextureProgressBar>($"Movement_Test/{area.Name}/Health_Bar_Container/Health_Bar").Value -= 10;
		}
	}
}
/*
using Godot;
using System;

public partial class player_script : CharacterBody2D
{
	float speed = 400;
	AnimationPlayer anim;
	String facing;
	Sprite2D sprite;
	TextureProgressBar health;
	Vector2 ScreenSize;
	bool attack_waiting = false;
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		
		Vector2 inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Velocity = inputDirection * speed;
		MoveAndSlide();
		if(Input.IsActionPressed("Attack")){
			if(!attack_waiting && anim.CurrentAnimation == ""){
				anim.Play("Player_melee");
				attack_delay();
			}
			
		}
		if(Input.IsActionPressed("Block")){
			if(anim.CurrentAnimation == ""){
				anim.Play("Player_block");
			}
		}
		if(Input.IsActionPressed("Dash")){
			if(anim.CurrentAnimation == ""){
				anim.Play("Player_dash");
				
			}
		}
			
		
		if(health.Value<=0){
			QueueFree();
			GetTree().Quit();
		}
		health.GetParent<Node2D>().GlobalRotationDegrees = 0;
	}
	
	
}
*/
