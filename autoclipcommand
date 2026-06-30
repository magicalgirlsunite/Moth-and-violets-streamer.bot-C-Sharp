using System;
using Twitch.Common.Models.Api; // Added this to read the ClipData box!

public class CPHInline
{
    public bool Execute()
    {
        // 1. Let chat know the clip is being processed
        CPH.SendMessage("Clipping the last 30 seconds...");

        // 2. Fire the official Twitch API to create the clip (Returns a ClipData object)
        ClipData myClip = CPH.CreateClip();

        // 3. Check if Twitch successfully handed a clip back to us
        if (myClip != null && !string.IsNullOrEmpty(myClip.Url))
        {
            CPH.SendMessage($"Clip successful! Watch it here: {myClip.Url}");
        }
        else
        {
            // Twitch will fail if the channel isn't live or if it's spammed too fast
            CPH.SendMessage("Failed to create clip. Twitch might be on cooldown or the stream isn't live!");
        }

        return true;
    }
}
