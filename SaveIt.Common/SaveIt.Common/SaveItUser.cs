using System;

namespace SaveIt.Common
{
    public class SaveItUser
    {
        public SaveItUser()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}