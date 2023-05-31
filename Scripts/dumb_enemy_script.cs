using Godot;
using System;

public partial class dumb_enemy_script : CharacterBody2D
{
    CharacterBody2D player;
    AnimationPlayer enemy_anim;
    TextureProgressBar healthbar;
    TextureProgressBar health;
    private NavigationAgent2D _navigationAgent;
    private float _movementSpeed = 200.0f;
    CollisionShape2D collider;
    public Vector2 MovementTarget
    {
        get { return _navigationAgent.TargetPosition; }
        set { _navigationAgent.TargetPosition = value; }
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = GetTree().Root.GetNode<CharacterBody2D>($"Main/Player");
        enemy_anim = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
        healthbar = player.GetNode<TextureProgressBar>("Visuals_Container/Health_Bar_Container/Health_Bar");
        health = GetNode<TextureProgressBar>("Health_Bar_Container/Health_Bar");
        _navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        collider = GetNode<CollisionShape2D>("CollisionShape2D");

        // These values need to be adjusted for the actor's speed
        // and the navigation layout.
        _navigationAgent.PathDesiredDistance = 4.0f;
        _navigationAgent.TargetDesiredDistance = 4.0f;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        ActorSetup();
        Vector2 velocity = Velocity;
        Vector2 current = Vector2.Zero;
        LookAt(player.Position);
        if (_navigationAgent.IsNavigationFinished())
        {
            return;
        }

        Vector2 currentAgentPosition = GlobalTransform.Origin;
        Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();
		health.GetParent<Node2D>().GlobalRotation = 0;
        Vector2 newVelocity = (nextPathPosition - currentAgentPosition).Normalized();
        newVelocity *= _movementSpeed;
        if (Position.DistanceTo(player.Position) <= 70)
        {
            newVelocity = Vector2.Zero;
        }
        Velocity = newVelocity;

        MoveAndSlide();
        if (Position.DistanceTo(player.Position) <= 150)
        {
            enemy_anim.Play("Enemy_melee");
        }
        if (health.Value <= 0)
        {
            QueueFree();
        }
        collider.GlobalRotation = 0;
    }
    public void _on_hitbox_container_area_entered(Area2D area)
    {
        if (area.Name == "Player_hitbox_container")
        {
            healthbar.Value -= 5;
        }
    }
    private async void ActorSetup()
    {
        // Wait for the first physics frame so the NavigationServer can sync.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Now that the navigation map is no longer empty, set the movement target.
        MovementTarget = player.Position;
    }
}
