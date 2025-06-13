using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagoClient
{
    public partial class ArenasClient
    {
        static partial void UpdateJsonSerializerSettings(Newtonsoft.Json.JsonSerializerSettings settings)
        {
            settings.Converters.Add(new Shared.Converter.FallbackDateTimeOffsetConverter());
        }
    }

}
