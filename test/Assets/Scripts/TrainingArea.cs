using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TrainingArea: MonoBehaviour {

    [SerializeField]
    internal List<BotAgent> team1;
    [SerializeField]
    private Transform team1SpawnArea;
    [SerializeField]
    private Text team1RewardText;

    [SerializeField]
    internal List<BotAgent> team2;
    [SerializeField]
    private Transform team2SpawnArea;
    [SerializeField]
    private Text team2RewardText;

    [SerializeField]
    private List<Transform> arenas;//curriculum arenas, steps up in dificulty to navigate

    internal BotAcademy academy;

    [SerializeField]
    private float resetCooldown = 1;

    private float timeAtLastReset = 0;

    private void FixedUpdate() {

        team1RewardText.text = "";
        foreach (BotAgent bot in team1) {
            team1RewardText.text += string.Format("{0:N3}", bot.GetCumulativeReward()) + "\n";
        }
        team2RewardText.text = "";
        foreach (BotAgent bot in team2) {
            team2RewardText.text += string.Format("{0:N3}", bot.GetCumulativeReward()) + "\n";
        }
    }

    internal void ResetArea(BotAgent resetter) {
        if (Time.time - timeAtLastReset >= resetCooldown) {
            Debug.Log("RESETTING TRAINING AREA");
            ResetSpawnPositions();
            int arena = (int)academy.resetParameters["arena"];

            foreach (Transform ar in arenas) {
                ar.gameObject.SetActive(false);
            }

            arenas[arena].gameObject.SetActive(true);

            foreach (BotAgent bot in team1) {
                if (bot != resetter) {
                    bot.Done();
                }
            }
            foreach (BotAgent bot in team2) {
                if (bot != resetter) {
                    bot.Done();
                }
            }
            timeAtLastReset = Time.time;
        }
    }

    //moves the agents to a random position within the spawn area
    private void ResetSpawnPositions() {
        foreach (BotAgent bot in team1) {
            Vector3 spawnPos = GetRandomSpawnPosition(team1SpawnArea);
            bot.transform.position = spawnPos;
            bot.SpawnPosition = spawnPos;
            bot.transform.localRotation = bot.spawnRotation;
        }
        foreach (BotAgent bot in team2) {
            Vector3 spawnPos = GetRandomSpawnPosition(team2SpawnArea);
            bot.transform.position = spawnPos;
            bot.SpawnPosition = spawnPos;
            bot.transform.localRotation = bot.spawnRotation;
        }
    }

    private Vector3 GetRandomSpawnPosition(Transform spawn) {

        Vector3 origin = spawn.position;
        Vector3 range = spawn.localScale / 2.0f;
        Vector3 randomRange = new Vector3(Random.Range(-range.x, range.x), Random.Range(-range.y, range.y), Random.Range(-range.z, range.z));

        return origin + randomRange;
    }
}
