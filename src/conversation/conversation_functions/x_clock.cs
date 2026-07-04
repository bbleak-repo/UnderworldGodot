using System.Diagnostics;

namespace Underworld
{
    public partial class ConversationVM:UWClass
    {
        public static void x_clock()
        {
            var xvalue = at(at(stackptr-1));
            var xnumber = at(at(stackptr-2));            
           
            if (xvalue>0x100)
            {                
                result_register = playerdat.GetXClock(xnumber);
                Debug.Print($"getting xclock {xnumber} {result_register}");
            }
            else
            {
                if (xnumber==0)
                {//advance gameclock by xvalue units
                    Debug.Print($"x_clock: advancing game clock by {xvalue}");
                    playerdat.ClockValue += xvalue * 0x3BC4; // advance by xvalue game-time units
                }
                else
                {
                    //set xclock value
                    Debug.Print($"setting xclock {xnumber} to {xvalue}");
                    playerdat.SetXClock(xnumber,xvalue);
                }  
                result_register = 0;              
            }
        }
    } //end class
}//end namespace