using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenTwo : MonoBehaviour
{
    
    public class node {
        
        public node Parent;
        public node Left;
        public node Right;
        public List<node> ConnectedNode;
        
        // Debug Variables
        public int level;
        
        public node(node parentValue = null, int levelValue = 0) {
            
            Parent = parentValue;
            level  = levelValue;
            
            Left = null;
            Right = null;
        }
    }
	
	////////////////////////////////////
	/// Variables and Parameters
	
	// RoomFootPrint.x = width, RoomFootPrint.y = height
	private Vector2Int RoomFootPrint = new Vector2Int(10,10);
	private int TreeLevels = 2;
	
	public Tilemap FloorTileMap;
	public Tile FloorTile;
	private node Parent;
    
    public void Start() { 

        Parent = createTree(0, TreeLevels, null);
        
        printTree(Parent);
		
		// Point the Parent Room is centered on
		new Vector2Int Origin = new Vector2Int(0, 0);
		
        GenerateRoomsFromTree(Parent, Origin);
    }
	
	public void GenerateRoomsFromTree(node Node, Vector2Int Origin) {
		
		SetTilesInTheGivenArea(Origin);
		
		if (Node.Left) 
		{
			Origin = new Vector2Int(Origin.x - Node.level * 10, Origin.y);
			
			GenerateRoomsFromTree(Node.Left, Origin);
		}
		if (Node.Right)
		{
			Origin = new Vector2Int(Origin.x + Node.level * 10, Origin.y);
			
			GenerateRoomsFromTree(Node.Right, Origin);
		}
	}
	
	public void SetTilesInTheGivenArea(Vector2Int Origin)
	{
		int width = RoomFootPrint.x;
		int height = RoomFootPrint.y;
		
		for (int row = -height/2 + Origin.y; row < height/2 + Origin.y; row++)
		{
			for (int col = -width/2 + Origin.x; col < width/2 + Origin.x; col++)
			{
				FloorTileMap.SetTile(row, col, FloorTile);
			}
		}
		
	}
    
    public node createTree(int currentLevel, int TreeLevels, node Parent) {
        
        node Node = new node(Parent, currentLevel);
        
        if (currentLevel < TreeLevels)
        {
            Node.Left = createTree(currentLevel + 1, TreeLevels, Node);
            Node.Right = createTree(currentLevel + 1, TreeLevels, Node);
        }
        else 
        {
            Node.Left = null;
            Node.Right = null;
        }
        
        return Node;
    }
    
    // Debug Function
    public void printTree(node Node)
    {
        Debug.Log(Node.level);
        
        if (Node.Left != null)
        {
            printTree(Node.Left);   
        }
        if (Node.Right != null)
        {
            printTree(Node.Right);
        }
    }
}