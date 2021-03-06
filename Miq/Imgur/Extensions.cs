﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Miq.Imgur
{
    public static class ExtensionMethods
    {
        public static MediaTypeHeaderValue GetMediaTypeValueFromJObjecT(this JObject jObject, string propertyName)
        {
            string type = GetValueFromJObject<string>(jObject, propertyName);
            return new MediaTypeHeaderValue(type);
        }

        public static DateTime GetDateTimeValueFromJObject(this JObject jObject, string propertyName)
        {
            long timestamp = GetValueFromJObject<long>(jObject, propertyName);
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(timestamp);
        }

        public static TReturn GetValueFromJObject<TReturn>(this JObject jObject, string propertyName)
        {
            var jTitleProperty = jObject.Property(propertyName);
            TReturn valueToAssign = default(TReturn);
            JTokenType type = GetJTokenTypeFor(typeof(TReturn));
            if (jTitleProperty != null && jTitleProperty.Value.Type == type)
            {
                valueToAssign = jTitleProperty.Value.Value<TReturn>();
            }
            return valueToAssign;

        }

        private static JTokenType GetJTokenTypeFor(Type type)
        {
            switch (type.Name)
            {
                case "String": return JTokenType.String;
                case "Int64": return JTokenType.Integer;
                case "Int32": return JTokenType.Integer;
                case "Boolean": return JTokenType.Boolean;
            }

            throw new NotImplementedException(type.Name + " not implemented yet.");
        }
    }
}
