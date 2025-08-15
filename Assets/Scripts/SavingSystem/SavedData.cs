using System;
using System.Collections.Generic;
using Grid.DataObjects;
using Rooms;
using UnityEngine;

[Serializable]
public class SavedData
{
    public bool playTutorial;
    public List<RoomContext> rooms;
    public List<TileContext> tiles;
}
