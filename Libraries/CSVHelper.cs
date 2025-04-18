using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Libraries
{
    public class CSVHelper
    {

        static PropertyInfo[] props = null;
        static int PropsCount;


        delegate object GetterDelegate(object target);
        private static GetterDelegate[] _getterDelegates = null;

        delegate void SetterDelegate(object target, object value);
        private static SetterDelegate[] _setterDelegates = null;

        private static GetterDelegate CreateGetter(PropertyInfo property)
        {
            var targetType = typeof(object);
            var targetParam = Expression.Parameter(targetType, "target");
            var castTarget = Expression.Convert(targetParam, property.DeclaringType);
            var propertyGetter = Expression.Call(castTarget, property.GetGetMethod());
            var lambda = Expression.Lambda<GetterDelegate>(propertyGetter, targetParam);
            return lambda.Compile();
        }
        private static SetterDelegate CreateSetter(PropertyInfo property)
        {
            var targetType = typeof(object);
            var valueType = typeof(object);

            var targetParam = Expression.Parameter(targetType, "target");
            var valueParam = Expression.Parameter(valueType, "value");

            var castTarget = Expression.Convert(targetParam, property.DeclaringType);
            var castValue = Expression.Convert(valueParam, property.PropertyType);

            var propertySetter = Expression.Call(castTarget, property.GetSetMethod(), castValue);

            var lambda = Expression.Lambda<SetterDelegate>(propertySetter, targetParam, valueParam);
            return lambda.Compile();
        }


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
                writer.WriteLine(',');
                stringBuilder.Clear();
            }
        }

        private void OptimizeWriteList<T>(string path, List<T> csvdatas)
        {
            if (props == null || _getterDelegates == null)
            {
                props = typeof(T).GetProperties();
                PropsCount = props.Length;
                _getterDelegates = props.Select(p => CreateGetter(p)).ToArray();
            }

            StringBuilder stringBuilder = new StringBuilder(90);
            char[] chars = new char[90];

            using (StreamWriter writer = new StreamWriter($"{path}", true))
            {
                foreach (T csvdata in csvdatas)
                {
                    for (int i = 0; i < props.Length; i++)
                    {
                        stringBuilder.Append(props[i].GetValue(csvdata));
                        if (i < props.Length - 1)
                            stringBuilder.Append(",");
                    }

                    stringBuilder.CopyTo(0, chars, 0, stringBuilder.Length - 1);
                    writer.WriteLine(chars, 0, stringBuilder.Length - 1);
                    stringBuilder.Clear();
                }

                writer.Flush();

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

        public List<T> OptimizeRead<T>(string path, int startIndex, int count) where T : class, new()
        {
            List<T> list = new List<T>();

            if (props == null || _setterDelegates == null)
            {
                props = typeof(T).GetProperties();
                PropsCount = props.Length;
                _setterDelegates = props.Select(p => CreateSetter(p)).ToArray();
            }

            using (StreamReader reader = new StreamReader($"{path}"))
            {
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
                    ReadOnlySpan<char> datas = reader.ReadLine().AsSpan();

                    int start = 0;
                    for (int i = 0; i < PropsCount; i++)
                    {
                        // 找逗號位置
                        int commaIndex = datas.Slice(start).IndexOf(',');

                        if (commaIndex == -1)
                        {
                            // 最後一欄
                            _setterDelegates[i](t, datas.Slice(start).ToString());
                            break;
                        }
                        else
                        {
                            _setterDelegates[i](t, datas.Slice(start, commaIndex).ToString());
                            start += commaIndex + 1;
                        }
                    }
                    list.Add(t);
                }
            }
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
