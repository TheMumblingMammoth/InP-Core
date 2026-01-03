using UnityEngine;
public class Grid : MonoBehaviour
{
    [SerializeField] GridTile sample;
    [SerializeField] Vector2Int size;
    [SerializeField] Vector2Int coords = Vector2Int.zero;
    GridTile [][] matrix;
    
    void Awake() {
        matrix = new GridTile[size.y][];
        for(int i = 0; i < size.y; i++)
        {
            matrix[i] = new GridTile[size.x];
            for(int j = 0; j < size.x; j++)
            {
                matrix[i][j] = Instantiate(sample);
                matrix[i][j].transform.SetParent(transform);
                matrix[i][j].transform.localPosition = new Vector3(j - size.x / 2, i - size.y / 2, 0);
                matrix[i][j].gameObject.SetActive(true);
            }
        }
    }

    void Update()
    {
        if(Vector2.Distance(Camera.main.transform.position, transform.position) >= 1f)
        {
            Snap(new Vector2Int((int)Camera.main.transform.position.x, (int)Camera.main.transform.position.y));
        }
    }
    
    void Snap(Vector2Int coords)
    {
        this.coords = coords;
        transform.position = (Vector2)coords;
        for(int i = 0; i < size.y; i++)
        {
            for(int j = 0; j < size.x; j++)
            {
                matrix[i][j].transform.localPosition = new Vector3(j - size.x / 2, i - size.y / 2, 0);
                matrix[i][j].transform.localPosition = new Vector3(j - size.x / 2, i - size.y / 2, 0);
                matrix[i][j].Upload(j, i, World.chunkSize.z / 2);
            }
        }
    }

}
