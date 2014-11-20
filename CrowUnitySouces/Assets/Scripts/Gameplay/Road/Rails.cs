using UnityEngine;
using UnityEditor;
using System.Collections;

class BezierPoint
{
	public Vector3 pos;
	public Vector3 delta;
}

public class Rails : MonoBehaviour
{
	const int NB_POSITIONS_PER_SEGMENT=10;

	public int nbRails=0;
	public int nbPoints=0;
	
	
	BezierPoint[,] points; // [point, rail]
	Vector3[,] positions;
	
	BezierPoint test=new BezierPoint();
	
	void Start ()
	{
	}
	
	void Update ()
	{
	
	}
	
	void OnValidate()
	{
		int oldNbPoints=points.GetLength(0);
		int oldNbRails=points.GetLength (1);
		if(oldNbPoints!=nbPoints || oldNbRails!=nbRails)
		{
			resizeArray(oldNbPoints, oldNbRails);
			computePositions();
		}	
	}
	
	void resizeArray(int oldNbPoints, int oldNbRails)
	{
		BezierPoint[,] newPoints=new BezierPoint[nbPoints,nbRails];
		for(int p=0;p<nbPoints;p++) for(int r=0;r<nbRails;r++)
		{
			if(p<oldNbPoints && r<oldNbRails) newPoints[p,r]=points[p,r];
			else newPoints[p,r]=new BezierPoint();
		}
		positions=new Vector3[nbPoints*NB_POSITIONS_PER_SEGMENT+1,nbRails];
	}
	
	void computePositions()
	{
	}
	
	/*void OnDrawGizmos()
	{
		// Utiliser la classe Gizmos
	}*/

	[CustomEditor(typeof(Rails))]
	class RailsEditor : Editor
	{
		void OnSceneGUI()
		{
			Rails tgt=(Rails) target;
			/*Handles.color=new Color(1,0.5f,0);
			tgt.test.pos=Handles.FreeMoveHandle(tgt.test.pos,Quaternion.identity,0.2f,Vector3.zero,
	                                       Handles.SphereCap);*/
	        for(int p=0;p<nbPoints;p++) for(int r=0;r<nbRails;r++)
	        {
				Handles.color=new Color(1,0.5f,0);
				tgt.points[i].pos=Handles.FreeMoveHandle(tgt.points[i].pos,Quaternion.identity,0.2f,Vector3.zero,
				                                         Handles.SphereCap);
	        }
		}
	}
}