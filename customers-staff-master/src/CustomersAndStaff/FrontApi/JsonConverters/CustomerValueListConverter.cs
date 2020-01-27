using System;
using System.Collections.Generic;
using System.Linq;

using Market.CustomersAndStaff.FrontApi.Dto.WorkOrders;

using Newtonsoft.Json;

namespace Market.CustomersAndStaff.FrontApi.JsonConverters
{
    public class CustomerValueListConverter : JsonConverter<CustomerValueListDto>
    {
        public override void WriteJson(JsonWriter writer, CustomerValueListDto value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;

        public override CustomerValueListDto ReadJson(JsonReader reader, Type objectType, CustomerValueListDto existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if(reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if(reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonSerializationException(nameof(CustomerValueListConverter));
            }
            reader.Read();

            if(reader.TokenType != JsonToken.PropertyName || !string.Equals((string)reader.Value, nameof(CustomerValueListDto.CustomerValueType), StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonSerializationException(nameof(CustomerValueListConverter));
            }

            reader.Read();

            var type = serializer.Deserialize<CustomerValueTypeDto>(reader);

            reader.Read();

            if(reader.TokenType != JsonToken.PropertyName || !string.Equals((string)reader.Value, nameof(CustomerValueListDto.CustomerValues), StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonSerializationException(nameof(CustomerValueListConverter));
            }

            reader.Read();

            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new JsonSerializationException(nameof(CustomerValueListConverter));
            }

            IEnumerable<BaseCustomerValueDto> values;

            switch(type)
            {
                case CustomerValueTypeDto.Vehicle:
                    values = serializer.Deserialize<VehicleCustomerValueDto[]>(reader);
                    break;
                case CustomerValueTypeDto.Appliances:
                    values = serializer.Deserialize<ApplianceCustomerValueDto[]>(reader);
                    break;
                case CustomerValueTypeDto.Other:
                    values = serializer.Deserialize<OtherCustomerValueDto[]>(reader);
                    break;
                default:
                    throw new JsonSerializationException(nameof(CustomerValueListConverter));
            }

            reader.Skip();
            reader.Read();

            return new CustomerValueListDto
                {
                    CustomerValueType = type,
                    CustomerValues = values.ToArray(),
                };
        }
    }
}