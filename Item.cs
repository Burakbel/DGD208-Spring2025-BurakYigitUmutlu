using System.Collections.Generic;

public class Item
{
	// Name of the item, to be displayed in the game
    public required string Name { get; set; }
	
	// Item type, determines which situations it can be used in
    public required ItemType Type { get; set; }
	
	// Which pets it can be used in
    public required List<PetType> CompatibleWith { get; set; }
	
	// Which stat of the pet the item affects
    public required PetStat AffectedStat { get; set; }
	
	// How much it affects
    public required int EffectAmount { get; set; }
	
	// How long it takes for the item to be used (you should use async to implement this
    public required float Duration { get; set; }
}