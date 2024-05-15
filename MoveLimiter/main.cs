using MelonLoader;
using RUMBLE.Managers;
using RUMBLE.Players.Subsystems;
using RUMBLE.Poses;
using System.Collections;
using UnityEngine;

namespace MoveLimiter
{
    public class main : MelonMod
    {
        private bool sceneChanged = false;
        private string currentScene = "";
        int sceneChangeCount = 0;

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            currentScene = sceneName;
            sceneChanged = true;
            sceneChangeCount++;
        }

        public override void OnFixedUpdate()
        {
            if (sceneChanged)
            {
                if ((currentScene == "Map0") || (currentScene == "Map1"))
                {
                    MelonCoroutines.Start(WaitASecondBeforePlayerCheck(sceneChangeCount));
                }
                sceneChanged = false;
            }
        }

        public IEnumerator WaitASecondBeforePlayerCheck(int sceneCount)
        {
            for (int i = 0; i < 300; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            while (PlayerManager.instance.localPlayer.Controller == null)
            {
                yield return new WaitForFixedUpdate();
            }
            bool enabled = PlayerManager.instance.localPlayer.Controller.gameObject.transform.GetComponentInChildren<PlayerPoseSystem>().enabled;
            while (!enabled)
            {
                if (sceneCount != sceneChangeCount) { break; }
                enabled = PlayerManager.instance.localPlayer.Controller.gameObject.transform.GetComponentInChildren<PlayerPoseSystem>().enabled;
                yield return new WaitForFixedUpdate();
            }
            if (enabled)
            {
                if (PlayerManager.instance.AllPlayers.Count == 2)
                {
                    ChangeAvailableMoves(PlayerManager.instance.AllPlayers[1].Data.GeneralData.BattlePoints);
                }
            }
        }

        public void ChangeAvailableMoves(int opponentBP)
        {
            if ((opponentBP >= 156) || (opponentBP >= PlayerManager.instance.localPlayer.Data.GeneralData.BattlePoints)) { return; }
            System.Collections.Generic.List<string> movesToKeep = new System.Collections.Generic.List<string>();
            string beltRankFound = "White Belt";
            movesToKeep.Add("PoseSetDisc");
            movesToKeep.Add("PoseSetSpawnPillar");
            movesToKeep.Add("PoseSetStraight");
            movesToKeep.Add("SprintingPoseSet");
            if (opponentBP >= 12)
            {
                beltRankFound = "Yellow Belt";
                movesToKeep.Add("PoseSetBall");
                movesToKeep.Add("PoseSetKick");
                movesToKeep.Add("PoseSetStomp");
            }
            if (opponentBP >= 30)
            {
                beltRankFound = "Green Belt";
                movesToKeep.Add("PoseSetWall_Grounded");
                movesToKeep.Add("PoseSetRockjump");
                movesToKeep.Add("PoseSetUppercut");
            }
            if (opponentBP >= 54)
            {
                beltRankFound = "Blue Belt";
                movesToKeep.Add("PoseSetSpawnCube");
                movesToKeep.Add("PoseSetDash");
            }
            if (opponentBP >= 96)
            {
                beltRankFound = "Red Belt";
                movesToKeep.Add("PoseSetParry");
                movesToKeep.Add("PoseSetHoldLeft");
                movesToKeep.Add("PoseSetHoldRight");
            }
            Il2CppSystem.Collections.Generic.List<PoseInputSource> activePoses = PlayerManager.instance.localPlayer.Controller.gameObject.transform.GetComponentInChildren<PlayerPoseSystem>().currentInputPoses;
            string poseList = "";
            for (int i = 0; i < activePoses.Count; i++)
            {
                bool poseFound = false;
                for (int x = 0; x < movesToKeep.Count; x++)
                {
                    if (activePoses[i].poseSet.name == movesToKeep[x])
                    {
                        poseFound = true;
                    }
                }
                if (!poseFound)
                {
                    if (poseList != "")
                    {
                        poseList += ", ";
                    }
                    poseList += activePoses[i].poseSet.name;
                    activePoses.RemoveAt(i);
                    i--;
                }
            }
            MelonLogger.Msg($"Facing {beltRankFound}, Removing Poses: {poseList}");
        }
    }
}
