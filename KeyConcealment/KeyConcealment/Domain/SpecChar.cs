using System;

namespace KeyConcealment.Domain;

/// <summary>
/// Class that represent a special character that can be included inside an auto generated password
/// </summary>
public class SpecChar
{
    private char _specialCharacter;
    // the boolean indicates if the character was chosen or not by the user
    private bool _chosen;

    public SpecChar(char sc, bool chosen)
    {
        this._specialCharacter = sc;
        this._chosen = chosen;
    }

    public char SpecialCharacter {get => this._specialCharacter;}
    public bool Chosen {get => this._chosen; set => this._chosen = value;}

}
