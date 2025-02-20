[System.Serializable]
public class Cards
{
    public int id;
    public string cardName;
    public int cost;
    public int power;
    public int life;
    public string cardDescription;

    public Cards(int id, string cardName, int cost, int power, int life, string cardDescription)
    {
        this.id = id;
        this.cardName = cardName;
        this.cost = cost;
        this.power = power;
        this.life = life;
        this.cardDescription = cardDescription;
    }
}