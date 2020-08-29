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
		public const float DAMPING = 0.8f;
		public const float REPULSION_DISTANCE = 0.2f;

		public List<T> nodes;
		public IDistanceMetric metric;

		public NewtonSolver(List<T> ns, IDistanceMetric m)
		{
			nodes = ns;
			metric = m;

			foreach (T n in nodes)
			{
				Vector3 p = new Vector3();
				p.x = (float)Eleven.random.NextDouble();
				p.y = (float)Eleven.random.NextDouble();

				n.transform.position = p;
			}
		}

		public void solve()
		{
			for (int i = 0; i != nodes.Count; ++i)
			{
				T a = nodes[i];
				Vector3 force = Vector3.zero;

				for (int j = 0; j != nodes.Count; ++j)
				{
					T b = nodes[j];
					if (a == b)
						continue;

					Vector3 dist = (b.transform.position - a.transform.position);
					float magsqr = dist.sqrMagnitude;

					if (magsqr != 0.0f && magsqr < REPULSION_DISTANCE)
						force += dist / magsqr * SPRING;
				}

				a.transform.position += force * DAMPING;
			}
		}
	}
}
