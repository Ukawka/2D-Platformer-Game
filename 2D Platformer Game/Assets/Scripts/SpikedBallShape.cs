using Unity.VisualScripting;
using UnityEngine;

public class SpikedBallShape : MonoBehaviour
{
    public GameObject ChainPrefab; // 链条预制体
    public GameObject SpikedBallPrefab; // 链条预制体
    [SerializeField] GameObject[] chains;
    [SerializeField] float chainLength = .1f;
    private float chainGap = 0f;
    void OnValidate()
    {
        foreach(Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
        int chainNum = chains.Length;
        Vector3 chainSize = ChainPrefab.GetComponent<SpriteRenderer>().bounds.size;
        Vector3 spikedBallSize = SpikedBallPrefab.GetComponent<SpriteRenderer>().bounds.size;
        chainGap = chainLength / chainNum;
        chains[0] = Instantiate(ChainPrefab);
        chains[0].transform.parent = transform;
        for(int i = 1; i < chainNum; i++)
        {
            Vector2 nextposition = new Vector2(chains[i-1].transform.position.x, chains[i-1].transform.position.y + chainGap);
            chains[i] = Instantiate(ChainPrefab, nextposition, chains[i-1].transform.rotation);
            chains[i].transform.parent = transform;
        }
        Vector2 newposition = new Vector2(chains[chainNum - 1].transform.position.x, chains[chainNum - 1].transform.position.y + chainSize.y / 2 + chainGap + spikedBallSize.y / 2);
        GameObject SpikedBall = Instantiate(SpikedBallPrefab, newposition, SpikedBallPrefab.transform.rotation);
        SpikedBall.transform.parent = transform;
        
    }
    // GameObject newChain = Instantiate(chainPrefab);
    // SpriteRenderer sr = 
}
