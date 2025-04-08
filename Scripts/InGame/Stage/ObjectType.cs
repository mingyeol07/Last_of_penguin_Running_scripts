public enum ObjectType
{
    None,

    //ground
    Ground_ice, Ground_ice_snow, Ground_snow,

    //tile
    AirTile_center_1, AirTile_center_2, AirTile_center_3,
    AirTile_left, AirTile_right, AirTile_alone,
    IceTile_1, IceTile_2, IceTile_half,
    SnowTile, SnowTile_half,

    //bottom obstacle
    Obstacle_crystal, Obstacle_crystal_flower, Obstacle_snowman, Obstacle_snowman_melted,
    Obstacle_thorn, Obstacle_thorn_big, Obstacle_wave,

    //top obstacle
    Obstacle_crystal_top, 
    Obstacle_icicle, Obstacle_icicle_big, Obstacle_icicle_many,
    Obstacle_pillar_1, Obstacle_pillar_2,

    Obstacle_hexagon, Obstacle_hexagon_half,

    //monster
    FireBall, MiniSnowMan, Shark, Walrus,

    //clearFlag
    ClearFlag
}

public enum JellyType
{
    None,
    Fish_red, Fish_blue,
    ShellFish, Shrimp, Squid,

    FeverStar_ice, FeverStar_gold,
    Apple_ice, Apple_gold,
    TimeItem,
}