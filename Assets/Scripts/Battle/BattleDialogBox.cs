using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ActionOption
{
   public Image image;
   public Sprite normalSprite;
   public Sprite selectedSprite;
}

[Serializable]
public class MovementTypeSprite
{
   public PokemonType type;
   public Sprite normalSprite;
   public Sprite selectedSprite;
}

public class BattleDialogBox : MonoBehaviour
{
   [SerializeField] Text dialogText;

   [SerializeField] GameObject actionSelect;
   [SerializeField] GameObject movementSelect;
   [SerializeField] GameObject movementDesc;

   [SerializeField] List<ActionOption> actionTexts;
   [SerializeField] List<Text> movementTexts;
   [SerializeField] List<Image> movementImages;
   [SerializeField] List<MovementTypeSprite> movementTypeSprites;

   [SerializeField] private Text typeText;
   [SerializeField] private Text ppText;

   public float charactersPerSecond = 10.0f;
   public float timeToWaitAfterText = 1.0f;

   public bool isWriting = false;
   
   public AudioClip[] characterSounds;

   private List<Move> currentMoves;

   public IEnumerator SetDialog(string message)
   {
      isWriting = true;
      
      dialogText.text = "";
      foreach (var character in message)
      {
         if (character != ' ')
         {
            SoundManager.SharedInstance.RandomSoundEffect(characterSounds);
         }
         dialogText.text += character;
         yield return new WaitForSeconds(1 / charactersPerSecond);
      }

      yield return new WaitForSeconds(timeToWaitAfterText);
      isWriting = false;
   }

   public void ToggleDialogText(bool activated)
   {
      dialogText.enabled = activated;
   }

   public void ToggleActions(bool activated)
   {
      actionSelect.SetActive(activated);
   }

   public void ToggleMovements(bool activated)
   {
      movementSelect.SetActive(activated);
      movementDesc.SetActive(activated);
   }

   public void SelectAction(int selectedAction)
   {
      for (int i = 0; i < actionTexts.Count; i++)
      {
         actionTexts[i].image.sprite = (i == selectedAction)
             ? actionTexts[i].selectedSprite
             : actionTexts[i].normalSprite;
      }
   }

   public void SetPokemonMovements(List<Move> moves)
   {
      currentMoves = moves;
      for (int i = 0; i < movementTexts.Count; i++)
      {
         if (i < moves.Count)
         {
            movementTexts[i].text = moves[i].Base.Name;
            movementImages[i].sprite = GetTypeSprite(moves[i].Base.Type, false);
         }
         else
         {
            movementTexts[i].text = "----";
         }
      }
   }

   public void SelectMovement(int selectedMovement, Move move)
   {
      if (currentMoves == null) return;

      for (int i = 0; i < movementImages.Count; i++)
      {
         if (i >= currentMoves.Count) continue;
         movementImages[i].sprite = GetTypeSprite(currentMoves[i].Base.Type, i == selectedMovement);
      }
      
      ppText.text = $"PP: {move.Pp}/{move.Base.PP}";
      typeText.text = move.Base.Type.ToString();
   }

   Sprite GetTypeSprite(PokemonType type, bool selected)
   {
      var entry = movementTypeSprites.Find(t => t.type == type);
      if (entry == null) return null;
      return selected ? entry.selectedSprite : entry.normalSprite;
   }
}
