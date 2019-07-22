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

	[Type(1, "number")]
	public float playerTurn = 0;

	[Type(2, "number")]
	public float winningPlayer = 0;

	[Type(3, "map", typeof(MapSchema<Ship>))]
	public MapSchema<Ship> player1Ships = new MapSchema<Ship>();

	[Type(4, "map", typeof(MapSchema<Ship>))]
	public MapSchema<Ship> player2Ships = new MapSchema<Ship>();

	[Type(5, "array", "string")]
	public ArraySchema<string> player1Shots = new ArraySchema<string>();

	[Type(6, "array", "string")]
	public ArraySchema<string> player2Shots = new ArraySchema<string>();
}

