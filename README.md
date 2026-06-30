# Moth-and-violets-streamer.bot-C-
Various C# scripts for doing stuff like auto raid messages and shoutout commands


#Setup#

1. go to https://streamer.bot/ make an account.
   The field avatar is asking for your twitch page https://www.twitch.tv/brazenbehemothh

 2.go back to https://streamer.bot/ download the latest streamerbot zip folder, extract the zip folder, and then scroll down until you see the blue icon .exe file. double click that. Now the app should be open.

 3. go to platforms on the left hand side of the menu hell we're getting into. you should see a list of streaming type platforms here.. login to twitch here. also very bottom left side login there too just for good measure.

 4. go to stream apps on the left side menu and login to obs.
    to do this you will right click in the empty rectangle and click add.
    <img width="1335" height="696" alt="image" src="https://github.com/user-attachments/assets/ec607887-bd5d-4a8b-bc67-69df3bd6efb5" />
    make the form look like this but you need your obs password from obs.

5. open obs. go to the tools tab at the top. click websocket server settings.
   <img width="665" height="651" alt="image" src="https://github.com/user-attachments/assets/5efee2ef-93af-4a7d-a238-ad5f18fdb59f" />

   whats important here is that you click show contact info to get the password and checking the box at the top that says enable websocket server. also make sure to press apply.


6. go to actions in the streamer bot app.
   in the left box right click to bring up a menu within the menu then click add to bring up another menu. lord these menus bout to intensify.
   so that box should be called add action. in here name it auto clip and press ok.
   do this again and name it autoshoutout.

7. now go to the commands menu.(its in the menu on the left side)
   right click add a new command. <img width="830" height="541" alt="image" src="https://github.com/user-attachments/assets/77567fd8-9d21-4f84-bfab-5b392eb36a30" />

   do it again for the auto shoutout
   <img width="833" height="547" alt="image" src="https://github.com/user-attachments/assets/c93dcdd7-5860-43b7-bd21-d348353a4a18" />

<img width="640" height="480" alt="image" src="https://github.com/user-attachments/assets/12c3f6e9-5b95-4867-9eff-6ed3abde65d7" />


8. meanwhile back to the actions menu.
   click on the auto clip action you made earlier.
   right click on the triggers box, go down to add-core-commands-commandtrigger. select the clip command you made. <img width="545" height="548" alt="image" src="https://github.com/user-attachments/assets/1c89251a-95a2-4060-bc68-11ced4ed5d10" />

   9.add the code to make the auto clipping command work.
   go to the sub actions box and search c# then click execute c#.
   in the new box click on the code control+A then backspace to delete that shii.
   thennn go to https://github.com/magicalgirlsunite/Moth-and-violets-streamer.bot-C-Sharp/blob/auto-clip-command/autoclipcommand

   copy paste the code into the box, press compile if you did it right it will say successful in the log. press compile and save.
   all set on the clip command 🔢

   10. I will add the rest of the instructions when I can for the autoshout with the homies that get shouted out automagically, the raiders that get automagically shouted out, the overlay video that says wow thanks for da raid or whatever video you got or it can be changed to a sound, and lastly the auto raider clip player that will play after the hype video plays or whatevss. I sleep nows :3 

   

   

   


    
