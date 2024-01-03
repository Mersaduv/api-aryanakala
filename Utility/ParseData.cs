using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ApiAryanakala.Utility;

public class ParseHelper
{
    public static List<T> ParseData<T>(string productAttributeData) where T : class
    {
        try
        {
            string jsonString = $"[{productAttributeData}]";

            T[] myObjectsArray = JsonConvert.DeserializeObject<T[]>(jsonString);
            List<T> myObjectsList = new List<T>(myObjectsArray);
            return myObjectsList;
        }
        catch (JsonException)
        {
            return new List<T>();
        }
    }

}