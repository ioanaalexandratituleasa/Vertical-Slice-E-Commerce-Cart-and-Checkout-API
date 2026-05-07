using System.Data;

namespace VerticalSlice_Backend.Common
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
