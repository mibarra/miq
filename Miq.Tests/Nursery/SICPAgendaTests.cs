using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Miq.Tests.Nursery
{
	public class Agenda
	{
		public uint CurrentTime
		{ get; private set; }

		public void AfterDelay(uint delay, Action action)
		{
			Add(delay + CurrentTime, action);
		}

		public void Propagate()
		{
			while (!Empty)
			{
				var action = Dequeue();
				action();
			}
		}

		public Agenda()
		{
			Segments = new SortedList<uint, Queue<Action>>();
			CurrentTime = 0;
		}

		private SortedList<uint, Queue<Action>> Segments;

		private bool Empty
		{
			get
			{
				return Segments.Count == 0;
			}
		}

		private Action Dequeue()
		{
			var queue = Segments.First().Value;
			var action = queue.Dequeue();
			CurrentTime = Segments.First().Key;
			if (queue.Count == 0)
			{
				Segments.RemoveAt(0);
			}
			return action;
		}

		private void Add(uint time, Action action)
		{
			if (!Segments.ContainsKey(time))
			{
				Segments.Add(time, new Queue<Action>());
			}
			Segments[time].Enqueue(action);
		}
	}

	public class Wire
	{
		public bool Signal
		{
			get
			{
				return _signal;
			}
			set
			{
				if (value != _signal)
				{
					_signal = value;
					if (_actions != null)
						_actions(this, new EventArgs());
				}
			}
		}

		public delegate	void WireAction(object sender, EventArgs e);

		public event WireAction Actions
		{
			add
			{
				lock (objectLock)
				{
					_actions += value;
					value(this, new EventArgs());
				}
			}

			remove
			{
				lock (objectLock)
				{
					_actions -= value;
				}
			}
		}

		object objectLock = new Object();
		private event WireAction _actions;
		private bool _signal;
	}

	public class HalfAdder
	{
		public Wire d;
		public Wire e;

		public HalfAdder(Wire a, Wire b, Wire s, Wire c, Agenda agenda)
		{
			d = new Wire();
			e = new Wire();

			new OrGate(a, b, d, agenda);
			new AndGate(a, b, c, agenda);
			new Inverter(c, e, agenda);
			new AndGate(d, e, s, agenda);
		}
	}

	public class HalfAdderExample
	{
		public Wire input1;
		public Wire input2;
		public Wire sum;
		public Wire carry;

		public HalfAdderExample(Agenda agenda)
		{
			input1 = new Wire();
			input2 = new Wire();
			sum = new Wire();
			carry = new Wire();

			new Probe("sum", sum, agenda);
			new Probe("carry", carry, agenda);

			new HalfAdder(input1, input2, sum, carry, agenda);
		}
	}

	public abstract class BinaryGate
	{
		public abstract bool Value { get; }
		public abstract uint Delay { get; }

		public BinaryGate(Wire a1, Wire a2, Wire output, Agenda agenda)
		{
			A1 = a1;
			A2 = a2;
			Output = output;
			Agenda = agenda;

			a1.Actions += ActionProcedure;
			a2.Actions += ActionProcedure;
		}

		public void ActionProcedure(object sender, EventArgs e)
		{
			var value = Value;
			Agenda.AfterDelay(Delay, () => Output.Signal = value);
		}

		protected Wire A1;
		protected Wire A2;
		protected Wire Output;
		protected Agenda Agenda;
	}

	public class AndGate : BinaryGate
	{
		public AndGate(Wire a1, Wire a2, Wire output, Agenda agenda)
			: base(a1, a2, output, agenda)
		{ }

		public override uint Delay
		{
			get { return 3; }
		}

		public override bool Value
		{
			get { return A1.Signal && A2.Signal; }
		}
	}

	public class OrGate : BinaryGate
	{
		public OrGate(Wire a1, Wire a2, Wire output, Agenda agenda)
			: base(a1, a2, output, agenda)
		{ }

		public override uint Delay
		{
			get { return 5; }
		}

		public override bool Value
		{
			get { return A1.Signal || A2.Signal; }
		}
	}

	public class Inverter
	{
		public Inverter(Wire input, Wire output, Agenda agenda)
		{
			Input = input;
			Output = output;
			Agenda = agenda;

			Input.Actions += ActionProcedure;
		}

		public void ActionProcedure(object sender, EventArgs e)
		{
			var value = Value;
			Agenda.AfterDelay(Delay, () => Output.Signal = value);
		}

		public uint Delay { get { return 2; } }
		public bool Value { get { return !Input.Signal; } }

		protected Wire Input;
		protected Wire Output;
		protected Agenda Agenda;
	}

	public class Probe
	{
		public Probe(string name, Wire wire, Agenda agenda)
		{
			Name = name;
			Wire = wire;
			Agenda = agenda;

			wire.Actions += ActionProcedure;
		}

		public void ActionProcedure(object sender, EventArgs e)
		{
			Debug.WriteLine("{0} {1} new value = {2}", Name, Agenda.CurrentTime, Wire.Signal);
		}

		string Name;
		Wire Wire;
		Agenda Agenda;
	}

	[TestClass]
	public class SICPAgendaTests
	{
		Agenda Agenda;

		[TestInitialize]
		[Ignore]
		public void Initialize()
		{
			Agenda = new Agenda();
		}

		[TestMethod]
		[Ignore]
		public void TestMethod1()
		{
			Agenda.AfterDelay(10, () =>
				Debug.WriteLine("time is: {0}", Agenda.CurrentTime));
			Agenda.Propagate();
		}

		[TestMethod]
		[Ignore]
		public void TestMethod2()
		{
			var circuit = new HalfAdderExample(Agenda);
			circuit.input1.Signal = true;
			Agenda.Propagate();

			circuit.input2.Signal = true;
			Agenda.Propagate();
		}

		[TestMethod]
		[Ignore]
		public void MyTestMethod()
		{
			for (int i = 0; i < Cashiers.Length; i++)
			{
				Cashiers[i] = new Cashier(Agenda);
			}
			Agenda.AfterDelay(ClientInterArrivalTime(), ClientArrives);
			Agenda.Propagate();
		}

		static Random r = new Random();

		private static uint ClientInterArrivalTime()
		{
			return (uint)ExponentialDistribution(r.NextDouble(), 20.0);
		}

		private static uint ServiceTime()
		{
			return (uint)ExponentialDistribution(r.NextDouble(), 100.0);
		}

		private static double ExponentialDistribution(double uniform, double rate)
		{
			double val = -rate * System.Math.Log(1d - uniform);
			return val;
		}

		static class ClientQueue
		{
			public static int Length;
		}

		class Cashier
		{
			public Cashier(Agenda agenda)
			{
				Agenda = agenda;
			}
			public bool Busy;

			internal void TakeClient()
			{
				if (ClientQueue.Length > 0)
				{
					ClientQueue.Length--;
					Busy = true;
					Agenda.AfterDelay(ServiceTime(), FinishedWithClient);
				}
				else
				{
					Busy = false;
				}
			}

			private void FinishedWithClient()
			{
				ClientLeaves();
				TakeClient();
			}

			Agenda Agenda;
		}
		Cashier[] Cashiers = new Cashier[10];
		
		private void ClientArrives()
		{
			ClientQueue.Length++;

			var idleCashier = Cashiers.FirstOrDefault(c => !c.Busy);
			if (idleCashier != null)
			{
				idleCashier.TakeClient();
			}

			Agenda.AfterDelay(ClientInterArrivalTime(), ClientArrives);
		}

		private static void ClientLeaves()
		{
		}
	}
}
