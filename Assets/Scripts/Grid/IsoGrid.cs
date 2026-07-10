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
                    float x = coords.x - size.x / 2 + j, y = coords.y - size.y / 2 + i;
                    matrix[q][i][j].transform.position = new Vector3(x, y - x / 2, q);
                    //matrix[q][i][j].transform.localPosition = new Vector3( j * IsoGridTile.TileSizeX - size.x / 2,
                    //                                                  (i + q) * IsoGridTile.TileSizeY - ((coords.x + j) % 2 == 0 ? 0 : IsoGridTile.TileSizeY / 2) - size.y / 2, q);
                    matrix[q][i][j].gameObject.SetActive(true);
                }
            }
        }
    }

    void Start()
    {
        Upload();
    }
    [SerializeField] Vector2 mouse;
    [SerializeField] Vector2 m, m1, m2, m3;
    [SerializeField] Vector2 aim; public static Vector2 GetAim() { return proxy.aim; }




    Vector2 Iso2Screen (Vector2 iso) {
        float c = Mathf.Cos(Theta/2);
        float s = Mathf.Sin(Theta/2);

        float x = IsoGridTile.TileSizeX * (iso.x * c + iso.y * c);
        float y = IsoGridTile.TileSizeY * (iso.x * -s + iso.y * s);
        return new Vector2(x, y);
    }

    Vector2 Screen2Iso (Vector2 screen) {
        float c = Mathf.Cos(Theta/2);
        float s = Mathf.Sin(Theta/2);


        float x = (screen.x / c - screen.y / s) ;
        float y = (screen.x / c + screen.y / s);
        return new Vector2(x, y);
    }


    Vector2 Screen2IsoMy (Vector2 screen) {
        /*
         ay = y / sin al;
         ox = cos al * ay;
         Ox = x - ox;
         by = cos al * Ox;
         y_iso = ay + by = y / sin  + cos * (x - cos / sin * y)
         x_iso = cos al * Ox ?
        */

        float c = Mathf.Cos(Theta/2);
        float s = Mathf.Sin(Theta/2);
        
        float isoY = screen.y / s + c * (screen.x - c * screen.y / s);
        float isoX;
        if(screen.y > 0)
            isoX = c * (screen.x - c * screen.y / s);
        else
            isoX = (-screen.y / s) + c * (screen.x + c * screen.y / s);

        return new Vector2(isoX, isoY);

    }

    void Update()
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aim = Lin2Iso(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        m1 = Iso2Screen(m);
        m2 = Screen2Iso(m1);
        m3 = Screen2IsoMy(m1);
        
        if(Vector2.Distance(Camera.main.transform.position, transform.position) >= 1.5f)
        {
            Snap(new Vector3Int((int)Camera.main.transform.position.x, (int)Camera.main.transform.position.y, 4));
        }
        click = false;
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
                    float x = coords.x - size.x / 2 + j, y = coords.y - size.y / 2 + i;
                    matrix[q][i][j].transform.position = new Vector3(x, y - x / 2, q);
                    matrix[q][i][j].Upload(coords.x - size.x / 2 + j, coords.y - size.y / 2 + i, q);
                }
            }
        }
    }

    #region Focus
        static Vector3Int focusPos = Vector3Int.zero;
        public static void SetFocus(Vector3Int pos)
        {
            proxy.matrix[focusPos.z][focusPos.y][focusPos.x].SetFocus(false);
            focusPos = pos;
            proxy.matrix[pos.z][pos.y][pos.x].SetFocus(true);
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

    #region OrderOnGrid
    public static int CalculateOrderOnGrid(Vector3 position)
    {
        return (int)(-(position.y - position.z) * 1000 - 1 + position.z + position.x + 1);
    }
    #endregion OrderOnGrid

    #region OrderOnGrid
    private static bool click;
    public static bool HasClicked(){ return click; }
    public static void Click(){ click = true; }

    #endregion OrderOnGrid

    #region 1LinAlg
        public const float L2 = (30*30 + 15*15)/32f/32f;
        public const float Theta = Mathf.PI / 3;
        private float s = Mathf.Sin(Theta/2);
        private float c = Mathf.Sin(Theta/2);
        public static Vector2 Lin2Iso(Vector2 lin)
        {
            //  a = A b
            // [a.x  = [ cos L,  cos L] [b.x
            //  a.y]   [-sin L,  sin L]  b.y]
            // {a.x = cos L * b.x + cos L * b.y
            // {a.y = -sin L * b.x + sin L * b.y
            float s = Mathf.Sin(Theta/2);
            float c = Mathf.Sin(Theta/2);
            float L = Mathf.Sqrt(L2);
            float x = (c / s * lin.y - lin.x) / (-2 * c * L);
            float y = lin.x / c / L - x;
            
            return new Vector2(x, y);
        }

    #endregion LinAlg
}
