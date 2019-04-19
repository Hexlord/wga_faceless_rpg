////////////////////////////////////////////////////////////////////////////////
//  
// @module Quick Save for Unity3D 
// @author Michael Clayton
// @support clayton.inds+support@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

#if NETFX_CORE
using System;
using Newtonsoft.Json;

namespace CI.QuickSave.Core.Serialisers
{
    public class JsonSerialiserUWP : IJsonSerialiser
    {
        public string Serialise<T>(T value)
        {
            Json
            return JsonConvert.SerializeObject(value);
        }

        public T Deserialise<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string Serialize(object value, Type type)
        {
            throw new NotImplementedException();
        }

        public object Deserialise(string json, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
#endif