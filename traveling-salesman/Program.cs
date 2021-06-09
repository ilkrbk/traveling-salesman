using System;
using System.Collections.Generic;
using System.IO;

namespace traveling_salesman
{
    class Program
    {
        static void Main(string[] args)
        {
            List<(int, int)> edgesList = Read("test.txt");
            Console.WriteLine("Координаты точек\n№ | X Y\n-------");
            for (int i = 0; i < edgesList.Count; i++)
                Console.WriteLine($"{i + 1} | {edgesList[i].Item1} {edgesList[i].Item2}");
            CPath path = new CPath(edgesList);
            List<(int[], double)> result = new List<(int[], double)>();
            while (result.Count < Factorial(edgesList.Count - 2))
            {
                path.FindBestPath();
                if (Check(result, path.PathLength()))
                {
                    result.Add((path.Path, path.PathLength()));
                }
            }
            Console.WriteLine("Матрица");
            for (int i = 0; i < edgesList.Count; i++)
            {
                for (int j = 0; j < edgesList.Count; j++)
                    Console.Write($"{Math.Round(path.distance[i,j], 3),8}");
                Console.WriteLine();
            }
            result.Sort();
            Console.WriteLine("Путь по точкам");
            double count = 0;
            for (int i = 0; i < result[0].Item1.Length; i++)
                Console.Write($"{result[0].Item1[i] + 1,3}");
            Console.WriteLine($"\nСтоймость пути {result[0].Item2}");
        }
        static int Factorial(int x)
        {
            if (x == 0)
            {
                return 1;
            }
            else
            {
                return x * Factorial(x - 1);
            }
        }
        static List<(int, int)> Read(string path)
        {
            List<(int, int)> list = new List<(int, int)>();
            StreamReader read = new StreamReader(path);
            string[] size = read.ReadLine()?.Split(' ');
            if (size != null)
            {
                int sizeGraph = Convert.ToInt32(size[0]);
                for (int i = 0; i < sizeGraph; ++i)
                {
                    size = read.ReadLine()?.Split(' ');
                    if (size != null)
                        list.Add((Convert.ToInt32(size[0]), Convert.ToInt32(size[1])));
                }
            }

            return list;
        }
        static bool Check(List<(int[], double)> list, double count)
        {
            foreach (var item in list)
            {
                if (item.Item2 == count)
                {
                    return false;
                }
            }
            return true;
        }
    }
    class CPath
    {
        //расстояния между городами
        public double[,] distance;
        //индексы городов формируют искомый путь
        public int[] Path;
        public CPath(List<(int, int)> map)
        {
            distance = new double[map.Count, map.Count];
            for (int j = 0; j < map.Count; j++)
            {
                distance[j, j] = 0;

                for (int i = 0; i < map.Count; i++)
                {
                    double value = Math.Sqrt(Math.Pow(map[i].Item1 - map[j].Item1, 2) + Math.Pow(map[i].Item2 - map[j].Item2, 2));
                    distance[i, j] = distance[j, i] = value;
                }
            }
            //создаем начальный путь
            //массив на 1 больше кол-ва городов, а первый и последний индексы равны 0 - это сделано для того чтобы "замкнуть" путь
            Path = new int[map.Count + 1];
            for (int i = 0; i < map.Count; i++)
            {
                Path[i] = i;
            }
            Path[map.Count] = 0;
        }
        //метод, реулизующий алгоритм поиска оптимального пути
        public void FindBestPath()
        {
            Random random = new Random();
            for (int fails = 0, F = Path.Length * Path.Length; fails < F; )
            {
                //выбираем два случайных города
                //первый и последний индексы не трогаем
                int p1 = 0, p2 = 0;
                while (p1 == p2)
                {
                    p1 = random.Next(1, Path.Length - 1);
                    p2 = random.Next(1, Path.Length - 1);
                }
                //проверка расстояний
                double sum1 = distance[Path[p1 - 1], Path[p1]] + distance[Path[p1], Path[p1 + 1]] +
                              distance[Path[p2 - 1], Path[p2]] + distance[Path[p2], Path[p2 + 1]];
                double sum2 = distance[Path[p1 - 1], Path[p2]] + distance[Path[p2], Path[p1 + 1]] +
                              distance[Path[p2 - 1], Path[p1]] + distance[Path[p1], Path[p2 + 1]];

                if (sum2 < sum1)
                {
                    int temp = Path[p1];
                    Path[p1] = Path[p2];
                    Path[p2] = temp;
                }
                else
                {
                    fails++;
                }
            }
        }
        //возвращает длину пути
        public double PathLength()
        {
            double pathSum = 0;
            for (int i = 0; i < Path.Length - 1; i++)
            {
                pathSum += distance[Path[i], Path[i + 1]];
            }
            return pathSum;
        }
    }
}