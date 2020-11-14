using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Kmeans
    {
        public static double[][] run(List<int[]> points,int nMeans)
        {
            double[][] means = new double[nMeans][];
            bool[] used = new bool[points.Count];
            //Assign means randomly
            for (int i = 0; i < means.Length; i++)
            {
                int c = 0;
                int chosenIndex = -1;
                for (int j = 0; j < points.Count; j++)
                {
                    if (used[j]) { continue; }
                    c += 1;
                    if (Eleven.random.Next(c) == 0)
                    {
                        chosenIndex = j;
                    }
                }
                means[i] = new double[] { points[chosenIndex][0], points[chosenIndex][1] };
            }
            for (int cycle = 0; cycle < 16; cycle++)
            {
                double[,] sums = new double[nMeans,2];
                int[] counts = new int[nMeans];
                foreach (int[] loc in points)
                {
                    int m = getClosest(loc[0], loc[1], means);
                    sums[m,0] += loc[0];
                    sums[m, 1] += loc[1];
                    counts[m] += 1;
                }
                for (int m = 0; m < counts.Length; m++)
                {
                    if (counts[m] > 0)
                    {
                        means[m][0] = sums[m, 0] / counts[m];
                        means[m][1] = sums[m, 1] / counts[m];
                    }
                    else
                    {
                        //Random jump to random point within dimension bounds (crossover of two points)
                        means[m][0] = points[Eleven.random.Next(points.Count)][0];
                        means[m][1] = points[Eleven.random.Next(points.Count)][1];
                    }
                }
            }
            return means;
        }

        public static int getClosest(int x,int y, double[][] means)
        {
            double bestDist = 0;
            int bestM = -1;
            for (int m = 0; m < means.Length; m++)
            {
                double dist = Math.Abs(means[m][0] - x) + Math.Abs(means[m][1] - y);
                if (bestM == -1 || dist < bestDist)
                {
                    bestM = m;
                    bestDist = dist;
                }
            }
            return bestM;
        }
    }
}
