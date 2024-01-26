using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GoalTemplate : MonoBehaviour
{
    public TMP_Text RequiredAmountText;
    public Image Image;

    public void Init(int RequiredAmount, Sprite sprite)
    {
        RequiredAmountText.text = RequiredAmount.ToString();
        Image.sprite = sprite;
    }
    public void ChangeText(int RequiredAmount)
    {
        RequiredAmountText.text = RequiredAmount.ToString();
    }
}
