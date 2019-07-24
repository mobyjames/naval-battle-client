// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.4.39
// 

using Colyseus.Schema;

public class State : Schema {
	[Type(0, "string")]
	public string phase = "";

	[Type(1, "int16")]
	public short playerTurn = 0;

	[Type(2, "number")]
	public float winningPlayer = 0;

	[Type(3, "string")]
	public string player1 = "";

	[Type(4, "string")]
	public string player2 = "";

	[Type(5, "array", "int16")]
	public ArraySchema<short> player1Shots = new ArraySchema<short>();

	[Type(6, "array", "int16")]
	public ArraySchema<short> player2Shots = new ArraySchema<short>();
}

