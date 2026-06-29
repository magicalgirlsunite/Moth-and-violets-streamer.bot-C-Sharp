using System;
using System.Collections.Generic;
using Twitch.Common.Models.Api;

public class CPHInline
{
    public bool Execute()
    {
        string targetLogin = "";
        string viewers = "0";
        
        bool isCommand = args.ContainsKey("command");

        if (isCommand)
        {
            if (!args.ContainsKey("input0"))
            {
                CPH.SendMessage("You forgot the name! Use: !soo @username");
                return false; 
            }
            targetLogin = args["input0"].ToString().Replace("@", "").Trim().ToLower();
        }
        else
        {
            targetLogin = args.ContainsKey("userName") ? args["userName"].ToString() : "";
            viewers = args.ContainsKey("viewers") ? args["viewers"].ToString() : "0";
        }

        if (string.IsNullOrEmpty(targetLogin))
        {
            CPH.LogWarn("AutoShoutout: No target name found.");
            return false;
        }

        // --- OBS ASSET NAMES ---
        // Change these strings to match your exact OBS setup!
        string sceneName = "Your Scene Name"; 
        string raidVideoSource = "Shoutout Storm"; 
        string clipBrowserSource = "Clip Player"; 

        // 1. GET THEIR MOST VIEWED CLIP
        string clipUrl = "";
        string clipId = "";
        float clipDuration = 30f; // Fallback duration in case the API misses it

        try
        {
            List<ClipData> clips = CPH.GetClipsForUser(targetLogin);
            
            if (clips != null && clips.Count > 0)
            {
                clipUrl = clips[0].Url; 
                clipId = clips[0].Id; // We need the raw ID to build the embed player
                
                // Streamer.bot pulls the exact duration of the clip in seconds
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

        // 2. DROP THE CHAT MESSAGE
        string customMessage = isCommand 
            ? $"Check out this awesome broadcaster: https://twitch.tv/{targetLogin}" 
            : $"Thank you to @{targetLogin} for raiding with {viewers} viewers! Check them out at https://twitch.tv/{targetLogin}";
        
        if (!string.IsNullOrEmpty(clipUrl))
        {
            customMessage += $" | Check out their most viewed clip! {clipUrl}";
        }
        CPH.SendMessage(customMessage);

        // 3. RUN THE TWITCH API SHOUTOUT
        if (targetLogin != "violetdufromage") 
        {
            CPH.TwitchSendShoutoutByLogin(targetLogin);
        }

        // 4. PHASE ONE: THE RAID VIDEO
        // Show the video, wait for its duration, then hide it.
        CPH.ObsSetSourceVisibility(sceneName, raidVideoSource, true);
        CPH.Wait(10000); // CHANGE THIS: 10000ms = 10 seconds. Set to the exact length of your storm video!
        CPH.ObsSetSourceVisibility(sceneName, raidVideoSource, false);

        // 5. PHASE TWO: THE CLIP PLAYER
        if (!string.IsNullOrEmpty(clipId))
        {
            // Build the Twitch Embed URL. (Twitch requires the parent parameter, and autoplay forces it to start).
            string embedUrl = $"https://clips.twitch.tv/embed?clip={clipId}&parent=twitch.tv&autoplay=true";

            // Inject the URL into the OBS Browser Source
            CPH.ObsSetBrowserUrl(sceneName, clipBrowserSource, embedUrl);
            
            // Make the Browser Source visible so it starts playing
            CPH.ObsSetSourceVisibility(sceneName, clipBrowserSource, true);

            // Wait for the exact duration of the clip (convert seconds to milliseconds)
            int waitTimeMs = (int)(clipDuration * 1000);
            CPH.Wait(waitTimeMs);

            // Hide the clip and clear the URL so phantom audio doesn't keep playing
            CPH.ObsSetSourceVisibility(sceneName, clipBrowserSource, false);
            CPH.ObsSetBrowserUrl(sceneName, clipBrowserSource, "about:blank");
        }

        return true;
    }
}
