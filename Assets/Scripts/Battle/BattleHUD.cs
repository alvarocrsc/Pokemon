using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text pokemonName;
    public Text pokemonLevel;
    public HealthBar healthBar;
    public Text pokemonHealth;

    private Pokemon _pokemon;

    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        
        pokemonName.text = pokemon.Base.Name;
        pokemonLevel.text = $"Lv. {pokemon.Level}";
        healthBar.SetHP((float) _pokemon.HP / _pokemon.MaxHP);
        UpdatePokemonData(pokemon.HP);
    }

    public void UpdatePokemonData(int oldHPValue)
    {
        StartCoroutine(healthBar.SetSmoothHP((float) _pokemon.HP / _pokemon.MaxHP));

        if (pokemonHealth != null)
        {
            StartCoroutine(DecreaseHealthPoints(oldHPValue));
        }
    }

    private IEnumerator DecreaseHealthPoints(int oldHPValue)
    {
        if (pokemonHealth != null)
        {
            while (oldHPValue > _pokemon.HP)
            {
                oldHPValue--;
                pokemonHealth.text = $"{oldHPValue} / {_pokemon.MaxHP}";
                yield return new WaitForSeconds(0.1f);
            }
            pokemonHealth.text = $"{_pokemon.HP} / {_pokemon.MaxHP}";
        }
    }
}
