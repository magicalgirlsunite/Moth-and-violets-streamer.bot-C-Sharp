using System;

public class CPHInline
{
    public bool Execute()
    {
        string targetLogin = "";
        string viewers = "0";
        
        // 1. Check if the action was triggered by a chat command
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

        // 2. DROP THE CHAT MESSAGE FIRST!
        // This guarantees the message goes through even if the Twitch API throws a fit.
        string customMessage = isCommand 
            ? $"Check out this awesome broadcaster: https://twitch.tv/{targetLogin}" 
            : $"Thank you to @{targetLogin} for raiding with {viewers} viewers! Check them out at https://twitch.tv/{targetLogin}";
        
        CPH.SendMessage(customMessage);

        // 3. RUN THE TWITCH API SECOND!
        // We are explicitly blocking your own name so testing doesn't crash the API.
                          //change this to your twitch name!
        if (targetLogin != "violetdufromage") 
        {
            CPH.TwitchSendShoutoutByLogin(targetLogin);
        }

        return true;
    }
} 