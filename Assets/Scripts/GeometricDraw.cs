using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public class GeometricDraw : MonoBehaviour {

    //public GameObject GazePoint;
    public GameObject drawObject;
    Vector2 lastDrawPoint;
    Quaternion orientation;
    public bool rotateSprite;


    public float multiplier = 0.3f;
    private Vector2 _historicPoint;
    private bool _hasHistoricPoint;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        if (!TobiiAPI.GetUserPresence().IsUserPresent())
            return;

        GazePoint gazePoint = TobiiAPI.GetGazePoint();
        

        Vector2 drawPoint = smoothFilter(Camera.main.ScreenToWorldPoint(gazePoint.Screen));
        
        if(rotateSprite)
        {
            Instantiate(drawObject, drawPoint, Quaternion.LookRotation(Vector3.forward, drawPoint - lastDrawPoint));
        }
        else
        {
            Instantiate(drawObject, drawPoint, Quaternion.identity);
        }


        lastDrawPoint = drawPoint;
	}



    /*private Vector2 movingAverage(Vector2 newValue)
    {
        if (localPositionAverager.Count >= averageLimit)    //if the number of datapoints is equal to the moving average, remove the oldest
        {
            //remove oldest from average
            localPositionAverage = Vector3.LerpUnclamped(localPositionAverager[0], localPositionAverage, ((float)localPositionAverager.Count) / (localPositionAverager.Count - 1));
            //Vec3_RemoveFromAverage(localPositionAverager[0], localPositionAverage, localPositionAverager.Count);
            localRotationAverage = Quaternion.LerpUnclamped(localRotationAverager[0], localRotationAverage, ((float)localRotationAverager.Count) / (localRotationAverager.Count - 1));

            //remove oldest from list
            localPositionAverager.RemoveAt(0);
            localRotationAverager.RemoveAt(0);
        }

        //add the newest data point
        localPositionAverager.Add(transformCalc.transform.localPosition);
    }*/
        
    


    private Vector2 smoothFilter(Vector2 point)
    {
        if (!_hasHistoricPoint)
        {
            _historicPoint = point;
            _hasHistoricPoint = true;
        }

        var smoothedPoint = new Vector2(point.x * multiplier + _historicPoint.x * (1.0f - multiplier),
                                point.y * multiplier + _historicPoint.y * (1.0f - multiplier));

        _historicPoint = smoothedPoint;

        return smoothedPoint;
    }
}
