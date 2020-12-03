namespace TShockAPI.Configuration
{
	/// <summary>
	/// Describes a generic configuration interface wrapping some settings
	/// </summary>
	/// <typeparam name="TSettings"></typeparam>
	public interface IConfigFile<TSettings>
	{
		/// <summary>
		/// Settings managed by this config file
		/// </summary>
		TSettings Settings { get; set; }

		/// <summary>
		/// Reads settings from a given path
		/// </summary>
		/// <param name="path">The path to the file containing the settings</param>
		/// <param name="incompleteSettings">
		/// Whether the settings object has any new fields in it, meaning that the configuration should be
		/// overwritten.
		/// </param>
		/// <returns>Settings object</returns>
		TSettings Read(string path, out bool incompleteSettings);
		/// <summary>
		/// Reads settings from a given stream
		/// </summary>
		/// <param name="stream">The stream containing the settings</param>
		/// <param name="incompleteSettings">
		/// Whether the settings object has any new fields in it, meaning that the configuration should be
		/// overwritten.
		/// </param>
		/// <returns>Settings object</returns>
		TSettings Read(System.IO.Stream stream, out bool incompleteSettings);

		/// <summary>
		/// Converts a json-formatted string into the settings object used by this configuration
		/// </summary>
		/// <param name="json">Json string to parse</param>
		/// <param name="incompleteSettings">Whether or not the json string contained an incomplete set of settings</param>
		/// <returns>Settings object</returns>
		TSettings ConvertJson(string json, out bool incompleteSettings);

		/// <summary>
		/// Writes this configuration to a given path
		/// </summary>
		/// <param name="path">File location the configuration will be written to</param>
		void Write(string path);
		/// <summary>
		/// Writes this configuration to a stream
		/// </summary>
		/// <param name="stream">Stream the configuration will be written to</param>
		void Write(System.IO.Stream stream);

	}
}
