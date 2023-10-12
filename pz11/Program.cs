using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading;
using static System.Console;

namespace lab1
{
    internal class PaymentMatrix
    {
        public int[,] Data;
        int n;
        int m;
        int minmax;
        int maxmin;
        Random random = new Random();
        object objLock = new object();
        int maxThreads;
        public PaymentMatrix(int n, int m, int maxThreads)
        {
            this.maxThreads = maxThreads;
            Data = new int[n, m];
            this.n = n;
            this.m = m;
            RandMatrixFill();
        }
        public PaymentMatrix(int[,] matrix, int maxThreads)
        {
            this.maxThreads = maxThreads;
            this.Data = matrix;
            this.n = matrix.GetLength(0);
            this.m = matrix.GetLength(1);
            RandMatrixFill();
        }
        public void RandMatrixFill()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Data[i, j] = random.Next(-3, 3);
                }
            }
        }
        public void Print(int[,] Data)
        {
            for (int i = 0; i < Data.GetLength(0); i++)
            {
                for (int j = 0; j < Data.GetLength(1); j++)
                {
                    Write(Data[i, j] + "\t");
                }
                WriteLine();
            }
        }
        private void MM(int i)
        {
            int min = Data[0, 0];
            int max = Data[0, 0];
            for (int j = 0; j < m; j++)
            {
                if (Data[i, j] <= min)
                    min = Data[i, j];
                if (Data[j, i] >= max)
                    max = Data[j, i];
            }
            if (i == 0)
                minmax = min;
            if (minmax <= min)
                minmax = min;
            if (i == 0)
                maxmin = max;
            if (maxmin >= max)
                maxmin = max;
        }
        public void SolveGameParallel()
        {
            int length = Data.GetLength(0);
            Queue<Thread> queueInRun = new Queue<Thread>();
            int step = (int)Math.Ceiling((double)length / maxThreads);
            for (int i = 0; i < step * maxThreads; i += step)
            {
                int k = i;
                Thread thread = new Thread(() =>
                {
                    WriteLine("Thread" + k + " come in");
                    for (int j = 0; j < step && length > 0; j++)
                    {
                        lock (objLock)
                        {
                            length--;
                        }
                        MM(k + j);
                    }
                    Thread.Sleep(1000);
                    WriteLine("Thread" + k + " come out");

                });
                thread.Start();
                queueInRun.Enqueue(thread);
            }
            while (queueInRun.Count > 0)
            {
                Thread thread = queueInRun.Peek();
                if (!thread.IsAlive)
                    queueInRun.Dequeue();
            }           

            WriteLine("Minmax: " + minmax + ", Maxmin: " + maxmin);
            WriteLine("This game was solved with clear strategy: " + (maxmin == minmax));
            if (maxmin == minmax)
                WriteLine("V: " + maxmin);
            else if (maxmin != minmax)
            {
                int[,] B = Adj(Data);

                int[] X = XY(T(B));
                int[] Y = XY(B);
                int v = V(B);
                Write("X: ");
                for (int i = 0; i < X.Length; i++)
                {
                    Write(X[i] + " ");
                }
                WriteLine();
                Write("Y: ");
                for (int i = 0; i < Y.Length; i++)
                {
                    Write(Y[i] + " ");
                }
                WriteLine();
                WriteLine("V: " + v);
            }
        }
        private int V(int[,] matrix)
        {
            int determ = Determinant(matrix);
            int d = Sum(matrix);
            if (d != 0)
                return determ / d;
            else return 0;
        }
        private int[] XY(int[,] matrix)
        {
            int[] vector = new int[matrix.GetLength(0)];
            int d = Sum(matrix);
            for (int i = 0; i < n; i++)
            {
                int sum = 0;
                for (int j = 0; j < m; j++)
                {
                    sum += matrix[i, j];
                }
                if (sum != 0)
                {
                    try
                    {
                        vector[i] = sum / d;
                    }
                    catch (Exception e)
                    {
                        WriteLine(e.Message);
                    }                    
                }
            }
            return vector;
        }
        private int Sum(int[,] matrix)
        {
            int sum = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    sum += matrix[i, j];
                }
            }
            return sum;
        }
        private int[,] Adj(int[,] matrix)
        {
            int[,] adjMatrix = new int[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    adjMatrix[i, j] = A(matrix, i, j);
                }
            }
            return T(adjMatrix);
        }
        private int[,] T(int[,] matrix)
        {
            int[,] tMatrix = new int[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    tMatrix[j, i] = matrix[i, j];
                }
            }
            return tMatrix;
        }
        private int Determinant(int[,] matrix)
        {
            if (matrix.GetLength(0) == 1)
            {
                return matrix[0, 0];
            }
            else if (matrix.GetLength(0) == 2)
            {
                return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
            }
            else
            {
                int determ = 0;
                int length = matrix.GetLength(0);
                for (int i = 0; i < length; i++)
                {
                    int r1 = 1;
                    int r2 = 1;
                    for (int j = 0; j < length; j++)
                    {
                        if (j + i >= length)
                        {
                            r1 *= matrix[j, j + i - length];
                            r2 *= matrix[j, length * 2 - i - j - 1];
                        }
                        else
                        {
                            r1 *= matrix[j, j + i];
                            r2 *= matrix[j, length - i - j - 1];
                        }
                    }
                    determ += r1;
                    determ -= r2;
                }
                return determ;

            }
        }
        private int A(int[,] matrix, int i, int j)
        {
            WriteLine(Determinant(GetSubMatrix(matrix, i, j)));
            return (int)Math.Pow(-1, i + j) * Determinant(GetSubMatrix(matrix, i, j));
        }
        private int[,] GetSubMatrix(int[,] matrix, int i, int j)
        {
            int n = matrix.GetLength(0) - 1;
            int[,] subMatrix = new int[n, n];
            int m, l = 0;
            for (int h = 0; h < matrix.GetLength(0); h++)
            {
                m = 0;
                if (h != i)
                {
                    for (int k = 0; k < matrix.GetLength(0); k++)
                    {
                        if (k != j)
                        {
                            subMatrix[l, m] = matrix[h, k];
                            m++;
                        }
                    }
                    l++;
                }
            }
            //Print(subMatrix);
            //Console.WriteLine("------------------------------");
            return subMatrix;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int n = 3; // Replace with your desired value
            int m = 3; // Replace with your desired value
            var t1 = new Stopwatch();
            t1.Start();
            for (int i = 0; i < 3; i++)
            {
                PaymentMatrix paymentMatrix = new PaymentMatrix(n, m, 3);
                paymentMatrix.Print(paymentMatrix.Data); // Print the initial matrix
                //WriteLine("Solving the game:");
                paymentMatrix.SolveGameParallel();

                WriteLine(new string('-', 50));
            }
            t1.Stop();
            WriteLine(t1.Elapsed);

            ReadLine(); // Keep the console open
        }
    }
}
