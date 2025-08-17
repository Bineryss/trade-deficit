using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int money = 0;
    [SerializeField] private MainView mainView;


    void Start()
    {
        mainView.SetMoneyQuantity(money);
    }

    public void AddMoney(int amount)
    {
        money += amount;
        mainView.SetMoneyQuantity(money);
    }

    public void RemoveMoney(int amount)
    {
        money -= amount;
        mainView.SetMoneyQuantity(money);
    }

    public int GetMoney()
    {
        return money;
    }
}
