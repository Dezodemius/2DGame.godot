using Godot;
using System;

public partial class player : Area2D
{
	[Export]
	public int Speed { get; set; } = 400;
	
	public Vector2 ScreenSize;
}
