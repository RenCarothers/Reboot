using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public enum PlayerAttackInProgress { YES, NO }
public enum PlayerHealInProgress { YES, NO }

public class BattleSystem : MonoBehaviour
{
	public Fungus.Flowchart flowchart;

	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	public Transform playerBattleStation;
	public Transform enemyBattleStation;

	Unit playerUnit;
	Unit enemyUnit;

	public Text dialogueText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;
	public PlayerAttackInProgress playerAttackState; // state of if player is attacking currently
	public PlayerHealInProgress playerHealState; // "" healing

    // Start is called before the first frame update
    void Start()
    {
		state = BattleState.START;
		playerAttackState = PlayerAttackInProgress.NO; // at the start of the game, the player is not attacking
		playerHealState = PlayerHealInProgress.NO; // "" healing
		// Debug.Log(state);
		StartCoroutine(SetupBattle());
    }

	IEnumerator SetupBattle()
	{
		GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();

		GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		enemyUnit = enemyGO.GetComponent<Unit>();

		dialogueText.text = "A wild " + enemyUnit.unitName + " approaches...";

		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

		yield return new WaitForSeconds(3f);

		dialogueText.text = enemyUnit.unitName + " appears weakened.";

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}

	IEnumerator PlayerAttack()
	{
		playerAttackState = PlayerAttackInProgress.YES; // update state; the player is currently in an attack

		dialogueText.text = "Rockett attacks!";

		yield return new WaitForSeconds(1f);

		bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

		enemyHUD.SetHP(enemyUnit.currentHP);

		flowchart.ExecuteBlock("PunchEnemy");

		yield return new WaitForSeconds(1f);

		dialogueText.text = "The attack is successful!";

		yield return new WaitForSeconds(2f);

		Debug.Log(state);

		if(isDead)
		{
			state = BattleState.WON;
			EndBattle();
		} else
		{
			playerAttackState = PlayerAttackInProgress.NO; // the player's attack is finished
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
		}
	}

	IEnumerator EnemyTurn()
	{
		dialogueText.text = enemyUnit.unitName + " attacks!";

		flowchart.ExecuteBlock("PunchPlayer");

		yield return new WaitForSeconds(1f);

		dialogueText.text = "The attack lands feebly.";

		bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

		playerHUD.SetHP(playerUnit.currentHP);

		yield return new WaitForSeconds(1f);

		if(isDead)
		{
			state = BattleState.LOST;
			EndBattle();
		} else
		{
			state = BattleState.PLAYERTURN;
			PlayerTurn();
		}

	}

	void EndBattle()
	{
		if(state == BattleState.WON)
		{
			// dialogueText.text = "You won the battle!";

			// yield return new WaitForSeconds(2f);

			flowchart.ExecuteBlock("PlayerWins");

		} else if (state == BattleState.LOST)
		{
			dialogueText.text = "You were defeated."; // This is not possible in current setup
		}
	}

	void PlayerTurn()
	{
		dialogueText.text = "Choose an action:";
	}

	IEnumerator PlayerHeal()
	{
		playerHealState = PlayerHealInProgress.YES; // update state; the player is currently in a heal

		dialogueText.text = "Rockett turns to her friends.";

		yield return new WaitForSeconds(1f);

		dialogueText.text = "Stephanie cheers.";

		yield return new WaitForSeconds(1f);

		playerUnit.Heal(5);

		playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = "Rockett feels renewed strength!";

		yield return new WaitForSeconds(2f);

		state = BattleState.ENEMYTURN;
		playerHealState = PlayerHealInProgress.NO; // the player's heal is finished
		StartCoroutine(EnemyTurn());
	}

	public void OnAttackButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;
		else if (playerAttackState != PlayerAttackInProgress.NO) // if the player is in an attack, can't attack again
			return;
		else if (playerHealState != PlayerHealInProgress.NO) // if the player is in an heal, can't attack
			return;		

		StartCoroutine(PlayerAttack());
	}

	public void OnHealButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;
		else if (playerAttackState != PlayerAttackInProgress.NO) // if the player is in an attack, can't heal
			return;
		else if (playerHealState != PlayerHealInProgress.NO) // if the player is in an heal, can't heal again
			return;					

		StartCoroutine(PlayerHeal());
	}

}
