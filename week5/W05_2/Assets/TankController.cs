using UnityEngine;

public class TankController : MonoBehaviour
{
    public int playerNum = 1;
    string mvAxisName;
    string rotAxisName;
    float tkMvSpeed = 10.0f;
    float tkRotSpeed = 150.0f;
    public Color tankColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mvAxisName = "Vertical" + playerNum;
        rotAxisName = "Horizontal" + playerNum;

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material.color = tankColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float mv = Input.GetAxis(mvAxisName) * tkMvSpeed * Time.deltaTime;
        float rot  = Input.GetAxis(rotAxisName) * tkRotSpeed * Time.deltaTime;

        //Debug.Log($"V: {mv}, H: {rot}");

        transform.Translate(0, 0, mv);
        transform.Rotate(0, rot, 0);
    }
}
