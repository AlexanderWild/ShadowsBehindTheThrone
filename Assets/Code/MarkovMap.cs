using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class MarkovModel
    {
        public double[] p0 = new double[26];
        public double[] sum0 = new double[26];
        public double count0;

        //1st order
        public double[,] p1 = new double[26, 27];//The spare char is terminate
        public double[,] sum1 = new double[26, 27];
        public double[] count1 = new double[26];
        //2nd order
        public double[,,] p2 = new double[26, 26, 27];//The spare char is terminate
        public double[,,] sum2 = new double[26, 26, 27];
        public double[,] count2 = new double[26, 26];


        public string alpha = "abcdefghijklmnopqrstuvwxyz";

        public string capFirst(string word)
        {
            string firstLetter = "" + word[0];
            firstLetter = firstLetter.ToUpper();
            string rest = word.Substring(1);
            return firstLetter + rest;
        }

        public void buildModel(string[] words)
        {
            foreach (string word in words)
            {
                if (word.Length < 3) { continue; }

                string w = word.ToLower();

                int firstLetter = id(w[0]);
                if (firstLetter == -1)
                {
                    throw new Exception("" + firstLetter + " " + w + " " + w[0]);
                }
                sum0[firstLetter] += 1;
                count0 += 1;

                int prevLetter = -1;
                int ppLetter = -1;
                for (int i = 0; i < w.Length; i++)
                {
                    int letter = id(w[i]);
                    if (prevLetter != -1)
                    {
                        sum1[prevLetter, letter] += 1;
                        count1[prevLetter] += 1;
                    }
                    if (ppLetter != -1)
                    {
                        sum2[ppLetter, prevLetter, letter] += 1;
                        count2[ppLetter, prevLetter] += 1;
                    }
                    ppLetter = prevLetter;
                    prevLetter = letter;
                }
                sum1[prevLetter, 26] += 1;
                count1[prevLetter] += 1;
                if (ppLetter != -1)
                {
                    sum2[ppLetter, prevLetter, 26] += 1;
                    count2[ppLetter, prevLetter] += 1;
                }
            }

            for (int i = 0; i < 26; i++)
            {
                p0[i] = sum0[i] / count0;

                if (count1[i] == 0) { continue; }
                for (int j = 0; j < 27; j++)
                {
                    p1[i, j] = sum1[i, j] / count1[i];
                }
            }
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 26; j++)
                {
                    if (count2[i, j] == 0) { continue; }
                    for (int k = 0; k < 27; k++)
                    {
                        p2[i, j, k] = sum2[i, j, k] / count2[i, j];
                    }
                }
            }
        }

        public int id(char c)
        {
            int reply = alpha.IndexOf(c);
            if (reply == -1) { return 0; }
            return reply;
        }

        public string getWord()
        {
            int letter = 0;
            double roll = Eleven.random.NextDouble();
            for (int i = 0; i < 26; i++)
            {
                roll -= p0[i];
                if (roll <= 0)
                {
                    letter = i;
                    break;
                }
            }

            string word = "";

            word += alpha[letter];
            int a = -1;
            int b = -1;
            for (int i = 0; i < 9; i++)
            {
                bool canEnd = i > 2;
                a = b;
                b = letter;
                letter = getLetter(a, b, canEnd);

                double pEnd = getPEnd(a, b);

                if (letter == 26 || (Eleven.random.NextDouble() < pEnd && canEnd))
                {
                    break;
                }
                else
                {
                    word += alpha[letter];
                }
            }
            return word;
        }

        public int getLetter(int a, int b, bool canEnd)
        {
            for (int t = 0; t < 8; t++)
            {

                double roll;
                int chosen = 26;
                if (a != -1 && count2[a, b] != 0)
                {
                    //Do second order
                    roll = Eleven.random.NextDouble();
                    for (int j = 0; j < 27; j++)
                    {
                        roll -= p2[a, b, j];
                        if (roll <= 0)
                        {
                            chosen = j;
                            break;
                        }
                    }

                    if (canEnd || chosen != 26)
                    {
                        return chosen;
                    }
                }

                //Second order failed, down to first
                roll = Eleven.random.NextDouble();
                for (int j = 0; j < 27; j++)
                {
                    roll -= p1[b, j];
                    if (roll <= 0) { chosen = j; break; }
                }


                if (canEnd || chosen != 26)
                {
                    return chosen;
                }
            }
            return 0;
        }

        public double getPEnd(int a, int b)
        {
            if (a != -1 && count2[a, b] != 0)
            {
                return p2[a, b, 26];
            }
            return p1[b, 26];
        }
    }
}
