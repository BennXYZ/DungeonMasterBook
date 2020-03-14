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

    public MapItemLink SetTransforms(Transform a, Transform b, Slider slid)
    {
        firstObject = a;
        secondObject = b;
        slider = slid;
        return this;
    }

    // Update is called once per frame
    void Update()
    {
        if(lineRenderer != null && firstObject != null && secondObject != null && lineRenderer.positionCount > 1)
        {
            lineRenderer.SetPosition(0, Vector3.right * firstObject.position.x + Vector3.up * firstObject.position.y + Vector3.forward * 70);
            lineRenderer.SetPosition(1, Vector3.right * secondObject.position.x + Vector3.up * secondObject.position.y + Vector3.forward * 70);
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
