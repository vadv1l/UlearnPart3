using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inheritance.MapObjects
{
    public interface IOwner
    {
        int Owner { get; set; }
    }
    public interface IArmy
    {
        Army Army { get; set; }
    }
    public interface ITreasure
    {
        Treasure Treasure { get; set; }
    }
    public class Dwelling:IOwner
    {
        public int Owner { get; set; }
    }

    public class Mine:IOwner,IArmy,ITreasure
    {
        public int Owner { get; set; }
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Creeps:IArmy,ITreasure
    {
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Wolves:IArmy
    {
        public Army Army { get; set; }
    }

    public class ResourcePile:ITreasure
    {
        public Treasure Treasure { get; set; }
    }

    public static class Interaction
    {
        public static void Make(Player player, object mapObject)
        {
            if (mapObject is IArmy army)
            {
                if (!player.CanBeat(army.Army))
                {
                    player.Die();
                }
            }
            //«десь и далее используйте следующий сокращенный синтаксис преобразовани€ типа
            if (mapObject is IOwner owner)
            {
                //ќн более короткий и позвол€ет не производить множественных преобразований таких, как
                //((Dwelling)mapObject).Owner = player.Id;
                //а сразу обращатьс€ к объекту, если он €вл€етс€ каким-то классом.
                if (!player.Dead)
                {
                    owner.Owner = player.Id;
                }
            }
            if(mapObject is ITreasure treasure)
            {
                if (!player.Dead)
                {
                    player.Consume(treasure.Treasure);
                }
            }
            
            //ѕеред выполнение задани€ потренируйтесь и замените неправильное использование is ниже
            /*if (mapObject is Mine)
            {
                if (player.CanBeat(((Mine)mapObject).Army))
                {
                    ((Mine)mapObject).Owner = player.Id;
                    player.Consume(((Mine)mapObject).Treasure);
                }
                else player.Die();
                return;
            }

            if (mapObject is Creeps)
            {
                if (player.CanBeat(((Creeps)mapObject).Army))
                    player.Consume(((Creeps)mapObject).Treasure);
                else
                    player.Die();
                return;
            }

            if (mapObject is ResourcePile)
            {
                player.Consume(((ResourcePile)mapObject).Treasure);
                return;
            }

            if (mapObject is Wolves)
            {
                if (!player.CanBeat(((Wolves)mapObject).Army))
                    player.Die();
            }
            */
        }
    }
}
