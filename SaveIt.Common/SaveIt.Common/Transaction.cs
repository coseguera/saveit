using System;

namespace SaveIt.Common
{
    public class Transaction : SaveItEntityBase
    {
        public decimal Amount { get; set; }

        public string Concept { get; set; }

        public Guid PersonId { get; set; }

        public Person Person { get; set; }

        public Guid CategoryId { get; set; }

        public Category Category { get; set; }

        public Guid AccountTransactionId { get; set; }

        public AccountTransaction AccountTransaction { get; set; }
    }
}