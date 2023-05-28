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
    public bool quick_attack_waiting = false;
    public bool power_attack_waiting = false;
	bool dash_waiting = false;
	float look_direction;
	Vector2 look_position;
	Timer iframe_timer;
	Sprite2D mouse;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        anim = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
        health = GetNode<TextureProgressBar>("Health_Bar_Container/Health_Bar");
        ScreenSize = GetViewportRect().Size;
		iframe_timer = GetNode<Timer>("Iframe_timer");
		mouse = GetTree().Root.GetNode<Sprite2D>("Main/Mouse");
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
            velocity.Y = Mathf.MoveToward(velocity.Y, 0, Speed);
        }
        

        if (Input.IsActionPressed("Power_Attack"))
        {
            if (!power_attack_waiting && anim.CurrentAnimation == "")
            {
                anim.Play("Player_power_melee");
                power_attack_delay();
            }

        }
        if (Input.IsActionPressed("Quick_Attack"))
        {
            if (!quick_attack_waiting && anim.CurrentAnimation == "")
            {
                anim.Play("Player_quick_melee");
                quick_attack_delay();
            }
        }
        if (Input.IsActionPressed("Block"))
        {
            if (anim.CurrentAnimation == "")
            {
                anim.Play("Player_block");
            }
        }
        if (Input.IsActionPressed("Dash"))
        {
            if (!dash_waiting && anim.CurrentAnimation == "")
            {
                anim.Play("Player_dash");
				dash_delay();
            }
        }
        Velocity = velocity;
        MoveAndSlide();
		if(Position.DistanceTo(mouse.Position)>=45){
			look_position = mouse.Position - Position;
			if(Mathf.Abs(look_position.Y)>Mathf.Abs(look_position.X)){
				if(look_position.Y > 0){
					look_direction = 90;
				}else{
					look_direction = -90;}
			}else{
				if(look_position.X > 0){
					look_direction = 0;
				}else{
					look_direction = 180;
				}
			}
		}        
		if (health.Value <= 0)
        {
            QueueFree();
            GetTree().Quit();
        }
        health.GetParent<Node2D>().GlobalRotationDegrees = 0;
		GlobalRotationDegrees = look_direction;
    }
    async void power_attack_delay()
    {
        power_attack_waiting = true;
        await ToSignal(GetTree().CreateTimer(2), "timeout");
        power_attack_waiting = false;
    }
    async void quick_attack_delay()
    {
        quick_attack_waiting = true;
        await ToSignal(GetTree().CreateTimer(1), "timeout");
        quick_attack_waiting = false;
    }
	async void dash_delay(){
		dash_waiting = true;
		await ToSignal(GetTree().CreateTimer(1),"timeout");
		dash_waiting = false;
	}
    public void _on_quick_attack_hitbox_container_area_entered(Area2D area)
    {
        if (area.Name == "Enemy_hitbox_container")
        {
            GetTree().Root.GetNode<TextureProgressBar>($"Main/{area.GetParent().Name}/Health_Bar_Container/Health_Bar").Value -= 10;
        }
    }
    public void _on_power_attack_hitbox_container_area_entered(Area2D area)
    {
        if (area.Name == "Enemy_hitbox_container")
        {
            GetTree().Root.GetNode<TextureProgressBar>($"Main/{area.GetParent().Name}/Health_Bar_Container/Health_Bar").Value -= 30;
        }
    }
	async void iframe(){
		GetNode<CollisionShape2D>("Player_hitbox_container/Player_hitbox").Disabled = true;
		iframe_timer.Start();
		await ToSignal(iframe_timer, "timeout");
		GetNode<CollisionShape2D>("Player_hitbox_container/Player_hitbox").Disabled = false;
	}
	public void hit(int damage){
		health.Value -=  damage;

	}

}
