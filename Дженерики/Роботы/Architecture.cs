using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Generics.Robots
{
    //Короче, здесь мы добавляем интерфейс. Пиши точь-в-точь.
    public interface IRobotAi<out T> where T: IMoveCommand
    {
        T GetCommand();
    }
    //Далее мы меняем наш первоначальный абстрактный класс, дописывая вот это:
    //<T>:IRobotAi<T> where T:IMoveCommand
    //После этого мы добавляем в класс переменную counter, чтобы в дальнейшем не дублировать код
    //также меняем переменную object на T
    public abstract class RobotAI<T>:IRobotAi<T> where T:IMoveCommand
    {
        public int counter = 1;
        public abstract T GetCommand();
    }
    //Здесь и далее уже мы наследуем интерфейс IMoveCommand.Запоминай его, он будет много где, практически везде
    //Также когда мы переопределяем метод, нам нужно писать возращаемый тип переменной IMoveCommand, крч ты понял
    public class ShooterAI : RobotAI<IMoveCommand>
    {
        public override IMoveCommand GetCommand()
        {
            return ShooterCommand.ForCounter(counter++);
        }
    }
    //Тут также, как и выше
    public class BuilderAI : RobotAI<IMoveCommand>
    {
        public override IMoveCommand GetCommand()
        {
            return BuilderCommand.ForCounter(counter++);
        }
    }
    //Теперь создаём интерфейс и для Device, только не перепутай: у робота out, у девайса in
    public interface IDevice<in T> where T: IMoveCommand
    {
        string ExecuteCommand(T command);
    }
    //тут впринципе пишем также, как и с роботами, когда реализуем интерфейс
    public abstract class Device<T>:IDevice<T> where T:IMoveCommand
    {
        public abstract string ExecuteCommand(T command);
    }
    //Здесь мы наследуем также, как и в роботах делали(заметь, везде у нас IMoveCommand в скобках<>)
    //Там мы вполне можем избавиться от одной строчки(var command = _command as IMoveCommand;)
    //у нас вместо object ставится IMoveCommand
    public class Mover : Device<IMoveCommand>
    {
        public override string ExecuteCommand(IMoveCommand command)
        {
            if (command == null)
                throw new ArgumentException();
            return $"MOV {command.Destination.X}, {command.Destination.Y}";
        }
    }
    //Тут уже поинтереснее, т.к у нас нужно command преобразовывать в другой интерфейс, а именно в IShooterMoveCommand
    //Без этого не заведётся, можно через костыль, что тебе кидал ранее, но это не совсем мобильный вариант-этот легче и короче
    public class ShooterMover : Device<IMoveCommand>
    {
        public override string ExecuteCommand(IMoveCommand command)
        {
            if (command == null)
                throw new ArgumentException();
            var hide = ((IShooterMoveCommand)command).ShouldHide ? "YES" : "NO";
            return $"MOV {command.Destination.X}, {command.Destination.Y}, USE COVER {hide}";
        }
    }
    //Тут мы создаём статический класс Robot для реализации метода Create-везде в конце добавляй <IMoveCommand>
    public static class Robot
    {
        public static Robot<IMoveCommand> Create<TCommand>(RobotAI<IMoveCommand> ai, Device<IMoveCommand> executor)
        {
            return new Robot<IMoveCommand>(ai, executor);
        }
    }
    //Тут добавляем <T> в начале класса и также везде приписываем <IMoveCommand>
    public class Robot<T>
    {
        private readonly RobotAI<IMoveCommand> ai;
        private readonly Device<IMoveCommand> device;

        public Robot(RobotAI<IMoveCommand> ai, Device<IMoveCommand> executor)
        {
            this.ai = ai;
            this.device = executor;
        }

        public IEnumerable<string> Start(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                var command = ai.GetCommand();
                if (command == null)
                    break;
                yield return device.ExecuteCommand(command);
            }
        }
    }
    //Задача решена!
}
