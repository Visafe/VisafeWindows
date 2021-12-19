using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFP
{
    // Error: Error processing procedure FwpmFilterAdd0: Expected token of type ParenClose but found Number instead.
    public enum FWP_DATA_TYPE_
    {

        /// FWP_EMPTY -> 0
        FWP_EMPTY = 0,

        FWP_UINT8,

        FWP_UINT16,

        FWP_UINT32,

        FWP_UINT64,

        FWP_INT8,

        FWP_INT16,

        FWP_INT32,

        FWP_INT64,

        FWP_FLOAT,

        FWP_DOUBLE,

        FWP_BYTE_ARRAY16_TYPE,

        FWP_BYTE_BLOB_TYPE,

        FWP_SID,

        FWP_SECURITY_DESCRIPTOR_TYPE,

        FWP_TOKEN_INFORMATION_TYPE,

        FWP_TOKEN_ACCESS_INFORMATION_TYPE,

        FWP_UNICODE_STRING_TYPE,

        /// FWP_SINGLE_DATA_TYPE_MAX -> 0xff
        FWP_SINGLE_DATA_TYPE_MAX = 255,

        FWP_V4_ADDR_MASK,

        FWP_V6_ADDR_MASK,

        FWP_RANGE_TYPE,

        FWP_DATA_TYPE_MAX,
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
    public struct _GUID
    {

        /// unsigned int
        public uint Data1;

        /// unsigned short
        public ushort Data2;

        /// unsigned short
        public ushort Data3;

        /// unsigned char[8]
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 8)]
        public string Data4;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct SID_IDENTIFIER_AUTHORITY
    {

        /// BYTE[6]
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
        public byte[] Value;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct SID
    {

        /// BYTE->unsigned char
        public byte Revision;

        /// BYTE->unsigned char
        public byte SubAuthorityCount;

        /// SID_IDENTIFIER_AUTHORITY->_SID_IDENTIFIER_AUTHORITY
        public SID_IDENTIFIER_AUTHORITY IdentifierAuthority;

        /// DWORD[1]
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U4)]
        public uint[] SubAuthority;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct SEC_WINNT_AUTH_IDENTITY_W
    {

        /// unsigned short*
        public System.IntPtr User;

        /// unsigned int
        public uint UserLength;

        /// unsigned short*
        public System.IntPtr Domain;

        /// unsigned int
        public uint DomainLength;

        /// unsigned short*
        public System.IntPtr Password;

        /// unsigned int
        public uint PasswordLength;

        /// unsigned int
        public uint Flags;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FWP_BYTE_BLOB_
    {

        /// UINT32->unsigned int
        public uint size;

        /// UINT8*
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)]
        public string data;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FWPM_DISPLAY_DATA0_
    {

        /// wchar_t*
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        public string name;

        /// wchar_t*
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        public string description;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FWPM_SESSION0_
    {

        /// GUID->_GUID
        public _GUID sessionKey;

        /// FWPM_DISPLAY_DATA0->FWPM_DISPLAY_DATA0_
        public FWPM_DISPLAY_DATA0_ displayData;

        /// UINT32->int
        public int flags;

        /// UINT32->int
        public int txnWaitTimeoutInMSec;

        /// DWORD->int
        public int processId;

        /// SID*
        public System.IntPtr sid;

        /// wchar_t*
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        public string username;

        /// BOOL->int
        public int kernelMode;
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FWPM_SESSION0
    {
        public _GUID sessionKey;/// GUID->_GUID
        public FWPM_DISPLAY_DATA0_ displayData;/// FWPM_DISPLAY_DATA0->FWPM_DISPLAY_DATA0_
        public uint flags;/// UINT32->unsigned int
        public uint txnWaitTimeoutInMSec;/// UINT32->unsigned int
        public uint processId;/// DWORD->unsigned int
        public IntPtr sid;/// SID*
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        public string username;/// wchar_t*
        public int kernelMode;// BOOL->int
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FWPM_SUBLAYER0_
    {

        /// GUID->_GUID
        public _GUID subLayerKey;

        /// FWPM_DISPLAY_DATA0->FWPM_DISPLAY_DATA0_
        public FWPM_DISPLAY_DATA0_ displayData;

        /// UINT16->unsigned short
        public ushort flags;

        /// GUID*
        public System.IntPtr providerKey;

        /// FWP_BYTE_BLOB->FWP_BYTE_BLOB_
        public FWP_BYTE_BLOB_ providerData;

        /// UINT16->unsigned short
        public ushort weight;
    }

    public enum FWP_MATCH_TYPE_
    {

        /// FWP_MATCH_EQUAL -> 0
        FWP_MATCH_EQUAL = 0,

        /// FWP_MATCH_GREATER -> (FWP_MATCH_EQUAL+1)
        FWP_MATCH_GREATER = (FWP_MATCH_TYPE_.FWP_MATCH_EQUAL + 1),

        /// FWP_MATCH_LESS -> (FWP_MATCH_GREATER+1)
        FWP_MATCH_LESS = (FWP_MATCH_TYPE_.FWP_MATCH_GREATER + 1),

        /// FWP_MATCH_GREATER_OR_EQUAL -> (FWP_MATCH_LESS+1)
        FWP_MATCH_GREATER_OR_EQUAL = (FWP_MATCH_TYPE_.FWP_MATCH_LESS + 1),

        /// FWP_MATCH_LESS_OR_EQUAL -> (FWP_MATCH_GREATER_OR_EQUAL+1)
        FWP_MATCH_LESS_OR_EQUAL = (FWP_MATCH_TYPE_.FWP_MATCH_GREATER_OR_EQUAL + 1),

        /// FWP_MATCH_RANGE -> (FWP_MATCH_LESS_OR_EQUAL+1)
        FWP_MATCH_RANGE = (FWP_MATCH_TYPE_.FWP_MATCH_LESS_OR_EQUAL + 1),

        /// FWP_MATCH_FLAGS_ALL_SET -> (FWP_MATCH_RANGE+1)
        FWP_MATCH_FLAGS_ALL_SET = (FWP_MATCH_TYPE_.FWP_MATCH_RANGE + 1),

        /// FWP_MATCH_FLAGS_ANY_SET -> (FWP_MATCH_FLAGS_ALL_SET+1)
        FWP_MATCH_FLAGS_ANY_SET = (FWP_MATCH_TYPE_.FWP_MATCH_FLAGS_ALL_SET + 1),

        /// FWP_MATCH_FLAGS_NONE_SET -> (FWP_MATCH_FLAGS_ANY_SET+1)
        FWP_MATCH_FLAGS_NONE_SET = (FWP_MATCH_TYPE_.FWP_MATCH_FLAGS_ANY_SET + 1),

        /// FWP_MATCH_EQUAL_CASE_INSENSITIVE -> (FWP_MATCH_FLAGS_NONE_SET+1)
        FWP_MATCH_EQUAL_CASE_INSENSITIVE = (FWP_MATCH_TYPE_.FWP_MATCH_FLAGS_NONE_SET + 1),

        /// FWP_MATCH_NOT_EQUAL -> (FWP_MATCH_EQUAL_CASE_INSENSITIVE+1)
        FWP_MATCH_NOT_EQUAL = (FWP_MATCH_TYPE_.FWP_MATCH_EQUAL_CASE_INSENSITIVE + 1),

        /// FWP_MATCH_TYPE_MAX -> (FWP_MATCH_NOT_EQUAL+1)
        FWP_MATCH_TYPE_MAX = (FWP_MATCH_TYPE_.FWP_MATCH_NOT_EQUAL + 1),
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
    public struct FWP_BYTE_ARRAY6_
    {

        /// UINT8[6]
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 6)]
        public string byteArray6;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
    public struct FWP_BYTE_ARRAY16_
    {

        /// UINT8[16]
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 16)]
        public string byteArray16;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FWP_TOKEN_INFORMATION
    {

        /// ULONG->int
        public int sidCount;

        /// PSID_AND_ATTRIBUTES->_SID_AND_ATTRIBUTES*
        public System.IntPtr sids;

        /// ULONG->int
        public int restrictedSidCount;

        /// PSID_AND_ATTRIBUTES->_SID_AND_ATTRIBUTES*
        public System.IntPtr restrictedSids;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FWP_V4_ADDR_AND_MASK_
    {
        /// UINT32->int
        public int addr;

        /// UINT32->int
        public int mask;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
    public struct FWP_V6_ADDR_AND_MASK_
    {

        /// UINT8[0]
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0)]
        public string addr;

        /// UINT8->char
        public byte prefixLength;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public struct Anonymous_3f540a14_ed1a_423f_8859_8858be876f2d
    {

        /// UINT8->char
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public byte uint8;

        /// UINT16->short
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public short uint16;

        /// UINT32->int
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public int uint32;

        /// UINT64*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr uint64;

        /// INT8->char
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public byte int8;

        /// INT16->short
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public short int16;

        /// INT32->int
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public int int32;

        /// INT64*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr int64;

        /// float
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public float float32;

        /// double*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr double64;

        /// FWP_BYTE_ARRAY16*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr byteArray16;

        /// FWP_BYTE_BLOB*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr byteBlob;

        /// SID*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr sid;

        /// FWP_BYTE_BLOB*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr sd;

        /// FWP_TOKEN_INFORMATION*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr tokenInformation;

        /// FWP_BYTE_BLOB*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr tokenAccessInformation;

        /// LPWSTR->WCHAR*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr unicodeString;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FWP_VALUE0_
    {

        /// FWP_DATA_TYPE->FWP_DATA_TYPE_
        public FWP_DATA_TYPE_ type;

        /// Anonymous_3f540a14_ed1a_423f_8859_8858be876f2d
        public Anonymous_3f540a14_ed1a_423f_8859_8858be876f2d Union1;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FWP_RANGE0_
    {

        /// FWP_VALUE0->FWP_VALUE0_
        public FWP_VALUE0_ valueLow;

        /// FWP_VALUE0->FWP_VALUE0_
        public FWP_VALUE0_ valueHigh;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public struct Anonymous_0a80b53b_64e8_4f48_84aa_30c42e223b3a
    {

        /// GUID->_GUID
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public _GUID filterType;

        /// GUID->_GUID
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public _GUID calloutKey;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FWPM_ACTION0_
    {

        /// FWP_ACTION_TYPE->UINT32->int
        public int type;

        /// Anonymous_0a80b53b_64e8_4f48_84aa_30c42e223b3a
        public Anonymous_0a80b53b_64e8_4f48_84aa_30c42e223b3a Union1;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public struct Anonymous_b08e17c4_df25_4864_a89c_2193fc5df73c
    {

        /// UINT8->char
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public byte uint8;

        /// UINT16->short
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public short uint16;

        /// UINT32->int
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public int uint32;

        /// UINT64*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr uint64;

        /// INT8->char
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public byte int8;

        /// INT16->short
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public short int16;

        /// INT32->int
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public int int32;

        /// INT64*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr int64;

        /// float
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public float float32;

        /// double*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr double64;

        /// FWP_BYTE_ARRAY16*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr byteArray16;

        /// FWP_BYTE_BLOB*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr byteBlob;

        /// SID*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr sid;

        /// FWP_BYTE_BLOB*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr sd;

        /// FWP_BYTE_BLOB*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr tokenInformation;

        /// FWP_BYTE_BLOB*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr tokenAccessInformation;

        /// LPWSTR->WCHAR*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr unicodeString;

        /// FWP_BYTE_ARRAY6*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr byteArray6;

        /// FWP_V4_ADDR_AND_MASK*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr v4AddrMask;

        /// FWP_V6_ADDR_AND_MASK*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr v6AddrMask;

        /// FWP_RANGE0*
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public System.IntPtr rangeValue;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FWP_CONDITION_VALUE0_
    {

        /// FWP_DATA_TYPE->FWP_DATA_TYPE_
        public FWP_DATA_TYPE_ type;

        /// Anonymous_b08e17c4_df25_4864_a89c_2193fc5df73c
        public Anonymous_b08e17c4_df25_4864_a89c_2193fc5df73c Union1;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FWPM_FILTER_CONDITION0_
    {

        /// GUID->_GUID
        public _GUID fieldKey;

        /// FWP_MATCH_TYPE->FWP_MATCH_TYPE_
        public FWP_MATCH_TYPE_ matchType;

        /// FWP_CONDITION_VALUE0->FWP_CONDITION_VALUE0_
        public FWP_CONDITION_VALUE0_ conditionValue;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public struct Anonymous_8a20add2_1a7e_403a_b71c_912925f09c93
    {

        /// UINT64->__int64
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public long rawContext;

        /// GUID->_GUID
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public _GUID providerContextKey;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FWPM_FILTER0_
    {

        /// GUID->_GUID
        public _GUID filterKey;

        /// FWPM_DISPLAY_DATA0->FWPM_DISPLAY_DATA0_
        public FWPM_DISPLAY_DATA0_ displayData;

        /// UINT32->int
        public int flags;

        /// GUID*
        public System.IntPtr providerKey;

        /// FWP_BYTE_BLOB->FWP_BYTE_BLOB_
        public FWP_BYTE_BLOB_ providerData;

        /// GUID->_GUID
        public _GUID layerKey;

        /// GUID->_GUID
        public _GUID subLayerKey;

        /// FWP_VALUE0->FWP_VALUE0_
        public FWP_VALUE0_ weight;

        /// UINT32->int
        public int numFilterConditions;

        /// FWPM_FILTER_CONDITION0*
        public System.IntPtr filterCondition;

        /// FWPM_ACTION0->FWPM_ACTION0_
        public FWPM_ACTION0_ action;

        /// Anonymous_8a20add2_1a7e_403a_b71c_912925f09c93
        public Anonymous_8a20add2_1a7e_403a_b71c_912925f09c93 Union1;

        /// GUID*
        public System.IntPtr reserved;

        /// UINT64->__int64
        public long filterId;

        /// FWP_VALUE0->FWP_VALUE0_
        public FWP_VALUE0_ effectiveWeight;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct _SEC_WINNT_AUTH_IDENTITY_W
    {

        /// short*
        public System.IntPtr User;

        /// int
        public int UserLength;

        /// short*
        public System.IntPtr Domain;

        /// int
        public int DomainLength;

        /// short*
        public System.IntPtr Password;

        /// int
        public int PasswordLength;

        /// int
        public int Flags;
    }

    static class RPC
    {
        public static uint RPC_C_AUTHN_NONE = 0;//No authentication.
        public static uint RPC_C_AUTHN_DCE_PRIVATE = 1;//DCE private key authentication.
        public static uint RPC_C_AUTHN_DCE_PUBLIC = 2;//DCE public key authentication.
        public static uint RPC_C_AUTHN_DEC_PUBLIC = 4;//DEC public key authentication.Reserved for future use.
        public static uint RPC_C_AUTHN_GSS_NEGOTIATE = 9;//Snego security support provider.
        public static uint RPC_C_AUTHN_WINNT = 10;//NTLMSSP
        public static uint RPC_C_AUTHN_GSS_SCHANNEL = 14;//Schannel security support provider. This authentication service supports SSL 2.0, SSL 3.0, TLS, and PCT.
        public static uint RPC_C_AUTHN_GSS_KERBEROS = 16;//Kerberos security support provider.
        public static uint RPC_C_AUTHN_DPA = 17;//DPA security support provider.
        public static uint RPC_C_AUTHN_MSN = 18;//MSN security support provider.
        public static uint RPC_C_AUTHN_KERNEL = 20;//Kernel security support provider.
        public static uint RPC_C_AUTHN_DIGEST = 21;//Digest security support provider.
        public static uint RPC_C_AUTHN_NEGO_EXTENDER = 30;//NEGO extender security support provider.
        public static uint RPC_C_AUTHN_PKU2U = 31;//PKU2U security support provider.
        public static uint RPC_C_AUTHN_MQ = 100;//MQ security support provider.
        public static uint RPC_C_AUTHN_DEFAULT = 0xFFFFFFFF; //The system default authentication service. When this value is specified, COM uses its normal security blanket negotiation algorithm to pick an authentication service.For more information, see Security Blanket Negotiation. 
    }

    public partial class NativeMethods
    {
        /// Return Type: DWORD->int
        ///serverName: wchar_t*
        ///authnService: UINT32->int
        ///authIdentity: SEC_WINNT_AUTH_IDENTITY_W*
        ///session: FWPM_SESSION0*
        ///engineHandle: HANDLE*
        [System.Runtime.InteropServices.DllImportAttribute("<Unknown>", EntryPoint = "FwpmEngineOpen0")]
        public static extern int FwpmEngineOpen0([System.Runtime.InteropServices.InAttribute()][System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string serverName, uint authnService, [System.Runtime.InteropServices.In()] IntPtr authIdentity, ref FWPM_SESSION0_ session, ref System.IntPtr engineHandle);
        //public static extern int FwpmEngineOpen0([System.Runtime.InteropServices.InAttribute()][System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string serverName, uint authnService, ref _SEC_WINNT_AUTH_IDENTITY_W authIdentity, ref FWPM_SESSION0_ session, ref System.IntPtr engineHandle);


        /// Return Type: DWORD->unsigned int
        ///engineHandle: HANDLE->void*
        ///subLayer: FWPM_SUBLAYER0*
        ///sd: PSECURITY_DESCRIPTOR->PVOID->void*
        [System.Runtime.InteropServices.DllImportAttribute("FWPUClnt.dll", EntryPoint = "FwpmSubLayerAdd0")]
        public static extern uint FwpmSubLayerAdd0([System.Runtime.InteropServices.InAttribute()] System.IntPtr engineHandle, [System.Runtime.InteropServices.InAttribute()] ref FWPM_SUBLAYER0_ subLayer, [System.Runtime.InteropServices.InAttribute()] System.IntPtr sd);

    }
}
