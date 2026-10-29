using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Ink.Runtime;
using Ink.UnityIntegration;

public class SimpleInkManager : MonoBehaviour
{
    [Header("UI Layout Elements")]
    public TextMeshProUGUI nameTextField;
    public TextMeshProUGUI dialogueTextField;
    public RectTransform choiceButtonsContainer;

    [Header("UI Templates")]
    public Button buttonPrefabTemplate;

    private Story activeStoryLoop;
    private Dictionary<string, object> savedVariablesCache = new Dictionary<string, object>();
    private bool isDialogueSessionLive = false;

    // This kicks off the whole chat system when a player talks to an NPC
    public void LaunchStoryDialogue(InkFile rawInkAssetFile)
    {
        if (rawInkAssetFile == null) return;

        // Turn on the UI elements right away
        ToggleUIState(true);

        // Grab the JSON text and load up the actual Ink story data
        activeStoryLoop = new Story(rawInkAssetFile.storyJson);

        // Look through our saved data and inject those old variables back into the story if they match
        foreach (var savedVariable in savedVariablesCache)
        {
            if (activeStoryLoop.variablesState.GlobalVariableExistsWithName(savedVariable.Key))
            {
                activeStoryLoop.variablesState[savedVariable.Key] = savedVariable.Value;
            }
        }

        // Tell the script that the conversation is officially running
        isDialogueSessionLive = true;

        // Push the text forward to get the first sentence rolling
        AdvanceToNextSentence();
    }

    // Moves the chat forward to either the next line of text or the player choices
    public void AdvanceToNextSentence()
    {
        // Don't do anything if a conversation isn't active
        if (!isDialogueSessionLive) return;

        // Clean out any leftover choice buttons from the last question
        ClearOldChoiceButtons();

        // If the story file still has lines left to show...
        if (activeStoryLoop.canContinue)
        {
            // Grab the next line of text from Ink
            string cleanTextSentence = activeStoryLoop.Continue();

            // If the line is totally empty (like a hidden logic step), just skip it and grab the next one
            if (string.IsNullOrWhiteSpace(cleanTextSentence))
            {
                AdvanceToNextSentence();
                return;
            }

            // Put the text on the screen and make sure the text clicker button is active
            dialogueTextField.text = cleanTextSentence.Trim();
            buttonPrefabTemplate.gameObject.SetActive(true);

            // Grab the hashtags attached to this line and tell the script to look for commands
            ParseActiveDialogueLineTags(activeStoryLoop.currentTags);
        }
        // If we ran out of normal lines but there are player choices waiting...
        else if (activeStoryLoop.currentChoices.Count > 0)
        {
            // Hide the normal continue button and spawn the choice layout instead
            buttonPrefabTemplate.gameObject.SetActive(false);
            GenerateChoiceOptionButtons(activeStoryLoop.currentChoices);
        }
        // If there's no more text and no choices left, the conversation is done
        else
        {
            ShutdownDialogueUI();
        }
    }

    // Reads the hashtag tags from the dialogue line to find speaker names or visual scripting events
    private void ParseActiveDialogueLineTags(List<string> currentTagsList)
    {
        // Reset the name box to dots just in case no speaker is tagged
        nameTextField.text = "...";

        // Spin through every tag on this line
        foreach (string tag in currentTagsList)
        {
            // Look for a tag that tells us who is speaking
            if (tag.StartsWith("Speaker:"))
            {
                // Grab the name part after the colon and drop it into the UI
                nameTextField.text = tag.Replace("Speaker:", "").Trim();
            }
            // Look for a tag that triggers a game event
            else if (tag.StartsWith("trigger:"))
            {
                // Grab the event name and tell Unity Visual Scripting to run it
                string eventName = tag.Replace("trigger:", "").Trim();
                Unity.VisualScripting.CustomEvent.Trigger(gameObject, eventName);
            }
        }
    }

    // Spawns clickable UI buttons for whatever choices Ink is giving us
    private void GenerateChoiceOptionButtons(List<Choice> activeChoicesPool)
    {
        // Loop through each option in the list
        foreach (Choice choice in activeChoicesPool)
        {
            // Clone a new button using our prefab template and drop it into the UI container
            Button choiceButton = Instantiate(buttonPrefabTemplate, choiceButtonsContainer);
            choiceButton.gameObject.SetActive(true);

            // Grab the text component inside the button and change it to the choice text
            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;

            // Tell the button to listen for a click, send that choice index back to Ink, and advance the text
            choiceButton.onClick.AddListener(() => {
                activeStoryLoop.ChooseChoiceIndex(choice.index);
                AdvanceToNextSentence();
            });
        }
    }

    // Saves the data states and puts the dialogue UI away safely
    private void ShutdownDialogueUI()
    {
        // Tell the script the conversation is over
        isDialogueSessionLive = false;

        // Before we wipe the story object, grab all current variables and cache them into our dictionary
        if (activeStoryLoop != null)
        {
            foreach (string variableName in activeStoryLoop.variablesState)
            {
                savedVariablesCache[variableName] = activeStoryLoop.variablesState[variableName];
            }
        }

        // Kill the story loop instance and hide the interface components
        activeStoryLoop = null;
        ToggleUIState(false);
        gameObject.SetActive(false);
    }

    // Cleans up any cloned choice buttons left over from the last choice layout
    private void ClearOldChoiceButtons()
    {
        // Look through every object inside our buttons container
        foreach (Transform child in choiceButtonsContainer)
        {
            // Make sure we don't accidentally delete the master template button
            if (child.gameObject != buttonPrefabTemplate.gameObject)
            {
                Destroy(child.gameObject);
            }
        }
    }

    // Turns the display state of all the core UI components on or off at once
    private void ToggleUIState(bool showUI)
    {
        if (dialogueTextField != null) dialogueTextField.gameObject.SetActive(showUI);
        if (nameTextField != null) nameTextField.gameObject.SetActive(showUI);
        if (choiceButtonsContainer != null) choiceButtonsContainer.gameObject.SetActive(showUI);
        if (buttonPrefabTemplate != null) buttonPrefabTemplate.gameObject.SetActive(showUI);

        // Grab the Visual Scripting scene variable and tell it whether we are talking right now
        Unity.VisualScripting.Variables.Scene(this.gameObject.scene).Set("isTalking", showUI);
    }
}
