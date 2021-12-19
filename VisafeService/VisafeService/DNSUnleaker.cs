using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFP;

namespace VisafeService
{
    public class DNSUnleaker
    {
        private int _blockPort = 53;

        public DNSUnleaker()
        {

        }

        //public _GUID ToGUID(Guid guid)
        //{
        //    byte[] guidData = guid.ToByteArray();

        //    _GUID newGuid = new _GUID();
        //    newGuid.Data1 = guidData[0];

        //    newGuid.Data2 = guidData[1];
        //    newG
        //}

        unsafe public void Execute()
        {
            FWPM_SESSION0_ session = new FWPM_SESSION0_();
            session.flags = 0xFFF;
            IntPtr engineHandle = new IntPtr();
            NativeMethods.FwpmEngineOpen0(null, RPC.RPC_C_AUTHN_WINNT, IntPtr.Zero, ref session, ref engineHandle);

            // create a subLayer to attach filters to
            //var subLayerGuid = Guid.NewGuid();
            //var subLayer = new FWPM_SUBLAYER0_();
            //subLayer.subLayerKey = subLayerGuid;
            //subLayer.displayData.name = DisplayName;
            //subLayer.displayData.description = DisplayName;
            //subLayer.flags = 0;
            //subLayer.weight = 0x100;
        }
    }
}
