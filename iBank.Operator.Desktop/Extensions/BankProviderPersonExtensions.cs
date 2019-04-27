using iBank.Core.BankProvider;
using iBank.Operator.Desktop.Files;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace iBank.Operator.Desktop.Extensions
{
    public static class BankProviderPersonExtensions
    {
        public static IEnumerable<BankProviderPerson> GetExecutedByDate(DateTime date)
        {
            using (var client = new ConfigJsonFile().GetBankProviderClient())
                return JsonConvert.DeserializeObject<List<BankProviderPerson>>(client.GetExecutedByDate(date));
        }

        public static IEnumerable<BankProviderPerson> GetAll()
        {
            using (var client = new ConfigJsonFile().GetBankProviderClient())
                return JsonConvert.DeserializeObject<List<BankProviderPerson>>(client.GetAll());
        }
    }
}
