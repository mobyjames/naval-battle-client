// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.4.39
// 

using Colyseus.Schema;

public class Ship : Schema {
	[Type(0, "string")]
	public string name = "";

	[Type(1, "number")]
	public float size = 0;

	[Type(2, "number")]
	public float health = 0;

	[Type(3, "string")]
	public string status = "";
}

