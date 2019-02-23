using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ISaveable
{
    SaveObject Save();
    void Load(SaveObject saveObject);
}
