using System;
using System.Data.Entity;

namespace Demo_Exzamen
{
    public class Client
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public string GenderCode { get; set; }
        public string PhotoPath { get; set; }
    }

}