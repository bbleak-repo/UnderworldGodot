using System.Diagnostics;

namespace Underworld
{
    public partial class ConversationVM : UWClass
    {
        public static void give_ptr_npc(uwObject talker)
        {
            var qty = at(at(stackptr-1));
            var ObjectIndex = at(at(stackptr-2));
            Debug.Print($"{qty} of {ObjectIndex}");
            //try and find in trade area first.
            var itemcount = GetPlayerSelectedTradeItems(out int[] itemIds, out int[] itemIndices);

            for (int i=0; i<itemcount;i++)
            {
                if (ObjectIndex == itemIndices[i])
                {
                    GiveItemIndexToNPC(talker, ObjectIndex);
                    result_register =1;
                    return;
                }                
            }
            //if not found try and find directly in player inventory and take qty of that object
            var ptrObject = UWTileMap.current_tilemap.LevelObjects[ObjectIndex];
            if (ptrObject != null)
            {
                if (qty <= 0)
                {
                    //give all -- move the entire object to the NPC's inventory chain
                    ptrObject.next = talker.link;
                    talker.link = ptrObject.index;
                }
                else
                {
                    if (ptrObject.ObjectQuantity > qty)
                    {
                        //give partial qty -- clone a subset and give the clone to the NPC
                        var clone = ObjectCreator.spawnObjectInTile(
                            itemid: ptrObject.item_id,
                            tileX: 99, tileY: 99,
                            xpos: ptrObject.xpos, ypos: ptrObject.ypos, zpos: ptrObject.zpos,
                            WhichList: ObjectFreeLists.ObjectListType.StaticList);

                        if (clone != null)
                        {
                            clone.is_quant = ptrObject.is_quant;
                            clone.flags_full = ptrObject.flags_full;
                            clone.quality = ptrObject.quality;
                            clone.owner = ptrObject.owner;
                            clone.link = (short)qty;
                            ptrObject.link -= (short)qty;

                            clone.next = talker.link;
                            talker.link = clone.index;
                        }
                    }
                    else
                    {
                        //give all -- player has qty or fewer, give the whole stack
                        ptrObject.next = talker.link;
                        talker.link = ptrObject.index;
                    }
                }
                result_register = 1;
                return;
            }

            result_register = 0;//nothing traded
        }
    }//end class
}//end namespace