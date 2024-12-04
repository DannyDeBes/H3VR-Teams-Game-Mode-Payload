using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TeamGameMode
{
    [System.Serializable]
    public class TGM_Teams
    {
        public static TGM_Teams instance;

        public Team[] teams = new Team[2];

        public class Team
        {
            public int iff; //Matches the array index, for internal access
            public TGM_SosigTeam sosigTeam; //Sosig Team
            public int sosigCount = 8;      //Total amount of sosigs on this team
            public int scoreGoal = 80;
            public int currentScore = 80;

            public TGM_Area currentSpawnArea;
        }
        public Vector3 GetTeamRoomSpawnPoint(int team)
        {
            return TGM_Manager.instance.gamemode.GetTeamRoomSpawnPoint(team);
        }
    }
}
