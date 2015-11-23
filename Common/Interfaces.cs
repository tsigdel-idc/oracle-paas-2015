using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDC.Common
{
    public interface ICryptoService
    {
        string Encrypt(string clearText);
        string Decrypt(string cipherText);
    }

}
