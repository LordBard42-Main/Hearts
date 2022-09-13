using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerialize
{
    public void DeserializeInformation(PathAndFileName pathAndFileName);
    public void SerializeInformation(PathAndFileName pathAndFileName);
    void CopyFrom(ISerialize incomingClass);
}
