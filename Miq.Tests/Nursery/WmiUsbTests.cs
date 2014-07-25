using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Management;

namespace Miq.Tests.Nursery
{
	[TestClass]
	public class WmiUsbTests
	{
		[TestMethod]
		public void TestMethod1()
		{
			System.Management.ManagementClass USBClass = new ManagementClass("Win32_USBDevice");
			System.Management.ManagementObjectCollection USBCollection = USBClass.GetInstances();

			foreach (System.Management.ManagementObject usb in USBCollection)
			{
				try
				{
					string deviceId = usb["deviceid"].ToString();
					string a0 = usb["Caption"].ToString();
					string a1 = usb["Description"].ToString();
					string a2 = usb["Manufacturer"].ToString();
					string a3 = usb["Name"].ToString();
					string a4 = usb["PNPDeviceID"].ToString();
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
