using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterData
{
	public enum CharacterType
	{
		CHARACTER,
		SPACE,
		NEWLINE
	}
	public CharacterType charType;
	private float progress;

	public float Progress
	{
		get { return progress; }
	}

	private float startingTime;

	private float totalAnimationTime;
	private int order;

	public VertexPos vertexPos;

	public int Order
	{
		get { return order; }
	}

	public CharacterData(float startTime, float targetAnimationTime, int targetOrder, VertexPos vertPos)
	{
		progress = 0.0f;
		startingTime = startTime;
		totalAnimationTime = (startingTime + targetAnimationTime) - startTime;
		order = targetOrder;
		vertexPos = vertPos;
		charType = CharacterType.CHARACTER;
	}

	public void UpdateTime(float time)
	{
		if (time < startingTime)
		{
			progress = 0;
		}
		else
		{
			progress = (time - startingTime) / totalAnimationTime;
		}
	}
}

public struct VertexPos
{
	public Vector3[] basePositions;
	public Vector3[] modifiedPositions;
	public VertexPos(Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4)
	{
		basePositions = new Vector3[4];
		modifiedPositions = new Vector3[4];
		basePositions[0] = modifiedPositions[0] = pos1;
		basePositions[1] = modifiedPositions[1] = pos2;
		basePositions[2] = modifiedPositions[2] = pos3;
		basePositions[3] = modifiedPositions[3] = pos4;
	}
	public void MoveBaseVertex(Vector3 vert)
	{
		basePositions[0] += vert;
		basePositions[1] += vert;
		basePositions[2] += vert;
		basePositions[3] += vert;
		ResetModified();
	}
	public void ResetModified()
	{
		for(int i = 0; i < 4; i++)
		{
			modifiedPositions[i] = basePositions[i];
		}
	}
}