
using UnityEngine;
using UnityEditor;
using System.Collections;


public class Rails : MonoBehaviour
{
	const int NB_POSITIONS_PER_SEGMENT=25;

	public int nbRails=0;
	public int nbPoints=0;
	public bool drawShafts=false;
	
	public Vector3[] positions;
	public Vector3[] deltas;
	
	private int nbComputedPositions;
	private Vector3[] computedPositions;
	private bool needToComputePositions=true;
	
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
		if(positions==null)
		{
			positions=new Vector3[0];
			deltas=new Vector3[0];
			computedPositions=new Vector3[0];
			nbPoints=0;
			nbComputedPositions=0;
			oldNbPoints=0;
			oldNbRails=0;
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
		Vector3[] newPositions=new Vector3[nbPoints*nbRails];
		Vector3[] newDeltas=new Vector3[nbPoints*nbRails];
		for(int p=0;p<nbPoints;p++) for(int r=0;r<nbRails;r++)
		{
			if(p<oldNbPoints && r<oldNbRails)
			{
				newPositions[p*nbRails+r]=positions[p*oldNbRails+r];
				newDeltas[p*nbRails+r]=deltas[p*oldNbRails+r];				
			}
			else
			{
				newPositions[p*nbRails+r]=Vector3.zero;
				newDeltas[p*nbRails+r]=Vector3.zero;			
			}
		}
		nbComputedPositions=(nbPoints-1)*NB_POSITIONS_PER_SEGMENT+1;
		computedPositions=new Vector3[nbComputedPositions*nbRails];
		positions=newPositions;
		deltas=newDeltas;
	}
	
	void computePositions()
	{
		nbComputedPositions=(nbPoints-1)*NB_POSITIONS_PER_SEGMENT+1;
		computedPositions=new Vector3[nbComputedPositions*nbRails];		
		for(int r=0;r<nbRails;r++)
		{
			for(int p=0;p<nbPoints-1;p++)
			{
				int i0=p*nbRails+r;
				int i1=(p+1)*nbRails+r;
				for(int sub=0;sub<NB_POSITIONS_PER_SEGMENT;sub++)
				{
					computedPositions[(p*NB_POSITIONS_PER_SEGMENT+sub)*nbRails+r]=
						bezier (new Vector3[4]{positions[i0],positions[i0]+deltas[i0],positions[i1]-deltas[i1],positions[i1]},
									sub/(NB_POSITIONS_PER_SEGMENT+1f));				
				}
			}
			computedPositions[(nbComputedPositions-1)*nbRails+r]=positions[(nbPoints-1)*nbRails+r];
		}
		needToComputePositions=false;
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
		if(needToComputePositions) computePositions();
		for(int r=0;r<nbRails;r++)
		{
			for(int p=0;p<nbPoints;p++)
			{
				int index=p*nbRails+r;
				if(drawShafts)
				{
					Gizmos.color=new Color(0,1f,0.5f);
					Vector3 shaftBottom=positions[index];
					shaftBottom.y=-100;
					Vector3 shaftTop=shaftBottom;
					shaftTop.y=100;
					Gizmos.DrawLine(shaftBottom,shaftTop);	
				}
				Gizmos.color=new Color(0,0.5f,0.5f);
				Gizmos.DrawLine(positions[index]-deltas[index],positions[index]+deltas[index]);
			}
			Gizmos.color=new Color(0,0.5f,1);
			for(int p=0;p<nbComputedPositions-1;p++)
				Gizmos.DrawLine(computedPositions[p*nbRails+r],computedPositions[(p+1)*nbRails+r]);
		}
	}
	
	[CustomEditor(typeof(Rails))]
	class RailsEditor : Editor
	{
		void OnSceneGUI()
		{
			Rails tgt=(Rails) target;
			if(tgt.nbPoints!=tgt.oldNbPoints || tgt.nbRails!=tgt.oldNbRails) return;
	        for(int p=0;p<tgt.nbPoints;p++) for(int r=0;r<tgt.nbRails;r++)
	        {
				Handles.color=new Color(1,0.5f,0);
				int index=p*tgt.nbRails+r;
				tgt.positions[index]=Handles.FreeMoveHandle(tgt.positions[index],Quaternion.identity,0.2f,Vector3.zero,Handles.SphereCap);
				Handles.color=new Color(0.5f,0.5f,0);
				tgt.deltas[index]=Handles.FreeMoveHandle(tgt.positions[index]+tgt.deltas[index],Quaternion.identity,0.2f,Vector3.zero,Handles.SphereCap)
								-tgt.positions[index];
				tgt.deltas[index]=tgt.positions[index]-
     		             Handles.FreeMoveHandle(tgt.positions[index]-tgt.deltas[index],Quaternion.identity,0.2f,Vector3.zero,Handles.SphereCap);
	        }
	        if(GUI.changed)
	        {
		        tgt.computePositions();
		        EditorUtility.SetDirty(target);
		    }
		}
	}
}