using System;
using System.Collections.Generic;
using System.Text;

namespace AdofaiTheater.Foundation.Basic
{
	public interface IParameterizedEase
	{
		public double Ease(double t);
	}

    public class LinearParameterizedEase : IParameterizedEase { public double Ease(double t) { return t; } }
    public class InSineParameterizedEase : IParameterizedEase { public double Ease(double t) { return 1 - Math.Cos(t * Math.PI / 2); } }
    public class OutSineParameterizedEase : IParameterizedEase { public double Ease(double t) { return Math.Sin(t * Math.PI / 2); } }
}
