
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;


public class Rails : MonoBehaviour
{
	const int NB_POSITIONS_PER_SEGMENT=25;

	public int nbRails=0;
	public int nbPoints=0;
	public bool drawShafts=false;
    public bool sortRails = false;
	public bool sortRailsInverted=false;
	public bool invertStartEnd=false;
	
	public Vector3[] positions;
	public Vector3[] deltas;
    public float[] speedOverrides;

    public float Dist
    {
        get { return dist; }
    }

	private int nbComputedPositions;
	private Vector3[] computedPositions;
    private float[] computedDists;
	private bool needToComputePositions=true;
    private float dist=0;

	[SerializeField, HideInInspector]	
	private int oldNbPoints=0;
	[SerializeField, HideInInspector]	
	private int oldNbRails=0;

	void Start ()
	{
        computePositions();
        //updateDist();
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
        if(sortRails || sortRailsInverted)
        {
        	int way= sortRails ? 1 : -1;
            int[] railsIndex=new int[nbRails];
            float[] railsPos=new float[nbRails];
            for(int i=0;i<nbRails;i++)
            {
                railsIndex[i] = i;
                Vector3 forward=deltas[i].normalized;
                Vector3 right = Vector3.Cross(forward, Vector3.up);
                railsPos[i] = way * -Vector3.Dot(right, positions[i] - positions[0]);
            }
            mergeSortRails(0, nbRails - 1, ref railsIndex, ref railsPos);
            swapArrays(railsIndex);
            sortRails = false;
            sortRailsInverted=false;
        }
        if(invertStartEnd)
        {
        	invertStartEnd=fasle;
        	invertRails();
        }
        computePositions();
        //updateDist();
	}

    /*void updateDist()
    {
        Vector3 prevPoint=getPoint(0,0);
        dist = 0;
        for(float i=0.01f;i<=1;i+=0.01f)
        {
            Vector3 point = getPoint(0, i);
            dist += (point - prevPoint).magnitude;
            prevPoint=point;
        }
    }*/

	void invertRails()
	{
		for(int r=0;r<nbRails;r++) for(int i=0;i<nbPoints/2;i++)
		{
			int inv=nbPoints-1-i;
			Vector3 tmp=positions[i*nbRails+r];
			positions[i*nbRails+r]=positions[inv*nbRails+r];
			positions[inv*nbRails+r]=tmp;
			tmp=deltas[i*nbRails+r];
			deltas[i*nbRails+r]=deltas[inv*nbRails+r];
			deltas[inv*nbRails+r]=tmp;
		}
		computePositions();
	}
	
    void mergeSortRails(int first, int last,ref int[] railsIndex, ref float[] railsPos)
    {
        if (first == last) return;
        else if(first+1==last)
        {
            if (railsPos[first] > railsPos[last]) swapMergeSorteRails(first, last, ref railsIndex, ref railsPos); 
        }
        else
        {
            int mid = (first + last) / 2;
            mergeSortRails(first, mid, ref railsIndex, ref railsPos);
            mergeSortRails(mid + 1, last, ref railsIndex, ref railsPos);
            int lPos = first, rPos = mid + 1, fPos=0;
            int[] newIndex = new int[last - first + 1];
            float[] newPos = new float[last - first + 1];
            Debug.Log(railsIndex[0]+","+railsIndex[1]+","+railsIndex[2]);
            while(lPos<=mid || rPos<=last)
            {
                if(rPos>last || (lPos<=mid && railsPos[lPos]<railsPos[rPos]))
                {
                    newIndex[fPos] = railsIndex[lPos];
                    newPos[fPos++] = railsIndex[lPos++];
                }
                else
                {
                    newIndex[fPos] = railsIndex[rPos];
                    newPos[fPos++] = railsIndex[rPos++];
                }
                Debug.Log(newIndex[fPos - 1]);
            }
            for(int i=first;i<=last;i++)
            {
                railsIndex[i] = newIndex[i - first];
                railsPos[i] = newPos[i - first];
            }
        }
    }

    void swapMergeSorteRails(int a, int b,ref int[] railsIndex, ref float[] railsPos)
    {
            int indexA = railsIndex[a];
            float posA = railsPos[a];
            railsIndex[a] = railsIndex[b];
            railsPos[a] = railsPos[b];
            railsIndex[b] = indexA;
            railsPos[b] = posA;
    }

    void swapArrays(int[] railsIndex)
    {
        Vector3[] newPositions=new Vector3[nbPoints*nbRails];
		Vector3[] newDeltas=new Vector3[nbPoints*nbRails];
        float[] newOverrides = new float[nbPoints * nbRails];
        for (int r = 0; r < nbRails; r++)
        {
            int index = railsIndex[r];
            for (int p = 0; p < nbPoints; p++)
            {
                newPositions[p * nbRails + r] = positions[p * nbRails + index];
                newDeltas[p * nbRails + r] = deltas[p * nbRails + index];
                newOverrides[p * nbRails + r] += speedOverrides[p * nbRails + index];
            }
        }
		positions=newPositions;
		deltas=newDeltas;
        speedOverrides = newOverrides;
    }

	void resizeArray(int oldNbPoints, int oldNbRails)
	{
		Vector3[] newPositions=new Vector3[nbPoints*nbRails];
		Vector3[] newDeltas=new Vector3[nbPoints*nbRails];
        float[] newOverrides = new float[nbPoints * nbRails];

		for(int p=0;p<nbPoints;p++) for(int r=0;r<nbRails;r++)
		{
			if(p<oldNbPoints && r<oldNbRails)
			{
				newPositions[p*nbRails+r]=positions[p*oldNbRails+r];
				newDeltas[p*nbRails+r]=deltas[p*oldNbRails+r];
                newOverrides[p * nbRails + r] = speedOverrides[p * oldNbRails + r];
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
        speedOverrides = newOverrides;
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
        computedDists = new float[nbComputedPositions];
        computedDists[0] = 0;
        for(int i=1;i<nbComputedPositions;i++)
            computedDists[i] = computedDists[i - 1] + (computedPositions[(i - 1) * nbRails] - computedPositions[i * nbRails]).magnitude;
        dist = computedDists[nbComputedPositions - 1];
        dist*=transform.localScale.x;
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

	// Compute the variables necessary to interpolate between the points and the rails
    void computeInterpolations(float rail, float progress, int nbPoints,
                              out int prevPP, out int nextPP,
                              out int prevRail, out int nextRail,
                              out float pPos, out float railPos)
    {
		// Compute the interpolation between the rails
		prevRail=Mathf.FloorToInt(rail);
		if(prevRail<0) prevRail=0;
		if(prevRail>=nbRails) prevRail=nbRails-1;
		nextRail=prevRail+1;
		if(nextRail>=nbRails) nextRail=nbRails-1;
		railPos=rail-prevRail;
		if(railPos>1) railPos=1;
        if (railPos < 0) railPos = 0;

		// Compute the interpolation between the points
		float pointProgress=progress*(nbPoints-1);
		prevPP=Mathf.FloorToInt(pointProgress);
		if(prevPP<0) prevPP=0;
		if(prevPP>=nbPoints-1) prevPP=nbPoints-1;
		nextPP=prevPP+1;
		if(nextPP>=nbPoints-1) nextPP=nbPoints-1;
		pPos=pointProgress-prevPP;
		if(pPos>1) pPos=1;
	
    }

    // Returns the correct progress distant-wise
    public float correct2Incorrect(float correct)
    {
        if (correct <= 0) return 0;
        if (correct >= 1) return 1;
        float tgtDist = correct * dist;
        int pos = 0;
        while (pos != nbComputedPositions - 1 && computedDists[pos + 1] < tgtDist) pos++;
        if (pos == nbComputedPositions - 1) return 1;
        float inProgress = (tgtDist - computedDists[pos]) / (computedDists[pos + 1] - computedDists[pos]);
        return (pos+inProgress)/(nbComputedPositions-1);
    }

    public float incorrect2Correct(float incorrect)
    {
        if (incorrect >= 1) return 1;
        if (incorrect <= 0) return 0;
        float inProgress = incorrect % (1f / (nbComputedPositions - 1f));
        int pos = Mathf.FloorToInt(incorrect * (nbComputedPositions-1));
        if(pos==nbComputedPositions-1) return 1;
        float ptDist = Mathf.Lerp(computedDists[pos], computedDists[pos + 1], inProgress *(nbComputedPositions - 1f));
        return ptDist / dist;
    }

	// Return a point in the curve in [0,nbRails-1] and [0,1]
	public Vector3 getPoint(float rail, float progress)
	{
		if(needToComputePositions) computePositions();
		
        int prevPP, nextPP, prevRail, nextRail;
        float pPos, railPos;
        computeInterpolations(rail, progress, nbComputedPositions,
                              out prevPP, out nextPP,
                              out prevRail, out nextRail,
                              out pPos, out railPos);
		
		// Compute the point
		Vector3	prPp=computedPositions[prevPP*nbRails+prevRail];
		Vector3	prNp=computedPositions[nextPP*nbRails+prevRail];
		Vector3 pr=prPp+(prNp-prPp)*pPos;
		Vector3 nrPp=computedPositions[prevPP*nbRails+nextRail];
		Vector3 nrNp=computedPositions[nextPP*nbRails+nextRail];
		Vector3 nr=nrPp+(nrNp-nrPp)*pPos;
		return transform.TransformPoint(pr+(nr-pr)*railPos);
	}
	
    public Vector3 getForward(float rail, float progress)
    {
        return (getPoint(rail, progress + 0.001f) - getPoint(rail, progress - 0.001f)).normalized;
    }

    // Return the set speed according to the car's own set speed and the rails speed override
    public float getSpeed(float rail, float progress, float speed)
    {
		if(needToComputePositions) computePositions();

        int prevPP, nextPP, prevRail, nextRail;
        float pPos, railPos;
        computeInterpolations(rail, progress, nbPoints,
                              out prevPP, out nextPP,
                              out prevRail, out nextRail,
                              out pPos, out railPos);

        // Compute the point
        float prPp=speedOverrides[prevPP*nbRails+prevRail];
        if (prPp == 0) prPp = speed;
        float prNp=speedOverrides[nextPP*nbRails+prevRail];
        if (prNp == 0) prNp = speed;
        float pr=prPp+(prNp-prPp)*pPos;
        float nrPp=speedOverrides[prevPP*nbRails+nextRail];
        if (nrPp == 0) nrPp = speed;
        float nrNp=speedOverrides[nextPP*nbRails+nextRail];
        if (nrNp == 0) nrNp = speed;
        float nr=nrPp+(nrNp-nrPp)*pPos;

        return pr + (nr - pr) * railPos; 
    }

#if UNITY_EDITOR
    
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
					Vector3 shaftBottom=transform.TransformPoint(positions[index]);
					shaftBottom.y=-100;
					Vector3 shaftTop=shaftBottom;
					shaftTop.y=100;
					Gizmos.DrawLine(shaftBottom,shaftTop);	
					Gizmos.color=new Color(0.5f,0,1);
					shaftBottom=transform.TransformPoint(positions[index]+deltas[index]);
					shaftBottom.y=-100;
					shaftTop=shaftBottom;
					shaftTop.y=100;
					Gizmos.DrawLine(shaftBottom,shaftTop);
					shaftBottom=transform.TransformPoint(positions[index]-deltas[index]);
					shaftBottom.y=-100;
					shaftTop=shaftBottom;
					shaftTop.y=100;
					Gizmos.DrawLine(shaftBottom,shaftTop);
				}
				Gizmos.color=new Color(0,0.5f,0.5f);
				Gizmos.DrawLine(transform.TransformPoint(positions[index]-deltas[index]),transform.TransformPoint(positions[index]+deltas[index]));
			}
			Gizmos.color=new Color(0,0.5f,1);
			for(int p=0;p<nbComputedPositions-1;p++)
				Gizmos.DrawLine(transform.TransformPoint(computedPositions[p*nbRails+r]),transform.TransformPoint(computedPositions[(p+1)*nbRails+r]));
		}
	}
#endif

	public int getNbRails()
	{
		return nbRails;
	}
	
#if UNITY_EDITOR
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
				tgt.positions[index]=tgt.transform.InverseTransformPoint(
					Handles.FreeMoveHandle(tgt.transform.TransformPoint(tgt.positions[index]),Quaternion.identity,0.2f,Vector3.zero,Handles.SphereCap));
				Handles.color=new Color(0.5f,0.5f,0);
				tgt.deltas[index]=tgt.transform.InverseTransformPoint(
					Handles.FreeMoveHandle(tgt.transform.TransformPoint(tgt.positions[index]+tgt.deltas[index]),Quaternion.identity,0.2f,Vector3.zero,Handles.SphereCap))
								-tgt.positions[index];
				tgt.deltas[index]=tgt.positions[index]-tgt.transform.InverseTransformPoint(
				    Handles.FreeMoveHandle(tgt.transform.TransformPoint(tgt.positions[index]-tgt.deltas[index]),Quaternion.identity,0.2f,Vector3.zero,Handles.SphereCap));
	        }
	        if(GUI.changed)
	        {
		        tgt.computePositions();
		        EditorUtility.SetDirty(target);
		    }
		}
	}
#endif
}