using System;
using System.Collections.Generic;
using Twitch.Common.Models.Api;

public class CPHInline
{
    public bool Execute()
    {
        // --- CONFIGURATION ---
        // Add the Twitch usernames (lowercase only!) of the homies you want to auto-shoutout
        List<string> vipStreamers = new List<string> { "valentinewar", "violetdufromage", "hotshot87420" };
        int cooldownHours = 3; //how often it will shout these friends out

        //don't mess with these 3 lines
        string targetLogin = "";
        string viewers = "0";
        string customMessage = "";
        
        //Determine what caused this script to run
        bool isCommand = args.ContainsKey("command");
        bool isRaid = args.ContainsKey("viewers") && !isCommand;
        bool isChatMessage = args.ContainsKey("message") && !isCommand && !isRaid;

        if (isCommand)
        {
            if (!args.ContainsKey("input0"))
            {
                CPH.SendMessage("You forgot the name! Use: !soo @username");
                return false; 
            }
            targetLogin = args["input0"].ToString().Replace("@", "").Trim().ToLower();
            customMessage = $"Check out this awesome broadcaster: https://twitch.tv/{targetLogin}";
        }
        else if (isRaid)
        {
            targetLogin = args.ContainsKey("userName") ? args["userName"].ToString().ToLower() : "";
            viewers = args.ContainsKey("viewers") ? args["viewers"].ToString() : "0";
            customMessage = $"Thank you @{targetLogin} for raiding with {viewers} viewers! Check them out at https://twitch.tv/{targetLogin}";
        }
        else if (isChatMessage)
        {
            targetLogin = args.ContainsKey("userName") ? args["userName"].ToString().ToLower() : "";

            //Fail fast if they aren't on the VIP list
            if (!vipStreamers.Contains(targetLogin))
            {
                return true; 
            }

           //Check the 3-hour cooldown
            string lastSoStr = CPH.GetGlobalVar<string>("lastVipSo_" + targetLogin, true);
            if (!string.IsNullOrEmpty(lastSoStr))
            {
                if (DateTime.TryParse(lastSoStr, out DateTime lastShoutout))
                {
                    if ((DateTime.Now - lastShoutout).TotalHours < cooldownHours)
                    {
                        return true; // Still on cooldown, exit silently
                    }
                }
            }

            // 3. Update the cooldown timer in Streamer.bot's memory
            CPH.SetGlobalVar("lastVipSo_" + targetLogin, DateTime.Now.ToString(), true);

            // 4. Wait 10 seconds before shouting out
            CPH.Wait(10000); //how long to wait before shouting them out so it doesn't feel so aggressive
            
            customMessage = $"Look who just dropped into chat! Check out this awesome broadcaster: https://twitch.tv/{targetLogin}"; //the message. you can change to whatever you want here
        }
        else
        {
            return false; 
        }

        // Final Safety Check
        if (string.IsNullOrEmpty(targetLogin))
        {
            CPH.LogWarn("AutoShoutout: No target name found.");
            return false;
        }

        // --- OBS ASSET NAMES ---
        // Change these to whatever you named your things:3
        string sceneName = "Your Scene Name"; 
        string raidVideoSource = "Shoutout Storm"; 
        string clipBrowserSource = "Clip Player"; 

        // 1. GET THEIR MOST VIEWED CLIP
        string clipUrl = "";
        string clipId = "";
        float clipDuration = 20f; // how long to play the clip before cutting it off. in seconds

        try
        {
            List<ClipData> clips = CPH.GetClipsForUser(targetLogin);
            
            if (clips != null && clips.Count > 0)
            {
                clipUrl = clips[0].Url; 
                clipId = clips[0].Id; 
                
                if (clips[0].Duration > 0)
                {
                    clipDuration = clips[0].Duration;
                }
            }
        }
        catch (Exception ex)
        {
            CPH.LogWarn("AutoShoutout Clip Error: " + ex.Message);
        }

        // 2. DROP THE CHAT MESSAGE VERSION or delete this block if you don't want it to send in chat too.
        if (!string.IsNullOrEmpty(clipUrl))
        {
            customMessage += $" | Check out their most viewed clip! {clipUrl}";
        }
        CPH.SendMessage(customMessage);
        //block ends here

        // 3. RUN THE TWITCH API SHOUTOUT
        if (targetLogin != "violetdufromage") // change this to your twitch name.
        {
            CPH.TwitchSendShoutoutByLogin(targetLogin);
        }

        
        CPH.ObsSetSourceVisibility(sceneName, raidVideoSource, true);
        //Change the line below!
        CPH.Wait(10000); // CHANGE THIS: Set to the length of your video 10000 = 10 seconds!
        //change the line above!
        CPH.ObsSetSourceVisibility(sceneName, raidVideoSource, false);

        // THE CLIP PLAYER
        if (!string.IsNullOrEmpty(clipId))
        {
            string embedUrl = $"https://clips.twitch.tv/embed?clip={clipId}&parent=twitch.tv&autoplay=true";
            CPH.ObsSetBrowserSource(sceneName, clipBrowserSource, embedUrl);
            CPH.ObsSetSourceVisibility(sceneName, clipBrowserSource, true);
            
            int waitTimeMs = (int)(clipDuration * 1000);
            CPH.Wait(waitTimeMs);
            
            CPH.ObsSetSourceVisibility(sceneName, clipBrowserSource, false);
            CPH.ObsSetBrowserSource(sceneName, clipBrowserSource, "about:blank");
        }

        return true;
    }
}
