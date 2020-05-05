using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EigthQueens
{
    public class Population
    {
        double MutationProbability { get; set; }
        int PopulationSize { get; set; }
        int Parents { get; set; }
        int MaxEval { get; set; }
        int BoardSize { get; set; }
        public int CurrentEvaluation { get; set; }
        List<Subject> Subjects { get; set; }
        static Random random = new Random();
        public List<GenerationData> Generations { get; set; }
        const int MaxValue = 28;

        public Population(int populationSize, int boardSize, int maxEval, double mutationProbability, int parents)
        {
            PopulationSize = populationSize;
            BoardSize = boardSize;
            MaxEval = maxEval;
            MutationProbability = mutationProbability;
            Parents = parents;

            Subjects = new List<Subject>();
            Generations = new List<GenerationData>();
            GenerateRandomPopulation();
        }

        void GenerateRandomPopulation()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                Subject tempSubject = new Subject(BoardSize);
                tempSubject.Board = RandomGenerator();
                tempSubject.CalculateFitnessValue();
                Subjects.Add(tempSubject);
            }
            OrderList();
            CurrentEvaluation = PopulationSize;
        }

        void OrderList()
        {
            Subjects = Subjects.OrderBy(a => a.FitnessValue).ToList();
        }
        int[] RandomGenerator()
        {
            int[] result = new int[BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                result[i] = i;
            }
            result = result.OrderBy(x => random.Next()).ToArray();
            return result;
        }

        Subject GetRandomParent()
        {
            return Subjects[random.Next(0, PopulationSize)];
        }

        Subject GetChild(Subject ParentA, Subject ParentB)
        {
            Subject child = new Subject(BoardSize);
            for (int i = 0; i < BoardSize; i++)
            {
                if (i < 4)
                {
                    for (int j = 0; j < BoardSize; j++)
                    {
                        if (!child.Board.Contains(ParentA.Board[j]))
                        {
                            child.Board[i] = ParentA.Board[j];
                            break;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < BoardSize; j++)
                    {
                        if (!child.Board.Contains(ParentB.Board[j]))
                        {
                            child.Board[i] = ParentB.Board[j];
                            break;
                        }
                    }
                }
            }
            CurrentEvaluation++;
            return child;
        }
        void ReplaceWorst(Subject ChildA, Subject ChildB)
        {
            Subjects.Remove(Subjects.First());
            Subjects.Remove(Subjects.First());

            ChildA.CalculateFitnessValue();
            ChildB.CalculateFitnessValue();
            Subjects.Add(ChildA);
            Subjects.Add(ChildB);
            OrderList();
        }

        void Reproduction()
        {
            List<Subject> parents = new List<Subject>();
            for (int i = 0; i < Parents; i++)
            {
                parents.Add(GetRandomParent());
            }
            parents = parents.OrderBy(a => a.FitnessValue).ToList();

            Subject childA = GetChild(parents[0], parents[1]);
            Subject childB = GetChild(parents[1], parents[0]);
            if (random.NextDouble() <= MutationProbability)
            {
                childA = Mutate(childA);
            }

            if (random.NextDouble() <= MutationProbability)
            {
                childB = Mutate(childB);
            }

            ReplaceWorst(childA, childB);
        }

        Subject Mutate(Subject Child)
        {
            int changeValueA = random.Next(0, BoardSize); 
            int changeValueB = random.Next(0, BoardSize);
            int tempValue = Child.Board[changeValueA];
            while (changeValueA == changeValueB)
            {
                changeValueB = random.Next(0, BoardSize);
            }
            Child.Board[changeValueA] = Child.Board[changeValueB];
            Child.Board[changeValueB] = tempValue;

            return Child;
        }

        public Subject ObtainBestSubject()
        {
            Subject subject = Generations.Last().BetterSubject;
            return subject;
        }

        public void StartEvolutionProcess()
        {
            int generationCont = 0;
            while (CurrentEvaluation < MaxEval)
            {
                GenerationData generation = new GenerationData(generationCont + 1, Subjects);
                Generations.Add(generation);
                generationCont++;
                if (generation.BetterSubject.FitnessValue == MaxValue)
                {
                    Console.WriteLine("Found");
                    break;
                }

                Reproduction();
            }
        }
    }
}
