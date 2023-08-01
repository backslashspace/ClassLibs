using System;
using System.IO;
using System.Reflection;

namespace EmbedExtractor
{
    ///<summary>Extracts embeded files.</summary>
    public sealed class EmbedExtractor
    {
        ///<summary>Extracts embeded files.</summary>
        ///<exception cref="FileNotFoundException"></exception>
        public static void ExtractEmbedded(String Path, String OutFile, String Namespace, String EmbededFileName)
        {
            ValidateInput();

            //extract
            try
            {
                Stream RawFile = Assembly.GetExecutingAssembly().GetManifestResourceStream(Namespace + "." + EmbededFileName);
                FileStream FileStream = new(Path + "\\" + OutFile, FileMode.Create);

                for (Int32 i = 0; i < RawFile.Length; i++)
                {
                    FileStream.WriteByte((Byte)RawFile.ReadByte());
                }

                FileStream.Close();
                FileStream.Dispose();
            }
            catch (UnauthorizedAccessException)
            {
                var attributes = File.GetAttributes(Path + "\\" + OutFile);
                if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    attributes &= ~FileAttributes.Hidden;
                    File.SetAttributes(Path + "\\" + OutFile, attributes);
                }
                ExtractEmbedded(Path, OutFile, Namespace, EmbededFileName);
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                Directory.CreateDirectory(Path);
                ExtractEmbedded(Path, OutFile, Namespace, EmbededFileName);
            }

            void ValidateInput()
            {
                //get all embeded resources
                String[] EmbededResources = Assembly.GetExecutingAssembly().GetManifestResourceNames();

                foreach (String Item in EmbededResources)
                {
                    if (Item == Namespace + "." + EmbededFileName) return;
                }

                throw new Exception();

                throw new FileNotFoundException("Embeded resource not found:\nProvided resource: " + Namespace + "." + EmbededFileName);
            }
        }
    }
}
