using System;
using System.Collections;
using UnityEngine;

public enum BattleState
{
    StartBattle,
    PlayerSelectAction,
    PlayerMove,
    EnemyMove,
    Busy
}
public class BattleManager : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHUD playerHUD;

    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHUD enemyHUD;

    [SerializeField] BattleDialogBox battleDialogBox;

    public BattleState state;

    public event Action<bool> OnBattleFinish;

    private PokemonParty playerParty;
    private Pokemon wildPokemon;

    public AudioClip attackClip, damageClip, endBattleClip; 

    public void HandleStartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        state = BattleState.StartBattle;

        playerUnit.SetupPokemon(playerParty.GerFirstNonFaintedPokemon());
        playerHUD.SetPokemonData(playerUnit.Pokemon);

        battleDialogBox.SetPokemonMovements(playerUnit.Pokemon.Moves);

        enemyUnit.SetupPokemon(wildPokemon);
        enemyHUD.SetPokemonData(enemyUnit.Pokemon);

        yield return battleDialogBox.SetDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared!");

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerSelectAction;
        StartCoroutine(battleDialogBox.SetDialog($"What will {playerUnit.Pokemon.Base.Name} do?"));
        battleDialogBox.ToggleDialogText(true);
        battleDialogBox.ToggleActions(true);
        battleDialogBox.ToggleMovements(false);
        currentSelectedAction = 0;
        battleDialogBox.SelectAction(currentSelectedAction);
    }

    void PlayerMovement()
    {
        state = BattleState.PlayerMove;
        battleDialogBox.ToggleDialogText(false);
        battleDialogBox.ToggleActions(false);
        battleDialogBox.ToggleMovements(true);
        currentSelectedMovement = 0;
        battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokemon.Moves[currentSelectedMovement]);
    }

    public void HandleUpdate()
    {
        if (battleDialogBox.isWriting)
        {
            return;
        }

        if (state == BattleState.PlayerSelectAction)
        {
            HandlePlayerActionSelection();
            HandlePlayerActionConfirm();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandlePlayerMovementSelection();
            HandlePlayerMovementConfirm();
        }
    }

    private int currentSelectedAction;

    void HandlePlayerActionSelection()
    {
        // 2x2 grid:
        //   0 Fight  | 1 Bag
        //   2 Pkmn   | 3 Run
        int row = currentSelectedAction / 2;
        int col = currentSelectedAction % 2;

        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && row < 1)
        {
            row++;
        }
        else if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && row > 0)
        {
            row--;
        }
        else if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && col < 1)
        {
            col++;
        }
        else if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && col > 0)
        {
            col--;
        }
        else
        {
            return;
        }

        currentSelectedAction = row * 2 + col;
        battleDialogBox.SelectAction(currentSelectedAction);
    }

    void HandlePlayerActionConfirm()
    {
        if (Input.GetButtonDown("Submit"))
        {
            if (currentSelectedAction == 0)
            {
                PlayerMovement();
            }
            else if (currentSelectedAction == 3)
            {
                //TODO: implement run
            }
        }
    }

    private int currentSelectedMovement;

    void HandlePlayerMovementSelection()
    {
        // 2x2 grid:
        //   0 Move 1  | 1 Move 2
        //   2 Move 3  | 3 Move 4
        int moveCount = playerUnit.Pokemon.Moves.Count;
        int row = currentSelectedMovement / 2;
        int col = currentSelectedMovement % 2;

        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && row < 1)
        {
            row++;
        }
        else if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && row > 0)
        {
            row--;
        }
        else if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && col < 1)
        {
            col++;
        }
        else if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && col > 0)
        {
            col--;
        }
        else
        {
            return;
        }

        int newIndex = row * 2 + col;
        if (newIndex >= moveCount)
        {
            return;
        }

        currentSelectedMovement = newIndex;
        battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokemon.Moves[currentSelectedMovement]);
    }

    void HandlePlayerMovementConfirm()
    {
        if (Input.GetButtonDown("Submit"))
        {
            battleDialogBox.ToggleMovements(false);
            battleDialogBox.ToggleDialogText(true);
            StartCoroutine(PerformPlayerMovement());
        }
    }

    IEnumerator PerformPlayerMovement()
    {
        Move move = playerUnit.Pokemon.Moves[currentSelectedMovement];
        move.Pp--;
        yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.Name}");

        var oldHPValue = enemyUnit.Pokemon.HP;

        playerUnit.PlayAttackAnimation();
        SoundManager.SharedInstance.PlaySound(attackClip);
        yield return new WaitForSeconds(1f);
        enemyUnit.PlayReceiveAttackAnimation();
        SoundManager.SharedInstance.PlaySound(damageClip);

        var damageDesc = enemyUnit.Pokemon.ReceiveDamage(playerUnit.Pokemon, move);
        enemyHUD.UpdatePokemonData(oldHPValue);
        yield return ShowDamageDescription(damageDesc);

        if (damageDesc.Fainted)
        {
            yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} fainted.");
            enemyUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);

            OnBattleFinish(true);
        }
        else
        {
            StartCoroutine(EnemyAction());
        }
    }

    IEnumerator EnemyAction()
    {
        state = BattleState.EnemyMove;

        Move move = enemyUnit.Pokemon.RandomMove();
        if (move == null)
        {
            yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} has no moves and struggled!");
            PlayerAction();
            yield break;
        }
        move.Pp--;
        yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} used {move.Base.Name}");

        var oldHPValue = playerUnit.Pokemon.HP;

        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        playerUnit.PlayReceiveAttackAnimation();

        var damageDesc = playerUnit.Pokemon.ReceiveDamage(enemyUnit.Pokemon, move);
        playerHUD.UpdatePokemonData(oldHPValue);
        yield return ShowDamageDescription(damageDesc);

        if (damageDesc.Fainted)
        {
            yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} fainted {move.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);

            OnBattleFinish(false);
        }
        else
        {
            PlayerAction();
        }
    }

    IEnumerator ShowDamageDescription(DamageDescription desc)
    {
        if (desc.Critial > 1)
        {
            yield return battleDialogBox.SetDialog("A critical hit!");
        }

        if (desc.Type > 1)
        {
            yield return battleDialogBox.SetDialog("It's super effective!");
        }
        else if (desc.Type < 1)
        {
            yield return battleDialogBox.SetDialog("It's not very effective...");
        }
    }
}
