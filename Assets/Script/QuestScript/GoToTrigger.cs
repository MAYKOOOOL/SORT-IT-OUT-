using UnityEngine;

public class GoToTrigger : MonoBehaviour
{
    public Quest associatedQuest;

    private void Update()
    {
        // 1. Is the system active?
        // 2. Is THIS the active quest?
        // 3. Are we NOT yet ready to return?
        // 4. Does the quest want an arrow?
        if (QuestSystem.Instance.activeQuest == associatedQuest
            && !QuestSystem.Instance.isReadyToReturn
            && associatedQuest.showArrow)
        {
            // Tell the arrow to look at ME
            QuestSystem.Instance.arrowPointer.SetTarget(this.transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Report to system
            QuestSystem.Instance.ReportLocationReached(associatedQuest);
        }
    }
}