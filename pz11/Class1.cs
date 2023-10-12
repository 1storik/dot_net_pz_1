using System.Numerics;
using static System.Console;

namespace lab1
{
    internal class ParallelMatrix
    {
        public int[,] Data;
        int n;
        Random random = new Random();

        private Semaphore semaphore;
        object objLock = new object();
        public ParallelMatrix(int n, int countOfThreads)
        {
            Data = new int[n, n];
            this.n = n;
            semaphore = new Semaphore(countOfThreads, countOfThreads);

        }
        public ParallelMatrix(int[,] matrix, int countOfThreads)
        {
            this.Data = matrix;
            this.n = matrix.GetLength(0);
            semaphore = new Semaphore(countOfThreads, countOfThreads);
        }
        public void FillIn()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Data[i, j] = random.Next(10);
                }
            }
        }
        public void Print(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    Write(matrix[i, j] + "\t");
                }
                WriteLine();
            }
        }
        public BigInteger Determinant()
        {
            if (Data.GetLength(0) == 1)
            {
                return Data[0, 0];
            }
            else if (Data.GetLength(0) == 2)
            {
                return Data[0, 0] * Data[1, 1] - Data[0, 1] * Data[1, 0];
            }
            else
            {
                BigInteger determ = 0;
                int length = Data.GetLength(0);
                Queue<Thread> queueInRun = new Queue<Thread>();
                for (int i = 0; i < length; i++)
                {
                    int k = i;
                    Thread thred = new Thread(() =>
                    {
                        semaphore.WaitOne();
                        BigInteger r1 = 1;
                        BigInteger r2 = 1;
                        for (int j = 0; j < length; j++)
                        {
                            if (j + k >= length)
                            {
                                r1 *= Data[j, j + k - length];
                                r2 *= Data[j, length * 2 - k - j - 1];
                            }
                            else
                            {
                                r1 *= Data[j, j + k];
                                r2 *= Data[j, length - k - j - 1];
                            }
                        }
                        lock (objLock)
                        {
                            determ += r1;
                            determ -= r2;
                        }
                        semaphore.Release();

                    });
                    thred.Start();
                    queueInRun.Enqueue(thred);
                }
                while (queueInRun.Count > 0)
                {
                    Thread thread = queueInRun.Peek();
                    if (!thread.IsAlive)
                        queueInRun.Dequeue();
                }
                return determ;

            }
        }
    }
}