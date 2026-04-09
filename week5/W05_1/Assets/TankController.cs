using UnityEngine;

public class TankController : MonoBehaviour
{
    float tkMvSpeed = 10.0f;
    float tkRotSpeed = 150.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mv = Input.GetAxis("Vertical1") * tkMvSpeed * Time.deltaTime;
        float rot  = Input.GetAxis("Horizontal1") * tkRotSpeed * Time.deltaTime;

        //Debug.Log($"V: {mv}, H: {rot}");

        transform.Translate(0, 0, mv);
        transform.Rotate(0, rot, 0);
    }
}
