using SimpleEventSystem;
using UnityEngine;

/*
 * When there are too many events, you need to start categorizing each event to a separate class as shown in the example below.
 * An alternative approach is to use robust naming scheme (Ex. Player_Jumped) and use partial classes.
 */

namespace SimpleEventSystem
{
    public static class Events
    {
        public static readonly PlayerEvents Player;
        public static readonly ItemEvents Item;
    }

    // You can put this in a separate script
    public class PlayerEvents
    {
        public static readonly GameEvent onJump = new();
        public static readonly GameEvent<float> onDamage = new();
    }

    public class ItemEvents
    {
        public static readonly GameEvent<GameObject> onPickup = new();
    }
}

/* Alternative approach using partial classes with event naming scheme.
 * We use underscores to help Intellisense filter the events based on their category.

// PlayerEvents.cs
public static partial class Events
{
    public static readonly GameEvent Player_Jumped = new();
    public static readonly GameEvent<float> Player_Damaged = new();
    
}

// ItemEvents.cs
public static partial class Events
{
    public static readonly GameEvent<GameObject> Item_Pickedup = new();
}

*/