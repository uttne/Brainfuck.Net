using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Brainfuck.Net.ArgsAnalysis
{
    public interface IArgsData<TOption>
    {
        TOption Option { get; }
        bool Has<T>(Expression<Func<TOption, T>> selectedProperty);
    }
    
    public class ArgsParser<TOption>
    where TOption : new()
    {
        #region Inner class
        
        private class ArgsData:IArgsData<TOption>
        {
            private readonly HashSet<PropertyInfo> _propertyInfoSet;

            public ArgsData(TOption option,HashSet<PropertyInfo> propertyInfoSet)
            {
                if (option == null)
                    throw new ArgumentNullException(nameof(option));
            
                Option = option;
            
                _propertyInfoSet = propertyInfoSet;
            }
        
            public TOption Option { get; }

            public bool Has<T>(Expression<Func<TOption, T>> selectedProperty)
            {
                var selectedPropertyInfo = (selectedProperty.Body as MemberExpression)?.Member as PropertyInfo;
            
                if(selectedProperty == null)
                    throw new InvalidOperationException($"{(selectedProperty.Body as MemberExpression)?.Member.Name} is not property.");

                if (_propertyInfoSet == null)
                    return false;
                return _propertyInfoSet.Contains(selectedPropertyInfo);
            }
        }
        
        private class Option
        {
            private Option(PropertyInfo propertyInfo,string optionName,string shortOptionName)
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

                OptionHashKey = OptionName.ToLower();
                ShortOptionHashKey = ShortOptionName;

                IsRequiredValue = propertyInfo.PropertyType == typeof(bool);
            }

            public static Option Create(PropertyInfo propertyInfo,OptionAttribute optionAttribute)
            {
                var regex = new Regex(@"^((([a-zA-Z])[|]([a-zA-Z\-]+))|([a-zA-Z])|([a-zA-Z\-]+))$",RegexOptions.Compiled);
                

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
                return new Option(propertyInfo,optionName,shortOptionName)
                {
                    Description = description,
                    IsDefault = optionAttribute.IsDefault,
                };
            }
            
            public PropertyInfo PropertyInfo { get; }
            public string OptionName { get; }
            public string ShortOptionName { get; }
            public string Description { get; private set; }
            public bool IsDefault { get; private set; }
            
            public bool IsRequiredValue { get; }

            public void Set(ref TOption target,string valueText)
            {
                var propertyType = PropertyInfo.PropertyType;

                try
                {
                    var value = Convert(propertyType, valueText);
                    PropertyInfo.SetValue(target, value);
                }
                catch (ArgumentException ex)
                {
                    throw new AnalysisException(PropertyInfo, ex.Message, ex);
                }
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

            private object Convert(Type type,string valueText)
            {
                object value;

                var parseMethodInfo = type.GetMethod(
                    "Parse",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    new[]{typeof(string)},
                    null
                    );
                
                if(type == typeof(string))
                {
                    value = valueText ?? throw new ArgumentException($"Value is not found.");
                }
                else if (type == typeof(bool))
                {
                    value = true;
                }
                else if(type.IsEnum)
                {
                    try
                    {
                        value = Enum.Parse(type, valueText ?? throw new ArgumentException($"Value is not found."));
                    }
                    catch (ArgumentException ex)
                    {
                        throw new ArgumentException($"'{valueText}' is not a suitable value.", ex);
                    }
                    catch (OverflowException ex)
                    {
                        throw new ArgumentException($"'{valueText}' is outside the range.", ex);
                    }
                }
                else if(parseMethodInfo != null)
                {
                    value = parseMethodInfo.Invoke(null, new object[] {valueText});
                }
                else
                {
                    var constructorInfo = type.GetConstructor(new[] {typeof(string)});

                    if (constructorInfo == null)
                    {
                        throw new InvalidOperationException($"'{type.FullName}' has no constructor with an argument of type string.");
                    }
                    
                    value = constructorInfo.Invoke(new object[] {valueText});
                }

                return value;
            }
            
            public string ShortOptionHashKey { get; private set; }
            public string OptionHashKey { get; private set; }
        }

        private class Parameters
        {
            public static Parameters Create(PropertyInfo propertyInfo,ParametersAttribute parameters)
            {
                return new Parameters();
            }
        }
        
        #endregion
        
        
        private Parameters[] _parameters;
        private readonly Dictionary<string,Option> _optionDic = new Dictionary<string,Option>();
        private readonly Dictionary<string,Option> _shortOptionDic = new Dictionary<string,Option>();
        private readonly Option _defaultOption;


        public ArgsParser()
        {
            var propertyInfos = typeof(TOption).GetProperties();

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
                .Where(x=>x.ignore == null)
                .Select(x =>
                {
                    var option = x.option != null ? Option.Create(x.propertyInfo, x.option) : null;
                    var parameters = x.parameters != null ? Parameters.Create(x.propertyInfo, x.parameters) : null;

                    return new {option, parameters};
                })
                .ToArray();

            // Eliminate duplicate options.
            foreach (var option in options.Where(x=>x.option != null).Select(x=>x.option))
            {
                if (option.IsDefault)
                    _defaultOption = _defaultOption == null ? option : throw new InvalidOperationException();
                _optionDic.Add(option.OptionHashKey, option);
                _shortOptionDic.Add(option.ShortOptionHashKey, option);
            }
            
            _parameters = options.Where(x => x.parameters != null).Select(x=>x.parameters).ToArray();
        }

        private static bool IsOption(string arg,out string optionText)
        {
            optionText = null;

            var regex = new Regex("--(.*)", RegexOptions.Compiled);

            var match = regex.Match(arg);

            if (match.Success)
                optionText = match.Groups[1].Value.ToLower();

            return optionText != null;
        }
        
        private static bool IsShortOption(string arg,out string[] optionTexts)
        {
            optionTexts = null;

            var regex = new Regex("-(.*)", RegexOptions.Compiled);

            var match = regex.Match(arg);

            if (match.Success)
                optionTexts = match.Groups[1].Value.Select(x => x.ToString()).ToArray();

            return optionTexts != null && optionTexts.Length != 0;
        }
        
        public IArgsData<TOption> Parse(string[] args)
        {
            var ret = new TOption();
            var propertyInfoSet = new HashSet<PropertyInfo>();

            var defaultOption = _defaultOption;

            for (var i = 0; i < args.Length; i++)
            {
                string GetValueText()
                {
                    var index = i + 1;
                    if (0 <= index && index < args.Length)
                        return args[index];
                    return null;
                }
                
                var s = args[i];
                
                if (IsOption(s, out var optionText))
                {
                    if (_optionDic.TryGetValue(optionText, out var option) == false)
                    {
                        throw new ArgumentException($"Option '{s}' can not be specified.");
                    }

                    propertyInfoSet.Add(option.PropertyInfo);

                    var valueText = GetValueText();
                    option.Set(ref ret,valueText);
                    
                    if(option.IsRequiredValue == false)
                        i++;
                }
                else if (IsShortOption(s, out var optionTexts))
                {
                    foreach (var text in optionTexts)
                    {
                        if (_shortOptionDic.TryGetValue(text, out var option) == false)
                        {
                            throw new ArgumentException($"Option '{text}' can not be specified.");
                        }

                        propertyInfoSet.Add(option.PropertyInfo);
                        
                        var valueText = GetValueText();
                        option.Set(ref ret,valueText);
                    
                        if(option.IsRequiredValue == false)
                            i++;
                    }
                }
                else
                {
                    if (defaultOption == null) 
                        continue;
                    
                    defaultOption.Set(ref ret,s);

                    defaultOption = null;
                }
            }

            
            return new ArgsData(ret,propertyInfoSet); ;
        }

        public string GetHelpText(object option)
        {
            throw new NotImplementedException();
        }
    }

    #region Attribute
    
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : Attribute
    {
        public string Option { get; }
        public string Description { get; }
        public bool IsDefault { get; }

        public OptionAttribute(string option,string description,bool isDefault = false)
        {
            Option = option;
            Description = description;
            IsDefault = isDefault;
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

    #endregion

    #region Exception

    public class AnalysisException:Exception
    {
        public AnalysisException(PropertyInfo analysisErrorPropertyInfo,string message,Exception innerException):base(message,innerException)
        {
            AnalysisErrorPropertyInfo = analysisErrorPropertyInfo;
        }
        public AnalysisException(PropertyInfo analysisErrorPropertyInfo,string message):base(message)
        {
            AnalysisErrorPropertyInfo = analysisErrorPropertyInfo;
        }
        public AnalysisException(PropertyInfo analysisErrorPropertyInfo)
        {
            AnalysisErrorPropertyInfo = analysisErrorPropertyInfo;
        }
        
        public PropertyInfo AnalysisErrorPropertyInfo { get; }
    }

    #endregion
}