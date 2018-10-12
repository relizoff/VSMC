using System;
using System.Threading;
using System.Threading.Tasks;
using Catel.Fody;
using Catel.Logging;
using Catel.Services;

namespace VSMC.ViewControllers
{
    public abstract class CancelableControllerBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private CancellationTokenSource _cancellationTokenSource;
        private readonly object _syncRoot = new object();

        protected virtual void OnStartOperation()
        {

        }

        protected virtual void OnEndOperation(CancellationToken cancellationToken)
        {

        }

        public Task<TOut> RunAsync<TOut>([NotNull] Func<CancellationToken, Task<TOut>> action, CancellationToken cancellationToken)
        {
            cancellationToken = CancelCurrentOperationIfActiveAndReplaceToken(cancellationToken);
            try
            {
                OnStartOperation();
                return action.Invoke(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Log.Debug("Operation canceled.");
                return Task.FromCanceled<TOut>(cancellationToken);
            }
            finally
            {
                OnEndOperation(cancellationToken);
            }
        }

        public Task<TOut> RunAsync<TOut>([NotNull] Func<CancellationToken, Task<TOut>> action)
        {
            var cancellationToken = CancelCurrentOperationIfActiveAndGetToken();
            try
            {
                OnStartOperation();
                return action.Invoke(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Log.Debug("Operation canceled.");
                return Task.FromCanceled<TOut>(cancellationToken);
            }
            finally
            {
                OnEndOperation(cancellationToken);
            }
        }

        public Task RunAsync([NotNull] Func<CancellationToken, Task> action, CancellationToken cancellationToken)
        {
            cancellationToken = CancelCurrentOperationIfActiveAndReplaceToken(cancellationToken);
            try
            {
                OnStartOperation();
                return action.Invoke(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Log.Debug("Operation canceled.");
                return Task.FromCanceled(cancellationToken);
            }
            finally
            {
                OnEndOperation(cancellationToken);
            }
        }

        public Task RunAsync([NotNull] Func<CancellationToken, Task> action)
        {
            var cancellationToken = CancelCurrentOperationIfActiveAndGetToken();
            try
            {
                OnStartOperation();
                return action.Invoke(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Log.Debug("Operation canceled.");
                return Task.FromCanceled(cancellationToken);
            }
            finally
            {
                OnEndOperation(cancellationToken);
            }
        }

        public TOut Run<TOut>([NotNull]Func<CancellationToken, TOut> action)
        {
            var cancellationToken = CancelCurrentOperationIfActiveAndGetToken();
            try
            {
                OnStartOperation();
                return action.Invoke(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Log.Debug("Operation canceled.");
                return default(TOut);
            }
            finally
            {
                OnEndOperation(cancellationToken);
            }
        }

        public CancellationToken Run([NotNull]Action<CancellationToken> action)
        {
            var cancellationToken = CancelCurrentOperationIfActiveAndGetToken();
            try
            {
                OnStartOperation();
                action.Invoke(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Log.Debug("Operation canceled.");
            }
            finally
            {
                OnEndOperation(cancellationToken);
            }

            return cancellationToken;
        }

        public TOut Run<TOut>([NotNull]Func<CancellationToken, TOut> action, CancellationToken cancellationToken)
        {
            cancellationToken = CancelCurrentOperationIfActiveAndReplaceToken(cancellationToken);
            try
            {
                OnStartOperation();
                return action.Invoke(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Log.Debug("Operation canceled.");
                return default(TOut);
            }
            finally
            {
                OnEndOperation(cancellationToken);
            }
        }

        public CancellationToken Run([NotNull]Action<CancellationToken> action, CancellationToken cancellationToken)
        {
            cancellationToken = CancelCurrentOperationIfActiveAndReplaceToken(cancellationToken);
            try
            {
                OnStartOperation();
                action.Invoke(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Log.Debug("Operation canceled.");
            }
            finally
            {
                OnEndOperation(cancellationToken);
            }

            return cancellationToken;
        }

        public void Cancel()
        {
            lock (_syncRoot)
            {
                if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }

                _cancellationTokenSource = null;
            }
        }

        private CancellationToken CancelCurrentOperationIfActiveAndGetToken()
        {
            lock (_syncRoot)
            {
                if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }

                _cancellationTokenSource = new CancellationTokenSource();

                return _cancellationTokenSource.Token;
            }
        }

        private CancellationToken CancelCurrentOperationIfActiveAndReplaceToken(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return cancellationToken;
            }

            lock (_syncRoot)
            {
                if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }

                _cancellationTokenSource = new CancellationTokenSource();

                cancellationToken.Register(() => _cancellationTokenSource.Cancel());

                if (cancellationToken.IsCancellationRequested)
                {
                    return cancellationToken;
                }

                return _cancellationTokenSource.Token;
            }
        }

    }
}