using System;

namespace ProyectoFinal_ISII.Exceptions;

public class ElementNotFoundException: Exception
{
    public ElementNotFoundException(string message): base(message) {}
    public ElementNotFoundException(string message, Exception innerException): base(message, innerException) {}
}