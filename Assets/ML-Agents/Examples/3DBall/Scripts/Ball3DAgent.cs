using UnityEngine;
using MLAgents;

public class Ball3DAgent : Agent
{
    [Header("Specific to Ball3D")]
    public GameObject ball;
    private Rigidbody m_BallRb;
    private ResetParameters m_ResetParams;

    public override void InitializeAgent()
    {
        m_BallRb = ball.GetComponent<Rigidbody>();
        var academy = FindObjectOfType<Academy>();
        m_ResetParams = academy.resetParameters;
        SetResetParameters();
    }

    public override void CollectObservations()
    {
        AddVectorObs(gameObject.transform.rotation.z);
        AddVectorObs(gameObject.transform.rotation.x);
        AddVectorObs(ball.transform.position - gameObject.transform.position);
        AddVectorObs(m_BallRb.linearVelocity);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (brain.brainParameters.vectorActionSpaceType == SpaceType.Continuous)
        {
            var actionZ = 2f * Mathf.Clamp(vectorAction[0], -1f, 1f);
            var actionX = 2f * Mathf.Clamp(vectorAction[1], -1f, 1f);

            if ((gameObject.transform.rotation.z < 0.25f && actionZ > 0f) ||
                (gameObject.transform.rotation.z > -0.25f && actionZ < 0f))
            {
                gameObject.transform.Rotate(new Vector3(0, 0, 1), actionZ);
            }

            if ((gameObject.transform.rotation.x < 0.25f && actionX > 0f) ||
                (gameObject.transform.rotation.x > -0.25f && actionX < 0f))
            {
                gameObject.transform.Rotate(new Vector3(1, 0, 0), actionX);
            }
        }
        if ((ball.transform.position.y - gameObject.transform.position.y) < -2f ||
            Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 3f ||
            Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 3f)
        {
            Done();
            SetReward(-1f);
        }
        else
        {
            SetReward(0.1f);
        }
    }

    public override void AgentReset()
    {
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gameObject.transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        gameObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));
        m_BallRb.linearVelocity = new Vector3(0f, 0f, 0f);
        ball.transform.position = new Vector3(Random.Range(-1.5f, 1.5f), 4f, Random.Range(-1.5f, 1.5f))
            + gameObject.transform.position;
        //Reset the parameters when the Agent is reset.
        SetResetParameters();
    }

    public void SetBall()
    {
        //Set the attributes of the ball by fetching the information from the academy
        m_BallRb.mass = m_ResetParams["mass"];
        var scale = m_ResetParams["scale"];
        ball.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetResetParameters()
    {
        SetBall();
    }
}
