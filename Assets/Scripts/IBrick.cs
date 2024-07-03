using System.Collections.Generic;


/// <summary>
/// DO NOT MODIFY THIS CODE
/// 
/// Interface used by the <see cref="BrickResolver"/> class to resolve its destruction
/// </summary>
public interface IBrick
{
	/// <summary>
	/// All neighbouring bricks of this brick
	/// </summary>
	IEnumerable<IBrick> Neighbours { get; }

	/// <summary>
	/// If <see langword="true"/>, the <see cref="BrickResolver"/> will also resolve all the neighbours
	/// </summary>
	bool WillHitNeighboursOnDeath { get; }

	/// <summary>
	/// The amount of lives (hits) this brick can take before it can be destroyed
	/// Will be returned by the <see cref="BrickResolver"/> if Lives <= 0;
	/// </summary>
	int Lives { get; }

	/// <summary>
	/// Will be called by the <see cref="BrickResolver"/> before the check if the brick should be destroyed due to the lives reaching 0
	/// </summary>
	void OnResolveHit();
}
