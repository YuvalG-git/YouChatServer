using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace YouChatServer
{
    internal class XmlDataHandler<T>
    {
        private List<T> items = new List<T>();
        private string filePath;

        public XmlDataHandler(string filePath)
        {
            this.filePath = filePath;

            // Load existing data, if any
            if (File.Exists(filePath))
            {
                LoadData();
            }
        }

        public void InsertItem(T item)
        {
            items.Add(item);
            SaveData();
        }

        public List<T> RetrieveItems()
        {
            return items;
        }

        public void ChangeItem(Func<T, bool> predicate, Action<T> modifyAction)
        {
            T itemToUpdate = items.FirstOrDefault(predicate);
            if (itemToUpdate != null)
            {
                modifyAction(itemToUpdate);
                SaveData();
            }
        }

        private void LoadData()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    items = (List<T>)serializer.Deserialize(fileStream);
                }
            }
            catch (Exception ex)
            {
                // Handle deserialization errors
                Console.WriteLine("Error loading data: " + ex.Message);
            }
        }

        private void SaveData()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    serializer.Serialize(fileStream, items);
                }
            }
            catch (Exception ex)
            {
                // Handle serialization errors
                Console.WriteLine("Error saving data: " + ex.Message);
            }
        }
    }
}
