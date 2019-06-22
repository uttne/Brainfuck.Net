using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Brainfuck.Net.ArgParsers
{
    public class ArgsParser<T>
    {
        private PropertyInfo[] _propertyInfos;

        public ArgsParser()
        {
            _propertyInfos = typeof(T).GetProperties();

            var attributes = _propertyInfos
                .Select(propertyInfo => new {propertyInfo, attributs = Attribute.GetCustomAttributes(propertyInfo)})
                .Select(x => new
                {
                    x.propertyInfo,
                    option = x.attributs.OfType<OptionAttribute>().FirstOrDefault(),
                    parameters = x.attributs.OfType<ParametersAttribute>().FirstOrDefault(),
                })
                .ToArray();
        }
        
        public T Parse(string[] args)
        {
            throw new NotImplementedException();
        }

        public string GetHelpText()
        {
            throw new NotImplementedException();
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : Attribute
    {
        public string Option { get; }
        public string Description { get; }

        public OptionAttribute(string option,string description)
        {
            Option = option;
            Description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ParametersAttribute : Attribute
    {
        
    }

    public class Options
    {
        [Parameters]
        public IList<string> Parameters { get; set; }
        
        [Option("h|help","show help")]
        public bool Help { get; set; }
    }
}