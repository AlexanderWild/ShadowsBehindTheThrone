using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    //[Serializable,HideInInspector]
    public class StatRecorder
    {
        public Map map;

        public List<StatSnapshot> snapshots = new List<StatSnapshot>();
        public float[][][] prevColours;

        public StatRecorder() { }
        public StatRecorder(Map map)
        {
            this.map = map;
        }

        public void turnTick()
        {
            StatSnapshot snapshot = new StatSnapshot();
            snapshot.day = map.turn;
            snapshots.Add(snapshot);

            if (prevColours == null)
            {
                prevColours = new float[map.sizeX][][];
                for (int i = 0; i < prevColours.Length; i++)
                {
                    prevColours[i] = new float[map.sizeY][];
                    for (int j = 0; j < prevColours[i].Length; j++)
                    {
                        prevColours[i][j] = new float[3];
                    }
                }
            }
            float[][][] colours = new float[map.sizeX][][];
            for (int i = 0; i < colours.Length; i++)
            {
                colours[i] = new float[map.sizeY][];
                for (int j = 0; j < colours[i].Length; j++)
                {
                    colours[i][j] = new float[3];
                }
            }

            for (int i = 0; i < map.sizeX; i++)
            {
                for (int j = 0; j < map.sizeY; j++)
                {
                    bool delta = false;
                    if (map.landmass[i][j] == false)
                    {
                        colours[i][j][ 0] = 0;
                        colours[i][j][ 1] = 0;
                        colours[i][j][ 2] = 0.25f;
                    }
                    else if (map.grid[i][j].owner == null)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            colours[i][j][ k] = map.grid[i][j].purity;
                        }
                    } else {
                        for (int k = 0; k < 3; k++) {
                            colours[i][j][ k] = map.grid[i][j].owner.color[k] * map.grid[i][j].purity;
                        }
                    }
                    for (int k = 0; k < 3; k++)
                    {
                        if (colours[i][j][ k] != prevColours[i][j][ k]) { delta = true; }
                    }
                    if (delta)
                    {
                        snapshot.hexColIndices.Add(new int[] { i, j });
                        snapshot.hexColValues.Add(new float[] { colours[i][j][ 0], colours[i][j][ 1], colours[i][j][ 2] });
                    }
                }
            }
            prevColours = colours;
        }
    }
}
