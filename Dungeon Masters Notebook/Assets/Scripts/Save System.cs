using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveCampaign(Campaign campaign)
    {
        string json = JsonConvert.SerializeObject(new CampaignData(campaign));
        File.WriteAllText(campaign.path, json);
    }

    public static CampaignData LoadCampaign(string path)
    {
        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            CampaignData data = JsonConvert.DeserializeObject<CampaignData>(json);
            return data;
        }
        else
        {
            Debug.LogError("No File Found in " + path );
        }
        return null;
    }
}
