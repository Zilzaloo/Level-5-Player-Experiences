// 1. Create our single memory tracking variable
VAR wizard_completed = false

// ============================================================
// 2. THE WIZARD. (Simple English If-Statement)
// Moving this to the absolute top means we check the memory 
// BEFORE the opening line ever prints to the screen!
// ============================================================
{ wizard_completed == true:
    -> LoopDialogue
}

// ------------------------------------------------------------
// FIRST ENCOUNTER (Only plays if the wizard is NOT completed)
// ------------------------------------------------------------
Hello there, traveler! Perhaps a magic trick to boggle the mind? # Speaker: Friendly Wizard

// ============================================================
// ROUND 1: Origin Question (Only North and South buttons appear)
// ============================================================
Hello traveller. 

I am the wizard as called by Ray and I am experimental. # Speaker: Friendly Wizard

I have A slightly more complex INK script that Ray himself is still toying with. # Speaker: Friendly Wizard

If you want to push further, explore my attached INK file, but alas, question time! # Speaker: Friendly Wizard

Are you from the north or the south Traveller? # Speaker: Friendly Wizard

* [The North] 
    Excellent, a warrior of the snow # Speaker: Friendly Wizard # trigger: SnowVFX
    -> ElementRound // Step forward to Round 2

* [The South] 
    Excellent, a warrior of the sands # Speaker: Friendly Wizard # trigger: SandVFX 
    -> ElementRound // Step forward to Round 2


// ============================================================
// ROUND 2: Element Question (Only Fire and Earth buttons appear)
// ============================================================
=== ElementRound ===
Which element do you desire? # Speaker: Friendly Wizard

* [Fire] 
    Then you have chosen. Why not visit the shop and pick up some armor. # Speaker: Friendly Wizard # trigger: FireFountainVFX
    # trigger: Turned 
    ~ wizard_completed = true // Flip the switch to lock the wizard forever!
    -> END

* [Water] 
    Then you have chosen. Why not visit the shop and pick up some armor. # Speaker: Friendly Wizard
    # trigger: TurnBrown # trigger: WaterFountainVFX 
    ~ wizard_completed = true // Flip the switch to lock the wizard forever!
    -> END


// ============================================================
// ROUND 3: Repeatable Hub (Only appears instantly on future visits)
// ============================================================
=== LoopDialogue ===
// Notice: The [Talk] choice button is completely gone! 
// The text prints automatically right when you interact.
The cosmic stars have spoken, you must press on with your quest. # Speaker: Friendly Wizard 
-> END
