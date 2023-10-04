using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Generics.Robots
{
    //������, ����� �� ��������� ���������. ���� ����-�-����.
    public interface IRobotAi<out T> where T: IMoveCommand
    {
        T GetCommand();
    }
    //����� �� ������ ��� �������������� ����������� �����, ��������� ��� ���:
    //<T>:IRobotAi<T> where T:IMoveCommand
    //����� ����� �� ��������� � ����� ���������� counter, ����� � ���������� �� ����������� ���
    //����� ������ ���������� object �� T
    public abstract class RobotAI<T>:IRobotAi<T> where T:IMoveCommand
    {
        public int counter = 1;
        public abstract T GetCommand();
    }
    //����� � ����� ��� �� ��������� ��������� IMoveCommand.��������� ���, �� ����� ����� ���, ����������� �����
    //����� ����� �� �������������� �����, ��� ����� ������ ����������� ��� ���������� IMoveCommand, ��� �� �����
    public class ShooterAI : RobotAI<IMoveCommand>
    {
        public override IMoveCommand GetCommand()
        {
            return ShooterCommand.ForCounter(counter++);
        }
    }
    //��� �����, ��� � ����
    public class BuilderAI : RobotAI<IMoveCommand>
    {
        public override IMoveCommand GetCommand()
        {
            return BuilderCommand.ForCounter(counter++);
        }
    }
    //������ ������ ��������� � ��� Device, ������ �� ���������: � ������ out, � ������� in
    public interface IDevice<in T> where T: IMoveCommand
    {
        string ExecuteCommand(T command);
    }
    //��� ��������� ����� �����, ��� � � ��������, ����� ��������� ���������
    public abstract class Device<T>:IDevice<T> where T:IMoveCommand
    {
        public abstract string ExecuteCommand(T command);
    }
    //����� �� ��������� �����, ��� � � ������� ������(������, ����� � ��� IMoveCommand � �������<>)
    //��� �� ������ ����� ���������� �� ����� �������(var command = _command as IMoveCommand;)
    //� ��� ������ object �������� IMoveCommand
    public class Mover : Device<IMoveCommand>
    {
        public override string ExecuteCommand(IMoveCommand command)
        {
            if (command == null)
                throw new ArgumentException();
            return $"MOV {command.Destination.X}, {command.Destination.Y}";
        }
    }
    //��� ��� ������������, �.� � ��� ����� command ��������������� � ������ ���������, � ������ � IShooterMoveCommand
    //��� ����� �� ��������, ����� ����� �������, ��� ���� ����� �����, �� ��� �� ������ ��������� �������-���� ����� � ������
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
    //��� �� ������ ����������� ����� Robot ��� ���������� ������ Create-����� � ����� �������� <IMoveCommand>
    public static class Robot
    {
        public static Robot<IMoveCommand> Create<TCommand>(RobotAI<IMoveCommand> ai, Device<IMoveCommand> executor)
        {
            return new Robot<IMoveCommand>(ai, executor);
        }
    }
    //��� ��������� <T> � ������ ������ � ����� ����� ����������� <IMoveCommand>
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
    //������ ������!
}
