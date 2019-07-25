// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.4.39
// 

using Colyseus.Schema;

public class Player : Schema {
	[Type(0, "string")]
	public string sessionId = "";

	[Type(1, "int16")]
	public short seat = 0;
}

