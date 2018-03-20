using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public class PlaceObject : MonoBehaviour {

    //public GameObject GazePoint;
    public GameObject drawObject;
    public float focusRadius = 2;
    Vector2 lastDrawPoint;
    public List<Vector2> localPositionAverager = new List<Vector2>();
    private Vector2 localPositionAverage = new Vector2();
    private Vector2 spotPause = new Vector2();
    GameObject currentObject;

    private float timeStart = 0;
    public float timeInterval = 1000;

    private bool toggleDraw = true;

    public int averageLimit = 100;

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

        if(Vector3.Magnitude(drawPoint-localPositionAverage) < focusRadius)
        {
            if((Time.time - timeStart > timeInterval) && !toggleDraw && (null == TobiiAPI.GetFocusedObject()))
            {
                Debug.Log(Vector3.Magnitude(drawPoint - localPositionAverage));
                Debug.Log("average: " + localPositionAverage);
                Debug.Log("draw point: " + drawPoint);
                currentObject = Instantiate(drawObject, new Vector3(localPositionAverage.x,localPositionAverage.y,-Time.time / 100000), Quaternion.identity);
                timeStart = Time.time;
                toggleDraw = true;
                spotPause = localPositionAverage;
            }
        }
        else
        {
            timeStart = Time.time;
            
        }

        /*if(toggleDraw && (Vector3.Magnitude(localPositionAverage - spotPause) > focusRadius))
        {
            toggleDraw = false;
        }*/

        if(toggleDraw)
        {
            if (Vector3.Magnitude(localPositionAverage - spotPause) > focusRadius)
            {
                toggleDraw = false;
            }

            if(currentObject != null)
                currentObject.transform.localScale *= 1.01f;
        }
        else
        {

        }


        movingAverage(drawPoint);

    }


    private void movingAverage(Vector2 newValue)
    {
        if (localPositionAverager.Count >= averageLimit)    //if the number of datapoints is equal to the moving average, remove the oldest
        {
            //remove oldest from average
            //localPositionAverage = Vector3.LerpUnclamped(localPositionAverager[0], localPositionAverage, ((float)localPositionAverager.Count) / (localPositionAverager.Count - 1));
            //Vec3_RemoveFromAverage(localPositionAverager[0], localPositionAverage, localPositionAverager.Count);

            //remove oldest from list
            localPositionAverager.RemoveAt(0);
        }

        //add the newest data point
        localPositionAverager.Add(newValue);

        Vector2 average = new Vector2(0,0);
        for(int i = 0; i < localPositionAverager.Count; i++)
        {
            average += localPositionAverager[i];
        }
        localPositionAverage = average / localPositionAverager.Count;

        //localPositionAverage = Vector3.LerpUnclamped(localPositionAverager[localPositionAverager.Count - 1], localPositionAverage, ((float)localPositionAverager.Count - 1) / localPositionAverager.Count);
    }

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
