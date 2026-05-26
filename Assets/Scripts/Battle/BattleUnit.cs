using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class BattleUnit : MonoBehaviour
{
    public PokemonBase _base;
    public int _level;
    public bool isPlayer;

    public float framesPerSecond = 240f;

    public Pokemon Pokemon { get; set; }

    private Image pokemonImage;
    Vector3 initialPosition;
    private Color initialColor;

    [SerializeField] private float startTimeAnim = 0.8f, attackTimeAnim = 0.3f, dieTimeAnim = 0.8f, hitTimeAnim = 0.4f;

    private SpriteAnimator animator;

    private void Awake()
    {
        pokemonImage = GetComponent<Image>();
        initialPosition = pokemonImage.transform.localPosition;
        initialColor = pokemonImage.color;
    }

    public void SetupPokemon(Pokemon pokemon)
    {
        Pokemon = pokemon;

        if (Pokemon == null || Pokemon.Base == null)
        {
            Debug.LogError($"[BattleUnit] SetupPokemon called with invalid Pokemon (isPlayer={isPlayer}). " +
                           "Check your PokemonParty / PokemonMapArea — every entry must have a Base assigned in the Inspector.", this);
            return;
        }

        var frames = isPlayer ? Pokemon.Base.BackSprites : Pokemon.Base.FrontSprites;

        if (frames != null && frames.Count > 0)
        {
            animator = new SpriteAnimator(pokemonImage, frames, framesPerSecond);
            animator.Start();
        }
        else
        {
            animator = null;
            pokemonImage.sprite =
                (isPlayer) ? Pokemon.Base.BackSprite : Pokemon.Base.FrontSprite;
        }

        pokemonImage.color = initialColor;

        PlayStartAnimation();
    }

    private void Update()
    {
        if (animator != null)
        {
            animator.FramesPerSecond = framesPerSecond;
            animator.HandleUpdate();
        }
    }

    public void PlayStartAnimation()
    {
        pokemonImage.transform.localPosition =
            new Vector3(initialPosition.x + (isPlayer ? -1 : 1) * 400, initialPosition.y);

        pokemonImage.transform.DOLocalMoveX(initialPosition.x, startTimeAnim);
    }

    public void PlayAttackAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.transform.DOLocalMoveX(initialPosition.x + (isPlayer ? 1 : -1) * 60, attackTimeAnim));
        seq.Append(pokemonImage.transform.DOLocalMoveX(initialPosition.x, attackTimeAnim));
    }

    public void PlayReceiveAttackAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.DOColor(Color.gray, hitTimeAnim));
        seq.Append(pokemonImage.DOColor(initialColor, hitTimeAnim));
    }

    public void PlayFaintAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.transform.DOLocalMoveY(initialPosition.y - 150, dieTimeAnim));
        seq.Join(pokemonImage.DOFade(0, dieTimeAnim));
    }
}
