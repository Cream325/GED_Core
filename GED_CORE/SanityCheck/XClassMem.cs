using System.Runtime.InteropServices;

namespace GED.Core.SanityCheck
{
    /// <summary>
    /// Represents a memory object without a responsiblity/owning. <br/>
    /// See <see cref="XClassMem"/>.
    /// </summary>
    public unsafe class XClassMemRef
    {
        #region Member Fields

        internal nint bytes;

        #endregion

        #region Constructors

        internal XClassMemRef(nint ptr = 0)
        {
            bytes = ptr;
        }

        #endregion
    }

    /// <summary>
    /// It owns the unmanaged memory.
    /// </summary>
    public unsafe class XClassMem : XClassMemRef
    {
        #region Constructors

        internal XClassMem(out int state, byte* raw, nuint len) : base()
        {
            try
            {
                bytes = Marshal.AllocHGlobal((nint)len);
                byte* __b = (byte*)bytes;
            
                for(nuint i = 0; i < len; i++)
                {
                    __b[i] = raw[i];
                }
            }
            catch
            {
                bytes = 0;
                state = States.ALLOC_FAILED;
                return;
            }

            state = States.OK;
            return;
        }

        internal XClassMem(out int state, byte[] raw) : base()
        {
            nuint rc = (nuint)raw.Length;

            try
            {
                bytes = Marshal.AllocHGlobal((nint)rc);
                byte* __b = (byte*)bytes;

                for(nuint i = 0; i < rc; i++)
                {
                    __b[i] = raw[i];
                }
            }
            catch
            {
                bytes = 0;
                state = States.ALLOC_FAILED;
                return;
            }

            state = States.OK;
            return;
        }

        internal XClassMem(out int state, nuint __sz) : base()
        {
            try
            {
                bytes = Marshal.AllocHGlobal((nint)__sz);

                for(nuint i = 0; i < __sz; i++)
                {
                    ((byte*)bytes)[i] = 0;
                }
            }
            catch
            {
                bytes = 0;
                state = States.ALLOC_FAILED;
                return;
            }

            state = States.OK;
            return;
        }

        #endregion

        #region Destructors

        ~XClassMem()
        {
            if(bytes != 0) Marshal.FreeHGlobal(bytes);
        }

        #endregion
    }
}