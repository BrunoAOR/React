using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utils {

	static public string GetSignedStringFromNumber (float number) {
		return (string.Format ("{0}{1}", number < 0 ? "" : "+", number.ToString ("F1")));
	}

	static public string GetSignedStringFromNumber (int number) {
		return (string.Format ("{0}{1}", number < 0 ? "" : "+", number.ToString ("D")));
	}

	static public Color SetAlpha (Color color, float alpha) {

		if (alpha < 0) {
			alpha = 0;
		} else if (alpha > 1) {
			alpha = 1;
		}

		color.a = alpha;
		return (color);

	}

	static public Vector3 Bezier (float u, List<Vector3> vectors) {
		if (vectors.Count == 0) {
			return (Vector3.zero);
		}

		if (vectors.Count == 1) {
			return vectors [0];
		}

		List<Vector3> vectorsL = vectors.GetRange (0, vectors.Count - 1);
		List<Vector3> vectorsR = vectors.GetRange (1, vectors.Count - 1);

		return ( (1 - u) * Bezier (u, vectorsL)  + u * Bezier (u, vectorsR) );

	}

	static public Vector3 Bezier (float u, params Vector3[] vectors) {
		return (Bezier (u, new List<Vector3> (vectors)) );
	}


	/// <summary>
	/// <para>Solves a quadratic equation of the form</para>
	/// <para>"a x^2 + b x + c = 0"</para>
	/// Where a != 0
	/// </summary>
	/// <returns>An array which is either:
	/// null if there are no solution,
	/// a one element array if there is one solution or
	/// a two-element array if there are two solutions.</returns>
	/// <param name="a">The first constant of the equation.</param>
	/// <param name="b">The second constant of the equation.</param>
	/// <param name="c">The third constant of the equation.</param>
	static public float[] SolveQuadratic (float a, float b, float c) {
		// The solutions for a quadration equation of the form "a x^2 + b x + c = 0" are:
		// x1 = (-b - Sqrt (b^2 - 4ac)) / 2a
		// x2 = (-b + Sqrt (b^2 - 4ac)) / 2a

		if (a == 0) {
			return null;
		}

		float[] solutions;
		float x1;
		float x2;
		if ((b * b - 4 * a * c) < 0) {
			// No solutions
			solutions = null;
		} else if ((b * b - 4 * a * c) == 0) {
			// 1 solution
			x1 = (-b) / (2 * a);
			solutions = new float[] { x1 };
		} else {
			x1 = (-b - Mathf.Sqrt (b * b - 4 * a * c)) / (2 * a);
			x2 = (-b + Mathf.Sqrt (b * b - 4 * a * c)) / (2 * a);
			solutions = new float[] { x1, x2 };
		}
		return solutions;
	}


	/// <summary>
	/// <para>Determines whether two Spheres of the Sphere class (defined only by center and radius)</para>
	/// <para>moving through space with a velocity will collide. </para>
	/// <para></para>
	/// </summary>
	/// <returns><c>true</c>, If the spheres will collide, <c>false</c> otherwise.</returns>
	/// <param name="sphere1">sphere1.</param>
	/// <param name="velocity1">velocity1.</param>
	/// <param name="sphere2">sphere2.</param>
	/// <param name="velocity2">velocity2.</param>
	/// <param name="solutions">solutions.</param>
	static public bool SpheresInMotionWillCollide (Sphere sphere1, Vector3 velocity1, Sphere sphere2, Vector3 velocity2, out float[] collisionTimes) {
		/*
		To determine if a collision will happen, the following is done:
		
		For both spheres, the following applies:
		1: s1 = pos1 + v1 * t  &  2: s2 = pos2 + v2 * t 
		where,
			s (V3) is the position at any time t (float)
			pos (V3) is the start position at the time of the calculation
				pos = sphere#.center
			v (V3) is the velocity vector which equals speed (float) * direction (V3)
		
		For a collision to occur, then the following inequality must be solvable:
		3: (s1-s2).magnitude < d
		where,
			d is the minimum distance between sphere centers before a collision occurs
				d = sphere1.radius + sphere2.radius

		To solve the inequation, we'll first solve it as an equation
		using Eq1 and Eq2 in Eq3:
		([pos1 + v1 * t] - [pos2 + v2 * t]).magnitude = d
		4: ( [pos1-pos2] + t*[v1-v2] ).magnitude = d
		let 5: p = pos1-pos2  &  6: v = v1-v2

		using Eq5 and Eq6 in Eq 4:
		( p + t*v ).magnitude = d
		Sqrt ( (p_x + t * v_x)^2 + (p_y + t * v_y)^2 + (p_z + t * v_z)^2 ) = d		| ^2
		(p_x + t * v_x)^2 + (p_y + t * v_y)^2 + (p_z + t * v_z)^2 = d^2
		Sorting eveything out:
		[v.magnitude]^2 * t^2 + 2*Dot (v, p) * t + [p.magnitude]^2 = d^2
		7: [v.magnitude]^2 * t^2 + 2*Dot (v, p) * t + [p.magnitude]^2 - d^2 = 0

		Eq 7 can be re-written as:
		8: a * t^2 + b * t + c = 0
		where,
			a = [v.magnitude]^2 = [(v1-v2).magnitude]^2
			b = 2 * Dot (v,p) = 2 * Dot ( (v1-v2), (pos1-pos2) )
			c = [p.magnitude]^2 - d^2 = [(p1-p2).magnitude]^2 - d^2
		This is an equation that can easily be solved.
		If there is are two solutions for Eq8, then the two spheres will collide in their current paths.
		*/

		// So we calculate v and p and then a, b and c and d
		Vector3 v = velocity1 - velocity2;
		Vector3 p = sphere1.center - sphere2.center;
		float d = sphere1.radius + sphere2.radius;

		float a = v.magnitude * v.magnitude;
		float b = 2 * Vector3.Dot (v, p);
		float c = p.magnitude * p.magnitude - d*d;

		// Get solutions for the quadratic equation
		collisionTimes = Utils.SolveQuadratic (a,b,c);

		if (collisionTimes == null) {
			return false;
		} else if (collisionTimes.Length == 1) {
			return false;
		} else if (collisionTimes.Length == 2) {
			return true;
		}
		Debug.Log ("There were more than 2 solutions to a quadratic equation. Something is wrong");
		return false;
	}

	static public bool SpheresInMotionWillCollide (Sphere sphere1, Vector3 velocity1, Sphere sphere2, Vector3 velocity2) {
		float[] collisionTimes;
		return (SpheresInMotionWillCollide (sphere1, velocity1, sphere2, velocity2, out collisionTimes));
	}

}

public class Sphere {
	public Vector3 center;
	public float radius;

	public Sphere (Vector3 center, float radius) {
		this.center = center;
		this.radius = radius;
	}

}


