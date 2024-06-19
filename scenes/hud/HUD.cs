using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Godot;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using Timer = Godot.Timer;

public partial class HUD : CanvasLayer
{
	[Signal]
	public delegate void StartGameEventHandler();

	private void ShowMessage(string text)
	{
		var message = GetNode("MainMenu").GetNode<Label>("Message");
		message.Text = text;
		message.Show();

		GetNode("MainMenu").GetNode<Timer>("MessageTimer").Start();
	}

	public async void ShowGameOver()
	{
		ShowMessage("Game Over");
		GetNode("MainMenu").GetNode<Label>("ScoreLabel").Hide();
		GetNode("MainMenu").GetNode<Label>("RecordLabel").Hide();

		var messageTimer = GetNode("MainMenu").GetNode<Timer>("MessageTimer");
		await ToSignal(messageTimer, Timer.SignalName.Timeout);

		var message = GetNode("MainMenu").GetNode<Label>("Message");
		message.Text = "Dodge the Creeps!";
		message.Show();

		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		GetNode("MainMenu").GetNode<Button>("StartButton").Show();
		GetNode("MainMenu").GetNode<Button>("ShowRecordsButton").Show();
	}
	public void OnNewGame()
	{
		UpdateScore(0);
		ShowMessage("Get Ready!");
	}

	public void UpdateScore(int score)
	{
		GetNode("MainMenu").GetNode<Label>("ScoreLabel").Text = score.ToString();
	}

	public void UpdateRecord(int record)
	{
		GetNode("MainMenu").GetNode<Label>("RecordLabel").Text = $"record: {record}";
		HttpRequest httpRequest = GetNode<Control>("RecordsTable")
			.GetNode<HttpRequest>("HTTPRequest");
		httpRequest.Request("http://localhost:5000/Records/AddOrUpdateUserRecord", 
		  null, 
			HttpClient.Method.Post,
		new UserScore("test", 123123).ToString());
	 
	}

	private void HideRecordLabel()
	{
	  GetNode("MainMenu").GetNode<Label>("RecordLabel").Hide();
	}

	private void OnStartButtonPressed()
	{
	  GetNode("MainMenu").GetNode<Button>("StartButton").Hide();
	  GetNode("MainMenu").GetNode<Button>("ShowRecordsButton").Hide();
	  EmitSignal(SignalName.StartGame);
	  GetNode("MainMenu").GetNode<Label>("RecordLabel").Show();
	  GetNode("MainMenu").GetNode<Label>("ScoreLabel").Show();
	}

	private void OnRecordsButtonPressed()
	{
	  HttpRequest httpRequest = GetNode<Control>("RecordsTable")
		.GetNode<HttpRequest>("HTTPRequest");
	  httpRequest.RequestCompleted += OnRequestCompleted;
	  httpRequest.Request("http://localhost:5000/Records/GetTopRecords");
	}
	
	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		using var stream = new MemoryStream(body);
		using var reader = new StreamReader(stream, Encoding.UTF8);
		var bodyStr = Encoding.UTF8.GetString(body);
		var records = JsonConvert.DeserializeObject<List<UserScore>>(bodyStr);
		
		GetNode<CanvasLayer>("MainMenu").Hide();
		GetNode<Control>("RecordsTable").Show();
	}

	private void OnMessageTimerTimeout()
	{
	  GetNode("MainMenu").GetNode<Label>("Message").Hide();
	}
}



public class UserScore
{
	public string name { get; set; }
	public int score { get; set; }

	public override bool Equals(object? obj)
	{
		if (obj is UserScore record)
			return record.name == name;
		return false;
	}

	protected bool Equals(UserScore other)
	{
		return name == other.name;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(name);
	}

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}

	public UserScore(string name, int score)
	{
		this.name = name;
		this.score = score;
	}
}
