using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MyApp.Exceptions
{
    public class BadModelException : ArgumentException
    {
        public readonly ModelStateDictionary ModelState;

        public BadModelException(ModelStateDictionary state) => ModelState = state;
    }
}