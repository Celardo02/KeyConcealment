using System;

namespace KeyConcealment.Domain;

public interface IDom<ID>
{
    ID Id {get; set;}
}
