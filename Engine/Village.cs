using Grpc.Core;
using GrpcVillage.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcVillage.Engine
{
    public class Village
    {
        private readonly VillageStartup _startup;
        private readonly VillageStatus _status;
        private bool _running = true;

        private Random _r = new Random();
        private int _year;
        private int _month;
        private ServerCallContext _context;

        private List<Villager> _villagers;
        
        public Village(VillageStartup startup, ServerCallContext context)
        {
            _context = context;
            _startup = startup;
            _year = 346;
            _month = 1;
            _status = new VillageStatus
            {
                Time = "",
                People = _startup.People,
                Message = ""
            };
            _villagers = new List<Villager>();
            for (int i = 0; i < startup.People; i++)
            {
                _villagers.Add(new Villager
                {
                    IsMale = _r.Next(1) == 1,
                    IsPregnant = false,
                    Age = _r.Next(30),
                    MonthBorn = _r.Next(1, 12)
                });
                
            }
        }

        public async Task Run(IServerStreamWriter<VillageStatus> crier)
        {
            try
            {
                _status.Message = $"Hear ye Hear ye - The village of the '{_startup.Name}' has been created!";
                await crier.WriteAsync(_status);
                while (_running)
                {
                    await Task.Delay(16);
                    Tick();
                    if (_context.CancellationToken.IsCancellationRequested) break;
                    await crier.WriteAsync(_status);
                    if (_status.People > 350)
                    {
                        _running = false;
                    }
                }
                _status.Message = $"The village of the '{_startup.Name}' has finished its glorious mission!";
                if (!_context.CancellationToken.IsCancellationRequested)
                {
                    await crier.WriteAsync(_status);
                }
            }
            finally
            {
                Console.WriteLine("This village is dead");
            }
        }

        private void Tick()
        {
            AdvanceTime();

            GetOlder();

            Pregnancies();

            List<Villager> newVillagers = Births();

            List<Villager> deadVillagers = Deaths();

            SetStatus(newVillagers, deadVillagers);
        }

        private void AdvanceTime()
        {
            _month = _month == 12 ? 1 : _month + 1;
            _year = _month == 1 ? _year + 1 : _year;
        }

        private void SetStatus(List<Villager> newVillagers, List<Villager> deadVillagers)
        {
            _status.People = _villagers.Count;
            if (deadVillagers.Count > 0 || newVillagers.Count > 0)
            {
                _status.Message = $"There are {_status.People} in the village, there have been {deadVillagers.Count} deaths and {newVillagers.Count} births";
            }
            else
            {
                _status.Message = "";
            }
            _status.Time = $"{_month.ToString('m')}, The Year of our Lord {_year}";
        }

        private List<Villager> Deaths()
        {
            var deadVillagers = new List<Villager>();
            foreach (var v in _villagers.Where(v => v.DeathRoll))
            {
                var roll = _r.Next(100);
                if (roll > (99 - (v.LifeExpectancy / 2 + v.Age)))
                {
                    deadVillagers.Add(v);
                }
            }
            foreach (var v in deadVillagers)
            {
                _villagers.Remove(v);
            }

            return deadVillagers;
        }

        private List<Villager> Births()
        {
            var newVillagers = new List<Villager>();
            foreach (var v in _villagers.Where(v => v.IsPregnant))
            {
                v.MonthsUntilBirth--;
                if (v.MonthsUntilBirth == 0)
                {
                    newVillagers.Add(new Villager
                    {
                        IsMale = _r.Next(1) == 1,
                        IsPregnant = false,
                        Age = 0,
                        MonthBorn = _month
                    });
                }
            }
            _villagers.AddRange(newVillagers);
            return newVillagers;
        }

        private void Pregnancies()
        {
            foreach (var v in _villagers.Where(v => v.IsFertile && !v.IsMale && !v.IsPregnant))
            {
                var roll = _r.Next(100);
                if (roll > 66)
                {
                    v.IsPregnant = true;
                    v.MonthsUntilBirth = 9;
                }
            }
        }

        private void GetOlder()
        {
            foreach (var v in _villagers)
            {
                if (v.MonthBorn == _month)
                {
                    v.Age++;
                }
            }
        }
    }
}
