using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using System;

public class UnityCoordinates : MonoBehaviour, IMixedRealityPointerHandler
{
    public TMP_Text coordinates1;
    public TMP_Text coordinates2;
    public TMP_Text distance;
    public GameObject dropPoint;
    public GameObject dropPoint2;
    public bool debugger = true;

    public static int xscale = 20; // x pixels divided by the picture's Unity width. Ex: 200/10
    public static int yscale = 20; // y pixels divided by the picture's Unity height.
    public Vector3 scale = new Vector3(xscale, yscale, 1);

    [HideInInspector]
    public String coords, coords2;
    [HideInInspector]
    public int state;
    //static variable that holds the first point the user clicks
    public static Vector3 selPoint1 = Vector3.zero;
    //static variable that holds the second point the user clicks
    public static Vector3 selPoint2 = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        state = 0;
        coords = "(x1,y1,z1)";
    }

    // Update is called once per frame
    void Update()
    {
        if (state == 0)
        {
            coords = "(x1,y1,z1)";
            coords2 = "(x2,y2,z2)";
            distance.text = "Distance = ________";
        }
        foreach(var source in MixedRealityToolkit.InputSystem.DetectedInputSources)
        {
            // Ignore anything that is not a hand because we want articulated hands
            if (source.SourceType == Microsoft.MixedReality.Toolkit.Input.InputSourceType.Hand)
            {
                foreach (var p in source.Pointers)
                {
                    if (p is IMixedRealityNearPointer)
                    {
                        // Ignore near pointers, we only want the rays
                        continue;
                    }
                    if (p.Result != null)
                    {
                        var startPoint = p.Position;
                        var endPoint = p.Result.Details.Point;
                        var hitObject = p.Result.Details.Object;
                        if (hitObject)
                        {
                            if (state == 1) // Pick first point
                            {
                                dropPoint.transform.position = endPoint;
                                
                                endPoint = Vector3.Scale(endPoint, scale);
                                endPoint.z = 0;
                                coords = endPoint.ToString();
                                //(x, y, z) of first point selected should be placed in selPoint1
                                selPoint1 = endPoint;
                            }
                            if (state == 2) // Pick second point
                            {
                                dropPoint2.transform.position = endPoint;
                            
                                endPoint = Vector3.Scale(endPoint, scale);
                                endPoint.z = 0;
                                coords2 = endPoint.ToString();
                                //(x, y, z) of second point selected should be placed in selPoint2
                                selPoint2 = endPoint;
                            }
                            if (state == 3) // Calculate distance
                            {
                                if (selPoint1 != Vector3.zero && selPoint2 != Vector3.zero)
                                {
                                    calcDistance(selPoint1, selPoint2);
                                }
                            }
                        }
                    }
                }
            }
        }
 
        coordinates1.text = ("Point1: " + coords);
        coordinates2.text = ("Point2: " + coords2);

        // Click to cycle through states
        if (Input.GetMouseButtonDown(0))
        {
            if(state == 3) // End to placeholder
            {
                state = 4;
            }
            if (state == 2) // Point2 to end
            {
                state = 3;
            }
            if (state == 1) // Point1 to Point2
            {
                state = 2;
            }
            if (state == 0) // Start to Point1
            {
                state = 1;
            }
            if (state == 4) // Placeholder to Start
            {
                state = 0;
            }
            if (debugger)
            {
                Debug.Log("left mouse clicked, State: " + state);
            }
            
        }
    }

    // Function is to change the state on MR clicks. Doesn't work on emulator at least.
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if(state == 3) // End to placeholder
        {
            state = 4;
        }
        if (state == 2) // Point2 to end
        {
            state = 3;
        }
        if (state == 1) // Point1 to Point2
        {
            state = 2;
        }
        if (state == 0) // Start to Point1
        {
            state = 1;
        }
        if (state == 4) // Placeholder to Start
        {
            state = 0;
        }
        if (debugger)
        {
            Debug.Log("Pointer clicked, State: " + state);
        }
    }
    
    //This function will calculate the distance between the current first point and second point that the user has selected on the screen
    //Then it prints the distance and resets the two values to zero for the user to add the next 2 points
    private void calcDistance(Vector3 s1, Vector3 s2) 
    {
        Vector3 difference = new Vector3(s1.x - s2.x, s1.y - s2.y, s1.z - s2.z);
        double dist = Math.Sqrt(Math.Pow(difference.x, 2f) + Math.Pow(difference.y, 2f) + Math.Pow(difference.z, 2f));
        distance.text = ("Dist: " + dist.ToString());
        // Debug.Log(distance);
        selPoint1 = Vector3.zero;
        selPoint2 = Vector3.zero;
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData){}
    public void OnPointerDown(MixedRealityPointerEventData eventData){}
    public void OnPointerDragged(MixedRealityPointerEventData eventData){}

}
