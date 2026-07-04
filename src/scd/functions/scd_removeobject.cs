namespace Underworld
{
    public partial class scd : UWClass
    {
        /// <summary>
        /// Removes object(s) specified by the filter from the map.
        /// </summary>
        /// <param name="currentblock"></param>
        /// <param name="eventOffset"></param>
        /// <returns></returns>
        static int RemoveObject(byte[] currentblock, int eventOffset)
        {
            RunCodeOnObjects_SCD(
                methodToCall: RemoveObject,
                mode: currentblock[eventOffset + 5],
                filter: currentblock[eventOffset + 6],
                loopAll: true,
                currentblock: currentblock,
                eventOffset: eventOffset);
            return 0;
        }


        static void RemoveObject(uwObject obj, int[] paramsarray)
        {
            if (UWTileMap.ValidTile(obj.tileX, obj.tileY))
            {
                // Check if this object has an associated animation overlay and remove it
                var overlay = AnimationOverlay.FindOverlay(obj.index);
                if (overlay != null)
                {
                    AnimationOverlay.RemoveAnimationOverlay(overlay.link);
                }

                ObjectRemover_OLD.DeleteObjectFromTile_DEPRECIATED(
                    tileX: obj.tileX, tileY: obj.tileY,
                    indexToDelete: obj.index);
            }
        }
    }//end class
}//end namesace