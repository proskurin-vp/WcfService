using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataModelLibrary;
using Newtonsoft.Json;

namespace WcfServiceLibraryCheck
{
    public class FakeRepository : IRepository
    {
        private readonly string _folderAppData;
        public FakeRepository(string folderAppData)
        {
            _folderAppData = folderAppData;
            Name = "Fake Repository";
        }

        public string Name { get; private set; }

        public List<Check> GetLastPack(int packSize)
        {
            Random random = new Random();
            List<Check> checks = new List<Check>();
            List<int> numbers = Enumerable.Range(1, packSize).ToList();
            numbers.ForEach(number => checks.Add(new Check(){CheckNumber = numbers.ToString(), Id = random.Next()}));
            return checks;
        }

        public int SaveCheck(Check check)
        {
            string fileName = Path.GetRandomFileName() + ".txt";
            string output = JsonConvert.SerializeObject(check);
            File.WriteAllText(Path.Combine(_folderAppData, fileName), output);
            return check.Id;
        }
    }
}
