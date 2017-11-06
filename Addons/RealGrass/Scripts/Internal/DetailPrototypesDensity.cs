﻿// Project:         Real Grass for Daggerfall Unity
// Project:         Real Grass for Daggerfall Unity
// Web Site:        http://forums.dfworkshop.net/viewtopic.php?f=14&t=17
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/TheLacus/realgrass-du-mod
// Original Author: TheLacus
// Contributors:    

using UnityEngine;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game;

namespace RealGrass
{
    /// <summary>
    /// Set density for detail prototypes.
    /// </summary>
    public class DetailPrototypesDensity
    {
        #region Fields & Properties

        // Size of tile map
        const int tilemapSize = 128;

        bool waterPlants;
        bool terrainStones;
        bool flowers;

        Density density;

        // Details layers
        int[,] details0, details1, details2, details3, details4;

        public Density Density
        {
            get { return density; }
            set { density = value; }
        }

        // Properties
        public int[,] Empty { get { return EmptyMap(); } }
        public int[,] Grass { get { return details0; } }
        public int[,] WaterPlants { get { return details1; } }
        public int[,] Waterlilies { get { return details2; } }
        public int[,] Stones { get { return details3; } }
        public int[,] Flowers { get { return details4; } }

        #endregion

        #region Public Methods

        public DetailPrototypesDensity(Density density)
        {
            RealGrass realGrass = RealGrass.Instance;
            this.waterPlants = realGrass.WaterPlants;
            this.terrainStones = realGrass.TerrainStones;
            this.flowers = realGrass.Flowers;

            this.density = density;
        }

        public void InitDetailsLayers()
        {
            details0 = EmptyMap();
            details1 = EmptyMap();
            details2 = EmptyMap();
            details3 = EmptyMap();
            details4 = EmptyMap();
        }
                
        /// <summary>
        /// Set density for Summer.
        /// </summary>
        public void SetDensitySummer(Terrain terrain, Color32[] tilemap, ClimateBases currentClimate)
        {
            for (int y = 0; y < tilemapSize; y++)
            {
                for (int x = 0; x < tilemapSize; x++)
                {
                    switch (tilemap[(y * tilemapSize) + x].r)
                    {
                        // Four corner tiles
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                            details0[y * 2, x * 2] = RandomThick();
                            details0[y * 2, (x * 2) + 1] = RandomThick();
                            details0[(y * 2) + 1, x * 2] = RandomThick();
                            details0[(y * 2) + 1, (x * 2) + 1] = RandomThick();
                            if (flowers)
                            {
                                int randomFlowers = RandomFlowers();
                                if (randomFlowers != 0)
                                {
                                    var index = RandomPosition(y, x);
                                    details4[index.First, index.Second] = randomFlowers;
                                }
                            }
                            break;

                        // Upper left corner 
                        case 40:
                        case 224:
                        case 164:
                        case 176:
                        case 181:
                            details0[(y * 2) + 1, x * 2] = RandomThin();
                            break;

                        // Lower left corner 
                        case 41:
                        case 221:
                        case 165:
                        case 177:
                        case 182:
                            details0[y * 2, x * 2] = RandomThin();
                            break;

                        // Lower right corner 
                        case 42:
                        case 222:
                        case 166:
                        case 178:
                        case 183:
                            details0[y * 2, (x * 2) + 1] = RandomThin();
                            break;

                        // Upper right corner 
                        case 43:
                        case 223:
                        case 167:
                        case 179:
                        case 180:
                            details0[(y * 2) + 1, (x * 2) + 1] = RandomThin();
                            break;

                        // Left side
                        case 44:
                        case 66:
                        case 160:
                        case 168:
                            details0[(y * 2) + 1, x * 2] = RandomThin();
                            details0[y * 2, x * 2] = RandomThin();
                            break;

                        // Left side: grass and plants
                        case 84:
                            details0[(y * 2) + 1, x * 2] = RandomThin();
                            details0[y * 2, x * 2] = RandomThin();
                            if (waterPlants)
                            {
                                details1[(y * 2) + 1, x * 2] = RandomWaterPlants();
                                details1[y * 2, x * 2] = RandomWaterPlants();
                            }
                            break;

                        // Lower side
                        case 45:
                        case 67:
                        case 161:
                        case 169:
                            details0[y * 2, (x * 2) + 1] = RandomThin();
                            details0[y * 2, x * 2] = RandomThin();
                            break;

                        // Lower side: grass and plants
                        case 85:
                            details0[y * 2, (x * 2) + 1] = RandomThin();
                            details0[y * 2, x * 2] = RandomThin();
                            if (waterPlants)
                            {
                                details1[y * 2, (x * 2) + 1] = RandomWaterPlants();
                                details1[y * 2, (x * 2)] = RandomWaterPlants();
                            }
                            break;

                        // Right side
                        case 46:
                        case 64:
                        case 162:
                        case 170:
                            details0[(y * 2) + 1, (x * 2) + 1] = RandomThin();
                            details0[y * 2, (x * 2) + 1] = RandomThin();
                            break;

                        // Right side: grass and plants
                        case 86:
                            details0[(y * 2) + 1, (x * 2) + 1] = RandomThin();
                            details0[y * 2, (x * 2) + 1] = RandomThin();
                            if (waterPlants)
                            {
                                details1[(y * 2) + 1, (x * 2) + 1] = RandomWaterPlants();
                                details1[y * 2, (x * 2) + 1] = RandomWaterPlants();
                            }
                            break;

                        // Upper side
                        case 47:
                        case 65:
                        case 163:
                        case 171:
                            details0[(y * 2) + 1, (x * 2) + 1] = RandomThin();
                            details0[(y * 2) + 1, x * 2] = RandomThin();
                            break;

                        // Upper side: grass and plants
                        case 87:
                            details0[(y * 2) + 1, (x * 2) + 1] = RandomThin();
                            details0[(y * 2) + 1, x * 2] = RandomThin();
                            if (waterPlants)
                            {
                                details1[(y * 2) + 1, (x * 2) + 1] = RandomWaterPlants();
                                details1[(y * 2) + 1, x * 2] = RandomWaterPlants();
                            }
                            break;

                        // All expect lower right
                        case 48:
                        case 62:
                        case 156:
                            details0[y * 2, x * 2] = RandomThin();
                            details0[(y * 2) + 1, x * 2] = RandomThin();
                            details0[(y * 2) + 1, (x * 2) + 1] = RandomThin();
                            break;

                        // All expect lower right: grass and plants
                        case 88:
                            details0[y * 2, x * 2] = RandomThin();
                            details0[(y * 2) + 1, x * 2] = RandomThin();
                            details0[(y * 2) + 1, (x * 2) + 1] = RandomThin();
                            if (waterPlants)
                            {
                                details1[y * 2, x * 2] = RandomWaterPlants();
                                details1[(y * 2) + 1, (x * 2) + 1] = RandomWaterPlants();
                            }
                            break;

                        // All expect upper right
                        case 49:
                        case 63:
                        case 157:
                            details0[y * 2, x * 2] = RandomThin();
                            details0[y * 2, (x * 2) + 1] = RandomThin();
                            details0[(y * 2) + 1, x * 2] = RandomThin();
                            break;

                        // All expect upper right: grass and plants
                        case 89:
                            details0[y * 2, x * 2] = RandomThin();
                            details0[y * 2, (x * 2) + 1] = RandomThin();
                            details0[(y * 2) + 1, x * 2] = RandomThin();
                            if (waterPlants)
                            {
                                details1[y * 2, (x * 2) + 1] = RandomWaterPlants();
                                details1[(y * 2) + 1, x * 2] = RandomWaterPlants();
                            }
                            break;


                        // All expect upper left
                        case 50:
                        case 60:
                        case 158:
                            details0[y * 2, x * 2] = RandomThin();
                            details0[y * 2, (x * 2) + 1] = RandomThin();
                            details0[(y * 2) + 1, (x * 2) + 1] = RandomThin();
                            break;

                        // All expect upper left: grass and plants
                        case 90:
                            details0[y * 2, x * 2] = RandomThin();
                            details0[y * 2, (x * 2) + 1] = RandomThin();
                            details0[(y * 2) + 1, (x * 2) + 1] = RandomThin();
                            if (waterPlants)
                            {
                                details1[y * 2, x * 2] = RandomWaterPlants();
                                details1[(y * 2) + 1, (x * 2) + 1] = RandomWaterPlants();
                            }
                            break;

                        // All expect lower left
                        case 51:
                        case 61:
                        case 159:
                            details0[y * 2, (x * 2) + 1] = RandomThin();
                            details0[(y * 2) + 1, x * 2] = RandomThin();
                            details0[(y * 2) + 1, (x * 2) + 1] = RandomThin();
                            break;

                        // All expect lower left: grass and plants
                        case 91:
                            details0[y * 2, (x * 2) + 1] = RandomThin();
                            details0[(y * 2) + 1, x * 2] = RandomThin();
                            details0[(y * 2) + 1, (x * 2) + 1] = RandomThin();
                            if (waterPlants)
                            {
                                details1[y * 2, (x * 2) + 1] = RandomWaterPlants();
                                details1[(y * 2) + 1, x * 2] = RandomWaterPlants();
                            }
                            break;

                        // Left to right
                        case 204:
                        case 206:
                        case 214:
                            details0[y * 2, x * 2] = RandomThin();
                            details0[(y * 2) + 1, (x * 2) + 1] = RandomThin();
                            break;

                        // Right to left
                        case 205:
                        case 207:
                        case 213:
                            details0[(y * 2) + 1, x * 2] = RandomThin();
                            details0[y * 2, (x * 2) + 1] = RandomThin();
                            break;

                        // Swamp upper right corner
                        case 81:
                            if (waterPlants)
                                details1[(y * 2), (x * 2)] = RandomWaterPlants();
                            break;

                        // Swamp lower left corner
                        case 83:
                            if (waterPlants)
                                details1[(y * 2) + 1, (x * 2) + 1] = RandomWaterPlants();
                            break;

                        // In-water grass
                        // case 0 is not enabled because is used for the sea
                        case 1:
                        case 2:
                        case 3:
                            if (waterPlants)
                            {
                                // Mountain: grass
                                if (currentClimate == ClimateBases.Mountain)
                                {
                                    details2[y * 2, x * 2] = Random.Range(1, 2);
                                    details2[(y * 2) + 1, (x * 2) + 1] = Random.Range(1, 2);
                                }
                                // Temperate: waterlilies
                                else if (currentClimate == ClimateBases.Temperate)
                                {
                                    details2[y * 2, x * 2] = 1;
                                    details2[(y * 2) + 1, (x * 2) + 1] = 1;
                                    details2[(y * 2) + 1, x * 2] = 1;
                                    details2[y * 2, (x * 2) + 1] = 1;
                                }
                            }
                            break;

                        // Little stones
                        case 216:
                        case 217:
                        case 218:
                        case 219:
                            if (terrainStones)
                            {
                                details3[y * 2, x * 2] = RandomStones();
                                details3[(y * 2) + 1, (x * 2) + 1] = RandomStones();
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Set density for Winter.
        /// </summary>
        public void SetDensityWinter(Color32[] tilemap)
        {
            for (int y = 0; y < tilemapSize; y++)
            {
                for (int x = 0; x < tilemapSize; x++)
                {
                    switch (tilemap[(y * tilemapSize) + x].r)
                    {
                        // Left side
                        case 84:
                            details1[(y * 2) + 1, x * 2] = RandomWaterPlants();
                            details1[y * 2, x * 2] = RandomWaterPlants();
                            break;
                        // Lower side
                        case 85:
                            details1[y * 2, (x * 2) + 1] = RandomWaterPlants();
                            details1[y * 2, (x * 2)] = RandomWaterPlants();
                            break;
                        // Right side
                        case 86:
                            details1[(y * 2) + 1, (x * 2) + 1] = RandomWaterPlants();
                            details1[y * 2, (x * 2) + 1] = RandomWaterPlants();
                            break;
                        // Upper side
                        case 87:
                            details1[(y * 2) + 1, (x * 2) + 1] = RandomWaterPlants();
                            details1[(y * 2) + 1, x * 2] = RandomWaterPlants();
                            break;
                        // Corners
                        case 88:
                            details1[y * 2, x * 2] = RandomWaterPlants();
                            details1[(y * 2) + 1, (x * 2) + 1] = RandomWaterPlants();
                            break;
                        case 89:
                            details1[y * 2, (x * 2) + 1] = RandomWaterPlants();
                            details1[(y * 2) + 1, x * 2] = RandomWaterPlants();
                            break;
                        case 90:
                            details1[y * 2, x * 2] = RandomWaterPlants();
                            details1[(y * 2) + 1, (x * 2) + 1] = RandomWaterPlants();
                            break;
                        case 91:
                            details1[y * 2, (x * 2) + 1] = RandomWaterPlants();
                            details1[(y * 2) + 1, x * 2] = RandomWaterPlants();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Set density for Desert locations.
        /// Desert has grass around water but not on mainland, also desert regions don't have winter season.
        /// </summary>
        public void SetDensityDesert(Color32[] tilemap)
        {
            for (int y = 0; y < tilemapSize; y++)
            {
                for (int x = 0; x < tilemapSize; x++)
                {
                    switch (tilemap[(y * tilemapSize) + x].r)
                    {
                        // Left side
                        case 84:
                            details1[(y * 2) + 1, x * 2] = RandomDesert();
                            details1[y * 2, x * 2] = RandomDesert();
                            break;
                        // Lower side
                        case 85:
                            details1[y * 2, (x * 2) + 1] = RandomDesert();
                            details1[y * 2, (x * 2)] = RandomDesert();
                            break;
                        // Right side
                        case 86:
                            details1[(y * 2) + 1, (x * 2) + 1] = RandomDesert();
                            details1[y * 2, (x * 2) + 1] = RandomDesert();
                            break;
                        // Upper side
                        case 87:
                            details1[(y * 2) + 1, (x * 2) + 1] = RandomDesert();
                            details1[(y * 2) + 1, x * 2] = RandomDesert();
                            break;
                        // Corners
                        case 88:
                            details1[y * 2, x * 2] = RandomDesert();
                            details1[(y * 2) + 1, (x * 2) + 1] = RandomDesert();
                            break;
                        case 89:
                            details1[y * 2, (x * 2) + 1] = RandomDesert();
                            details1[(y * 2) + 1, x * 2] = RandomDesert();
                            break;
                        case 90:
                            details1[y * 2, x * 2] = RandomDesert();
                            details1[(y * 2) + 1, (x * 2) + 1] = RandomDesert();
                            break;
                        case 91:
                            details1[y * 2, (x * 2) + 1] = RandomDesert();
                            details1[(y * 2) + 1, x * 2] = RandomDesert();
                            break;
                    }
                }
            }
        }

        public void CleanDetailMap(DaggerfallLocation dfLocation)
        {
            var terrainObject = GameManager.Instance.StreamingWorld.GetTerrainFromPixel(dfLocation.Summary.MapPixelX, dfLocation.Summary.MapPixelY);
            Terrain terrain = terrainObject.GetComponent<Terrain>();
            TerrainData terrainData = terrain.terrainData;
            TerrainCollider terrainCollider = terrain.GetComponent<TerrainCollider>();

            var map = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, 0);

            Vector3 origin = terrain.GetPosition();

            for (int y = 0; y < tilemapSize; y++)
            {
                for (int x = 0; x < tilemapSize; x++)
                {
                    float terrainSize = (MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale);
                    float tileSize = terrainSize / tilemapSize;

                    Vector3 pos = origin;
                    pos.x += terrain.terrainData.size.x / tilemapSize * x;
                    pos.z += terrain.terrainData.size.z / tilemapSize * y;
                    pos.y = terrain.terrainData.GetHeight(x, y) + 0.55f;

                    var colliders = Physics.OverlapBox(pos, new Vector3(tileSize / 2, 0.5f, tileSize / 2));
                    if (colliders.Length > 0 && !(colliders.Length == 1 && colliders[0] == terrainCollider))
                    {
                        if (Physics.CheckBox(pos, new Vector3(tileSize / 8, 0.5f, tileSize / 8)))
                        {
                            map[y * 2, x * 2] = 0;
                            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            cube.transform.position = pos;
                            cube.transform.localScale = new Vector3(tileSize / 4, 1, tileSize / 4);
                        }

                        pos.x += tileSize / 2;
                        if (Physics.CheckBox(pos, new Vector3(tileSize / 8, 0.5f, tileSize / 8)))
                        {
                            map[y * 2, (x * 2) + 1] = 0;
                            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            cube.transform.position = pos;
                            cube.transform.localScale = new Vector3(tileSize / 4, 1, tileSize / 4);
                        }

                        pos.x -= tileSize / 2;
                        pos.z += tileSize / 2;
                        if (Physics.CheckBox(pos, new Vector3(tileSize / 8, 0.5f, tileSize / 8)))
                        {
                            map[(y * 2) + 1, x * 2] = 0;
                            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            cube.transform.position = pos;
                            cube.transform.localScale = new Vector3(tileSize / 4, 1, tileSize / 4);
                        }

                        pos.x += tileSize / 2;
                        if (Physics.CheckBox(pos, new Vector3(tileSize / 8, 0.5f, tileSize / 8)))
                        {
                            map[(y * 2) + 1, (x * 2) + 1] = 0;
                            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            cube.transform.position = pos;
                            cube.transform.localScale = new Vector3(tileSize / 4, 1, tileSize / 4);
                        }

                        //var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //cube.transform.position = pos;
                        //cube.transform.localScale = new Vector3(tileSize - 0.2f, 1, tileSize - 0.2f);
                    }

                    //if (Physics.CheckBox(pos, new Vector3(tileSize / 2, 0.5f, tileSize / 2)))
                    //{
                    //    map[y * 2, x * 2] = 0;
                    //    map[y * 2, (x * 2) + 1] = 0;
                    //    map[(y * 2) + 1, x * 2] = 0;
                    //    map[(y * 2) + 1, (x * 2) + 1] = 0;

                    //    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //    cube.transform.position = pos;
                    //    cube.transform.localScale = new Vector3(tileSize, 1, tileSize);
                    //}
                }
            }

            terrainData.SetDetailLayer(0, 0, 0, map);
        }
        
        #endregion

        #region Random Generators

        /// <summary>
        /// Generate random values for the placement of thin grass. 
        /// </summary>
        private int RandomThin()
        {
            return density.grassThin.Random();
        }

        /// <summary>
        /// Generate random values for the placement of thick grass.
        /// </summary>
        private int RandomThick()
        {
            return density.grassThick.Random();
        }

        /// <summary>
        /// Generate random values for the placement of water plants.
        /// </summary>
        private int RandomWaterPlants()
        {
            return density.waterPlants.Random();
        }

        /// <summary>
        /// Generate random values for the placement of desert grass.
        /// </summary>
        private int RandomDesert()
        {
            return density.desertPlants.Random();
        }

        /// <summary>
        /// Generate random values for the placement of stones. 
        /// </summary>
        private int RandomStones()
        {
            return density.stones.Random();
        }

        /// <summary>
        /// Generate random values for the placement of flowers. 
        /// </summary>
        private int RandomFlowers()
        {
            if (Random.Range(0, 100) < density.flowers)
                return density.flowersBush.Random();

            return 0;
        }

        /// <summary>
        /// Get a random position on tile.
        /// </summary>
        /// <param name="y">First index.</param>
        /// <param name="x">Second index.</param>
        /// <returns>One of the four possible position on tile.</returns>
        private static Tuple<int, int> RandomPosition(int y, int x)
        {
            switch (Random.Range(0, 4))
            {
                case 0:
                    return new Tuple<int, int>(y * 2, x * 2);
                case 1:
                    return new Tuple<int, int>(y * 2, (x * 2) + 1);
                case 2:
                    return new Tuple<int, int>((y * 2) + 1, x * 2);
                case 3:
                default:
                    return new Tuple<int, int>((y * 2) + 1, (x * 2) + 1);
            }
        }

        #endregion

        #region Private Methods

        private static int[,] EmptyMap()
        {
            const int size = 256;
            return new int[size, size];
        }

        #endregion
    }
}
