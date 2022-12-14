using System.Data;

namespace APi_DataBase.Utils
{
    public class Tools
    {
        public static List<T> GetList<T>(IDataReader reader)
        {
            List<T> obj_list = new List<T>();
            var type = typeof(T);
            while (reader.Read())
            {
                T obj = (T)Activator.CreateInstance(type);
                foreach (var prop in type.GetProperties())
                {
                    Type propType = prop.PropertyType;
                    prop.SetValue(obj, ChangeType(reader[prop.Name].ToString(), propType));
                }
                obj_list.Add(obj);
            }
            return obj_list;
        }
        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }
    }
}
