using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/New Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private string name;
    public string Name => name;

    [TextArea][SerializeField] private string description;
    public string Description => description;

    [SerializeField] private PokemonType type;
    [SerializeField] private Category category;
    [SerializeField] private int pp;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;

    public PokemonType Type => type;
    public Category Category => category;
    public int PP => pp;
    public int Power => power;
    public int Accuracy => accuracy;
}
