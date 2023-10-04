using System;

namespace Memory.API
{
    public class APIObject : IDisposable
    {
        private readonly int _number;
        private bool _isDisposed;

        public APIObject(int number)
        {
            _number = number;
            MagicAPI.Allocate(number);
            _isDisposed = false;
        }

        protected virtual void Dispose(bool fromDisposeMethod)
        {
            if (_isDisposed) return;
            _isDisposed = true;
            MagicAPI.Free(_number);
        }

        ~APIObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}