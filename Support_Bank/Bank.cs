﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Support_Bank
{
    public class Bank
    {
        public List<Transaction> listOfAllTransactions;
        public List<Person> listOfAllPeople;

        public void GenerateTransactionList(string filename)
        {
            List<Transaction> listOfTransactions = new List<Transaction>();

            StreamReader sr = new StreamReader(filename);
            string line;
            string[] row = new string [5];
            line = sr.ReadLine();
            while ((line = sr.ReadLine()) != null)
            {
                row = line.Split(',');
                listOfTransactions.Add(new Transaction(row));
            }

            this.listOfAllTransactions = listOfTransactions;
        }

        public void GeneratePeopleList()
        {
            List<string> listofnames = new List<string>();
            List<Person> listofpeople = new List<Person>();
            foreach (Transaction transaction in listOfAllTransactions)
            {
                listofnames.Add(transaction.from);
            }
            listofnames = listofnames.Distinct().ToList();
            foreach (string name in listofnames)
            {
                listofpeople.Add(new Person(name));
            }

            this.listOfAllPeople = listofpeople;
        }

        public void AssignTransactions()
        {
            foreach (Transaction payment in listOfAllTransactions)
            {
                Person payer = listOfAllPeople.Find(item => item.name == payment.from);
                Person payee = listOfAllPeople.Find(item => item.name == payment.to);
                
                payer.AddPaymentOut(payment);
                payee.AddPaymentIn(payment);
            } 
        }

        public void SetUpFromCsv(string filename)
        {
            GenerateTransactionList(filename);
            GeneratePeopleList();
            AssignTransactions();
        }

        public void ListAll()
        {
            foreach (Person person in listOfAllPeople)
            {
                float balance = person.CalculateBalance();
                switch (balance>=0)
                {
                case    true:
                    Console.WriteLine($"{person.name} is owed {balance:c}");
                    break;
                case false:
                    balance = Math.Abs(balance);
                    Console.WriteLine($"{person.name} owes {balance:C}");
                    break;
                }
                
            }
        }


        public void ListIndividual(string name)
        {
            Person person = listOfAllPeople.Find(item => item.name == name);
            person.PrintTransactions();
        }
    }
}