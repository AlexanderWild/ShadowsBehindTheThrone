using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Assets.Code
{
	public class NewtonSolver<T> where T : MonoBehaviour
	{
		public interface IDistanceMetric
		{
			double getDistance(T a, T b);
		}

		public const float SPRING = 0.01f;
		public const float REPULSION_DISTANCE = 0.5f;

		public List<T> nodes;
		public IDistanceMetric metric;

		public NewtonSolver(List<T> ns, IDistanceMetric m)
		{
			nodes = ns;
			metric = m;

			foreach (T n in nodes)
				n.transform.position = Vector3.zero;
		}

		public void solve()
		{
			for (int i = 0; i != nodes.Count - 1; ++i)
			{
				for (int j = i + 1; j != nodes.Count; ++j)
				{
					T a = nodes[i];
					T b = nodes[j];

					//
				}
			}
		}
	}
}
