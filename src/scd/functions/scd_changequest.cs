using System.Diagnostics;

namespace Underworld
{
    public partial class scd : UWClass
    {
        /// <summary>
        /// SCD function 4: Changes a quest variable value.
        /// Event row layout: [+5] = quest number, [+6] = new value
        /// </summary>
        public static int ChangeQuest(byte[] currentblock, int eventOffset)
        {
            int questNo = currentblock[eventOffset + 5];
            int newValue = currentblock[eventOffset + 6];

            Debug.Print($"SCD ChangeQuest: quest {questNo} = {newValue}");
            playerdat.SetQuest(questNo, newValue);

            return 0;
        }
    }//end class
}//end namespace