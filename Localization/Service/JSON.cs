using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Localization.Service
{
    public class JSON
    {
        public static void CreateJSResourceFile(string basePath)
        {
            string directoryPath = Utilidades.IO.FileStorageService.LocalStorage.Combine(basePath, "Localization");
            string filePath = Utilidades.IO.FileStorageService.LocalStorage.Combine(directoryPath, "Localization.js");

            if (Utilidades.IO.FileStorageService.Storage.Exists(filePath))
                Utilidades.IO.FileStorageService.Storage.Delete(filePath);

            StringBuilder jsObjects = new StringBuilder();

            jsObjects.Append("var Localization = ");

            jsObjects.Append(GetJSONResourceObject("Localization.Resources.Gerais.*", "Localization.Resources.Consultas.*", "Localization.Resources.Enumeradores.*", "Localization.Resources.Patrimonio.*", "Localization.Resources.Configuracoes.*"));

            jsObjects.Append(";");

            Utilidades.IO.FileStorageService.Storage.WriteAllText(filePath, jsObjects.ToString());
        }

        /// <summary>
        /// Returns the JSON object of an specific resource path (ex: "Cargas.Carga", "Pedidos.Pedido").
        /// </summary>
        /// <param name="resourceNames">The resource paths.</param>
        public static string GetJSONResourceObject(params string[] resourceNames)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

            JObject jObject = new JObject();

            string[] availableResources = new string[] { };

            if (resourceNames.Any(o => o.EndsWith(".*")))
                availableResources = assembly.GetManifestResourceNames();

            foreach (string resourceName in resourceNames)
            {
                if (resourceName.EndsWith(".*"))
                {
                    foreach (string availableResource in availableResources)
                        if (availableResource.StartsWith(resourceName.Replace("*", "")))
                            SetJSONResourceObject(ref jObject, availableResource.Replace(".resources", ""), assembly);
                }
                else
                    SetJSONResourceObject(ref jObject, $"Localization.Resources.{resourceName}", assembly);
            }

            return jObject.ToString();
        }

        /// <summary>
        /// Returns an dictionary with the JSON object of an specific resource path (ex: "Cargas.Carga", "Pedidos.Pedido").
        /// </summary>
        /// <param name="resourceNames">The resource paths.</param>
        public static IDictionary<string, string> GetJSONResourceObjectDictionary(params string[] resourceNames)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

            IDictionary<string, string> resources = new Dictionary<string, string>();

            foreach (string resourceName in resourceNames)
            {
                JObject jObject = new JObject();

                SetRawJSONResourceObject(ref jObject, $"Localization.Resources.{resourceName}", assembly);

                resources.Add(resourceName, jObject.ToString());
            }

            return resources;
        }

        /// <summary>
        /// Returns the entire resource repository organized in one unique JSON object.
        /// </summary>
        //public static string GetJSONResourceObject()
        //{
        //    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

        //    JObject jObject = new JObject();

        //    string[] resourceNames = assembly.GetManifestResourceNames();

        //    foreach (string resourceName in resourceNames)
        //        SetJSONResourceObject(ref jObject, resourceName.Replace(".resources", ""), assembly);

        //    return jObject.ToString();
        //}

        private static void SetJSONResourceObject(ref JObject jObject, string resourceName, System.Reflection.Assembly assembly)
        {
            JObject currentObject = new JObject();

            SetRawJSONResourceObject(ref currentObject, resourceName, assembly);

            string[] objectNames = resourceName.Replace("Localization.", "").Split('.');

            JToken currentJToken = jObject.SelectToken(objectNames[0]);

            if (currentJToken == null)
            {
                jObject[objectNames[0]] = new JObject();
                currentJToken = jObject.SelectToken(objectNames[0]);
            }

            for (int i = 1; i < objectNames.Length; i++)
            {
                string objectName = objectNames[i];

                JToken jTokenAux = currentJToken[objectName];

                if (jTokenAux == null)
                {
                    currentJToken[objectName] = new JObject();
                    currentJToken = currentJToken[objectName];
                }
                else
                    currentJToken = jTokenAux;
            }

            currentJToken.Replace(currentObject);
        }

        private static void SetRawJSONResourceObject(ref JObject jObject, string resourceName, System.Reflection.Assembly assembly)
        {
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager(resourceName, assembly);

            System.Resources.ResourceSet resourceSet = rm.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);

            foreach (DictionaryEntry entry in resourceSet)
            {
                string key = entry.Key.ToString();
                object value = entry.Value;

                jObject.Add(new JProperty(key, value));
            }
        }
    }
}
