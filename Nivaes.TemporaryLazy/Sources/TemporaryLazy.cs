namespace Nivaes
{
    using System;

    public class TemporaryLazy<T>
    {
        private readonly Func<T> mFactory;
        private readonly TimeSpan mLifetime;
        private readonly object mValueLock = new object();

        private T mValue;
        private DateTime mCreationTime;

        public TemporaryLazy(Func<T> factory, TimeSpan lifetime)
        {
            mFactory = factory;
            mLifetime = lifetime;
        }

        public bool HasValue
        {
            get
            {
                lock (mValueLock)
                {
                    return !(mValue == null && mCreationTime.Add(mLifetime) > DateTime.UtcNow);
                }
            }
        }

        public T Value
        {
            get
            {
                DateTime now = DateTime.UtcNow;
                lock (mValueLock)
                {
                    if (mValue == null || mCreationTime.Add(mLifetime) > DateTime.UtcNow)
                    {
                        mValue = mFactory();
                        mCreationTime = DateTime.UtcNow;
                    }

                    return mValue;
                }
            }
        }
    }
}
