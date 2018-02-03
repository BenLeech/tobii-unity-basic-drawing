using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using UnityEngine.UI;

public class ClickBrush : MonoBehaviour {

	public GameObject GazePoint;
	public GameObject brushPrefab;
    public AudioSource audioSource;
	public GameObject thisBrush;
    public Canvas canvas;
    public Text text;
    public Text positionText;
    public Text colourText;
    public Texture2D texture;
    //blic Camera camera;
    public Material material;


    Vector2 startPos;
	Plane objPlane;

	//Vector3 prevPos;

	public float multiplier = 0.3f;
	private Vector2 _historicPoint;
	private bool _hasHistoricPoint;

    // Called at the start of the scene
	void Start(){

        objPlane = new Plane(Camera.main.transform.forward*-1, this.transform.position);
        audioSource = GetComponent<AudioSource>();
        text.text = "Pitch: ";
        positionText.text = "Position: ";
        colourText.text = "Colour: ";
        audioSource.mute = true;
	}
	
	// Update is called once per frame
	void Update () {

		GazePoint gazePoint = TobiiAPI.GetGazePoint();

		if (Input.GetKeyDown (KeyCode.Space)) {

			thisBrush = (GameObject)Instantiate (brushPrefab, this.transform.position, Quaternion.identity);

			Ray mRay = Camera.main.ScreenPointToRay (gazePoint.Screen);

			float rayDistance;
            _hasHistoricPoint = false;
			if (objPlane.Raycast (mRay, out rayDistance)) {
				startPos = mRay.GetPoint (rayDistance);
			}
		} else if (Input.GetKey (KeyCode.Space) && gazePoint.IsRecent ()) {
			Ray mRay = Camera.main.ScreenPointToRay (gazePoint.Screen);
            audioSource.mute = false;

            float rayDistance;
			if (objPlane.Raycast (mRay, out rayDistance)) {
				thisBrush.transform.position = smoothFilter(new Vector2(mRay.GetPoint(rayDistance).x,mRay.GetPoint(rayDistance).y));

                audioSource.pitch = 3 + (thisBrush.transform.position.y / 4);
                text.text = "Pitch: " + audioSource.pitch.ToString();
            }

            //grab current position
            positionText.text = "Position: " + thisBrush.transform.position.x.ToString() + " " + thisBrush.transform.position.y.ToString();


            /**if (thisBrush.transform.position.x > 0)
            {
                camera.backgroundColor = Color.green;
            }
            else
            {
                camera.backgroundColor = Color.blue;
            }*/

            Vector3 mpos = new Vector3(thisBrush.transform.position.x, thisBrush.transform.position.y, 0);
            var tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            //yield WaitForEndOfFrame();
            tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

            Color bla = tex.GetPixel((int)mpos.x, (int)mpos.y);
            //check value of current position
            if(bla.g != 1.00)
            {
                tex.SetPixel((int)mpos.x, (int)mpos.y, Color.green);
                material.color = Color.green;
                thisBrush.GetComponent<TrailRenderer>().material = material;
                thisBrush.GetComponent<TrailRenderer>().material.color = bla;
            }
            colourText.text = "Colour: " + bla.ToString();
            tex.Apply();

            //update to next colour if needed


        } else if (Input.GetKey (KeyCode.Space)) {
            audioSource.mute = true;

            if (Vector2.Distance (thisBrush.transform.position, startPos) < 0.1)
            {
                Destroy (thisBrush);
                _hasHistoricPoint = false;
            }	
		} else
        {
            audioSource.mute = true;
        }
	}

	private Vector2 smoothFilter(Vector2 point){
		if (!_hasHistoricPoint) {
			_historicPoint = point;
			_hasHistoricPoint = true;
		}

		var smoothedPoint = new Vector2 (point.x * multiplier + _historicPoint.x * (1.0f - multiplier), 
			                    point.y * multiplier + _historicPoint.y * (1.0f - multiplier));

		_historicPoint = smoothedPoint;

		return smoothedPoint;
	}
}
