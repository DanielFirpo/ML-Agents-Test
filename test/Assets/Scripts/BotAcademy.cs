using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAcademy : Academy {

    [SerializeField]
    private List<TrainingArea> trainingAreas;

    private void Start() {
        foreach (TrainingArea trainingArea in trainingAreas) {
            trainingArea.academy = this;
        }
    }

    public override void AcademyReset() {//The academy finished with all training or finished a curriculum, lets reset all the arenas with updated reset parameters
        Debug.Log("RESETTING ACADEMY");
        foreach(TrainingArea trainingArea in trainingAreas) {
            trainingArea.ResetArea(null);
        }
    }

}
