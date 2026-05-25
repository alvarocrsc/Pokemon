using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHUD playerHUD;

    private void Start()
    {
        SetupBattle();
    }

    public void SetupBattle()
    {
        playerUnit.SetupPokemon();
        playerHUD.SetPokemonData(playerUnit.Pokemon);
    }
}
