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
        public void Write<T>(string path, T csvdata)
        {
            WritePreCheck(path);
            StreamWriter writer = new StreamWriter($"{path}", true);

            List<T> csvdatas = new List<T>() { csvdata };
            WriteDatas(writer, csvdatas);

            writer.Flush();
            writer.Close();
        }

        public void WriteList<T>(string path, List<T> csvdatas) where T : class, new()
        {
            WritePreCheck(path);
            StreamWriter writer = new StreamWriter($"{path}", true);
            WriteDatas(writer, csvdatas);

            writer.Flush();
            writer.Close();
        }

        private void WriteDatas<T>(StreamWriter writer, List<T> csvdatas)
        {
            PropertyInfo[] props = typeof(T).GetProperties();
            StringBuilder stringBuilder = new StringBuilder();

            foreach (T csvdata in csvdatas)
            {
                for (int i = 0; i < props.Length; i++)
                {
                    stringBuilder.Append(props[i].GetValue(csvdata));
                    if (i < props.Length - 1)
                        stringBuilder.Append(",");
                }
                writer.WriteLine($"{stringBuilder}");
                stringBuilder.Clear();
            }
        }


        public List<T> Read<T>(string path, int startIndex, int count) where T : class, new()
        {
            ReadPreCheck(path);

            StreamReader reader = new StreamReader($"{path}");
            List<T> list = new List<T>();

            int accumulationNum = 0;

            while (!reader.EndOfStream)
            {
                accumulationNum++;

                if (accumulationNum < startIndex)
                {
                    reader.ReadLine();
                    continue;
                }
                else if (accumulationNum >= startIndex + count)
                {
                    break;
                }
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


        private void WritePreCheck(string path)
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
        }


        private void ReadPreCheck(string path)
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
        }
    }

}
