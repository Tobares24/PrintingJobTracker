using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Data.Common;

namespace PrintingJobTracker.Infrastructure.Persistence
{
    public class SqlConnectionPoolService<TContext> where TContext : DbContext
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentQueue<KeyValuePair<DateTime, DbConnection>> _availableConnections;
        private readonly List<KeyValuePair<DateTime, DbConnection>> _inUseConnections;
        private readonly int _poolSize;
        private readonly string _connectionString;

        public SqlConnectionPoolService(int poolSize, string connectionString)
        {
            _poolSize = poolSize;
            _connectionString = connectionString;
            _semaphore = new SemaphoreSlim(poolSize, poolSize);
            _availableConnections = new ConcurrentQueue<KeyValuePair<DateTime, DbConnection>>();
            _inUseConnections = new List<KeyValuePair<DateTime, DbConnection>>();
            InitializeConnections();
        }

        private void InitializeConnections()
        {
            try
            {
                for (int i = 0; i < _poolSize; i++)
                {
                    var connection = CreateNewConnection();
                    _availableConnections.Enqueue(new KeyValuePair<DateTime, DbConnection>(DateTime.Now, connection));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DbConnection CreateNewConnection()
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection();
                sqlConnection.ConnectionString = _connectionString;
                sqlConnection.Open();
                return sqlConnection;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DbConnection?> GetConnectionAsync()
        {
            try
            {
                await _semaphore.WaitAsync();

                KeyValuePair<DateTime, DbConnection> connection;

                lock (_inUseConnections)
                {
                    if (_availableConnections.TryDequeue(out connection))
                    {
                        if (!IsConnectionAlive(connection))
                        {
                            connection = new KeyValuePair<DateTime, DbConnection>(DateTime.Now, CreateNewConnection());
                        }

                        _inUseConnections.Add(connection);
                    }

                }
                return connection.Value;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ReleaseConnection(DbConnection connection)
        {
            try
            {
                if (connection is not null)
                {
                    KeyValuePair<DateTime, DbConnection> keyValue;
                    lock (_inUseConnections)
                    {
                        keyValue = _inUseConnections.FirstOrDefault(x => x.Value.Equals(connection));

                        _inUseConnections.Remove(keyValue);

                        _availableConnections.Enqueue(keyValue);

                        _semaphore.Release();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool IsConnectionAlive(KeyValuePair<DateTime, DbConnection> connection)
        {
            try
            {
                string tiempoActualizacionSQLPool = Environment.GetEnvironmentVariable("TIEMPO_ACTUALIZACION_SQLPOOL") ?? "5";
                return connection.Value.State == System.Data.ConnectionState.Open && (DateTime.Now - connection.Key).TotalMinutes <= int.Parse(tiempoActualizacionSQLPool);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
