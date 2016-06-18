using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace KioskiLaskin
{
    class InputOutput
    {
        public static async Task<T> ReadObjectFromXmlFileAsync<T>(string filename)
        {
            // this reads XML content from a file ("filename") and returns an object  from the XML
            T objectFromXml = default(T);
            var serializer = new XmlSerializer(typeof(T));
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync(filename);
            Stream stream = await file.OpenStreamForReadAsync();
            try
            {
                objectFromXml = (T)serializer.Deserialize(stream);
            }catch(Exception)
            {
                string f = filename + "_corrupted";
                /* Problem with the file. Rename corrupted file */
                await file.RenameAsync(f);
                objectFromXml = default(T);
            }
            stream.Dispose();
            return objectFromXml;
        }

        internal static async void DeleteAll(string filePrefix)
        {
            IReadOnlyList<StorageFile> files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            foreach (StorageFile file in files)
            {
                if (filePrefix == null || (file.Name.Substring(0, filePrefix.Length) == filePrefix && file.Name.Substring(file.Name.Length - 3, 3) == "xml"))
                {
                    await file.DeleteAsync();
                }
            }
        }

        internal static async void DeleteFile(string fileName)
        {
            IReadOnlyList<StorageFile> files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            foreach (StorageFile file in files)
            {
                if (file.Name == fileName)
                {
                    await file.DeleteAsync();
                    break;
                }
            }
        }

        public static async Task<List<Tapahtuma>> ReadTapahtumatFromXmlFilesAsyncToList(string filePrefix)
        {
            List<Tapahtuma> Tapahtumat = new List<Tapahtuma>();
            IReadOnlyList<StorageFile> files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            foreach(StorageFile file in files)
            {
                if(file.Name.Substring(0, filePrefix.Length) == filePrefix && file.Name.Substring(file.Name.Length - 3,3) == "xml")
                {
                    Tapahtuma t = await ReadObjectFromXmlFileAsync<Tapahtuma>(file.Name);
                    if (t != null)
                    {
                        Tapahtumat.Add(t);
                    }
                }
            }

/*
            string s = await GetNextFileName(filePrefix);
            UInt16 i;
            string ii = s.Substring(filePrefix.Length, s.Length - filePrefix.Length - 4);
            bool parsed = UInt16.TryParse(ii, out i);
            if(parsed)
            {
                for(UInt16 j=1; j<i; j++)
                {
                    string filename = filePrefix + j.ToString() + ".xml";
                    Tapahtuma t = await ReadObjectFromXmlFileAsync<Tapahtuma>(filename);
                    Tapahtumat.Add(t);
                }
            }*/
            return Tapahtumat;
        }

        public static async Task SaveObjectToXml<T>(T objectToSave, string filename)
        {
            // stores an object in XML format in file called 'filename'
            var serializer = new XmlSerializer(typeof(T));
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            Stream stream = await file.OpenStreamForWriteAsync();

            using (stream)
            {
                serializer.Serialize(stream, objectToSave);
            }
        }

        public static async Task<string> GetNextFileName(string prefix)
        {
            bool found = false;
            UInt16 i = 0;
            do
            {
                i++;
                found = await DoesFileExistAsync(prefix + i.ToString() + ".xml");
            } while (found);
            return prefix + i.ToString() + ".xml";
        }

        public static async Task<bool> DoesFileExistAsync(string fileName)
        {
            try
            {
                await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}