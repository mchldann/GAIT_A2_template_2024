// Adapted from: https://github.com/SebLague/Pathfinding-2D

using UnityEngine;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour
{
	public static AStarGrid grid;
	static Pathfinding instance;
	public float waterCostMultiplier;

	// For path smoothing. (Note: You may need to add more variables here.)
	public bool UsePathSmoothing;

	void Awake()
	{
		grid = GetComponent<AStarGrid>();
		instance = this;
	}

	public static Node[] RequestPath(Vector2 from, Vector2 to)
	{
		return instance.FindPath(from, to);
	}

	Node[] FindPath(Vector2 from, Vector2 to)
	{
		Node[] waypoints = new Node[0];
		bool pathSuccess = false;
		
		Node startNode = grid.NodeFromWorldPoint(from);
		Node targetNode = grid.NodeFromWorldPoint(to);
		startNode.parent = startNode;

		if (!startNode.walkable)
		{
			startNode = grid.ClosestWalkableNode(startNode);
		}
		if (!targetNode.walkable)
		{
			targetNode = grid.ClosestWalkableNode(targetNode);
		}
		
		if (startNode.walkable && targetNode.walkable)
		{
			Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
			
			while (openSet.Count > 0)
			{
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);
				
				if (currentNode == targetNode)
				{
					pathSuccess = true;
					break;
				}
				
				foreach (Node neighbour in grid.GetNeighbours(currentNode))
				{
					if (!neighbour.walkable || closedSet.Contains(neighbour))
					{
						continue;
					}

					float newMovementCostToNeighbour = currentNode.gCost + GCost(currentNode, neighbour);

					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
					{
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = Heuristic(neighbour, targetNode);
						neighbour.parent = currentNode;
						
						if (!openSet.Contains(neighbour))
                        {
							openSet.Add(neighbour);
						}
						else
                        {
							openSet.UpdateItem(neighbour);
						}
					}
				}
			}
		}

		if (pathSuccess)
		{
			waypoints = RetracePath(startNode, targetNode);
		}

		if (UsePathSmoothing)
        {
			waypoints = SmoothPath(waypoints);
		}

		return waypoints;
	}
	
	Node[] RetracePath(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		
		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}

		Node[] waypoints = path.ToArray();
		Array.Reverse(waypoints);
		return waypoints;
	}

	private Node[] SmoothPath(Node[] path)
	{
		// TODO: Implement me properly!
		return path;
	}

	private float GCost(Node nodeA, Node nodeB)
	{
		return 1.0f;
	}

	private float Heuristic(Node nodeA, Node nodeB)
	{
		// Manhattan distance
		return Mathf.Abs(nodeB.gridX - nodeA.gridX) + Mathf.Abs(nodeB.gridY - nodeA.gridY);
	}
}
