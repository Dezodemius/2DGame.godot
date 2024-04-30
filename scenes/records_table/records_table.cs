using Godot;

public partial class records_table : Control
{
	private void OnBackButtonPressed()
	{
		GetParent().GetNode<Control>("RecordsTable").Hide();
		GetParent().GetNode<CanvasLayer>("MainMenu").Show();
	}
}
