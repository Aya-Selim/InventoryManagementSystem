using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Core.Models
{
    public class PasswordHashResult
    {
        public byte[] Hash { get; }

        public byte[] Salt { get; }

        public PasswordHashResult(byte[] hash, byte[] salt)
        {
            Hash = hash;
            Salt = salt;
        }
    }
}
