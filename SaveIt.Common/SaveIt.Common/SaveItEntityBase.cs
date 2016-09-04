using System;
using Newtonsoft.Json;

namespace SaveIt.Common
{
    public abstract class SaveItEntityBase
    {
        public SaveItEntityBase()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid SaveItUserId { get; set; }

        [JsonIgnore]
        public SaveItUser SaveItUser { get; set; }
    }
}