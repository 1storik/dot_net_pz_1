//using static System.Console;

//namespace pz11
//{
//    internal class Class2
//    {
//        public void SolveGame()
//        {
//            int minmax = Data[0, 0];
//            int maxmin = Data[0, 0];
//            for (int i = 0; i < n; i++)
//            {
//                int min = Data[i, 0];
//                for (int j = 0; j < m; j++)
//                {
//                    if (Data[i, j] <= min)
//                        min = Data[i, j];
//                }
//                if (i == 0)
//                    minmax = min;
//                if (minmax <= min)
//                    minmax = min;
//            }
//            for (int i = 0; i < m; i++)
//            {
//                int max = Data[0, i];
//                for (int j = 0; j < n; j++)
//                {
//                    if (Data[j, i] >= max)
//                        max = Data[j, i];
//                }
//                if (i == 0)
//                    maxmin = max;
//                if (maxmin >= max)
//                    maxmin = max;
//            }
//            WriteLine("Minmax: " + minmax + ", Maxmin: " + maxmin);
//            WriteLine("This game was solved with clear strategy: " + (maxmin == minmax));
//            if (maxmin == minmax)
//                WriteLine("V: " + maxmin);
//            else if (maxmin != minmax)
//            {
//                int[,] B = Adj(Data);

//                int[] X = XY(T(B));
//                int[] Y = XY(B);
//                int v = V(B);
//                Write("X: ");
//                for (int i = 0; i < X.Length; i++)
//                {
//                    Write(X[i] + " ");
//                }
//                WriteLine();
//                Write("Y: ");
//                for (int i = 0; i < Y.Length; i++)
//                {
//                    Write(Y[i] + " ");
//                }
//                WriteLine();
//                WriteLine("V: " + v);
//            }
//        }
//    }
//}
