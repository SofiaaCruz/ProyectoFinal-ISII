using System;

namespace ProyectoFinal_ISII.Exceptions;

public class ElementNotCreatedException: Exception
{
    public ElementNotCreatedException(string message): base(message) {}
    public ElementNotCreatedException(string message, Exception innerException): base(message, innerException) {}
}