using UnityEngine;
using UnityEngine.UIElements;

public class MainView : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    private Label QuantityLabel => uiDocument.rootVisualElement.Q<Label>("money");

    public void SetMoneyQuantity(int quantity)
    {
        QuantityLabel.text = quantity.ToString();
    }
}
