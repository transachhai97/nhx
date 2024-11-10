using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoiThiDV.Model
{
    public static class SendObject
    {
        public static Messages Serialize(object anySerializableObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                (new BinaryFormatter()).Serialize(memoryStream, anySerializableObject);
                memoryStream.Position = 0;
                memoryStream.Seek(0, SeekOrigin.Begin);
                return new Messages { Data = memoryStream.ToArray() };
            }
        }

        public static object Deserialize(Messages message)
        {
            MemoryStream mm = new MemoryStream(message.Data);
            BinaryFormatter b = new BinaryFormatter();
            mm.Position = 0;
            mm.Seek(0, SeekOrigin.Begin);
            return b.Deserialize(mm);

            //using (var memoryStream = new MemoryStream(message.Data))
            //    return (new BinaryFormatter()).Deserialize(memoryStream);
        }
    }
}
