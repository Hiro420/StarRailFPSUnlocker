using Microsoft.Win32;
using System.Collections.Generic;
using System;

class UnlockerRegistryHandler
{
    private readonly string parent = @"SOFTWARE\Cognosphere\Star Rail";
    private string path = "";
    private bool patched = false;
    private bool patchable = true;
    private readonly Dictionary<string, object> config = new Dictionary<string, object>
    {
        { "FPS", 120 },
        { "EnableVSync", false }, // this is just for example, not really used
        { "RenderScale", 1.4 }, // this is just for example, not really used
        { "ResolutionQuality", 5 }, // this is just for example, not really used
        { "ShadowQuality", 5 }, // this is just for example, not really used
        { "LightQuality", 5 }, // this is just for example, not really used
        { "CharacterQuality", 5 }, // this is just for example, not really used
        { "EnvDetailQuality", 5 }, // this is just for example, not really used
        { "ReflectionQuality", 5 }, // this is just for example, not really used
        { "BloomQuality", 5 }, // this is just for example, not really used
        { "AAMode", 1 } // this is just for example, not really used
    };
    private RegistryKey hKey = null;

    public void FindData()
    {
        hKey = Registry.CurrentUser.OpenSubKey(parent, true);

        int index = 0;
        string[] valueNames = hKey.GetValueNames();

        while (!valueNames[index].ToLower().Contains("graphicssettings_model"))
        {
            index++;
        }

        path = valueNames[index];
    }

    public void PatchGame()
    {
        string organised = Newtonsoft.Json.JsonConvert.SerializeObject(config).Replace("\t", "").Replace(" ", "").Replace("'", "\"").Replace("False", "false").Replace("True", "true");

        byte[] binary = System.Text.Encoding.UTF8.GetBytes(organised + "\0");

        hKey.SetValue(path, binary, RegistryValueKind.Binary);

        Console.WriteLine("--> [UNLOCKER]: SUCCESSFULLY UNLOCKED FPS\n");
        Console.WriteLine("--> [ PRESS ANY KEY TO EXIT ] ");

        if (Console.ReadKey(true) != null)
        {
            Environment.Exit(0);
        }
    }

    public void Start()
    {
        FindData();

        if (hKey == null)
        {
            patchable = false;
            return;
        }

        byte[] value = (byte[])hKey.GetValue(path);
        string decodedValue = System.Text.Encoding.UTF8.GetString(value).TrimEnd('\0');
        Dictionary<string, object> dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(decodedValue);

        foreach (string key in dictionary.Keys)
        {
            //Console.WriteLine("--> [DEBUG]: config key: "+ config[key].ToString());
            //Console.WriteLine("--> [DEBUG]: dictionary key: " + dictionary[key].ToString());
            if (key == "FPS")
            {
                continue;
            }

            config[key] = dictionary[key];
        }

        PatchGame();
    }
    static void Main(string[] args)
    {
        var unlocker = new UnlockerRegistryHandler();
        unlocker.Start();
    }
}