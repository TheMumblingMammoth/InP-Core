using UnityEngine;
public class IsoGrid : MonoBehaviour
{
    private static IsoGrid proxy;
    [SerializeField] IsoGridTile sample;
    [SerializeField] Vector2Int size;
    [SerializeField] Vector3Int coords = new Vector3Int(0, 0, 4);
    IsoGridTile [][][] matrix;

    void Awake()
    {
        proxy = this;
        matrix = new IsoGridTile[World.chunkSize.z][][];
        for (int q = 0; q < World.chunkSize.z; q++){
            matrix[q] = new IsoGridTile[size.y][];
            for (int i = 0; i < size.y; i++)
            {
                matrix[q][i] = new IsoGridTile[size.x];
                for (int j = 0; j < size.x; j++)
                {
                    matrix[q][i][j] = Instantiate(sample);
                    matrix[q][i][j].transform.SetParent(transform);
                    matrix[q][i][j].transform.localPosition = new Vector3( j * IsoGridTile.TileSizeX - size.x / 2,
                                                                        (i + q) * IsoGridTile.TileSizeY - ((coords.x + j) % 2 == 0 ? 0 : IsoGridTile.TileSizeY / 2) - size.y / 2, q);
                    matrix[q][i][j].gameObject.SetActive(true);
                }
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
            Snap(new Vector3Int((int)Camera.main.transform.position.x, (int)Camera.main.transform.position.y, 4));
        }
    }
    
    public static void Upload()
    {
        for(int q = 0; q < World.chunkSize.z; q++)
        {
            for(int i = 0; i < proxy.size.y; i++)
            {
                for(int j = 0; j < proxy.size.x; j++)
                {
                    proxy.matrix[q][i][j].Upload(proxy.coords.x - proxy.size.x / 2 + j, proxy.coords.y - proxy.size.y /2 + i, q);
                }
            }
        }
    }
    void Snap(Vector3Int coords)
    {
        this.coords = coords;
        transform.position = new Vector3(coords.x, coords.y, 0);
        for(int q = 0; q < World.chunkSize.z; q++)
        {
            for(int i = 0; i < size.y; i++)
            {
                for(int j = 0; j < size.x; j++)
                {
                    matrix[q][i][j].transform.localPosition = new Vector3( j * IsoGridTile.TileSizeX - size.x / 2,
                                                                        (i+q) * IsoGridTile.TileSizeY - ((coords.x + j) % 2 == 0 ? 0 : IsoGridTile.TileSizeY / 2) - size.y / 2,
                                                                        0);
                    
                    matrix[q][i][j].Upload(coords.x - size.x / 2 + j, coords.y - size.y / 2 + i, q);
                }
            }
        }
    }

    #region Focus
        [SerializeField] GameObject ff;
        public static void SetFocus(Vector2 pos)
        {
            proxy.ff.transform.position = new Vector3(pos.x, pos.y, 0);
        }
    #endregion

    public static Vector3Int GoNE(Vector3Int pos)
    {
        return new Vector3Int(pos.x + 1, pos.y + (pos.x % 2 == 0 ? 1 : 0), pos.z);
    }

    public static Vector3Int GoNW(Vector3Int pos)
    {
        return new Vector3Int(pos.x - 1, pos.y + (pos.x % 2 == 0 ? 1 : 0), pos.z);
    }

    public static Vector3Int GoSE(Vector3Int pos)
    {
        return new Vector3Int(pos.x + 1, pos.y - (pos.x % 2 == 0 ? 0 : 1), pos.z);
    }

    public static Vector3Int GoSW(Vector3Int pos)
    {
        return new Vector3Int(pos.x - 1, pos.y - (pos.x % 2 == 0 ? 0 : 1), pos.z);
    }
}
