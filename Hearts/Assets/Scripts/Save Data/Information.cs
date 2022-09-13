using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Information {

    private string pathToFile;

    public string PathToFile { get => pathToFile; private set => pathToFile = value; }
}
