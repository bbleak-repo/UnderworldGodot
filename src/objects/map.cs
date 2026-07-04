namespace Underworld
{

    /// <summary>
    /// The map scroll
    /// </summary>
    public class map : objectInstance
    {
        public static bool Use(uwObject obj, bool WorldObject)
        {
            if (WorldObject) { return false; }
            if (_RES == GAME_UW2)
            {
                var worldno = worlds.GetWorldNo(playerdat.dungeon_level);
                uimanager.DrawAutoMap(playerdat.dungeon_level - 1, worldno);
                // Override combat music with exploration theme while viewing map
                if (playerdat.play_drawn == 1)
                {
                    XMIMusic.PickLevelThemeMusic(0);
                }
            }
            else
            {
                XMIMusic.ChangeThemeMusic(XMIMusic.MapsAndLegends);//play maps and legends in UW1 only.
                XMIMusic.RefreshMusic();
                uimanager.DrawAutoMap(playerdat.dungeon_level - 1, 0);

            }

            return false;
        }
    }//end class
}//end namespace