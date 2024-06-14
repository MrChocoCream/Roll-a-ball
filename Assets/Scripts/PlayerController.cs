using UnityEngine;

// Include the namespace required to use Unity UI
using UnityEngine.UI;

using System.Collections;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;

public class PlayerController : MonoBehaviour {
	
	// Create public variables for player speed, and for the Text UI game objects
	public float speed;
	public Text countText;
	public Text winText;
	public Text studentText; 
	public Text timerText;
	 

	// Create private references to the rigidbody component on the player, and the count of pick up objects picked up so far
	private Rigidbody rb;
	private int count;
	private float timeRemaining = 60f; // Tiempo restante en segundos
	private bool gameEnded = false; // Bandera para comprobar si el juego ha terminado
	private bool isMoving = false; // Bandera para comprobar si el jugador se está moviendo
	private Vector3 initialPosition; // Posición inicial del jugador al inicio del juego
	private GameObject[] pickUps;

	// At the start of the game..
	void Start ()
	{
		// Assign the Rigidbody component to our private rb variable
		rb = GetComponent<Rigidbody>();

		// Set the count to zero 
		count = 0;

		// Run the SetCountText function to update the UI (see below)
		SetCountText ();

		// Set the text property of our Win Text UI to an empty string, making the 'You Win' (game over message) blank
		winText.text = "";

		initialPosition = transform.position; //Guardar posicion del jugador

		// Buscar todos los objetos con el tag "Pick Up"
        pickUps = GameObject.FindGameObjectsWithTag("Pick Up");
		

		// Configurar el texto 
        studentText.text = "Jose A. Pascual 1-19-2529";
        // Posicion del texto
        RectTransform studentTextRect = studentText.GetComponent<RectTransform>();
        studentTextRect.anchorMin = new Vector2(1, 1);
        studentTextRect.anchorMax = new Vector2(1, 1);
        studentTextRect.pivot = new Vector2(1, 1);
        studentTextRect.anchoredPosition = new Vector2(-10, -10);
		// Configurar el texto del temporizador
        //timerText.text = timeRemaining.ToString("F0");

	}

	// Each physics step..
	void FixedUpdate ()
	{
		if (gameEnded) return;
		// Set some local float variables equal to the value of our Horizontal and Vertical Inputs
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		// Create a Vector3 variable, and assign X and Z to feature our horizontal and vertical float variables above
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		if (movement.magnitude > 0f && !isMoving)
        {
            // El jugador comienza a moverse
            isMoving = true;
            StartTimer(); // Iniciar el temporizador cuando el jugador se mueva por primera vez
        }

		// Add a physical force to our Player rigidbody using our 'movement' Vector3 above, 
		// multiplying it by 'speed' - our public player speed that appears in the inspector
		rb.AddForce (movement * speed);

		// Actualizar el temporizador
		if (isMoving && timerText != null)
        {
			timeRemaining -= Time.deltaTime;
			//timerText.text = timeRemaining.ToString("F0");

			// Verificar si el tiempo se ha agotado
			if (timeRemaining <= 0)
			{
				EndGame("Time's up! You Lose!");
			}
			UpdateTimerText();
		}	
	}

	// When this game object intersects a collider with 'is trigger' checked, 
	// store a reference to that collider in a variable named 'other'..
	void OnTriggerEnter(Collider other) 
	{
		if (gameEnded) return;
		// ..and if the game object we intersect has the tag 'Pick Up' assigned to it..
		if (other.gameObject.CompareTag ("Pick Up"))
		{
			// Make the other game object (the pick up) inactive, to make it disappear
			other.gameObject.SetActive (false);

			// Add one to the score variable 'count'
			count = count + 1;

			// Run the 'SetCountText()' function (see below)
			SetCountText ();
		}
	}

	// Create a standalone function that can update the 'countText' UI and check if the required amount to win has been achieved
	void SetCountText()
	{
		// Update the text field of our 'countText' variable
		countText.text = "Count: " + count.ToString ();

		// Check if our 'count' is equal to or exceeded 12
		if (count >= 12) 
		{
			// Set the text value of our 'winText'
			EndGame("You Win!");
			foreach (GameObject pickUp in pickUps)
			{
				pickUp.SetActive(false);
			}

		}
	}
	void EndGame(string message)
	{
		gameEnded = true;
		winText.text = message;

		//Activamos los objetos
		foreach (GameObject pickUp in pickUps)
        {
            pickUp.SetActive(true);
        }

		// Centramos el texto de fin del juego
		RectTransform rectTransform = winText.GetComponent<RectTransform>();
		rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
		rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
		rectTransform.pivot = new Vector2(0.5f, 0.5f);
		rectTransform.anchoredPosition = Vector2.zero;

		StartCoroutine(RestartAfterDelay());
	}
	IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(5f); // Esperar 5 segundos

        RestartGame();
    }
	void UpdateTimerText()
	{
		timerText.text = "Timer: " + timeRemaining.ToString("F0");
	}
	void StartTimer()
    {
        UpdateTimerText(); // Actualizar el texto del temporizador al inicio
    }
	public void RestartGame()
    {
		foreach (GameObject pickUp in pickUps)
        {
            pickUp.SetActive(true);
        }
        // Reiniciar todas las variables y objetos necesarios
        count = 0;
        SetCountText();
        timeRemaining = 60f;
        UpdateTimerText();
        gameEnded = false;
        winText.text = "";
        
		// Volver al jugador a la posición inicial
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;

        // Reiniciar las banderas y variables relacionadas con el movimiento
        isMoving = false;
    }

}