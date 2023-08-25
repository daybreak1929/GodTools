using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GodTools.Code
{
    internal static class MyLocalizedTextManager
    {
        /// <summary>
        ///     存储额外加载的所有语言的key和text
        ///     能写多少，顶多多用几MB的内存，全存了
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, string>> languages_key_text = new();

        private static readonly HashSet<string> loaded_path = new();

        public static void init()
        {
            load_json(Path.Combine(Mod.Info.Path, "Locales/cz.json"), "cz");
            load_json(Path.Combine(Mod.Info.Path, "Locales/tc.json"), "tc");
            load_json(Path.Combine(Mod.Info.Path, "Locales/en.json"), "en");
        }

        private static void load_json(string path, string language)
        {
            if (loaded_path.Contains(path)) return;
            string json = File.ReadAllText(path);
            Dictionary<string, string> key_text = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (!languages_key_text.ContainsKey(language)) languages_key_text[language] = key_text;
            else
            {
                foreach (KeyValuePair<string, string> key_text_pair in key_text)
                {
                    languages_key_text[language][key_text_pair.Key] = key_text_pair.Value;
                }
            }

            loaded_path.Add(path);
        }

        public static void apply_localization(Dictionary<string, string> target_dict, string language)
        {
            if (language != "cz" && language != "tc" && language != "en")
            {
                load_json(Path.Combine(Mod.Info.Path, "GameResources/cw_locales/en.json"), language);
            }

            Dictionary<string, string> key_text = languages_key_text[language];
            foreach (KeyValuePair<string, string> key_text_pair in key_text)
            {
                target_dict[key_text_pair.Key] = key_text_pair.Value;
            }
        }
    }
}
