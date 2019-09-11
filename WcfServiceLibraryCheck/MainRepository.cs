using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DataModelLibrary;

namespace WcfServiceLibraryCheck
{
    public class MainRepository : IRepository
    {
        private readonly string _connectionString;

        public MainRepository(string connectionString)
        {
            _connectionString = connectionString;
            Name = "Main Repository";
        }

        public string Name { get; private set; }

        public List<Check> GetLastPack(int packSize)
        {
            var queryParameters = new DynamicParameters();
            queryParameters.Add("@parPackSize", packSize);
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Check>("spGetLastPack", queryParameters,
                commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public int SaveCheck(Check check)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string sql = "spSaveCheck";
                var affectedRows = connection.Execute(sql,
                    new { parCheckNumber = check.CheckNumber, parSumm = check.Summ, parDiscount = check.Discount, parArticles = check.Articles },
                    commandType: CommandType.StoredProcedure);

                return affectedRows;
            }
        }
    }
}
