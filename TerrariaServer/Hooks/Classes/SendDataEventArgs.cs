using System;
using System.ComponentModel;
namespace Hooks
{
	public class SendDataEventArgs : HandledEventArgs
	{
		public PacketTypes MsgID
		{
			get;
			set;
		}
		public int remoteClient
		{
			get;
			set;
		}
		public int ignoreClient
		{
			get;
			set;
		}
		public string text
		{
			get;
			set;
		}
		public int number
		{
			get;
			set;
		}
		public float number2
		{
			get;
			set;
		}
		public float number3
		{
			get;
			set;
		}
		public float number4
		{
			get;
			set;
		}

        public int number5
        {
            get; 
            set; 
        }
	}
}
