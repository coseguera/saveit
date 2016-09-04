using System;
using System.Collections.Generic;

namespace SaveIt.Common
{
    public class AccountTransaction : SaveItEntityBase
    {
        public DateTime Date { get; set; }

        public string BankDescription { get; set; }

        public Guid AccountId { get; set; }

        public Account Account { get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}