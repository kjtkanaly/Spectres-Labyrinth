using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

// Perlin Noise Map Generation
public class WorldGeneration : MonoBehaviour
{
	public enum Mode
    {
		Perlin,
		Maze,
		Combo,
		PerlinRooms
    }
    
    public Transform PlayerTransform;
	public Tilemap FloorMap, WallMap;
	public Tile EmptySpaceTile;
	public Tile[] FloorTileSet;
	public RuleTile WallTile;
	
	// Vector 2: x = Width/Cols, y = Height/Rows
	// MapSize: The area that we will fill with noise
	// Offset:  Offsets the perlin cordinates
	// RoomsSizes: The dimensions of room footprints (height, width)
	public Mode       OperationMode    = Mode.Maze;
	public Vector2Int StartingRoomSize = new Vector2Int(30,30);
	private Vector2Int MapSize          = new Vector2Int(401, 301);
	private Vector2Int MapOffset        = new Vector2Int(0, 0);
	private Vector2Int RoomSizes        = new Vector2Int(20, 10);
	private Vector2Int PerlinOffset;
	
	// PerlinScale:       Used to scale my perlin coord
	// RoundCutOff: Used to round the perlin values
	public float PerlinScale = 20f;
	public float RoundCutOff = 0.5f;
	public int   MazeScale = 8;
	public int   TotalRooms = 3;
	public int   StartingHallwayDepth = 24;


	public void Start()
	{
	    // Place Starting Room
	    Vector2Int StartingRoomOrigin = new Vector2Int(MapSize.x/2 - StartingRoomSize.x/2, MapSize.y);
	    TileArea(StartingRoomOrigin, StartingRoomSize, FloorTileSet[0]);
	    	    
	    // Generate First Zone
	    PerlinOffset = new Vector2Int(Random.Range(-1000,1001), Random.Range(-1000, 1001));
	    GenerateZone(MapSize, MapOffset, PerlinOffset);
	    
	    // Setting the Hallway to connect the Starting Room to the Maze
	    Vector2Int StartingHallwayOrigin = new Vector2Int(MapSize.x/2 - MazeScale/2, MapSize.y - StartingHallwayDepth);
	    TileArea(StartingHallwayOrigin, new Vector2Int(MazeScale, StartingHallwayDepth), FloorTileSet[0]);
	    
	    // Setting the Walls
	    PlaceWallTiles(MapOffset, new Vector2Int(MapSize.x, MapSize.y + StartingRoomSize.y), WallTile);
		
		// Move Player to Starting Room
		PlayerTransform.position = new Vector2(StartingRoomOrigin.x + StartingRoomSize.x/2, StartingRoomOrigin.y + StartingRoomSize.y/2);

		//Test();

		/*
		string   FileData = System.IO.File.ReadAllText(path);
		String[] Lines    = fileData.Split("\n"[0]);
		String[] lineData = (lines[0].Trim()).Split(","[0]);*/
	}
	
	public void TileArea(Vector2Int ZoneOrigin, Vector2Int ZoneSize, Tile tile)
	{
	    // Placing Walls
	    for (int row = ZoneOrigin.y; row < ZoneOrigin.y + ZoneSize.y + 1; row++)
	    {
	        for (int col = ZoneOrigin.x; col < ZoneOrigin.x + ZoneSize.x + 1; col++)
	        {
	            FloorMap.SetTile(new Vector3Int(col, row), tile);
	        }
	    } 
	}
	
	public void PlaceWallTiles(Vector2Int ZoneOrigin, Vector2Int ZoneSize, RuleTile tile)
	{
	    // Placing Walls
	    for (int row = ZoneOrigin.y; row < ZoneOrigin.y + ZoneSize.y + 1; row++)
	    {
	        for (int col = ZoneOrigin.x; col < ZoneOrigin.x + ZoneSize.x + 1; col++)
	        {
	            if (FloorMap.GetTile(new Vector3Int(col, row)) == null)
	            {
	                WallMap.SetTile(new Vector3Int(col, row), tile);
	            }
	        }
	    } 
	}
	
	/*
	static void ReadString()
    {
        string path = "Assets/Resources/test.txt";
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path); 
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }
    */

    public void Update()
    {
    }

	// Lays floor tiles for a given area by calling perlin noise
	public void GenerateZone(Vector2Int ZoneSize, Vector2Int ZoneOffset, Vector2Int PerlinOffset)
	{
		if (OperationMode == Mode.Combo)
		{
			int[,] Maze = GenerateMaze(ZoneSize);

			int row = 0;

			while (row < ZoneSize.y / MazeScale)
			{
				int col = 0;

				while (col < ZoneSize.x / MazeScale)
				{
					for (int tileRow = row * MazeScale; tileRow < row * MazeScale + MazeScale; tileRow++)
					{
						for (int tileCol = col * MazeScale; tileCol < col * MazeScale + MazeScale; tileCol++)
						{
							int sample = CalcNoise(tileCol, tileRow);

							if (Maze[row,col] == 0)
                            {
								FloorMap.SetTile(new Vector3Int(tileCol + ZoneOffset.y, tileRow + ZoneOffset.x), FloorTileSet[0]);
							}

							if (sample == 0)
                            {
								FloorMap.SetTile(new Vector3Int(tileCol + ZoneOffset.y, tileRow + ZoneOffset.x), FloorTileSet[1]);
							}

							if (Maze[row,col] == 1 && sample == 1)
                            {
								WallMap.SetTile(new Vector3Int(tileCol + ZoneOffset.y, tileRow + ZoneOffset.x), WallTile);
                            }
						}
					}

					col += 1;
				}

				row += 1;
			}
		}
	}

	public int[,] GenerateMaze(Vector2Int MazeSize)
    {
		int[,] Maze = new int[MazeSize.y, MazeSize.x];

		// Setting the Seeds
		for (int row = 1; row < MazeSize.y; row+=2)
        {
			for (int col = 1; col < MazeSize.x; col+=2)
            {
				Maze[row, col] = 1;

				int dir = Random.Range(0, 4);
				
				switch (dir)
				{
					case 0:
						Maze[row + 1, col] = 1;
						break;
					case 1:
						Maze[row, col + 1] = 1;
						break;
					case 2:
						Maze[row - 1, col] = 1;
						break;
					case 3:
						Maze[row, col - 1] = 1;
						break;
				}
			}
        }

		return Maze;
    }

	public void setArrayRangeOfElements(int[,] array, int start, int end, int dir)
    {

    }

	// Returns rounded Perlin Noise Values: (0 -or- 1)
	public int CalcNoise(int x, int y)
	{
		// Perlin Coord x = 0 -> 0, x = width -> 1
		float xCoord = (float)x / MapSize.x * PerlinScale + PerlinOffset.x;
		float yCoord = (float)y / MapSize.y * PerlinScale + PerlinOffset.y;
		
		// Get the perlin value for the Coords
		float sample = Mathf.PerlinNoise(xCoord, yCoord);
		
		// Round the Perlin value
		if (sample >= RoundCutOff)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}


}