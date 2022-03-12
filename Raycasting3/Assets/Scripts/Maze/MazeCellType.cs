public enum MazeCellType {
    // Basic types
    WALL,
    PASSAGE,
    DEAD_END,
    ROOM,
    HIDDEN_ROOM,
    DOOR,
    HIDDEN_DOOR,

    // Used for generation; not in final maze
    DISCONNECTED_DOOR,
    DISCONNECTED_HIDDEN_DOOR
};
