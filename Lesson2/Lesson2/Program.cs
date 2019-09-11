using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson2
{
    class Program
    {
        static void Main(string[] args)
        {
            var history = new History();
            var billsManager = new BillsManager();
            CreateBillCommand createBill = new CreateBillCommand(billsManager, history);
            CloseBillCommand closeBill = new CloseBillCommand(billsManager, history);
            TransactionBillCommand transactionBill = new TransactionBillCommand(billsManager, history);

            var commands = new []
            {
                new { name = "undo", action = new Action(() => history.Undo()) },
                new { name = "redo", action = new Action(() => history.Redo()) },
                new { name = "add", action = new Action(AddBill) },
                new { name = "show", action = new Action(() => billsManager.Show()) },
                new { name = "close", action = new Action(CloseBill) },
                new { name = "transaction", action = new Action(Transaction) },
            };

            Console.WriteLine("Welcome.");
            string command;
            do
            {
                command = Console.ReadLine().ToLower();
                commands.FirstOrDefault(item => item.name == command)?.action.Invoke();
            } while (command != "exit");

            void AddBill()
            {
                Console.WriteLine("How much money?");
                createBill.Do(ReadId());
            }

            void CloseBill()
            {
                Console.WriteLine("enter id ");
                closeBill?.Do(ReadId());
            }

            void Transaction()
            {
                Console.WriteLine("enter id from ");
                var idFrom = ReadId();
                Console.WriteLine("enter id to ");
                var idTo = ReadId();
                Console.WriteLine("enter how mach ");
                var money = ReadId();

                transactionBill?.Do(idFrom, idTo, money);
            }

            int ReadId()
            {
                int id;
                while (int.TryParse(Console.ReadLine(), out id) == false)
                    Console.WriteLine("incorrect value, enter again");

                return id;
            }
        }
    }

    public interface ICommand
    {
        void Undo();
        void Redo();
    }

    public class History
    {
        private readonly List<ICommand> _commands = new List<ICommand>();
        private int _currentIndex = -1;

        public void Undo()
        {
            if (_currentIndex < 0)
                return;

            _commands[_currentIndex].Undo();
            _currentIndex--;
        }

        public void Redo()
        {
            if (_commands.Count <= 0 || _commands.Count - 1 <= _currentIndex)
                return;

            _currentIndex++;
            _commands[_currentIndex].Redo();
        }

        public void Add(ICommand item)
        {
            CutOffHistory();
            _commands.Add(item);
        }

        private void CutOffHistory()
        {
            int index = _currentIndex + 1;
            if (index < _commands.Count)
                _commands.RemoveRange(index, _commands.Count - index);
        }
    }

    class SimpleCommand : ICommand
    {

        private readonly Action _execute, _undo;

        public SimpleCommand(Action execute, Action undo)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _undo = undo ?? throw new ArgumentNullException("undo");
        }

        public void Redo() => _execute.Invoke();
        public void Undo() => _undo.Invoke();
    }

    abstract class BillCommand: ICommand, ICloneable
    {
        protected readonly BillsManager _billsManager;
        protected History _history;

        public BillCommand(BillsManager billsManager, History history)
        {
            _billsManager = billsManager ?? throw new ArgumentNullException("billsManager");
            _history = history ?? throw new ArgumentNullException("history");
        }

        protected void RecordAndExecute()
        {
            ICommand command = (ICommand)Clone();
            _history.Add(command);
            _history.Redo();
        }

        public abstract void Redo();
        public abstract void Undo();
        public abstract object Clone();
    }

    class CreateBillCommand : BillCommand
    {
        private Bill _bill;
        public CreateBillCommand(BillsManager billsManager, History history, Bill bill = null) : base(billsManager, history)
        {
            _bill = bill;
        }

        public void Do(float money)
        {
            _bill = new Bill(_billsManager.GetNextId(), money);
            RecordAndExecute();
        }

        public override void Redo() => _billsManager.Add(_bill);
        public override void Undo() => _billsManager.Remove(_bill.Id);

        public override object Clone() => new CreateBillCommand(_billsManager, _history, _bill?.Clone() as Bill);
    }

    class CloseBillCommand : BillCommand
    {
        private int _id;
        private bool _previousActive;
        public CloseBillCommand(BillsManager billsManager, History history, int id = -1, bool previousActive = false) : base(billsManager, history)
        {
            _id = id;
            _previousActive = previousActive;
        }

        public void Do(int id)
        {
            if (id < 0 || _billsManager.CheckActive(id, ref _previousActive) == false)
            {
                Console.WriteLine("Bill not found. The command could not be executed.");
                return;
            }

            _id = id;
            RecordAndExecute();
        }

        public override void Redo() => _billsManager.SetActive(_id, false);
        public override void Undo() => _billsManager.SetActive(_id, _previousActive);

        public override object Clone() => new CloseBillCommand(_billsManager, _history, _id, _previousActive);
    }

    class TransactionBillCommand : BillCommand
    {
        private int _idFrom, _idTo, _money;
        public TransactionBillCommand(BillsManager billsManager, History history, int idFrom = -1, int idTo = -1, int money = 0) : base(billsManager, history)
        {
            _idFrom = idFrom;
            _idTo = idTo;
            _money = money;
        }

        public void Do(int idFrom, int idTo, int money)
        {
            if (idFrom == idTo)
            {
                Console.WriteLine("You cannot transfer between the same bill");
                return;
            }
            if (money <= 0)
            {
                Console.WriteLine("You cannot transfer an amount less than or equal to 0");
                return;
            }
            if (idFrom < 0 || _billsManager.CheckId(idFrom) == false)
            {
                Console.WriteLine("Bill 'from' not found. The command could not be executed.");
                return;
            }
            if (idTo < 0 || _billsManager.CheckId(idTo) == false)
            {
                Console.WriteLine("Bill 'to' not found. The command could not be executed.");
                return;
            }
            bool active = false;
            _billsManager.CheckActive(idTo, ref active);
            if (active == false)
            {
                Console.WriteLine("Bill 'To' is closed");
                return;
            }
            active = false;
            _billsManager.CheckActive(idFrom, ref active);
            if (active == false)
            {
                Console.WriteLine("Bill 'From' is closed");
                return;
            }
            if (_billsManager.CheckEnoughMoney(idFrom, money) == false)
            {
                Console.WriteLine("Bill 'From' is not have enough money");
                return;
            }

            _idFrom = idFrom;
            _idTo = idTo;
            _money = money;
            RecordAndExecute();
        }

        public override void Redo() => _billsManager.Transaction(_idFrom, _idTo, _money);
        public override void Undo() => _billsManager.Transaction(_idFrom, _idTo, _money);

        public override object Clone() => new TransactionBillCommand(_billsManager, _history, _idFrom, _idTo, _money);
    }
    

    class BillsManager
    {
        private List<Bill> _listBills = new List<Bill>();

        private Bill FindById(int id) => _listBills.Find(item => item.Id == id);

        public bool CheckId(int id) => _listBills.Any(item => item.Id == id);

        public int GetNextId() => _listBills.Any() ? _listBills.OrderBy(bill => bill.Id).Last().Id + 1 : 0;

        public void Add(Bill bill)
        {
            if (CheckId(bill.Id))
                throw new Exception("Tried add bill with existing id to list Bills");

            _listBills.Add(bill);
        }

        public void Remove(int id)
        {
            var bill = FindById(id) ?? throw new ArgumentNullException("id");
            _listBills.Remove(bill);
        }

        public void RemoveLast()
        {
            if (_listBills.Any())
                _listBills.RemoveAt(_listBills.Count - 1);
        }

        public bool SetActive(int id, bool active)
        {
            var bill = FindById(id);
            if (bill == null)
                return false;

            bill.Active(active);
            return true;
        }
        public bool CheckActive(int id, ref bool active)
        {
            var bill = FindById(id);
            if (bill == null)
                return false;

            active = bill.IsActive;
            return true;
        }


        public void Show()
        {
            if (_listBills.Any() == false)
            {
                Console.WriteLine("Empty");
                return;
            }

            foreach (var bill in _listBills)
                Console.WriteLine($"{bill.Id} {bill.Money} {bill.IsActive}");
            Console.WriteLine();

        }

        internal void Transaction(int idFrom, int idTo, int money)
        {
            if (idFrom == idTo)
                throw new Exception("idFrom == idTo");

            var billFrom = FindById(idFrom);
            var billTo = FindById(idTo);

            if (billFrom == null || billTo == null)
                throw new Exception("billFrom or billTo == null");

            if (billFrom.Money < money)
                throw new Exception("not enough money in billFrom");

            billFrom.Transaction(-money);
            billTo.Transaction(money);
        }

        internal bool CheckEnoughMoney(int id, float money)
        {
            var billFrom = FindById(id);
            return billFrom != null && billFrom.Money >= money;
        }
    }

    class Bill: ICloneable
    {
        public int Id { get; private set; }
        public float Money { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Bill(int id, float money = 0f)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException("id");

            Id = id;
            Money = money;
        }

        public void Transaction(int money)
        {
            if (IsActive == false)
                throw new Exception("Attempt to transfer to a closed account");

            var result = Money + money;
            if (result < 0)
                throw new ArgumentOutOfRangeException("Money");

            Money = result;
        }

        public void Active(bool active) => IsActive = active;
        public object Clone() => new Bill(Id, Money);
    }
}
