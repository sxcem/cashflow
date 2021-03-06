﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{

    public static string FormatNumberCommas(int num)
    {
        return string.Format("{0:n0}", num);
    }

    public static string FormatMoney(int money)
    {
        return string.Format("${0:n0}", money);
    }

    public static void ShuffleList<T>(List<T> list, int passes = 1)
    {
        for (int i = 0; i < list.Count * passes; i++)
        {
            Swap(list, Random.Range(0, list.Count), Random.Range(0, list.Count));
        }
    }

    public static void Swap<T>(List<T> list, int i, int j)
    {
        T temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }

    public static int RandomSign()
    {
        return Random.Range(0f, 1f) < 0.5 ? 1 : -1;
    }

    public static int RoundToNearestPowerOfTen(int n, int pow)
    {
        int power = (int)Mathf.Pow(10, pow);
        return n % power >= (power / 2) ? n + power - n % power : n - n % power;
    }

}
