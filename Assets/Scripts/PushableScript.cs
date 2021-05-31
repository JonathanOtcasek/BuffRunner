using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableScript : MonoBehaviour
{
    float[] muscleVals = new float[] { 0f, .5f, 1f, 1.75f, 2f, 2.25f, 2.5f, 2.75f, 3f, 3.25f, 3.5f };
    public int strengthValue;
    public float timePushed;
    public bool dumbbell;
    public bool final;

    public Material myMat;

    // Start is called before the first frame update
    void Start()
    {
        if (!dumbbell && !final)
        {
            transform.localScale = new Vector3(.7f, (muscleVals[strengthValue] * muscleVals[strengthValue]/2.1f) - .25f, .7f);
            transform.position = new Vector3(transform.position.x, transform.localScale.y / 2, transform.position.z);
            myMat = new Material(myMat);
            myMat.color = Color.HSVToRGB(muscleVals[strengthValue] / 3.5f, 1, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dumbbell)
        {
            transform.Rotate(new Vector3(0f, 1f, 0f));
        }
    }
}
