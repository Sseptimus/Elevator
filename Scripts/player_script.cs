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
    bool block_waiting = false;
	float look_direction;
	Vector2 look_position;
	Timer iframe_timer;
	Sprite2D mouse;
    Node2D visuals;
    TextEdit line;
    AudioStreamPlayer2D sound_player;
    Random rnd;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        anim = GetNode<AnimationPlayer>("Visuals_Container/Sprite2D/AnimationPlayer");
        health = GetNode<TextureProgressBar>("Visuals_Container/Health_Bar_Container/Health_Bar");
        ScreenSize = GetViewportRect().Size;
		iframe_timer = GetNode<Timer>("Iframe_timer");
		mouse = GetNode<Sprite2D>("Mouse");
        visuals = GetNode<Node2D>("Visuals_Container");
        line = GetTree().Root.GetNode<TextEdit>("Main/Level_Display");
        sound_player = GetNode<AudioStreamPlayer2D>("Visuals_Container/AudioStreamPlayer2D");
        rnd = new Random();
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
        if(Mathf.Round(direction.Angle()/(Mathf.Pi/2))*(Mathf.Pi/2) != Mathf.Round(look_position.Angle()/(Mathf.Pi/2))*(Mathf.Pi/2)){
            velocity *= (float)0.75;
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
            if (!block_waiting && anim.CurrentAnimation == "")
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
		await ToSignal(GetTree().CreateTimer(0.75),"timeout");
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
        var num = rnd.Next(1,4);
        GD.Print(num);
        AudioStreamWav crowbar_hit = (AudioStreamWav)ResourceLoader.Load($"res://Assets/Audio/Crowbar_hit_{num.ToString()}.wav");
        sound_player.Stream = crowbar_hit;
        sound_player.Playing = true;
        Color red = new Color(1, 0, 0, 1);
        GetTree().Root.GetNode<Sprite2D>($"Main/{area.GetParent().Name}/Sprite2D").Modulate = red;
        Color yellow = new Color(1, 1, 0, 1);
        await ToSignal(GetTree().CreateTimer(0.2),"timeout");
        GetTree().Root.GetNode<Sprite2D>($"Main/{area.GetParent().Name}/Sprite2D").Modulate = yellow;

	}
    public override void _Input(InputEvent @event)
    {
			look_position = mouse.GlobalPosition - GlobalPosition;
            float angle = look_position.Angle();
            look_direction = Mathf.Round(angle/(Mathf.Pi/4))*(Mathf.Pi/4);
            visuals.GlobalRotation = look_direction;
    }

}
