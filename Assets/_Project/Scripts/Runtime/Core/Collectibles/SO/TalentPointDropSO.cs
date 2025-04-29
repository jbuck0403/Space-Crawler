using UnityEngine;

[CreateAssetMenu(fileName = "TalentPointDrop", menuName = "Collectibles/TalentPointDrop")]
public class TalentPointDropSO : CollectibleDropSO
{
    [SerializeField]
    private int talentPointAmount = 1;

    public override void HandleCollection()
    {
        GameManager.Instance.GameData.AddTalentPoint(talentPointAmount);
    }
}
