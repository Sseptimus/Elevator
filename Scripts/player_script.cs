using Godot;
using System;
using System.Collections;
public partial class player_script : CharacterBody2D
{
    [Export]
    public float Speed = 300.0f;
    AnimatedSprite2D sprite_anim;
    AnimationPlayer player_anim;
    AnimationPlayer action_anim;
    String facing;
    Sprite2D sprite;
    TextureProgressBar health;
    Vector2 ScreenSize;
    public bool quick_attack_waiting = false;
    public bool power_attack_waiting = false;
	bool dash_waiting = false;
    bool block_waiting = false;
	Vector2 look_position;
    Vector2 aim_direction;
	Timer iframe_timer;
	Sprite2D mouse;
    Node2D visuals;
    TextEdit line;
    AudioStreamPlayer2D sound_player;
    Random rnd;
    Timer hurt_timer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite_anim = GetNode<AnimatedSprite2D>("Visuals_Container/AnimatedSprite2D");
        player_anim = sprite_anim.GetNode<AnimationPlayer>("AnimationPlayer");
        action_anim = sprite_anim.GetNode<AnimationPlayer>("AnimationActions");
        health = GetNode<TextureProgressBar>("Visuals_Container/Health_Bar_Container/Health_Bar");
        ScreenSize = GetViewportRect().Size;
		iframe_timer = GetNode<Timer>("Iframe_timer");
		mouse = GetNode<Sprite2D>("Mouse");
        visuals = GetNode<Node2D>("Visuals_Container");
        hurt_timer = GetNode<Timer>("Hurt_Timer");
        sound_player = GetNode<AudioStreamPlayer2D>("Visuals_Container/AudioStreamPlayer2D");
        rnd = new Random();
        Modulate = new Color(1, 1, 1, 1);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (hurt_timer.TimeLeft != 0){
            Modulate = new Color(1, 0, 0, 1);
        }
        Vector2 velocity = Velocity;
        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        if (direction != Vector2.Zero)
        {
            if(Mathf.Round(aim_direction.X) == -0 ){
                aim_direction.X = 0;
            }if(Mathf.Round(aim_direction.Y) == -0 ){
                aim_direction.Y = 0;
            }
            velocity = direction * Speed;
            if(player_anim.CurrentAnimation == ""){
            sprite_anim.Play($"{MathF.Round(aim_direction.X)} {MathF.Round(aim_direction.Y)}");
            }
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Y = Mathf.MoveToward(velocity.Y, 0, Speed);
            if(player_anim.CurrentAnimation == ""){
            sprite_anim.Stop();
            }
        }
        if(Mathf.Round(direction.Angle()/(Mathf.Pi/2))*(Mathf.Pi/2) != Mathf.Round(look_position.Angle()/(Mathf.Pi/2))*(Mathf.Pi/2)){
            velocity *= (float)0.75;
            
        }
        
        if(Mathf.Round(direction.Angle()/(Mathf.Pi/2))*(Mathf.Pi/2) == Mathf.Round(look_position.Angle()/(Mathf.Pi/2))*(Mathf.Pi/2) + Mathf.Pi || Mathf.Round(direction.Angle()/(Mathf.Pi/2))*(Mathf.Pi/2) == Mathf.Round(look_position.Angle()/(Mathf.Pi/2))*(Mathf.Pi/2) - Mathf.Pi){
            sprite_anim.SpeedScale = -1;
            
        }
        else{
            sprite_anim.SpeedScale = 1;

        }

        if (Input.IsActionPressed("Power_Attack"))
        {
            if (!power_attack_waiting)
            {
                if(Mathf.Round(aim_direction.X) == -0 ){
                aim_direction.X = 0;
                }if(Mathf.Round(aim_direction.Y) == -0 ){
                aim_direction.Y = 0;
                }
                player_anim.Play($"Melee_{MathF.Round(aim_direction.X)} {MathF.Round(aim_direction.Y)}");
                power_attack_delay();
            }

        }
        if (Input.IsActionPressed("Quick_Attack"))
        {
            if (!quick_attack_waiting)
            {
                sprite_anim.Play("Player_quick_melee");
                quick_attack_delay();
            }
        }
        if (Input.IsActionPressed("Block"))
        {
            if (!block_waiting)
            {
                sprite_anim.Play("Player_block");
                block_delay();
            }
        }
        if (Input.IsActionPressed("Dash"))
        {
            if (!dash_waiting)
            {
                action_anim.Play("Player_Dash");
				dash_delay();
            }
        }
        Velocity = velocity;
        MoveAndSlide();
		if (health.Value <= 0)
        {
            QueueFree();
            GetTree().Quit();
        }
        health.GetParent<Node2D>().GlobalRotationDegrees = 0;
    }
    async void power_attack_delay()
    {
        power_attack_waiting = true;
        await ToSignal(GetTree().CreateTimer(1), "timeout");
        power_attack_waiting = false;
    }
    async void quick_attack_delay()
    {
        quick_attack_waiting = true;
        await ToSignal(GetTree().CreateTimer(0.5), "timeout");
        quick_attack_waiting = false;
    }
	async void dash_delay(){
		dash_waiting = true;
		await ToSignal(GetTree().CreateTimer(1),"timeout");
		dash_waiting = false;
	}
    async void block_delay(){
        block_waiting = true;
        await ToSignal(GetTree().CreateTimer(20),"timeout");
        block_waiting = false;
    }
    public void _on_quick_attack_hitbox_container_area_entered(Area2D area)
    {
        if (area.Name == "Enemy_hitbox_container")
        {
            hit(10,area);
        }
    }
    public void _on_power_attack_hitbox_container_area_entered(Area2D area)
    {
        if (area.Name == "Enemy_hitbox_container")
        {
            hit(30,area);
            
        }
    }
	async void iframe(){
		GetNode<CollisionShape2D>("Visuals_Container/Player_hitbox_container/Player_hitbox").Disabled = true;
		iframe_timer.Start();
		await ToSignal(iframe_timer, "timeout");
		GetNode<CollisionShape2D>("Visuals_Container/Player_hitbox_container/Player_hitbox").Disabled = false;
	}
	public async void hit(int damage, Area2D area){
		GetTree().Root.GetNode<TextureProgressBar>($"Main/{area.GetParent().Name}/Health_Bar_Container/Health_Bar").Value -= damage;
        int num = rnd.Next(1,4);
        AudioStreamWav crowbar_hit = (AudioStreamWav)ResourceLoader.Load($"res://Assets/Audio/Crowbar_hit_{num.ToString()}.wav");
        sound_player.Stream = crowbar_hit;
        sound_player.Playing = true;
        Color red = new Color(1, 0, 0, 1);
        Color saved_modulate = GetTree().Root.GetNode<AnimatedSprite2D>($"Main/{area.GetParent().Name}/AnimatedSprite2D").Modulate;
        GetTree().Root.GetNode<AnimatedSprite2D>($"Main/{area.GetParent().Name}/AnimatedSprite2D").Modulate = red;
        await ToSignal(GetTree().CreateTimer(0.2),"timeout");
        GetTree().Root.GetNode<AnimatedSprite2D>($"Main/{area.GetParent().Name}/AnimatedSprite2D").Modulate = saved_modulate;

	}
    public void _on_hurt_timer_timeout(){
        Modulate = new Color(1,1,1,1);
    }
    public override void _Input(InputEvent @event)
    {
			look_position = mouse.GlobalPosition - GlobalPosition;
            float angle = look_position.Angle();
            float look_direction = Mathf.Round(angle/(Mathf.Pi/2))*(Mathf.Pi/2);
            aim_direction.X = Mathf.Cos(look_direction);
            aim_direction.Y = MathF.Sin(look_direction);
    }
    
}


