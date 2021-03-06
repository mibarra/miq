﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextElite
{
	class Program
	{
		static void Main()
		{
			Console.Write("\nWelcome to Text Elite 1.4.\n");

			mysrand(12345);/* Ensure repeatability */

			galaxynum = 1;
			buildgalaxy(galaxynum);

			currentplanet = numforLave;                        /* Don't use jump */
			localmarket = genmarket(0x00, galaxy[numforLave]);/* Since want seed=0 */

			fuel = maxfuel;

			parser("hold 20");         /* Small cargo bay */
			parser("cash +100");       /* 100 CR */
			parser("help");

			for (; ; )
			{
				Console.Write(string.Format("\n\nCash: {0:0.0}> ", (float)cash / 10));
				string getcommand = Console.ReadLine();
				parser(getcommand);
			}
		}

		#region Player
		// XXX Refactor; this is the planet the player is on; should be a property of the player instead of a global.
		static int currentplanet;
		static Stock shipshold = new Stock();  /* Contents of cargo bay */
		static int cash;
		static uint fuel;
		static int holdspace;

		static int fuelcost = 2; /* 0.2 CR/Light year */
		static uint maxfuel = 70; /* 7.0 LY tank */
		#endregion

		#region RNG
		static char randbyte() { return (char)(myrand() & 0xFF); }

		static void tweakseed(ref seedtype s)
		{
			int temp;
			temp = (s.w0) + (s.w1) + (s.w2); /* 2 byte aritmetic */
			s.w0 = s.w1;
			s.w1 = s.w2;
			s.w2 = (ushort)temp;
		}

		/* four byte random number used for planet description */
		struct fastseedtype
		{
			public byte a, b, c, d;
		}

		static fastseedtype rnd_seed;
		static Random rand = new Random();

		static void mysrand(int seed)
		{
			rand = new Random(seed);
		}

		static int myrand()
		{
			return rand.Next();
		}
		#endregion

		#region Trading

		class GoodIndexedIntegerCollection
		{
			public GoodIndexedIntegerCollection(int defaultValue)
			{
				Values = new Dictionary<TradeGood, int>();
				DefaultValue = defaultValue;
			}

			public int this[TradeGood good]
			{
				get { return Values.ContainsKey(good)? Values[good]: DefaultValue; }
				set { Values[good] = value; }
			}

			private Dictionary<TradeGood, int> Values;
			private int DefaultValue;
		}

		class QuantityCollection : GoodIndexedIntegerCollection
		{
			public QuantityCollection(): base(0)
			{

			}
		}

		class PriceList : GoodIndexedIntegerCollection
		{
			public PriceList():base(int.MaxValue)
			{

			}
		}

		class Stock
		{
			public Stock()
			{
				Quantity = new QuantityCollection();
			}

			public QuantityCollection Quantity { get; private set; }
		}

		class PricedStock : Stock
		{
			public PricedStock() : base()
			{
				Price = new PriceList();
			}

			public PriceList Price { get; private set; }
		}

		static PricedStock localmarket;

		static PricedStock genmarket(uint fluct, plansys p)
		{
			var market = new PricedStock();

			for (int i = 0; i <= lasttrade; i++)
			{
				TradeGood good = TradeGood.AllGoods[i];
				long q;
				long product = (p.economy) * (good.gradient);
				uint changing = (fluct & (good.maskbyte));
				q = (good.basequant) + changing - product;
				q = q & 0xFF;
				if ((q & 0x80) != 0) { q = 0; };                       /* Clip to positive 8-bit */

				market.Quantity[good] = (UInt16)(q & 0x3F); /* Mask to 6 bits */

				q = (good.baseprice) + changing + product;
				q = q & 0xFF;
				market.Price[good] = (UInt16)(q * 4);
			}
			return market;
		}

		// XXX Refactor; can we get rid of this?
		const int lasttrade = 15;
		#endregion

		#region Universe
		/* Seperation between two planets (4*sqrt(X*X+Y*Y/4)) */
		static uint distance(plansys a, plansys b)
		{
			return (uint)Math.Round(4 * Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) / 4));
		}

		// XXX Refactor; get rid of this
		const int numforLave = 7;       /* Lave is 7th generated planet in galaxy one */

		// XXX refactor; convert to enum
		static string[] govnames = { "Anarchy", "Feudal", "Multi-gov", "Dictatorship",
"Communist", "Confederacy", "Democracy", "Corporate State" };

		// XXX; Refactor; convert to enum
		static string[] econnames = {
			"Rich Ind",		// 000
			"Average Ind",	// 001
			"Poor Ind",		// 010
			"Mainly Ind",	// 011

			"Mainly Agri",  // 100
			"Rich Agri",    // 101
			"Average Agri", // 110
			"Poor Agri" };  // 111

		// XXX Refactor; convert to class
		struct plansys
		{
			public int x;
			public int y;       /* One byte unsigned */
			public int economy; /* These two are actually only 0-7  */
			public int govtype;
			public int techlev; /* 0-16 i think */
			public int population;   /* One byte */
			public int productivity; /* Two byte */
			public int radius; /* Two byte (not used by game at all) */
			public fastseedtype goatsoupseed;
			public string name;
		}

		// XXX Refactor; can we get rid of this?
		const int galsize = 256;
		static plansys[] galaxy = new plansys[galsize]; /* Need 0 to galsize-1 inclusive */

		static uint galaxynum;               /* Galaxy number (1-8) */

		struct seedtype
		{
			public UInt16 w0;
			public UInt16 w1;
			public UInt16 w2;
		}
		/* six byte random number used as seed for planets */

		static seedtype seed;

		const UInt16 base0 = 0x5A4A;
		const UInt16 base1 = 0x0248;
		const UInt16 base2 = 0xB753;  /* Base seed for galaxy 1 */

		static int rotatel(int x) /* rotate 8 bit number leftwards */
		/* (tried to use chars but too much effort persuading this braindead
		   language to do bit operations on bytes!) */
		{
			var temp = x & 128;
			return (2 * (x & 127)) + (temp >> 7);
		}

		static UInt16 twist(UInt16 x)
		{
			return (UInt16)((256 * rotatel(x >> 8)) + rotatel(x & 255));
		}

		static void nextgalaxy(ref seedtype s) /* Apply to base seed; once for galaxy 2  */
		{
			s.w0 = twist(s.w0);  /* twice for galaxy 3, etc. */
			s.w1 = twist(s.w1);  /* Eighth application gives galaxy 1 again*/
			s.w2 = twist(s.w2);
		}

		static plansys makesystem(ref seedtype s)
		{
			plansys thissys = new plansys();
			int pair1, pair2, pair3, pair4;
			int longnameflag = (s.w0) & 64;

			thissys.x = ((s.w1) >> 8);
			thissys.y = ((s.w0) >> 8);

			thissys.govtype = (((s.w1) >> 3) & 7); /* bits 3,4 &5 of w1 */
			thissys.economy = (((s.w0) >> 8) & 7); /* bits 8,9 & A of w0 */
			if (thissys.govtype <= 1)
			{
				thissys.economy = ((thissys.economy) | 2);
			}

			thissys.techlev = (((s.w1) >> 8) & 3) + ((thissys.economy) ^ 7);
			thissys.techlev += ((thissys.govtype) >> 1);
			if (((thissys.govtype) & 1) == 1) thissys.techlev += 1;

			thissys.population = 4 * (thissys.techlev) + (thissys.economy);

			thissys.productivity = (((thissys.economy) ^ 7) + 3) * ((thissys.govtype) + 4);
			thissys.productivity *= (thissys.population) * 8;

			thissys.radius = (256 * ((((s.w2) >> 8) & 15) + 11) + thissys.x);

			thissys.goatsoupseed.a = (byte)(s.w1 & 0xFF);
			thissys.goatsoupseed.b = (byte)(s.w1 >> 8);
			thissys.goatsoupseed.c = (byte)(s.w2 & 0xFF);
			thissys.goatsoupseed.d = (byte)(s.w2 >> 8);

			pair1 = (2 * (((s.w2) >> 8) & 31)); tweakseed(ref s);
			pair2 = (2 * (((s.w2) >> 8) & 31)); tweakseed(ref s);
			pair3 = (2 * (((s.w2) >> 8) & 31)); tweakseed(ref s);
			pair4 = (2 * (((s.w2) >> 8) & 31)); tweakseed(ref s);
			/* Always four iterations of random number */

			var aname = new StringBuilder();
			aname.Length = 6;
			aname[0] = pairs[pair1];
			aname[1] = pairs[pair1 + 1];
			aname[2] = pairs[pair2];
			aname[3] = pairs[pair2 + 1];
			aname[4] = pairs[pair3];
			aname[5] = pairs[pair3 + 1];

			if (longnameflag != 0) /* bit 6 of ORIGINAL w0 flags a four-pair name */
			{
				aname.Length = 8;
				aname[6] = pairs[pair4];
				aname[7] = pairs[pair4 + 1];
			}
			stripout(aname, '.');

			thissys.name = aname.ToString();

			return thissys;
		}

		static void buildgalaxy(uint galaxynum)
		{
			seed.w0 = base0; seed.w1 = base1; seed.w2 = base2;
			for (int galcount = 1; galcount < galaxynum; ++galcount) nextgalaxy(ref seed);
			for (int syscount = 0; syscount < galsize; ++syscount) galaxy[syscount] = makesystem(ref seed);
		}
		#endregion

		#region Commands
		delegate void comfunc(string cmd);

		static comfunc[] comfuncs = {
			dobuy, dosell, dofuel, dojump,
			docash, domkt, dohelp, dohold,
			dosneak, dolocal, doinfo, dogalhyp,
			doquit
		};

		static string[] commands = {
			"buy", "sell", "fuel", "jump",
			"cash", "mkt", "help", "hold",
			"sneak", "local", "info", "galhyp",
			"quit"
		};

		static void dolocal(string s)
		{
			Console.WriteLine("Galaxy number {0}", galaxynum);
			for (int syscount = 0; syscount < galsize; ++syscount)
			{
				uint d = distance(galaxy[syscount], galaxy[currentplanet]);
				if (d <= maxfuel)
				{
					if (d <= fuel)
						Console.Write("\n * ");
					else
						Console.Write("\n - ");

					prisys(galaxy[syscount], true);
					Console.Write(" ({0:0.0} LY)", (float)d / 10);
				}
			}
		}

		/**-Print data for given system **/
		static void prisys(plansys plsy, bool compressed)
		{
			if (compressed)
			{
				Console.Write(plsy.name);
				Console.Write(" TL: {0} ", (plsy.techlev) + 1);
				Console.Write("{0}", econnames[plsy.economy]);
				Console.Write(" {0}", govnames[plsy.govtype]);
			}
			else
			{
				Console.Write("\n\nSystem:  ");
				Console.Write(plsy.name);
				Console.Write("\nPosition ({0},", plsy.x);
				Console.Write("{0})", plsy.y);
				Console.Write("\nEconomy: ({0}) ", plsy.economy);
				Console.Write(econnames[plsy.economy]);
				Console.Write("\nGovernment: ({0}) ", plsy.govtype);
				Console.Write(govnames[plsy.govtype]);
				Console.Write("\nTech Level: {0}", (plsy.techlev) + 1);
				Console.Write("\nTurnover: {0}", (plsy.productivity));
				Console.Write("\nRadius: {0}", plsy.radius);
				Console.Write("\nPopulation: {0} Billion", (plsy.population) >> 3);

				rnd_seed = plsy.goatsoupseed;
				Console.WriteLine(" ");
				goat_soup("\x8F is \x97.", ref plsy);
			}
		}

		static void dojump(string s)
		{
			int dest = matchsys(s);
			if (dest == currentplanet)
			{
				Console.WriteLine("\nBad jump");
				return;
			}
			uint d = distance(galaxy[dest], galaxy[currentplanet]);
			if (d > fuel)
			{
				Console.WriteLine("\nJump to far");
				return;
			}
			fuel -= d;
			gamejump(dest);
			prisys(galaxy[currentplanet], false);
			return;
		}

		static void dosneak(string s)
		{
			uint fuelkeep = fuel;
			fuel = 666;
			dojump(s);
			fuel = fuelkeep;
			return;
		}

		static void dogalhyp(string s)
		{
			galaxynum++;
			if (galaxynum == 9) { galaxynum = 1; }
			buildgalaxy(galaxynum);
		}

		static void doinfo(string s)
		{
			int dest = matchsys(s);
			prisys(galaxy[dest], false);
		}

		static void dohold(string s)
		{
			int a = int.Parse(s),
				t = 0;

			foreach (TradeGood good in TradeGood.AllGoods.Where(good => good.Unit.AccumulateOnShipHold))
			{
				t += shipshold.Quantity[good];
			}

			if (t > a)
			{
				Console.WriteLine("Hold too full");
				return;
			}

			holdspace = a - t;
			return;
		}

		static uint gamefuel(uint f)
		{
			if (f + fuel > maxfuel) f = maxfuel - fuel;
			if (fuelcost > 0)
			{
				if ((int)f * fuelcost > cash) f = (uint)(cash / fuelcost);
			}
			fuel += f;
			cash -= (int)(fuelcost * f);
			return f;
		}

		static void dofuel(string s)
		{
			uint f = gamefuel((uint)Math.Floor(10 * float.Parse(s)));
			if (f == 0)
			{
				Console.WriteLine("\nCan't buy any fuel");
			}
			Console.WriteLine("\nBuying {0}LY fuel", (float)f / 10);
		}

		static void docash(string s)
		{
			int a = (int)(10 * float.Parse(s));
			cash += a;
			if (a != 0)
			{
				return;
			}

			Console.WriteLine("Number not understood");
		}

		static void domkt(string s)
		{
			displaymarket(localmarket);
			Console.WriteLine("\nFuel :{0:0.1}", (float)fuel / 10);
			Console.WriteLine("      Hold space :{0}t", holdspace);
		}

		static void doquit(string s)
		{
			System.Environment.Exit(0);
		}

		static void dohelp(string s)
		{
			Console.WriteLine("Commands are:");
			Console.WriteLine("Buy   {trade good} amount");
			Console.WriteLine("Sell  {trade good} amount");
			Console.WriteLine("Fuel  amount    (buy amount LY of fuel)");
			Console.WriteLine("Jump  {planet name} (limited by fuel)");
			Console.WriteLine("Sneak {planet name} (any distance - no fuel cost)");
			Console.WriteLine("Galhyp           (jumps to next galaxy)");
			Console.WriteLine("Info  {planet name} (prints info on system)");
			Console.WriteLine("Mkt              (shows market prices)");
			Console.WriteLine("Local            (lists systems within 7 light years)");
			Console.WriteLine("Cash number      (alters cash - cheating!)");
			Console.WriteLine("Hold number      (change cargo bay)");
			Console.WriteLine("Quit or ^C       (exit)");
			Console.WriteLine("Help             (display this text)");
			Console.WriteLine("\nAbbreviations allowed eg. b fo 5 = Buy Food 5, m= Mkt");
		}

		void stop(string str)
		{
			Console.WriteLine("\n" + str);
			System.Environment.Exit(0);
		}

		static void parser(string s) /* Obey command s */
		{
			uint i;
			string[] parts = s.Split(new char[] { ' ' }, 2);
			i = stringmatch(parts[0], commands);
			if (i == 0)
			{
				Console.WriteLine("\n Bad command ({0})", parts[0]);
				return;
			}
			comfuncs[i - 1](parts.Length > 1 ? parts[1] : null);
		}

		int toupper(char c)
		{
			if ((c >= 'a') && (c <= 'z')) return (c + 'A' - 'a');
			return ((int)c);
		}

		int tolower(char c)
		{
			if ((c >= 'A') && (c <= 'Z')) return (c + 'a' - 'A');
			return ((int)c);
		}

		// Check string s against n options in string array a   If matches ith element return i+1 else return 0 
		static uint stringmatch(string s, string[] a)
		{
			uint i = 0;
			while (i < a.Length)
			{
				if (a[i].StartsWith(s, StringComparison.OrdinalIgnoreCase)) return i + 1;
				i++;
			}
			return 0;
		}

		static void displaymarket(PricedStock m)
		{
			for (int i = 0; i <= lasttrade; i++)
			{
				TradeGood good = TradeGood.AllGoods[i];
				Console.Write("\n");
				Console.Write(good.name);
				Console.Write("   {0:0.1}", ((float)(m.Price[good]) / 10));
				Console.Write("   {0}", m.Quantity[good]);
				Console.Write(good.Unit.Symbol);
				Console.Write("   {0}", shipshold.Quantity[good]);
			}
		}

		static void gamejump(int i)
		{
			currentplanet = i;
			localmarket = genmarket(randbyte(), galaxy[i]);
		}

		static int matchsys(string s)
		// Return id of the planet whose name matches passed strinmg closest to currentplanet - if none return currentplanet
		{
			int p = currentplanet;
			uint d = 9999;
			for (int syscount = 0; syscount < galsize; ++syscount)
			{
				if (galaxy[syscount].name.StartsWith(s, StringComparison.OrdinalIgnoreCase))
				{
					if (distance(galaxy[syscount], galaxy[currentplanet]) < d)
					{
						d = distance(galaxy[syscount], galaxy[currentplanet]);
						p = syscount;
					}
				}
			}
			return p;
		}

		static void dosell(string s)
		{
			string[] parts = s.Split(' ');
			int i, a, t;

			a = int.Parse(parts[1]);
			if (a == 0) { a = 1; }

			i = (int)stringmatch(parts[0], TradeGood.tradnames);
			if (i == 0)
			{
				Console.WriteLine("\nUnknown trade good");
			}
			i -= 1;

			t = gamesell(i, a);

			if (t == 0) { Console.Write("Cannot sell any "); }
			else
			{
				Console.Write("\nSelling {0}", t);
				Console.Write(TradeGood.AllGoods[i].Unit.Symbol);
				Console.Write(" of ");
			}
			Console.WriteLine(TradeGood.tradnames[i]);
		}

		static void dobuy(string s)
		{
			string[] parts = s.Split(' ');
			int i, a, t;

			a = int.Parse(parts[1]);
			if (a == 0) a = 1;

			i = (int)stringmatch(parts[0], TradeGood.tradnames);
			if (i == 0)
			{
				Console.WriteLine("\nUnknown trade good");
			}
			i -= 1;

			t = gamebuy(i, a);
			if (t == 0) Console.Write("Cannot buy any ");
			else
			{
				Console.Write("\nBuying {0}", t);
				Console.Write(TradeGood.AllGoods[i].Unit.Symbol);
				Console.Write(" of ");
			}
			Console.WriteLine(TradeGood.tradnames[i]);
		}

		// Try to buy ammount a  of good i  Return ammount bought Cannot buy more than is availble, can afford, or will fit in hold
		static int gamebuy(int i, int a)
		{
			// ZZZ pass in good instead of index to this method
			var good = TradeGood.AllGoods[i];
			int t;
			if (cash < 0)
			{
				t = 0;
			}
			else
			{
				t = Math.Min(localmarket.Quantity[good], a);
				if (TradeGood.AllGoods[i].Unit.AccumulateOnShipHold) { t = Math.Min(holdspace, t); }
				t = Math.Min(t, (int)Math.Floor((double)cash / (localmarket.Price[good])));
			}

			shipshold.Quantity[good] += t;
			localmarket.Quantity[good] -= t;
			cash -= t * (localmarket.Price[good]);
			if (TradeGood.AllGoods[i].Unit.AccumulateOnShipHold) { holdspace -= t; }
			return t;
		}

		static int gamesell(int i, int a)
		{
			var good = TradeGood.AllGoods[i];
			int t = Math.Min(shipshold.Quantity[good], a);
			shipshold.Quantity[good] -= t;
			localmarket.Quantity[good] += t;
			if (TradeGood.AllGoods[i].Unit.AccumulateOnShipHold) { holdspace += t; }
			cash += t * localmarket.Price[good];
			return t;
		}
		#endregion

		#region Random Name & Description Generator

		static void stripout(StringBuilder s, char c) /* Remove all c's from string s */
		{
			int i = 0, j = 0;
			while (i < s.Length)
			{
				if (s[i] != c) { s[j] = s[i]; j++; }
				i++;
			}
			s.Length = j;
		}

		struct desc_choice { public string[] option; };

		// XXX Refactor; Extract a name & description generator
		static string pairs0 = "ABOUSEITILETSTONLONUTHNO..LEXEGEZACEBISOUSESARMAINDIREA.ERATENBERALAVETIEDORQUANTEISRION";
		static string pairs = "..LEXEGEZACEBISOUSESARMAINDIREA.ERATENBERALAVETIEDORQUANTEISRION";
		// Dots should be nullprint characters

		static desc_choice[] desc_list = {
	/* 81 */new desc_choice() { option = new string[] {"fabled", "notable", "well known", "famous", "noted" }},
	/* 82 */new desc_choice() { option = new string[] {"very", "mildly", "most", "reasonably", "" }},
	/* 83 */new desc_choice() { option = new string[] {"ancient", "\x95", "great", "vast", "pink" }},
	/* 84 */new desc_choice() { option = new string[] {"\x9E \x9D plantations", "mountains", "\x9C", "\x94 forests", "oceans" }},
	/* 85 */new desc_choice() { option = new string[] {"shyness", "silliness", "mating traditions", "loathing of \x86", "love for \x86" }},
	/* 86 */new desc_choice() { option = new string[] {"food blenders", "tourists", "poetry", "discos", "\x8E" }},
	/* 87 */new desc_choice() { option = new string[] {"talking tree", "crab", "bat", "lobst", "\xB2" }},
	/* 88 */new desc_choice() { option = new string[] {"beset", "plagued", "ravaged", "cursed", "scourged" }},
	/* 89 */new desc_choice() { option = new string[] {"\x96 civil war", "\x9B \x98 \x99s", "a \x9B disease", "\x96 earthquakes", "\x96 solar activity" }},
	/* 8A */new desc_choice() { option = new string[] {"its \x83 \x84", "the \xB1 \x98 \x99", "its inhabitants' \x9A \x85", "\xA1", "its \x8D \x8E" }},
	/* 8B */new desc_choice() { option = new string[] {"juice", "brandy", "water", "brew", "gargle blasters" }},
	/* 8C */new desc_choice() { option = new string[] {"\xB2", "\xB1 \x99", "\xB1 \xB2", "\xB1 \x9B", "\x9B \xB2" }},
	/* 8D */new desc_choice() { option = new string[] {"fabulous", "exotic", "hoopy", "unusual", "exciting" }},
	/* 8E */new desc_choice() { option = new string[] {"cuisine", "night life", "casinos", "sit coms", " \xA1 " }},
	/* 8F */new desc_choice() { option = new string[] {"\xB0", "The planet \xB0", "The world \xB0", "This planet", "This world" }},
	/* 90 */new desc_choice() { option = new string[] {"n unremarkable", " boring", " dull", " tedious", " revolting" }},
	/* 91 */new desc_choice() { option = new string[] {"planet", "world", "place", "little planet", "dump" }},
	/* 92 */new desc_choice() { option = new string[] {"wasp", "moth", "grub", "ant", "\xB2" }},
	/* 93 */new desc_choice() { option = new string[] {"poet", "arts graduate", "yak", "snail", "slug" }},
	/* 94 */new desc_choice() { option = new string[] {"tropical", "dense", "rain", "impenetrable", "exuberant" }},
	/* 95 */new desc_choice() { option = new string[] {"funny", "wierd", "unusual", "strange", "peculiar" }},
	/* 96 */new desc_choice() { option = new string[] {"frequent", "occasional", "unpredictable", "dreadful", "deadly" }},
	/* 97 */new desc_choice() { option = new string[] {"\x82 \x81 for \x8A", "\x82 \x81 for \x8A and \x8A", "\x88 by \x89", "\x82 \x81 for \x8A but \x88 by \x89", "a\x90 \x91" }},
	/* 98 */new desc_choice() { option = new string[] {"\x9B", "mountain", "edible", "tree", "spotted" }},
	/* 99 */new desc_choice() { option = new string[] {"\x9F", "\xA0", "\x87oid", "\x93", "\x92" }},
	/* 9A */new desc_choice() { option = new string[] {"ancient", "exceptional", "eccentric", "ingrained", "\x95" }},
	/* 9B */new desc_choice() { option = new string[] {"killer", "deadly", "evil", "lethal", "vicious" }},
	/* 9C */new desc_choice() { option = new string[] {"parking meters", "dust clouds", "ice bergs", "rock formations", "volcanoes" }},
	/* 9D */new desc_choice() { option = new string[] {"plant", "tulip", "banana", "corn", "\xB2weed" }},
	/* 9E */new desc_choice() { option = new string[] {"\xB2", "\xB1 \xB2", "\xB1 \x9B", "inhabitant", "\xB1 \xB2" }},
	/* 9F */new desc_choice() { option = new string[] {"shrew", "beast", "bison", "snake", "wolf" }},
	/* A0 */new desc_choice() { option = new string[] {"leopard", "cat", "monkey", "goat", "fish" }},
	/* A1 */new desc_choice() { option = new string[] {"\x8C \x8B", "\xB1 \x9F \xA2", "its \x8D \xA0 \xA2", "\xA3 \xA4", "\x8C \x8B" }},
	/* A2 */new desc_choice() { option = new string[] {"meat", "cutlet", "steak", "burgers", "soup" }},
	/* A3 */new desc_choice() { option = new string[] {"ice", "mud", "Zero-G", "vacuum", "\xB1 ultra" }},
	/* A4 */new desc_choice() { option = new string[] {"hockey", "cricket", "karate", "polo", "tennis" }}
};

		static int gen_rnd_number()
		{
			int a, x;
			x = (rnd_seed.a * 2) & 0xFF;
			a = x + rnd_seed.c;
			if (rnd_seed.a > 127) a++;
			rnd_seed.a = (byte)(a & 0xFF);
			rnd_seed.c = (byte)x;

			a = a / 256;	// a = any carry left from above 
			x = rnd_seed.b;
			a = (a + x + rnd_seed.d) & 0xFF;
			rnd_seed.b = (byte)a;
			rnd_seed.d = (byte)x;
			return a;
		}

		static void goat_soup(string source, ref plansys psy)
		{
			for (; ; )
			{
				if (source.Length == 0)
				{
					break;
				}
				int c = (int)(source[0]);
				source = source.Substring(1);

				if (c < 0x80) Console.Write((char)c);
				else
				{
					if (c <= 0xA4)
					{
						int rnd = gen_rnd_number();
						goat_soup(desc_list[c - 0x81].option[
							(rnd >= 0x33 ? 1 : 0) +
							(rnd >= 0x66 ? 1 : 0) +
							(rnd >= 0x99 ? 1 : 0) +
							(rnd >= 0xCC ? 1 : 0)], ref psy);
					}
					else
					{
						switch (c)
						{
							case 0xB0: // planet name
								Console.Write(psy.name[0]);
								Console.Write(psy.name.Substring(1).ToLower());
								break;
							case 0xB1: // <planet name>ian
								Console.Write(psy.name[0]);
								Console.Write(psy.name.Substring(1, psy.name.Length - 2).ToLower());
								if (psy.name[psy.name.Length - 1] != 'E' && psy.name[psy.name.Length - 1] != 'I')
								{
									Console.Write(psy.name.ToLower()[psy.name.Length - 1]);
								}
								Console.Write("ian");
								break;
							case 0xB2: // random name
								int len = gen_rnd_number() & 3;
								for (int i = 0; i <= len; i++)
								{
									int x = gen_rnd_number() & 0x3e;
									if (pairs0[x] != '.') Console.Write(pairs0[x]);
									if (i != 0 && (pairs0[x + 1] != '.')) Console.Write(pairs0[x + 1]);
								}
								break;
							default: Console.WriteLine("<bad char in data [{0}]>", (char)c);
								return;
						}
					}
				}
			}
		}

		#endregion
	}

	class Unit
	{
		public Unit(string symbol, bool accumulateOnShipHold)
		{
			Symbol = symbol;
			AccumulateOnShipHold = accumulateOnShipHold;
		}

		public string Symbol { get; private set; }
		public bool AccumulateOnShipHold { get; private set; }

		public static Unit Ton = new Unit("t", true);
		public static Unit Kilogram = new Unit("kg", false);
		public static Unit Gram = new Unit("g", false);
	}

	class TradeGood
	{
		public uint baseprice;
		public Int16 gradient;
		public uint basequant;
		public uint maskbyte;
		public Unit Unit;
		public string name;

		public static string[] tradnames
		{
			get
			{
				return AllGoods.Select(good => good.name).ToArray();
			}
		}

		public static TradeGood[] AllGoods = new TradeGood[] {
			new TradeGood() { baseprice = 0x13, gradient = -0x02, basequant = 0x06, maskbyte = 0x01, Unit = Unit.Ton,      name = "Food        " },
			new TradeGood() { baseprice = 0x14, gradient = -0x01, basequant = 0x0A, maskbyte = 0x03, Unit = Unit.Ton,      name = "Textiles    " },
			new TradeGood() { baseprice = 0x41, gradient = -0x03, basequant = 0x02, maskbyte = 0x07, Unit = Unit.Ton,      name = "Radioactives" },
			new TradeGood() { baseprice = 0x28, gradient = -0x05, basequant = 0xE2, maskbyte = 0x1F, Unit = Unit.Ton,      name = "Slaves      " },
			new TradeGood() { baseprice = 0x53, gradient = -0x05, basequant = 0xFB, maskbyte = 0x0F, Unit = Unit.Ton,      name = "Liquor/Wines" },
			new TradeGood() { baseprice = 0xC4, gradient = +0x08, basequant = 0x36, maskbyte = 0x03, Unit = Unit.Ton,      name = "Luxuries    " },
			new TradeGood() { baseprice = 0xEB, gradient = +0x1D, basequant = 0x08, maskbyte = 0x78, Unit = Unit.Ton,      name = "Narcotics   " },
			new TradeGood() { baseprice = 0x9A, gradient = +0x0E, basequant = 0x38, maskbyte = 0x03, Unit = Unit.Ton,      name = "Computers   " },
			new TradeGood() { baseprice = 0x75, gradient = +0x06, basequant = 0x28, maskbyte = 0x07, Unit = Unit.Ton,      name = "Machinery   " },
			new TradeGood() { baseprice = 0x4E, gradient = +0x01, basequant = 0x11, maskbyte = 0x1F, Unit = Unit.Ton,      name = "Alloys      " },
			new TradeGood() { baseprice = 0x7C, gradient = +0x0d, basequant = 0x1D, maskbyte = 0x07, Unit = Unit.Ton,      name = "Firearms    " },
			new TradeGood() { baseprice = 0xB0, gradient = -0x09, basequant = 0xDC, maskbyte = 0x3F, Unit = Unit.Ton,      name = "Furs        " },
			new TradeGood() { baseprice = 0x20, gradient = -0x01, basequant = 0x35, maskbyte = 0x03, Unit = Unit.Ton,      name = "Minerals    " },
			new TradeGood() { baseprice = 0x61, gradient = -0x01, basequant = 0x42, maskbyte = 0x07, Unit = Unit.Kilogram, name = "Gold        " },
			new TradeGood() { baseprice = 0xAB, gradient = -0x02, basequant = 0x37, maskbyte = 0x1F, Unit = Unit.Kilogram, name = "Platinum    " },
			new TradeGood() { baseprice = 0x2D, gradient = -0x01, basequant = 0xFA, maskbyte = 0x0F, Unit = Unit.Gram,     name = "Gem-Strones " }
		};

		public override bool Equals(object obj)
		{
			return (obj as TradeGood) != null && this.GetHashCode() == obj.GetHashCode();
		}

		public override int GetHashCode()
		{
			return name.GetHashCode();
		}
	};
}
