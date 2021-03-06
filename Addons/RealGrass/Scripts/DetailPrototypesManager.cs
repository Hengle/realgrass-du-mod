// Project:         Real Grass for Daggerfall Unity
// Web Site:        http://forums.dfworkshop.net/viewtopic.php?f=14&t=17
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/TheLacus/realgrass-du-mod
// Original Author: TheLacus
// Contributors:    

using System.IO;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace RealGrass
{
    #region Structs

    public struct GrassColors
    {
        public Color
            SpringHealthy,
            SpringDry,
            SummerHealty,
            SummerDry,
            FallHealty,
            FallDry;
    }

    public struct PrototypesProperties
    {
        public Range<float> GrassHeight;
        public Range<float> GrassWidth;
        public float NoiseSpread;
        public GrassColors GrassColors;
        public bool UseGrassShader;
        public float NoiseSpreadPlants;
        public bool TextureOverride;
    }

    #endregion

    /// <summary>
    /// Manages terrain detail prototypes.
    /// </summary>
    public class DetailPrototypesManager
    {
        #region Constants

        // Textures/Prefabs for grass billboards
        const string brownGrass = "BrownGrass";
        const string brownGrassRealistic = "Grass_01";
        const string greenGrass = "GreenGrass";
        const string greenGrassRealistic = "Grass_02";

        static readonly string[] grassDetailsTextures = {
            "GrassDetails_01", "GrassDetails_02", "GrassDetails_03", "GrassDetails_04", "GrassDetails_05"
        };

        // Models for water plants
        const string plantsTemperate = "PlantsTemperate"; 
        const string plantsTemperateAlt = "PlantsTemperateAlt";
        const string plantsSwamp = "PlantsSwamp";
        const string plantsSwampAlt = "PlantsSwampAlt";
        const string plantsMountain = "PlantsMountain"; 
        const string plantsMountainAlt = "PlantsMountainAlt";
        const string plantsDesert = "PlantsDesert"; 
        const string plantsDesertAlt = "PlantsDesertAlt";

        // Winter models for water plants
        const string plantsTemperateWinter = "PlantsTemperateWinter";
        const string plantsSwampWinter = "PlantsSwampWinter";
        const string plantsMountainWinter = "PlantsMountainWinter";

        const string bushSwamp = "BushSwamp";
        const string bushTemperate = "BushTemperate";
        const string bushMountain = "BushMountain";
        const string bushDesert = "BushDesert";

        // Stones
        const string stone = "Stone";
        const string rockLight = "RockLight";
        const string rockDark = "RockDark";

        // Flowers
        static readonly string[] flowers = {
            "FlowersRed", "FlowersPink" , "FlowersBlue" , "FlowersYellow" };

        struct UpdateType { public const short Summer = 0, Winter = 1, Desert = 2; }

        #endregion

        #region Fields

        static GameObject grassDetailPrefab;
        static GameObject grassAccentPrefab;

        readonly DetailPrototype[] detailPrototypes;

        readonly bool useGrassShader;
        readonly GrassColors grassColors;
        readonly Range<float> grassHeight;

        readonly bool textureOverride;

        int currentkey = -1;

        #endregion

        #region Properties

        /// <summary>
        /// Detail prototypes used by the terrain.
        /// </summary>
        public DetailPrototype[] DetailPrototypes
        {
            get { return detailPrototypes; }
        }

        // Layers index
        public int Grass { get; private set; }
        public int GrassDetails { get; private set; }
        public int GrassAccents { get; private set; }
        public int WaterPlants { get; private set; }
        public int WaterPlantsAlt { get; private set; }
        public int Stones { get; private set; }
        public int Rocks { get; private set; }
        public int Flowers { get; private set; }
        public int Bushes { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize detail protoypes.
        /// </summary>
        public DetailPrototypesManager(PrototypesProperties properties)
        {
            Color healthyColor = new Color(0.70f, 0.70f, 0.70f);
            Color dryColor = new Color(0.40f, 0.40f, 0.40f);

            this.textureOverride = properties.TextureOverride;

            // Create a holder for our grass and plants
            List<DetailPrototype> detailPrototypes = new List<DetailPrototype>();
            int index = 0;

            // Grass settings
            grassHeight = properties.GrassHeight;
            grassColors = properties.GrassColors;
            useGrassShader = properties.UseGrassShader;

            // We use GrassBillboard or Grass rendermode
            var grassPrototypes = new DetailPrototype()
            {
                minWidth = properties.GrassWidth.Min,
                maxWidth = properties.GrassWidth.Max,
                noiseSpread = properties.NoiseSpread,
                renderMode = useGrassShader ? DetailRenderMode.Grass : DetailRenderMode.GrassBillboard,
                usePrototypeMesh = useGrassShader
            };
            detailPrototypes.Add(grassPrototypes);
            Grass = index;

            if (RealGrass.Instance.RealisticGrass)
            {
                detailPrototypes.Add(new DetailPrototype()
                {
                    minWidth = properties.GrassWidth.Min,
                    maxWidth = properties.GrassWidth.Max,
                    noiseSpread = properties.NoiseSpread,
                    healthyColor = healthyColor,
                    dryColor = dryColor,
                    renderMode = useGrassShader ? DetailRenderMode.Grass : DetailRenderMode.GrassBillboard,
                    usePrototypeMesh = useGrassShader
                });
                GrassDetails = ++index;

                detailPrototypes.Add(new DetailPrototype()
                {
                    minWidth = properties.GrassWidth.Min,
                    maxWidth = properties.GrassWidth.Max,
                    noiseSpread = properties.NoiseSpread,
                    healthyColor = healthyColor,
                    dryColor = dryColor,
                    renderMode = useGrassShader ? DetailRenderMode.Grass : DetailRenderMode.GrassBillboard,
                    usePrototypeMesh = useGrassShader
                });
                GrassAccents = ++index;
            }

            if (RealGrass.Instance.WaterPlants)
            {
                // Near-water plants settings
                // Here we use the Grass shader which support meshes, and textures with transparency.
                // This allow us to have more realistic plants which still bend in the wind.
                var waterPlantsNear = new DetailPrototype()
                {
                    usePrototypeMesh = true,
                    noiseSpread = properties.NoiseSpreadPlants,
                    healthyColor = healthyColor,
                    dryColor = dryColor,
                    renderMode = DetailRenderMode.Grass
                };
                detailPrototypes.Add(waterPlantsNear);
                WaterPlants = ++index;

                // In-water plants settings
                // We use Grass as above
                var waterPlantsInside = new DetailPrototype()
                {
                    usePrototypeMesh = true,
                    noiseSpread = properties.NoiseSpreadPlants,
                    healthyColor = healthyColor,
                    dryColor = dryColor,
                    renderMode = DetailRenderMode.Grass
                };
                detailPrototypes.Add(waterPlantsInside);
                WaterPlantsAlt = ++index;
            }

            if (RealGrass.Instance.TerrainStones)
            {
                // Little stones
                // For stones we use VertexLit as we are placing 3d static models.
                var stonesPrototypes = new DetailPrototype()
                {
                    usePrototypeMesh = true,
                    noiseSpread = 0.4f,
                    healthyColor = healthyColor,
                    dryColor = dryColor,
                    renderMode = DetailRenderMode.VertexLit,
                    prototype = LoadGameObject(stone)
                };
                detailPrototypes.Add(stonesPrototypes);
                Stones = ++index;

                var rocksPrototypes = new DetailPrototype()
                {
                    usePrototypeMesh = true,
                    noiseSpread = 0.4f,
                    healthyColor = healthyColor,
                    dryColor = dryColor,
                    renderMode = DetailRenderMode.VertexLit,
                };
                detailPrototypes.Add(rocksPrototypes);
                Rocks = ++index;
            }

            if (RealGrass.Instance.Flowers)
            {
                // Medium-sized vegetation
                var bushesPrototypes = new DetailPrototype()
                {
                    usePrototypeMesh = true,
                    noiseSpread = 0.4f,
                    healthyColor = healthyColor,
                    dryColor = dryColor,
                    renderMode = DetailRenderMode.Grass,
                };
                detailPrototypes.Add(bushesPrototypes);
                Bushes = ++index;

                // Little flowers
                var flowerPrototypes = new DetailPrototype()
                {
                    usePrototypeMesh = true,
                    noiseSpread = 0.4f,
                    healthyColor = healthyColor,
                    dryColor = healthyColor,
                    renderMode = DetailRenderMode.Grass,
                };
                detailPrototypes.Add(flowerPrototypes);
                Flowers = ++index;
            }

            this.detailPrototypes = detailPrototypes.ToArray();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load assets for Summer.
        /// </summary>
        public void UpdateClimateSummer(ClimateBases currentClimate)
        {
            SetGrassColor();
            SetGrassSize();

            if (RealGrass.Instance.RealisticGrass)
            {
                SetGrassDetail(GrassDetails, ref grassDetailPrefab);
                SetGrassDetail(GrassAccents, ref grassAccentPrefab);
            }

            if (RealGrass.Instance.Flowers)
                detailPrototypes[Flowers].prototype = LoadGameObject(GetRandomFlowers());

            if (!NeedsUpdate(UpdateType.Summer, currentClimate))
                return;

            switch (currentClimate)
            {
                case ClimateBases.Mountain:

                    // Mountain
                    SetGrass(brownGrass, brownGrassRealistic);

                    if (RealGrass.Instance.WaterPlants)
                    {
                        detailPrototypes[WaterPlants].prototype = LoadGameObject(plantsMountain);
                        detailPrototypes[WaterPlantsAlt].prototype = LoadGameObject(plantsMountainAlt);
                    }

                    if (RealGrass.Instance.Flowers)
                        detailPrototypes[Bushes].prototype = LoadGameObject(bushMountain);

                    if (RealGrass.Instance.TerrainStones)
                        detailPrototypes[Rocks].prototype = LoadGameObject(rockLight);
                    break;

                case ClimateBases.Swamp:

                    // Swamp
                    SetGrass(brownGrass, brownGrassRealistic);

                    if (RealGrass.Instance.WaterPlants)
                    {
                        detailPrototypes[WaterPlants].prototype = LoadGameObject(plantsSwamp);
                        detailPrototypes[WaterPlantsAlt].prototype = LoadGameObject(plantsSwampAlt);
                    }

                    if (RealGrass.Instance.Flowers)
                        detailPrototypes[Bushes].prototype = LoadGameObject(bushSwamp);

                    if (RealGrass.Instance.TerrainStones)
                        detailPrototypes[Rocks].prototype = LoadGameObject(rockDark);
                    break;

                case ClimateBases.Temperate:

                    // Temperate
                    SetGrass(greenGrass, greenGrassRealistic);

                    if (RealGrass.Instance.WaterPlants)
                    {
                        detailPrototypes[WaterPlants].prototype = LoadGameObject(plantsTemperate);
                        detailPrototypes[WaterPlantsAlt].prototype = LoadGameObject(plantsTemperateAlt);
                    }

                    if (RealGrass.Instance.Flowers)
                        detailPrototypes[Bushes].prototype = LoadGameObject(bushTemperate);

                    if (RealGrass.Instance.TerrainStones)
                        detailPrototypes[Rocks].prototype = LoadGameObject(rockLight);
                    break;

                default:
                    Debug.LogError(string.Format("RealGrass: {0} is not a valid climate (Summer)", currentClimate));
                    RealGrass.Instance.ToggleMod(false);
                    break;
            }
        }

        /// <summary>
        /// Load assets for Winter.
        /// </summary>
        public void UpdateClimateWinter(ClimateBases currentClimate)
        {
            bool drawGrass = IsGrassTransitioning();
            if (drawGrass)
            {
                SetGrassColor();
                SetGrassSize();
            }

            if (!NeedsUpdate(UpdateType.Winter, currentClimate))
                return;

            switch (currentClimate)
            {
                case ClimateBases.Mountain:
                    if (drawGrass)
                        SetGrass(brownGrass, brownGrassRealistic);

                    if (RealGrass.Instance.WaterPlants)
                        detailPrototypes[WaterPlants].prototype = LoadGameObject(plantsMountainWinter);
                    break;

                case ClimateBases.Swamp:
                    if (drawGrass)
                        SetGrass(brownGrass, brownGrassRealistic);

                    if (RealGrass.Instance.WaterPlants)
                        detailPrototypes[WaterPlants].prototype = LoadGameObject(plantsSwampWinter);
                    break;

                case ClimateBases.Temperate:
                    if (drawGrass)
                        SetGrass(greenGrass, greenGrassRealistic);

                    if (RealGrass.Instance.WaterPlants)
                        detailPrototypes[WaterPlants].prototype = LoadGameObject(plantsTemperateWinter);
                    break;

                default:
                    Debug.LogError(string.Format("RealGrass: {0} is not a valid climate (Winter)", currentClimate));
                    RealGrass.Instance.ToggleMod(false);
                    break;
            }
        }

        /// <summary>
        /// Load assets for Desert, which doesn't support seasons.
        /// </summary>
        public void UpdateClimateDesert()
        {
            if (!NeedsUpdate(UpdateType.Desert, ClimateBases.Desert))
                return;

            if (RealGrass.Instance.WaterPlants)
            {
                detailPrototypes[WaterPlants].prototype = LoadGameObject(plantsDesert);
                detailPrototypes[WaterPlantsAlt].prototype = LoadGameObject(plantsDesertAlt);
            }

            if (RealGrass.Instance.Flowers)
                detailPrototypes[Bushes].prototype = LoadGameObject(bushDesert);

            if (RealGrass.Instance.TerrainStones)
                detailPrototypes[Rocks].prototype = LoadGameObject(rockDark);
        }

        #endregion

        #region Private Methods

        private void SetGrass(string classic, string realistic)
        {
            string assetName = RealGrass.Instance.RealisticGrass ? realistic : classic;

            if (!useGrassShader)
                detailPrototypes[Grass].prototypeTexture = LoadTexture(assetName);
            else
                detailPrototypes[Grass].prototype = LoadGameObject(assetName);
        }

        private void SetGrassDetail(int layer, ref GameObject prefab)
        {
            Texture2D tex = LoadTexture(grassDetailsTextures[Random.Range(0, grassDetailsTextures.Length)]);

            if (!useGrassShader)
            { 
                detailPrototypes[layer].prototypeTexture = tex;
            }
            else
            {
                GameObject go = prefab ?? (prefab = GameObject.Instantiate(LoadGameObject(greenGrass)));
                go.GetComponent<Renderer>().material.mainTexture = tex;
                detailPrototypes[layer].prototype = go;
            }
        }

        /// <summary>
        /// Import texture from loose files or from mod.
        /// </summary>
        /// <param name="name">Name of texture.</param>
        private Texture2D LoadTexture(string name)
        {
            Texture2D tex;
            if (TryImportTextureFromLooseFiles(name, out tex))
                return tex;

            return RealGrass.Mod.GetAsset<Texture2D>(name);
        }

        /// <summary>
        /// Import gameobject from mod and override material with texture from loose files.
        /// </summary>
        /// <param name="name">Name of gameobject.</param>
        private GameObject LoadGameObject(string name)
        {
            GameObject go = RealGrass.Mod.GetAsset<GameObject>(name);

            Texture2D tex;
            if (TryImportTextureFromLooseFiles(name, out tex))
                go.GetComponent<MeshRenderer>().material.mainTexture = tex;

            return go;
        }

        private bool TryImportTextureFromLooseFiles(string name, out Texture2D tex)
        {
            if (textureOverride)
                return TextureReplacement.TryImportTextureFromLooseFiles(Path.Combine(RealGrass.TexturesFolder, name), true, false, out tex);

            tex = null;
            return false;
        }

        /// <summary>
        /// True if season or climate changed and detail prototypes should be updated.
        /// </summary>
        private bool NeedsUpdate(short updateType, ClimateBases climate)
        {
            int key = (updateType << 16) + (short)climate;

            if (key == currentkey)
                return false;

            currentkey = key;
            return true;
        }

        /// <summary>
        /// Set grass color according to day of year.
        /// </summary>
        private void SetGrassColor()
        {
            int day = DaggerfallUnity.Instance.WorldTime.Now.DayOfYear;
            if (day < DaysOfYear.Spring)
            {
                float t = Mathf.InverseLerp(DaysOfYear.GrowDay, DaysOfYear.Spring, day);
                detailPrototypes[Grass].healthyColor = Color.Lerp(Color.white, grassColors.SpringHealthy, t);
                detailPrototypes[Grass].dryColor = Color.Lerp(Color.white, grassColors.SpringDry, t);
            }
            else if (day <= DaysOfYear.MidYear)
            {
                float t = Mathf.InverseLerp(DaysOfYear.Spring, DaysOfYear.MidYear, day);
                detailPrototypes[Grass].healthyColor = Color.Lerp(grassColors.SpringHealthy, grassColors.SummerHealty, t);
                detailPrototypes[Grass].dryColor = Color.Lerp(grassColors.SpringDry, grassColors.SummerDry, t);
            }
            else if (day < DaysOfYear.Winter)
            {
                float t = Mathf.InverseLerp(DaysOfYear.MidYear, DaysOfYear.Winter, day);
                detailPrototypes[Grass].healthyColor = Color.Lerp(grassColors.SummerHealty, grassColors.FallHealty, t);
                detailPrototypes[Grass].dryColor = Color.Lerp(grassColors.SummerDry, grassColors.FallDry, t);
            }
            else
            {
                float t = Mathf.InverseLerp(DaysOfYear.Winter, DaysOfYear.DieDay, day);
                detailPrototypes[Grass].healthyColor = Color.Lerp(grassColors.FallHealty, Color.white, t);
                detailPrototypes[Grass].dryColor = Color.Lerp(grassColors.FallDry, Color.white, t);
            }
        }

        /// <summary>
        /// Set grass size according to day of year.
        /// </summary>
        private void SetGrassSize()
        {
            // Settings size is size on summer (max size).
            // Height increase on spring and decrease on fall up to this amount(%).
            const int seasonalModifier = 65;
            const float minScale = 1 - (float)seasonalModifier / 100;

            int day = DaggerfallUnity.Instance.WorldTime.Now.DayOfYear;

            if (day < DaysOfYear.Spring)
            {
                detailPrototypes[Grass].minHeight = grassHeight.Min * minScale;
                detailPrototypes[Grass].maxHeight = grassHeight.Max * minScale;
            }
            else if (day < DaysOfYear.Summer)
            {
                float t = Mathf.InverseLerp(DaysOfYear.Spring, DaysOfYear.Summer, day);
                float scale = Mathf.SmoothStep(minScale, 1, t);

                detailPrototypes[Grass].minHeight = grassHeight.Min * scale;
                detailPrototypes[Grass].maxHeight = grassHeight.Max * scale;
            }
            else if (day < DaysOfYear.Fall)
            {
                detailPrototypes[Grass].minHeight = grassHeight.Min;
                detailPrototypes[Grass].maxHeight = grassHeight.Max;
            }
            else if (day < DaysOfYear.Winter)
            {
                float t = Mathf.InverseLerp(DaysOfYear.Fall, DaysOfYear.Winter, day);
                float scale = Mathf.SmoothStep(minScale, 1, 1 - t);

                detailPrototypes[Grass].minHeight = grassHeight.Min * scale;
                detailPrototypes[Grass].maxHeight = grassHeight.Max * scale;
            }
            else
            {
                detailPrototypes[Grass].minHeight = grassHeight.Min * minScale;
                detailPrototypes[Grass].maxHeight = grassHeight.Max * minScale;
            }

            if (RealGrass.Instance.RealisticGrass)
            {
                // Secondary grass layers are slightly bigger than the base one
                detailPrototypes[GrassDetails].minHeight = detailPrototypes[GrassAccents].minHeight = detailPrototypes[Grass].minHeight * 1.35f;
                detailPrototypes[GrassDetails].maxHeight = detailPrototypes[GrassAccents].maxHeight = detailPrototypes[Grass].maxHeight * 1.35f;
            }
        }

        private bool IsGrassTransitioning()
        {
            int day = DaggerfallUnity.Instance.WorldTime.Now.DayOfYear;
            return day > DaysOfYear.GrowDay || day < DaysOfYear.DieDay;
        }

        private static string GetRandomFlowers()
        {
            return flowers[Random.Range(0, flowers.Length)];
        }

        #endregion
    }
}
