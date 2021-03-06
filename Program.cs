﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

namespace Tjuv_och_Polis
{
    class Program
    {
        static Random random = new Random();
        static int yAxisLength = 25;
        static int xAxisLength = 100;
        static int arrestedThiefs = 0;
        static int robbedCitizens = 0;
        static List<Person> people = new List<Person>();
        static List<Thief> prison = new List<Thief>();

        static void Main(string[] args)
        {
            SimulationStart();
        }

        static void SimulationStart()
        {
            CreatePlayers(30, 20, 10);

            while (true)
            {
                //räknar tjuvarnas tid i fängelset
                Jail();
                Console.Clear();
                //flyttar folket
                MovePlayers();
                //Ritar upp spelplan
                DrawGame();


                Thread.Sleep(500);
            }
        }

        static void CreatePlayers(int citizen, int thief, int police)
        {
            for (int i = 0; i < citizen; i++)
            {
                people.Add(new Citizen());
            }
            for (int i = 0; i < thief; i++)
            {
                people.Add(new Thief());
            }
            for (int i = 0; i < police; i++)
            {
                people.Add(new Police());
            }

        }

        static void MovePlayers()
        {
            foreach (var person in people)
            {
                //Flyttar på personens x axel
                person.PositionX += person.MovementX;
                if (person.PositionX > xAxisLength)
                    person.PositionX = 0;
                else if (person.PositionX < 0)
                    person.PositionX = xAxisLength;
                //Flyttar på personens y axel
                person.PositionY += person.MovementY;
                if (person.PositionY > yAxisLength)
                    person.PositionY = 0;
                else if (person.PositionY < 0)
                    person.PositionY = yAxisLength;
            }
        }

        static void DrawGame()
        {
            string meet = "";
            for (int y = 0; y < yAxisLength; y++)
            {
                for (int x = 0; x < xAxisLength; x++)
                {
                    var peopleOnSquare = new List<Person>();
                    foreach (var person in people)
                    {
                        //Ritar upp person
                        if (person.PositionX == x && person.PositionY == y)
                        {
                            //om det är mer än 1 person på rutan
                            if (person is Thief && ((Thief)person).timeInJail > 0)
                            {

                            }
                            else
                            {
                                if (peopleOnSquare.Count > 0)
                                {
                                    meet = Meet(peopleOnSquare[0], person);
                                }
                                peopleOnSquare.Add(person);
                                Console.Write(person.Type);
                            }

                        }
                    }
                    if (peopleOnSquare.Count == 0)
                        Console.Write(" ");
                }
                Console.Write("\n");
            }
            if (meet != "")
            {
                Console.WriteLine("\n" + meet);
                Thread.Sleep(2000);
            }
            Console.WriteLine("Antal rånade medborgare: " + robbedCitizens);
            Console.WriteLine("Antal gripna tjuvar: " + arrestedThiefs);
            Console.Write("Antal fångar: " + prison.Count);
            for (int i = 0; i < prison.Count; i++)
            {
                Console.Write($" Fånge {i + 1}: {prison[i].timeInJail}s");
            }
            Console.WriteLine();
        }

        static string Meet(Person p1, Person p2)
        {
            string result = "";

            if (p1 is Thief && p2 is Citizen)
            {
                result = RobCitizen((Citizen)p2, (Thief)p1);
            }
            else if (p1 is Citizen && p2 is Thief)
            {
                result = RobCitizen((Citizen)p1, (Thief)p2);
            }
            else if (p1 is Thief && p2 is Police)
            {
                result = ArrestThief((Thief)p1, (Police)p2);
            }
            else if (p1 is Police && p2 is Thief)
            {
                result = ArrestThief((Thief)p2, (Police)p1);
            }
                

            return result;
        }

        static string RobCitizen(Citizen citizen, Thief thief)
        {
            string result = "Tjuven ser inget av värde att råna av medborgaren";
            
            // kollar om medborgaren har något att råna
            if(citizen.Belongings.Count > 0)
            {
                robbedCitizens++;
                var itemIndex = random.Next(0, citizen.Belongings.Count);
                thief.StolenGoods.Add(citizen.Belongings[itemIndex]);
                result = "Tjuven rånar medborgaren på " + citizen.Belongings[itemIndex].Name;
                citizen.Belongings.RemoveAt(itemIndex);
            }
            
            return result;
        }
        
        
        static string ArrestThief(Thief thief, Police police)
        {
            string result = "Polisen konfiskerar tjuvens stöldgods";
            arrestedThiefs++;


            police.Confiscated.AddRange(thief.StolenGoods);
            thief.StolenGoods.Clear();

            prison.Add(thief);

            return result;
        }

        static void Jail()
        {
            foreach (var prisoner in prison.ToList())
            {
                if (prisoner.timeInJail < 30)
                    prisoner.timeInJail++;
                else
                {
                    Console.WriteLine("Fånge friad");
                    prisoner.timeInJail = 0;
                    prison.Remove(prisoner);
                    Thread.Sleep(1000);
                }


            }
        }
    }

    class Person
    {
        public Person()
        {
            Random rnd = new Random();
            do
            {
                MovementX = rnd.Next(-1, 1);
                MovementY = rnd.Next(-1, 1);
            } while (MovementX == 0 && MovementY == 0);

            PositionX = rnd.Next(0, 100);
            PositionY = rnd.Next(0, 25);
        }

        public int MovementX { get; set; }
        public int MovementY { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }

    class Thief : Person
    {
        public Thief()
        {
            Type = "T";
            Name = "Tjuven";
        }
        public List<Sak> StolenGoods = new List<Sak>();
        public int timeInJail { get; set; } = 0;

    }

    class Police : Person
    {
        public Police()
        {
            Type = "P";
            Name = "Polisen";
        }

        public List<Sak> Confiscated = new List<Sak>();
    }

    class Citizen : Person
    {
        public Citizen()
        {
            Type = "M";
            Name = "Medborgaren";
            Belongings.Add(new Sak("Nycklar"));
            Belongings.Add(new Sak("Mobil"));
            Belongings.Add(new Sak("Pengar"));
            Belongings.Add(new Sak("Klocka"));
        }

        public List<Sak> Belongings = new List<Sak>();
    }

    class Sak
    {
        public Sak(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
