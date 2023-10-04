using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Reflection.Randomness
{
    public class FromDistribution : Attribute
    {
        //Для начала нам нужно создать свойство Distribution, что значит наше распределение и числа, что будут на входе
        public IContinuousDistribution Distribution { get; set; }
        //Далее конструктор, в котором мы смотрим, возможно ли вообще реализовать какое-либо из распределений и в случае если нельзя-выводим ошибку
        public FromDistribution(Type distribution,params object[] array)
        {
            //присваиваем переменной TypeContDistr интерфейс (IContinuousDistribution)
            var typeContDistr = typeof(IContinuousDistribution);
            //Далее мы проверим,реализует ли наше распределение интерфейс IContinuousDistribution, также не забываем, что на входе может быть от 0 до 2 значений.
            if (distribution.GetInterface(typeContDistr.Name) == typeContDistr && array.Length < 3)
            {
                //В случае, если всё ок и он наследует, то в этом случае мы создаём объект IContinuousDistribution(тут главное CreateInstance выучить и с кайфом)
                Distribution = (IContinuousDistribution)Activator.CreateInstance(distribution, array);
            }
            //Иначе мы выводим ошибку
            else
            {
                throw new ArgumentException(distribution.Name);
            }
        }
    }
    //Далее идёт уже ёбнутый слегка класс Generator<T>.
    public class Generator<T>
    {
        //Создаём словарь, куда будем записывать свойство-распределение
        Dictionary<PropertyInfo, IContinuousDistribution> dicktionary = new Dictionary<PropertyInfo, IContinuousDistribution>();
        //далее конструктор
        public Generator()
        {
            //После этого создаём переменную, в которую мы запихаем все свойства нашего класса T
            PropertyInfo[] properties = typeof(T).GetProperties();
            //Далее пробегаемся по всем свойствам
            foreach (var item in properties)
            {
                //Создаём ещё одну переменную для того, чтобы мы получили 
                var distr = item.GetCustomAttributes(true).OfType<FromDistribution>().FirstOrDefault();
                if (distr != null)
                {
                    dicktionary[item] = distr.Distribution;
                }
            }
        }
        public T Generate(Random rnd)
        {
            var ctorGenDistr = typeof(T).GetConstructor(new Type[0]).Invoke(new object[0]);
            foreach (var item in dicktionary.Keys)
            {
                item.SetValue(ctorGenDistr, dicktionary[item].Generate(rnd));
            }
            return (T)ctorGenDistr;
        }
    }
}
