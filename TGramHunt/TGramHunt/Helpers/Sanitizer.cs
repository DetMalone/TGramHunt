using Ganss.XSS;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using TGramHunt.Contract.Exceptions;

namespace TGramHunt.Helpers
{
    public class Sanitizer
    {
        private readonly string exceptionTemplate = "Bad value for {0}";

        public T Sanitize<T>(T obj, ModelStateDictionary modelState)
        {
            if (obj is null)
            {
                return obj;
            }

            var sanitizer = new HtmlSanitizer();
            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                var val = propertyInfo.GetValue(obj);
                if (val == null)
                {
                    continue;
                }

                if (propertyInfo.PropertyType == typeof(string))
                {
                    var valStr = val.ToString();
                    if (string.IsNullOrWhiteSpace(valStr))
                    {
                        continue;
                    }

                    valStr = valStr.Trim();
                    var sanitized = sanitizer.Sanitize(valStr);
                    if (string.IsNullOrWhiteSpace(sanitized))
                    {
                        modelState.TryAddModelException(propertyInfo.Name, new SanitizedException(string.Format(exceptionTemplate, propertyInfo.Name), val.ToString()));
                    }

                    propertyInfo.SetValue(obj, sanitized);
                }
                else if (propertyInfo.PropertyType == typeof(List<string>))
                {
                    var list = new List<string>(val as List<string>);

                    for (int i = 0; i < list.Count; i++)
                    {
                        var strVal = list[i].Trim();

                        if (string.IsNullOrWhiteSpace(strVal))
                        {
                            continue;
                        }

                        var sanitized = sanitizer
                            .Sanitize(strVal);
                        if (string.IsNullOrEmpty(sanitized))
                        {
                            modelState.TryAddModelException(propertyInfo.Name, new SanitizedException(string.Format(exceptionTemplate, propertyInfo.Name)));
                        }

                        list[i] = sanitized;
                    }

                    propertyInfo.SetValue(obj, list);
                }
            }

            return obj;
        }

        public string Sanitize(
            string obj,
            string key,
            ModelStateDictionary modelState)
        {
            var sanitizer = new HtmlSanitizer();
            if (string.IsNullOrWhiteSpace(obj))
            {
                return obj;
            }

            var obj2 = sanitizer.Sanitize(obj).Trim();
            if (string.IsNullOrWhiteSpace(obj2))
            {
                modelState.TryAddModelException(key, new SanitizedException(string.Format(exceptionTemplate, key), obj));
            }

            return obj2;
        }

        public string Sanitize(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
            {
                return val;
            }

            return new HtmlSanitizer()
                .Sanitize(val)
                .Trim();
        }
    }
}