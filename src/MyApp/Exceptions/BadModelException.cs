using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MyApp.Exceptions
{
    public class BadModelException : ArgumentException
    {
        public BadModelException(ModelStateDictionary state)
        {
            
        }
    }
}