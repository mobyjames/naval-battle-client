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

	[Type(2, "int16")]
	public short winningPlayer = 0;

	[Type(3, "map", typeof(MapSchema<Player>))]
	public MapSchema<Player> players = new MapSchema<Player>();

	[Type(4, "array", "int16")]
	public ArraySchema<short> player1Shots = new ArraySchema<short>();

	[Type(5, "array", "int16")]
	public ArraySchema<short> player2Shots = new ArraySchema<short>();
}

