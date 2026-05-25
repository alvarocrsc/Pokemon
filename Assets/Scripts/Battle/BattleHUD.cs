using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text pokemonName;
    public Text pokemonLevel;
    public HealthBar healthBar;
    public Text pokemonHealth;

    public void SetPokemonData(Pokemon pokemon)
    {
        pokemonName.text = pokemon.Base.Name;
        pokemonLevel.text = $"Lv. {pokemon.Level}";
        healthBar.SetHP(pokemon.HP / pokemon.MaxHP);
        pokemonHealth.text = $"{pokemon.HP} / {pokemon.MaxHP}";
    }
}
