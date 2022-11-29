using System;
using System.Collections.Generic;
using System.Linq;
using DataSender.Algorithms.Models;

namespace DataSender.Algorithms;

public class FanoAlgorithm
{
    private List<char> Alphabit;
    private int g = 0;
    private int m;
    private List<float> Probabilities;

    private double schet1;
    private double schet2;
    private List<FanoData> Table;

    public FanoAlgorithm(string message)
    {
        Alphabit = new List<char>();
        Probabilities = new List<float>();
        CalculateProbabilitiesAndAlphabit(message);
        Res = new string[Alphabit.Count];
        Sort();
        Fano(0, Alphabit.Count - 1);
    }

    public string[] Res { get; }

    public List<FanoData> ReturnTable()
    {
        Table = new List<FanoData>();
        if (Alphabit.Count == 1)
        {
            Table.Add(new FanoData(
                Alphabit[0],
                1,
                0,
                "0"
            ));
            return Table;
        }
        for (var i = 0; i < Alphabit.Count; i++)
            Table.Add(new FanoData(
                Alphabit[i],
                Probabilities[i],
                Res[i].Count(),
                Res[i]
            ));
        return Table;
    }

    private void CalculateProbabilitiesAndAlphabit(string message)
    {
        Alphabit = InfoAboutCode.FindAlphabet(message);
        Probabilities = InfoAboutCode.CalculateProbabilities(message);
    }

    private void Sort()
    {
        for (var i = 0; i < Probabilities.Count; i++)
            for (var j = 0; j < Probabilities.Count - i - 1; j++)
                if (Probabilities[j] < Probabilities[j + 1])
                {
                    char temp2;
                    float temp1 = 0;

                    temp1 = Probabilities[j];
                    temp2 = Alphabit[j];
                    Probabilities[j] = Probabilities[j + 1];
                    Alphabit[j] = Alphabit[j + 1];
                    Probabilities[j + 1] = temp1;
                    Alphabit[j + 1] = temp2;
                }
    }

    private int Delenie_Posledovatelnosty(int L, int R)
    {
        schet1 = 0;
        for (var i = L; i <= R - 1; i++) schet1 = schet1 + Probabilities[i];

        schet2 = Probabilities[R];
        m = R;
        while (schet1 >= schet2)
        {
            m = m - 1;
            schet1 = schet1 - Probabilities[m];
            schet2 = schet2 + Probabilities[m];
        }

        return m;
    }

    private void Fano(int L, int R)
    {
        int n;

        if (L < R)
        {
            n = Delenie_Posledovatelnosty(L, R);
            for (var i = L; i <= R; i++)
                if (i <= n)
                    Res[i] += Convert.ToByte(0);
                else
                    Res[i] += Convert.ToByte(1);

            Fano1(L, n);

            Fano(n + 1, R);
        }
    }

    private void Fano1(int L, int R)
    {
        int n;

        if (L < R)
        {
            n = Delenie_Posledovatelnosty(L, R);
            Console.WriteLine(n);
            for (var i = L; i <= R; i++)
                if (i <= n)
                    Res[i] += Convert.ToByte(0);
                else
                    Res[i] += Convert.ToByte(1);

            Fano(L, n);

            Fano1(n + 1, R);
        }
    }
}