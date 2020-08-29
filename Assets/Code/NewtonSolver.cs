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

		public class Node
		{
			public T element;
			public Vector3 velocity = Vector3.zero;

			public Node(T e) { element = e; }
		}

		public const float SPRING = 0.001f;
		public const float DAMPING = 0.8f;
		public const float REPULSION_DISTANCE = 0.2f;

		public List<Node> nodes = new List<Node>();
		public IDistanceMetric metric;

		public NewtonSolver(List<T> ns, IDistanceMetric m)
		{
			metric = m;
			foreach (T n in ns)
			{
				Vector3 p = new Vector3();
				p.x = (float)Eleven.random.NextDouble();
				p.y = (float)Eleven.random.NextDouble();

				n.transform.position = p;
				nodes.Add(new Node(n));
			}
		}

		public void solve()
		{
			for (int i = 0; i != nodes.Count - 1; ++i)
			{
				for (int j = i + 1; j != nodes.Count; ++j)
				{
					T a = nodes[i].element;
					T b = nodes[j].element;

					Vector3 dist = (b.transform.position - a.transform.position);
					float magsqr = dist.sqrMagnitude;

					if (magsqr != 0.0f && magsqr < REPULSION_DISTANCE)
					{
						nodes[i].velocity += dist / magsqr * SPRING;
						nodes[j].velocity -= dist / magsqr * SPRING;
					}
				}
			}

			foreach (Node n in nodes)
			{
				n.element.transform.position += n.velocity;
				n.velocity *= DAMPING;
			}
		}
	}
}
