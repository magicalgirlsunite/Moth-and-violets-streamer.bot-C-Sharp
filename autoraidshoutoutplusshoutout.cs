using System;
using System.Collections.Generic; // Required for List<>
using Twitch.Common.Models.Api; // Required so Streamer.bot understands "ClipData"

public class CPHInline
{
    public bool Execute()
    {
        string targetLogin = "";
        string viewers = "0";
        
        // 1. Determine if this was triggered by a Chat Command or a Raid
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

        // Safety check
        if (string.IsNullOrEmpty(targetLogin))
        {
            CPH.LogWarn("AutoShoutout: No target name found.");
            return false;
        }

        // 2. GET THEIR MOST VIEWED CLIP
        string clipUrl = "";
        try
        {
            // Streamer.bot pulls clips sorted by view count descending by default
            List<ClipData> clips = CPH.GetClipsForUser(targetLogin);
            
            // If they actually have clips, grab the URL of the top one
            if (clips != null && clips.Count > 0)
            {
                clipUrl = clips[0].Url; 
            }
        }
        catch (Exception ex)
        {
            CPH.LogWarn("AutoShoutout Clip Error: " + ex.Message);
        }

        // 3. DROP THE CHAT MESSAGE FIRST
        string customMessage = isCommand 
            ? $"Check out this awesome broadcaster: https://twitch.tv/{targetLogin}" 
            : $"Thank you to @{targetLogin} for raiding with {viewers} viewers! Check them out at https://twitch.tv/{targetLogin}";
        
        // Append the clip to the string if we successfully grabbed one
        if (!string.IsNullOrEmpty(clipUrl))
        {
            customMessage += $" | Check out their most viewed clip! {clipUrl}";
        }

        CPH.SendMessage(customMessage);

        // 4. RUN THE TWITCH API SHOUTOUT
        if (targetLogin != "violetdufromage") 
        {
            CPH.TwitchSendShoutoutByLogin(targetLogin);
        }

        // 5. OBS VIDEO TRIGGER (The Raid/Storm Video)
        // Ensure these match your OBS naming exactly!
        string sceneName = "Your Scene Name"; 
        string sourceName = "Your Video Source Name"; 

        // Play the video by making it visible
        CPH.ObsSetSourceVisibility(sceneName, sourceName, true);
        
        // Stall the script for the duration of the video (e.g., 10000 = 10 seconds)
        CPH.Wait(10000); 

        // Hide the video again
        CPH.ObsSetSourceVisibility(sceneName, sourceName, false);

        return true;
    }
}
