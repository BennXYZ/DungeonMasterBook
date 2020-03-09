using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveCampaign(Campaign campaign)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + campaign.name + ".book";
        FileStream stream = new FileStream(path, FileMode.Create);

        CampaignData data = new CampaignData(campaign);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static CampaignData LoadCampaign(string title)
    {
        string path = Application.persistentDataPath + "/" + title + ".book";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            CampaignData data = formatter.Deserialize(stream) as CampaignData;

            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("No File Found in " + path );
        }
        return null;
    }
}
