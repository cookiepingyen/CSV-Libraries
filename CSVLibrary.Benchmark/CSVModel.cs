using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVLibrary.Benchmark
{
    internal class CSVModel
    {
        public CSVModel() { }
        public CSVModel(string id, string firstName, string lastName, string email, string gender, string ipAddress)
        {
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.gender = gender;
            this.ipAddress = ipAddress;
        }

        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string ipAddress { get; set; }
    }
}
