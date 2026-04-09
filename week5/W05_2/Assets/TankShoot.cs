using UnityEngine;

public class TankShoot : MonoBehaviour
{
    public int playerNum = 1;
    string fireNmae;
    public Rigidbody prefabShell;
    public Transform fireTransform;

    private void Start()
    {
        fireNmae = "Fire" + playerNum;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(fireNmae))
        {
            Fire();
        }
    }

    void Fire()
    {
        Rigidbody shellInstance = Instantiate(prefabShell, 
            fireTransform.position, fireTransform.rotation) as Rigidbody;

        shellInstance.linearVelocity = 20.0f * fireTransform.forward;
    }
}
