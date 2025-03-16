﻿using Microsoft.PowerPlatform.Dataverse.Client;

namespace DataverseBenchmarkProject.Connections
{
    /// <summary>
    /// Represents a connection to Dataverse.
    /// </summary>
    public class Connection : IDisposable
    {
        // Constants for connection expiration
        private const int _connectionLifetimeMinutes = 55;
        private const int _preemptiveRenewalThresholdMinutes = 5;

        // Connection properties
        public required string ConnectionString { get; set; }
        private int _counter = 0;
        public int Counter => _counter;

        // Connection state
        private ServiceClient? _currentClient;
        private DateTime _expirationTime = DateTime.MinValue;
        private Task<ServiceClient>? _renewalTask;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);
        private bool _disposed;

        /// <summary>
        /// Gets a ServiceClient instance, creating or renewing it if necessary.
        /// </summary>
        /// <returns>A ServiceClient instance.</returns>
        public ServiceClient GetServiceClient()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(Connection));
            }

            // Fast path - if we have a valid client, increment counter and return it
            if (_currentClient != null && DateTime.UtcNow < _expirationTime)
            {
                // Check if we need to start preemptive renewal
                if (_renewalTask == null && ShouldStartRenewal())
                {
                    StartRenewalTask();
                }

                Interlocked.Increment(ref _counter);
                return _currentClient;
            }

            // Slow path - we need to create or renew the client
            _connectionLock.Wait();
            try
            {
                // Check again after acquiring the lock
                if (_currentClient != null && DateTime.UtcNow < _expirationTime)
                {
                    Interlocked.Increment(ref _counter);
                    return _currentClient;
                }

                // If we have a completed renewal task, use its result
                if (_renewalTask != null && _renewalTask.IsCompleted && !_renewalTask.IsFaulted)
                {
                    SetNewClient(_renewalTask.Result);
                    _renewalTask = null;
                }
                // Otherwise, create a new client synchronously
                else
                {
                    var newClient = CreateServiceClient();
                    if (!newClient.IsReady)
                    {
                        throw new InvalidOperationException(
                            "Failed to create a ready ServiceClient."
                        );
                    }
                    SetNewClient(newClient);
                }

                Interlocked.Increment(ref _counter);
                return _currentClient!;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        /// <summary>
        /// Determines if we should start a renewal task.
        /// </summary>
        /// <returns>True if renewal should start, false otherwise.</returns>
        private bool ShouldStartRenewal()
        {
            var timeUntilExpiration = _expirationTime - DateTime.UtcNow;
            return timeUntilExpiration.TotalMinutes <= _preemptiveRenewalThresholdMinutes;
        }

        /// <summary>
        /// Starts a task to renew the connection.
        /// </summary>
        private void StartRenewalTask()
        {
            _renewalTask = Task.Run(() => CreateServiceClient());
        }

        /// <summary>
        /// Creates a new ServiceClient instance.
        /// </summary>
        /// <returns>A new ServiceClient instance.</returns>
        private ServiceClient CreateServiceClient()
        {
            return new ServiceClient(ConnectionString) { EnableAffinityCookie = false };
        }

        /// <summary>
        /// Sets a new client as the current client.
        /// </summary>
        /// <param name="newClient">The new ServiceClient instance.</param>
        private void SetNewClient(ServiceClient newClient)
        {
            var oldClient = _currentClient;
            _currentClient = newClient;
            _expirationTime = DateTime.UtcNow.AddMinutes(_connectionLifetimeMinutes);
            Interlocked.Exchange(ref _counter, 0);

            // Dispose the old client in the background
            if (oldClient != null)
            {
                Task.Run(() => oldClient.Dispose());
            }
        }

        /// <summary>
        /// Disposes the Connection instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the Connection instance.
        /// </summary>
        /// <param name="disposing">Whether the method is called from Dispose() or the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _connectionLock.Dispose();
                    _currentClient?.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
