using Godot;
using System;

public partial class records_table : Control
{
	private void OnBackButtonPressed()
	{
		GetParent().GetNode<Control>("RecordsTable").Hide();
	}
}
