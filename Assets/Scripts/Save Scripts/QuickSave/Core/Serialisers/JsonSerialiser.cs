////////////////////////////////////////////////////////////////////////////////
//  
// @module Quick Save for Unity3D 
// @author Michael Clayton
// @support clayton.inds+support@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using CI.QuickSave.Core.Helpers;
using System;

namespace CI.QuickSave.Core.Serialisers
{
    public static class JsonSerialiser
    {
#if !NETFX_CORE
        private static IJsonSerialiser _serialiser = new JsonSerialiserMono();
#else
        private static IJsonSerialiser _serialiser = new JsonSerialiserUWP();
#endif

        public static string Serialise<T>(T value)
        {
            return _serialiser.Serialise(value);
        }

        public static T Deserialise<T>(string json)
        {
            if (TypeHelper.IsUnityType<T>())
            {
                return TypeHelper.DeserialiseUnityType<T>(json, _serialiser);
            }
            else
            {
                return _serialiser.Deserialise<T>(json);
            }
        }

        public static string Serialise(object value, Type type)
        {
            return _serialiser.Serialize(value, type);
        }

        public static object Deserialise(string json, Type type)
        {
            if (TypeHelper.IsUnityType(type))
            {
                return TypeHelper.DeserialiseUnityType(json, _serialiser, type);
            }
            else
            {
                return _serialiser.Deserialise(json, type);
            }
        }
    }
}