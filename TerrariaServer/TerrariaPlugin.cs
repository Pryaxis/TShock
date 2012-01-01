using System;
using Terraria;
public abstract class TerrariaPlugin : IDisposable
{
	public virtual string Name
	{
		get
		{
			return "None";
		}
	}
	public virtual Version Version
	{
		get
		{
			return new Version(1, 0);
		}
	}
	public virtual string Author
	{
		get
		{
			return "None";
		}
	}
	public virtual string Description
	{
		get
		{
			return "None";
		}
	}
	public virtual bool Enabled
	{
		get;
		set;
	}
	public int Order
	{
		get;
		set;
	}
	protected Main Game
	{
		get;
		private set;
	}
	protected TerrariaPlugin(Main game)
	{
		this.Order = 1;
		this.Game = game;
	}
	~TerrariaPlugin()
	{
		this.Dispose(false);
	}
	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}
	protected virtual void Dispose(bool disposing)
	{
	}
	public abstract void Initialize();
}
