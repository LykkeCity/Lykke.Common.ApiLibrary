﻿using System;
using System.Security.Claims;
using Newtonsoft.Json;

namespace Lykke.Common.ApiLibrary.Authentication.Introspection.Infrastructure
{
    internal class ClaimConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Claim) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var source = serializer.Deserialize<ClaimLite>(reader);
            var target = new Claim(source.Type, source.Value);

            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Claim source = (Claim)value;

            var target = new ClaimLite
            {
                Type = source.Type,
                Value = source.Value
            };

            serializer.Serialize(writer, target);
        }
    }

    internal class ClaimLite
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}