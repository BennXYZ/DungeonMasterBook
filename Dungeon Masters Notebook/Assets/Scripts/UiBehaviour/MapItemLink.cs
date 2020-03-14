using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MapItemLink : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform firstObject;
    public Transform secondObject;
    public Slider slider;
    public bool followMouse = false;

    public MapItemLink SetTransforms(Transform a, Transform b, Slider slid)
    {
        firstObject = a;
        secondObject = b;
        slider = slid;
        return this;
    }

    Vector3 MouseWorldPosition
    {
        get
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(lineRenderer != null && firstObject != null && secondObject != null && lineRenderer.positionCount > 1)
        {
            lineRenderer.SetPosition(0, Vector3.right * firstObject.position.x + Vector3.up * firstObject.position.y + Vector3.forward * 70);
            lineRenderer.SetPosition(1, Vector3.right * secondObject.position.x + Vector3.up * secondObject.position.y + Vector3.forward * 70);
            if(slider != null)
                lineRenderer.widthMultiplier = slider.value;
        }
        else if(lineRenderer != null && lineRenderer.positionCount > 1 && followMouse)
        {
            lineRenderer.SetPosition(0, Utils.NewVector3(firstObject != null ? firstObject.position.x : MouseWorldPosition.x, firstObject != null ? firstObject.position.y : MouseWorldPosition.y, 70));
            lineRenderer.SetPosition(1, Utils.NewVector3(secondObject != null ? secondObject.position.x : MouseWorldPosition.x, secondObject != null ? secondObject.position.y : MouseWorldPosition.y, 70));
            if (slider != null)
                lineRenderer.widthMultiplier = slider.value;
        }
        else
        {
            if(Application.isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }
}
