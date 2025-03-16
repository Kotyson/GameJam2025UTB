using UnityEngine;

public class LoadEnd : MonoBehaviour
{
    public GameObject hovno;
    private void OnTriggerEnter(Collider other)
    {
        hovno.SetActive(true);
    }
}
