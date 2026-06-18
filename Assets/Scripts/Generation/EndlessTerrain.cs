using JetBrains.Annotations;
using MarchingBytes;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UIElements;

public class EndlessTerain : MonoBehaviour
{
    [SerializeField]
    public static float maxViewDst = 300;               // дальность прорисовки чанков и деревьев


    [SerializeField]
    public static float existDst = 2000;                // расстояние, на котором они существуют


    public Transform viewer;

    const float updateThreshold = 150f;
    const float sqrUpdateThreshold = updateThreshold * updateThreshold;

    const float deleteThreshold = 500f;
    const float sqrDeleteThreshold = deleteThreshold * deleteThreshold;

    public static Vector2 viewerPosition;
    static Vector2 viewerPositionOld;
    static Vector2 viewerPositionOldForDelete;

    static MapGenerator mapGenerator;
    int chunkSize;
    int chunksVisibleInViewDst;
    public Material mapMaterial;

    public static int treeSpacing = 12;

    static Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

    static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    private void Start()
    {
        mapGenerator = FindAnyObjectByType<MapGenerator>();
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
        UpdateVisibleChunks();
    }
    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.y);
        if ((viewerPosition-viewerPositionOld).sqrMagnitude > sqrUpdateThreshold)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
        if ((viewerPosition - viewerPositionOldForDelete).sqrMagnitude > sqrDeleteThreshold)
        {
            viewerPositionOldForDelete = viewerPosition;
            UpdateExistingChunks();
        }
    }
    void UpdateVisibleChunks()
    {
        Dictionary<Vector2, TerrainChunk> chunksForDelete = new Dictionary<Vector2, TerrainChunk>();
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            chunksForDelete.Add(terrainChunksVisibleLastUpdate[i].position, terrainChunksVisibleLastUpdate[i]);
            //terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if (chunksForDelete.ContainsKey(viewedChunkCoord))
                    {
                        chunksForDelete.Remove(viewedChunkCoord);
                    }
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform, mapMaterial));
                }
            }
        }
        foreach (Vector2 v in chunksForDelete.Keys.ToArray())
        {
            chunksForDelete[v].UpdateTerrainChunk();
            //chunksForDelete[v].SetVisible(false);
        }
        chunksForDelete.Clear();
    }

    void UpdateExistingChunks()
    {
        //for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        //{
        //    terrainChunksExistedLastUpdate[i].SetVisible(false);
        //}
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        foreach (Vector2 chunkPosition in terrainChunkDictionary.Keys.ToList())     // ToList создает копию списка, поэтому после удаления старых чанков ничего не сломается
        {
            TerrainChunk chunk = terrainChunkDictionary[chunkPosition];
            if (!chunk.Exists())
            {
                terrainChunkDictionary.Remove(chunkPosition);
                chunk.Delete();
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        public Vector2 position;
        Bounds bounds;
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        NatureObject[] natureObjects;
        GameObject[] natureGameObjects;
        public bool hasMapData = false;

        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, position.y, 0);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            SetVisible(false);

            mapGenerator.RequestMapData(position, OnMapDataRecieved);
        }

        void OnMapDataRecieved(MapData mapData)
        {
            mapGenerator.RequestMeshData(mapData, OnMeshDataRecieved);
            Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colourMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;
            // Генерация деревьев (хардкод, мб исправить)
            
            natureObjects = ObjectsGenerator.GenerateObjects(mapData.heightMap, 50, position, treeSpacing);
            natureGameObjects = new GameObject[natureObjects.Length];

            hasMapData = true;
            UpdateTerrainChunk();
        }

        void OnMeshDataRecieved(MeshData meshData)
        {
            meshFilter.mesh = meshData.CreateMesh();
        }

        public void UpdateTerrainChunk()
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDstFromNearestEdge <= maxViewDst;

            if (natureGameObjects.Length!=0)            //(natureGameObjects!=null)
            {
                if (visible && natureGameObjects[0] == null)
                {
                    for (int i = 0; i < natureObjects.Length; i++)  // достаем деревья из пула
                    {
                        natureGameObjects[i] = EasyObjectPool.instance.GetObjectFromPool(natureObjects[i].objectName, natureObjects[i].position, Quaternion.identity);
                        //obj.transform.parent = meshObject.transform;      // если сделать родительским объектом чанк, пул перестанет реагировать на дерево
                    }
                }
                else if (!visible && natureGameObjects[0] != null)
                {
                    if (natureGameObjects != null)                          // если есть деревья
                    {
                        for (int i = 0; i < natureGameObjects.Length; i++)  // возвращаем все объекты в пул, если чанк не в поле зрения (данные о них остаются, пока существует чанк)
                        {                                                   // стоит сохранять мапдаты в долгосрочную память, чтобы удалять чанки и создавать точно такие же
                            if (natureGameObjects[i] != null)
                            {
                                EasyObjectPool.instance.ReturnObjectToPool(natureGameObjects[i]);
                                natureGameObjects[i] = null;
                            }
                        }
                    }

                }
            }
            

            if (visible)
            {
                if (!terrainChunksVisibleLastUpdate.Contains(this))
                {
                    terrainChunksVisibleLastUpdate.Add(this);
                }
            }
            SetVisible(visible);
        }
        public bool Exists()
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool exists = viewerDstFromNearestEdge <= existDst;
            return exists;
        }
        public void Delete()
        {
            Destroy(meshObject);
        }
        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }
        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }
}
