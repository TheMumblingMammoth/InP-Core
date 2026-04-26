using UnityEngine;
public class IsoGrid : MonoBehaviour
{
    private static IsoGrid proxy;
    [SerializeField] IsoGridTile sample;
    [SerializeField] Vector2Int size;
    [SerializeField] Vector2Int coords = Vector2Int.zero;
    IsoGridTile [][] matrix;

    void Awake()
    {
        proxy = this;
        matrix = new IsoGridTile[size.y][];
        for (int i = 0; i < size.y; i++)
        {
            matrix[i] = new IsoGridTile[size.x];
            for (int j = 0; j < size.x; j++)
            {
                matrix[i][j] = Instantiate(sample);
                matrix[i][j].transform.SetParent(transform);
                matrix[i][j].transform.localPosition = new Vector3( j * IsoGridTile.TileSizeX / 2 - size.x / 2,
                                                                    i * IsoGridTile.TileSizeY - ((coords.x + j) % 2 == 0 ? 0 : IsoGridTile.TileSizeY / 2) - size.y / 2, 0);
                matrix[i][j].gameObject.SetActive(true);
            }
        }
    }

    void Start()
    {
        Upload();
    }

    void Update()
    {
        if(Vector2.Distance(Camera.main.transform.position, transform.position) >= 1.5f)
        {
            Snap(new Vector2Int((int)Camera.main.transform.position.x, (int)Camera.main.transform.position.y));
        }
    }
    public static void Upload()
    {
        for(int i = 0; i < proxy.size.y; i++)
        {
            for(int j = 0; j < proxy.size.x; j++)
            {
                proxy.matrix[i][j].Upload(proxy.coords.x - proxy.size.x / 2 + j, proxy.coords.y - proxy.size.y /2 + i, World.chunkSize.z / 2);
            }
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
                matrix[i][j].transform.localPosition = new Vector3( j * IsoGridTile.TileSizeX / 2 - size.x / 2,
                                                                    i * IsoGridTile.TileSizeY - ((coords.x + j) % 2 == 0 ? 0 : IsoGridTile.TileSizeY / 2) - size.y / 2, 0);
                
                matrix[i][j].Upload(coords.x - size.x / 2 + j, coords.y - size.y / 2 + i, World.chunkSize.z / 2);
            }
        }
    }

}
