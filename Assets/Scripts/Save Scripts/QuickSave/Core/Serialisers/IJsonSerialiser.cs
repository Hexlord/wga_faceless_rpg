////////////////////////////////////////////////////////////////////////////////
//  
// @module Quick Save for Unity3D 
// @author Michael Clayton
// @support clayton.inds+support@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using System;

namespace CI.QuickSave.Core.Serialisers
{
    public interface IJsonSerialiser
    {
        string Serialise<T>(T value);
        T Deserialise<T>(string json);

        string Serialize(object value, Type type);
        object Deserialise(string json, Type type);
    }
}