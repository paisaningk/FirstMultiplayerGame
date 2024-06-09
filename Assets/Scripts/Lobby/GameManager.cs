﻿using System;
using Fusion;
using TMPro;
using UnityEngine;

namespace Lobby
{
    public class GameManager : NetworkBehaviour
    {
        public static bool MatchIsOver { get; private set; }
        public new Camera camera;
        public TMP_Text timerText;
        public float matchTime = 60;
        
        [Networked] private TickTimer MatchTimer { get; set; }

        public override void Spawned()
        {
            MatchIsOver = false;
            camera.gameObject.SetActive(false);

            MatchTimer = TickTimer.CreateFromSeconds(Runner, matchTime);
        }

        public override void FixedUpdateNetwork()
        {
            if (MatchTimer.Expired(Runner) == false && MatchTimer.RemainingTime(Runner).HasValue)
            {
                var timeSpan = TimeSpan.FromSeconds(MatchTimer.RemainingTime(Runner).Value);
                var output = $"{timeSpan.Minutes:D2} : {timeSpan.Seconds:D2}";
                timerText.SetText(output);
            }
            else if (MatchTimer.Expired(Runner))
            {
                MatchIsOver = true;
                MatchTimer = TickTimer.None;
            }
        }
    }
}