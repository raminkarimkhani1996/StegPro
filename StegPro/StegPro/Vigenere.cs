using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace StegPro
{
    public static class Vigenere
    {
//VIGENERE ENCRYPTION
        public static void encrypt(string fileName, string key)
        {

            int tmp, tmp1, tmp2, keyiter = 0, fileiter = 0;
            key = key.ToUpper();
            string tmptext = "";
            string[] cipherText = new string[File.ReadLines(fileName).Count()];
            foreach (string line in File.ReadLines(fileName, Encoding.UTF8))
            {
                tmptext = "";
                keyiter = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] >= 97 && line[i] <= 122)
                    {
                        tmp1 = line[i] - 97;
                        tmp2 = key[keyiter] - 65;
                        tmp = ((tmp1 + tmp2) % 26) + 97;
                        tmptext += (char)tmp;
                        keyiter++;
                        if (keyiter >= key.Length)
                        {
                            keyiter = 0;
                        }
                    }
                    else
                        if (line[i] >= 65 && line[i] <= 90)
                    {
                        tmp1 = line[i] - 65;
                        tmp2 = key[keyiter] - 65;
                        tmp = ((tmp1 + tmp2) % 26) + 65;
                        tmptext += (char)tmp;
                        keyiter++;
                        if (keyiter >= key.Length)
                        {
                            keyiter = 0;
                        }
                    }
                    else
                    {
                        tmptext += line[i];
                    }

                }
                cipherText[fileiter] = tmptext;
                fileiter++;
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\ProgramData\stegpro_encrypted.txt"))
            {
                foreach (string line in cipherText)
                {

                    file.WriteLine(line);

                }
            }
        }
//VIGENERE DECRYPTION
        public static void decrypt(byte[] bytes, string key)
        {
            int tmp, tmp1, tmp2, keyiter = 0;
            key = key.ToUpper();
            for (int i=0; i<bytes.Length; i++)
            {
                if(bytes[i]==13)
                {
                    keyiter = 0;
                }
                else
                if(bytes[i]>=97 && bytes[i] <=122)
                {
                    tmp1 = bytes[i] - 97;
                    tmp2 = key[keyiter] - 65;
                    if(tmp1 < tmp2)
                    {
                        tmp1 += 26;
                    }
                    tmp = (tmp1 - tmp2) + 97;
                    bytes[i] = (byte)tmp;
                    keyiter++;
                    if (keyiter >= key.Length)
                    {
                        keyiter = 0;
                    }
                }
                else
                if (bytes[i] >= 65 && bytes[i] <= 90)
                {
                    tmp1 = bytes[i] - 65;
                    tmp2 = key[keyiter] - 65;
                    if (tmp1 < tmp2)
                    {
                        tmp1 += 26;
                    }
                    tmp = (tmp1 - tmp2) + 65;
                    bytes[i] = (byte)tmp;
                    keyiter++;
                    if (keyiter >= key.Length)
                    {
                        keyiter = 0;
                    }
                }
                else
                {
                    continue;
                }
            }
        }
    }
}
