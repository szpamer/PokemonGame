using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;

    public event Action OnEncountered;
	public event Action<Collider2D> OnEnterTrainersView;
	
	private Vector2 input;
	
	private Character character;
	
	private void Awake()
	{
		character = GetComponent<Character>();
	}
	
	public void HandleUpdate()
	{
		if (!character.IsMoving)
		{
			input.x = Input.GetAxisRaw("Horizontal");
			input.y = Input.GetAxisRaw("Vertical");
			
			if (input.x != 0) input.y = 0;
			
			if (input != Vector2.zero)
			{
				StartCoroutine(character.Move(input, OnMoveOver));
			}
		}

		character.HandleUpdate();
		
		if (Input.GetKeyDown(KeyCode.Z))
			Interact();

	}
	
	void Interact()
	{
		var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
		var interactPos = transform.position + facingDir;

		var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
		if (collider != null)
		{
			collider.GetComponent<Interactable>()?.Interact(transform);
		}
	}
	
	private void OnMoveOver()
	{
		CheckForEncounters();
		CheckIfInTrainersView();
	}
	
	private void CheckForEncounters()
	{
		if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.GrassLayer) != null)
		{
			if (UnityEngine.Random.Range(1, 101) <= 10)
			{
				character.Animator.IsMoving = false;
				OnEncountered();
			}				
		}
	}

	private void CheckIfInTrainersView()
	{
		var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.FovLayer);
        if ( collider  != null)
		{
			character.Animator.IsMoving = false;
			OnEnterTrainersView?.Invoke(collider);
		}
    }
    public string Name
    {
        get { return name; }
    }
    public Sprite Sprite
    {
        get { return sprite; }
    }
}
