using System.Collections.Generic;
using UnityEngine;

public enum Direction { North, South, East, West }

public class RoomSpawner : MonoBehaviour
{
    Gate Gate;

    [Header("Room Prefabs")]
    public List<Room> roomPrefabs;
    public List<Room> deadEndPrefabs;

    [Header("Spawn Settings")]
    public int minRooms = 4;
    public int maxRooms = 6;
    public Vector3 spawnOffset = new Vector3(-200, 0, -200);

    [Header("Room Size Settings")]
    public float roomWidth = 20f;
    public float roomLength = 20f;
    public float defaultRotationY = -180f;

    [Header("Entrance Gate (Home Base)")]
    public Teleport entranceTeleport;
    public Transform entranceTunnel;
    public Direction entranceDirection = Direction.East;

    private Dictionary<Direction, Direction> opposite = new Dictionary<Direction, Direction>()
    {
        { Direction.North, Direction.South },
        { Direction.South, Direction.North },
        { Direction.East,  Direction.West  },
        { Direction.West,  Direction.East  }
    };

    private Dictionary<Direction, Vector3> directionOffsets;
    private Dictionary<Vector3, Room> spawnedRooms = new Dictionary<Vector3, Room>();
    private Transform firstRoomGateTunnel;

    void Start()
    {
        Gate = gameObject.GetComponent<Gate>();
        directionOffsets = new Dictionary<Direction, Vector3>()
        {
            { Direction.North, new Vector3(0, 0, roomLength) },
            { Direction.South, new Vector3(0, 0, -roomLength) },
            { Direction.East,  new Vector3(roomWidth, 0, 0) },
            { Direction.West,  new Vector3(-roomWidth, 0, 0) }
        };
        Gate.SetSpawnedRooms(spawnedRooms);

    }
    public void RespawnRooms()
    {
        // Destroy all spawned rooms GameObjects
        foreach (var room in spawnedRooms.Values)
        {
            if (room != null)
                Destroy(room.gameObject);
        }

        spawnedRooms.Clear();
        firstRoomGateTunnel = null;

        GenerateRooms();
    }

    public void GenerateRooms()
    {
        int roomsToGenerate = Random.Range(minRooms, maxRooms + 1);
        Vector3 startPos = spawnOffset;

        // Spawn the first room connected to the gate on the opposite tunnel side
        Room startRoom = SpawnRoom(startPos, FindRoomWithTunnel(opposite[entranceDirection]), opposite[entranceDirection]);

        if (startRoom == null)
        {
            Debug.LogError("Failed to spawn start room");
            return;
        }

        spawnedRooms.Add(startPos, startRoom);

        // Get the tunnel in the startRoom connected back to the gate
        Transform startRoomTunnel = GetTunnelTransform(startRoom, opposite[entranceDirection]);
        firstRoomGateTunnel = startRoomTunnel;

        // Setup teleport connections between gate and first room
        if (entranceTeleport != null && entranceTunnel != null && startRoomTunnel != null)
        {
            if (startRoomTunnel.childCount > 0)
                entranceTeleport.teleportTo = startRoomTunnel.GetChild(0);
            else
                entranceTeleport.teleportTo = startRoomTunnel; // fallback

            Teleport backTeleport = startRoomTunnel.GetComponent<Teleport>();
            if (backTeleport != null)
            {
                if (entranceTunnel.childCount > 0)
                    backTeleport.teleportTo = entranceTunnel.GetChild(0);
                else
                    backTeleport.teleportTo = entranceTunnel; // fallback
            }
        }

        Queue<(Room room, Vector3 pos)> processingQueue = new Queue<(Room, Vector3)>();
        processingQueue.Enqueue((startRoom, startPos));

        int roomsSpawned = 1;

        while (processingQueue.Count > 0 && roomsSpawned < roomsToGenerate)
        {
            var (currentRoom, currentPos) = processingQueue.Dequeue();

            foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
            {
                Transform tunnel = GetTunnelTransform(currentRoom, dir);
                if (tunnel == null) continue;

                // Skip tunnel connected to gate to avoid spawning room there
                if (currentRoom == startRoom && tunnel == firstRoomGateTunnel)
                    continue;

                Vector3 nextPos = currentPos + directionOffsets[dir];

                if (spawnedRooms.ContainsKey(nextPos))
                    continue;

                Room nextRoom = FindRoomWithTunnel(opposite[dir]);

                // If close to max rooms or no room found, try dead end instead
                if (nextRoom == null || roomsSpawned + 1 >= roomsToGenerate)
                {
                    nextRoom = FindDeadEndWithTunnel(opposite[dir]);
                    if (nextRoom == null) continue;
                }

                Room spawnedRoom = SpawnRoom(nextPos, nextRoom, opposite[dir]);
                if (spawnedRoom == null) continue;

                spawnedRooms.Add(nextPos, spawnedRoom);
                roomsSpawned++;

                ConnectTeleport(currentRoom, dir, spawnedRoom);

                if (!IsDeadEndRoom(spawnedRoom))
                {
                    processingQueue.Enqueue((spawnedRoom, nextPos));
                }

                if (roomsSpawned >= roomsToGenerate)
                    break;
            }
        }

        AssignDeadEndsToUnconnectedTunnels();
    }

    Room SpawnRoom(Vector3 position, Room prefabRoom = null, Direction? entranceDir = null)
    {
        Room roomToSpawn = prefabRoom ?? GetRandomRoomPrefab();

        if (roomToSpawn == null)
        {
            Debug.LogError("No valid room prefab to spawn");
            return null;
        }

        // 
        // Apply fixed rotation of -180 degrees on the Y axis
        Quaternion fixedRotation = Quaternion.Euler(0, defaultRotationY, 0);

        GameObject go = Instantiate(roomToSpawn.gameObject, position, fixedRotation);

        Room spawnedRoom = go.GetComponent<Room>();
        if (spawnedRoom != null)
        {
            spawnedRoom.SetGate(Gate);
        }
        return spawnedRoom;
    }

    Room GetRandomRoomPrefab()
    {
        if (roomPrefabs.Count == 0) return null;
        return roomPrefabs[Random.Range(0, roomPrefabs.Count)];
    }

    Transform GetTunnelTransform(Room room, Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return room.northTunnel;
            case Direction.South: return room.southTunnel;
            case Direction.East: return room.eastTunnel;
            case Direction.West: return room.westTunnel;
            default: return null;
        }
    }

    Room FindRoomWithTunnel(Direction tunnelDir)
    {
        var shuffled = new List<Room>(roomPrefabs);
        Shuffle(shuffled);

        foreach (var room in shuffled)
        {
            if (GetTunnelTransform(room, tunnelDir) != null)
                return room;
        }
        return null;
    }

    Room FindDeadEndWithTunnel(Direction tunnelDir)
    {
        foreach (var room in deadEndPrefabs)
        {
            if (GetTunnelTransform(room, tunnelDir) != null)
                return room;
        }
        return null;
    }

    bool IsDeadEndRoom(Room room)
    {
        int count = 0;
        if (room.northTunnel != null) count++;
        if (room.southTunnel != null) count++;
        if (room.eastTunnel != null) count++;
        if (room.westTunnel != null) count++;
        return count == 1;
    }

    void ConnectTeleport(Room fromRoom, Direction fromDir, Room toRoom)
    {
        Transform fromTunnel = GetTunnelTransform(fromRoom, fromDir);
        Transform toTunnel = GetTunnelTransform(toRoom, opposite[fromDir]);

        if (fromTunnel == null || toTunnel == null) return;

        Teleport fromTp = fromTunnel.GetComponent<Teleport>();
        if (fromTp != null && fromTunnel != firstRoomGateTunnel)
        {
            fromTp.teleportTo = toTunnel;
        }

        Teleport backTp = toTunnel.GetComponent<Teleport>();
        if (backTp != null)
        {
            backTp.teleportTo = fromTunnel;
        }
    }

    void AssignDeadEndsToUnconnectedTunnels()
    {
        var roomsToAdd = new List<(Vector3 pos, Room room)>();

        foreach (var kvp in spawnedRooms)
        {
            Room room = kvp.Value;
            Vector3 roomPos = kvp.Key;

            foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
            {
                Transform tunnel = GetTunnelTransform(room, dir);
                if (tunnel == null) continue;

                Teleport tp = tunnel.GetComponent<Teleport>();
                if (tp == null || tp.teleportTo != null || tunnel == firstRoomGateTunnel)
                    continue;

                Vector3 deadEndPos = roomPos + directionOffsets[dir];
                if (!spawnedRooms.ContainsKey(deadEndPos))
                {
                    Room deadEndRoom = FindDeadEndWithTunnel(opposite[dir]);
                    if (deadEndRoom != null)
                    {
                        Room spawnedDeadEnd = SpawnRoom(deadEndPos, deadEndRoom, opposite[dir]);
                        if (spawnedDeadEnd != null)
                        {
                            roomsToAdd.Add((deadEndPos, spawnedDeadEnd));

                            Transform deadEndTunnel = GetTunnelTransform(spawnedDeadEnd, opposite[dir]);
                            tp.teleportTo = deadEndTunnel;
                            Teleport backTp = deadEndTunnel.GetComponent<Teleport>();
                            if (backTp != null)
                            {
                                backTp.teleportTo = tunnel;
                            }
                        }
                    }
                    else
                    {
                        tp.teleportTo = tunnel;
                    }
                }
                else
                {
                    tp.teleportTo = tunnel;
                }
            }
        }

        foreach (var item in roomsToAdd)
        {
            if (!spawnedRooms.ContainsKey(item.pos))
            {
                spawnedRooms.Add(item.pos, item.room);
            }
            else
            {
                Debug.LogWarning($"Attempted to add duplicate room at position {item.pos}");
            }
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}