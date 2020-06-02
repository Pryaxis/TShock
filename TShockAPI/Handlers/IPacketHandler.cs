namespace TShockAPI.Handlers
{
	/// <summary>
	/// Describes a packet handler that receives a packet from a GetDataHandler
	/// </summary>
	/// <typeparam name="TEventArgs"></typeparam>
	public interface IPacketHandler<TEventArgs> where TEventArgs : GetDataHandledEventArgs
	{
		/// <summary>
		/// Invoked when the packet is received
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		void OnReceive(object sender, TEventArgs args);
	}
}
