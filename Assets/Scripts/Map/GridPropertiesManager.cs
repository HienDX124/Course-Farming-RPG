﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    private Transform cropParentTransform;

    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;

    private Grid grid;
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;
    [SerializeField] private SO_CropDetailsList sO_CropDetailsList = null;
    [SerializeField] private SO_GridProperties[] sO_GridPropertiesArray = null;
    [SerializeField] private Tile[] dugGround = null;
    [SerializeField] private Tile[] wateredGround = null;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISaveableRegister();

        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent += AdvanceDay;
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent -= AdvanceDay;
    }

    private void Start()
    {
        InitialiseGridProperties();
    }

    private void ClearDisplayGroundDecorations()
    {
        // Remove ground decorations
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayAllPlantedCrops()
    {
        // Destroy all crops in scene
        Crop[] cropArray;
        cropArray = FindObjectsOfType<Crop>();

        foreach (Crop crop in cropArray)
        {
            Destroy(crop.gameObject);
        }
    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecorations();

        ClearDisplayAllPlantedCrops();
    }

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        // Dug
        if (gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }
    private void DisplayGridPropertyDetails()
    {
        // Loop through all grid items
        foreach (KeyValuePair<string, GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;

            DisplayDugGround(gridPropertyDetails);
            DisplayWateredGround(gridPropertyDetails);
            DisplayPlantedCrops(gridPropertyDetails);
        }
    }

    public void DisplayPlantedCrops(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.seedItemCode > -1)
        {
            // Get crop details
            CropDetails cropDetails = sO_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

            // Prefab to use
            GameObject cropPrefab;

            // Instantiate crop prefab at grid location
            int growthStages = cropDetails.growthDays.Length;
            int currentGrowthStage = 0;
            int daysCounter = cropDetails.totalGrowthDays;
            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (gridPropertyDetails.growthDays >= daysCounter)
                {
                    currentGrowthStage = i;
                    break;
                }
                daysCounter = daysCounter - cropDetails.growthDays[i];
            }
            cropPrefab = cropDetails.growthPrefab[currentGrowthStage];

            Sprite growthSprite = cropDetails.growthSprite[currentGrowthStage];

            Vector3 worldPosition = groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));

            worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2, worldPosition.y, worldPosition.z);

            GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);

            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;
            cropInstance.transform.SetParent(cropParentTransform);
            cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        }
    }

    public void DisplayWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        // Watered
        if (gridPropertyDetails.daysSinceWatered > -1)
        {
            ConnectWateredGround(gridPropertyDetails);
        }
    }

    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        // Select the tile based on surrounding dug tiles
        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);

        // Set 4 tiles if dug surrounding current tile - up, down, left, right now that is central tile has been dug
        GridPropertyDetails adjacentGridPropertyDetails;

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), dugTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile2 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), dugTile2);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile3 = SetDugTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), dugTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile4 = SetDugTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), dugTile4);
        }
    }

    public void ConnectWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        // Select tile based on surrounding watered tiles
        Tile wateredTile0 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), wateredTile0);

        // Set 4 tiles if watered surrounding current tile - up, down, left, right now that this central tile has been watered
        GridPropertyDetails adjacentGridPropertyDetails;

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTiled1 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), wateredTiled1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTiled2 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), wateredTiled2);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTiled3 = SetWateredTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), wateredTiled3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTiled4 = SetWateredTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), wateredTiled4);
        }


    }

    private Tile SetDugTile(int xGrid, int yGrid)
    {
        // Get whether surrounding tiles (up, down, left, right) are dug or not
        bool upDug = IsGridSquareDug(xGrid, yGrid + 1);
        bool downDug = IsGridSquareDug(xGrid, yGrid - 1);
        bool leftDug = IsGridSquareDug(xGrid - 1, yGrid);
        bool rightDug = IsGridSquareDug(xGrid + 1, yGrid);

        #region Set appropriate tile based on whether surrounding tiles are dug or not
        if (!upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[0];
        }
        else if (!upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[1];
        }
        else if (!upDug && downDug && rightDug && leftDug)
        {
            return dugGround[2];
        }
        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[3];
        }
        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[4];
        }
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[5];
        }
        else if (upDug && downDug && rightDug && leftDug)
        {
            return dugGround[6];
        }
        else if (upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[7];
        }
        else if (upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[8];
        }
        else if (upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[9];
        }
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[10];
        }
        else if (upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[11];
        }
        else if (upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[12];
        }
        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[13];
        }
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[14];
        }
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[15];
        }
        return null;

        #endregion Set appropriate tile based on whethwe surrounding tiles are dug or not
    }

    private bool IsGridSquareDug(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);
        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daysSinceDug > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsGridSquareWatered(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daysSinceWatered > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    private Tile SetWateredTile(int xGrid, int yGrid)
    {
        // Get whether surrounding tiles (up, down, left, right) are dug or not
        bool upWatered = IsGridSquareWatered(xGrid, yGrid + 1);
        bool downWatered = IsGridSquareWatered(xGrid, yGrid - 1);
        bool leftWatered = IsGridSquareWatered(xGrid - 1, yGrid);
        bool rightWatered = IsGridSquareWatered(xGrid + 1, yGrid);

        #region Set appropriate tile based on whether surrounding tiles are watered or not
        if (!upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[0];
        }
        else if (!upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[1];
        }
        else if (!upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGround[2];
        }
        else if (!upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[3];
        }
        else if (!upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[4];
        }
        else if (upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[5];
        }
        else if (upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGround[6];
        }
        else if (upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[7];
        }
        else if (upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[8];
        }
        else if (upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[9];
        }
        else if (upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGround[10];
        }
        else if (upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[11];
        }
        else if (upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[12];
        }
        else if (!upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[13];
        }
        else if (!upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGround[14];
        }
        else if (!upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[15];
        }
        return null;

        #endregion Set appropriate tile based on whethwe surrounding tiles are watered or not

    }

    /// <sumary>
    /// This initialises the grid property dictionary with the values from the SO_GridProperties assets and stores the value for each scene in GameObjectSave sceneData
    /// </sumary>
    private void InitialiseGridProperties()
    {
        // Loop through all girdproperties in the array
        foreach (SO_GridProperties sO_GridProperties in sO_GridPropertiesArray)
        {
            // Creat dictionary of grid property details
            Dictionary<string, GridPropertyDetails> gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();

            // Populate grid property dictionary - Iterate through all the grid properties in the so gridproperties list
            foreach (GridProperty gridProperty in sO_GridProperties.gridPropertyList)
            {
                GridPropertyDetails gridPropertyDetails;
                gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDictionary);

                if (gridPropertyDetails == null)
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }
                switch (gridProperty.gridBoolProperty)
                {
                    case GridBoolProperty.diggable:
                        gridPropertyDetails.isDiggable = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.canDropItem:
                        gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.canPlaceFurniture:
                        gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.isPath:
                        gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.isNPCObstacle:
                        gridPropertyDetails.isNPCObstacle = gridProperty.gridBoolValue;
                        break;
                    default:
                        break;
                }
                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, gridPropertyDictionary);
            }

            // Create scene save for this gameobject
            SceneSave sceneSave = new SceneSave();

            // Add grid property dictionary to scene save data
            sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

            // If starting scene set the gridPropertyDictionary member variable to the current interation
            if (sO_GridProperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this.gridPropertyDictionary = gridPropertyDictionary;
            }

            // Add scene save to gameobject scene data
            GameObjectSave.sceneData.Add(sO_GridProperties.sceneName.ToString(), sceneSave);
        }
    }

    private void AfterSceneLoaded()
    {
        if (GameObject.FindGameObjectWithTag(Tags.CropsParentTransform) != null)
        {
            cropParentTransform = GameObject.FindGameObjectWithTag(Tags.CropsParentTransform).transform;
        }
        else
        {
            cropParentTransform = null;
        }


        // Get grid
        grid = GameObject.FindObjectOfType<Grid>();

        // Get tilemaps
        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
    }

    /// <sumary>
    /// Returns the gridPropertyDetails at the gridlocation for the supplied dictionary, or null if no properties exist at that loacation.
    /// </sumary>
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        // Construct key from coordinate
        string key = "x" + gridX + "y" + gridY;

        GridPropertyDetails gridPropertyDetails;

        // Check if the grid property details exist for coordinate and retrieve
        if (!gridPropertyDictionary.TryGetValue(key, out gridPropertyDetails))
        {
            // if not found
            return null;
        }
        else
        {
            return gridPropertyDetails;
        }
    }

    /// <sumary>
    /// Get the grid property details for the tile at (gridX, girdY). If no grid property details exist null is returned and can assume that all grid property details values are null or false
    /// </sumary>
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, gridPropertyDictionary);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // Get sceneSave for scene - it exists since we created it in initialise
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            // Get grid property details dictionary - it exists since we created it in initialise
            if (sceneSave.gridPropertyDetailsDictionary != null)
            {
                gridPropertyDictionary = sceneSave.gridPropertyDetailsDictionary;
            }

            if (gridPropertyDictionary.Count > 0)
            {
                // grid property details found for the current scene destroy existing ground decoration
                ClearDisplayGridPropertyDetails();

                // Instantiate grid property details for current scene
                DisplayGridPropertyDetails();
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        // Remove sceneSave for scene
        GameObjectSave.sceneData.Remove(sceneName);

        // Create sceneSave for scene
        SceneSave sceneSave = new SceneSave();

        // Create and add dict grid property details dictionary
        sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

        // Add scene save to gameobject scene data
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }

    /// <sumary>
    /// Set the grid property details to gridPropertyDetails for the tile at (gridX, girdY) for current scene
    /// </sumary>
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridPropertyDictionary);
    }

    /// <sumary>
    /// Set the grid property details to gridPropertyDetails for the tile at (gridX, gridY) for the gridPropertyDictionary
    /// </sumary>
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        // Construct key from coordinate
        string key = "x" + gridX + "y" + gridY;

        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        // Set value
        gridPropertyDictionary[key] = gridPropertyDetails;
    }

    private void AdvanceDay(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        // Clear display all grid property details
        ClearDisplayGridPropertyDetails();

        // Loop through all scens - by looping through all gridproperties in the array
        foreach (SO_GridProperties sO_GridProperties in sO_GridPropertiesArray)
        {
            // Get gridpropertydetails dictionary for scene
            if (GameObjectSave.sceneData.TryGetValue(sO_GridProperties.sceneName.ToString(), out SceneSave sceneSave))
            {
                if (sceneSave.gridPropertyDetailsDictionary != null)
                {
                    for (int i = sceneSave.gridPropertyDetailsDictionary.Count - 1; i >= 0; i--)
                    {
                        KeyValuePair<string, GridPropertyDetails> item = sceneSave.gridPropertyDetailsDictionary.ElementAt(i);

                        GridPropertyDetails gridPropertyDetails = item.Value;

                        #region Update all grid properties to reflect the advance in the day

                        // If a crop is planted
                        if (gridPropertyDetails.growthDays > -1)
                        {
                            gridPropertyDetails.growthDays += 1;
                        }

                        // If ground is watered, then clear water
                        if (gridPropertyDetails.daysSinceWatered > -1)
                        {
                            gridPropertyDetails.daysSinceWatered = -1;
                        }

                        // Set gridpropertiesdetails
                        SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave.gridPropertyDetailsDictionary);

                        #endregion Update all grid properties to reflect the advance in the day
                    }
                }
            }
        }

        // Display grid property details to reflect changed values
        DisplayGridPropertyDetails();
    }
}
