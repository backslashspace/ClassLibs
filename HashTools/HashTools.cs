using System;
using System.IO;
using System.Security.Cryptography;

namespace HashTools
{
    ///<summary>Functions to work with file hashes</summary>
    public sealed class FileHashTools
    {
        ///<summary>Returns the Hash-Value of a given file.</summary>
        ///<param name="FilePath">File to check</param>
        ///<param name="Algorithm">Valid arguments:<br/><br/>
        ///MD5<br/>
        ///RIPEMD160<br/>
        ///SHA1<br/>
        ///SHA256<br/>
        ///SHA384<br/>
        ///SHA512<br/>
        ///</param>
        ///<returns><see langword="null"/> when file not found</returns>
        ///<exception cref="FileLoadException"></exception>
        public static String GetFileHash(String FilePath, String Algorithm = "SHA256")
        {
            return ComputeFileHash(FilePath, Algorithm);
        }

        ///<summary>Checks if a given file has a given hash.</summary>
        ///<param name="Algorithm">Valid arguments:<br/><br/>
        ///MD5<br/>
        ///RIPEMD160<br/>
        ///SHA1<br/>
        ///SHA256<br/>
        ///SHA384<br/>
        ///SHA512<br/>
        ///</param>
        ///<param name="Hash">Hash to check against</param>
        ///<param name="FilePath">File to check</param>
        ///<exception cref="FileLoadException"></exception>
        public static Boolean CompareHash(String FilePath, String Hash, String Algorithm = "SHA256")
        {
            String FileHash = ComputeFileHash(FilePath, Algorithm) ?? throw new FileLoadException("Could not read file: " + FilePath);

            if (FileHash.Equals(Hash, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private static String ComputeFileHash(String FilePath, String Algorithm)
        {
            String ErrorMSG = "Could not read file";

            if (!File.Exists(FilePath))
            {
                return null;
            }

            return Algorithm.ToUpper() switch
            {
                "MD5" => MD5() ?? throw new Exception(ErrorMSG + FilePath),
                "SHA1" => SHA1() ?? throw new Exception(ErrorMSG + FilePath),
                "SHA256" => SHA256() ?? throw new Exception(ErrorMSG + FilePath),
                "SHA384" => SHA384() ?? throw new Exception(ErrorMSG + FilePath),
                "SHA512" => SHA512() ?? throw new Exception(ErrorMSG + FilePath),
                "RIPEMD160" => RIPEMD160() ?? throw new Exception(ErrorMSG + FilePath),
                _ => throw new Exception("invalid hash algorithm: " + Algorithm),
            };

            //##################################################################

            String MD5()
            {
                using MD5 MD5 = MD5.Create();

                String Hash;

                try
                {
                    using FileStream Stream = File.OpenRead(FilePath);

                    Hash = BitConverter.ToString(MD5.ComputeHash(Stream)).Replace("-", String.Empty);

                    Stream.Close();
                    Stream.Dispose();
                }
                catch
                {
                    Hash = null;
                }

                MD5.Dispose();

                return Hash;
            }

            String SHA1()
            {
                using SHA1 SHA1 = SHA1.Create();

                String Hash;

                try
                {
                    using FileStream Stream = File.OpenRead(FilePath);

                    Hash = BitConverter.ToString(SHA1.ComputeHash(Stream)).Replace("-", String.Empty);

                    Stream.Close();
                    Stream.Dispose();
                }
                catch
                {
                    Hash = null;
                }

                SHA1.Dispose();

                return Hash;
            }

            String SHA256()
            {
                using SHA256 SHA256 = SHA256.Create();

                String Hash;

                try
                {
                    using FileStream Stream = File.OpenRead(FilePath);

                    Hash = BitConverter.ToString(SHA256.ComputeHash(Stream)).Replace("-", String.Empty);

                    Stream.Close();
                    Stream.Dispose();
                }
                catch
                {
                    Hash = null;
                }

                SHA256.Dispose();

                return Hash;
            }

            String SHA384()
            {
                using SHA384 SHA384 = SHA384.Create();

                String Hash;

                try
                {
                    using FileStream Stream = File.OpenRead(FilePath);

                    Hash = BitConverter.ToString(SHA384.ComputeHash(Stream)).Replace("-", String.Empty);

                    Stream.Close();
                    Stream.Dispose();
                }
                catch
                {
                    Hash = null;
                }

                SHA384.Dispose();

                return Hash;
            }

            String SHA512()
            {
                using SHA512 SHA512 = SHA512.Create();

                String Hash;

                try
                {
                    using FileStream Stream = File.OpenRead(FilePath);

                    Hash = BitConverter.ToString(SHA512.ComputeHash(Stream)).Replace("-", String.Empty);

                    Stream.Close();
                    Stream.Dispose();
                }
                catch
                {
                    Hash = null;
                }

                SHA512.Dispose();

                return Hash;
            }

            String RIPEMD160()
            {
                using RIPEMD160 RIPEMD160 = RIPEMD160.Create();

                String Hash;

                try
                {
                    using FileStream Stream = File.OpenRead(FilePath);

                    Hash = BitConverter.ToString(RIPEMD160.ComputeHash(Stream)).Replace("-", String.Empty);

                    Stream.Close();
                    Stream.Dispose();
                }
                catch
                {
                    Hash = null;
                }

                RIPEMD160.Dispose();

                return Hash;
            }
        }
    }
}