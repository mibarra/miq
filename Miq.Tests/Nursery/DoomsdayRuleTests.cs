using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Miq.Tests.Nursery
{
    [TestClass]
    public class DoomsdayRuleTests
    {
        [TestMethod]
        public void WeekDay_Values_AreCorrect()
        {
            Assert.AreEqual(0, (int)DayOfWeek.Sunday);
            Assert.AreEqual(1, (int)DayOfWeek.Monday);
            Assert.AreEqual(2, (int)DayOfWeek.Tuesday);
            Assert.AreEqual(3, (int)DayOfWeek.Wednesday);
            Assert.AreEqual(4, (int)DayOfWeek.Thursday);
            Assert.AreEqual(5, (int)DayOfWeek.Friday);
            Assert.AreEqual(6, (int)DayOfWeek.Saturday);
        }

        [TestMethod]
        public void AnchorDay_ReturnsCorrectValue()
        {
            Assert.AreEqual(DayOfWeek.Friday, AnchorDay(1800));
            Assert.AreEqual(DayOfWeek.Wednesday, AnchorDay(1950));
            Assert.AreEqual(DayOfWeek.Tuesday, AnchorDay(2050));
            Assert.AreEqual(DayOfWeek.Sunday, AnchorDay(2111));
        }

        private DayOfWeek AnchorDay(int year)
        {
            int c = (int)System.Math.Floor((float)year/100.0);
            int anchorDay = 5 * (c % 4) + (int)DayOfWeek.Tuesday;
            return (DayOfWeek)(anchorDay % 7);
        }


        /*
         * 
         * leap  year
         * 
         * if year is divisible by 400 then is_leap_year
else if year is divisible by 100 then not_leap_year
else if year is divisible by 4 then is_leap_year
else not_leap_year
         * 
         * Leap Years 1800 - 2400
1804
1808
1812
1816
1820
1824
1828
1832
1836
1840
1844
1848
1852
1856
1860
1864
1868
1872
1876
1880
1884
1888
1892
1896
1904
1908
1912
1916
1920
1924
1928
1932
1936
1940
1944
1948
1952
1956
1960
1964
1968
1972
1976
1980
1984
1988
1992
1996
2000
2004
2008
2012
2016
2020
2024
2028
2032
2036
2040
2044
2048
2052
2056
2060
2064
2068
2072
2076
2080
2084
2088
2092
2096
2104
2108
2112
2116
2120
2124
2128
2132
2136
2140
2144
2148
2152
2156
2160
2164
2168
2172
2176
2180
2184
2188
2192
2196
2204
2208
2212
2216
2220
2224
2228
2232
2236
2240
2244
2248
2252
2256
2260
2264
2268
2272
2276
2280
2284
2288
2292
2296
2304
2308
2312
2316
2320
2324
2328
2332
2336
2340
2344
2348
2352
2356
2360
2364
2368
2372
2376
2380
2384
2388
2392
2396
2400


         * * Use the anchor day for the century to calculate the doomsday for the year.
         * 
         * Divide the year's last two digits (call this y) by 12 and let a be the floor of the quotient.
Let b be the remainder of the same quotient.
Divide that remainder by 4 and let c be the floor of the quotient.
Let d be the sum of the three numbers (d = a + b + c). (It is again possible here to divide by seven and take the remainder. This number is equivalent, as it must be, to the sum of the last two digits of the year taken collectively plus the floor of those collective digits divided by four.)
Count forward the specified number of days (d or the remainder of d/7) from the anchor day to get the year's Doomsday.

         * \begin{matrix}\left({\left\lfloor{\frac{y}{12}}\right\rfloor+y \bmod 12+\left\lfloor{\frac{y \bmod 12}{4}}\right\rfloor}\right) \bmod 7+\rm{anchor}=\rm{Doomsday}\end{matrix}
            \mbox{Doomsday} =  \mbox{Tuesday} + y + \left\lfloor\frac{y}{4}\right\rfloor - \left\lfloor\frac{y}{100}\right\rfloor + \left\lfloor\frac{y}{400}\right\rfloor = \mbox{Tuesday} + 5\times (y\mod 4) + 4\times (y\mod 100) + 6\times (y\mod 400)

         * * Choose the closest date out of the ones that always fall on the doomsday (e.g. 4/4, 6/6, 8/8), and count the number of days (modulo 7) between that date and the date in question to arrive at the day of the week.
         * 
         * 
                Month	Memorable date	Month/Day	Mnemonic
                January	
         *              January 3 (common years),
         *              January 4 (leap years)
                        January 11 (leap years)
                February	
         *              February 14 (common years)
                        February 22 (leap years)
                        February 28 (common years),
         *              February 29 (leap years)
                March	
         *              "March 0"
                        March 14
                        March 21
         *      April   	
         *              April 4
                May	    
         *              May 9
                        May 30
         *      June    
         *              June 6
                        June 20
                July
         *              July 4
                        July 11
                August
         *              August 8
         *      September
         *              September 5
                October
         *              October 10
                        October 31
                November
         *              November 7
                December
         *              December 12
                        December 26
         * 
         * Overview of all Doomsdays[edit]

                Month	                Dates	        Week numbers *
                January (common years)	3, 10, 17, 24, 31	1–5
                January (leap years)	    4, 11, 18, 25	    1–4
                February (common years)	7, 14, 21, 28	    6–9
                February (leap years)   	1, 8, 15, 22, 29	5–9
                March	                7, 14, 21, 28	    10–13
                April	                    4, 11, 18, 25	    14–17
                May	                    2, 9, 16, 23, 30	18–22
                June	                    6, 13, 20, 27	    23–26
                July	                    4, 11, 18, 25   	27–30
                August	                1, 8, 15, 22, 29	31–35
                September           	5, 12, 19, 26	    36–39
                October	                3, 10, 17, 24, 31	40–44
                November            	7, 14, 21, 28   	45–48
                December            	5, 12, 19, 26   	49–52
         * */
    }
}
