////////////////////////////////////////////////////////////////////////////////
//  
// @module Quick Save for Unity3D 
// @author Michael Clayton
// @support clayton.inds+support@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

#if !NETFX_CORE
using JsonFx.Json;
using JsonFx.Json.Resolvers;
using JsonFx.Serialization;
using JsonFx.Serialization.Resolvers;
using System;

namespace CI.QuickSave.Core.Serialisers
{
    public class JsonSerialiserMono : IJsonSerialiser
    {
        public string Serialise<T>(T value)
        {
            CombinedResolverStrategy resolver = new CombinedResolverStrategy(new JsonResolverStrategy());

            JsonWriter writer = new JsonWriter(new DataWriterSettings(resolver));

            return writer.Write(value);
        }

        public T Deserialise<T>(string json)
        {
            CombinedResolverStrategy resolver = new CombinedResolverStrategy(new JsonResolverStrategy());

            JsonReader reader = new JsonReader(new DataReaderSettings(resolver));
            return reader.Read<T>(json);
        }

        public string Serialize(object value, Type type)
        {
            CombinedResolverStrategy resolver = new CombinedResolverStrategy(new JsonResolverStrategy());

            JsonWriter writer = new JsonWriter(new DataWriterSettings(resolver));

            return writer.Write(value);
        }

        public object Deserialise(string json, Type type)
        {
            CombinedResolverStrategy resolver = new CombinedResolverStrategy(new JsonResolverStrategy());

            JsonReader reader = new JsonReader(new DataReaderSettings(resolver));
            return reader.Read(json, type);
        }
    }
}
#endif