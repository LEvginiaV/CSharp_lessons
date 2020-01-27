using System.Collections.Generic;
using System.IO;

namespace Market.CustomersAndStaff.GodLikeTools
{
    public class CommandLine : ICommandLine
    {
        public CommandLine(string[] args)
        {
            commandLineSettings = ParseCommandLine(args);
            if (commandLineSettings.ContainsKey(workinDirectorySettingName))
            {
                workingDirectory = commandLineSettings[workinDirectorySettingName];
                hasWorkingDirectory = true;
            }
        }

        public string TryGetCommandLineSetting(string settingName)
        {
            if (!commandLineSettings.ContainsKey(settingName))
                return null;
            return commandLineSettings[settingName];
        }

        public string GetCommandLineSetting(string settingName)
        {
            if (!commandLineSettings.ContainsKey(settingName))
                throw new BadCommandLineException("The setting '{0}' not found in command line", settingName);
            return commandLineSettings[settingName];
        }

        public string GetFilePathFromSetting(string settingName)
        {
            var settingValue = GetCommandLineSetting(settingName);
            return hasWorkingDirectory ? Path.Combine(workingDirectory, settingValue) : settingValue;
        }

        private static bool IsCommandLineSetting(string commandLineArgument)
        {
            return commandLineArgument.StartsWith("-");
        }

        private static IDictionary<string, string> ParseCommandLine(string[] args)
        {
            var result = new Dictionary<string, string>();
            var i = 0;
            while (i < args.Length - 1)
            {
                if (!IsCommandLineSetting(args[i]))
                {
                    i++;
                    continue;
                }
                if (result.ContainsKey(args[i]))
                    throw new BadCommandLineException("Double setting '{0}'", args[i]);
                result.Add(args[i], args[i + 1]);
                i += 2;
            }
            return result;
        }

        private readonly string workingDirectory;
        private readonly bool hasWorkingDirectory;

        private readonly IDictionary<string, string> commandLineSettings;
        private const string workinDirectorySettingName = "-wd";
    }
}