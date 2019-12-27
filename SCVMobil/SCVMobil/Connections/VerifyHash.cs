using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace SCVMobil.Connections
{
    public class VerifyHash
    {
        int GetCounter, VerCounter;
        public VerifyHash(int HashCounter)
        {
            this.GetCounter = HashCounter;
            this.VerCounter = HashCounter;
        }

        public string GetMd5Hash(MD5 md5Hash, string input, int Counter)
        {
            try
            {
                // Crea un nuevo Stringbuilder para almacenar los bytes
                // y asi ir formando el string hash
                StringBuilder sBuilder = new StringBuilder();

                // Convertir el input string a un arreglo de byte y computar el hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Iterar a treves de cada byte de la data hasheada
                // y formatear cada uno a string en hexadecimal.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                // Retornar el string hexadecimal.
                Debug.WriteLine("Contador No.:" + Counter + "Actual Hash: " + sBuilder.ToString());
                Counter = Counter - 1;

                if (Counter >= 0)
                {
                    return GetMd5Hash(md5Hash, sBuilder.ToString(), Counter);
                }
                else
                {
                    Debug.WriteLine("Contador No.:" + Counter + "Actual Hash: " + sBuilder.ToString());
                    return sBuilder.ToString().Substring(0, 20);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Excepcion en el metodo GetHash " + ex.Message);
                return null;
            }
        }

        // Verificar un hash contra un string.
        public bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            try
            {
                // Hash de la entrada

                string hashOfInput = GetMd5Hash(md5Hash, input, VerCounter);

                // Create a StringComparer an compare the hashes.
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;

                if (0 == comparer.Compare(hashOfInput, hash))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Excepcion en el metodo VerifyHash " + ex.Message);
                return false;
            }
        }

    }

}
