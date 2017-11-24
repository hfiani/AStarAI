using System;
using UnityEngine;
using System.Collections;

public class AStarFunctions
{
	class block
	{
		public Vector2 position;
		public float cost;
		public block previous;
		public float costSoFar;
	}

	public static Stack AStar(Vector3 Position, Vector3 TargetPosition)
	{
		Stack roadToTake = new Stack();

		ArrayList closedList = new ArrayList ();
		ArrayList openedList = new ArrayList ();

		int i = -(int)Position.y;
		int j = +(int)Position.x;

		int i_target = -(int)TargetPosition.y;
		int j_target = +(int)TargetPosition.x;

		block current = new block();
		current.cost = 0;
		current.position = new Vector2 (i, j);
		current.previous = null;
		current.costSoFar = 0;

		openedList.Add (current);

		while(openedList.Count > 0)
		{
			ArrayList openedListClose = new ArrayList();

			// we make a loop on just the lowest cost blocks
			GetMinCostBlock (openedList, ref openedListClose);

			foreach (block item_block in openedListClose)
			{
				Vector2 item = item_block.position;
				int i_current = (int)item.x;
				int j_current = (int)item.y;
				if (i_target == i_current && j_target == j_current)
				{
					openedList.Clear ();
					DrawToBeforeLine(item_block, ref roadToTake);
					break;
				}
				else
				{
					openedList.Remove (item_block);
					closedList.Add (item_block);

					GetSurroundingBlocks (TargetPosition, i_current, j_current, ref openedList, closedList, item_block);
				}
			}
		}
		return roadToTake;
	}

	/// <summary>
	/// Gets the surrounding blocks and checks who must be put in the openedlist
	/// </summary>
	/// <param name="TargetPosition">Target position.</param>
	/// <param name="i_current">I current.</param>
	/// <param name="j_current">J current.</param>
	/// <param name="openedList">Opened list.</param>
	/// <param name="closedList">Closed list.</param>
	/// <param name="item_block">Item block.</param>
	static void GetSurroundingBlocks(Vector3 TargetPosition, int i_current, int j_current, ref ArrayList openedList, ArrayList closedList, block item_block)
	{
		int[] i_directions = new int[4];
		int[] j_directions = new int[4];

		i_directions [0] = i_current - 1;
		j_directions [0] = j_current;
		i_directions [1] = i_current;
		j_directions [1] = j_current - 1;
		i_directions [2] = i_current + 1;
		j_directions [2] = j_current;
		i_directions [3] = i_current;
		j_directions [3] = j_current + 1;

		for (int k = 0; k < 4; k++)
		{
			int i_test = i_directions [k];
			int j_test = j_directions [k];
			int tile = MapCreate.Map [i_test, j_test];
			if (MapCreate.TileCost [tile] != 0)
			{
				// if block is in opened list
				bool inside_open = false;
				foreach (block item_blocksearch in openedList)
				{
					Vector2 item_search = item_blocksearch.position;
					if (item_search.x == i_test && item_search.y == j_test)
					{
						inside_open = true;
						break;
					}
				}

				// if block is in closed list
				bool inside_closed = false;
				foreach (block item_blocksearch in closedList)
				{
					Vector2 item_search = item_blocksearch.position;
					if (item_search.x == i_test && item_search.y == j_test)
					{
						inside_closed = true;
						break;
					}
				}

				if (!inside_open && !inside_closed)
				{
					float cost = 
						item_block.costSoFar +
						MapCreate.TileCost [tile] + 
						CalculateRoadCost (TargetPosition + MapCreate.PositionStarter, new Vector3 (j_test, -i_test) + MapCreate.PositionStarter);

					block new_block = new block();
					new_block.cost = cost;
					new_block.position = new Vector2 (i_test, j_test);
					new_block.previous = item_block;
					new_block.costSoFar = item_block.costSoFar + MapCreate.TileCost [tile];
					openedList.Add (new_block);
				}
			}
		}
	}

	/// <summary>
	/// Gets the minimum cost block which will be the next block to use
	/// </summary>
	/// <returns>The minimum cost block.</returns>
	/// <param name="openedList">Opened list.</param>
	static void GetMinCostBlock(ArrayList openedList, ref ArrayList openedListClose)
	{
		ArrayList indices = new ArrayList ();
		float min = 999999;
		// get minimum cost
		foreach (block line in openedList)
		{
			float cost = line.cost;
			if (cost < min)
			{
				min = cost;
			}
		}

		// get first block with minimum cost
		foreach(block line in openedList)
		{
			float cost = line.cost;
			if (cost == min)
			{
				openedListClose.Add(line);
			}
		}
	}

	/// <summary>
	/// Calculates the road cost without any obstacle
	/// </summary>
	/// <returns>The road cost.</returns>
	/// <param name="from">From.</param>
	/// <param name="to">To.</param>
	static float CalculateRoadCost(Vector3 from, Vector3 to)
	{
		float i_from = from.x;
		float j_from = from.y;
		float i_to = to.x;
		float j_to = to.y;

		return Mathf.Abs(i_to - i_from) + Mathf.Abs(j_to - j_from);
	}

	/// <summary>
	/// Returns backwards from final block to first block and put them in correct order.
	/// Stack is used to reverse order from final-first to first-final
	/// </summary>
	/// <param name="now">Now.</param>
	/// <param name="roadToTake">Road to take.</param>
	static void DrawToBeforeLine(block now, ref Stack roadToTake)
	{
		if (now.previous != null)
		{
			// draw a debug line, will not show in build
			Debug.DrawLine (
				new Vector3(
					now.position.y,
					-now.position.x,
					-5
				) + MapCreate.PositionStarter,
				new Vector3(
					now.previous.position.y,
					-now.previous.position.x,
					-5
				) + MapCreate.PositionStarter,
				Color.cyan,
				10000
			);
			// push the road to take
			roadToTake.Push (now.position);

			// recursive, we stop when we the previous is null => we arrived to first block
			DrawToBeforeLine (now.previous, ref roadToTake);
		}
	}
}

