namespace Market.CustomersAndStaff.GodLikeTools
{
    public interface ICommandLine
    {
        string TryGetCommandLineSetting(string settingName);
        string GetCommandLineSetting(string settingName);
        string GetFilePathFromSetting(string settingName);
    }
}