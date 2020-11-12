# challenges

## Elevator Algorithm Challenge

     You are hired by an elevator manufacturer as a software developer.
     Your first task is to improve the existing elevator management algorithm to achieve the following goals:
     
         1. Decrease the average amount of time people has to wait for the elevator.
         2. Decrease the total energy expenditure of the elevator.
         
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
     
     At the end of each day, a customer satisfaction index is calculated.
     xxx
     
     
     You will find the current algorithm below.
     
     The current algorithm processes calls for the elevator in sequential order,
     which is inefficient in terms of energy usage and is also frustrating for
     the people using the elevator during crowded hours.
     
     Luckily, the quantity and quality of in-line comments in the code is not bad.
     
     Good luck.
     
     
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
         
            Sensory Input             Algorithm Output               Explanation
            
                 F#:12 L#:300                --                      Initialization. Total number of floors: 12, Load capacity: 300 Kilograms  
                                                                     The algorithm has started working and knows that
                                                                     the elevator is on the ground floor and its door is closed
                                                                     
                 --:0                        --                      No events, no actions. 
                                                                     
                 --:0                        --                      No events, no actions. 
                                                                     
                 D-:5                        G+                      Someone has pressed the "Down" call button on floor 5.
                                                                     Algorithm responds with a G+ action (Go up)
                                                                     
                 FL:1                        --                      Floor 1 has been reached  
                 
                 FL:2                        --                      Floor 2 has been reached
                 
                 FL:3 D+:2                   --                      Floor 3 has been reached
                                                                     Also, someone has pressed the "Down" call button on floor 2.
                                                                     
                 FL:4                        --                      Floor 4 has been reached
                 
                 FL:5                        ST                      The elevator has reached floor 5.
                                                                     Algorithm responds with ST (Stop)
                                                                     
                 --:0                        O-                      Algorithm opens the door, elevator is going down
                                                                    
                 D-:5 L+:89 L+:75            CL                      The "Down" call button on floor 5 has been automatically deactivated
                                                                     Elevator load has been increased by 89 Kilograms and then by 75 Kilograms
                                                                     Which probably means that 2 people got onboard
                                                                     Algorithm closes the door
                                                                     
                 G+:0                        G-                      Floor 0 button is activated by the people onboard
                                                                     Algorithm responds with a G- action (Go down)
                                                                     
                 FL:4                        --                      Floor 4 has been reached
                 
                 FL:3                        --                      Floor 3 has been reached
                 
                 FL:2                        --                      Floor 2 has been reached
                 
                 FL:1                        --                      Floor 1 has been reached
                 
                 FL:0                        ST                      Floor 0 has been reached
                                                                     Algorithm responds with ST (Stop)
                                                                     
                 --:0                        O+                      Algorithm opens the door, elevator is going up
                                                                     
                 G-:0 L-:89 L-:75            CL                      Floor 0 button is deactivated automatically
                                                                     Elevator load has been decreased by 89 Kilograms and then by 75 Kilograms
                                                                     Algorithm closes the door
                                                                     
                 --:0                        G+                      Algorithm goes up (to serve the person on the 2nd floor)
     
                 ....                        ..                      ........................................................                                                    
                 