using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/********************
 * DIALOGUE MANAGER *
 ********************
 * This Dialogue Manager is what links your dialogue which is sent by the Dialogue Trigger to Unity
 *
 * The Dialogue Manager navigates the sent text and prints it to text objects in the canvas and will toggle
 * the Dialogue Box when appropriate
 */

public class DialogueManager : MonoBehaviour
{
    public GameObject CanvasBox; // your fancy canvas box that holds your text objects
    public Text TextBox; // the text body
    public Text NameText; // the text body of the name you want to display
    public bool freezePlayerOnDialogue = true;
	private bool enteredY = false;
	private bool enteredN = false;
    // private bool isOpen; // represents if the dialogue box is open or closed

    private Queue<string> inputStream = new Queue<string>(); // stores dialogue
    private PlayerAnimController animController;

    private void Start()
    {
        CanvasBox.SetActive(false); // close the dialogue box on play
    }

    private void DisablePlayerController()
    {
        animController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAnimController>();
        animController.ForceIdle();
        animController.enabled = false;
    }

    private void EnablePlayerController()
    {
        animController.enabled = true;
    }

    public void StartDialogue(Queue<string> dialogue)
    {
        if (freezePlayerOnDialogue)
        {
            DisablePlayerController();
        }

        CanvasBox.SetActive(true); // open the dialogue box
        // isOpen = true;
        inputStream = dialogue; // store the dialogue from dialogue trigger
        PrintDialogue(); // Prints out the first line of dialogue
    }

    public void AdvanceDialogue() // call when a player presses a button in Dialogue Trigger
    {
        PrintDialogue();
    }

    private void PrintDialogue()
    {
        if (inputStream.Peek().Contains("EndQueue")) // special phrase to stop dialogue
        {
            inputStream.Dequeue(); // Clear Queue
            EndDialogue();
        }
		else if (inputStream.Peek().Contains("EndBranch")) 
		{
			enteredY = false;
			enteredN = false;
			inputStream.Dequeue();
			PrintDialogue();
		}
        else if (inputStream.Peek().Contains("[NAME="))
        {
            string name = inputStream.Peek();
            name = inputStream.Dequeue().Substring(name.IndexOf('=') + 1, name.IndexOf(']') - (name.IndexOf('=') + 1));
            NameText.text = name;
            PrintDialogue(); // print the rest of this line
        }
		else if (inputStream.Peek().Contains("{Y}") && Input.GetKey(KeyCode.C)) //Prints branch of dialogue entered by pressing C. MUST have {Y} first in dialogue document.
		{ 
			enteredY = true;
			string text = inputStream.Peek();
			text = inputStream.Dequeue().Substring(text.IndexOf('}')+1);
			TextBox.text = text;
		}
		else if (inputStream.Peek().Contains("{Y}") && enteredY) //continues Y branch
		{
			string text = inputStream.Peek();
			text = inputStream.Dequeue().Substring(text.IndexOf('}')+1);
			TextBox.text = text;
		}
		else if (inputStream.Peek().Contains("{N}") && Input.GetKey(KeyCode.X) && !enteredY) //Prints branch of dialogue entered by pressing X, only if have not already picked {Y}
		{ 
			enteredN = true;
			string text = inputStream.Peek();
			text = inputStream.Dequeue().Substring(text.IndexOf('}')+1);
			TextBox.text = text;
		}
		else if (inputStream.Peek().Contains("{N}") && enteredN && !enteredY) //continues N branch 
		{
			string text = inputStream.Peek();
			text = inputStream.Dequeue().Substring(text.IndexOf('}')+1);
			TextBox.text = text;
		}
		
		else if (inputStream.Peek().Contains("{")) { //catches any stray N branch element and removes it before it gets to default
			inputStream.Dequeue();
			PrintDialogue();
		}
        else
        {
            TextBox.text = inputStream.Dequeue();
        }
    }

    public void EndDialogue()
    {
        TextBox.text = "";
        NameText.text = "";
        inputStream.Clear();
        CanvasBox.SetActive(false);
        // isOpen = false;
        if (freezePlayerOnDialogue)
        {
            EnablePlayerController();
        }
    }

}