﻿using Microsoft.PowerPlatform.Dataverse.Client;

namespace DataverseBenchmarkProject.Connections
{
    public class XrmConnection : IDisposable
    {
        // Use a more efficient collection for storing connections
        private readonly List<Connection> _connections = [];

        // Use a reader-writer lock for better concurrency
        private readonly ReaderWriterLockSlim _rwLock = new();

        // Track if the object has been disposed
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the XrmConnection class.
        /// </summary>
        /// <param name="connectionString">The connection strings to use.</param>
        public XrmConnection(ConnectionString connectionString)
        {
            InitializeConnections(connectionString.ConnectionStrings).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously initializes connections.
        /// </summary>
        /// <param name="connectionStrings">Array of connection strings.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task InitializeConnections(string[] connectionStrings)
        {
            // Create a list to hold the tasks
            var connectionTasks = new List<Task<Connection?>>();

            // Create a task for each connection string
            foreach (var connectionStr in connectionStrings)
            {
                connectionTasks.Add(CreateConnectionAsync(connectionStr));
            }

            // Wait for all tasks to complete
            var connections = await Task.WhenAll(connectionTasks);

            // Add all valid connections to the list
            _rwLock.EnterWriteLock();
            try
            {
                foreach (var connection in connections)
                {
                    if (connection != null)
                    {
                        _connections.Add(connection);
                    }
                }
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }

            // Ensure we have at least one valid connection
            if (_connections.Count == 0)
            {
                throw new InvalidOperationException("No valid connections could be established.");
            }
        }

        /// <summary>
        /// Creates a connection asynchronously.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A task that returns a Connection object or null if the connection failed.</returns>
        private async Task<Connection?> CreateConnectionAsync(string connectionString)
        {
            try
            {
                var conn = new Connection { ConnectionString = connectionString };
                // Check if the connection is ready
                var client = await Task.Run(() => conn.GetServiceClient());
                return client.IsReady ? conn : null;
            }
            catch (Exception)
            {
                // Log the exception if needed
                return null;
            }
        }

        /// <summary>
        /// Gets a service client with the lowest usage count.
        /// </summary>
        /// <returns>A ServiceClient instance.</returns>
        public ServiceClient GetServiceClient()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(XrmConnection));
            }

            // Use a read lock for better concurrency
            _rwLock.EnterReadLock();
            try
            {
                // Find the connection with the minimum counter
                // This is more efficient than using OrderBy().First()
                Connection? minConnection = null;
                int minCounter = int.MaxValue;

                foreach (var connection in _connections)
                {
                    if (connection.Counter < minCounter)
                    {
                        minCounter = connection.Counter;
                        minConnection = connection;
                    }
                }

                if (minConnection == null)
                {
                    throw new InvalidOperationException("No valid connections available.");
                }

                return minConnection.GetServiceClient();
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Disposes the XrmConnection instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the XrmConnection instance.
        /// </summary>
        /// <param name="disposing">Whether the method is called from Dispose() or the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _rwLock.Dispose();

                    // Dispose all connections
                    foreach (var connection in _connections)
                    {
                        if (connection is IDisposable disposableConnection)
                        {
                            disposableConnection.Dispose();
                        }
                    }

                    // Clear the connections list
                    _connections.Clear();
                }

                _disposed = true;
            }
        }
    }
}
