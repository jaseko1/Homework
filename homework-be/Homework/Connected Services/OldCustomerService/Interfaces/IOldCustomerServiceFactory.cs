namespace OldCustomerService
{
    public interface IOldCustomerServiceFactory
    {
        OldCustomerService.ICustomerService CreateService();
    }
}
