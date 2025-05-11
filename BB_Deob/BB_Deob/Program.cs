using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib;
using dnlib.DotNet;
using System.Text.RegularExpressions;

namespace BB_Deob
{
    class Program
    {
        static void Main(string[] args)
        {
            string mapping_path = "";
            ModuleDef module;   
            try
            {
                module = ModuleDefMD.Load(args[0]);
                mapping_path = args[1];
            }
            catch
            {
                Console.WriteLine("No arguments added!");
                return;
            }
            if (!Mappings.Read_mapping(mapping_path))
            {
                Console.WriteLine("the mapping failed to load.");
                return;
            }


            Console.WriteLine("Loaded module " + module.Name + ", Runtime: " + module.RuntimeVersion);
            string bb_version = "unknown/bb obfuscator not found";

            Console.WriteLine($"bb_mapper: Loading mapping json from {mapping_path}...");

            // Note: some BB Obfuscator Version may mest around some more stuff like enum name
            foreach (TypeDef type in module.Types)
            {
                if (type.Name.Contains("__BB_OBFUSCATOR_VERSION_"))
                {
                    bb_version = type.Name.String.Split('_')[5] + "." + type.Name.String.Split('_')[6] + "." + type.Name.String.Split('_')[7];
                }
            }
            Console.WriteLine("some BB Obfuscator Version may mest around some more stuff like enum name");
            Console.WriteLine("BB Obfuscator Version: " + bb_version);
            Console.WriteLine("bb_renamer: Renaming...");
            int count = 0;
            int classNumber = 0;
            Regex regex = new Regex(@"^[A-Z]{11}$");
            foreach (TypeDef type in module.Types)
            {
                if (!type.Name.Contains("__BB_OBFUSCATOR_VERSION_") || !type.Name.Contains("ArrayCopy"))
                {
                    if (type.Name != "<Module>")
                    {
                        string temp = null;
                        if (regex.IsMatch(type.Name))
                        {
                            temp = Mappings.Get_class_name(type.Name);
                            if (string.IsNullOrEmpty(temp))
                            {
                                type.Name = "Class" + classNumber;
                                ++classNumber;
                            }
                            else
                                type.Name = temp;
                        }
                        int fieldNumber = 0;
                        int PropertieNumber = 0;
                        foreach (FieldDef field in type.Fields)
                        {
                            if (regex.IsMatch(field.Name))
                            {
                                field.Name = "Field" + fieldNumber;
                                ++fieldNumber;
                            }
                        }
                        foreach (PropertyDef field in type.Properties)
                        {
                            if (regex.IsMatch(field.Name))
                            {
                                field.Name = "Properties" + PropertieNumber;
                                ++PropertieNumber;
                            }
                        }
                        int methodNumber = 0;
                        foreach (MethodDef method in type.Methods)
                        {
                            int paramNumber = 0;
                            foreach (ParamDef param in method.ParamDefs)
                            {
                                if (regex.IsMatch(param.Name))
                                {
                                    param.Name = "Param" + paramNumber;
                                    ++paramNumber;
                                }
                            }
                            if (regex.IsMatch(method.Name)
                                && "Initialize" != method.Name
                                && "OnDisable" != method.Name
                                && "OnDestroy" != method.Name
                                && "OnEnable" != method.Name
                                && "OnPostRender" != method.Name
                                && "Awake" != method.Name
                                && "Start" != method.Name
                                && "Update" != method.Name
                                && "FixedUpdate" != method.Name
                                && "LateUpdate" != method.Name
                                && "OnGUI" != method.Name
                                && "OnCollisionEnter" != method.Name
                                && "LateUpdate" != method.Name)
                            {
                                method.Name = "Method" + methodNumber;
                                ++methodNumber;
                            }
                        }
                    }
                }
                count++;
                Console.WriteLine("bb_renamer: Renaming [" + count + "/" + module.Types.Count + "]");
            }
            Console.WriteLine("bb_file: Saving...");
            string out_file_name = module.FullName.Split('.')[0] + "-Renamed.dll";
            module.Write(out_file_name);
            Console.WriteLine("bb_file: Saved to " + Directory.GetCurrentDirectory() + @"\" + out_file_name);
        }
    }
}
