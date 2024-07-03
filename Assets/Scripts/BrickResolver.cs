using System.Collections.Generic;

/// <summary>
/// DO NOT MODIFY THIS CODE!
/// 
/// This static class is here to resolve the chain of destruction a brick might cause
/// Given a brick, it will call the OnResolveHit function before checking if a brick 
/// should be detroyed or not and if it affects any of the neighbours.
/// </summary>
public static class BrickResolver
{
	/// <summary>
	/// Returns all bricks that need to be destroyed
	/// </summary>
	/// <param name="source">The initial brick that was hit</param>
	/// <returns>All bricks that need to be destroyed</returns>
	public static IEnumerable<IBrick> ResolveBricksToDestroy(IBrick source)
	{
		source.OnResolveHit();

		List<IBrick> resolved = new List<IBrick>();

		if (source.Lives <= 0)
		{
			yield return source;
			resolved.Add(source);

			if (source.WillHitNeighboursOnDeath)
			{
				foreach (IBrick neighbour in ResolveBricksToDestroyRecursively(source.Neighbours, resolved))
				{
					yield return neighbour;
				}
			}
		}
	}

	private static IEnumerable<IBrick> ResolveBricksToDestroyRecursively(IEnumerable<IBrick> bricks, List<IBrick> resolved)
	{
		foreach (IBrick brick in bricks)
		{
			if (resolved.Contains(brick))
			{
				continue;
			}

			brick.OnResolveHit();
			resolved.Add(brick);

			if (brick.Lives <= 0)
			{
				yield return brick;

				if (brick.WillHitNeighboursOnDeath)
				{
					foreach (IBrick neighbour in ResolveBricksToDestroyRecursively(brick.Neighbours, resolved))
					{
						yield return neighbour;
					}
				}
			}
		}
	}
}
