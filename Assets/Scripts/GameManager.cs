using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GridManager.Instance.SpawnLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}