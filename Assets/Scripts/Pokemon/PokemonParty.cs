using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemons;

    public List<Pokemon> Pokemon
    {
        get => pokemons;
        set => pokemons = value;
    }

    private void Awake()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.InitPokemon();
        }
    }

    public Pokemon GerFirstNonFaintedPokemon()
    {
        return pokemons.FirstOrDefault(p => p.Base != null && p.HP > 0);
    }
}
