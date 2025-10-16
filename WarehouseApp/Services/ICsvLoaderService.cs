using System.Threading.Tasks;

namespace WarehouseApp.Services;

public interface ICsvLoaderService
{
    Task LoadCsvAsync();
}