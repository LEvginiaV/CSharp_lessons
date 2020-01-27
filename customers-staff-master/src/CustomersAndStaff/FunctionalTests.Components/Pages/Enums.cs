namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages
{
    public enum ErrorSide
    {
        LeftAndRight,
        Left,
        Right,
    }

    public enum Level
    {
        Ok,
        Warning,
        Error,
    }
    
    public enum UpdateCardType
    {
        SwitchFromServiceToProduct,
        ArchiveCard,
        DeleteCard,
        UpdatePrice,
        DeletePrice,
    }
}
