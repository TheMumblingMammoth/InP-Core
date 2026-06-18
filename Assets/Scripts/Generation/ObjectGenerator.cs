using UnityEngine;
using System.Threading;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;

public static class ObjectsGenerator
{

    static string[] GenerateNames(string prefix, int amount)
    {
        string[] names = new string[amount];
        for (int i = 0; i < amount; i++)
        {
            names[i] = prefix + " " + i.ToString();
        }
        return names;
    }


    static int numberOfTrees = 7;                                      // количество префабов объектов
    static int numberOfStones = 6;

    static string[] treeNames = GenerateNames("tree", numberOfTrees);       // разделительный пробел добавляется внутри функции
    static string[] stoneNames = GenerateNames("stone", numberOfStones);       // разделительный пробел добавляется внутри функции
    public static NatureObject[] GenerateObjects(float[,] heightMap, int density, Vector2 offset, int spacing)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        int simplifiedWidth = ((width - 1) / spacing);
        int simplifiedHeight = ((height - 1) / spacing);
        //float bottomLeftX = (width - 1) / 2f;
        //float bottomLeftY = (height - 1) / 2f;
        float topLeftX = (width - 1) / -2f;
        float topLeftY = (height - 1) / 2f;
        int amount = Random.Range(density-5, density+5);
        List<NatureObject> objects = new List<NatureObject>();
        List<Vector2> filled = new List<Vector2>();

        for (int i = 0; i < amount; i++)
        {
            int x = Random.Range(0, simplifiedWidth) * spacing;
            int y = Random.Range(0, simplifiedHeight) * spacing;
            Vector2 pos = offset + new Vector2(topLeftX + x, topLeftY - y);
            float pointHeight = heightMap[x, y];
            if (pointHeight > 0.1f && !filled.Contains(pos))     //0.1f - высота, начиная с которой растут деревья
            {
                float randChoice = Random.Range(0f, 1f);        // случайная дробь от 0 до 1
                string objectName;
                if (randChoice > 0.85f)                          // Если попадется число больше 0.85 (меньший шанс) - выбираем камень из списка камней
                {
                    objectName = stoneNames[Random.Range(0, stoneNames.Length - 1)];
                }
                else                                            // иначе - дерево
                {
                    objectName = treeNames[Random.Range(0, treeNames.Length - 1)];
                }
                objects.Add(new NatureObject(objectName, pos));
                filled.Add(pos);
                //objects.Add(new NatureObject(natureObjectNames[Random.Range(0, natureObjectNames.Length - 1)], new Vector2(x, y) + offset - new Vector2(bottomLeftX, bottomLeftY)));
            }
        }
        return objects.ToArray();
    }
}


public class NatureObject
{
    public string objectName;
    public Vector2 position;
    public NatureObject(string objectName, Vector2 position)
    {
        this.objectName = objectName;
        this.position = position;
    }
    //public GameObject prefab;
}
