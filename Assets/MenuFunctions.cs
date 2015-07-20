using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuFunctions : MonoBehaviour {

	public Text painUI;
	public Text salaryUI;
	public Text totalMoneyUI;
	// Use this for initialization
	void Start () {
		int salary = 100;
		int lives = PlayerPrefs.GetInt("lives");
		int initialLives = PlayerPrefs.GetInt("initialLives");
		int totalMoney = PlayerPrefs.GetInt("totalMoney");

		int totalPain = initialLives - lives;
		int onePainPerSalaryRest = 2;
		int totalPainPerSalaryRest = totalPain * onePainPerSalaryRest;
		int totalSalary = salary - totalPainPerSalaryRest;
		totalSalary = totalSalary < 0 ? 0 : totalSalary;
		totalMoney = totalMoney + totalSalary;

		painUI.text = totalPain.ToString();
		salaryUI.text = totalSalary.ToString();
		totalMoneyUI.text = totalMoney.ToString();

		PlayerPrefs.SetInt("totalMoney", totalMoney);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void startGame() {
		Application.LoadLevel("home1");
	}
}
