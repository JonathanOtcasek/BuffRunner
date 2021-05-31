using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManScript : MonoBehaviour
{
    float[] muscleVals = new float[] { 0f, .5f, 1f, 1.75f, 2f, 2.25f, 2.5f, 2.75f, 3f, 3.25f, 3.5f };
    public int muscleLevel = 3;
    public float muscleValue = .5f;
    float walkForwardSpeedMultiplier = 10f; //lower is faster
    bool sideToSide = true;
    bool alive = true;
    bool pushing = false;
    bool final = false;

    int score = 0;

    PushableScript tailer;

    public Animator myAnim;

    public Transform wholeBody;
    public Transform spine;
    public Transform rightArm;
    public Transform leftArm;
    public Transform rightHand;
    public Transform leftHand;
    public Transform rightLeg;
    public Transform leftLeg;
    public Transform rightFoot;
    public Transform leftFoot;

    public GameObject leftDumbbell;
    public GameObject rightDumbbell;

    public GameObject goldenLight;
    public Text scoreText;
    public Text winText;

    float scoreTimer = 0f;

    Vector3 lastFramePos = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        scoreTimer += Time.deltaTime;

        if(muscleValue <= .3)
        {
            alive = false;
            muscleValue = .3f;
            if (!final)
            {
                myAnim.Play("Die");
            } else
            {
                myAnim.Play("Dance");
                winText.gameObject.SetActive(true);
            }
        }

        wholeBody.transform.localScale = Vector3.one * Mathf.Clamp(muscleValue * muscleValue/ 3.5f, 1f,20);
        spine.transform.localScale = Vector3.one * Mathf.Clamp(muscleValue / 2f, .8f, 10);

        transform.position = new Vector3(transform.position.x, .25f * Mathf.Clamp(muscleValue * muscleValue / 3.5f, 1, 20), transform.position.z);

        rightArm.transform.localScale = new Vector3(rightArm.localScale.x, rightArm.localScale.y, muscleValue);
        leftArm.transform.localScale = new Vector3(leftArm.localScale.x, leftArm.localScale.y, muscleValue);
        rightLeg.transform.localScale = new Vector3(Mathf.Clamp(muscleValue/2, .5f, 10f), rightLeg.localScale.y, Mathf.Clamp(muscleValue / 2, .5f, 10f));
        leftLeg.transform.localScale = new Vector3(Mathf.Clamp(muscleValue / 2, .5f, 10f), leftLeg.localScale.y, Mathf.Clamp(muscleValue / 2, .5f, 10f));

        rightHand.transform.localScale = new Vector3(rightHand.localScale.x, rightHand.localScale.y, 1 / muscleValue);
        leftHand.transform.localScale = new Vector3(leftHand.localScale.x, leftHand.localScale.y, 1 / muscleValue);
        rightFoot.transform.localScale = new Vector3(1 / (Mathf.Clamp(muscleValue / 2, .5f, 10f)), rightFoot.localScale.y, 1 / (Mathf.Clamp(muscleValue / 2, .5f, 10f)));
        leftFoot.transform.localScale = new Vector3(1 / (Mathf.Clamp(muscleValue / 2, .5f, 10f)), leftFoot.localScale.y, 1 / (Mathf.Clamp(muscleValue / 2, .5f, 10f)));

        if (alive)
        {
            if (!pushing)
            {
                muscleValue = Mathf.Lerp(muscleValue, muscleVals[muscleLevel], .005f);
            }
            else
            {
                muscleValue = Mathf.Lerp(muscleValue, muscleVals[muscleLevel], .00125f);
                //if (scoreTimer >= 1)
                //{
                    score++;
                    if (final)
                    {
                        score += 3;
                    }
                //}
            }

            //if(scoreTimer >= 1)
            //{
                score++;
                scoreTimer = 0f;
            //}
            
            if (Input.GetMouseButton(0))
            {
                float xDiff = Camera.main.ScreenToViewportPoint(Input.mousePosition).x - lastFramePos.x;

                if (xDiff < 0 && sideToSide)
                {
                    if (transform.position.x > -1.5f)
                    {
                        transform.Translate(-transform.right / 20f);
                    }
                }
                else
                {
                    if (xDiff > 0 && sideToSide)
                    {
                        if (transform.position.x < 1.5f)
                        {
                            transform.Translate(transform.right / 20f);
                        }
                    }
                }
            } else
            {
                if (winText.gameObject.activeInHierarchy)
                {
                    Camera.main.transform.position -= Camera.main.transform.right;
                    Camera.main.transform.LookAt(this.transform.position + new Vector3(0f,1f,0f));
                }
            }

            /*if (Input.GetMouseButtonDown(0) && sideToSide == false)
            {
                muscleValue += .4f;
            }*/

            lastFramePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            scoreText.text = "Score: " + score;
        }
    }

    private void FixedUpdate()
    {
        if (alive)
        {
            if (!sideToSide && tailer != null)
            {
                tailer.transform.Translate(transform.forward / walkForwardSpeedMultiplier);

                if (!final)
                {
                    tailer.timePushed += Time.deltaTime;
                }

                if(tailer.timePushed >= 4f)
                {
                    Destroy(tailer.gameObject);
                    myAnim.Play("HappyWalk");
                    walkForwardSpeedMultiplier = 10f; //lower is faster
                    sideToSide = true;
                    tailer = null;
                    pushing = false;
                }
            }
            transform.Translate(transform.forward / walkForwardSpeedMultiplier);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        tailer = other.GetComponent<PushableScript>();
        if (tailer.dumbbell) {
            StartCoroutine(DumbbellGo());
            muscleLevel = muscleLevel + 1 > muscleVals.Length-1 ? muscleVals.Length - 1 : muscleLevel + 1;
            Destroy(tailer.gameObject);
        } else
        {
            if (tailer.final)
            {
                final = true;
                goldenLight.SetActive(false);
                Camera.main.transform.position += Camera.main.transform.right * -3;
                Camera.main.transform.position += Camera.main.transform.forward * -2;
                Camera.main.transform.LookAt(this.gameObject.transform.position + new Vector3(0f,1f,0f));
            }
            pushing = true;
            myAnim.Play("Push");
            walkForwardSpeedMultiplier = 40f; //lower is faster
            sideToSide = false;
            muscleLevel = muscleLevel - (tailer.strengthValue - 1) < 0 ? 0 : muscleLevel - (tailer.strengthValue - 1);
        }
    }

    IEnumerator DumbbellGo()
    {
        leftDumbbell.SetActive(true);
        rightDumbbell.SetActive(true);
        myAnim.Play("Curl");
        yield return new WaitForSeconds(3f);
        myAnim.Play("HappyWalk");
        leftDumbbell.SetActive(false);
        rightDumbbell.SetActive(false);
    }
}
