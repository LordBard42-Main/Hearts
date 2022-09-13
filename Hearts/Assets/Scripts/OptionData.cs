using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionData : ISerialize

{
    public void CopyFrom(ISerialize incomingClass)
    {
        throw new System.NotImplementedException();
    }

    public void DeserializeInformation(PathAndFileName pathAndFileName)
    {
        var deserialziedClass = ClassSerializer.DeserializeClass(this, pathAndFileName);
        CopyFrom(deserialziedClass); 
    }

    public void SerializeInformation(PathAndFileName pathAndFileName)
    {
        ClassSerializer.SerializeClass(this, pathAndFileName);
    }
}
