# challenges

## Elevator Algorithm Challenge

    You are hired by an elevator manufacturer as a software developer.
    Your first task is to improve the existing elevator management algorithm to achieve the following goals:
    
        1. Decrease the average amount of time people has to wait for the elevator.
        2. Decrease the average amount of extra time people has to spend to reach their target floors.
        3. Decrease the total energy expenditure of the elevator.
        
    You have been given the following facts:
    
        1. The company bids only for projects for buildings with at most 12 floors
        2. The load capacity of the elevators range between 150 and 500 Kilograms
        3. The floor numbers for a building with n floors is from 0 (ground floor) to n-1 (top floor)
        4. "Up" and "Down" call buttons are installed on each floor (instead of a single call button)
          so that the algorithm can make a more efficient decision. The ground floor has only the 
          "Up" button installed. Likewise, the top floor has only the "Down" button installed.
        5. For safety reasons, any unexpected behavior of the elevator causes the whole system to halt, some of which are:
           5.a: An unlisted action is issued by the elevator
           5.b: An invalid action for the current state is issued by the elevator
              5.b.1: Going up while on the top floor
              5.b.2: Going down while on the ground floor
              5.b.3: Opening the door while it is already open
              5.b.4: Closing the door while it is already closed
              5.b.5: Changing direction when moving (without stopping first)
              5.b.6: Trying to move while the door is open
              5.b.7: There might be other cases too. These can bee seen by examining the runtime source code.S
           5.c: A compulsory action is not issued by the elevator
              5.c.1: Not stopping on the top floor while going up
              5.c.2: Not stopping on the ground floor while going down
        6. When the algorithm routine is initiated, the elevator is at the ground floor and its door is closed.
              
    The energy expenditure for each action is as follows:
        Opening the door: 1 units of energy
        Closing the door: 1 units of energy
        Travelling between two floors: 5 units of energy

    The performance of your algorithm will be calculated based on the following key performance indicators:
        
        1. Average Elevator Wait Time: The average number of cycles people had to wait for the elevator to take them
        2. Average Extended Travel Time On Board: The average number of extra cycles people spent in the elevator to reach their destination floor,
           which is calculated as: Total Cycles in Elevator - ABS(Target Floor - Start Floor)
        3. Energy Expenditure: The total energy expenditure of the elevator (for which the quantities are detailed above)
    
    There are some complaints reported by residents living in buildings fitted with our elevators. These must certainly be fixed:

        1. "The elevator just passed me by! It went on like I was not there!
           But it was going down and I was also waiting to go down! Ühü!"
           Emre Kantar - 30 - Businessman (Generally busy or else in a meeting)

        2. "I keep a book with me at all times so that I can read a significant amount of pages while waiting for your elevator.
           It changes direction like a roller coaster!"
           Muhammet Emin Tekin - 30 - Composer

        3. "We three got on at 7th floor and pressed buttons for floors 3, 2 and 1.
           The elevator went to the first floor first. So.. First things first? Is that it?"
           Berkant Gokdemir - 30 - Football Player - Manchester United FC
           
        4. "I rather use the stairs. It's good for my health."
           Ozan Yarci - 34 - Linguist
           
        5. "Let me tell you this: I will not pay the next electricity bill. You will!"
           Damla Firat - 25 - Landlord
           
        6. "Someone complained about waiting too much for the elevator to come.
           I was also thinking the same thing." 
           Ali Kaan Koc - 30 - ThinkTank
    
    You will find the current algorithm in the DefaultAlgorithm.cs class.
    
    The current algorithm processes calls for the elevator in sequential order,
    which is inefficient in terms of energy usage and is also frustrating for
    the people using the elevator during crowded hours.
    
    Luckily, the quantity and quality of in-line comments in the code is not bad.
    
    Good luck.
    
    
    NOTES:
    
    The OzgulOguz.ChallengeRunner executable runs in three modes:
    
        1. --run: Competition mode. Your algorithm is executed for 1M cycles and the results are reported

                                                ORIGINAL ALG.    YOUR ALGORITHM   IMPROVEMENT
                  ----------------------------  ---------------  ---------------  -----------
                  Floors Travelled            :          442307         312556     129751
                  Door Open/Close             :          313017         293469      19548
                  People Served               :           99906          99906          0
                  Avg Wait Time               :         21,2776        13,4644     7,8132
                  Avg Extended Travel Time    :          6,8475         0,4332     6,4143
                  Energy Expenditure          :         2524552        1968843     555709
        
        
        2. --rnd: Random mode (With UI support). You can observe your algorithm dealing with
                  randomly appearing people calling for a lift

        3. --ui: UI Mode (Default mode, with UI support). You can create people on any floor going to any floor by;
            3.a. Press and hold the left mouse button on the start floor,
            3.b. Move the cursor the the target floor
            3.c. Release the button
            3.d. You can Pause the progress at any time by pressing the right mouse button
            3.e. You can continue the progress by pressing the right mouse button again
    
    
    IO Scheme:
    
        The communication between the algorithm and the elevator machine is cycle based,
        that is, all events occured during one cycle are read from the input and the action
        for that cycle is written back to the output.
        
        Each of the following actions take one cycle to complete:
           * Opening the door
           * Closing the door
           * Travelling one floor
    
        EVENT MESSAGE FORMAT:
        
        The message format consists of multiple events seperated by a whitespace (" ")
        Each event consists of a two character event type and an integer seperated by a colon (":")
        
        EXAMPLE EVENT MESSAGE:
        
        "D+:5 U+:0 FL:3"
           D+:5 -> Down button has been activated on floor 5
           U+:0 -> Up button has been activated on floor 0
           FL:3 -> The elevator has reached floor 3
                    
        EVENTS:
        
           L#:N -> Recevied as part of the initialization event, specifies the capacity of the elevator
           
           F#:N -> Recevied as part of the initialization event, specifies the total number of floors
         
           U+:N -> The Up button on floor N is activated
                 (Happens when someone presses the up button on floor N while it is inactive)
           U-:N -> The Up button on floor N is deactivated
                 (Happens automatically when the elevator is on floor N, is going up (S+) and opens its door (D+))
           D+:N -> The Down button on floor N is activated
                 (Happens when someone presses the down button on floor N while it is inactive)
           D-:N -> The Down button on floor N is deactivated
                 (Happens automatically when the elevator is on floor N, is going down (S-) and opens its door (D+))
           G+:N -> The button to command the elevator to go to floor N has been activated
                 (These are the numbered buttons inside the elevator)
           G-:N -> The button to command the elevator to go to floor N has been deactivated
                 (Happens automatically when the elevator reaches floor N and opens its door (D+))
           L+:N -> Change of load. The elevator load has increased by N Kilograms
                 (Someone's got on)
           L-:N -> Change of load. The elevator load has decreased by N Kilograms
                 (Someone's got off)
           FL:N -> The elevator has reached floor N
                 (Happens while the elevator is moving and reaches a floor. (S+), (S-) and (S.) actions can be issued as a response to this event)
           EX:0 -> Shut down
                 (Stop execution immediately and exit routine)
           --:0 -> No events
                 (No events occured during this cycle)
           
        ACTIONS:
        
           ST -> Stop
           G+ -> Go up
           G- -> Go down
           O+ -> Open door, elevator is going up
           O- -> Open door, elevator is going down
           O. -> Open door, elevator stays here
           CL -> Close door
           -- -> No action
        
        SAMPLE EXECUTION OF THE ALGORITHM
        
          Sensory Input           Algorithm Output            Explanation
          
              F#:12 L#:300            --                  Initialization. Total number of floors: 12, Load capacity: 300 Kilograms  
                                                          The algorithm has started working and knows that
                                                          the elevator is on the ground floor and its door is closed
                                                        
              --:0                    --                  No events, no actions. 
                                                        
              --:0                    --                  No events, no actions. 
                                                        
              D-:5                    G+                  Someone has pressed the "Down" call button on floor 5.
                                                          Algorithm responds with a G+ action (Go up)
                                                        
              FL:1                    --                  Floor 1 has been reached  
              
              FL:2                    --                  Floor 2 has been reached
              
              FL:3 D+:2               --                  Floor 3 has been reached
                                                          Also, someone has pressed the "Down" call button on floor 2.
                                                        
              FL:4                    --                  Floor 4 has been reached
              
              FL:5                    ST                  The elevator has reached floor 5.
                                                          Algorithm responds with ST (Stop)
                                                        
              --:0                    O-                  Algorithm opens the door, elevator is going down
                                                       
              D-:5 L+:89 L+:75        CL                  The "Down" call button on floor 5 has been automatically deactivated
                                                          Elevator load has been increased by 89 Kilograms and then by 75 Kilograms
                                                          Which probably means that 2 people got onboard
                                                          Algorithm closes the door
                                                        
              G+:0                    G-                  Floor 0 button is activated by the people onboard
                                                          Algorithm responds with a G- action (Go down)
                                                        
              FL:4                    --                  Floor 4 has been reached
              
              FL:3                    --                  Floor 3 has been reached
              
              FL:2                    --                  Floor 2 has been reached
              
              FL:1                    --                  Floor 1 has been reached
              
              FL:0                    ST                  Floor 0 has been reached
                                                          Algorithm responds with ST (Stop)
                                                        
              --:0                    O+                  Algorithm opens the door, elevator is going up
                                                        
              G-:0 L-:89 L-:75        CL                  Floor 0 button is deactivated automatically
                                                          Elevator load has been decreased by 89 Kilograms and then by 75 Kilograms
                                                          Algorithm closes the door
                                                        
              --:0                    G+                  Algorithm goes up (to serve the person on the 2nd floor)
    
              ....                    ..                  ........................................................                                          
              