using System.IO;
using System.Text.Json;
using Aetherium.Utility;

namespace Aetherium.Configuration;

/// <summary>
/// Configuration to store settings for a Aetherium plugin.
/// </summary>
public sealed class PluginConfigurations
{
    private readonly DirectoryInfo configDirectory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfigurations"/> class.
    /// </summary>
    /// <param name="storageFolder">Directory for storage of plugin configuration files.</param>
    public PluginConfigurations(string storageFolder)
    {
        this.configDirectory = new DirectoryInfo(storageFolder);
        this.configDirectory.Create();
    }

    /// <summary>
    /// Save/Load plugin configuration.
    /// NOTE: Save/Load are still using Type information for now,
    /// despite LoadForType superseding Load and not requiring or using it.
    /// It might be worth removing the Type info from Save, to strip it from all future saved configs,
    /// and then Load() can probably be removed entirely.
    /// </summary>
    /// <param name="config">Plugin configuration.</param>
    /// <param name="pluginName">Plugin name.</param>
    public void Save(IPluginConfiguration config, string pluginName)
    {
        Util.WriteAllTextSafe(this.GetConfigFile(pluginName).FullName, SerializeConfig(config));
    }

    /// <summary>
    /// Load plugin configuration.
    /// </summary>
    /// <param name="pluginName">Plugin name.</param>
    /// <returns>Plugin configuration.</returns>
    public IPluginConfiguration? Load(string pluginName)
    {
        var path = this.GetConfigFile(pluginName);

        if (!path.Exists)
            return null;

        return DeserializeConfig(File.ReadAllText(path.FullName));
    }

    /// <summary>
    /// Delete the configuration file and folder for the specified plugin.
    /// This will throw an <see cref="IOException"/> if the plugin did not correctly close its handles.
    /// </summary>
    /// <param name="pluginName">The name of the plugin.</param>
    public void Delete(string pluginName)
    {
        var directory = this.GetDirectoryPath(pluginName);
        if (directory.Exists)
            directory.Delete(true);

        var file = this.GetConfigFile(pluginName);
        if (file.Exists)
            file.Delete();
    }

    /// <summary>
    /// Get plugin directory.
    /// </summary>
    /// <param name="pluginName">Plugin name.</param>
    /// <returns>Plugin directory path.</returns>
    public string GetDirectory(string pluginName)
    {
        try
        {
            var path = this.GetDirectoryPath(pluginName);
            if (!path.Exists)
            {
                path.Create();
            }

            return path.FullName;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Load Plugin configuration. Parameterized deserialization.
    /// Currently this is called via reflection from AetheriumPluginInterface.GetPluginConfig().
    /// Eventually there may be an additional pluginInterface method that can call this directly
    /// without reflection - for now this is in support of the existing plugin api.
    /// </summary>
    /// <param name="pluginName">Plugin Name.</param>
    /// <typeparam name="T">Configuration Type.</typeparam>
    /// <returns>Plugin Configuration.</returns>
    public T LoadForType<T>(string pluginName) where T : IPluginConfiguration
    {
        var path = this.GetConfigFile(pluginName);

        return !path.Exists ? default : JsonSerializer.Deserialize<T>(File.ReadAllText(path.FullName));

        // intentionally no type handling - it will break when updating a plugin at runtime
        // and turns out to be unnecessary when we fully qualify the object type
    }

    /// <summary>
    /// Get FileInfo to plugin config file.
    /// </summary>
    /// <param name="pluginName">InternalName of the plugin.</param>
    /// <returns>FileInfo of the config file.</returns>
    public FileInfo GetConfigFile(string pluginName) => new(Path.Combine(this.configDirectory.FullName, $"{pluginName}.json"));

    /// <summary>
    /// Serializes a plugin configuration object.
    /// </summary>
    /// <param name="config">The configuration object.</param>
    /// <returns>A string representing the serialized configuration object.</returns>
    internal static string SerializeConfig(object? config)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        return JsonSerializer.Serialize(config, options);
    }

    /// <summary>
    /// Deserializes a plugin configuration from a string.
    /// </summary>
    /// <param name="data">The serialized configuration.</param>
    /// <returns>The configuration object, or null.</returns>
    internal static IPluginConfiguration? DeserializeConfig(string data)
    {
        return JsonSerializer.Deserialize<IPluginConfiguration>(data, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    private DirectoryInfo GetDirectoryPath(string pluginName) => new(Path.Combine(this.configDirectory.FullName, pluginName));
}
