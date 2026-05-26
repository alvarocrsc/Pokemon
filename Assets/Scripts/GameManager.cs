using System;
using UnityEngine;

public enum GameState
{
    Travel,
    Battle
}
public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Camera worldMainCamera;
    
    private GameState _gameState;

    public AudioClip worldClip, battleClip;

    private void Awake()
    {
        _gameState = GameState.Travel;
    }

    private void Start()
    {
        playerController.OnPokemonEncountered += StartPokemonBattle;
        battleManager.OnBattleFinish += FinishPokemonBattle;
    }

    void StartPokemonBattle()
    {
        SoundManager.SharedInstance.PlayMusic(battleClip);
        
        _gameState = GameState.Battle;
        battleManager.gameObject.SetActive(true);
        worldMainCamera.gameObject.SetActive(false);
        
        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<PokemonMapArea>().GetComponent<PokemonMapArea>().GetRandomWildPokemon();
        
        battleManager.HandleStartBattle(playerParty, wildPokemon);
    }

    void FinishPokemonBattle(bool playerHasWon)
    {
        SoundManager.SharedInstance.PlayMusic(worldClip);
        
        _gameState = GameState.Travel;
        battleManager.gameObject.SetActive(false);
        worldMainCamera.gameObject.SetActive(true);

        if (!playerHasWon)
        {
            // TODO: differentiate between win and loss
        }
    }

    private void Update()
    {
        if (_gameState == GameState.Travel)
        {
            playerController.HandleUpdate();
        } else if (_gameState == GameState.Battle)
        {
            battleManager.HandleUpdate();
        }
    }
}
