using System;

namespace TicketStateMachine
{
    public interface ITicketState
    {
        void SelectTicket();
        void InsertMoney(decimal amount);
        void Cancel();
        void DispenseTicket();
    }

    public class TicketMachine
    {
        public ITicketState IdleState { get; }
        public ITicketState WaitingForMoneyState { get; }
        public ITicketState MoneyReceivedState { get; }
        public ITicketState TicketDispensedState { get; }
        public ITicketState TransactionCanceledState { get; }

        public ITicketState State { get; set; }

        public decimal TicketPrice { get; } = 100m;
        public decimal CurrentAmount { get; set; }

        public TicketMachine()
        {
            IdleState = new IdleState(this);
            WaitingForMoneyState = new WaitingForMoneyState(this);
            MoneyReceivedState = new MoneyReceivedState(this);
            TicketDispensedState = new TicketDispensedState(this);
            TransactionCanceledState = new TransactionCanceledState(this);

            State = IdleState;
        }

        public void SelectTicket() => State.SelectTicket();
        public void InsertMoney(decimal amount) => State.InsertMoney(amount);
        public void Cancel() => State.Cancel();
        public void DispenseTicket() => State.DispenseTicket();

        public void Reset()
        {
            CurrentAmount = 0;
            State = IdleState;
            Console.WriteLine("Автомат сброшен в состояние Idle.\n");
        }
    }

    public class IdleState : ITicketState
    {
        private readonly TicketMachine _machine;

        public IdleState(TicketMachine machine)
        {
            _machine = machine;
        }

        public void SelectTicket()
        {
            Console.WriteLine("Билет выбран. Переход в состояние WaitingForMoney.");
            _machine.State = _machine.WaitingForMoneyState;
        }

        public void InsertMoney(decimal amount)
        {
            Console.WriteLine("Сначала выберите билет.");
        }

        public void Cancel()
        {
            Console.WriteLine("Отменять нечего, автомат в ожидании.");
        }

        public void DispenseTicket()
        {
            Console.WriteLine("Нельзя выдать билет: билет не выбран и деньги не внесены.");
        }
    }
    public class WaitingForMoneyState : ITicketState
    {
        private readonly TicketMachine _machine;

        public WaitingForMoneyState(TicketMachine machine)
        {
            _machine = machine;
        }

        public void SelectTicket()
        {
            Console.WriteLine("Билет уже выбран, внесите деньги.");
        }

        public void InsertMoney(decimal amount)
        {
            _machine.CurrentAmount += amount;
            Console.WriteLine($"Внесено {amount}. Всего: {_machine.CurrentAmount}.");

            if (_machine.CurrentAmount >= _machine.TicketPrice)
            {
                Console.WriteLine("Достаточно денег. Переход в состояние MoneyReceived.");
                _machine.State = _machine.MoneyReceivedState;
            }
        }

        public void Cancel()
        {
            Console.WriteLine("Транзакция отменена. Возврат денег пользователю.");
            _machine.CurrentAmount = 0;
            _machine.State = _machine.TransactionCanceledState;
        }

        public void DispenseTicket()
        {
            Console.WriteLine("Недостаточно средств, билет выдать нельзя.");
        }
    }

    public class MoneyReceivedState : ITicketState
    {
        private readonly TicketMachine _machine;

        public MoneyReceivedState(TicketMachine machine)
        {
            _machine = machine;
        }

        public void SelectTicket()
        {
            Console.WriteLine("Билет уже выбран и оплачён.");
        }

        public void InsertMoney(decimal amount)
        {
            Console.WriteLine("Денег уже достаточно, выберите выдачу билета или отмену.");
        }

        public void Cancel()
        {
            Console.WriteLine("Покупка отменена. Деньги возвращены пользователю.");
            _machine.CurrentAmount = 0;
            _machine.State = _machine.TransactionCanceledState;
        }

        public void DispenseTicket()
        {
            Console.WriteLine("Билет выдан пользователю.");
            _machine.CurrentAmount = 0;
            _machine.State = _machine.TicketDispensedState;
        }
    }

    public class TicketDispensedState : ITicketState
    {
        private readonly TicketMachine _machine;

        public TicketDispensedState(TicketMachine machine)
        {
            _machine = machine;
        }

        public void SelectTicket()
        {
            Console.WriteLine("Предыдущая операция завершена. Сначала вернёмся в Idle.");
            _machine.Reset();
            _machine.SelectTicket();
        }

        public void InsertMoney(decimal amount)
        {
            Console.WriteLine("Операция завершена. Начните новую покупку.");
        }

        public void Cancel()
        {
            Console.WriteLine("Операция уже завершена, отменять нечего.");
        }

        public void DispenseTicket()
        {
            Console.WriteLine("Билет уже был выдан.");
        }
    }

    public class TransactionCanceledState : ITicketState
    {
        private readonly TicketMachine _machine;

        public TransactionCanceledState(TicketMachine machine)
        {
            _machine = machine;
        }

        public void SelectTicket()
        {
            Console.WriteLine("Начинаем новую покупку после отмены.");
            _machine.Reset();
            _machine.SelectTicket();
        }

        public void InsertMoney(decimal amount)
        {
            Console.WriteLine("Транзакция отменена. Начните с выбора билета.");
        }

        public void Cancel()
        {
            Console.WriteLine("Уже отменено.");
        }

        public void DispenseTicket()
        {
            Console.WriteLine("После отмены билет не выдается.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var machine = new TicketMachine();

            Console.WriteLine("=== Успешная покупка ===");
            machine.SelectTicket();          
            machine.InsertMoney(50);         
            machine.InsertMoney(50);         
            machine.DispenseTicket();        

            Console.WriteLine("\n=== Отмена во время оплаты ===");
            machine.Reset();
            machine.SelectTicket();          // укпапва
            machine.InsertMoney(30);
            machine.Cancel();                // 
        }
    }
}