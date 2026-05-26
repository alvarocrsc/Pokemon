using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    private bool isMoving;

    public float speed;
    private Vector2 input;

    private Animator _animator;

    public LayerMask solidObjectsLayer, pokemonLayer;

    public event Action OnPokemonEncountered;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0)
                input.y = 0; // Prevent diagonal movement

            if (input != Vector2.zero)
            {
                _animator.SetFloat("Move x", input.x);
                _animator.SetFloat("Move y", input.y);

                var targetPosition = transform.position;
                targetPosition.x += input.x;
                targetPosition.y += input.y;

                if (IsAvailable(targetPosition))
                {
                    StartCoroutine(MoveTowards(targetPosition));
                }
            }
        }
    }

    private void LateUpdate()
    {
        _animator.SetBool("is Moving", isMoving);
    }

    IEnumerator MoveTowards(Vector3 destination)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, destination) > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = destination;
        isMoving = false;

        CheckForPokemon();
    }

    private bool IsAvailable(Vector3 target)
    {
        if (Physics2D.OverlapCircle(target, 0.1f, solidObjectsLayer) != null)
        {
            return false;
        }

        return true;
    }

    private void CheckForPokemon()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.1f, pokemonLayer) != null)
        {
            if (Random.Range(0, 100) < 10)
            {
                OnPokemonEncountered();
            }
        }
    }
}
