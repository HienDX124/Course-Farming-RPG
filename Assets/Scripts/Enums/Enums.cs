public enum AnimationName
{
    idleDown,
    idleUp,
    idleRight,
    idleLeft,

    walkDown,
    walkUp,
    walkRight,
    walkLeft,

    runDown,
    runUp,
    runRight,
    runLeft,

    useDown,
    useUp,
    useRight,
    useLeft,

    swingDown,
    swingUp,
    swingRight,
    swingLeft,

    liftDown,
    liftUp,
    liftRight,
    liftLeft,

    holdDown,
    holdUp,
    holdRight,
    holdLeft,

    pickDown,
    pickUp,
    pickRight,
    pickLeft,

    count
}

public enum CharacterPartAnimator
{
    body,
    arms,
    hair,
    tool,
    hat,
    count
}

public enum PartVariantColour
{
    none,
    count
}

public enum PartVariantType
{
    none,
    carry,
    hoe,
    pickaxe,
    axe,
    scythe,
    wateringCan,
    count
}

public enum GridBoolProperty
{
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,
    isNPCObstacle

}

public enum InventoryLocation
{
    player,
    chest,
    count
}

public enum SceneName
{
    Scene1_Farm,
    Scene2_Field,
    Scene3_Cabin
}

public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter,
    none,
    count
}

public enum ToolEffect
{
    none,
    watering
}

public enum Direction
{
    up,
    down,
    left,
    right
}

public enum ItemType
{
    Seed,
    Commondity,
    Watering_tool,
    Hoeing_tool,
    Chopping_tool,
    Breaking_tool,
    Reaping_tool,
    Collecting_tool,
    Reapable_scenary,
    Furniture,
    none,
    count
}