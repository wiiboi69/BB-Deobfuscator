using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BB_Deob
{
    public class Mappings
    {
        public static bool Read_mapping(string path)
        {
            try
            {
                module_map = JsonConvert.DeserializeObject<Module_mapping>(File.ReadAllText(path));
                return true;
            }
            catch {}
            return false;
        }
        public static Module_mapping module_map;
        public static string Get_class_name(string name)
        {
            foreach (Class_mapping item in module_map.Class)
            {
                if (item.obfuscated_name == name)
                {
                    return item.name;
                }
            }
            return null;
        }
        public class Module_mapping
        {
            public string File_hash;
            public List<Class_mapping> Class = new List<Class_mapping>();
        }
        public class Method_mapping
        {
            public string obfuscated_name;
            public string name;
            /// <summary>
            /// the key is the obfuscated_name, the value is the name
            /// </summary>
            public Dictionary<string, string> Param;
        }
        public class Field_mapping
        {
            public string obfuscated_name;
            public string name;
        }
        public class Class_mapping
        {
            public string obfuscated_name;
            public string name;
            public List<Method_mapping> Methods = new List<Method_mapping>();
            public List<Field_mapping> Fields = new List<Field_mapping>();
            public List<Field_mapping> Properties = new List<Field_mapping>();
        }
    }
}
