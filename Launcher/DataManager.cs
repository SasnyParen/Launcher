using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tommy;

public static class DataManager {
    static TomlTable Data;
    public static TomlTable GetTomlData()
    {
        return Data;
    }
    public static void Read()
    {
        StreamReader reader = File.OpenText("temp.toml");
        Data = TOML.Parse(reader);
    }
    public static void Save()
    {
        StreamWriter writer = File.CreateText("temp.toml");
        Data.WriteTo(writer);   
        writer.Flush();
    }
}

