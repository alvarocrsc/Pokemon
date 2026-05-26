using System.Collections.Generic;
using UnityEngine;

public class PokemonMapArea : MonoBehaviour
{
    [SerializeField] private List<Pokemon> wildPokemon;

    public Pokemon GetRandomWildPokemon()
    {
        var pokemon = wildPokemon[Random.Range(0, wildPokemon.Count)];
        pokemon.InitPokemon();
        return pokemon;
    }

}
