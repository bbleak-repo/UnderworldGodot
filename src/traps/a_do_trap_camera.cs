using Peaky.Coroutines;
using System.Collections;

namespace Underworld
{

    public class a_do_trap_camera : objectInstance
    {
       
        /// <summary>
        /// Waits until further input before clearing the cam.
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// True while camera trap is showing a remote view. Unlike WaitingForMore,
        /// this does NOT set blockinput, so NPCs continue to move and the game
        /// world keeps updating during the camera view.
        /// </summary>
        public static bool CameraViewActive = false;

        static IEnumerator CameraWaitForInput()
        {
            playerdat.CameraReference = null;//switch to the do trap
            playerdat.PositionPlayerCamera();
            bool automap = playerdat.AutomapEnabled;
            playerdat.AutomapEnabled = false;

            // Use our own flag instead of WaitingForMore so the game loop
            // keeps running (NPCs move, physics update) during camera views.
            CameraViewActive = true;
            MessageDisplay.WaitingForMore = true; // still needed to catch the keypress that dismisses it
            while (MessageDisplay.WaitingForMore)
            {
                yield return new WaitOneFrame();
            }
            CameraViewActive = false;

            playerdat.AutomapEnabled = automap;
            playerdat.CameraReference = playerdat.playerObject;
            playerdat.PositionPlayerCamera();
        }


        /// <summary>
        /// Activates the camera linked to the trap
        /// </summary>
        /// <param name="trapObj"></param>
        /// <param name="triggerObj"></param>
        /// <param name="objList"></param>
        public static void Activate(uwObject trapObj, uwObject[] objList)
        {
            //switch camera            
            do_trap_camera(trapObj.tileX, trapObj.tileY, trapObj.xpos, trapObj.ypos, trapObj.zpos, trapObj.heading);
        }

        public static void do_trap_camera(int tileX, int tileY, int xpos, int ypos, int zpos, int heading)
        {
            playerdat.DoCameraX = (short)((tileX << 8) + (xpos << 5));
            playerdat.DoCameraY = (short)((tileY << 8) + (ypos << 5));
            playerdat.DoCameraZ = (short)(zpos << 3);
            playerdat.DoCameraH = (short)(heading << 0xD);
            playerdat.DoCameraPitch = 0;
            playerdat.DoCameraRoll = 0;

            _ = Coroutine.Run(
                   CameraWaitForInput(),
                    main.instance
               );
        }
    }//end class
}//end namespace