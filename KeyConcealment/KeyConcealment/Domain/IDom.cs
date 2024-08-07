using System;

namespace KeyConcealment.Domain;

public interface IDom<ID>
{
    ID Id {get; set;}
    /// <summary>
    /// Checks if the instance has the minimum pieces of information required to a domain 
    /// class to be meaningful
    /// </summary>
    /// <returns> Returns <c>true</c> if the object has the minimum pieces of information required, <c>false</c> otherwise</returns>
    bool IsComplete();
}
