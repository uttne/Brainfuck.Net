using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Brainfuck.Net.ArgParsers
{
    public class ArgsParser<T>
    where T : new()
    {
        private Option[] _options;
        private Parameters[] _parameters;

        public class Option
        {
            public Option(PropertyInfo propertyInfo,string optionName,string shortOptionName,string description)
            {
                if(propertyInfo == null)
                    throw new ArgumentNullException();
                if(propertyInfo.CanWrite == false)
                    throw new ArgumentException();
                
                PropertyInfo = propertyInfo;
                OptionName = optionName ?? ConvertOptionName(propertyInfo.Name);
                
                if(shortOptionName != null && shortOptionName.Length != 1)
                    throw new ArgumentException();
                
                ShortOptionName = shortOptionName;
                Description = description;
            }

            public static Option Create(PropertyInfo propertyInfo,OptionAttribute optionAttribute)
            {
                var regex = new Regex("^((([a-zA-Z])[|]([a-zA-Z[-]]+))|([a-zA-Z])|([a-zA-Z[-]]+))$",RegexOptions.Compiled);

                var match = regex.Match(optionAttribute.Option);
                
                if(match.Success == false)
                    throw new InvalidOperationException("The format of the option is invalid.");

                var optionName =
                    match.Groups[4].Success ? match.Groups[4].Value :
                    match.Groups[6].Success ? match.Groups[6].Value : null;
                
                var shortOptionName =
                    match.Groups[3].Success ? match.Groups[3].Value :
                    match.Groups[5].Success ? match.Groups[5].Value : null;
                
                var description = optionAttribute.Description;
                return new Option(propertyInfo,optionName,shortOptionName,description);
            }
            
            public PropertyInfo PropertyInfo { get; }
            public string OptionName { get; }
            public string ShortOptionName { get; }
            public string Description { get; }

            public void Set(ref T target,string optionText)
            {
                var propertyType = PropertyInfo.PropertyType;

                var value = Convert(propertyType, optionText);
                
                PropertyInfo.SetValue(target, value);
            }

            public static string ConvertOptionName(string propertyName)
            {
                var sb = new StringBuilder();

                foreach (var c in propertyName)
                {
                    if ('A' <= c && c <= 'Z')
                    {
                        sb.Append('-');
                        sb.Append(c - 'A' + 'a');
                        continue;
                    }

                    sb.Append(c);
                }

                return sb.ToString();
            }
            
            public static object Convert(Type type,string text)
            {
                object value;

                var parseMethodInfo = type.GetMethod(
                    "Parse",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    new Type[]{typeof(string)},
                    null
                    );
                
                if(type == typeof(string))
                {
                    value = text;
                }
                else if (type == typeof(bool))
                {
                    value = true;
                }
                else if(type.IsEnum)
                {
                    value = Enum.Parse(type, text);
                }
                else if(parseMethodInfo != null)
                {
                    value = parseMethodInfo.Invoke(null, new object[] {text});
                }
                else
                {
                    var constructorInfo = type.GetConstructor(new[] {typeof(string)});

                    if (constructorInfo == null)
                    {
                        throw new InvalidOperationException($"'{type.FullName}' has no constructor with an argument of type string.");
                    }
                    
                    value = constructorInfo.Invoke(new object[] {text});
                }

                return value;
            }
        }

        public class Parameters
        {
            public static Parameters Create(PropertyInfo propertyInfo,ParametersAttribute parameters)
            {
                return new Parameters();
            }
        }
        
        public ArgsParser()
        {
            var propertyInfos = typeof(T).GetProperties();

            var options = propertyInfos
                .Where(x=>x.CanWrite)
                .Select(propertyInfo => new {propertyInfo, attributs = Attribute.GetCustomAttributes(propertyInfo)})
                .Select(x => new
                {
                    x.propertyInfo,
                    option = x.attributs.OfType<OptionAttribute>().FirstOrDefault(),
                    parameters = x.attributs.OfType<ParametersAttribute>().FirstOrDefault(),
                    ignore = x.attributs.OfType<IgnoreAttribute>().FirstOrDefault(),
                })
                .Where(x=>x.ignore != null)
                .Select(x =>
                {
                    var option = x.option != null ? Option.Create(x.propertyInfo, x.option) : null;
                    var parameters = x.parameters != null ? Parameters.Create(x.propertyInfo, x.parameters) : null;

                    return new {option, parameters};
                })
                .ToArray();

            _options = options.Where(x => x.option != null).Select(x=>x.option).ToArray();
            _parameters = options.Where(x => x.parameters != null).Select(x=>x.parameters).ToArray();
        }

        public static bool TryOption(string arg,out string optionText)
        {
            optionText = null;

            var regex = new Regex("--(.*)", RegexOptions.Compiled);

            var match = regex.Match(arg);

            if (match.Success)
                optionText = match.Groups[1].Value.ToLower();

            return optionText != null;
        }
        
        public static bool TryShortOption(string arg,out string[] optionTexts)
        {
            optionTexts = null;

            var regex = new Regex("-(.*)", RegexOptions.Compiled);

            var match = regex.Match(arg);

            if (match.Success)
                optionTexts = match.Groups[1].Value.Select(x => x.ToString()).ToArray();

            return optionTexts != null && optionTexts.Length != 0;
        }
        
        public T Parse(string[] args)
        {
            var ret = new T();

            
            
            return ret;
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
    
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
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