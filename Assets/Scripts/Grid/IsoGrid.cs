using UnityEditor.Experimental.GraphView;
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
                    //float x = coords.x - size.x / 2 + j, y = coords.y - size.y / 2 + i;
                    //matrix[q][i][j].transform.position = new Vector3(x, y - x / 2 + q, q); //- x / 2
                    //matrix[q][i][j].transform.localPosition = new Vector3( j * IsoGridTile.TileSizeX - size.x / 2,
                    //                                                  (i + q) * IsoGridTile.TileSizeY - ((coords.x + j) % 2 == 0 ? 0 : IsoGridTile.TileSizeY / 2) - size.y / 2, q);
                    matrix[q][i][j].gameObject.SetActive(true);
                }
            }
        }
    }

    void Start()
    {
        Snap(Camera.main.transform.position);
        //Upload();
    }
    [SerializeField] Vector2 mouse;
    
    [SerializeField] Vector2 aim; public static Vector2 GetAim() { return proxy.aim; }
    Vector3Int aimedTile; public static Vector3Int GetAimedTile() { return proxy.aimedTile; }


    

    void Update()
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //aim = LinToIso(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if(PlayerControlUnit.proxy != null){
            matrix[aimedTile.z][aimedTile.y][aimedTile.x].ResetColor();
            matrix[aimedTile.z][aimedTile.y][aimedTile.x].SetFocus(false);
            aimedTile = IsoToTile(PlayerControlUnit.proxy.targetTile);
            matrix[aimedTile.z][aimedTile.y][aimedTile.x].SetFocus(true);
            matrix[aimedTile.z][aimedTile.y][aimedTile.x].SetColor(Color.blue);
        }
        float x = Camera.main.transform.position.x, y = Camera.main.transform.position.y;
        if(Vector2.Distance(new Vector2(x, y + x / 2), transform.position) >= 1.5f)
        {
            Snap(Camera.main.transform.position);
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
                    int x = proxy.coords.x - proxy.size.x / 2 + j, y = proxy.coords.y - proxy.size.y + i;
                    if(i < j / 2)
                        y += proxy.size.y;

                    proxy.matrix[q][i][j].Upload(x, y, q);
                }
            }
        }
    }
    void Snap(Vector2 pos)
    {        
        pos = new Vector2(pos.x / IsoGridTile.TileSizeX, pos.y / IsoGridTile.TileSizeY);
        coords = new Vector3Int((int)pos.x, (int)(pos.y + pos.x / 2), 4);// new Vector3Int((int)pos.x, (int)pos.y, 4);
        transform.position = new Vector3(coords.x * IsoGridTile.TileSizeX, coords.y * IsoGridTile.TileSizeY, 0);
        for(int q = 0; q < World.chunkSize.z; q++)
        {
            for(int i = 0; i < size.y; i++)
            {
                for(int j = 0; j < size.x; j++)
                {
                    //int I = i < j ? size.y - j + i : i;
                    float x = coords.x - size.x / 2 + j, y = coords.y - size.y + i;
                    if(i < j / 2)
                        y += size.y;

                    matrix[q][i][j].transform.position = new Vector3(x * IsoGridTile.TileSizeX, (y - x / 2 + q) * IsoGridTile.TileSizeY, q); //
                    matrix[q][i][j].Upload((int)x, (int)y, q);
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
        return (int)(-(position.y - 2*position.z) * 100 - 1 + position.z + position.x + 1);
    }
    #endregion OrderOnGrid

    #region Click
    private static bool click;
    public static bool HasClicked(){ return click; }
    public static void Click(){ click = true; Upload(); }

    #endregion Click

    #region 1LinAlg

        public static Vector3Int IsoToTile(Vector3Int pos)
        {
            //                    float x = coords.x - size.x / 2 + j, y = coords.y - size.y + i;
            int j = pos.x - proxy.coords.x + proxy.size.x / 2;
            int i = pos.y - proxy.coords.y + proxy.size.y;
            
            if(i >= proxy.size.y)
                i -= proxy.size.y;
            
            return new Vector3Int(j, i, pos.z); //
        }
        public static Vector3 IsoToLin(Vector3Int pos)
        {
            //                    float x = coords.x - size.x / 2 + j, y = coords.y - size.y + i;
            
            
            return new Vector3(pos.x * IsoGridTile.TileSizeX, (pos.y - pos.x / 2 + pos.z) * IsoGridTile.TileSizeY, pos.z); //
        }

        public static Vector3Int LinToIso(Vector2 lin, int z)
        {
            Vector3Int c = LinToIso(lin);
            return new Vector3Int(c.x, c.y, z);
        }

        public static Vector3Int LinToIso(Vector2 lin)
        {
            Vector3Int c = LinToTile(lin, true);
            float x = proxy.coords.x - proxy.size.x / 2 + c.x, y = proxy.coords.y - proxy.size.y + c.y;
            if(c.y < c.x / 2)
                y += proxy.size.y;
            
            return new Vector3Int((int)x, (int)y, (int)World.GetBlockZ(new Vector2Int((int)x, (int)y)));
        }

        static Vector3Int LinToTile(Vector2 lin, bool flat = false)
        {
            lin = new Vector2(lin.x / IsoGridTile.TileSizeX, lin.y / IsoGridTile.TileSizeY);
            lin += new Vector2(0.5f / IsoGridTile.TileSizeX, -0.5f * IsoGridTile.TileSizeY); //  ((int)lin.x % 2 == 0) ? -0.5f : 
            // x = coords.x - size.x / 2 + j,
            // y = coords.y - size.y + i;
            // if(i < j / 2){ y += size.y;
            // x, y - x/2
            int i, j, q;
            j = (int)lin.x - proxy.coords.x + proxy.size.x / 2;
            lin += new Vector2(0f, j % 2 != proxy.coords.x % 2 ? 0f : -0.5f); //  ((int)lin.x % 2 == 0) ? -0.5f : 
            i = (int)lin.y + (int)lin.x / 2 - proxy.coords.y + proxy.size.y;

            float dx =  lin.x % 1f - 0.5f;
            float dy = (lin.y + (int)lin.x / 2) % 1f - 0.5f;

            //dx =  lin.x - matrix[0][i][j].transform.position.x ;
            //dy =  lin.y - (matrix[0][i][j].transform.position.y + 1.5f);

            //Debug.Log(dx + " : " + dy);
            
            if (dx > 0 && dy > 0)
                if(1 - dx < 2 * dy)
                {
                    i++;
                    j++;
                }
            if (dx < 0 && dy > 0)
                if(1 + dx < 2 * dy)  j--;
            if (dx > 0 && dy < 0)
                if(1 - dx < -2 * dy) j++;
            if (dx < 0 && dy < 0)
                if(1 + dx < -2 * dy)
                {
                    i--;
                    j--;
                }
            
            if(i >= proxy.size.y)
                i -= proxy.size.y;


            j = Mathf.Min(Mathf.Max(j, 0), proxy.size.x - 1);
            i = Mathf.Min(Mathf.Max(i, 0), proxy.size.y - 1);
            if(!flat)
            {
                q = Mathf.Min(World.chunkSize.z - 1, i);
                i -= q;
                while (q > 0 && proxy.matrix[q][i][j].IsEmpty())
                {
                    q--;
                    i++;
                }
                i = Mathf.Min(Mathf.Max(i, 0), proxy.size.y - 1);
                q = Mathf.Min(Mathf.Max(q, 0), World.chunkSize.z - 1);
                return new Vector3Int(j, i, q);
            }
            return new Vector3Int(j, i, 0);
        }

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
    #endregion LinAlg
}
