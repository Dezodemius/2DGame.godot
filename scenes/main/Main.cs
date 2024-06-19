using System.Net.Http;
using First2DGame;
using Godot;
using Newtonsoft.Json;
using HttpClient = Godot.HttpClient;

public partial class Main : Node
{
	[Export]
	public PackedScene MobScene { get; set; }

	private int _score;
	private int _record;

	private void GameOver()
	{
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		GetNode<HUD>("HUD").ShowGameOver();

		GetNode<AudioStreamPlayer>("Music").Stop();
		GetNode<AudioStreamPlayer>("DeathSound").Play();

		if (_score > _record)
		{
			_record = _score;
			GetNode<HUD>("HUD").UpdateRecord(_record);
		}
	}

	private void NewGame()
	{
		_score = 0;

		var player = GetNode<Player>("Player");
		var startPosition = GetNode<Marker2D>("StartPosition");
		player.Start(startPosition.Position);

		GetNode<Timer>("StartTimer").Start();

		GetNode<HUD>("HUD").OnNewGame();

		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);

		GetNode<AudioStreamPlayer>("Music").Play();
	}

	private void OnMobTimerTimeout()
	{
		Mob mob = MobScene.Instantiate<Mob>();

		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();

		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

		mob.Position = mobSpawnLocation.Position;

		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mob.Rotation = direction;

		var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction);

		AddChild(mob);
	}

	private void OnScoreTimerTimeout()
	{
		_score++;
		GetNode<HUD>("HUD").UpdateScore(_score);
	}

	private void OnStartTimerTimeout()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}

	private void OnReady()
	{
		var httpRequest = GetNode<HttpRequest>("RegisterUserRecordRequest");
		httpRequest.RequestCompleted += OnRequestCompleted;

		var headers = new string[] { "Content-Type: application/json" };
		var body = new UserScore(UserInfoManager.Instance.GetUserUniqueName(), 0);
		
		httpRequest.Request("http://localhost:5000/Records/AddOrUpdateUserRecord", 
			headers, 
			HttpClient.Method.Post, 
			JsonConvert.SerializeObject(body));
	}

	private void OnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		// throw new System.NotImplementedException();
	}
}
