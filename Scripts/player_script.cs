using Godot;
using System;
using System.Collections;
public partial class player_script : CharacterBody2D
{
    //declaring variables
    [Export]
    public float Speed = 300.0f;
    AnimatedSprite2D sprite_anim;
    AnimationPlayer player_anim;
    AnimationPlayer action_anim;
    AnimationPlayer menu_anim;
    String facing;
    Sprite2D sprite;
    TextureProgressBar health;
    public bool power_attack_waiting = false;
    bool dash_waiting = false;
    Vector2 look_position;
    Vector2 aim_direction;
    Sprite2D mouse;
    Node2D visuals;
    TextEdit line;
    AudioStreamPlayer2D sound_player;
    Random rnd;
    Timer hurt_timer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //setting variables
        sprite_anim = GetNode<AnimatedSprite2D>("Visuals_Container/AnimatedSprite2D");
        player_anim = sprite_anim.GetNode<AnimationPlayer>("AnimationPlayer");
        action_anim = sprite_anim.GetNode<AnimationPlayer>("AnimationActions");
        menu_anim = GetTree().Root.GetNode<AnimationPlayer>("Main/AnimationPlayer");
        health = GetNode<TextureProgressBar>("Visuals_Container/Health_Bar_Container/Health_Bar");
        mouse = GetNode<Sprite2D>("Mouse");
        visuals = GetNode<Node2D>("Visuals_Container");
        hurt_timer = GetNode<Timer>("Hurt_Timer");
        sound_player = GetNode<AudioStreamPlayer2D>("Visuals_Container/AudioStreamPlayer2D");
        rnd = new Random();
        Modulate = new Color(1, 1, 1, 1);
    }

    public override void _PhysicsProcess(double delta)
    {
        //sets player colour to red if hit
        if (hurt_timer.TimeLeft != 0)
        {
            Modulate = new Color(1, 0, 0, 1);
        }
        Vector2 velocity = Velocity;
        // Get the input direction and handle the movement/deceleration.
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        if (direction != Vector2.Zero)
        {
            // Corrects for -0 anomaly
            if (Mathf.Round(aim_direction.X) == -0)
            {
                aim_direction.X = 0;
            }
            if (Mathf.Round(aim_direction.Y) == -0)
            {
                aim_direction.Y = 0;
            }
            // Calculates velocity
            velocity = direction * Speed*(int)Upgrades.PlayerSpeedMultiplier;
            // Runs corresponding animation to direction faced (fetched during input checks)
            if (player_anim.CurrentAnimation == "")
            {
                sprite_anim.Play($"{MathF.Round(aim_direction.X)} {MathF.Round(aim_direction.Y)}");
            }
        }
        else
        {
            // Removes current velocity from player if no input
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Y = Mathf.MoveToward(velocity.Y, 0, Speed);
            if (player_anim.CurrentAnimation == "")
            {
                sprite_anim.Stop();
            }
        }
        // Checks if player is facing the same way the player is moving and if not true reduces velocity
        if (Mathf.Round(direction.Angle() / (Mathf.Pi / 2)) * (Mathf.Pi / 2) != Mathf.Round(look_position.Angle() / (Mathf.Pi / 2)) * (Mathf.Pi / 2))
        {
            velocity *= (float)0.75;

        }
        //slows player down if not facing direction they are moving
        if (Mathf.Round(direction.Angle() / (Mathf.Pi / 2)) * (Mathf.Pi / 2) == Mathf.Round(look_position.Angle() / (Mathf.Pi / 2)) * (Mathf.Pi / 2) + Mathf.Pi || Mathf.Round(direction.Angle() / (Mathf.Pi / 2)) * (Mathf.Pi / 2) == Mathf.Round(look_position.Angle() / (Mathf.Pi / 2)) * (Mathf.Pi / 2) - Mathf.Pi)
        {
            sprite_anim.SpeedScale = -1;

        }
        else
        {
            sprite_anim.SpeedScale = 1;

        }
        //runs power attack animation on attack input
        if (Input.IsActionPressed("Power_Attack"))
        {
            if (!power_attack_waiting)
            {
                if (Mathf.Round(aim_direction.X) == -0)
                {
                    aim_direction.X = 0;
                }
                if (Mathf.Round(aim_direction.Y) == -0)
                {
                    aim_direction.Y = 0;
                }
                player_anim.Play($"Melee_{MathF.Round(aim_direction.X)} {MathF.Round(aim_direction.Y)}");
                power_attack_delay();
            }

        }
        //runs dash animation
        if (Input.IsActionPressed("Dash"))
        {
            if (!dash_waiting)
            {
                action_anim.Play("Player_Dash");
                dash_delay();
            }
        }
        Velocity = velocity;
        //runs collisions
        MoveAndSlide();
        //stops game on death
        if (health.Value <= 0 && !GetTree().Paused)
        {
            GetTree().Paused = true;
            Input.MouseMode = Input.MouseModeEnum.Visible;
            menu_anim.Play("Death");
        }
        health.GetParent<Node2D>().GlobalRotationDegrees = 0;
    }
    //runs cooldowns on attacks and dashes
    async void power_attack_delay()
    {
        power_attack_waiting = true;
        await ToSignal(GetTree().CreateTimer(1), "timeout");
        power_attack_waiting = false;
    }
    async void dash_delay()
    {
        dash_waiting = true;
        await ToSignal(GetTree().CreateTimer(1), "timeout");
        dash_waiting = false;
    }
    public void _on_power_attack_hitbox_container_area_entered(Area2D area)
    {
        //detects enemies hit and runs damage function
        if (area.Name == "Enemy_hitbox_container")
        {
            hit(30*(int)Upgrades.PlayerDamageMultiplier, area);

        }
    }
    public async void hit(int damage, Area2D area)
    {
        //deals hit damage and causes knockback on enemy + changes them to red
        GetTree().Root.GetNode<TextureProgressBar>($"Main/{area.GetParent().Name}/Health_Bar_Container/Health_Bar").Value -= damage;
        int num = rnd.Next(1, 4);
        AudioStreamWav crowbar_hit = (AudioStreamWav)ResourceLoader.Load($"res://Assets/Audio/Crowbar_hit_{num.ToString()}.wav");
        sound_player.Stream = crowbar_hit;
        sound_player.Playing = true;
        Color red = new Color(1, 0, 0, 1);
        Color saved_modulate = GetTree().Root.GetNode<CharacterBody2D>($"Main/{area.GetParent().Name}").Modulate;
        GetTree().Root.GetNode<CharacterBody2D>($"Main/{area.GetParent().Name}").Modulate = red;
        await ToSignal(GetTree().CreateTimer(0.2), "timeout");
        GetTree().Root.GetNode<CharacterBody2D>($"Main/{area.GetParent().Name}").Modulate = saved_modulate;

    }
    public void _on_hurt_timer_timeout()
    {
        //returns player to normal colour after hit
        Modulate = new Color(1, 1, 1, 1);
    }
    public override void _Input(InputEvent @event)
    {
        //sets direction the character is facing
        look_position = mouse.GlobalPosition - GlobalPosition;
        float angle = look_position.Angle();
        float look_direction = Mathf.Round(angle / (Mathf.Pi / 2)) * (Mathf.Pi / 2);
        aim_direction.X = Mathf.Cos(look_direction);
        aim_direction.Y = MathF.Sin(look_direction);
    }

}


