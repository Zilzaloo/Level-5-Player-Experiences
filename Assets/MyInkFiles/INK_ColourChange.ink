// === Colour Change and Accessorise === //
// 

Hello there, traveler! What can I do for you today? # Speaker: Friendly Villager
// The '+' means this choice is sticky and stays open across every conversation
+ [Just chatting.] 
    It is always nice to have company in the square! # Speaker: Friendly Villager 
    -> END

// The '*' means this choice is a fallback. It automatically cuts itself off forever once chosen!
* [Change Colour] 
    Which colour would you like your garment to be? # Speaker: Friendly Villager
    **[Red] 
    A fine Choice! # trigger: PlayerRedTunic
    -> END
    **[Blue] 
    A fine Choice! # trigger: PlayerBlueTunic
    -> END
    **[Green] 
    A fine Choice! # trigger: PlayerGreenTunic
    -> END
* [Accessorise]
    What kind of hat?
    I have two left
    **[Pointy]
    You have great taste! # trigger: PointyHat
    -> END
    **[Block]
    Each to their own. # trigger: BlockHat
    -> END
    
