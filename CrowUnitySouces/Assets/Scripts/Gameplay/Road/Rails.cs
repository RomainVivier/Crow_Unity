﻿using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
class BezierPoint : ScriptableObject
{
	public Vector3 pos=Vector3.zero;
	public Vector3 delta=Vector3.zero;
}

public class Rails : MonoBehaviour
{
	const int NB_POSITIONS_PER_SEGMENT=25;

	public int nbRails=0;
	public int nbPoints=0;
	public bool drawShafts=false;
	
	[SerializeField]
	BezierPoint[] points; // [point, rail]
	[SerializeField]
	Vector3[] positions;

	[SerializeField, HideInInspector]	
	private int nbPositions;
	[SerializeField, HideInInspector]	
	private int oldNbPoints=0;
	[SerializeField, HideInInspector]	
	private int oldNbRails=0;

		void Start ()
	{
	}
	
	void Update ()
	{
	
	}
	
	void OnValidate()
	{
		if(points==null)
		{
			points=new BezierPoint[0];
			positions=new Vector3[0];
			nbPositions=0;
		}
		if(oldNbPoints!=nbPoints || oldNbRails!=nbRails)
		{
			resizeArray(oldNbPoints, oldNbRails);
			computePositions();
			oldNbPoints=nbPoints;
			oldNbRails=nbRails;
		}	
	}
	
	void resizeArray(int oldNbPoints, int oldNbRails)
	{
		BezierPoint[] newPoints=new BezierPoint[nbPoints*nbRails];
		for(int p=0;p<nbPoints;p++) for(int r=0;r<nbRails;r++)
		{
			if(p<oldNbPoints && r<oldNbRails) newPoints[p*nbRails+r]=points[p*nbRails+r];
			else newPoints[p*nbRails+r]=new BezierPoint();
		}
		nbPositions=(nbPoints-1)*NB_POSITIONS_PER_SEGMENT+1;
		positions=new Vector3[nbPositions*nbRails];
		points=newPoints;
	}
	
	void computePositions()
	{
		for(int r=0;r<nbRails;r++)
		{
			for(int p=0;p<nbPoints-1;p++)
			{
				BezierPoint pt=points[p*nbRails+r];
				BezierPoint npt=points[(p+1)*nbRails+r];			
				for(int sub=0;sub<NB_POSITIONS_PER_SEGMENT;sub++)
				{
					positions[(p*NB_POSITIONS_PER_SEGMENT+sub*nbRails)+r]=
						bezier (new Vector3[4]{pt.pos,pt.pos+pt.delta,npt.pos-npt.delta,npt.pos},
									sub/(NB_POSITIONS_PER_SEGMENT+1f));
					
				}
			}
			positions[(nbPositions-1)*nbRails+r]=points[(nbPoints-1)*nbRails+r].pos;
		}
	}
	
	Vector3 bezier(Vector3[] points, float pos)
	{
		int nb=points.Length;
		if(nb==1) return points[0];
		else
		{
			Vector3[] newPoints=new Vector3[nb-1];
			for(int i=0;i<nb-1;i++)
				newPoints[i]=points[i]+(points[i+1]-points[i])*pos;
			return bezier (newPoints,pos);
		}
	}
	
	void OnDrawGizmos()
	{
		for(int r=0;r<nbRails;r++)
		{
		
			for(int p=0;p<nbPoints;p++)
			{
				BezierPoint pt=points[p*nbRails+r];
				if(drawShafts)
				{
					Gizmos.color=new Color(0,1f,0.5f);
					Vector3 shaftBottom=pt.pos;
					shaftBottom.y=-100;
					Vector3 shaftTop=shaftBottom;
					shaftTop.y=100;
					Gizmos.DrawLine(shaftBottom,shaftTop);	
				}
				Gizmos.color=new Color(0,0.5f,0.5f);
				Gizmos.DrawLine(pt.pos-pt.delta,pt.pos+pt.delta);
			}
			Gizmos.color=new Color(0,0.5f,1);
			for(int p=0;p<nbPositions-1;p++)
				Gizmos.DrawLine(positions[p*nbRails+r],positions[(p+1)*nbRails+r]);
		}
	}
	
	[CustomEditor(typeof(Rails))]
	class RailsEditor : Editor
	{
		void OnSceneGUI()
		{
			Rails tgt=(Rails) target;
	        for(int p=0;p<tgt.nbPoints;p++) for(int r=0;r<tgt.nbRails;r++)
	        {
				Handles.color=new Color(1,0.5f,0);
				BezierPoint pt=tgt.points[p*tgt.nbRails+r];
				pt.pos=Handles.FreeMoveHandle(pt.pos,Quaternion.identity,0.2f,Vector3.zero,Handles.SphereCap);
				Handles.color=new Color(0.5f,0.5f,0);
				pt.delta=Handles.FreeMoveHandle(pt.pos+pt.delta,Quaternion.identity,0.2f,Vector3.zero,Handles.SphereCap)
								-pt.pos;
				pt.delta=pt.pos-
     		             Handles.FreeMoveHandle(pt.pos-pt.delta,Quaternion.identity,0.2f,Vector3.zero,Handles.SphereCap);
	        }
	        tgt.computePositions();
		}
	}
}