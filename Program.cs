using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Tjuv_och_Polis
{
    class Program
    {
        static int yAxisLength = 25;
        static int xAxisLength = 100;
        static int arrestedThiefs = 0;
        static int robbedCitizens = 0;
        static List<Person> people = new List<Person>();
        static List<Thief> prison = new List<Thief>();

        static void Main(string[] args)
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
        }

        static string Meet(Person p1, Person p2)
        {
            string result = "";
            Random rnd = new Random();

            if (p1 is Thief && p2 is Citizen)
            {
                robbedCitizens++;
                var itemIndex = rnd.Next(0, ((Citizen)p2).Belongings.Count -1);
                ((Thief)p1).StolenGoods.Add(((Citizen)p2).Belongings[itemIndex]);
                result = "Tjuven rånar medborgaren på " + ((Citizen)p2).Belongings[itemIndex].Name;

                ((Citizen)p2).Belongings.RemoveAt(itemIndex);
            }
            else if (p1 is Citizen && p2 is Thief)
            {
                robbedCitizens++;
                var itemIndex = rnd.Next(0, ((Citizen)p1).Belongings.Count -1);
                ((Thief)p2).StolenGoods.Add(((Citizen)p1).Belongings[itemIndex]);
                result = "Tjuven rånar medborgaren på " + ((Citizen)p1).Belongings[itemIndex].Name;

                ((Citizen)p1).Belongings.RemoveAt(itemIndex);

            }
            else if (p1 is Thief && p2 is Police || p1 is Police && p2 is Thief)
            {
                result = "Polisen konfiskerar tjuvens stöldgods";
                arrestedThiefs++;
                if (p1 is Police)
                {
                    ((Police)p1).Confiscated.AddRange(((Thief)p2).StolenGoods);
                    ((Thief)p2).StolenGoods.Clear();

                    //
                    prison.Add((Thief)p2);
                }
                else
                {
                    ((Police)p2).Confiscated.AddRange(((Thief)p1).StolenGoods);
                    ((Thief)p1).StolenGoods.Clear();

                    prison.Add((Thief)p1);
                }
            }

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
