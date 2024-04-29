using System;
using System.Text;
using System.Threading;
using Godot;
using Timer = Godot.Timer;

public partial class HUD : CanvasLayer
{
	[Signal]
	public delegate void StartGameEventHandler();

	public void ShowMessage(string text)
	{
		var message = GetNode<Label>("Message");
		message.Text = text;
		message.Show();

		GetNode<Timer>("MessageTimer").Start();
	}

	public async void ShowGameOver()
	{
		ShowMessage("Game Over");
		GetNode<Label>("ScoreLabel").Hide();
		GetNode<Label>("RecordLabel").Hide();

		var messageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(messageTimer, Timer.SignalName.Timeout);

		var message = GetNode<Label>("Message");
		message.Text = "Dodge the Creeps!";
		message.Show();

		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		GetNode<Button>("StartButton").Show();
		GetNode<Button>("ShowRecordsButton").Show();
	}
	public void OnNewGame()
	{
		UpdateScore(0);
		ShowMessage("Get Ready!");
	}

	public void UpdateScore(int score)
	{
		GetNode<Label>("ScoreLabel").Text = score.ToString();
	}

	public void UpdateRecord(int record)
	{
		GetNode<Label>("RecordLabel").Text = $"record: {record}";
		//string json = Newtonsoft.Json.JsonConvert.SerializeObject(new UserRecord("test", record));
		//string[] headers = new string[] { "Content-Type: application/json" };
		//HttpRequest httpRequest = GetNode<HttpRequest>("UpdateRecordRequest");
		//httpRequest.RequestCompleted += HttpRequestOnRequestCompleted;
		//string body = Json.Stringify(new Godot.Collections.Dictionary
		//{
			//{ "userName", "Godette" },
			//{ "record", 111 }
		//});
		//var error = httpRequest.Request("http://localhost:5056/Records/?userName=egor&record=22", headers, HttpClient.Method.Post, "{\"userName\":\"egor\",\"record\":22}");
		//if (error != Error.Ok)
		//{
			//GD.PushError("An error occurred in the HTTP request.");
		//}
	}

	private void HttpRequestOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		GD.Print(responsecode);
		GD.Print(Encoding.UTF8.GetString(body));
	}

	public void HideRecordLabel()
	{
		GetNode<Label>("RecordLabel").Hide();
	}

	private void OnStartButtonPressed()
	{
		GetNode<Button>("StartButton").Hide();
		GetNode<Button>("ShowRecordsButton").Hide();
		EmitSignal(SignalName.StartGame);
		GetNode<Label>("RecordLabel").Show();
		GetNode<Label>("ScoreLabel").Show();
	}

	private void OnRecordsButtonPressed()
	{
		GetNode<Button>("StartButton").Hide();
		GetNode<Button>("ShowRecordsButton").Hide();
		GetNode<Label>("Message").Hide();
	
	  GetNode<Control>("RecordsTable").Show();
	}

	private void OnMessageTimerTimeout()
	{
		GetNode<Label>("Message").Hide();
	}
}

public class UserRecord
{
	public string UserName { get; set; }
	public int RecordValue { get; set; }

	public UserRecord(string userName, int value)
	{
		UserName = userName;
		RecordValue = value;
	}
}
