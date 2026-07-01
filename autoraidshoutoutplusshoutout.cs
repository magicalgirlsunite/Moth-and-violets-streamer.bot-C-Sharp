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
            
            // Wait 6 seconds for the Sound Alerts raid song to finish before doing anything else
            CPH.Wait(6000); 
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

            // Update the cooldown timer in Streamer.bot's memory
            CPH.SetGlobalVar("lastVipSo_" + targetLogin, DateTime.Now.ToString(), true);

            // Wait 10 seconds before shouting out
            CPH.Wait(10000); //how long to wait before shouting them out so it doesn't feel so aggressive 10000 = 10 seconds
            
            customMessage = $"Look who just dropped into chat! Check out this awesome broadcaster: https://twitch.tv/{targetLogin}"; 
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

        // 2. DROP THE CHAT MESSAGE VERSION 
        if (!string.IsNullOrEmpty(clipUrl))
        {
            customMessage += $" | Check out their most viewed clip! {clipUrl}";
        }
        CPH.SendMessage(customMessage);

        // 3. RUN THE TWITCH API SHOUTOUT
        if (targetLogin != "violetdufromage") // change this to your twitch name.
        {
            CPH.TwitchSendShoutoutByLogin(targetLogin);
        }

        // 4. THE SIMULTANEOUS CLIP & VIDEO PLAYER
        
        // Setup the Clip Player (if we successfully found a clip)
        if (!string.IsNullOrEmpty(clipId))
        {
            // Note the &muted=false at the end of the URL!
            string embedUrl = $"https://clips.twitch.tv/embed?clip={clipId}&parent=twitch.tv&autoplay=true&muted=false";
            CPH.ObsSetBrowserSource(sceneName, clipBrowserSource, embedUrl);
            
            // Turn the clip player ON
            CPH.ObsSetSourceVisibility(sceneName, clipBrowserSource, true);
        }

        // Turn the hype video ON at the exact same time
        CPH.ObsSetSourceVisibility(sceneName, raidVideoSource, true);

        // Wait for the length of your Hype Video (10000 = 10 seconds)
        CPH.Wait(10000);//change this to the length of your video 

        // Turn the hype video OFF
        CPH.ObsSetSourceVisibility(sceneName, raidVideoSource, false);

        // If we had a clip playing, let it finish its remaining time
        if (!string.IsNullOrEmpty(clipId))
        {
            // Calculate how much time the clip has left by subtracting the 10 seconds we already waited
            int remainingWaitTimeMs = (int)(clipDuration * 1000) - 10000;
            
            // Wait out the rest of the clip's natural length
            if (remainingWaitTimeMs > 0)
            {
                CPH.Wait(remainingWaitTimeMs);
            }
            
            // Clean up and hide the browser source
            CPH.ObsSetSourceVisibility(sceneName, clipBrowserSource, false);
            CPH.ObsSetBrowserSource(sceneName, clipBrowserSource, "about:blank");
        }

        return true;
    }
}
