using UnityEngine;
using System.Collections;

public class testinstantiater : MonoBehaviour
{
    [SerializeField] GameObject projectile;

    IEnumerator testSpawns()
    {
        while (true)
        {
            GameObject spawnedObject = Instantiate(projectile, transform.position, Quaternion.identity);

            yield return new WaitForSeconds(2f);
        }
    }

    private void Start()
    {
        StartCoroutine(testSpawns());
    }
}
