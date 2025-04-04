using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Libraries
{
    public class CSVHelper
    {
        public void Write(string path, object csvdata)
        {
            if (!CSVEnd(path))
            {
                throw new Exception("檔案沒有.csv結尾");
            }
            if (!PathExist(path))
            {
                if (!CreateDirPath(path))
                {
                    throw new Exception("建立資料夾失敗");
                }

            }

            StreamWriter writer = new StreamWriter($"{path}", true);
            PropertyInfo[] props = csvdata.GetType().GetProperties();

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < props.Length; i++)
            {
                stringBuilder.Append(props[i].GetValue(csvdata));
                if (i < props.Length - 1)
                    stringBuilder.Append(",");
            }

            writer.WriteLine($"{stringBuilder.ToString()}");
            writer.Flush();
            writer.Close();
        }

        public void WriteList<T>(string path, List<T> list, bool isNew = false) where T : class, new()
        {
            if (isNew)
            {
                // remove csv
                File.Delete(path);
            }

            foreach (object csvdata in list)
            {
                Write(path, csvdata);
            }

        }



        public List<T> Read<T>(string path) where T : class, new()
        {

            if (!CSVEnd(path))
            {
                throw new Exception("檔案沒有.csv結尾");
            }
            if (!PathExist(path))
            {
                throw new Exception("路徑不存在");
            }
            if (!FileExist(path))
            {
                throw new Exception("檔案不存在");
            }


            StreamReader reader = new StreamReader($"{path}");
            List<T> list = new List<T>();

            while (!reader.EndOfStream)
            {
                T t = new T();
                PropertyInfo[] props = t.GetType().GetProperties();

                string[] line = reader.ReadLine().Split(',');

                for (int i = 0; i < line.Length; i++)
                {
                    props[i].SetValue(t, line[i]);
                }
                list.Add(t);
            }
            reader.Close();

            return list;
        }


        private bool CSVEnd(string path)
        {
            // \r \n \t \\
            string fliename = path.Split('\\').Last();
            return fliename.Contains(".csv");
        }


        private bool PathExist(string path)
        {
            string[] pathArray = path.Split('\\');
            string[] dirArrays = pathArray.Take(pathArray.Length - 1).ToArray();

            string dirPath = string.Join("\\", dirArrays);
            return Directory.Exists(dirPath);
        }

        private bool CreateDirPath(string path)
        {
            string dirPath = "";
            string[] pathArray = path.Split('\\');
            string[] dirArrays = pathArray.Take(pathArray.Length - 1).ToArray();

            dirPath = string.Join("\\", dirArrays);
            DirectoryInfo directoryInfo = Directory.CreateDirectory(dirPath);
            return directoryInfo.Exists;
        }

        private bool FileExist(string path)
        {
            return File.Exists(path);
        }
    }

}
