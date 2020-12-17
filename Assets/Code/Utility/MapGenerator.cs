using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomType
{
    //AnyIrr: Camp, Body, Fight
    //OutsideIrr: Ambush, Catwalk, Traps,
    //InsideIrr: Den, CaveDung

    //Reg: Ruins, Crypt
    //Outside: Fort, Shack, Tower
    public string roomType;

    //mage, magefighter, magecaster, araach, wolf, etc
    public string[] enemyTypes;

    //random item of type, or specific
    public string[] itemTypes;

    //boxes/barrels/torchsconces/bonfire/trap/switch/button
    public string[] roomItemTypes;

}


public class WorldCell
{
    public int xValue;
    public int yValue;
    public string biomeType;
    public Dictionary<string, MapCell[,]> maps;
    public Dictionary<string, int[,]> warps;

    public WorldCell(
        int _xValue, 
        int _yValue,
        string _biomeType)
    {
        xValue = _xValue;
        yValue = _yValue;
        biomeType = _biomeType;
        maps = new Dictionary<string, MapCell[,]>();
        warps = new Dictionary<string, int[,]>();
    }
}

public class MapCell
{
    public int xValue;
    public int yValue;
    public string roomType;
    public string floorType;
    public string wallType;
    public string ceilingType;
    public string aboveFloorType;
    public string lowerFloorType;
    public string floorItemType;
    public string wallItemType;
    public string entityType;
    public string testType;

    public MapCell(
        int _xValue, 
        int _yValue, 
        string _roomType,
        string _floorType, 
        string _wallType,
        string _ceilingType,
        string _aboveFloorType,
        string _lowerFloorType,
        string _floorItemType,
        string _wallItemType,
        string _entityType,
        string _testType)
    {
        xValue = _xValue;
        yValue = _yValue;
        roomType = _roomType;
        floorType = _floorType;
        wallType = _wallType;
        ceilingType = _ceilingType;
        aboveFloorType = _aboveFloorType;
        lowerFloorType = _lowerFloorType;
        floorItemType = _floorItemType;
        wallItemType = _wallItemType;
        entityType = _entityType;
        testType = _testType;
    }
}

public class MapGenerator : MonoBehaviour
{
    public string seed;
    public bool randomSeed;
    public WorldCell[,] worldArray;
    public MapCell[,] biomeArray;

    // Start is called before the first frame update
    void Start()
    {
        GenWorld();
    }

    // Update is called once per frame
    void Update()
    {
        //try
        //{
        //    GenWorld();
        //}
        //catch
        //{
        //    Debug.Log("BIG LARGE ERROR");
        //    Debug.Break();
        //}
    }

    //WorldCell[,] GenWorld()
    void GenWorld()
    {
        int mapSize = 10;

        WorldCell[,] worldMap = new WorldCell[mapSize, mapSize];

        if (randomSeed == true)
        {
            seed = DateTime.Now.Ticks.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        string[] startBiomes = {

            "forest",
            "swamp",
            "tropical",
        };

        string[] midBiomes = {

            "canyon",
            "boreal",
            "tundra",
        };

        //startBiome chooser
        string biomeType = startBiomes[pseudoRandom.Next(startBiomes.Length)];
        //midBiome chooser
        string midBiomeType = midBiomes[pseudoRandom.Next(midBiomes.Length)];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                worldMap[x, y] = new WorldCell(x, y, "sea");
            }
        }

        //randomize land, assign beach
        WorldCell beachCell = new WorldCell(0, 0, null);
        List<WorldCell> possibleBeaches = new List<WorldCell>();

        int randFillPercent = 50;

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (x != 0 && x != mapSize - 1 && y != 0 && y != mapSize - 1)
                {
                    worldMap[x, y].biomeType = "land";
                }
            }
        }


        for (int i = 0; i < 1; i++)
        {
            for (int x = 1; x < mapSize - 1; x++)
            {
                for (int y = 1; y < mapSize - 1; y++)
                {
                    if (worldMap[x, y].biomeType == "land" && CheckNeighbours(x, y, "sea", worldMap))
                    {
                        worldMap[x, y].biomeType = (pseudoRandom.Next(0, 100) < randFillPercent) ? "land" : "temp";
                    }
                }
            }

            for (int x = 1; x < mapSize - 1; x++)
            {
                for (int y = 1; y < mapSize - 1; y++)
                {
                    if (worldMap[x, y].biomeType == "temp")
                    {
                        worldMap[x, y].biomeType = "sea";
                    }
                }
            }

            for (int x = 1; x < mapSize - 1; x++)
            {
                for (int y = 1; y < mapSize - 1; y++)
                {
                    if (worldMap[x, y].biomeType == "land")
                    {
                        int check = CheckNeighboursWorld(worldMap, "land", x, y);

                        if (check > 4)
                            worldMap[x, y].biomeType = "land";
                        else if (check < 3)
                            worldMap[x, y].biomeType = "sea";
                    }
                }
            }
        }

        for (int x = 1; x < mapSize - 1; x++)
        {
            for (int y = 1; y < mapSize - 1; y++)
            {
                if (worldMap[x, y].biomeType == "land")
                {
                    if (worldMap[x + 1, y].biomeType == "sea" ||
                        worldMap[x - 1, y].biomeType == "sea" ||
                        worldMap[x, y + 1].biomeType == "sea" ||
                        worldMap[x, y - 1].biomeType == "sea")
                    {
                        possibleBeaches.Add(worldMap[x, y]);
                    }
                }
            }
        }

        //choose beach
        beachCell = possibleBeaches[pseudoRandom.Next(possibleBeaches.Count)];
        beachCell.biomeType = "beach";

        //set first level ring
        for (int x = 1; x < mapSize - 1; x++)
        {
            for (int y = 1; y < mapSize - 1; y++)
            {
                if (worldMap[x, y].biomeType == "land")
                {
                    if (CheckNeighbours(x, y, "beach", worldMap))
                    {
                        worldMap[x, y].biomeType = startBiomes[pseudoRandom.Next(startBiomes.Length)];
                    }
                }
            }
        }

        ////set second ring
        for (int x = 1; x < mapSize - 2; x++)
        {
            for (int y = 1; y < mapSize - 2; y++)
            {
                if (worldMap[x, y].biomeType == "land")
                {
                    if (CheckNeighbours(x, y, "forest", worldMap)
                        || CheckNeighbours(x, y, "tropical", worldMap)
                        || CheckNeighbours(x, y, "swamp", worldMap))
                    {
                        worldMap[x, y].biomeType = "temp";
                    }
                }
            }
        }

        for (int x = 1; x < mapSize - 1; x++)
        {
            for (int y = 1; y < mapSize - 1; y++)
            {
                if (worldMap[x, y].biomeType == "temp")
                {
                    worldMap[x, y].biomeType = startBiomes[pseudoRandom.Next(startBiomes.Length)];
                }
            }
        }

        ////set third ring
        for (int x = 1; x < mapSize - 2; x++)
        {
            for (int y = 1; y < mapSize - 2; y++)
            {
                if (worldMap[x, y].biomeType == "land")
                {
                    if (CheckNeighbours(x, y, "forest", worldMap)
                        || CheckNeighbours(x, y, "tropical", worldMap)
                        || CheckNeighbours(x, y, "swamp", worldMap))
                    {
                        worldMap[x, y].biomeType = "temp";
                    }
                }
            }
        }

        for (int x = 1; x < mapSize - 1; x++)
        {
            for (int y = 1; y < mapSize - 1; y++)
            {
                if (worldMap[x, y].biomeType == "temp")
                {
                    worldMap[x, y].biomeType = midBiomes[pseudoRandom.Next(midBiomes.Length)];
                }
            }
        }

        ////set fourth ring
        for (int x = 1; x < mapSize - 2; x++)
        {
            for (int y = 1; y < mapSize - 2; y++)
            {
                if (worldMap[x, y].biomeType == "land")
                {
                    if (CheckNeighbours(x, y, "canyon", worldMap)
                        || CheckNeighbours(x, y, "tundra", worldMap)
                        || CheckNeighbours(x, y, "boreal", worldMap))
                    {
                        worldMap[x, y].biomeType = "temp";
                    }
                }
            }
        }

        for (int x = 1; x < mapSize - 1; x++)
        {
            for (int y = 1; y < mapSize - 1; y++)
            {
                if (worldMap[x, y].biomeType == "temp")
                {
                    worldMap[x, y].biomeType = midBiomes[pseudoRandom.Next(midBiomes.Length)];
                }
            }
        }

        possibleBeaches.Clear();
        WorldCell cityCell = new WorldCell(0, 0, null);

        //set city ring
        for (int x = 1; x < mapSize - 2; x++)
        {
            for (int y = 1; y < mapSize - 2; y++)
            {
                if (worldMap[x, y].biomeType == "land")
                {
                    if (CheckNeighboursCard(x, y, "canyon", worldMap)
                        || CheckNeighboursCard(x, y, "tundra", worldMap)
                        || CheckNeighboursCard(x, y, "boreal", worldMap))
                    {
                        possibleBeaches.Add(worldMap[x, y]);
                    }
                }
            }
        }

        cityCell = possibleBeaches[pseudoRandom.Next(possibleBeaches.Count)];
        cityCell.biomeType = "city";

        for (int x = 1; x < mapSize - 1; x++)
        {
            for (int y = 1; y < mapSize - 1; y++)
            {
                if (worldMap[x, y].biomeType == "land")
                {
                    worldMap[x, y].biomeType = "sea";
                }
            }
        }

        //generate biome map, add to map hash set with biome map key
        //biome spawn
        //biome/biome warps
        //biome/dungeon warps
        //dungeon/dungeon warps
        //dungeon/biome warps

        //generate any necessary 

        worldArray = worldMap;
        GenerateBiome(pseudoRandom, "beach", worldMap, beachCell);

        // check
        if (possibleBeaches.Count == 0)
        {
            throw new InvalidOperationException("Logfile cannot be read-only");
        }

    }

    //MapCell[,] GenerateBiome()
    void GenerateBiome(System.Random pseudoRandom, string biome, WorldCell[,] worldMap, WorldCell biomeCell)
    {
        int mapSize = 120;

        MapCell[,] biomeMap = new MapCell[mapSize, mapSize];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                biomeMap[x, y] = new MapCell(x, y, null, "cave", "cave", "yes", null, null, null, null, null, null);

                if (x != 0 && x != mapSize - 1 && y != 0 && y != mapSize - 1)
                {
                    biomeMap[x, y].wallType = "cave";
                    biomeMap[x, y].floorType = "cave";
                }
            }
        }

        //biome type cellular automata percentages

        int boundaryCavePercent = 0;
        int innerCavePercent = 0;
        int lakePercent = 0;
        int treePercent = 0;
        int grassPercent = 0;
        int bushPercent = 0;
        int fungusPercent = 0; //only in caves
        int smallRockPercent = 0;
        int flowerPercent = 0;
        int snowPercent = 0;

        if (biome == "beach" || biome == "forest")
        {
            boundaryCavePercent = 60;
            innerCavePercent = 95;
            lakePercent = 40;
            treePercent = 30;
            grassPercent = 20;
            bushPercent = 45;
            smallRockPercent = 95;
            flowerPercent = 50;
            snowPercent = 100;
        }
        else if (biome == "swamp")
        {
            boundaryCavePercent = 70;
            innerCavePercent = 98;
        }
        else if (biome == "tropical")
        {
            boundaryCavePercent = 65;
            innerCavePercent = 95;
        }
        else if (biome == "canyon")
        {
            boundaryCavePercent = 50;
            innerCavePercent = 75;
        }
        else if (biome == "boreal")
        {
            boundaryCavePercent = 60;
            innerCavePercent = 85;
        }
        else if (biome == "tundra")
        {
            boundaryCavePercent = 70;
            innerCavePercent = 98;
        }
        else if (biome == "city")
        {
            boundaryCavePercent = 70;
            innerCavePercent = 98;
        }

        //cave surrounding wall1
        for (int x = 5; x < mapSize - 5; x++)
        {
            for (int y = 5; y < mapSize - 5; y++)
            {
                //biomeMap[x, y].wallType = null;
                //biomeMap[x, y].floorType = "cave";

                biomeMap[x, y].wallType = (pseudoRandom.Next(0, 100) < boundaryCavePercent) ? null : "cave";
            }
        }

        //cave surrounding wall2
        for (int i = 0; i < 3; i++)
        {
            for (int x = mapSize - 5; x > mapSize / 2; x--)
            {
                for (int y = mapSize - 5; y > mapSize / 2; y--)
                {
                    int check = CheckNeighboursMap(biomeMap, "cave", x, y);

                    if (check > 4)
                        biomeMap[x, y].wallType = "cave";
                    else if (check < 4)
                        biomeMap[x, y].wallType = null;
                }
            }

            for (int x = 5; x < mapSize / 2; x++)
            {
                for (int y = 5; y < mapSize / 2; y++)
                {
                    int check = CheckNeighboursMap(biomeMap, "cave", x, y);

                    if (check > 4)
                        biomeMap[x, y].wallType = "cave";
                    else if (check < 4)
                        biomeMap[x, y].wallType = null;
                }
            }

            for (int x = 5; x < mapSize / 2 + 1; x++)
            {
                for (int y = mapSize - 5; y > mapSize / 2 - 1; y--)
                {
                    int check = CheckNeighboursMap(biomeMap, "cave", x, y);

                    if (check > 4)
                        biomeMap[x, y].wallType = "cave";
                    else if (check < 4)
                        biomeMap[x, y].wallType = null;
                }
            }

            for (int x = mapSize - 5; x > mapSize / 2 - 1; x--)
            {
                for (int y = 5; y < mapSize / 2 + 1; y++)
                {
                    int check = CheckNeighboursMap(biomeMap, "cave", x, y);

                    if (check > 4)
                        biomeMap[x, y].wallType = "cave";
                    else if (check < 4)
                        biomeMap[x, y].wallType = null;
                }
            }
        }

        //--cut cliffs

        //cut left cliff
        if (worldMap[biomeCell.xValue - 1, biomeCell.yValue].biomeType == "sea")
        {
            int i = 20;
            int j = 0;

            biomeMap[i, j].wallType = "temp";

            while (j < mapSize - 1 && i > 1 && i < mapSize - 1)
            {
                string[] randDir = { "botLeft", "bot", "botRight" };
                string randNext = randDir[pseudoRandom.Next(randDir.Length)];

                if (randNext == "botLeft")
                {
                    i--;
                    j++;

                    biomeMap[i, j].wallType = "temp";
                }
                else if (randNext == "bot")
                {
                    j++;

                    biomeMap[i, j].wallType = "temp";
                }
                else if (randNext == "botRight")
                {
                    i++;
                    j++;

                    biomeMap[i, j].wallType = "temp";
                }
            }

            for (int x = 0; x < mapSize - 1; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    if (biomeMap[x + 1, y].wallType == "temp")
                    {
                        biomeMap[x, y].wallType = null;
                        biomeMap[x, y].floorType = "temp";
                    }
                }
            }

            for (int x = mapSize - 2; x >= 0; x--)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    if (biomeMap[x + 1, y].floorType == "temp")
                    {
                        biomeMap[x, y].wallType = null;
                        biomeMap[x, y].floorType = "temp";
                    }
                }
            }

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    if (biomeMap[x, y].floorType == "temp")
                    {
                        biomeMap[x, y].wallType = null;
                        biomeMap[x, y].floorType = "sea";
                    }

                    if (biomeMap[x, y].wallType == "temp" && biomeMap[x, y].floorType != "sea")
                    {
                        biomeMap[x, y].floorType = "cliff";
                        biomeMap[x, y].wallType = null;
                    }
                    else if (biomeMap[x, y].wallType == "temp")
                    {
                        biomeMap[x, y].wallType = null;
                    }
                }
            }
        }

        //cut right cliff
        if (worldMap[biomeCell.xValue + 1, biomeCell.yValue].biomeType == "sea")
        {
            //cut cliff/beach
            int i = mapSize - 20;
            int j = 0;

            biomeMap[i, j].wallType = "temp";

            while (j < mapSize - 1 && i > 1 && i < mapSize - 1)
            {
                string[] randDir = { "botLeft", "bot", "botRight" };
                string randNext = randDir[pseudoRandom.Next(randDir.Length)];

                if (randNext == "botLeft")
                {
                    i--;
                    j++;

                    biomeMap[i, j].wallType = "temp";
                }
                else if (randNext == "bot")
                {
                    j++;

                    biomeMap[i, j].wallType = "temp";
                }
                else if (randNext == "botRight")
                {
                    i++;
                    j++;

                    biomeMap[i, j].wallType = "temp";
                }
            }

            for (int x = mapSize - 1; x > 1; x--)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    if (biomeMap[x - 1, y].wallType == "temp")
                    {
                        biomeMap[x, y].wallType = null;
                        biomeMap[x, y].floorType = "temp";
                    }
                }
            }

            for (int x = 1; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    if (biomeMap[x - 1, y].floorType == "temp")
                    {
                        biomeMap[x, y].wallType = null;
                        biomeMap[x, y].floorType = "temp";
                    }
                }
            }

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    if (biomeMap[x, y].floorType == "temp")
                    {
                        biomeMap[x, y].wallType = null;
                        biomeMap[x, y].floorType = "sea";
                    }

                    if (biomeMap[x, y].wallType == "temp" && biomeMap[x, y].floorType != "sea")
                    {
                        biomeMap[x, y].floorType = "cliff";
                        biomeMap[x, y].wallType = null;
                    }
                    else if (biomeMap[x, y].wallType == "temp")
                    {
                        biomeMap[x, y].wallType = null;
                    }
                }
            }
        }

        //cut top cliff
        if (worldMap[biomeCell.xValue, biomeCell.yValue + 1].biomeType == "sea")
        {
            //cut cliff/beach
            int i = 0;
            int j = mapSize - 20;

            biomeMap[i, j].wallType = "temp";

            while (i < mapSize - 1 && j > 1 && j < mapSize - 1)
            {
                string[] randDir = { "top", "right", "bot" };
                string randNext = randDir[pseudoRandom.Next(randDir.Length)];

                if (randNext == "top")
                {
                    j++;
                    i++;

                    biomeMap[i, j].wallType = "temp";
                }
                else if (randNext == "right")
                {
                    i++;

                    biomeMap[i, j].wallType = "temp";
                }
                else if (randNext == "bot")
                {
                    j--;
                    i++;

                    biomeMap[i, j].wallType = "temp";
                }
            }

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = mapSize - 1; y >= 1; y--)
                {
                    if (biomeMap[x, y - 1].wallType == "temp")
                    {
                        biomeMap[x, y].wallType = null;
                        biomeMap[x, y].floorType = "temp";
                    }
                }
            }

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 1; y < mapSize; y++)
                {
                    if (biomeMap[x, y - 1].floorType == "temp")
                    {
                        biomeMap[x, y].wallType = null;
                        biomeMap[x, y].floorType = "temp";
                    }
                }
            }

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    if (biomeMap[x, y].floorType == "temp")
                    {
                        biomeMap[x, y].wallType = null;
                        biomeMap[x, y].floorType = "sea";
                    }

                    if (biomeMap[x, y].wallType == "temp" && biomeMap[x, y].floorType != "sea")
                    {
                        biomeMap[x, y].floorType = "cliff";
                        biomeMap[x, y].wallType = null;
                    }
                    else if (biomeMap[x, y].wallType == "temp")
                    {
                        biomeMap[x, y].wallType = null;
                    }
                }
            }
        }

        //cut bot cliff
        if (worldMap[biomeCell.xValue, biomeCell.yValue - 1].biomeType == "sea")
        {
            //cut cliff/beach
            int i = 0;
            int j = 20;

            biomeMap[i, j].wallType = "temp";

            while (i < mapSize - 1 && j > 1 && j < mapSize - 1)
            {
                string[] randDir = { "top", "right", "bot" };
                string randNext = randDir[pseudoRandom.Next(randDir.Length)];

                if (randNext == "top")
                {
                    j++;
                    i++;

                    biomeMap[i, j].wallType = "temp";
                }
                else if (randNext == "right")
                {
                    i++;

                    biomeMap[i, j].wallType = "temp";
                }
                else if (randNext == "bot")
                {
                    j--;
                    i++;

                    biomeMap[i, j].wallType = "temp";
                }
            }

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize - 2; y++)
                {
                    if (biomeMap[x, y + 1].wallType == "temp")
                    {
                        biomeMap[x, y].wallType = null;
                        biomeMap[x, y].floorType = "temp";
                    }
                }
            }

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = mapSize - 2; y >= 0; y--)
                {
                    if (biomeMap[x, y + 1].floorType == "temp")
                    {
                        biomeMap[x, y].wallType = null;
                        biomeMap[x, y].floorType = "temp";
                    }
                }
            }

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    if (biomeMap[x, y].floorType == "temp")
                    {
                        biomeMap[x, y].wallType = null;
                        biomeMap[x, y].floorType = "sea";
                    }

                    if (biomeMap[x, y].wallType == "temp" && biomeMap[x, y].floorType != "sea")
                    {
                        biomeMap[x, y].floorType = "cliff";
                        biomeMap[x, y].wallType = null;
                    }
                    else if (biomeMap[x, y].wallType == "temp")
                    {
                        biomeMap[x, y].wallType = null;
                    }
                }
            }
        }

        //--gen warps

        //randomly choose left warp
        if (worldMap[biomeCell.xValue - 1, biomeCell.yValue].biomeType != "sea")
        {
            List<MapCell> potentialWarps = new List<MapCell>();

            for (int x = 3; x < 4; x++)
            {
                for (int y = mapSize / 3; y < mapSize - mapSize / 3; y++)
                {
                    if(biomeMap[x, y].floorType != "sea")
                    {
                        potentialWarps.Add(biomeMap[x, y]);
                    }
                }
            }

            MapCell warpCell = potentialWarps[pseudoRandom.Next(potentialWarps.Count)];

            warpCell.floorItemType = "leftWarp";
        }

        //randomly choose right warp
        if (worldMap[biomeCell.xValue + 1, biomeCell.yValue].biomeType != "sea")
        {
            List<MapCell> potentialWarps = new List<MapCell>();

            for (int x = mapSize - 4; x < mapSize - 3; x++)
            {
                for (int y = mapSize / 3; y < mapSize - mapSize / 3; y++)
                {
                    if (biomeMap[x, y].floorType != "sea")
                    {
                        potentialWarps.Add(biomeMap[x, y]);
                    }
                }
            }

            MapCell warpCell = potentialWarps[pseudoRandom.Next(potentialWarps.Count)];

            warpCell.floorItemType = "rightWarp";
        }

        //randomly choose top warp
        if (worldMap[biomeCell.xValue, biomeCell.yValue + 1].biomeType != "sea")
        {
            List<MapCell> potentialWarps = new List<MapCell>();

            for (int x = mapSize / 3; x < mapSize - mapSize / 3; x++)
            {
                for (int y = mapSize - 4; y < mapSize - 3; y++)
                {
                    if (biomeMap[x, y].floorType != "sea")
                    {
                        potentialWarps.Add(biomeMap[x, y]);
                    }
                }
            }

            MapCell warpCell = potentialWarps[pseudoRandom.Next(potentialWarps.Count)];

            warpCell.floorItemType = "topWarp";
        }

        //randomly choose bot warp
        if (worldMap[biomeCell.xValue, biomeCell.yValue - 1].biomeType != "sea")
        {
            List<MapCell> potentialWarps = new List<MapCell>();

            for (int x = mapSize / 3; x < mapSize - mapSize / 3; x++)
            {
                for (int y = 3; y < 4; y++)
                {
                    if (biomeMap[x, y].floorType != "sea")
                    {
                        potentialWarps.Add(biomeMap[x, y]);
                    }
                }
            }

            MapCell warpCell = potentialWarps[pseudoRandom.Next(potentialWarps.Count)];

            warpCell.floorItemType = "botWarp";
        }

        //add singular cave columns
        for (int x = 5; x < mapSize - 5; x++)
        {
            for (int y = 5; y < mapSize - 5; y++)
            {
                if (biomeMap[x, y].wallType == null && biomeMap[x, y].floorType != "sea" && CheckNeighboursMap(biomeMap, "cave", x, y) == 0)
                {
                    biomeMap[x, y].wallType = (pseudoRandom.Next(0, 100) < innerCavePercent) ? null : "temp";
                }
            }
        }

        for (int x = 5; x < mapSize - 5; x++)
        {
            for (int y = 5; y < mapSize - 5; y++)
            {
                if (biomeMap[x, y].wallType == "temp")
                {
                    biomeMap[x, y].wallType = "cave";
                }
            }
        }

        //lake
        for (int x = 5; x < mapSize - 5; x++)
        {
            for (int y = 5; y < mapSize - 5; y++)
            {
                if (biomeMap[x, y].wallType == null 
                    && biomeMap[x, y].floorType != "sea" 
                    && biomeMap[x, y].floorType != "cliff" 
                    && CheckNeighboursMap(biomeMap, "cave", x, y) == 0)
                {
                    biomeMap[x, y].floorType = (pseudoRandom.Next(0, 100) < lakePercent) ? "dirt" : "temp";
                }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int x = 1; x < mapSize - 1; x++)
            {
                for (int y = 1; y < mapSize - 1; y++)
                {
                    if (biomeMap[x, y].wallType == null 
                        && biomeMap[x, y].floorType != "sea" 
                        && biomeMap[x, y].floorType != "cliff" 
                        && CheckNeighboursMap(biomeMap, "cave", x, y) == 0
                        )
                    {
                        int check = CheckNeighboursMapFloor(biomeMap, "temp", x, y);

                        if (check > 4)
                            biomeMap[x, y].floorType = "temp";
                        else if (check < 4)
                            biomeMap[x, y].floorType = "dirt";
                    }
                }
            }
        }

        for (int x = 5; x < mapSize - 1; x++)
        {
            for (int y = 5; y < mapSize - 1; y++)
            {
                if (biomeMap[x, y].floorType == "temp")
                {
                    biomeMap[x, y].floorType = "lakeWater";
                }
            }
        }

        //add singular cave columns
        for (int x = 5; x < mapSize - 5; x++)
        {
            for (int y = 5; y < mapSize - 5; y++)
            {
                if (biomeMap[x, y].floorType == "lakeWater")
                {
                    biomeMap[x, y].wallType = (pseudoRandom.Next(0, 100) < innerCavePercent) ? null : "temp";
                }
            }
        }

        for (int x = 5; x < mapSize - 5; x++)
        {
            for (int y = 5; y < mapSize - 5; y++)
            {
                if (biomeMap[x, y].wallType == "temp")
                {
                    biomeMap[x, y].wallType = "cave";
                }
            }
        }

        //tree
        for (int x = 5; x < mapSize - 5; x++)
        {
            for (int y = 5; y < mapSize - 5; y++)
            {
                if (biomeMap[x, y].wallType == null
                    && biomeMap[x, y].floorType != "sea"
                    && biomeMap[x, y].floorType != "cliff"
                    && biomeMap[x, y].floorType != "lakeWater"
                    && CheckNeighboursMap(biomeMap, "cave", x, y) == 0
                    )
                {
                    biomeMap[x, y].testType = (pseudoRandom.Next(0, 100) < treePercent) ? null : "temp";
                }
            }
        }

        for (int i = 0; i < 2; i++)
        {
            for (int x = 1; x < mapSize - 1; x++)
            {
                for (int y = 1; y < mapSize - 1; y++)
                {
                    if (biomeMap[x, y].wallType == null
                        && biomeMap[x, y].floorType != "sea"
                        && biomeMap[x, y].floorType != "cliff"
                        && biomeMap[x, y].floorType != "lakeWater"
                       // && CheckNeighboursMap(biomeMap, "cave", x, y) == 0
                        )
                    {
                        int check = CheckNeighboursMapTest(biomeMap, "temp", x, y);

                        if (check > 4)
                            biomeMap[x, y].testType = "temp";
                        else if (check < 4)
                            biomeMap[x, y].testType = null;
                    }
                }
            }
        }

        for (int x = 5; x < mapSize - 1; x++)
        {
            for (int y = 5; y < mapSize - 1; y++)
            {
                if (biomeMap[x, y].testType == "temp")
                {
                    biomeMap[x, y].floorItemType = "tree";
                }
            }
        }

        //single tree
        for (int x = 5; x < mapSize - 5; x++)
        {
            for (int y = 5; y < mapSize - 5; y++)
            {
                biomeMap[x, y].testType = null;
            }
        }

        for (int x = 5; x < mapSize - 5; x++)
        {
            for (int y = 5; y < mapSize - 5; y++)
            {
                if (biomeMap[x, y].wallType == null
                    && biomeMap[x, y].floorType != "sea"
                    && biomeMap[x, y].floorType != "cliff"
                    && biomeMap[x, y].floorType != "lakeWater"
                    && CheckNeighboursMap(biomeMap, "cave", x, y) == 0
                    )
                {
                    biomeMap[x, y].testType = (pseudoRandom.Next(0, 100) < 90) ? null : "temp";
                }
            }
        }

        for (int x = 5; x < mapSize - 1; x++)
        {
            for (int y = 5; y < mapSize - 1; y++)
            {
                if (biomeMap[x, y].testType == "temp")
                {
                    biomeMap[x, y].floorItemType = "tree";
                }
            }
        }

        //grass
        for (int x = 5; x < mapSize - 5; x++)
        {
            for (int y = 5; y < mapSize - 5; y++)
            {
                if (biomeMap[x, y].wallType == null
                    && biomeMap[x, y].floorType != "sea"
                    && biomeMap[x, y].floorType != "cliff"
                    && biomeMap[x, y].floorType != "lakeWater")
                {
                    biomeMap[x, y].testType = (pseudoRandom.Next(0, 100) < grassPercent) ? null : "temp";
                }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int x = 1; x < mapSize - 1; x++)
            {
                for (int y = 1; y < mapSize - 1; y++)
                {
                    if (biomeMap[x, y].wallType == null
                        && biomeMap[x, y].floorType != "sea"
                        && biomeMap[x, y].floorType != "cliff"
                        && biomeMap[x, y].floorType != "lakeWater")
                    {
                        int check = CheckNeighboursMapTest(biomeMap, "temp", x, y);

                        if (check > 4)
                            biomeMap[x, y].testType = "temp";
                        else if (check < 4)
                            biomeMap[x, y].testType = null;
                    }
                }
            }
        }

        for (int x = 5; x < mapSize - 1; x++)
        {
            for (int y = 5; y < mapSize - 1; y++)
            {
                if (biomeMap[x, y].testType == "temp")
                {
                    biomeMap[x, y].aboveFloorType = "grass";
                    biomeMap[x, y].floorType = "dirt";
                }
            }
        }

        //bush
        for (int x = 5; x < mapSize - 5; x++)
        {
            for (int y = 5; y < mapSize - 5; y++)
            {
                if (biomeMap[x, y].wallType == null
                    && biomeMap[x, y].floorType != "sea"
                    && biomeMap[x, y].floorType != "cliff"
                    && biomeMap[x, y].floorType != "lakeWater"
                    && biomeMap[x, y].aboveFloorType == "grass")
                {
                    biomeMap[x, y].testType = (pseudoRandom.Next(0, 100) < bushPercent) ? null : "temp";
                }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int x = 1; x < mapSize - 1; x++)
            {
                for (int y = 1; y < mapSize - 1; y++)
                {
                    if (biomeMap[x, y].wallType == null
                        && biomeMap[x, y].floorType != "sea"
                        && biomeMap[x, y].floorType != "cliff"
                        && biomeMap[x, y].floorType != "lakeWater"
                        && biomeMap[x, y].aboveFloorType == "grass")
                    {
                        int check = CheckNeighboursMapTest(biomeMap, "temp", x, y);

                        if (check > 4)
                            biomeMap[x, y].testType = "temp";
                        else if (check < 4)
                            biomeMap[x, y].testType = null;
                    }
                }
            }
        }

        for (int x = 5; x < mapSize - 1; x++)
        {
            for (int y = 5; y < mapSize - 1; y++)
            {
                if (biomeMap[x, y].testType == "temp")
                {
                    biomeMap[x, y].aboveFloorType = "bush";
                }
            }
        }

        //flower
        for (int x = 5; x < mapSize - 5; x++)
        {
            for (int y = 5; y < mapSize - 5; y++)
            {
                biomeMap[x, y].testType = null;
            }
        }

        for (int x = 5; x < mapSize - 5; x++)
        {
            for (int y = 5; y < mapSize - 5; y++)
            {
                if (biomeMap[x, y].wallType == null
                    && biomeMap[x, y].floorType != "sea"
                    && biomeMap[x, y].floorType != "cliff"
                    && biomeMap[x, y].floorType != "lakeWater"
                    && biomeMap[x, y].aboveFloorType == "grass")
                {
                    biomeMap[x, y].testType = (pseudoRandom.Next(0, 100) < flowerPercent) ? null : "temp";
                }
            }
        }

        for (int i = 0; i < 2; i++)
        {
            for (int x = 1; x < mapSize - 1; x++)
            {
                for (int y = 1; y < mapSize - 1; y++)
                {
                    if (biomeMap[x, y].wallType == null
                        && biomeMap[x, y].floorType != "sea"
                        && biomeMap[x, y].floorType != "cliff"
                        && biomeMap[x, y].floorType != "lakeWater"
                        && biomeMap[x, y].aboveFloorType == "grass")
                    {
                        int check = CheckNeighboursMapTest(biomeMap, "temp", x, y);

                        if (check > 4)
                            biomeMap[x, y].testType = "temp";
                        else if (check < 4)
                            biomeMap[x, y].testType = null;
                    }
                }
            }
        }

        for (int x = 0; x < mapSize - 1; x++)
        {
            for (int y = 0; y < mapSize - 1; y++)
            {
                if (biomeMap[x, y].testType == "temp")
                {
                    biomeMap[x, y].aboveFloorType = "flower";
                }
            }
        }

        //smallrock
        for (int x = 5; x < mapSize - 5; x++)
        {
            for (int y = 5; y < mapSize - 5; y++)
            {
                if (biomeMap[x, y].wallType == null
                    && biomeMap[x, y].floorType != "sea"
                    && biomeMap[x, y].floorType != "cliff"
                    && biomeMap[x, y].floorType != "lakeWater")
                {
                    biomeMap[x, y].testType = (pseudoRandom.Next(0, 100) < smallRockPercent) ? null : "temp";
                }
            }
        }

        for (int x = 1; x < mapSize - 1; x++)
        {
            for (int y = 1; y < mapSize - 1; y++)
            {
                if (biomeMap[x, y].testType == "temp")
                {
                    biomeMap[x, y].aboveFloorType = "rock";
                }
            }
        }

        //snow
        for (int x = 5; x < mapSize - 5; x++)
        {
            for (int y = 5; y < mapSize - 5; y++)
            {
                biomeMap[x, y].testType = null;
            }
        }

        for (int x = 5; x < mapSize - 5; x++)
        {
            for (int y = 5; y < mapSize - 5; y++)
            {
                if (biomeMap[x, y].wallType == null
                    && biomeMap[x, y].floorType != "sea"
                    && biomeMap[x, y].floorType != "cliff"
                    && biomeMap[x, y].floorType != "lakeWater")
                {
                    biomeMap[x, y].testType = (pseudoRandom.Next(0, 100) < snowPercent) ? null : "temp";
                }
            }
        }

        for (int i = 0; i < 2; i++)
        {
            for (int x = 1; x < mapSize - 1; x++)
            {
                for (int y = 1; y < mapSize - 1; y++)
                {
                    if (biomeMap[x, y].wallType == null
                        && biomeMap[x, y].floorType != "sea"
                        && biomeMap[x, y].floorType != "cliff"
                        && biomeMap[x, y].floorType != "lakeWater")
                    {
                        int check = CheckNeighboursMapTest(biomeMap, "temp", x, y);

                        if (check > 4)
                            biomeMap[x, y].testType = "temp";
                        else if (check < 4)
                            biomeMap[x, y].testType = null;
                    }
                }
            }
        }

        for (int x = 0; x < mapSize - 1; x++)
        {
            for (int y = 0; y < mapSize - 1; y++)
            {
                if (biomeMap[x, y].testType == "temp")
                {
                    biomeMap[x, y].aboveFloorType = "snow";
                }
            }
        }

        //cut areas for interaction
        //- Camp of mages/natives. junk, urine, vomit, some caged creatures
        //- Mob of wolves/spiders/zombies/theives
        //- Dead mobs/adventurers
        //- Idyll 	ponds and some grass and forest, lone chest/npc/enemy/body
        //- Swamp mud, grass and some shallow water, swamp creatures, poisons

        //- Dismal blood, bones, charcoal, some rubble
        //- Chasm Catwalk 	narrow bridge over a chasm, possibly under fire from a turret or two
        //- Lake Walk   narrow bridge of shallow water through a lake, possibly under fire from a turret or two

        //- Shack with chest/traps/mobs
        //- Fort of mages
        //- Crypt/Ruins/Cave entrance with mobs/puzzle with dungeon warp
        //- Shrine with altar/pedestal/trap-puzzle
        //- Remnant Building carpet surrounded by ash and with some statues,
            //- item library
            //- archive/wands/potions/spells
            //- pedestal
            //- caged ally with mobs
            //- vampire lair
            //- entity spawner altar

        //Puzzles
        //- throwing button puzzle
        //- flammable barricade puzzle
        //- flood room puzzle
        //- collapsing floor puzzle
        //- levitation puzzle
        //- web climbing puzzle
        //- lava/poison moat/area puzzle
        //- explosive/poison gas closing door puzzle
        //- explosive gas with fire nearby puzzle
        //- Burning grass puzzle
        //- statue turns into gargoyle
        //- bog monsters in mud
        //- turretted/trapped area
        //- boss area
        //- Thieving Monster 

        List<MapCell> potentialRooms = new List<MapCell>();

        for (int x = 1; x < mapSize - 1; x++)
        {
            for (int y = 1; y < mapSize - 1; y++)
            {
                if (biomeMap[x, y].floorType != "sea" 
                    && biomeMap[x, y].floorItemType != "leftWarp"
                    && biomeMap[x, y].floorItemType != "rightWarp"
                    && biomeMap[x, y].floorItemType != "topWarp"
                    && biomeMap[x, y].floorItemType != "botWarp"
                    && CheckNeighboursMapFloor(biomeMap, "sea", x, y) == 0
                    && CheckNeighboursMapFloor(biomeMap, "lakeWater", x, y) == 0)
                {
                    potentialRooms.Add(biomeMap[x, y]);
                }
            }
        }

        int numRooms = 4;
        //rooms
        for(int i = 0; i < numRooms; i++)
        {
            //choosecentre
            MapCell room = potentialRooms[pseudoRandom.Next(potentialRooms.Count)];

            if (room.floorType == "cave")
            {
                room.roomType = "caveRoomC";
                room.wallType = "cave";
                room.aboveFloorType = null;
                room.floorItemType = null;
            }
            else
            {
                room.roomType = "roomC";
                room.wallType = "roomWall";
                room.aboveFloorType = null;
                room.floorItemType = null;
            }

            for (int x = 1; x < mapSize - 1; x++)
            {
                for (int y = 1; y < mapSize - 1; y++)
                {
                    biomeMap[x, y].testType = null;
                }
            }

            //iterate to build rooms
            for (int j = 0; j < 1; j++)
            {
                for (int x = 1; x < mapSize - 1; x++)
                {
                    for (int y = 1; y < mapSize - 1; y++)
                    {
                        if (CheckNeighboursMapRoom(biomeMap, "roomC", x, y) != 0)

                        {
                            if (biomeMap[x, y].floorType != "sea"
                                && biomeMap[x, y].roomType == null
                                && biomeMap[x, y].floorItemType != "leftWarp"
                                && biomeMap[x, y].floorItemType != "rightWarp"
                                && biomeMap[x, y].floorItemType != "topWarp"
                                && biomeMap[x, y].floorItemType != "botWarp")
                            {
                                biomeMap[x, y].testType = "temp1";
                            }
                        }
                        else if (CheckNeighboursMapRoom(biomeMap, "caveRoomC", x, y) != 0)
                        {
                            if (biomeMap[x, y].floorType != "sea"
                                && biomeMap[x, y].roomType == null
                                && biomeMap[x, y].floorItemType != "leftWarp"
                                && biomeMap[x, y].floorItemType != "rightWarp"
                                && biomeMap[x, y].floorItemType != "topWarp"
                                && biomeMap[x, y].floorItemType != "botWarp")
                            {
                                biomeMap[x, y].testType = "temp2";
                            }
                        }
                    }
                }

                for (int x = 1; x < mapSize - 1; x++)
                {
                    for (int y = 1; y < mapSize - 1; y++)
                    {
                        if (biomeMap[x, y].testType != null)
                        {
                            if (biomeMap[x, y].testType == "temp2")
                            {
                                biomeMap[x, y].roomType = "caveRoom";
                                biomeMap[x, y].wallType = "cave";
                                biomeMap[x, y].aboveFloorType = null;
                                biomeMap[x, y].floorItemType = null;
                            }
                            else
                            {
                                biomeMap[x, y].roomType = "room";
                                biomeMap[x, y].wallType = "roomWall";
                                biomeMap[x, y].aboveFloorType = null;
                                biomeMap[x, y].floorItemType = null;
                            }
                        }
                    }
                }
            }


            //iterate to widen spaces
            for (int j = 0; j < 2; j++)
            {
                //clear test array
                for (int x = 1; x < mapSize - 1; x++)
                {
                    for (int y = 1; y < mapSize - 1; y++)
                    {
                        biomeMap[x, y].testType = null;
                    }
                }

                //iterate to build spaces
                for (int x = 1; x < mapSize - 1; x++)
                {
                    for (int y = 1; y < mapSize - 1; y++)
                    {
                        int roomCheck = CheckNeighboursMapRoom(biomeMap, "room", x, y);
                        int caveCheck = CheckNeighboursMapRoom(biomeMap, "caveRoom", x, y);

                        if ((biomeMap[x, y].roomType == null)
                            && biomeMap[x, y].floorItemType != "leftWarp"
                            && biomeMap[x, y].floorItemType != "rightWarp"
                            && biomeMap[x, y].floorItemType != "topWarp"
                            && biomeMap[x, y].floorItemType != "botWarp")
                        {
                            if(roomCheck > 0)
                            {
                                biomeMap[x, y].testType = (pseudoRandom.Next(0, 100) < 50) ? null : "temp1";
                            }
                            else if (caveCheck > 0)
                            {
                                biomeMap[x, y].testType = (pseudoRandom.Next(0, 100) < 50) ? null : "temp2";
                            }
                        }
                    }
                }

                //assign spaces
                for (int x = 1; x < mapSize - 1; x++)
                {
                    for (int y = 1; y < mapSize - 1; y++)
                    {
                        if (biomeMap[x, y].testType != null)
                        {
                            if (biomeMap[x, y].testType == "temp2")
                            {
                                biomeMap[x, y].roomType = "caveRoom";
                                biomeMap[x, y].wallType = "cave";
                                biomeMap[x, y].aboveFloorType = null;
                                biomeMap[x, y].floorItemType = null;
                            }
                            else
                            {
                                biomeMap[x, y].roomType = "room";
                                biomeMap[x, y].wallType = "roomWall";
                                biomeMap[x, y].aboveFloorType = null;
                                biomeMap[x, y].floorItemType = null;
                            }
                        }
                    }
                }
            }

            //cull spaces
            for (int j = 0; j < 7; j++)
            {
                //clear test array
                for (int x = 1; x < mapSize - 1; x++)
                {
                    for (int y = 1; y < mapSize - 1; y++)
                    {
                        biomeMap[x, y].testType = null;
                    }
                }

                //iterate to cull spaces
                for (int x = 1; x < mapSize - 1; x++)
                {
                    for (int y = 1; y < mapSize - 1; y++)
                    {
                        int roomCheck = CheckNeighboursMapRoom(biomeMap, "room", x, y);
                        int caveCheck = CheckNeighboursMapRoom(biomeMap, "caveRoom", x, y);

                        if ((biomeMap[x, y].roomType == null || biomeMap[x, y].roomType == "room" || biomeMap[x, y].roomType == "caveRoom")
                            && biomeMap[x, y].floorItemType != "leftWarp"
                            && biomeMap[x, y].floorItemType != "rightWarp"
                            && biomeMap[x, y].floorItemType != "topWarp"
                            && biomeMap[x, y].floorItemType != "botWarp")
                        {
                            if (roomCheck > caveCheck)
                            {
                                if (roomCheck > 3)
                                    biomeMap[x, y].testType = "temp1";
                                else if (roomCheck < 3)
                                    biomeMap[x, y].testType = null;
                            }
                            else if (roomCheck < caveCheck)
                            {
                                if (caveCheck > 3)
                                    biomeMap[x, y].testType = "temp2";
                                else if (caveCheck < 3)
                                    biomeMap[x, y].testType = null;
                            }
                        }
                    }
                }

                //assign spaces
                for (int x = 1; x < mapSize - 1; x++)
                {
                    for (int y = 1; y < mapSize - 1; y++)
                    {
                        if (biomeMap[x, y].testType != null)
                        {
                            if (biomeMap[x, y].testType == "temp2")
                            {
                                biomeMap[x, y].roomType = "caveRoom";
                                biomeMap[x, y].wallType = "cave";
                                biomeMap[x, y].aboveFloorType = null;
                                biomeMap[x, y].floorItemType = null;
                            }
                            else
                            {
                                biomeMap[x, y].roomType = "room";
                                biomeMap[x, y].wallType = "roomWall";
                                biomeMap[x, y].aboveFloorType = null;
                                biomeMap[x, y].floorItemType = null;
                            }
                        }
                    }
                }
            }

            //set spaces as unique
            for (int x = 1; x < mapSize - 1; x++)
            {
                for (int y = 1; y < mapSize - 1; y++)
                {
                    if (biomeMap[x, y].roomType == "roomC")
                    {
                        biomeMap[x, y].roomType = "roomC" + i.ToString();
                    }
                    else if (biomeMap[x, y].roomType == "caveRoomC")
                    {
                        biomeMap[x, y].roomType = "caveRoomC" + i.ToString();
                    }
                    else if (biomeMap[x, y].roomType == "room")
                    {
                        biomeMap[x, y].roomType = "room" + i.ToString();
                    }
                    else if (biomeMap[x, y].roomType == "caveRoom")
                    {
                        biomeMap[x, y].roomType = "caveRoom" + i.ToString();
                    }
                }
            }
        }

        biomeArray = biomeMap;

        //error check

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (biomeMap[x, y].floorItemType != null && biomeMap[x, y].floorType == "sea")
                {
                    throw new InvalidOperationException("Logfile cannot be read-only");
                }
            }
        }
    }

    //MapCell[,] GenerateDungeon()
    //{

    //}

    int CheckNeighboursWorld(WorldCell[,] worldArray, string check, int x, int y)
    {
        int trueNeighbours = 0;

        if (worldArray[x + 1, y + 1].biomeType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x + 1, y - 1].biomeType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x + 1, y].biomeType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x - 1, y + 1].biomeType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x - 1, y - 1].biomeType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x - 1, y].biomeType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x, y + 1].biomeType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x, y - 1].biomeType == check)
        {
            trueNeighbours++;
        }

        return trueNeighbours;
    }

    int CheckNeighboursMap(MapCell[,] worldArray, string check, int x, int y)
    {
        int trueNeighbours = 0;

        if (worldArray[x + 1, y + 1].wallType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x + 1, y - 1].wallType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x + 1, y].wallType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x - 1, y + 1].wallType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x - 1, y - 1].wallType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x - 1, y].wallType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x, y + 1].wallType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x, y - 1].wallType == check)
        {
            trueNeighbours++;
        }

        return trueNeighbours;
    }

    int CheckNeighboursMapFloor(MapCell[,] worldArray, string check, int x, int y)
    {
        int trueNeighbours = 0;

        if (worldArray[x + 1, y + 1].floorType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x + 1, y - 1].floorType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x + 1, y].floorType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x - 1, y + 1].floorType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x - 1, y - 1].floorType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x - 1, y].floorType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x, y + 1].floorType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x, y - 1].floorType == check)
        {
            trueNeighbours++;
        }

        return trueNeighbours;
    }

    int CheckNeighboursMapRoom(MapCell[,] worldArray, string check, int x, int y)
    {
        int trueNeighbours = 0;

        if (worldArray[x + 1, y + 1].roomType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x + 1, y - 1].roomType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x + 1, y].roomType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x - 1, y + 1].roomType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x - 1, y - 1].roomType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x - 1, y].roomType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x, y + 1].roomType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x, y - 1].roomType == check)
        {
            trueNeighbours++;
        }

        return trueNeighbours;
    }

    int CheckNeighboursMapTest(MapCell[,] worldArray, string check, int x, int y)
    {
        int trueNeighbours = 0;

        if (worldArray[x + 1, y + 1].testType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x + 1, y - 1].testType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x + 1, y].testType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x - 1, y + 1].testType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x - 1, y - 1].testType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x - 1, y].testType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x, y + 1].testType == check)
        {
            trueNeighbours++;
        }
        if (worldArray[x, y - 1].testType == check)
        {
            trueNeighbours++;
        }

        return trueNeighbours;
    }

    bool CheckNeighbours(int x, int y, string biome, WorldCell[,] worldArray)
    {
        bool beachNeighbour = false;

        string top = worldArray[x, y + 1].biomeType;
        string topRight = worldArray[x + 1, y + 1].biomeType;
        string left = worldArray[x - 1, y].biomeType;
        string topLeft = worldArray[x - 1, y + 1].biomeType;
        string bot = worldArray[x, y - 1].biomeType;
        string botRight = worldArray[x + 1, y - 1].biomeType;
        string right = worldArray[x + 1, y].biomeType;
        string botleft = worldArray[x - 1, y - 1].biomeType;

        if (
            top == biome ||
            topRight == biome ||
            left == biome ||
            topLeft == biome ||
            bot == biome ||
            botRight == biome ||
            right == biome ||
            botleft == biome)
        {
            beachNeighbour = true;
        }

        return beachNeighbour;
    }

    bool CheckNeighboursCard(int x, int y, string biome, WorldCell[,] worldArray)
    {
        bool beachNeighbour = false;

        string top = worldArray[x, y + 1].biomeType;
        string left = worldArray[x - 1, y].biomeType;
        string bot = worldArray[x, y - 1].biomeType;
        string right = worldArray[x + 1, y].biomeType;

        if (
            top == biome ||
            left == biome ||
            bot == biome ||
            right == biome)
        {
            beachNeighbour = true;
        }

        return beachNeighbour;
    }

    void OnDrawGizmos()
    {
        if (worldArray != null)
        {
            for (int x = 0; x < worldArray.GetLength(0); x++)
            {
                for (int y = 0; y < worldArray.GetLength(0); y++)
                {
                    if (worldArray[x, y].biomeType == null)
                    {
                        Gizmos.color = Color.grey;
                    }

                    if (worldArray[x, y].biomeType == "sea")
                    {
                        Gizmos.color = Color.blue;
                    }

                    if (worldArray[x, y].biomeType == "land")
                    {
                        Gizmos.color = Color.green;
                    }

                    if (worldArray[x, y].biomeType == "beach")
                    {
                        Gizmos.color = Color.yellow;
                    }

                    if (worldArray[x, y].biomeType == "forest")
                    {
                        Gizmos.color = new Color(0.3f, 0.7f, 0.5f); //forest
                    }

                    if (worldArray[x, y].biomeType == "tropical")
                    {
                        Gizmos.color = new Color(0.6f, 0.8f, 0.5f); //tropjungle
                    }

                    if (worldArray[x, y].biomeType == "swamp")
                    {
                        Gizmos.color = new Color(0.4f, 0.6f, 0.5f); //swamp
                    }

                    if (worldArray[x, y].biomeType == "canyon")
                    {
                        Gizmos.color = new Color(0.39f, 0.26f, 0.12f); //canyon
                    }

                    if (worldArray[x, y].biomeType == "boreal")
                    {
                        Gizmos.color = Color.gray;
                    }

                    if (worldArray[x, y].biomeType == "tundra")
                    {
                        Gizmos.color = Color.white;
                    }

                    if (worldArray[x, y].biomeType == "city")
                    {
                        Gizmos.color = Color.black;
                    }

                    Vector3 pos = new Vector3(-worldArray.GetLength(0) / 2 + x, 0, -worldArray.GetLength(0) * 2 + y);
                    Gizmos.DrawCube(pos, new Vector3(1, 0.5f, 1));
                }
            }
        }

        if (biomeArray != null)
        {
            //floor
            for (int x = 0; x < biomeArray.GetLength(0); x++)
            {
                for (int y = 0; y < biomeArray.GetLength(0); y++)
                {
                    if (biomeArray[x, y].floorType == "cliff")
                    {
                        Gizmos.color = Color.yellow;
                    }

                    else if (biomeArray[x, y].floorType == "sea")
                    {
                        Gizmos.color = Color.blue;
                    }

                    else if (biomeArray[x, y].floorType == "lakeWater")
                    {
                        Gizmos.color = Color.cyan;
                    }

                    else if (biomeArray[x, y].floorType == "cave")
                    {
                        Gizmos.color = Color.white;
                    }

                    else if (biomeArray[x, y].floorType == "dirt")
                    {
                        Gizmos.color = new Color(0.39f, 0.26f, 0.12f);
                    }
                    else
                    {
                        //Debug.Log(biomeArray[x, y].floorType);
                        //Debug.Break();
                        Gizmos.color = Color.black;
                    }

                    Vector3 pos = new Vector3(-biomeArray.GetLength(0) + x, 0, -biomeArray.GetLength(0) * 2 + y);
                    Gizmos.DrawCube(pos, new Vector3(1, 0.5f, 1));
                }
            }

            //abovefloor
            for (int x = 0; x < biomeArray.GetLength(0); x++)
            {
                for (int y = 0; y < biomeArray.GetLength(0); y++)
                {
                    if (biomeArray[x, y].aboveFloorType == "grass")
                    {
                        Gizmos.color = new Color(0.3f, 0.7f, 0.5f); //forest
                    }

                    else if (biomeArray[x, y].aboveFloorType == "bush")
                    {
                        Gizmos.color = new Color(0.4f, 0.6f, 0.5f); //swamp
                    }

                    else if (biomeArray[x, y].aboveFloorType == "rock")
                    {
                        Gizmos.color = Color.gray;
                    }

                    else if (biomeArray[x, y].aboveFloorType == "flower")
                    {
                        Gizmos.color = Color.magenta;
                    }

                    else if (biomeArray[x, y].aboveFloorType == "snow")
                    {
                        Gizmos.color = Color.white;
                    }
                    else
                    {
                        //Debug.Log(biomeArray[x, y].aboveFloorType);
                        //Debug.Break();
                        Gizmos.color = Color.clear;
                    }

                    Vector3 pos = new Vector3(-biomeArray.GetLength(0) + x, 1, -biomeArray.GetLength(0) * 2 + y);
                    Gizmos.DrawCube(pos, new Vector3(0.75f, 0.5f, 0.75f));
                }
            }

            //wall
            for (int x = 0; x < biomeArray.GetLength(0); x++)
            {
                for (int y = 0; y < biomeArray.GetLength(0); y++)
                {
                    //if (biomeArray[x, y].wallType == "borderWall")
                    //{
                    //    Gizmos.color = Color.white;
                    //}

                    if (biomeArray[x, y].wallType == "cave")
                    {
                        Gizmos.color = Color.black;
                    }
                    else
                    {
                        Gizmos.color = Color.clear;
                    }

                    Vector3 pos = new Vector3(-biomeArray.GetLength(0) + x, 2, -biomeArray.GetLength(0) * 2 + y);
                    Gizmos.DrawCube(pos, new Vector3(1, 0.5f, 1));
                }
            }

            //item
            for (int x = 0; x < biomeArray.GetLength(0); x++)
            {
                for (int y = 0; y < biomeArray.GetLength(0); y++)
                {
                    if (biomeArray[x, y].floorItemType == "leftWarp"
                        || biomeArray[x, y].floorItemType == "rightWarp"
                        || biomeArray[x, y].floorItemType == "topWarp"
                        || biomeArray[x, y].floorItemType == "botWarp")
                    {
                        Gizmos.color = Color.blue;
                    }

                    else if (biomeArray[x, y].floorItemType == "tree")
                    {
                        Gizmos.color = new Color(0.39f, 0.26f, 0.12f);
                    }
                    else
                    {
                        Gizmos.color = Color.clear;
                    }

                    Vector3 pos = new Vector3(-biomeArray.GetLength(0) + x, 3, -biomeArray.GetLength(0) * 2 + y);
                    Gizmos.DrawCube(pos, new Vector3(0.5f, 0.5f, 0.5f));
                }
            }

            //room
            for (int x = 0; x < biomeArray.GetLength(0); x++)
            {
                for (int y = 0; y < biomeArray.GetLength(0); y++)
                {
                    string type = "";
                    if (biomeArray[x, y].roomType != null)
                    {
                        type = biomeArray[x, y].roomType.Remove(biomeArray[x, y].roomType.Length - 1, 1);
                        //Debug.Log($"Original: {biomeArray[x, y].roomType}");
                        //Debug.Log($"Copy: {type}");
                    }

                    if (type == "room")
                    {
                        Gizmos.color = Color.red;
                    }

                    else if (type == "caveRoom")
                    {
                        Gizmos.color = Color.yellow;
                    }
                    else
                    {
                        //Debug.Log(biomeArray[x, y].floorType);
                        //Debug.Break();
                        Gizmos.color = Color.clear;
                    }

                    Vector3 pos = new Vector3(-biomeArray.GetLength(0) + x, 4, -biomeArray.GetLength(0) * 2 + y);
                    Gizmos.DrawCube(pos, new Vector3(1, 0.5f, 1));
                }
            }

        }
    }
}
