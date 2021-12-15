using System.Collections.Generic;
using System;

[System.Serializable]
public class GameObjectSave
{
    // string key = scene name
    public Dictionary<string, SceneSave> sceneData;

    public GameObjectSave()
    {
        sceneData = new Dictionary<string, SceneSave>();
    }

    public GameObjectSave(Dictionary<string, SceneSave> sceneData)
    {
        this.sceneData = sceneData;
    }


}
