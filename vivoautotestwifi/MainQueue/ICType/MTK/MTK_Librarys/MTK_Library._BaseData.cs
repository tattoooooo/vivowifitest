using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace vivoautotestwifi.MainQueue.ICType.MTK.MTK_Librarys
{
    public partial class MTK_Library
    {
        int m_nDutID = 0;
        int m_hMeta = 0;
        int m_nStopFlag = 0;


        UInt32 m_wifiChipVersion = 0;

        const int MAX_DUT_INIT = 16;
        const int MAX_BARCODE_SIZE = 64;
        const int MAX_TIME_SIZE = 32;
        const int MOBILE_LOG_SOCKET_PORT = 10119;
        const int MODEM_LOG_SOCKET_PORT = 10001;


        public enum METAAPP_CONN_TYPE_E
        {
            METAAPP_CONN_CONNCET_BY_UART = 0,
            METAAPP_CONN_CONNCET_BY_USB = 1,
            METAAPP_CONN_CONNCET_BY_WIFI = 2,
            METAAPP_CONN_CONNCET_NUM = METAAPP_CONN_CONNCET_BY_WIFI + 1
        }

        /// <summary>
        /// 连接结果
        /// </summary>
        public enum METAAPP_HIGH_LEVEL_RESULT
        {
            METAAPP_HIGH_LEVEL_SUCCESS = 0,
            METAAPP_HIGH_LEVEL_FAIL = 1,
            METAAPP_HIGH_LEVEL_HANDLE_ALLOC_FAIL = 2,
            METAAPP_HIGH_LEVEL_OPEN_DLL_LOG_FAIL = 3,
            METAAPP_HIGH_LEVEL_GET_PRELOAD_PORT_FAIL = 4,
            METAAPP_HIGH_LEVEL_BOOT_PRELOAD_FAIL = 5,
            METAAPP_HIGH_LEVEL_GET_KERNEL_PORT = 6,
            METAAPP_HIGH_LEVEL_CONNECT_AP_META_FAIL = 7,
            METAAPP_HIGH_LEVEL_GET_AP_VERSION_FAIL = 8,
            METAAPP_HIGH_LEVEL_LOAD_AP_DATABASE_FROM_DUT_FAIL = 9,
            METAAPP_HIGH_LEVEL_LOAD_MD_DATABASE_FROM_DUT_FAIL = 10,
            METAAPP_HIGH_LEVEL_INIT_AP_DATABASE_FAIL = 11,
            METAAPP_HIGH_LEVEL_INIT_MINICOMLOGGER_FAIL = 12,
            METAAPP_HIGH_LEVEL_OPEN_MODEM_LOG_FAIL = 13,
            METAAPP_HIGH_LEVEL_AP_DATABASE_NOT_MATCH = 14,
            METAAPP_HIGH_LEVEL_MD_DATABASE_NOT_MATCH = 15,

            METAAPP_HIGH_LEVEL_REBOOT_MODEM_FAIL = 51,
            METAAPP_HIGH_LEVEL_GET_SP_MODEM_INFO_FAIL = 52,
            METAAPP_HIGH_LEVEL_DISCON_AP_TO_MODEM_FAIL = 53,
            METAAPP_HIGH_LEVEL_GET_MODEM_VERSION_FAIL = 54,
            METAAPP_HIGH_LEVEL_INIT_MODEM_DATABASE_FAIL = 55,
            METAAPP_HIGH_LEVEL_SET_MMC2K_INFO_FAIL = 56,

            METAAPP_HIGH_LEVEL_CONNECT_AP_FAIL = 101,
            METAAPP_HIGH_LEVEL_C2K_INIT_FAIL = 102,
            METAAPP_HIGH_LEVEL_SWTICH_TO_C2K_FAIL = 103,
            METAAPP_HIGH_LEVEL_EXIT_C2K_FAIL = 104,

            METAAPP_HIGH_LEVEL_MD_STATUS_NOT_CORRENT_FAIL = 120,

            METAAPP_HIGH_LEVEL_CLEAN_BOOT_FAIL = 150,
            METAAPP_HIGH_LEVEL_EXIT_META_FAIL = 151,


            METAAPP_HIGH_LEVEL_END = 65536
        }

        /// <summary>
        /// 选择连接的模式
        /// </summary>
        public enum METAAPP_CONN_MODE_E
        {
            METAAPP_CONN_BOOT_META_MODE = 0,                      // boot process
            METAAPP_CONN_ALREADY_IN_META_MODE = 1,                // already in meta mode(skip boot preloader)
            METAAPP_CONN_ATM_MODE = 2,                            // ATM mode
            METAAPP_CONN_MODE_NUM = METAAPP_CONN_ATM_MODE + 1
        }

        //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        //public struct METAAPP_CONN_BOOT_STTING_T
        //{
        //    [MarshalAs(UnmanagedType.SysUInt)]
        //    public IntPtr authHandle;               // void *, for security feature
        //    [MarshalAs(UnmanagedType.SysUInt)]
        //    public IntPtr scertHandle;              // void *, for security feature

        //    [MarshalAs(UnmanagedType.U1)]
        //    public bool bEnableAdbDevice;
        //    public uint uPortNumber;
        //    public ushort uMDMode;
        //}




        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct METAAPP_CONN_BOOT_STTING_T
        {

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool bEnableAdbDevice;

            /// unsigned int
            public uint uPortNumber;

            /// unsigned short
            public ushort uMDMode;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct METAAPP_CONN_AP_SETTING_T
        {
            public int kernelComPort;     //kernel com port
            [MarshalAs(UnmanagedType.U1)]
            public bool bDbFileFromDUT;     //true: database from DUT,false: database transmit by user
            [MarshalAs(UnmanagedType.LPStr)]
            public string pApDbFilePath;    // char *, specify ap nvram db file path, "" means get the file from device.
            [MarshalAs(UnmanagedType.LPStr)]
            public string pMdDbFilePath;    // char *, specify modem nvram db file path, "" means get the file from device. MD database path for WG\LWG\LWTG\LWCTG...database file
            [MarshalAs(UnmanagedType.LPStr)]
            public string pMdDbFilePath1;    // char *, specify modem nvram db file path, "" means get the file from device. MD database path for TG\LTG....database file
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct METAAPP_CONN_FILTER_SETTING_T
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string brom;
            [MarshalAs(UnmanagedType.LPStr)]
            public string preloader;
            [MarshalAs(UnmanagedType.LPStr)]
            public string kernel;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct METAAPP_CONN_LOG_SETTING_T
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool enableDllLog;                            // if bypass borom/preloader handshake
            [MarshalAs(UnmanagedType.U1)]
            public bool enableUartLog;                           // if enable modem log after modem bootup
            [MarshalAs(UnmanagedType.U1)]
            public bool enablePcUsbDriverLog;                    // if enable modem log after modem bootup
            public int iMDLoggEnable;                           //0 disable; 1:ELT Port output 2:Modem log in SD card
            public int iMobileLogEnable;                        //0 disable; 1:ELT Port output 2:Saving in SD card
            public int iConnsysLogEnable;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string IP;                                    //char IP[64];
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_BARCODE_SIZE + MAX_TIME_SIZE)]
            public string pcbSn;                                 //char pcbSn[MAX_BARCODE_SIZE + MAX_TIME_SIZE];
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string logSavePath;                           //char logSavePath[MAX_PATH];
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct METAAPP_CONN_STTING_T
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool autoScanPort;
            [MarshalAs(UnmanagedType.SysUInt)]
            public IntPtr stopFlag;                  //int *

            public METAAPP_CONN_TYPE_E connectType;
            public METAAPP_CONN_MODE_E connectMode;
            public uint uTimeOutMs;
            public METAAPP_CONN_BOOT_STTING_T bootSetting;
            public METAAPP_CONN_AP_SETTING_T connectSetting;
            public METAAPP_CONN_LOG_SETTING_T logSetting;
            public METAAPP_CONN_FILTER_SETTING_T filterSetting;
        };





        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct METAAPP_CONNCT_CNF_T
        {
            public int hMDHandle;
            public uint uPreloaderPortNumber;
            public uint uKernelPortNumber;
        };

        const int MAX_PATH = 260;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct LogConfigure_s
        {
            public int iMDLoggEnable;                    //0 disable; 1:ELT Port output 2:Modem log in SD card
            [MarshalAs(UnmanagedType.U1)]
            public bool bSDMDLoggEnable;
            public int iMobileLogEnable;                 //0 disable; 1:ELT Port output 2:Saving in SD card
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_BARCODE_SIZE + MAX_TIME_SIZE)]
            public string pLogName;                      //char pcbSn[MAX_BARCODE_SIZE + MAX_TIME_SIZE];
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string pLogPath;                      //char logSavePath[MAX_PATH];
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct LogWifiCon_s
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string IP;  //char IP[64];
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MiniComLogInput_s
        {
            public LogConfigure_s sLogConfigure;
            public uint nConnectType;
            public LogWifiCon_s sWifiCon;
            public uint m_uKernelCom;
            public uint m_uDebugCom;
        }



        //METACORE

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct VerInfo_V2_Cnf
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string BB_CHIP;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string ECO_VER;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string SW_TIME;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string DSP_FW;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string DSP_PATCH;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string SW_VER;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string HW_VER;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string MELODY_VER;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string BUILD_DISP_ID;
            public byte status;
        };


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct SetCleanBootFlag_REQ
        {
            public int Notused;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string BackupTime;   // unsigned char [64]
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct SetCleanBootFlag_CNF
        {
            int drv_status;
        };


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NVRAM_ACCESS_STRUCT
        {
            public UInt32 dataLen;
            public UInt32 dataOffset; /*set as zero for whole region */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
            public string data;
        }



        public enum ENUM_CFG_SRC_TYPE_T
        {
            CFG_SRC_TYPE_EEPROM,    //cfg data is queried/set from/to EEPROM
            CFG_SRC_TYPE_NVRAM,     //cfg data is queried/set from/to NVRAM
            CFG_SRC_TYPE_BOTH,      //cfg data is queried/set from/to NVRAM, and E2PROM presents too
            CFG_SRC_TYPE_AUTO
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct WIFI_TX_PARAM_T
        {
            public UInt32 bufSize;
            public UInt32 bLongPreamble;
            public UInt32 txRate;
            public UInt32 pktCount;
            public UInt32 pktInterval;
            public UInt32 bGainControl;
            public UInt32 gainControl;
            public UInt32 bTrackAlc;
            public UInt32 bTargetAlc;
            public UInt32 targetAlcValue;
            public UInt32 txAntenna;

        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct BUILD_PROP_REQ_S
        {
            byte[] tag;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct BUILD_PROP_CNF_S
        {
            byte[] content;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct AP_FT_NVRAM_READ_REQ
        {
            byte[] LID;
            ushort RID;

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct AP_FT_NVRAM_READ_CNF
        {
            ushort LID;
            ushort RID;
            byte status;
            UInt32 len;
            byte[] buf;
            byte read_status;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct AP_FT_NVRAM_WRITE_REQ
        {
            byte[] LID;
            ushort RID;
            UInt32 len;
            byte[] buff;

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct AP_FT_NVRAM_WRITE_CNF
        {
            ushort LID;
            ushort RID;
            byte status;
            byte read_status;
        }


        public delegate void SP_META_NVRAM_Read_CNF(ref AP_FT_NVRAM_READ_CNF cnf, short token, ref IntPtr usrData);

        public delegate void SP_META_NVRAM_Write_CNF(ref SP_META_NVRAM_Write_CNF cnf, short token, ref IntPtr usrData);

        /**********************by wzt**********************************/

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct QUERY_WCNDRIVER_READY_CNF
        {
            UInt32 result;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct BT_HCI_COMMAND
        {
            ushort m_opcode;
            byte mlen;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string m_cmd;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct BT_HCI_EVENT
        {
            ushort m_event;
            byte m_status;
            UInt16 m_handle;
            byte m_len;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string m_cmd;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct PHY_Value
        {
            public UInt32 Protocol; /* BIT0 : 11 a/b/g  BIT1: 11n , BIT2: 11ac , BIT3: 11ax */
            public UInt32 AntNum; /* 1:1x1, 2:2x2, ... */
            public UInt32 dbdc;    /* BIT0: DBDC support */
            public UInt32 Coding; /* BIT0: TxLDPC , BTI1 : RxLDPC , BIT2: TxSTBC , BIT3: RxSTBC */
            public UInt32 channel_Band; /* BIT0 : 2.4G  BIT1: 5G , BIT2: 6G */
            public UInt32 Bandwidth; /* BIT0: BW20, BIT1:BW40, BIT2:BW80, BIT3:BW160, BIT4:BW80+80 */
            public UInt32 channel_band_dbdc;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public UInt32[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct PHY_CAP
        {
            public UInt32 Tag;
            public UInt32 Length;
            public PHY_Value phy_value;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct EXT_Value
        {
            public UInt32 Feature1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
            public UInt32[] reserved;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct EXT_CAP
        {
            public UInt32 Tag;
            public UInt32 Length;
            public EXT_Value ext_value;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct CapBuffer
        {
            public UInt32 version;
            public UInt32 TagNum;
            public PHY_CAP phy;
            public EXT_CAP ext;
        }

        public delegate void META_BT_HCI_CNF(ref BT_HCI_EVENT Ccnf, Int16 token, IntPtr usrdata);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct GPS_CMD
        {
            public uint len;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string buff;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct GPS_ACK_BUF
        {
            uint len;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string buff;
        }


        public delegate void META_GPS_SEND_COMMAND_CNF(ref GPS_ACK_BUF cnf, UInt16 token, IntPtr intPtr);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MMRfTestCmdEtSetToolUsage
        {
            public UInt16 rat_idx;
            public UInt16 toolUsage;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct ERfTestCmdTasCfg
        {
            public byte tas_idx;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MMRfTestCmdCfgTas
        {
            public UInt16 rat_idx;
            public byte tas_cfg;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MMRfTestResultCfgTas
        {
            public UInt16 tas_idx;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MMRfTestCmdForceTasStateV7
        {
            public UInt16 rat_idx;
            public byte forced_enable; /**0: disable, 1: enable */
            public byte tx_tas_state;  /** the state of TX path */
            public byte rx_tas_state;  /** the state of RX path */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MMRfTestResultForceTasStateV7
        {
            UInt16 tas_idx;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MMRfTestResultQueryTasVerifyListV7
        {
            public byte status;                  /**< query status */
            public UInt16 gsm_count;               /**< the count of GSM elements */
            public UInt16 td_count;                /**< the count of TD-SCDMA elements */
            public UInt16 c2k_count;               /**< the count of C2K elements */
            public UInt16 wcdma_count;             /**< the count of WCDMA elements */
            public UInt16 lte_count;               /**< the count of LTE elements */
            public UInt16 nr_count;                /**< the count of NR elements */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
            public MMRfTasVerifyListElement[] gsm_list;    /**< the GSM element array */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
            public MMRfTasVerifyListElement[] td_list;     /**< the TD-SCDMA element array */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
            public MMRfTasVerifyListElement[] c2k_list;    /**< the C2K element array */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
            public MMRfTasVerifyListElement[] wcdma_list;  /**< the WCDMA element array */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
            public MMRfTasVerifyListElement[] lte_list;    /**< the LTE element array */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
            public MMRfTasVerifyListElement[] nr_list;     /**< the NR element array */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MMRfTasVerifyListElement
        {
            public UInt16 band;                 /**< band */
            public UInt16 tx_route_idx;         /**< TX route index */
            public UInt16 rx_route_idx;         /**< RX route index */
            public MMRfTasStatePairV7 forced_state;         /**< forced TAS state */
            public MMRfTasPortMap tx_mapping;           /**< TX port mapping */
            public MMRfTasPortMap rxm_mapping;          /**< RX main port mapping */
            public MMRfTasPortMap rxd_mapping;          /**< RX diversity port mapping */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MMRfTasPortMap
        {
            public byte path_logic;           /**< path logic */
            public byte ant_idx;              /**< antenna index */
            public byte carkit_idx;           /**< carkit index */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MMRfTasStatePairV7
        {
            public byte tx_tas_state; /**< the state of TX path */
            public byte rx_tas_state; /**< the state of RX path */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MMRfTestCmdFGetTasStateCfg
        {
            /** 
             * <pre> 
             * the RAT 
             *  1: GSM
             *  2: TDSCDMA
             *  4: C2K
             *  8: WCDMA
             *  16: LTE
             * </pre>
             */
            public UInt16 rat_idx;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MMRfTestResultGetTasStateCfg
        {
            /** 
             * <pre> 
             * the RAT 
             *  1: GSM
             *  2: TDSCDMA
             *  4: C2K
             *  8: WCDMA
             *  16: LTE
             * </pre>
             */
            public UInt16 rat_idx;
            public UInt16 tas_ver;    /**< the TAS version ( \sa E_MMTST_TAS_VER ) */
            public UInt16 band_cnt;   /**< the valid count of TAS state ( for accessing tas_state_cfg[] ) */
            /**
             * \ingroup MultiModeStruct
             */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public MMRfTasStateConfig[] tas_state_cfg; /**< the TAS configuration */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MMRfTasStateConfig
        {
            public UInt16 band;               /**< band */
            public UInt16 tas_state_up_bound; /**< the upper bound for TAS state */
            public UInt16 tas_state_bitmap;   /**< the bit map of TAS state ( tas_state_bitmap = 0xD: the TAS state 0,2,3 need to be toggled) */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MMRfTestCmdGetTasStateCfgV5Cnf
        {
            public UInt16 band_cnt;        /**< the valid band count for accessing the tas_state_cfg[]  */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public MMRfTestCmdTasStateConfigV5_Entry_T[] tas_state_cfg; /**< the UTAS information  */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MMRfTestCmdTasStateConfigV5_Entry_T
        {
            public UInt16 band;                   /**< the band */
            public UInt16 cal_default_state;      /**< the default state for calibration */
            public byte valid_state_num;        /**< the valid number for accessing the valid_state_list[] */
            public byte toggled_state_num;      /**< the valid number for accessing the toggled_state_list[] */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public UInt16[] valid_state_list;    /**< the valid state */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public UInt16[] toggled_state_list;  /**< the toggled state */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MMRfTestCmdRfCapabilityCnf
        {
           public int valid;    /** the response data is valid or not */
           public int status;   /** the execution status code ( 0: successful, others: failed ) */
           public MMRfCapabilityItemSet capabilityItems;   /** the supported capability items */
           public MMRfCalibrationItemSet calibrationItems;  /** the supported calibration items */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MMRfCalibrationItemSet
        {
            public RfFactoryModeCalItem  ratmap_cal_result_share_tadc;       /** [0] the RAT bit map for TADC calibration data sharing      ,is_capable (0: not supported, 1: supported) parameter (0|0|C2k|LTE FDD|LTE TDD|TDSCDMA|WCDMA|GSM) */
            public RfFactoryModeCalItem  ratmap_cal_result_share_afc;        /** [1] the RAT bit map of AFC calibration sharing             ,is_capable (0: not supported, 1: supported) parameter (0|0|C2k|LTE FDD|LTE TDD|TDSCDMA|WCDMA|GSM) */
            public RfFactoryModeCalItem  ratmap_support_RfSelf_cal;          /** [2] the RAT bit map of RF self-calibration    ,is_capable (0: not supported, 1: supported) parameter (0|0|LTE FDD|LTE TDD|WCDMA|C2K|TDSCDMA|GSM) */
            public RfFactoryModeCalItem  ratmap_support_RfSelf_cal_v2;       /** [3] the RAT bit map of RF self-calibration V2 ,is_capable (0: not supported, 1: supported) parameter (0|0|LTE FDD|LTE TDD|WCDMA|C2K|TDSCDMA|GSM) */
            public RfFactoryModeCalItem  ratmap_support_dpd_cal;             /** [4] the RAT bit map of DPD calibration        ,is_capable (0: not supported, 1: supported) parameter (0|0|LTE FDD|LTE TDD|WCDMA|C2K|TDSCDMA|GSM) */
            public RfFactoryModeCalItem  ratmap_support_cim3_cal;            /** [5] the RAT bit map of CIM3 calibration       ,is_capable (0: not supported, 1: supported) parameter (0|0|LTE FDD|LTE TDD|WCDMA|C2K|TDSCDMA|GSM) */
            public RfFactoryModeCalItem  ratmap_support_et_cal;              /** [6] the RAT bit map of ET calibration         ,is_capable (0: not supported, 1: supported) parameter (0|0|LTE FDD|LTE TDD|WCDMA|C2K|TDSCDMA|GSM) */
            public RfFactoryModeCalItem  ratmap_support_TAS;                 /** [7] the RAT bit map of TAS                    ,is_capable (0: not supported, 1: supported) parameter (0|0|LTE FDD|LTE TDD|WCDMA|C2K|TDSCDMA|GSM) */
            public RfFactoryModeCalItem  ratmap_support_WM;                  /** [8] the RAT bit map of world mode ID          ,is_capable (0: not supported, 1: supported) parameter (0|0|LTE FDD|LTE TDD|WCDMA|C2K|TDSCDMA|GSM) */
            public RfFactoryModeCalItem  ratmap_support_tool_usage;          /** [9] the RAT bit map of tool usage             ,is_capable (0: not supported, 1: supported) parameter (0|0|LTE FDD|LTE TDD|WCDMA|C2K|TDSCDMA|GSM) */
            public RfFactoryModeCalItem  support_query_temp_info;            /** [10] support querying the temperature DAC and temperature info.    ,is_capable (0: not supported, 1: supported) parameter (0|0|LTE FDD|LTE TDD|WCDMA|C2K|TDSCDMA|GSM) */
            public RfFactoryModeCalItem  support_query_vpa_voltage_list;     /** [11] support querying the VPA voltage list of PMIC.                ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  support_RfSelf_test_analyzer;       /** [12] support RF self-test analyzer                 ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  support_et_cal_capability;          /** [13] support querying maximum ET route count       ,is_capable (0: not supported, 1: supported) parameter (0: 301, 1:522) */
            public RfFactoryModeCalItem  support_mmrf_afc_nvram;             /** [14] support multi-mode RF AFC APIs or not         ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  ratmap_support_querying_tas_cfg;    /** [15] the RAT bit map TAS configuration info                   ,is_capable (0: not supported, 1: supported) parameter (0|0|LTE FDD|LTE TDD|WCDMA|C2K|TDSCDMA|GSM) */
            public RfFactoryModeCalItem  support_force_trad_cal_apt;         /** [16] support the traditional calibration into APT mode        ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  ratmap_support_RfPostSelf_cal;      /** [17] the RAT bit map of RF post self-calibration              ,is_capable (0: not supported, 1: supported) parameter (0|0|LTE FDD|LTE TDD|WCDMA|C2K|TDSCDMA|GSM) */
            public RfFactoryModeCalItem  support_et_pa_labk_capability;      /** [18] support reporting the ET PA parameters lab calibration   ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  support_query_tms_data_status;      /** [19] Deprecated */
            public RfFactoryModeCalItem  support_mmtst_record_for_cddc;      /** [20] support adding NVRAM record for CDDC                     ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  support_set_apt_pa_setting;         /** [21] support PA parameters setting function for APT mode      ,is_capable (0: not supported, 1: supported) parameter (0|0|LTE FDD|LTE TDD|WCDMA|C2K|TDSCDMA|GSM) */
            public MMRfTest32KLess support_32kless;                    /** [22] support 32K-less                                         ,is_capable (0: not supported, 1: supported) */
            public MMRfTestXtalInfo xtal_info;                          /** [23] Indicate the X-tal type and live in PMIC or no. */
            public RfFactoryModeCalItem  support_mm_cotms_setget;            /** [24] support CO-TMS data operating function                   ,is_capable (0: not supported, 1: supported) */
            public MMRfTestCoTmsCal support_mm_cotms_cal;               /** [25] support CO-TMS calibration                               ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  support_mipi_cw_setget;             /** [26] support MIPI CO-word operating function                  ,is_capable (0: not supported, 1: supported) parameter (V7:2) */
            public RfFactoryModeCalItem  support_mm_cmr_info;                /** [27] support carkit pair information function                 ,is_capable (0: not supported, 1: supported) */
            public MMRfTestSelfCal support_RfSelf_cal_v7;              /** [28] [M70 series] support RF self-calibration                              ,is_capable (0: not supported, 1: supported) parameter ( |...|Post self-calibartion|Pre self-calibartion| )*/
            public RfFactoryModeCalItem  support_checksum_verify;            /** [29] support the calibration data checksum check              ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  support_query_utas_cfg;             /** [30] support UTAS configuration                               ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  en_used_cc_num_info;                /** [31] the number of TX and RX CC for LTE and NR               ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  support_check_mipi_component;       /** [32] support the MIPI component check                         ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  support_txf_functionality;          /** [33] Indicates "TX forward test" is supported or not                  ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  support_mm_utas_query_cfg_v7;       /** [34] query supported utas state info v7 or not and max state num      ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  support_en_srs_tx_cal;              /** [35] query supported srs tx calibration mechanism                     ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  support_95_before_lte_fhc;          /** [36] Indicate whether "LTE AFC FHC API" is supported or not           ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  afc_dac_intial_from_xtal_info;      /** [37] indicate if query AFC DAC from xtal_info                         ,is_capable (0: not supported, 1: supported) */
            public MMRfTestAfcDacRange afc_dac_range_info;                 /** [38] indicate AFC DAC range info                                ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  support_en_data_shared;             /** [39] indicate LTE&NR shared data feature is support or not            ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  reserved_40;                        /** [40] reserved */
            public RfFactoryModeCalItem  reserved_41;                        /** [41] reserved */
            public MMRfTestSerDesVerno serdes_test_verno;                  /** [42] indicate SerDes test info                                  ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  support_query_tas_verify_list;      /** [43] support query TAS verify list                              ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  support_pa_bias_tuning_tool;        /** [44] support PA bias tuning tool                               ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem  support_per_route_max_prf_query;    /** [45] support query TAS verify list                              ,is_capable (0: not supported, 1: supported) */
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMRfTestSerDesVerno
        {

            /// is_capable : 1
            ///is_mandatory : 1
            ///version : 8
            ///reserve : 22
            public uint bitvector1;

            public uint is_capable
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint is_mandatory
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint version
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 1020u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }

            public uint reserve
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4294966272u)
                                / 1024)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 1024)
                                | this.bitvector1)));
                }
            }
        }




        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMRfTestAfcDacRange
        {

            /// is_capable : 1
            ///is_mandatory : 1
            ///afc_dac_range : 16
            ///reserve : 14
            public uint bitvector1;

            public uint is_capable
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint is_mandatory
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint afc_dac_range
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 262140u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }

            public uint reserve
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4294705152u)
                                / 262144)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 262144)
                                | this.bitvector1)));
                }
            }
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMRfTestSelfCal
        {

            /// is_capable : 1
            ///is_mandatory : 1
            ///PreSelfCal : 1
            ///PostSelfCal : 1
            ///reserve : 28
            public uint bitvector1;

            public uint is_capable
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint is_mandatory
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint PreSelfCal
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }

            public uint PostSelfCal
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 8u)
                                / 8)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 8)
                                | this.bitvector1)));
                }
            }

            public uint reserve
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4294967280u)
                                / 16)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 16)
                                | this.bitvector1)));
                }
            }
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMRfTestCoTmsCal
        {

            /// is_capable : 1
            ///is_mandatory : 1
            ///version : 8
            ///t0 : 16
            ///reserve : 6
            public uint bitvector1;

            public uint is_capable
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint is_mandatory
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint version
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 1020u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }

            public uint t0
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 67107840u)
                                / 1024)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 1024)
                                | this.bitvector1)));
                }
            }

            public uint reserve
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4227858432u)
                                / 67108864)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 67108864)
                                | this.bitvector1)));
                }
            }
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMRfTestXtalInfo
        {

            /// is_capable : 1
            ///is_mandatory : 1
            ///on_pmic : 1
            ///xtal_type : 3
            ///afc_origin_dac : 16
            ///reserve : 10
            public uint bitvector1;

            public uint is_capable
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint is_mandatory
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint on_pmic
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }

            public uint xtal_type
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 56u)
                                / 8)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 8)
                                | this.bitvector1)));
                }
            }

            public uint afc_origin_dac
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4194240u)
                                / 64)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 64)
                                | this.bitvector1)));
                }
            }

            public uint reserve
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4290772992u)
                                / 4194304)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4194304)
                                | this.bitvector1)));
                }
            }
        }




        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMRfTest32KLess
        {

            /// is_capable : 1
            ///is_mandatory : 1
            ///freqDrift : 16
            ///reserve : 14
            public uint bitvector1;

            public uint is_capable
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint is_mandatory
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint freqDrift
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 262140u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }

            public uint reserve
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4294705152u)
                                / 262144)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 262144)
                                | this.bitvector1)));
                }
            }
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMRfCalibrationItem
        {

            /// is_capable : 1
            ///is_mandatory : 1
            ///parameter : 30
            public uint bitvector1;

            public uint is_capable
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint is_mandatory
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint parameter
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4294967292u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }
        }


        public struct MMRfCapabilityItemSet
        {
            UInt32 reserved;     /** reserve data */
        }

        public struct MMRfTestCmdRfCapabilityReq
        {
            public UInt32 capabilityItemsSize;   /** the request size of capability parameter, MMRfCapabilityItemSet */
            public UInt32 calibrationItemsSize;  /** the request size of calibration parameter, MMRfCalibrationItemSet */
        }


        public struct MMRfTestCmdQueryCarKitMappingInfoReq
        {
            public byte queryMode;             /** the query mode of the carkit( 1: default, 0: by condition, such as RAT and TAS state) */
            public byte rat_bitmap;            /** the bit map for RAT info  ( |LTE FDD|LTE TDD|WCDMA|C2K|TDSCDMA|GSM| ) */
            public byte forced_tas_state;      /** force to query certain TAS state */
        }


        public struct MMRfTestParamCarKitNameMapping
        {
            public byte carkit_enum;        /** the carkit number */

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string carkit_name;    /** the Carkit name */
        }


        public struct MMRfTestParamCmrInfo
        {
            public byte is_before_switch;   /** whether the carkit is located before switching. CMR won't be affected by TAS state if this value is TRUE. */
            public UInt32 support_carkit_num; /** the supported carkit count */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public MMRfTestParamCarKitNameMapping[] carkit_info;    /** the detailed information for each carkit */
        }


        public struct MMRfTestParamCarKitInfo_T
        {
            public byte band;          /** band */
            public byte rx_car_kit;    /** the carkit number for RX main path */
            public byte rxd_car_kit;   /** the carkit number for RX diversity path */
            public byte tx_car_kit;    /** the carkit number for TX */
        }


        public struct MMRfTestParamCarKitTxRouteInfo_T
        {
            public UInt16 tx_route_idx; /** TX route */
            public UInt16 band;         /** band */
            public byte tx_car_kit;   /** the carkit number for TX */
        }



        public struct MMRfTestParamCarKitRxRouteInfo_T
        {
            public UInt16 rx_route_idx; /** RX route */
            public UInt16 band;         /** band */
            public byte rx_car_kit;   /** the carkit number for RX main path */
            public byte rxd_car_kit;  /** the carkit number for RX diversity path */
        }


        public struct MMRfTestCmdQueryCarKitMappingInfoCnf
        {
            public byte status;                                             /** query status */
            public MMRfTestParamCmrInfo common_info;                                        /** common carkit information */
            public byte gsm_count;                                          /** GSM case valid count ( for accessing gsm_cmr_indicator[] ) */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public MMRfTestParamCarKitInfo_T[] gsm_cmr_indicator;            /** the pair information for GSM band and carkit */
            public byte tdscdma_count;                                      /** TDSCDMA case valid count ( for accessing tdscdma_cmr_indicator[] ) */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public MMRfTestParamCarKitInfo_T[] tdscdma_cmr_indicator;    /** the pair information for TDSCDMA band and carkit */
            public byte c2k_count;                                          /** C2K case valid count ( for accessing c2k_cmr_indicator[] ) */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
            public MMRfTestParamCarKitInfo_T[] c2k_cmr_indicator;            /** the pair information for C2K band and carkit */
            public byte wcdma_count;                                        /** WCDMA case valid count ( for accessing wcdma_cmr_indicator[] ) */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
            public MMRfTestParamCarKitInfo_T[] wcdma_cmr_indicator;        /** the pair information for WCDMA band and carkit */
            public byte lte_tx_count;                                       /** LTE TX case valid count ( for accessing lte_tx_cmr_indicator[] ) */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public MMRfTestParamCarKitTxRouteInfo_T[] lte_tx_cmr_indicator;     /** the pair information for LTE TX route and carkit */
            public byte lte_rx_count;                                       /** LTE RX case valid count ( for accessing lte_rx_cmr_indicator[] ) */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
            public MMRfTestParamCarKitRxRouteInfo_T[] lte_rx_cmr_indicator;     /** the pair information for LTE RX route and carkit */
        }

        public struct MMRfTestCmdQueryCarKitMappingInfoCnfV7
        {
            public byte status;                                             /** Query status. */
            public MMRfTestParamCmrInfo common_info;                                        /** Common carkit information. */
            public byte gsm_count;                                          /** GSM band and carkit pair count. */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public MMRfTestParamCarKitInfo_T[] gsm_cmr_indicator;            /** GSM band and carkit pair information. */
            public byte tdscdma_count;                                      /** TDSCDMA band and carkit pair count. */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public MMRfTestParamCarKitInfo_T[] tdscdma_cmr_indicator;    /** TDSCDMA band and carkit pair information. */
            public byte c2k_count;                                          /** C2K band and carkit pair count. */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
            public MMRfTestParamCarKitInfo_T[] c2k_cmr_indicator;            /** C2K band and carkit pair information. */
            public byte wcdma_count;                                        /** WCDMA band and carkit pair count. */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
            public MMRfTestParamCarKitInfo_T[] wcdma_cmr_indicator;        /** WCDMA band and carkit pair information. */
            public byte lte_tx_count;                                       /** LTE TX route and carkit pair count. */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public MMRfTestParamCarKitTxRouteInfo_T[] lte_tx_cmr_indicator;     /** LTE TX route and carkit pair information. */
            public byte lte_rx_count;                                       /** LTE RX route and carkit pair count. */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
            public MMRfTestParamCarKitRxRouteInfo_T[] lte_rx_cmr_indicator;     /** LTE RX route and carkit pair information. */
            public byte nr_tx_count;                                        /** NR TX route and carkit pair count. */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public MMRfTestParamCarKitTxRouteInfo_T[] nr_tx_cmr_indicator;       /** NR TX route and carkit pair information. */
            public byte nr_rx_count;                                        /** NR RX route and carkit pair count. */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
            public MMRfTestParamCarKitRxRouteInfo_T[] nr_rx_cmr_indicator;       /** NR RX route and carkit pair information. */
        }

        public struct MMRfShareRouteInfo
        {
            public UInt16 band;                   /** band */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public UInt16[] rx_t1_shared_route;  /** the RX type 1 shared route of LTE/NR */
            public UInt16 nr_index;               /** the NR index for calibration data */
        }


        public struct MMRfTestResultShareRouteInfo
        {
            public UInt16 route_count;   /** the valid count of share_route_info[] */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public MMRfShareRouteInfo[] share_route_info; /** the shared route information */
        }


        public struct MMRfTestResultGetAfcSetting
        {
          public  UInt16 AfcDac;    /** AFC DAC value */
            public  int SlopeInv;  /** the factor for transforming FoE to AFC DAC ( fixed to be 1024 in fix-AFC solution )*/
            public UInt32 CapId;     /** CAP ID value */
        }

        //delegate MMRfTestResultGetAfcSetting MMRfTestCmdSetAfcSetting();

        public struct MMRfTestResultGetAfcSettingV2
        {
            /** Calibrated AFC DAC results (nominal value in fix-AFC solution)
              */
          public UInt16 AfcDac;

            /** Factor for transforming FoE to AFC DAC (fixed to be 1024 in fix-AFC solution)
              */
           public int SlopeInv;

            /** Calibrated CAP ID results
              */
           public UInt32 CapId;

            /** Calibrated low power mode results
              */
           public int cload_freq_offset;
        }

        //delegate MMRfTestResultGetAfcSetting MMRfTestCmdSetAfcSettingV2();

        public struct MMRfTestResultGetAfcSettingV3
        {
          public  UInt16 AfcDac;   /** AFC DAC value */
          public  short aac;      /** AFC AAC value */
          public  int SlopeInv; /** the factor for transforming FoE to AFC DAC ( fixed to be 1024 in fix-AFC solution )*/
          public  UInt32 CapId;    /** CAP ID value */
          public  int cload_freq_offset; /** low power mode calibration data */
        }

        //delegate MMRfTestResultGetAfcSettingV3 MMRfTestCmdSetAfcSettingV3();


        public struct RFMod_ID
        {
            public UInt32 id;             /** GSM RF chip ID */
        }

        public struct RfMsCapabilityEx2_S
        {
            public RfMsCapabilityBits_2 capability;    /** the bit map for capability */
            public RfMsBandSupportBits band_support;  /** the bit map for band supported */
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct RfMsCapabilityBits_2
        {

            /// GSM : 1
            ///GPRS : 1
            ///EDGE_RX : 1
            ///EDGE_8PSK_TX : 1
            ///Calibration_8PM : 1
            ///Calibration_FDT : 1
            ///Calibration_33Steps : 1
            ///NSFT : 1
            ///AFCType : 1
            ///GMSKClosedLoopPowerControl : 1
            ///OpenLoopPowerControl : 1
            ///ClosedLoopPowerControlTemperature : 1
            ///EPSKClosedLoopPowerControl : 1
            ///MiddleLowLnaCalibration : 1
            ///AuxTempADC : 1
            ///BsiTempADC : 1
            public uint bitvector1;

            public uint GSM
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint GPRS
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint EDGE_RX
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }

            public uint EDGE_8PSK_TX
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 8u)
                                / 8)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 8)
                                | this.bitvector1)));
                }
            }

            public uint Calibration_8PM
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 16u)
                                / 16)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 16)
                                | this.bitvector1)));
                }
            }

            public uint Calibration_FDT
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 32u)
                                / 32)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 32)
                                | this.bitvector1)));
                }
            }

            public uint Calibration_33Steps
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 64u)
                                / 64)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 64)
                                | this.bitvector1)));
                }
            }

            public uint NSFT
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 128u)
                                / 128)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 128)
                                | this.bitvector1)));
                }
            }

            public uint AFCType
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 256u)
                                / 256)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 256)
                                | this.bitvector1)));
                }
            }

            public uint GMSKClosedLoopPowerControl
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 512u)
                                / 512)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 512)
                                | this.bitvector1)));
                }
            }

            public uint OpenLoopPowerControl
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 1024u)
                                / 1024)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 1024)
                                | this.bitvector1)));
                }
            }

            public uint ClosedLoopPowerControlTemperature
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2048u)
                                / 2048)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2048)
                                | this.bitvector1)));
                }
            }

            public uint EPSKClosedLoopPowerControl
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4096u)
                                / 4096)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4096)
                                | this.bitvector1)));
                }
            }

            public uint MiddleLowLnaCalibration
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 8192u)
                                / 8192)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 8192)
                                | this.bitvector1)));
                }
            }

            public uint AuxTempADC
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 16384u)
                                / 16384)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 16384)
                                | this.bitvector1)));
                }
            }

            public uint BsiTempADC
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 32768u)
                                / 32768)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 32768)
                                | this.bitvector1)));
                }
            }
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct RfMsBandSupportBits
        {

            /// GSM400 : 1
            ///GSM850 : 1
            ///GSM900 : 1
            ///DCS1800 : 1
            ///PCS1900 : 1
            public uint bitvector1;

            public uint GSM400
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint GSM850
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint GSM900
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }

            public uint DCS1800
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 8u)
                                / 8)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 8)
                                | this.bitvector1)));
                }
            }

            public uint PCS1900
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 16u)
                                / 16)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 16)
                                | this.bitvector1)));
                }
            }
        }



        public struct RfMsCapabilityEx3_REQ_S
        {
            public UInt32 capabilityItemsSize;         /** the request size of capability parameter, RfCapabilityItem */
            public UInt32 calibrationItemsSize;        /** the request size of calibration parameter, RfCalibrationItem */
        }

        public struct RfMsCapabilityEx3_S
        {
            public int valid;               /** the response data is valid or not */
            public int status;              /** the execution status code ( 0: successful, others: failed ) */
            public RfCapabilityItem capabilityItems;     /** the supported capability */
            public RfCalibrationItem calibrationItems;    /** the supported calibration items */
        }

        public struct RfCalibrationItem
        {
            public RfFactoryModeCalItem cap_id;          /** [0] support CAP ID calibration      ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem afc;             /** [1] support AFC calibration         ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem rx_pathloss;     /** [2] support RX pathloss calibration ,is_capable (0: not supported, 1: supported) */
            /**
             * pre> 
             * [3] support TX PCL calibration ,is_capable (0: not supported, 1: supported), parameter: to get the GMSK and EPSK version
             * \sa GET_EPSK_VERSION, GET_GSM_VERSION
             * /pre>
            */
            public RfFactoryModeCalItem tx_pcl;
            public RfFactoryModeCalItem tx_subband;      /** [4] support TX sub-band calibration   ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem trx_offset;      /** [5] support TRX offset calibration    ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem tx_iq;           /** [6] support TX IQ calibration         ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem tx_fb_dac;       /** [7] support TX FB DAC calibration     ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem tx_slope_skew;   /** Deprecated. */
            public RfFactoryModeCalItem w_coef;          /** [9] support W-coefficient calibration ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem txpc;            /** [10] support TX PC                    ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem temp_adc;        /** [11] support temperature ADC          ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem remove_32k_xo;   /** [12] support remove 32k XO            ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem co_crystal;      /** [13] support Co-crystal capability    ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem high_lna_sp;     /** [14] support high mode LNA            ,is_capable (0: not supported, 1: supported) , parameter: to get the set DSP point value */
            public RfFactoryModeCalItem mid_lna_sp;      /** [15] support middle mode LNA          ,is_capable (0: not supported, 1: supported) , parameter: to get the set DSP point value */
            public RfFactoryModeCalItem low_lna_sp;      /** [16] support low mode LNA             ,is_capable (0: not supported, 1: supported) , parameter: to get the set DSP point value */
            /**
             * pre> 
             * [17] indicate the EDGE battery compensation calculation
             * (Unit: in dBm)       is_capable = 1 and in 14-bits parameter, LSB of 1st 7 bits all equal to 1 
             * (Unit: in weighting) is_capable = 0 or  in 14-bits parameter, LSB of 1st 7 bits all equal 0
             * /pre>
            */
            public RfFactoryModeCalItem bat_temp_comp;
            /**
             * pre> 
             * [18] support RX FHC band combination flow
             * is_capable (0: not supported, 1: supported),
             * parameter: max steps supported in target (0: 50 steps, 1: 100 steps, 2:512 steps)
             * /pre>
            */
            public RfFactoryModeCalItem dts_gain_cmb;
            /**
             * pre> 
             * [19] support TX FHC band combination flow
             * is_capable (0: not supported, 1: supported),
             * parameter: max steps supported in target (0: 50 steps, 1: 100 steps, 2:512 steps)
             * /pre>
            */
            public RfFactoryModeCalItem uts_band_cmb;
            /**
             * pre> 
             * [20] indicate if TDSCDMA ADC uses the GSM ADC NVRAM item
             * is_capable (0: not supported, 1: supported)
             *     TDSCDMA NVRAM item: NVRAM_EF_AST_TL1_TEMP_DAC_LID
             *     GSM NVRAM item: NVRAM_EF_L1_TEMPERATURE_ADC_LID
             * /pre>
            */
            public RfFactoryModeCalItem co_temp_adc;
            public RfFactoryModeCalItem adjustable_lna_mode_pathloss; /** [21] use the high mode results for middle and low mode ,is_capable ( 0: separately, 1: using high mode result ) */
            public RfFactoryModeCalItem gain_rf_cal;                  /** [22] support EDGE gain RF       ,is_capable (0: not supported, 1: supported) */
            /**
             * pre> 
             * [23] check the AP/MD NVRAM data or not
             * is_capable( 0: check AP/MD NVRAM, 1: bypass check )
             * parameter works when is_capable = 0 ( Don't care when is_capable = 1)
             *      1: GPS CO-TMS configuration is correct. Keep running the GPS CO-TMS.
             *      Others: Error 
             * pre> 
             **/
            public RfFactoryModeCalItem bypass_check_fixafc;
            public RfFactoryModeCalItem multi_rat_tadc_bitmap;  /** [24] the bit map of multi-rat TADC calibration ,is_capable (0: not supported, 1: supported), parameter ( bit map of RAT support: 0|0|C2k|LTE FDD|LTE TDD|TDSCDMA|WCDMA|GSM ) ) */
            public RfFactoryModeCalItem multi_rat_afc_bitmap;   /** [25] the bit map of multi-rat AFC calibration  ,is_capable (0: not supported, 1: supported), parameter ( bit map of RAT support: 0|0|C2k|LTE FDD|LTE TDD|TDSCDMA|WCDMA|GSM ) */
            /**
             * pre> 
             * [26] reduce RX LNA calibration or not
             * is_capable( 0: No, 1: Some LNA mode can bypass the calibration(check from parameter) )
             * parameter ( bit map of LNA mode |High|Middle|Low| )
             * pre> 
             **/
            public RfFactoryModeCalItem reduce_rx_lna_cal;
            public RfFactoryModeCalItem temperature_info;     /** [27] support getting temperature info function          ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem dt_coexistence_info;  /** [28] Deprecated                                         ,is_capable (0: not supported, 1: supported), parameter ( bit map of band support: |PCS1900|DCS1800|GSM900|GSM850 ) */
            public RfFactoryModeCalItem thermal_sensor_type;  /** [29] support the external temperature sensor            ,is_capable (0: not supported, 1: supported), parameter ( sensor type, 0:internal sensor, 2:external sensor) */
            public RfFactoryModeCalItem list_mode_nsft;       /** [30] support list mode NSFT                             ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem worldmode_id_info;    /** [31] support world mode ID capability                   ,is_capable (0: not supported, 1: supported), parameter ( bit map of RAT support:  0|0|LTE FDD|LTE TDD|WCDMA|C2K|TDSCDMA|GSM ) */
            public RfFactoryModeCalItem crystal_on_pmic_enable;   /** [32] support 32k less AFC DAC selection capability  ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem low_pcl_subband_cal;      /** [33] support sub-band low weighting calibration     ,is_capable (0: not supported, 1: supported), parameter ( bit 0~6 for low band, bit 7~13 for high band) */
            public RfFactoryModeCalItem fhc_dts_extra_fb_enable;  /** [34] enable extra FB for RX sync or not             ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem sawless_lna_sp;           /** [35] support sawless middle gain mode               ,is_capable (0: not supported, 1: supported), parameter: to get the set DSP point value */
            public RfFactoryModeCalItem nvram_access_interface;   /** [36] support calibration data access by L1 function ,is_capable (0: not supported, 1: supported), parameter ( |...|...|Not define yet|RX path loss|)*/
            public RfFactoryModeCalItem elna_support_band_bitmap; /** [37] support eLNA  capability                       ,is_capable (0: not supported, 1: supported), parameter ( bit map of band support: |PCS1900|DCS1800|GSM900|GSM850|GSM400| ) */
            public RfFactoryModeCalItem elna_high_sp;             /** [38] support eLNA high mode                         ,is_capable (0: not supported, 1: supported), parameter: to get the set DSP point value */
            public RfFactoryModeCalItem elna_mid_sp;              /** [39] support eLNA middle mode                       ,is_capable (0: not supported, 1: supported), parameter: to get the set DSP point value */
            public RfFactoryModeCalItem elna_low_sp;              /** [40] support eLNA low mode                          ,is_capable (0: not supported, 1: supported), parameter: to get the set DSP point value */
            public RfFactoryModeCalItem elna_w_coef_sp;           /** [41] support eLNA W-coefficient                     ,is_capable (0: not supported, 1: supported), parameter: to get the set DSP point value */
            public RfFactoryModeCalItem elna_mid_sawless_sp;      /** [42] support eLNA sawless                           ,is_capable (0: not supported, 1: supported), parameter: to get the set DSP point value */
            public RfFactoryModeCalItem elna_high_sensitivity_sp; /** [43] support eLNA high sensitivity                  ,is_capable (0: not supported, 1: supported), parameter: to get the set DSP point value */
            public RfFactoryModeCalItem elna_bypass_low_maxpin_sp;/** [44] support eLNA bypass low max pin                ,is_capable (0: not supported, 1: supported), parameter: to get the set DSP point value */
            public RfFactoryModeCalItem elna_bypass_gain_threshold;   /** [45] support eLNA bypass gain threshold         ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem elna_bypass_support_band_bitmap;/** [46] support eLNA bypass                      ,is_capable (0: not supported, 1: supported), parameter ( bit map of band support: |PCS1900|DCS1800|GSM900|GSM850|GSM400| ) */
            public RfFactoryModeCalItem sinwave_afc_get_temp_freq;      /** [47] unused. (reserve for sine tone AFC and Co-TMS(C0,C1)  )   ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem wcoef_setget_cmd_support;       /** [48] support eLNA W-coefficient data operating by L1 function  ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem tx_setget_cmd_support;          /** [49] support TX calibration data operation by L1 function      ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem nsft_adjust_tpo_support;        /** [50] support NSFT adjust TX power offset                       ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem rxd_v5_cmd_support;             /** [51] support RXD                                               ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem rxd_support_band_bitmap;        /** [52] support RXD band capability                               ,is_capable (0: not supported, 1: supported), parameter ( bit map of band support: |PCS1900|DCS1800|GSM900|GSM850| ) */
            public RfFactoryModeCalItem mlna_type_v7;                   /** [53] support MLAN capability                                   ,is_capable (0: not supported, 1: supported) */
            public RfFactoryModeCalItem rxlev_precision_extend;         /** [54] indicate RXLEV precision                                  ,is_capable (0: not supported, 1: supported), parameter (for unit : 1/(2^parameter) dB, |1/8dB|1/4dB|1/2dB|1dB| ) */
            public RfFactoryModeCalItem telematics_volt_temp_enhance;   /** [55] support the volt/temperature from 3X3 to 5X5 for telematics ,is_capable (0: not supported, 1: supported) */
        }


        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct RfFactoryModeCalItem
        {

            /// is_capable : 1
            ///is_mandatory : 1
            ///parameter : 14
            public uint bitvector1;

            public uint is_capable
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint is_mandatory
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint parameter
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 65532u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct RfCapabilityItem
        {

            /// support_gsm : 1
            ///support_gprs : 1
            ///support_edge_rx : 1
            ///support_epsk_tx : 1
            ///support_8pm : 1
            ///support_fhc : 1
            ///support_nsft : 1
            ///band_gsm400 : 1
            ///band_gsm850 : 1
            ///band_gsm900 : 1
            ///band_dcs1800 : 1
            ///band_pcs1900 : 1
            ///ps_unsupport_edge_tx : 1
            ///reserved1 : 2
            public uint bitvector1;

            public uint support_gsm
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint support_gprs
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint support_edge_rx
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }

            public uint support_epsk_tx
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 8u)
                                / 8)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 8)
                                | this.bitvector1)));
                }
            }

            public uint support_8pm
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 16u)
                                / 16)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 16)
                                | this.bitvector1)));
                }
            }

            public uint support_fhc
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 32u)
                                / 32)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 32)
                                | this.bitvector1)));
                }
            }

            public uint support_nsft
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 64u)
                                / 64)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 64)
                                | this.bitvector1)));
                }
            }

            public uint band_gsm400
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 128u)
                                / 128)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 128)
                                | this.bitvector1)));
                }
            }

            public uint band_gsm850
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 256u)
                                / 256)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 256)
                                | this.bitvector1)));
                }
            }

            public uint band_gsm900
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 512u)
                                / 512)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 512)
                                | this.bitvector1)));
                }
            }

            public uint band_dcs1800
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 1024u)
                                / 1024)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 1024)
                                | this.bitvector1)));
                }
            }

            public uint band_pcs1900
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2048u)
                                / 2048)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2048)
                                | this.bitvector1)));
                }
            }

            public uint ps_unsupport_edge_tx
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4096u)
                                / 4096)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4096)
                                | this.bitvector1)));
                }
            }

            public uint reserved1
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 24576u)
                                / 8192)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 8192)
                                | this.bitvector1)));
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct URfTestResultRFID
        {
            /// unsigned int
            public UInt32 m_u4Rfid;
        }


        public struct UMTS_MsCapabilityEx2
        {
            public UMTS_MsCapabilityEx_Bits capability;    /**< the bitmap for WCDMA capability */
            /**
             * <pre>
             * bit map of band supported, |....|band4|band3|band2|band1| 
             * \sa UMTS_SUPPORT_BAND1 to UMTS_SUPPORT_BAND_COUNT
             * </pre>
            */
            public UInt32 band_support;
            /**
             * <pre>
             * bit map of RXD band, |....|band4|band3|band2|band1|
             * \sa UMTS_SUPPORT_BAND1 to UMTS_SUPPORT_BAND_COUNT
             * </pre>
            */
            public UInt32 rxd_band_support;
            /**
             * <pre>
             * bit map of PA drift band, |....|band4|band3|band2|band1|
             * \sa UMTS_SUPPORT_BAND1 to  UMTS_SUPPORT_BAND_COUNT
             * </pre>
            */
            public UInt32 padrift_band_support;
            /**
             * <pre>
             * bit map of DPD band, |....|band4|band3|band2|band1|
             * \sa UMTS_SUPPORT_BAND1 to UMTS_SUPPORT_BAND_COUNT
             * </pre>
            */
            public UInt32 dpd_band_support;
        }


        /**
          * \ingroup WCDMAStruct
          * \details The WCDMA capability
          * \sa UMTS_MsCapabilityEx2
          */
        public struct UMTS_MsCapabilityEx
        {
            /**
             * <pre>
             * \details the bitmap for WCDMA capability 
             * It can be cast to the extend version structure, UMTS_MsCapabilityEx_Bits.
             * \sa UMTS_MsCapabilityEx_Bits
             * </pre>
             */
            public UInt32 capability;
            /**
             * <pre>
             * bit map of WCDMA band, |....|band4|band3|band2|band1|
             * \sa UMTS_SUPPORT_BAND1 to UMTS_SUPPORT_BAND_COUNT
             * </pre>
             */
            public UInt32 band_support;
            /**
             * <pre>
             * bit map of RXD band, |....|band4|band3|band2|band1|
             * \sa UMTS_SUPPORT_BAND1 to UMTS_SUPPORT_BAND_COUNT
             * </pre>
             */
            public UInt32 rxd_band_support;
            /**
             * <pre>
             * bit map of PA drift band, |....|band4|band3|band2|band1|
             * \sa UMTS_SUPPORT_BAND1 to UMTS_SUPPORT_BAND_COUNT
             * </pre>
             */
            public UInt32 padrift_band_support;
            /**
             * <pre>
             * bit map of DPD band, |....|band4|band3|band2|band1|
             * \sa UMTS_SUPPORT_BAND1 to UMTS_SUPPORT_BAND_COUNT
             * </pre>
             */
            public UInt32 dpd_band_support;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct UMTS_MsCapabilityEx_Bits
        {

            /// PaOctLevelControl : 1
            ///PaPhaseCompensationConversion : 1
            ///PaCouplerLossByPaMode : 1
            ///HsdpaNsft : 1
            ///HsupaNsft : 1
            ///UmtsFddDcxoSupport : 1
            ///UmtsFddRxDiversitySupport : 1
            ///UmtsFddRxDualCellSupport : 1
            ///PdMeasurementDbConversion : 1
            ///PaDriftCompenstaion : 1
            ///UmtsTempAdcUsingL1API : 1
            ///Dc2DcLevelUnused : 1
            ///RxSoftwareModeTracking : 1
            ///FhcTxVgaBB0Mode : 1
            ///FhcTxFrameDurationLimit : 1
            ///NsftSetICSInitialGain : 1
            ///UmtsFddTxPRACHTemperatureCompensationSupport : 1
            ///RfCapabilityExtension : 1
            ///UmtsFddDpdSupport : 1
            public uint bitvector1;

            public uint PaOctLevelControl
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint PaPhaseCompensationConversion
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint PaCouplerLossByPaMode
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }

            public uint HsdpaNsft
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 8u)
                                / 8)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 8)
                                | this.bitvector1)));
                }
            }

            public uint HsupaNsft
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 16u)
                                / 16)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 16)
                                | this.bitvector1)));
                }
            }

            public uint UmtsFddDcxoSupport
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 32u)
                                / 32)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 32)
                                | this.bitvector1)));
                }
            }

            public uint UmtsFddRxDiversitySupport
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 64u)
                                / 64)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 64)
                                | this.bitvector1)));
                }
            }

            public uint UmtsFddRxDualCellSupport
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 128u)
                                / 128)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 128)
                                | this.bitvector1)));
                }
            }

            public uint PdMeasurementDbConversion
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 256u)
                                / 256)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 256)
                                | this.bitvector1)));
                }
            }

            public uint PaDriftCompenstaion
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 512u)
                                / 512)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 512)
                                | this.bitvector1)));
                }
            }

            public uint UmtsTempAdcUsingL1API
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 1024u)
                                / 1024)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 1024)
                                | this.bitvector1)));
                }
            }

            public uint Dc2DcLevelUnused
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2048u)
                                / 2048)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2048)
                                | this.bitvector1)));
                }
            }

            public uint RxSoftwareModeTracking
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4096u)
                                / 4096)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4096)
                                | this.bitvector1)));
                }
            }

            public uint FhcTxVgaBB0Mode
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 8192u)
                                / 8192)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 8192)
                                | this.bitvector1)));
                }
            }

            public uint FhcTxFrameDurationLimit
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 16384u)
                                / 16384)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 16384)
                                | this.bitvector1)));
                }
            }

            public uint NsftSetICSInitialGain
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 32768u)
                                / 32768)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 32768)
                                | this.bitvector1)));
                }
            }

            public uint UmtsFddTxPRACHTemperatureCompensationSupport
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 65536u)
                                / 65536)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 65536)
                                | this.bitvector1)));
                }
            }

            public uint RfCapabilityExtension
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 131072u)
                                / 131072)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 131072)
                                | this.bitvector1)));
                }
            }

            public uint UmtsFddDpdSupport
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 262144u)
                                / 262144)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 262144)
                                | this.bitvector1)));
                }
            }
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct UL1TSTCalibrationItem
        {

            /// is_capable : 1
            ///is_mandatory : 1
            ///parameter : 30
            public uint bitvector1;

            public uint is_capable
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint is_mandatory
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint parameter
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4294967292u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct UL1TSTCapabilityItemSet
        {

            /// unsigned int
            public uint mipi_pa_support_band;

            /// unsigned int
            public uint rx_gain_sweep_support;
        }

        public enum URfTestWcdmaDpdGeneration
        {

            /// URF_TEST_WCDMA_DPD_V1 -> 0
            URF_TEST_WCDMA_DPD_V1 = 0,

            /// URF_TEST_WCDMA_DPD_V2 -> 1
            URF_TEST_WCDMA_DPD_V2 = 1,

            /// URF_TEST_WCDMA_DPD_V3 -> 2
            URF_TEST_WCDMA_DPD_V3 = 2,

            /// URF_TEST_WCDMA_DPD_V5 -> 3
            URF_TEST_WCDMA_DPD_V5 = 3,
        }

        public enum URfTestWcdmaGeneration
        {

            /// URF_TEST_WCDMA_V3 -> 1
            URF_TEST_WCDMA_V3 = 1,

            /// URF_TEST_WCDMA_V5 -> 2
            URF_TEST_WCDMA_V5 = 2,

            /// URF_TEST_WCDMA_V7 -> 3
            URF_TEST_WCDMA_V7 = 3,
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct UL1TSTCalibrationItemSet
        {

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem tadc_cal;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem multi_rat_tadc_bitmap;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem multi_rat_afc_bitmap;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem mipi_pa_level_and_cw_num;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem temperature_info;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem list_mode_support;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem thermal_sensor_type;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem ubin_mode_switch_support;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem nvram_access_interface;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem rx_gain_lpm_offset;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem nsft_get_RSSI;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem new_prf_select_method;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem nsft_reset_ber_result;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem elna_addition;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem apc_extend;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem elna_addition_diversity_path;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem cancel_polling_action_when_get_fhc_result;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem wcdma_md_cal_generation;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem dpd_generation;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem wcdma_iq_dump;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem wcdma_afc_rscp_generation;

            /// UL1TSTCalibrationItem->Anonymous_c9f72ad9_36bb_48db_aebe_c4f5d206cd47
            public UL1TSTCalibrationItem wcdma_nsft_tx_power_offset;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestCmdGetRfCapabilityReq
        {

            /// unsigned int
            public uint capabilityItemsSize;

            /// unsigned int
            public uint calibrationItemsSize;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultGetRfCapabilityCnf
        {

            /// int
            public int valid;

            /// int
            public int status;

            /// UL1TSTCapabilityItemSet->Anonymous_89b04087_dfca_4a10_915d_da18c1b3d387
            public UL1TSTCapabilityItemSet capabilityItems;

            /// UL1TSTCalibrationItemSet->Anonymous_ba03cc59_ab25_4d1a_9ffc_a45aef68c164
            public UL1TSTCalibrationItemSet calibrationItems;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct URfNsftListModeCnf
        {

            /// unsigned char
            public byte status;

            /// unsigned char
            public byte current_idx;

            /// short[]
            public short[] total_bits;

            /// short[]
            public short[] error_bits;

            /// short[]
            public short[] rssi;

            /// short[]
            public short[] rssi_rxd;

            /// char[3]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 3)]
            public string lnamode;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct UMTS_NSFTLinkStatusReport
        {

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool ok;

            /// unsigned char
            public byte link_status;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultRSSI
        {

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool ok;

            /// unsigned char
            public byte num_freq;

            /// short[]
            public short[] dl_freq;

            /// int[]
            public int[] rssi;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultRSSIRxD
        {

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool ok;

            /// unsigned char
            public byte num_freq;

            /// short[]
            public short[] dl_freq;

            /// int[]
            public int[] rssi;

            /// int[]
            public int[] rssi_rxd;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct URfTestResultELNARSSI
        {

            /// unsigned char
            public byte num_freq;

            /// short[]
            public short[] dl_freq;

            /// int[]
            public int[] rx_rssi;

            /// int[]
            public int[] rxd_rssi;

            /// unsigned char[]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 36)]
            public string rx_LNAmode;

            /// unsigned char[]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 36)]
            public string rxd_LNAmode;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct UL1TST_cpich_result_T
        {

            /// unsigned short
            public ushort psc;

            /// int
            public int tm;

            /// short
            public short off;

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool sttd;

            /// unsigned char
            public byte sample_num;

            /// short
            public short rscp_sum;

            /// short
            public short freq_error;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultRSCP
        {

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool ok;

            /// unsigned char
            public byte num_cell;

            /// UL1TST_cpich_result_T[]
            public UL1TST_cpich_result_T[] cpich_result;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct UL1D_RF_FHC_CNF_T
        {

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool ok;

            /// unsigned char
            public byte freq_count;

            /// unsigned char
            public byte pwr_count;

            /// unsigned char
            public byte freq_start_idx;

            /// unsigned char
            public byte pwr_start_idx;

            /// short[]
            public short[] rssi;

            /// short[]
            public short[] pwr_det_value;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultRxDPCh
        {

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool ok;

            /// unsigned int
            public uint ber;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct UL1D_RF_FHC_EX2_CNF_T
        {

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool ok;

            /// unsigned char
            public byte freq_count;

            /// unsigned char
            public byte pwr_count;

            /// unsigned char
            public byte freq_start_idx;

            /// unsigned char
            public byte pwr_start_idx;

            /// unsigned char[400]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 400)]
            public string rx_lna_mode;

            /// short[400]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 400, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rssi;

            /// short[120]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 120, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] pwr_det_value;

            /// short[400]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 400, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] Gbb_Offset;

            /// unsigned char[400]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 400)]
            public string rxd_lna_mode;

            /// short[40]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 40, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rxd_rssi;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultPwrDtDac
        {

            /// unsigned short
            public ushort m_u2Result;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultPwrDtStep
        {

            /// unsigned char
            public byte m_u1Result;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultGetRfTempSensor
        {

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool ok;

            /// unsigned int
            public uint sum;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultPwrDtDefaultThr
        {

            /// unsigned char
            public byte m_u1Result;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct UL1D_RF_GET_PWR_DET_REPORT_CNF_T
        {

            /// short
            public short m_s2PwrDetResult;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct UL1D_RF_NSFT_GET_BIT_CNT_FOR_BER_CNF_T
        {

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool bDataValid;

            /// unsigned int
            public uint u4TotalBits;

            /// unsigned int
            public uint u4ErrorBits;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultRxGainSweep
        {

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool ok;

            /// unsigned char
            public byte num_freq;

            /// short[]
            public short[] dl_freq;

            /// int[]
            public int[] inband_pow;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultNSFTRSSI
        {

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool ok;

            /// int
            public int NSFT_RSSI;

            /// int
            public int NSFT_RSSI_RXD;

            /// int
            public int cmd_location;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultNSFTResetBERResult
        {

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool ok;

            /// unsigned int
            public uint u4TotalBits;

            /// unsigned int
            public uint u4ErrorBits;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultAFC
        {

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool ok;

            /// unsigned short
            public ushort psc;

            /// int
            public int tm;

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool sttd;

            /// unsigned char
            public byte rscp_sum;

            /// short
            public short freq_error;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultAFC_Ex
        {

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool ok;

            /// unsigned short
            public ushort psc;

            /// int
            public int tm;

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool sttd;

            /// unsigned char
            public byte rscp_sum;

            /// int
            public int freq_error;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
        public struct URfTestResultParam
        {
            /// boolean
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool ok;

            /// char
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public byte nvramAccessResult;

            /// char
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public byte m_i1CurGainTableState;

            /// unsigned int
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public uint bsi_data;

            /// unsigned int
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public uint m_u4MaxCapId;

            /// unsigned char
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public byte currentLnaMode;

            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            URfTestResultRSSI rssi;                   /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            URfTestResultRSSIRxD rssi_rxd;               /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            URfTestResultELNARSSI rssi_elna;              /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            URfTestResultAFC afc;                    /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            URfTestResultRSCP rscp;                   /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            URfTestResultRFID rfid;                   /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            UL1D_RF_FHC_CNF_T m_rMQCResult;           /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            URfTestResultAFC_Ex afc_ex;                 /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            URfTestResultRxDPCh rx_dpch;                /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            UMTS_MsCapabilityEx m_rTargetCapability;    /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            UL1D_RF_FHC_EX2_CNF_T fhcExResult;            /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            URfTestResultPwrDtDac pwr_dt_dac;             /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            URfTestResultPwrDtStep pwr_dt_step;            /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            UMTS_NSFTLinkStatusReport m_rNSFTLinkStatusReport;          /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            URfNsftListModeCnf m_rNSFTListModeLinkStatusReport;  /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            URfTestResultGetRfTempSensor rfTemperature;                    /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            URfTestResultPwrDtDefaultThr pwr_dt_default_thr;               /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            UL1D_RF_GET_PWR_DET_REPORT_CNF_T m_rPwrDetResult;                  /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            UL1D_RF_NSFT_GET_BIT_CNT_FOR_BER_CNF_T m_rNSFTBERResult;                 /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            URfTestResultRxGainSweep rx_gain_sweep;                    /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            URfTestResultNSFTRSSI rssi_nsft;                        /** Deprecated. unused in any function by URfTestResultParam */
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            URfTestResultNSFTResetBERResult resetBERResult;                   /** Deprecated. unused in any function by URfTestResultParam */
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultGetRssiV3
        {

            /// unsigned char
            public byte ok;

            /// unsigned char
            public byte rx_elna_bypass;

            /// unsigned char
            public byte rxd_elna_bypass;

            /// unsigned char
            public byte rx_lna_mode;

            /// unsigned char
            public byte rxd_lna_mode;

            /// unsigned short
            public ushort uarfcn;

            /// short
            public short rx_rssi;

            /// short
            public short rxd_rssi;

            /// unsigned short
            public ushort rx_used_gain;

            /// unsigned short
            public ushort rxd_used_gain;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestCmdGetRssiV3
        {

            /// unsigned char
            public byte hwAGC;

            /// unsigned char
            public byte rx_elna_bypass;

            /// unsigned char
            public byte rxd_elna_bypass;

            /// unsigned char
            public byte conti_rssi;

            /// unsigned char
            public byte rx_elna_mode;

            /// unsigned char
            public byte rxd_elna_mode;

            /// unsigned char
            public byte temperature;

            /// unsigned char
            public byte antenna_path;

            /// unsigned char
            public byte lpm_mode;

            /// unsigned short
            public ushort uarfcn;

            /// unsigned short
            public ushort rx_gain;

            /// unsigned short
            public ushort rxd_gain;

            /// unsigned short
            public ushort rx_digital_gain;

            /// unsigned short
            public ushort rxd_digital_gain;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestCmdGetRssiV5
        {

            /// unsigned char
            public byte hwAGC;

            /// unsigned char
            public byte rx_elna_bypass;

            /// unsigned char
            public byte rxd_elna_bypass;

            /// unsigned char
            public byte conti_rssi;

            /// unsigned char
            public byte rx_elna_mode;

            /// unsigned char
            public byte rxd_elna_mode;

            /// unsigned char
            public byte temperature;

            /// unsigned char
            public byte antenna_path;

            /// unsigned char
            public byte lpm_mode;

            /// unsigned short
            public ushort uarfcn;

            /// unsigned short
            public ushort rx_gain;

            /// unsigned short
            public ushort rxd_gain;

            /// unsigned short
            public ushort rx_digital_gain;

            /// unsigned short
            public ushort rxd_digital_gain;

            /// unsigned char
            public byte rx_gain_table;

            /// unsigned char
            public byte rxd_gain_table;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultPwrToGainV3
        {

            /// unsigned char
            public byte ok;

            /// unsigned short
            public ushort rx_digital_gain;

            /// unsigned short
            public ushort rxd_digital_gain;

            /// unsigned short
            public ushort rx_rf_gain;

            /// unsigned short
            public ushort rxd_rf_gain;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestCmdPwrToGainV7
        {

            /// unsigned char
            public byte antenna_path;

            /// unsigned char
            public byte power_mode;

            /// unsigned char
            public byte rx_cal_sequency;

            /// short
            public short rx_dl_power;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultGetRssiV7
        {

            /// unsigned char
            public byte ok;

            /// unsigned char
            public byte rx_lna_mode;

            /// unsigned char
            public byte rxd_lna_mode;

            /// unsigned short
            public ushort uarfcn;

            /// short
            public short rx_rssi;

            /// short
            public short rxd_rssi;

            /// unsigned short
            public ushort rx_used_gain;

            /// unsigned short
            public ushort rxd_used_gain;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestCmdGetRssiV7
        {

            /// unsigned char
            public byte hwAGC;

            /// unsigned char
            public byte conti_rssi;

            /// unsigned char
            public byte temperature;

            /// unsigned char
            public byte antenna_path;

            /// unsigned char
            public byte power_mode;

            /// unsigned short
            public ushort uarfcn;

            /// unsigned short
            public ushort rx_gain;

            /// unsigned short
            public ushort rxd_gain;

            /// unsigned short
            public ushort rx_digital_gain;

            /// unsigned short
            public ushort rxd_digital_gain;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfCalItemSwitchTime
        {

            /// is_capable : 1
            ///is_mandatory : 1
            ///fdd_to_tdd_switch_time : 3
            ///fdd_tx_to_rx_switch_time : 3
            ///tdd_tx_to_rx_switch_time : 3
            ///freq_switch_time : 3
            ///band_switch_time : 3
            ///tx_step_width : 3
            ///reserve : 12
            public uint bitvector1;

            public uint is_capable
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint is_mandatory
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint fdd_to_tdd_switch_time
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 28u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }

            public uint fdd_tx_to_rx_switch_time
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 224u)
                                / 32)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 32)
                                | this.bitvector1)));
                }
            }

            public uint tdd_tx_to_rx_switch_time
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 1792u)
                                / 256)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 256)
                                | this.bitvector1)));
                }
            }

            public uint freq_switch_time
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 14336u)
                                / 2048)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2048)
                                | this.bitvector1)));
                }
            }

            public uint band_switch_time
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 114688u)
                                / 16384)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 16384)
                                | this.bitvector1)));
                }
            }

            public uint tx_step_width
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 917504u)
                                / 131072)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 131072)
                                | this.bitvector1)));
                }
            }

            public uint reserve
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4293918720u)
                                / 1048576)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 1048576)
                                | this.bitvector1)));
                }
            }
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestCmdRfCapabilityReq
        {

            /// unsigned int
            public uint capabilityItemsSize;

            /// unsigned int
            public uint calibrationItemsSize;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ErfCapabilityItemSet
        {

            /// unsigned int[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U4)]
            public uint[] supportBandMap;

            /// unsigned int[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U4)]
            public uint[] supportMipiBandMap;

            /// unsigned int[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U4)]
            public uint[] supportCoexistenceBandMap;

            /// unsigned int[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U4)]
            public uint[] supportDpdBandMap;

            /// unsigned int[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U4)]
            public uint[] supportCim3BandMap;

            /// unsigned int[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U4)]
            public uint[] supportHRMBandMap;

            /// unsigned int[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U4)]
            public uint[] supportNCCAOneElnaBandMap;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ErfCalibrationItem
        {

            /// is_capable : 1
            ///is_mandatory : 1
            ///parameter : 30
            public uint bitvector1;

            public uint is_capable
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint is_mandatory
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint parameter
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4294967292u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }
        }

        public enum ERfTestLteGeneration
        {

            /// ERF_TEST_MODEM_NULL -> 0
            ERF_TEST_MODEM_NULL = 0,

            /// ERF_TEST_MODEM_V1 -> 1
            ERF_TEST_MODEM_V1 = 1,

            /// ERF_TEST_MODEM_V2 -> 2
            ERF_TEST_MODEM_V2 = 2,

            /// ERF_TEST_MODEM_V3 -> 3
            ERF_TEST_MODEM_V3 = 3,

            /// ERF_TEST_MODEM_V5 -> 4
            ERF_TEST_MODEM_V5 = 4,

            /// ERF_TEST_MODEM_V7 -> 5
            ERF_TEST_MODEM_V7 = 5,

            /// ERF_TEST_MODEM_END -> 0xFFFF
            ERF_TEST_MODEM_END = 65535,
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ErfCalibrationItemSet
        {

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem tadc_cal;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem multi_rat_tadc_bitmap;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem multi_rat_afc_bitmap;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem pd_temp_comp;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem mipi_pa_level_and_cw_num;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem temperature_info;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem et_module_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem tool_usage_setting_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem thermal_sensor_type;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem cap_id_calibration;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem enable_csr;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem gps_co_tms_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_ca_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_ca_ena;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_dpd_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_cim3_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_coexistence_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_hrm_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem fhc_sw_time;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem nsft_extension;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_tas_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem ncca_bypass_check;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_generation_version;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_rftool_ui_version;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem additional_palevel_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_elna_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_band2bitmap;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_ubin_mode_setup;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_auxadc_read;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_fhc_rx_measurement_info;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_fhc_tx_measurement_info;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_ul256qam_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_max_ulcc_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_force_mode_rxtx_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem report_rx_gain_in_mix_mode;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_afc_fhc;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_tx_power_modification;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_mipi_pa_tuning_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_tx_forward_test;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_nsft_partial_band_info;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_tx_config_updt;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_pa_bias_lab_tunning_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_hpue_route_info;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_rfde_cal_version;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_afc_support_v5;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_rx_config_updt;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_start_rssi_rx;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_dpd_performance_check_support;

            /// ErfCalibrationItem->Anonymous_32115a04_924e_4191_a67c_351ff162b9a9
            public ErfCalibrationItem lte_et_special_function_bitmap;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestCmdRfCapabilityCnf
        {

            /// int
            public int valid;

            /// int
            public int status;

            /// unsigned int
            public uint rfId;

            /// ErfCapabilityItemSet->Anonymous_35c63530_bb6e_422b_a3b1_dd949a431473
            public ErfCapabilityItemSet capabilityItems;

            /// ErfCalibrationItemSet->Anonymous_283a1485_59ba_4e5d_9b0c_3ccd94de875a
            public ErfCalibrationItemSet calibrationItems;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestTxType1CaInfoV3_T
        {

            /// unsigned char
            public byte total_route_num;

            /// unsigned short
            public ushort band;

            /// unsigned char[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 4)]
            public string is_hrm;

            /// unsigned short[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U2)]
            public ushort[] total_route_idx;

            /// unsigned short[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U2)]
            public ushort[] comp_route_idx;

            /// unsigned short[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U2)]
            public ushort[] FE_route_idx;

            /// unsigned char[32]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string port;

            /// unsigned char[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 4)]
            public string stx;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestRxType1CaInfoV3_T
        {

            /// unsigned char
            public byte total_route_num;

            /// unsigned char
            public byte padding;

            /// unsigned short
            public ushort band;

            /// unsigned short[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U2)]
            public ushort[] total_route_idx;

            /// unsigned short[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U2)]
            public ushort[] comp_route_idx;

            /// unsigned short[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U2)]
            public ushort[] FE_route_idx;

            /// unsigned char[64]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 64)]
            public string port;

            /// unsigned char[8]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 8)]
            public string elna;

            /// unsigned char[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 4)]
            public string srx;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestRxType2CaInfoV3_T
        {

            /// unsigned char
            public byte total_route_num;

            /// unsigned char[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 2)]
            public string simultaneousl_k;

            /// unsigned short
            public ushort band;

            /// unsigned short[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U2)]
            public ushort[] total_route_idx;

            /// unsigned short[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U2)]
            public ushort[] comp_route_idx;

            /// unsigned char[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 2)]
            public string FE_route_idx;

            /// unsigned char[32]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string port;

            /// unsigned char[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 4)]
            public string elna;

            /// unsigned char[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 4)]
            public string srx;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestTRxCaInfoV3_T
        {

            /// ERfTestTxType1CaInfoV3_T[]
            public ERfTestTxType1CaInfoV3_T[] tx_type1_ca_info;

            /// ERfTestRxType1CaInfoV3_T[]
            public ERfTestRxType1CaInfoV3_T[] rx_type1_ca_info;

            /// ERfTestRxType2CaInfoV3_T[]
            public ERfTestRxType2CaInfoV3_T[] rx_type2_ca_info;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestTxCCARouteInfoV3_T
        {

            /// unsigned char
            public byte total_route_num;

            /// unsigned char[3]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 3)]
            public string padding;

            /// unsigned short
            public ushort band;

            /// unsigned short[]
            public ushort[] route_idx;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestTxCCAInfoV3_T
        {

            /// unsigned char
            public byte total_band_num;

            /// ERfTestTxCCARouteInfoV3_T[]
            public ERfTestTxCCARouteInfoV3_T[] tx_cca_route_info;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestTxFilterRouteInfoV3_T
        {

            /// unsigned char
            public byte total_route_num;

            /// unsigned char[3]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 3)]
            public string padding;

            /// unsigned short
            public ushort band;

            /// unsigned short[]
            public ushort[] route_idx;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestTxFILTERInfoV3_T
        {

            /// unsigned char
            public byte total_band_num;

            /// ERfTestTxFilterRouteInfoV3_T[]
            public ERfTestTxFilterRouteInfoV3_T[] tx_filter_route_info;
        }

        public enum ERfTestRxPowerMode_E
        {

            /// ERF_TEST_RX_HPM -> 0
            ERF_TEST_RX_HPM = 0,

            /// ERF_TEST_RX_LPM -> 1
            ERF_TEST_RX_LPM = 1,

            ERF_TEST_RX_POWER_MODE_NUM,

            /// ERF_TEST_RX_POWER_MODE_MAX -> 0xFF
            ERF_TEST_RX_POWER_MODE_MAX = 255,
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestElnaInfoT1V3_T
        {

            /// unsigned char[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 2)]
            public string seq_num;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] dlpow_default;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] dlpow_lbound;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] dlpow_hbound;

            /// unsigned char[28]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 28)]
            public string dlpow_lna_mode;

            /// unsigned char[28]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 28)]
            public string dlpow_elna_bypass;

            /// unsigned char[28]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 28)]
            public string nvram_idx;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestElnaInfoT2V3_T
        {

            /// unsigned char[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 2)]
            public string gbg_seq_num;

            /// short[36]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 36, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] gbg_dlpow_default;

            /// short[36]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 36, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] gbg_dlpow_lbound;

            /// short[36]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 36, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] gbg_dlpow_hbound;

            /// unsigned char[72]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 72)]
            public string gbg_dlpow_lna_mode;

            /// unsigned char[72]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 72)]
            public string gbg_dlpow_elna_bypass;

            /// unsigned char[72]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 72)]
            public string gbg_nvram_idx;

            /// unsigned char[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 2)]
            public string sc_seq_num;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] sc_dlpow_default;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] sc_dlpow_lbound;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] sc_dlpow_hbound;

            /// unsigned char[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 14)]
            public string sc_dlpow_lna_mode;

            /// unsigned char[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 14)]
            public string sc_dlpow_elna_bypass;

            /// unsigned char[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 14)]
            public string sc_nvram_idx;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestElnaCalInfoV3_T
        {

            /// ERfTestElnaInfoT1V3_T[]
            public ERfTestElnaInfoT1V3_T[] elna_info_t1;

            /// ERfTestElnaInfoT2V3_T[]
            public ERfTestElnaInfoT2V3_T[] elna_info_t2;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestCmdCaConfig_V3
        {

            /// unsigned short[]
            public ushort[] band_mapping_info;

            /// ERfTestTRxCaInfoV3_T->Anonymous_b156e3e2_c631_4f1b_af84_507ff3198a34
            public ERfTestTRxCaInfoV3_T trx_ca_info;

            /// ERfTestTxCCAInfoV3_T->Anonymous_e2821996_e50a_4d67_8ed8_ac10db8ca2aa
            public ERfTestTxCCAInfoV3_T tx_cca_info;

            /// ERfTestTxFILTERInfoV3_T->Anonymous_372bfc3b_b572_40c7_910c_399b10957cb4
            public ERfTestTxFILTERInfoV3_T tx_filter_info;

            /// ERfTestElnaCalInfoV3_T->Anonymous_11101bfd_5574_4fe9_a06a_186d21332d9a
            public ERfTestElnaCalInfoV3_T elna_cal_info;
        }


        public enum ERfTestRxMIMOLayer_E
        {

            /// ERF_TEST_MIMO_INVALID -> 0
            ERF_TEST_MIMO_INVALID = 0,

            /// ERF_TEST_MIMO_1X -> 1
            ERF_TEST_MIMO_1X = 1,

            /// ERF_TEST_MIMO_2X -> 2
            ERF_TEST_MIMO_2X = 2,

            /// ERF_TEST_MIMO_4X -> 3
            ERF_TEST_MIMO_4X = 3,

            /// ERF_TEST_MIMO_8X -> 4
            ERF_TEST_MIMO_8X = 4,

            ERF_TEST_MIMO_LAYERS_NUM,

            /// ERF_TEST_MIMO_LAYERS_MAX -> 0xFF
            ERF_TEST_MIMO_LAYERS_MAX = 255,
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestTxType1CaInfoV5_T
        {

            /// unsigned short
            public ushort band;

            /// unsigned short
            public ushort comp_route_idx;

            /// unsigned short
            public ushort FE_route_idx;

            /// unsigned char[]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 16)]
            public string port;

            /// unsigned char
            public byte stx;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestRxType1CaInfoV5_T
        {

            /// unsigned short
            public ushort band;

            /// unsigned short
            public ushort comp_route_idx;

            /// unsigned short
            public ushort ant_mask;

            /// unsigned short
            public ushort FE_route_idx;

            /// unsigned char[]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 150)]
            public string port;

            /// unsigned char[]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 150)]
            public string elna_gain;

            /// unsigned char[]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 150)]
            public string elna_type;

            /// unsigned char
            public byte gain_table_idx;

            /// unsigned char
            public byte srx;

            /// unsigned char
            public byte mimo_type;

            /// unsigned char
            public byte mimo_pair_route_num;

            /// unsigned short[]
            public ushort[] mimo_comp_route_pair;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestRxType2CaInfoV5_T
        {

            /// unsigned short
            public ushort band;

            /// unsigned short
            public ushort comp_route_idx;

            /// unsigned short
            public ushort ant_mask;

            /// unsigned char
            public byte FE_route_idx;

            /// unsigned char[]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string port;

            /// unsigned char[]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 2)]
            public string elna_gain;

            /// unsigned char[]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 2)]
            public string elna_type;

            /// unsigned char[]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 2)]
            public string gain_table_idx;

            /// unsigned char[]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 2)]
            public string srx;

            /// unsigned char[]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 2)]
            public string mimo_type;

            /// unsigned char
            public byte mimo_pair_route_num;

            /// unsigned short[]
            public ushort[] mimo_comp_route_pair;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestTRxCaInfoV5_T
        {

            /// ERfTestTxType1CaInfoV5_T[]
            public ERfTestTxType1CaInfoV5_T[] tx_type1_ca_info;

            /// ERfTestRxType1CaInfoV5_T[]
            public ERfTestRxType1CaInfoV5_T[] rx_type1_ca_info;

            /// ERfTestRxType2CaInfoV5_T[]
            public ERfTestRxType2CaInfoV5_T[] rx_type2_ca_info;
        }

        public enum ERfTestRxELNAType_V5_E
        {

            /// ERF_TEST_ELNA_OFF_V5 -> 0
            ERF_TEST_ELNA_OFF_V5 = 0,

            /// ERF_TEST_ELNA_BYPASS_LOW_TX_ISO -> 1
            ERF_TEST_ELNA_BYPASS_LOW_TX_ISO = 1,

            /// ERF_TEST_ELNA_BYPASS_HIGH_TX_ISO -> 2
            ERF_TEST_ELNA_BYPASS_HIGH_TX_ISO = 2,

            /// ERF_TEST_ELNA_ALWAYS_ON_LOW_TX_ISO -> 3
            ERF_TEST_ELNA_ALWAYS_ON_LOW_TX_ISO = 3,

            /// ERF_TEST_ELNA_ALWAYS_ON_HIGH_TX_ISO -> 4
            ERF_TEST_ELNA_ALWAYS_ON_HIGH_TX_ISO = 4,

            /// ERF_TEST_ELNA_BYPASS_LOW_TX_ISO_R_MATCHING -> 5
            ERF_TEST_ELNA_BYPASS_LOW_TX_ISO_R_MATCHING = 5,

            ERF_TEST_ELNA_TYPE_NUM_V5,

            /// ERF_TEST_ELNA_MAX_V5 -> 0xFF
            ERF_TEST_ELNA_MAX_V5 = 255,
        }

        public enum ERfTestRxELNAGain_V5_E
        {

            /// ELNA_GAIN_18_DB -> 0
            ELNA_GAIN_18_DB = 0,

            /// ELNA_GAIN_13_DB -> 1
            ELNA_GAIN_13_DB = 1,

            /// ELNA_GAIN_MAX_V5 -> 0xFF
            ELNA_GAIN_MAX_V5 = 255,
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestBandCalItemV5_T
        {

            /// unsigned short
            public ushort band;

            /// unsigned short
            public ushort cal_item;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestTxCCARouteInfoV5_T
        {

            /// unsigned char
            public byte total_route_num;

            /// unsigned char[3]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 3)]
            public string padding;

            /// unsigned short
            public ushort band;

            /// unsigned short[]
            public ushort[] route_idx;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestTxCCAInfoV5_T
        {

            /// unsigned char
            public byte total_band_num;

            /// ERfTestTxCCARouteInfoV5_T[]
            public ERfTestTxCCARouteInfoV5_T[] tx_cca_route_info;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestElnaInfoT1V5_T
        {

            /// unsigned char[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 2)]
            public string elna_gain;

            /// unsigned char[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 2)]
            public string elna_type;

            /// unsigned char
            public byte gain_table_idx;

            /// unsigned char[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 2)]
            public string seq_num;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] dlpow_default;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] dlpow_lbound;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] dlpow_hbound;

            /// unsigned char[28]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 28)]
            public string dlpow_lna_mode;

            /// unsigned char[28]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 28)]
            public string dlpow_elna_bypass;

            /// unsigned char[28]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 28)]
            public string nvram_idx;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestElnaInfoT2V5_T
        {

            /// unsigned char
            public byte elna_gain;

            /// unsigned char
            public byte elna_type;

            /// unsigned char
            public byte gain_table_idx;

            /// unsigned char[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 2)]
            public string gbg_seq_num;

            /// short[36]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 36, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] gbg_dlpow_default;

            /// short[36]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 36, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] gbg_dlpow_lbound;

            /// short[36]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 36, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] gbg_dlpow_hbound;

            /// unsigned char[72]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 72)]
            public string gbg_dlpow_lna_mode;

            /// unsigned char[72]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 72)]
            public string gbg_dlpow_elna_bypass;

            /// unsigned char[72]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 72)]
            public string gbg_nvram_idx;

            /// unsigned char[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 2)]
            public string sc_seq_num;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] sc_dlpow_default;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] sc_dlpow_lbound;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] sc_dlpow_hbound;

            /// unsigned char[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 14)]
            public string sc_dlpow_lna_mode;

            /// unsigned char[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 14)]
            public string sc_dlpow_elna_bypass;

            /// unsigned char[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 14)]
            public string sc_nvram_idx;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestElnaCalInfoV5_T
        {

            /// ERfTestElnaInfoT1V5_T[]
            public ERfTestElnaInfoT1V5_T[] elna_info_t1;

            /// ERfTestElnaInfoT2V5_T[]
            public ERfTestElnaInfoT2V5_T[] elna_info_t2;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestCmdCaConfig_V5
        {

            /// ERfTestBandCalItemV5_T[]
            public ERfTestBandCalItemV5_T[] band_cal_item_info;

            /// ERfTestTRxCaInfoV5_T->Anonymous_86076255_cd3d_4e5a_a691_b31b2b42cace
            public ERfTestTRxCaInfoV5_T trx_ca_info;

            /// ERfTestTxCCAInfoV5_T->Anonymous_bbdfa0cb_7723_4d3e_a5db_94b02e5c808e
            public ERfTestTxCCAInfoV5_T tx_cca_info;

            /// ERfTestElnaCalInfoV5_T->Anonymous_6625f0de_a354_45b4_b5dc_c40b33b4d6b9
            public ERfTestElnaCalInfoV5_T elna_cal_info;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMRfTestCmd_EN_QueryConfigV7
        {

            /// MMTST_RAT_E->Anonymous_e26f4364_72c0_43ab_a1dc_27913850fe04
            public MMTST_RAT_E rat;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMTST_RX_T1_INFO_T
        {

            /// unsigned short
            public ushort band;

            /// unsigned short
            public ushort comp_route;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMTST_RX_T2_INFO_T
        {

            /// unsigned short
            public ushort band;

            /// unsigned short
            public ushort comp_route;

            /// unsigned short
            public ushort t1_route_for_path0;

            /// unsigned short
            public ushort t2_route_for_path1;

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool is_en_route;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMTST_TX_INFO_T
        {

            /// unsigned short
            public ushort band;

            /// unsigned short
            public ushort comp_route;

            /// unsigned short
            public ushort bw_segment;

            /// unsigned int[]
            public uint[] bw_boundary_khz;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMTST_TRX_ROUTE_INFO_T
        {

            /// unsigned short
            public ushort tx_comp_route_used;

            /// unsigned short
            public ushort rx_t1_comp_route_used;

            /// unsigned short
            public ushort rx_t2_comp_route_used;

            /// MMTST_TX_INFO_T[]
            public MMTST_TX_INFO_T[] tx_info;

            /// MMTST_RX_T1_INFO_T[]
            public MMTST_RX_T1_INFO_T[] rx_t1_info;

            /// MMTST_RX_T2_INFO_T[]
            public MMTST_RX_T2_INFO_T[] rx_t2_info;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMTST_RX_USED_INFO_T
        {

            /// boolean[]
            public bool[] t1;

            /// boolean[]
            public bool[] t2;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMTST_RX_T1_NV_INFO_T
        {

            /// unsigned char
            public byte elna_idx;

            /// unsigned char
            public byte ilna_idx;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMTST_RX_T2_NV_INFO_T
        {

            /// unsigned char
            public byte G_idx;

            /// unsigned char
            public byte g_p0_idx;

            /// unsigned char
            public byte g_p1_idx;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct MMTST_RX_CAL_INFO_T
        {

            /// unsigned short
            public ushort seq_t1_num;

            /// unsigned short
            public ushort seq_t2_num;

            /// short[]
            public short[] t1_dlpow_default;

            /// short[]
            public short[] t1_dlpow_max;

            /// short[]
            public short[] t1_dlpow_min;

            /// MMTST_RX_T1_NV_INFO_T[]
            public MMTST_RX_T1_NV_INFO_T[] t1_lna_idx;

            /// unsigned char[]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 10)]
            public string t1_nv_idx;

            /// short[]
            public short[] t2_dlpow_default;

            /// short[]
            public short[] t2_dlpow_max;

            /// short[]
            public short[] t2_dlpow_min;

            /// MMTST_RX_T2_NV_INFO_T[]
            public MMTST_RX_T2_NV_INFO_T[] t2_nv_idx;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMTST_RX_GM_INFO_T
        {

            /// MMTST_RX_USED_INFO_T->Anonymous_3f65b4ff_f2e8_4c91_ac0a_02b0f3cd12ed
            public MMTST_RX_USED_INFO_T rx_used_info;

            /// MMTST_RX_CAL_INFO_T->Anonymous_5293566c_5ced_4a38_b3e6_f70e376a47f0
            public MMTST_RX_CAL_INFO_T rx_cal_info;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMTST_RX_PARTIAL_FERQ_INFO_T
        {

            /// unsigned char
            public byte route_num;

            /// unsigned short[]
            public ushort[] band;

            /// unsigned short[]
            public ushort[] comp_route;

            /// unsigned int[]
            public uint[] start_freq_khz;

            /// unsigned int[]
            public uint[] end_freq_khz;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMTST_TX_PARTIAL_FERQ_INFO_T
        {

            /// unsigned char
            public byte route_num;

            /// unsigned short[]
            public ushort[] band;

            /// unsigned short[]
            public ushort[] comp_route;

            /// unsigned int[]
            public uint[] start_freq_khz;

            /// unsigned int[]
            public uint[] end_freq_khz;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMTST_TRX_PARTIAL_FERQ_INFO_T
        {

            /// MMTST_TX_PARTIAL_FERQ_INFO_T->Anonymous_99fbd34f_b30e_4188_be88_f0c71684a59f
            public MMTST_TX_PARTIAL_FERQ_INFO_T tx;

            /// MMTST_RX_PARTIAL_FERQ_INFO_T->Anonymous_39a3564a_546d_48fb_832d_41de3e14497d
            public MMTST_RX_PARTIAL_FERQ_INFO_T rx;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMTST_TRX_BAND_INFO_T
        {

            /// unsigned short
            public ushort band;

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool is_rx_only;

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool is_sul;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMTST_BAND_INFO_T
        {

            /// unsigned int
            public uint band_used_num;

            /// MMTST_TRX_BAND_INFO_T[]
            public MMTST_TRX_BAND_INFO_T[] trx_band_info;
        }

        public enum MMTST_RAT_E
        {

            /// MMTST_RAT_LTE -> 0
            MMTST_RAT_LTE = 0,

            /// MMTST_RAT_NR -> 1
            MMTST_RAT_NR = 1,

            /// MMTST_RAT_EN_LTE -> 2
            MMTST_RAT_EN_LTE = 2,

            /// MMTST_RAT_EN_NR -> 3
            MMTST_RAT_EN_NR = 3,

            /// MMTST_RAT_MAX -> 0x7FFFFFFF
            MMTST_RAT_MAX = 2147483647,
        }

        public enum MMTST_MIMO_PAIR_E
        {

            /// MMTST_MIMO_PAIR_INVALID -> 0
            MMTST_MIMO_PAIR_INVALID = 0,

            /// MMTST_MIMO_PAIR_1X -> 1
            MMTST_MIMO_PAIR_1X = 1,

            /// MMTST_MIMO_PAIR_2X -> 1
            MMTST_MIMO_PAIR_2X = 1,

            /// MMTST_MIMO_PAIR_4X -> 2
            MMTST_MIMO_PAIR_4X = 2,

            /// MMTST_MIMO_PAIR_8X -> 4
            MMTST_MIMO_PAIR_8X = 4,

            MMTST_MIMO_PAIR_LAYERS_NUM,

            /// MMTST_MIMO_PAIR_LAYERS_MAX -> 0x7FFFFFFF
            MMTST_MIMO_PAIR_LAYERS_MAX = 2147483647,
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMTST_TX_TO_RX_ROUTE_PAIRING_INFO_T
        {

            /// unsigned short
            public ushort band;

            /// unsigned short
            public ushort tx_comp_route;

            /// unsigned short
            public ushort rx_comp_route;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMTST_TX_TO_RX_ROUTE_INFO_T
        {

            /// unsigned short
            public ushort tx_comp_route_used_count;

            /// MMTST_TX_TO_RX_ROUTE_PAIRING_INFO_T[]
            public MMTST_TX_TO_RX_ROUTE_PAIRING_INFO_T[] info;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct MMRfTestResult_EN_QueryConfigV7
        {

            /// MMTST_BAND_INFO_T->Anonymous_4659563a_3d30_4b8d_8d78_1075daad3ace
            public MMTST_BAND_INFO_T band_info;

            /// MMTST_TRX_PARTIAL_FERQ_INFO_T->Anonymous_951d5ea3_4692_402b_b261_ef1df34e01b8
            public MMTST_TRX_PARTIAL_FERQ_INFO_T partial_freq_info;

            /// MMTST_TRX_ROUTE_INFO_T->Anonymous_6c22b224_2c41_45c9_bd5a_393b29e9f02b
            public MMTST_TRX_ROUTE_INFO_T route_info;

            /// MMTST_RX_GM_INFO_T[]
            public MMTST_RX_GM_INFO_T[] rx_gm_info;

            /// MMTST_TX_TO_RX_ROUTE_INFO_T->Anonymous_e5d5f628_6a35_40c7_a627_432eb66e2c69
            public MMTST_TX_TO_RX_ROUTE_INFO_T trx_route_info;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestCmdPuschTxParamV5
        {

            /// unsigned char
            public byte band;

            /// unsigned char
            public byte duplexMode;

            /// unsigned char
            public byte tddConfig;

            /// unsigned char
            public byte tddSfConfig;

            /// unsigned short
            public ushort ulFrequency;

            /// unsigned short
            public ushort cellId;

            /// unsigned char
            public byte ulBandwidth;

            /// unsigned char
            public byte vrbStart;

            /// unsigned char
            public byte vrbLength;

            /// unsigned char
            public byte mcsMode;

            /// short
            public short txPowerValue;

            /// unsigned char
            public byte networkSelection;

            /// unsigned char
            public byte txCloseLoopDisbl;

            /// unsigned char
            public byte amprEnbl;

            /// unsigned short
            public ushort txRoute;

            /// unsigned char
            public byte enableMultiCluster;

            /// unsigned char
            public byte vrbStart2;

            /// unsigned char
            public byte vrbLength2;

            /// unsigned char
            public byte enableCsr;

            /// unsigned short
            public ushort dlFrequency;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestCmd_StartPuschTxCaV5_ReqParam
        {

            /// unsigned char
            public byte ulCCNum;

            /// short
            public short afcdac;

            /// unsigned char
            public byte isforcemode;

            /// ERfTestCmdPuschTxParamV5[]
            public ERfTestCmdPuschTxParamV5[] puschTxParam;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct Mix_Rx_Dl_Ul_Ca_Normal_Mode_Param
        {

            /// unsigned char
            public byte measCnt;

            /// unsigned char
            public byte measBandwidth;

            /// unsigned short
            public ushort dlFrequency;

            /// unsigned char
            public byte band;

            /// unsigned char
            public byte duplexMode;

            /// unsigned char
            public byte tddConfig;

            /// unsigned char
            public byte tddSfConfig;

            /// unsigned char
            public byte bandwidth;

            /// unsigned short
            public ushort rxRoute;

            /// unsigned short
            public ushort routePathSel;

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool hasUl;

            /// unsigned short
            public ushort ulFrequency;

            /// unsigned short
            public ushort txPowerValue;

            /// unsigned char
            public byte vrbStart;

            /// unsigned char
            public byte vrbLength;

            /// unsigned char
            public byte mcsMode;

            /// unsigned short
            public ushort txRoute;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct Mix_Rx_Dl_Ul_Ca_Normal_Mode_V3
        {

            /// unsigned char
            public byte mode;

            /// unsigned char
            public byte rxPath0Enbl;

            /// unsigned char
            public byte rxPath1Enbl;

            /// unsigned char
            public byte numOfDlCC;

            /// unsigned char
            public byte numOfUlCC;

            /// Mix_Rx_Dl_Ul_Ca_Normal_Mode_Param[]
            public Mix_Rx_Dl_Ul_Ca_Normal_Mode_Param[] mix_rx_dl_ul_ca_normal_mode_param;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct Mix_Rx_Tx_Force_Mode_V3
        {

            /// unsigned char
            public byte measCnt;

            /// unsigned char
            public byte measBandwidth;

            /// unsigned short
            public ushort dlFrequency;

            /// unsigned char
            public byte band;

            /// unsigned char
            public byte duplexMode;

            /// unsigned char
            public byte tddConfig;

            /// unsigned char
            public byte tddSfConfig;

            /// unsigned char
            public byte bandwidth;

            /// unsigned char
            public byte rxPath0Enbl;

            /// unsigned char
            public byte rxPath1Enbl;

            /// unsigned short
            public ushort rxRoute;

            /// unsigned short
            public ushort routePathSel;

            /// unsigned short
            public ushort lpmHpmConf;

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool hasUl;

            /// unsigned short
            public ushort ulFrequency;

            /// unsigned short
            public ushort txPowerValue;

            /// unsigned char
            public byte vrbStart;

            /// unsigned char
            public byte vrbLength;

            /// unsigned char
            public byte mcsMode;

            /// unsigned short
            public ushort txRoute;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct Mix_Rx_Force_Mode
        {

            /// unsigned char
            public byte mode;

            /// unsigned char
            public byte measCnt;

            /// unsigned char
            public byte measBandwidth;

            /// unsigned short
            public ushort dlFrequency;

            /// unsigned char
            public byte band;

            /// unsigned char
            public byte duplexMode;

            /// unsigned char
            public byte tddConfig;

            /// unsigned char
            public byte tddSfConfig;

            /// unsigned char
            public byte bandwidth;

            /// unsigned char
            public byte rxPath0Enbl;

            /// unsigned char
            public byte rxPath1Enbl;

            /// unsigned char
            public byte rxRoute;

            /// unsigned short
            public ushort routePathSel;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
        public struct Mix_Rx_Dl_Ul_Ca_Mode_V3
        {

            /// Mix_Rx_Dl_Ul_Ca_Normal_Mode_V3->Anonymous_3cdfcd3b_1077_4e9c_afd1_3f20526d181e
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public Mix_Rx_Dl_Ul_Ca_Normal_Mode_V3 mix_rx_dl_ul_ca_normal_mode;

            /// Mix_Rx_Force_Mode->Anonymous_452ef184_303c_4bd2_8c92_a9b2332b5dcd
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public Mix_Rx_Force_Mode mix_rx_force_mode;

            /// Mix_Rx_Tx_Force_Mode_V3->Anonymous_5c9be225_c5ac_4371_ab6b_80284c53f71a
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public Mix_Rx_Tx_Force_Mode_V3 mix_rx_tx_force_mode;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestCmdMixRx_CaMode_V3
        {

            /// unsigned char
            public byte opMode;

            /// Mix_Rx_Dl_Ul_Ca_Mode_V3->Anonymous_1c5c6080_66fd_4c7f_8503_81dc44ba3ac5
            public Mix_Rx_Dl_Ul_Ca_Mode_V3 MixRxCaModeV3Param;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct Mix_Rx_Dl_Ul_Ca_Normal_Mode_ParamV5
        {

            /// unsigned char
            public byte measCnt;

            /// unsigned short
            public ushort dlFrequency;

            /// unsigned char
            public byte band;

            /// unsigned char
            public byte duplexMode;

            /// unsigned char
            public byte tddConfig;

            /// unsigned char
            public byte tddSfConfig;

            /// unsigned char
            public byte dlBandwidth;

            /// unsigned short
            public ushort ulFrequency;

            /// short
            public short txPowerValue;

            /// unsigned char
            public byte vrbStart;

            /// unsigned char
            public byte vrbLength;

            /// unsigned char
            public byte mcsMode;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct Mix_Rx_Dl_Ul_Ca_Normal_Mode_V5
        {

            /// unsigned char
            public byte mode;

            /// unsigned char
            public byte rxLayer;

            /// Mix_Rx_Dl_Ul_Ca_Normal_Mode_ParamV5->Anonymous_72b2dc73_eff9_457b_b364_3fc6ec152af7
            public Mix_Rx_Dl_Ul_Ca_Normal_Mode_ParamV5 mix_rx_dl_ul_ca_normal_mode_param;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct Mix_Rx_Tx_Force_Mode_V5
        {

            /// unsigned char
            public byte mode;

            /// unsigned char
            public byte measCnt;

            /// unsigned short
            public ushort dlFrequency;

            /// unsigned char
            public byte band;

            /// unsigned char
            public byte duplexMode;

            /// unsigned char
            public byte tddConfig;

            /// unsigned char
            public byte tddSfConfig;

            /// unsigned char
            public byte dlBandwidth;

            /// unsigned short
            public ushort rxRoute;

            /// unsigned char
            public byte lpmHpmConfig;

            /// unsigned short
            public ushort ulFrequency;

            /// short
            public short txPowerValue;

            /// unsigned char
            public byte vrbStart;

            /// unsigned char
            public byte vrbLength;

            /// unsigned char
            public byte mcsMode;

            /// unsigned short
            public ushort txRoute;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
        public struct Mix_Rx_Dl_Ul_Ca_Mode_V5
        {

            /// Mix_Rx_Dl_Ul_Ca_Normal_Mode_V5->Anonymous_1b5ae2c6_93d8_478d_8cb9_28c1c9187b5b
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public Mix_Rx_Dl_Ul_Ca_Normal_Mode_V5 mix_rx_dl_ul_ca_normal_mode;

            /// Mix_Rx_Tx_Force_Mode_V5->Anonymous_324c001e_6a69_455e_a88a_094083b98f5e
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public Mix_Rx_Tx_Force_Mode_V5 mix_rx_tx_force_mode;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERFTestCmd_StartMixRxCaV5_ReqParam
        {

            /// unsigned char
            public byte opMode;

            /// Mix_Rx_Dl_Ul_Ca_Mode_V5->Anonymous_b5fa6b0d_cb37_405f_bd9d_38fbab077dfd
            public Mix_Rx_Dl_Ul_Ca_Mode_V5 reqParam;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestCmdGetMixRxRpt_CaMode_V2
        {

            /// unsigned char[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 4)]
            public string measRptCnt;

            /// short[40]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 40, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rssiPath0;

            /// short[40]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 40, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rssiPath1;

            /// short[40]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 40, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrpPath0;

            /// short[40]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 40, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrpPath1;

            /// short[40]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 40, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrqPath0;

            /// short[40]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 40, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrqPath1;

            /// unsigned int[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U4)]
            public uint[] crcOK_cnt;

            /// unsigned int[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U4)]
            public uint[] crcNG_cnt;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestResultGetMixRxRpt_CaMode_V5
        {

            /// unsigned char
            public byte measRptCnt;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rssiPath0;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rssiPath1;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rssiPath2;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rssiPath3;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrpPath0;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrpPath1;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrpPath2;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrpPath3;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrqPath0;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrqPath1;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrqPath2;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrqPath3;

            /// unsigned int
            public uint crcOK_cnt;

            /// unsigned int
            public uint crcNG_cnt;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NL1TST_CAL_SWITCH_TIME_T
        {

            /// is_capable : 1
            ///is_mandatory : 1
            ///fdd_2_tdd_swt : 3
            ///fdd_tx_2_rx_swt : 3
            ///tdd_tx_2_rx_swt : 3
            ///freq_swt : 3
            ///band_swt : 3
            ///tx_step_width : 3
            ///reserve : 12
            public uint bitvector1;

            public uint is_capable
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint is_mandatory
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint fdd_2_tdd_swt
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 28u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }

            public uint fdd_tx_2_rx_swt
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 224u)
                                / 32)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 32)
                                | this.bitvector1)));
                }
            }

            public uint tdd_tx_2_rx_swt
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 1792u)
                                / 256)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 256)
                                | this.bitvector1)));
                }
            }

            public uint freq_swt
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 14336u)
                                / 2048)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2048)
                                | this.bitvector1)));
                }
            }

            public uint band_swt
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 114688u)
                                / 16384)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 16384)
                                | this.bitvector1)));
                }
            }

            public uint tx_step_width
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 917504u)
                                / 131072)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 131072)
                                | this.bitvector1)));
                }
            }

            public uint reserve
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4293918720u)
                                / 1048576)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 1048576)
                                | this.bitvector1)));
                }
            }
        }

        public enum NL1TST_GEN_VERSION_E
        {

            /// NL1TST_MODEM_NULL -> 0
            NL1TST_MODEM_NULL = 0,

            /// NL1TST_MODEM_V7 -> 1
            NL1TST_MODEM_V7 = 1,

            /// NL1TST_MODEM_END -> 0x7FFFFFFF
            NL1TST_MODEM_END = 2147483647,
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NL1TST_GENERATION_VERNO_T
        {

            /// is_capable : 1
            ///is_mandatory : 1
            ///generation : 8
            ///reserve : 22
            public uint bitvector1;

            public uint is_capable
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint is_mandatory
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint generation
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 1020u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }

            public uint reserve
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4294966272u)
                                / 1024)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 1024)
                                | this.bitvector1)));
                }
            }
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NL1TST_CAL_MEAS_INFO_T
        {

            /// is_capable : 1
            ///is_mandatory : 1
            ///meas_start_time : 14
            ///meas_duration : 14
            ///reserve : 2
            public uint bitvector1;

            public uint is_capable
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint is_mandatory
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint meas_start_time
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 65532u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }

            public uint meas_duration
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 1073676288u)
                                / 65536)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 65536)
                                | this.bitvector1)));
                }
            }

            public uint reserve
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 3221225472u)
                                / 1073741824)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 1073741824)
                                | this.bitvector1)));
                }
            }
        }

        public enum NL1TST_RFID_E
        {

            /// NL1TST_RFID_NULL -> 0
            NL1TST_RFID_NULL = 0,

            /// NL1TST_RFID_COLUMBUS -> 1
            NL1TST_RFID_COLUMBUS = 1,

            /// NL1TST_RFID_END -> 0x7FFFFFFF
            NL1TST_RFID_END = 2147483647,
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NRfTestCmdRfCapabilityReq
        {

            /// unsigned int
            public uint capabilityItemsSize;

            /// unsigned int
            public uint calibrationItemsSize;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NrfCalibrationItem
        {

            /// is_capable : 1
            ///is_mandatory : 1
            ///parameter : 30
            public uint bitvector1;

            public uint is_capable
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint is_mandatory
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint parameter
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4294967292u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NrfCapabilityItemSet
        {

            /// unsigned int
            public uint reserved;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NrfCalibrationItemSet
        {

            /// NrfCalibrationItem->Anonymous_43f8cb4b_738e_4f0a_8d28_45f5c967e444
            public NrfCalibrationItem generation_version;

            /// NrfCalibrationItem->Anonymous_43f8cb4b_738e_4f0a_8d28_45f5c967e444
            public NrfCalibrationItem fhc_sw_time;

            /// NrfCalibrationItem->Anonymous_43f8cb4b_738e_4f0a_8d28_45f5c967e444
            public NrfCalibrationItem fhc_rx_measurement_info;

            /// NrfCalibrationItem->Anonymous_43f8cb4b_738e_4f0a_8d28_45f5c967e444
            public NrfCalibrationItem fhc_tx_measurement_info;

            /// NrfCalibrationItem->Anonymous_43f8cb4b_738e_4f0a_8d28_45f5c967e444
            public NrfCalibrationItem used_cc_num_info;

            /// NrfCalibrationItem->Anonymous_43f8cb4b_738e_4f0a_8d28_45f5c967e444
            public NrfCalibrationItem dpd_support_info;

            /// NrfCalibrationItem->Anonymous_43f8cb4b_738e_4f0a_8d28_45f5c967e444
            public NrfCalibrationItem et_support_info;

            /// NrfCalibrationItem->Anonymous_43f8cb4b_738e_4f0a_8d28_45f5c967e444
            public NrfCalibrationItem dpd_performance_check_support_info;

            /// NrfCalibrationItem->Anonymous_43f8cb4b_738e_4f0a_8d28_45f5c967e444
            public NrfCalibrationItem et_special_function_bitmap;

            /// NrfCalibrationItem->Anonymous_43f8cb4b_738e_4f0a_8d28_45f5c967e444
            public NrfCalibrationItem sensitivity_cal_dl_power;

            /// NrfCalibrationItem->Anonymous_43f8cb4b_738e_4f0a_8d28_45f5c967e444
            public NrfCalibrationItem nr_sync_free_nsft_info;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NRfTestCmdRfCapabilityCnf
        {

            /// int
            public int valid;

            /// int
            public int status;

            /// unsigned int
            public uint rfId;

            /// NrfCalibrationItemSet->Anonymous_890059e2_9025_4be6_bf16_003afb332e1a
            public NrfCalibrationItemSet calibrationItems;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NRfTestCmd_GetDlFreqV7
        {

            /// unsigned short
            public ushort band;

            /// unsigned int
            public uint ulFrequency;

            /// unsigned char
            public byte puschOpMode;

            /// unsigned short
            public ushort txRoute;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NRfTestResult_GetDlFreqV7
        {

            /// unsigned int
            public uint getStatus;

            /// unsigned int
            public uint dlFrequency;
        }


        public enum NL1TST_BW_SCS_15_KHZ_E
        {

            /// NL1TST_BW_SCS_15_KHZ_25RB -> 5000
            NL1TST_BW_SCS_15_KHZ_25RB = 5000,

            /// NL1TST_BW_SCS_15_KHZ_52RB -> 10000
            NL1TST_BW_SCS_15_KHZ_52RB = 10000,

            /// NL1TST_BW_SCS_15_KHZ_79RB -> 15000
            NL1TST_BW_SCS_15_KHZ_79RB = 15000,

            /// NL1TST_BW_SCS_15_KHZ_106RB -> 20000
            NL1TST_BW_SCS_15_KHZ_106RB = 20000,

            /// NL1TST_BW_SCS_15_KHZ_133RB -> 25000
            NL1TST_BW_SCS_15_KHZ_133RB = 25000,

            /// NL1TST_BW_SCS_15_KHZ_160RB -> 30000
            NL1TST_BW_SCS_15_KHZ_160RB = 30000,

            /// NL1TST_BW_SCS_15_KHZ_216RB -> 40000
            NL1TST_BW_SCS_15_KHZ_216RB = 40000,

            /// NL1TST_BW_SCS_15_KHZ_270RB -> 50000
            NL1TST_BW_SCS_15_KHZ_270RB = 50000,

            /// NL1TST_BW_SCS_15_KHZ_MAX -> 0x7FFFFFFF
            NL1TST_BW_SCS_15_KHZ_MAX = 2147483647,
        }

        public enum NL1TST_BW_SCS_30_KHZ_E
        {

            /// NL1TST_BW_SCS_30_KHZ_11RB -> 5000
            NL1TST_BW_SCS_30_KHZ_11RB = 5000,

            /// NL1TST_BW_SCS_30_KHZ_24RB -> 10000
            NL1TST_BW_SCS_30_KHZ_24RB = 10000,

            /// NL1TST_BW_SCS_30_KHZ_38RB -> 15000
            NL1TST_BW_SCS_30_KHZ_38RB = 15000,

            /// NL1TST_BW_SCS_30_KHZ_51RB -> 20000
            NL1TST_BW_SCS_30_KHZ_51RB = 20000,

            /// NL1TST_BW_SCS_30_KHZ_65RB -> 25000
            NL1TST_BW_SCS_30_KHZ_65RB = 25000,

            /// NL1TST_BW_SCS_30_KHZ_78RB -> 30000
            NL1TST_BW_SCS_30_KHZ_78RB = 30000,

            /// NL1TST_BW_SCS_30_KHZ_106RB -> 40000
            NL1TST_BW_SCS_30_KHZ_106RB = 40000,

            /// NL1TST_BW_SCS_30_KHZ_133RB -> 50000
            NL1TST_BW_SCS_30_KHZ_133RB = 50000,

            /// NL1TST_BW_SCS_30_KHZ_162RB -> 60000
            NL1TST_BW_SCS_30_KHZ_162RB = 60000,

            /// NL1TST_BW_SCS_30_KHZ_217RB -> 80000
            NL1TST_BW_SCS_30_KHZ_217RB = 80000,

            /// NL1TST_BW_SCS_30_KHZ_273RB -> 100000
            NL1TST_BW_SCS_30_KHZ_273RB = 100000,

            /// NL1TST_BW_SCS_30_KHZ_MAX -> 0x7FFFFFFF
            NL1TST_BW_SCS_30_KHZ_MAX = 2147483647,
        }

        public enum NL1TST_BW_SCS_60_KHZ_E
        {

            /// NL1TST_BW_SCS_60_KHZ_11RB -> 10000
            NL1TST_BW_SCS_60_KHZ_11RB = 10000,

            /// NL1TST_BW_SCS_60_KHZ_18RB -> 15000
            NL1TST_BW_SCS_60_KHZ_18RB = 15000,

            /// NL1TST_BW_SCS_60_KHZ_24RB -> 20000
            NL1TST_BW_SCS_60_KHZ_24RB = 20000,

            /// NL1TST_BW_SCS_60_KHZ_31RB -> 25000
            NL1TST_BW_SCS_60_KHZ_31RB = 25000,

            /// NL1TST_BW_SCS_60_KHZ_38RB -> 30000
            NL1TST_BW_SCS_60_KHZ_38RB = 30000,

            /// NL1TST_BW_SCS_60_KHZ_51RB -> 40000
            NL1TST_BW_SCS_60_KHZ_51RB = 40000,

            /// NL1TST_BW_SCS_60_KHZ_65RB -> 50000
            NL1TST_BW_SCS_60_KHZ_65RB = 50000,

            /// NL1TST_BW_SCS_60_KHZ_79RB -> 60000
            NL1TST_BW_SCS_60_KHZ_79RB = 60000,

            /// NL1TST_BW_SCS_60_KHZ_107RB -> 80000
            NL1TST_BW_SCS_60_KHZ_107RB = 80000,

            /// NL1TST_BW_SCS_60_KHZ_135RB -> 100000
            NL1TST_BW_SCS_60_KHZ_135RB = 100000,

            /// NL1TST_BW_SCS_60_KHZ_MAX -> 0x7FFFFFFF
            NL1TST_BW_SCS_60_KHZ_MAX = 2147483647,
        }

        public enum NL1TST_MCS_E
        {

            /// NL1TST_MCS_DFT_S_BPSK -> 0
            NL1TST_MCS_DFT_S_BPSK = 0,

            /// NL1TST_MCS_CP_QPSK -> 1
            NL1TST_MCS_CP_QPSK = 1,

            /// NL1TST_MCS_DFT_S_QPSK -> 2
            NL1TST_MCS_DFT_S_QPSK = 2,

            /// NL1TST_MCS_CP_16QAM -> 3
            NL1TST_MCS_CP_16QAM = 3,

            /// NL1TST_MCS_DFT_S_16QAM -> 4
            NL1TST_MCS_DFT_S_16QAM = 4,

            /// NL1TST_MCS_CP_64QAM -> 5
            NL1TST_MCS_CP_64QAM = 5,

            /// NL1TST_MCS_DFT_S_64QAM -> 6
            NL1TST_MCS_DFT_S_64QAM = 6,

            /// NL1TST_MCS_CP_256QAM -> 7
            NL1TST_MCS_CP_256QAM = 7,

            /// NL1TST_MCS_DFT_S_256QAM -> 8
            NL1TST_MCS_DFT_S_256QAM = 8,

            /// NL1TST_MCS_MAX -> 0x7FFFFFFF
            NL1TST_MCS_MAX = 2147483647,
        }

        public enum NL1TST_SCS_E
        {

            /// NL1TST_SCS_15KHZ -> 0
            NL1TST_SCS_15KHZ = 0,

            /// NL1TST_SCS_30KHZ -> 1
            NL1TST_SCS_30KHZ = 1,

            /// NL1TST_SCS_60KHZ -> 2
            NL1TST_SCS_60KHZ = 2,

            /// NL1TST_SCS_120KHZ -> 3
            NL1TST_SCS_120KHZ = 3,

            /// NL1TST_SCS_240KHZ -> 4
            NL1TST_SCS_240KHZ = 4,

            /// NL1TST_SCS_MAX -> 0x7FFFFFFF
            NL1TST_SCS_MAX = 2147483647,
        }

        public enum NL1TST_APT_E
        {

            /// NL1TST_APT_0 -> 0
            NL1TST_APT_0 = 0,

            /// NL1TST_APT_1 -> 1
            NL1TST_APT_1 = 1,

            /// NL1TST_APT_2 -> 2
            NL1TST_APT_2 = 2,

            /// NL1TST_APT_3 -> 3
            NL1TST_APT_3 = 3,

            /// NL1TST_APT_4 -> 4
            NL1TST_APT_4 = 4,

            /// NL1TST_APT_5 -> 5
            NL1TST_APT_5 = 5,

            /// NL1TST_APT_6 -> 6
            NL1TST_APT_6 = 6,

            /// NL1TST_APT_7 -> 7
            NL1TST_APT_7 = 7,

            /// NL1TST_APT_MAX -> 0x7FFFFFFF
            NL1TST_APT_MAX = 2147483647,
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
        public struct NL1TST_BW_KHZ_U
        {

            /// NL1TST_BW_SCS_15_KHZ_E->Anonymous_92d5fc53_20e2_4ed8_b98a_1ae19b42483b
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public NL1TST_BW_SCS_15_KHZ_E scs_15_enum;

            /// NL1TST_BW_SCS_30_KHZ_E->Anonymous_96d64b93_9160_4c02_a0d4_481e209e988c
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public NL1TST_BW_SCS_30_KHZ_E scs_30_enum;

            /// NL1TST_BW_SCS_60_KHZ_E->Anonymous_dd3fefb4_0c55_4080_83ed_56c29b3dca36
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public NL1TST_BW_SCS_60_KHZ_E scs_60_enum;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
        public struct Anonymous_1ad40c67_ed85_4617_8a5f_15c502bd98d3
        {

            /// NL1TST_BW_SCS_15_KHZ_E->Anonymous_92d5fc53_20e2_4ed8_b98a_1ae19b42483b
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public NL1TST_BW_SCS_15_KHZ_E scs_15_enum;

            /// NL1TST_BW_SCS_30_KHZ_E->Anonymous_96d64b93_9160_4c02_a0d4_481e209e988c
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public NL1TST_BW_SCS_30_KHZ_E scs_30_enum;

            /// NL1TST_BW_SCS_60_KHZ_E->Anonymous_dd3fefb4_0c55_4080_83ed_56c29b3dca36
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public NL1TST_BW_SCS_60_KHZ_E scs_60_enum;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NL1TSTCmd_StartForcePuschTx_ReqParam
        {

            /// unsigned short
            public ushort band;

            /// unsigned char
            public byte tddSlotConfig;

            /// unsigned int
            public uint ulFrequency;

            /// unsigned char
            public byte sulFreqShift;

            /// unsigned short
            public ushort cellId;

            /// Anonymous_1ad40c67_ed85_4617_8a5f_15c502bd98d3
            public Anonymous_1ad40c67_ed85_4617_8a5f_15c502bd98d3 Union1;

            /// unsigned short
            public ushort vrbStart;

            /// unsigned short
            public ushort vrbLength;

            /// NL1TST_SCS_E->Anonymous_de3bcf30_032c_4ca3_ab32_858d9003b087
            public NL1TST_SCS_E txScs;

            /// NL1TST_MCS_E->Anonymous_0c2907a4_0258_4f33_935f_68ccfe90124f
            public NL1TST_MCS_E mcsMode;

            /// short
            public short txPowerValue;

            /// NL1TST_APT_E->Anonymous_d42784e1_be5d_41c2_bd8c_af09bc3cd413
            public NL1TST_APT_E networkSelection;

            /// unsigned char
            public byte txCloseLoopDisbl;

            /// unsigned char
            public byte amprEnbl;

            /// unsigned short
            public ushort txRoute;

            /// unsigned char
            public byte enableCsr;

            /// unsigned char
            public byte enableMimo;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NL1TSTCmd_StartNormalPuschTx_ReqParam
        {

            /// unsigned char
            public byte ulCCNum;

            /// NL1TSTCmd_StartForcePuschTx_ReqParam[]
            public NL1TSTCmd_StartForcePuschTx_ReqParam[] puschTxParam;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
        public struct Anonymous_dff2166d_a7ad_49bf_8527_456314e7b630
        {

            /// NL1TSTCmd_StartForcePuschTx_ReqParam->Anonymous_c82fe71e_bb94_4107_9e40_cd04da4872c8
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public NL1TSTCmd_StartForcePuschTx_ReqParam forcePuschTxReq;

            /// NL1TSTCmd_StartNormalPuschTx_ReqParam->Anonymous_544e257d_56f0_4639_bf91_e7d89f7c252d
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public NL1TSTCmd_StartNormalPuschTx_ReqParam normalPuschTxReq;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NRfTestCmd_StartPuschTxCaV7
        {

            /// unsigned char
            public byte puschOpMode;

            /// Anonymous_dff2166d_a7ad_49bf_8527_456314e7b630
            public Anonymous_dff2166d_a7ad_49bf_8527_456314e7b630 Union1;
        }

        public partial class NativeMethods
        {

            /// Return Type: char*
            ///meta_handle: int
            ///id: unsigned int
            [System.Runtime.InteropServices.DllImportAttribute("<Unknown>", EntryPoint = "META_NRf_GetRfChipIdName_r", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
            public static extern System.IntPtr META_NRf_GetRfChipIdName_r(int meta_handle, uint id);

        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
        public struct Anonymous_682ab258_49c7_42d9_8f61_0390195e838e
        {

            /// NL1TST_BW_SCS_15_KHZ_E->Anonymous_67b804ce_56b8_4a36_970e_74d5ee6090b0
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public NL1TST_BW_SCS_15_KHZ_E scs_15_enum;

            /// NL1TST_BW_SCS_30_KHZ_E->Anonymous_e24ccdd9_b7f8_4bb3_b0e9_9ef77188be4b
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public NL1TST_BW_SCS_30_KHZ_E scs_30_enum;

            /// NL1TST_BW_SCS_60_KHZ_E->Anonymous_148004e2_f3b7_411c_95f0_52b656126c8c
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public NL1TST_BW_SCS_60_KHZ_E scs_60_enum;

            /// unsigned int
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public uint dlBandwidth;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NL1TSTCmd_StartForceMixRx_ReqParam
        {

            /// unsigned char
            public byte measCnt;

            /// unsigned int
            public uint dlFrequency;

            /// NL1TST_SCS_E->Anonymous_3b79bc55_ca1c_4ae3_9b42_7eb6d87125a1
            public NL1TST_SCS_E trxScs;

            /// unsigned short
            public ushort band;

            /// unsigned char
            public byte tddSlotConfig;

            /// Anonymous_682ab258_49c7_42d9_8f61_0390195e838e
            public Anonymous_682ab258_49c7_42d9_8f61_0390195e838e Union1;

            /// unsigned char
            public byte enableTx;

            /// short
            public short txPowerValue;

            /// unsigned short
            public ushort vrbStart;

            /// unsigned short
            public ushort vrbLength;

            /// NL1TST_MCS_E->Anonymous_4beee3ea_34eb_4fec_80e7_6e3fc984a578
            public NL1TST_MCS_E mcsMode;

            /// unsigned short
            public ushort rxRoute;

            /// unsigned short
            public ushort txRoute;

            /// unsigned char
            public byte lpmHpmConf;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
        public struct Anonymous_5f7e42ca_f592_43ad_9b16_5600a01be630
        {

            /// NL1TST_BW_SCS_15_KHZ_E->Anonymous_67b804ce_56b8_4a36_970e_74d5ee6090b0
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public NL1TST_BW_SCS_15_KHZ_E scs_15_enum;

            /// NL1TST_BW_SCS_30_KHZ_E->Anonymous_e24ccdd9_b7f8_4bb3_b0e9_9ef77188be4b
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public NL1TST_BW_SCS_30_KHZ_E scs_30_enum;

            /// NL1TST_BW_SCS_60_KHZ_E->Anonymous_148004e2_f3b7_411c_95f0_52b656126c8c
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public NL1TST_BW_SCS_60_KHZ_E scs_60_enum;

            /// unsigned int
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public uint dlBandwidth;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NL1TST_NORMAL_MIX_RX_DL_UL_PARAM_T
        {

            /// unsigned char
            public byte measCnt;

            /// unsigned int
            public uint dlFrequency;

            /// NL1TST_SCS_E->Anonymous_3b79bc55_ca1c_4ae3_9b42_7eb6d87125a1
            public NL1TST_SCS_E trxScs;

            /// unsigned short
            public ushort band;

            /// unsigned char
            public byte tddSlotConfig;

            /// Anonymous_5f7e42ca_f592_43ad_9b16_5600a01be630
            public Anonymous_5f7e42ca_f592_43ad_9b16_5600a01be630 Union1;

            /// unsigned char
            public byte enableTx;

            /// short
            public short txPowerValue;

            /// unsigned short
            public ushort vrbStart;

            /// unsigned short
            public ushort vrbLength;

            /// NL1TST_MCS_E->Anonymous_4beee3ea_34eb_4fec_80e7_6e3fc984a578
            public NL1TST_MCS_E mcsMode;

            /// unsigned char
            public byte enableMimo;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NL1TSTCmd_StartNormalMixRx_ReqParam
        {

            /// unsigned char
            public byte numOfCC;

            /// NL1TST_NORMAL_MIX_RX_DL_UL_PARAM_T[]
            public NL1TST_NORMAL_MIX_RX_DL_UL_PARAM_T[] param;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
        public struct Anonymous_50c01d2e_243f_4ebe_a48e_68adffc5e5b4
        {

            /// NL1TSTCmd_StartForceMixRx_ReqParam->Anonymous_9c2c3c93_f5a5_4f1c_8116_aa50ad3ef633
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public NL1TSTCmd_StartForceMixRx_ReqParam forceRsrpRsrqCmd;

            /// NL1TSTCmd_StartNormalMixRx_ReqParam->Anonymous_3c6aaf15_b2f5_4727_bd4a_f396ab5d7aca
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public NL1TSTCmd_StartNormalMixRx_ReqParam normalRsrpRsrqCmd;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NRfTestCmd_StartMixRxCaV7
        {

            /// unsigned char
            public byte startMixRxOpMode;

            /// Anonymous_50c01d2e_243f_4ebe_a48e_68adffc5e5b4
            public Anonymous_50c01d2e_243f_4ebe_a48e_68adffc5e5b4 Union1;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NRfTestResult_GetMixRxReportV7
        {

            /// unsigned char
            public byte measRptCnt;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rssiPath0;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rssiPath1;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rssiPath2;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rssiPath3;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrpPath0;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrpPath1;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrpPath2;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrpPath3;

            /// unsigned int
            public uint crcOK_cnt;

            /// unsigned int
            public uint crcNG_cnt;
        }

        public enum METAAPP_TEST_RAT_E
        {
            T_GSM = 0,
            T_TDS,
            T_WCDMA,
            T_LTE,
            T_C2K,
            T_EXIT_C2K
        }



        public enum METAAPP_DISCONNECT_META_PARA_E
        {

            /// METAAPP_Disconnect_DoNothing -> 0
            METAAPP_Disconnect_DoNothing = 0,

            /// METAAPP_Disconnect_Poweroff -> 1
            METAAPP_Disconnect_Poweroff = 1,

            /// METAAPP_Disconnect_Reboot -> 2
            METAAPP_Disconnect_Reboot = 2,

            /// METAAPP_Disconnect_UnmountData -> 3
            METAAPP_Disconnect_UnmountData = 3,

            /// METAAPP_Disconnect_KillWifiAPK -> 4
            METAAPP_Disconnect_KillWifiAPK = 4,

            /// METAAPP_Disconnect_NUM -> METAAPP_Disconnect_KillWifiAPK+1
            METAAPP_Disconnect_NUM = (METAAPP_DISCONNECT_META_PARA_E.METAAPP_Disconnect_KillWifiAPK + 1),
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct METAAPP_DISCONNECT_META_T
        {

            /// METAAPP_DISCONNECT_META_PARA_E->Anonymous_32797794_2fd7_4a6d_b8a0_374f24af1948
            public METAAPP_DISCONNECT_META_PARA_E eDisconPara;

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool bDoBackupNv;
        }


        /// Return Type: void
        ///logBuf: char*
        ///cbUserDate: void*
        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        public delegate void METAAPP_Conn_Log_Display_CallBackForMultiThread([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string logBuf, IntPtr cbUserDate);



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestVcoSrxLnaCfg
        {

            /// unsigned char
            public byte vco;

            /// unsigned char
            public byte srx;

            /// unsigned char
            public byte lna_port;

            /// unsigned char
            public byte lna_group;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestSupportVcoSrxLnaCfg
        {

            /// unsigned char
            public byte band;

            /// unsigned char[3]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 3)]
            public string padding;

            /// ERfTestVcoSrxLnaCfg[3]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public ERfTestVcoSrxLnaCfg[] cfg;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestUsageElm
        {

            /// unsigned char
            public byte band;

            /// unsigned char
            public byte route_idx;

            /// unsigned char
            public byte usg_start_idx;

            /// unsigned char
            public byte usg_stop_idx;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestSupportRxUsageByBand
        {

            /// ERfTestUsageElm[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public ERfTestUsageElm[] cat;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestSupportCaCombTblIdx
        {

            /// unsigned char
            public byte band;

            /// unsigned char[15]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 15)]
            public string index;
        }

        public enum ERfTestVcoSrxSel
        {

            /// VCO_SRX_SEL_INVALID -> 0
            VCO_SRX_SEL_INVALID = 0,

            /// VCO_SRX_SEL_VCO -> 1
            VCO_SRX_SEL_VCO = 1,

            /// VCO_SRX_SEL_SRX -> 2
            VCO_SRX_SEL_SRX = 2,
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestRouteDes
        {

            /// unsigned char
            public byte band;

            /// unsigned char
            public byte bw_class;

            /// unsigned char
            public byte lna_port;

            /// unsigned char
            public byte lna_group;

            /// unsigned char
            public byte vco_cfg;

            /// unsigned char
            public byte srx_cfg;

            /// unsigned char[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 2)]
            public string padding;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestScenarioDes
        {

            /// ERfTestRouteDes[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public ERfTestRouteDes[] cfg;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestCaCombinationTable
        {

            /// int
            public int table_idx_count;

            /// ERfTestScenarioDes[128]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public ERfTestScenarioDes[] caCombinationTableElement;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestCcElm
        {

            /// unsigned char
            public byte cc_band;

            /// unsigned char
            public byte cc_class;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestUsageDes
        {

            /// unsigned char
            public byte cc_num;

            /// unsigned char
            public byte usg_band;

            /// ERfTestCcElm[2]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public ERfTestCcElm[] cc_setting;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestUsageDesTable
        {

            /// unsigned short
            public ushort usg_num;

            /// ERfTestUsageDes[256]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 256, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public ERfTestUsageDes[] usg_array;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestSupportCaCalDim
        {

            /// unsigned char
            public byte band;

            /// unsigned char
            public byte ca_cat_rx_cal_bitmap;

            /// unsigned char
            public byte ca_cat_tx_cal_bitmap;

            /// unsigned char
            public byte padding;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestSupportRxGainModeMask
        {

            /// gain_mode1_ena : 1
            ///gain_mode2_ena : 1
            ///gain_mode3_ena : 1
            ///gain_mode4_ena : 1
            ///gain_mode5_ena : 1
            ///gain_mode6_ena : 1
            ///gain_mode_lpm_ena : 1
            ///gain_mode_reserved : 9
            public uint bitvector1;

            /// unsigned short
            public ushort gain_by_gain_expn_type;

            public uint gain_mode1_ena
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint gain_mode2_ena
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint gain_mode3_ena
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }

            public uint gain_mode4_ena
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 8u)
                                / 8)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 8)
                                | this.bitvector1)));
                }
            }

            public uint gain_mode5_ena
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 16u)
                                / 16)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 16)
                                | this.bitvector1)));
                }
            }

            public uint gain_mode6_ena
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 32u)
                                / 32)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 32)
                                | this.bitvector1)));
                }
            }

            public uint gain_mode_lpm_ena
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 64u)
                                / 64)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 64)
                                | this.bitvector1)));
                }
            }

            public uint gain_mode_reserved
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 65408u)
                                / 128)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 128)
                                | this.bitvector1)));
                }
            }
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestSupportRxGain
        {

            /// unsigned short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U2)]
            public ushort[] gain_mode_setting;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct ERfTestNccaGbgBypassCheck
        {

            /// char[72]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 72)]
            public string ncca_gbg_bypass_check_table;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestCmdCaConfig
        {

            /// unsigned int[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U4)]
            public uint[] support_route_map;

            /// unsigned int
            public uint ltea_vco_srx_sel;

            /// ERfTestSupportVcoSrxLnaCfg[21]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 21, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public ERfTestSupportVcoSrxLnaCfg[] support_vco_srx_lna_cfg;

            /// ERfTestCaCombinationTable->Anonymous_6acf7010_1664_4c71_bd8b_3de0e3631cdb
            public ERfTestCaCombinationTable ca_comb_tbl;

            /// ERfTestSupportCaCombTblIdx[21]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 21, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public ERfTestSupportCaCombTblIdx[] support_ca_cfg_tbl_idx;

            /// ERfTestUsageDesTable->Anonymous_15bede2e_a3c9_42ef_b071_4067bedbc5ef
            public ERfTestUsageDesTable rx_usg_tbl;

            /// ERfTestSupportRxUsageByBand[21]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 21, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public ERfTestSupportRxUsageByBand[] support_rx_usage_by_band;

            /// ERfTestSupportCaCalDim[21]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 21, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public ERfTestSupportCaCalDim[] ca_cal_dim;

            /// ERfTestSupportRxGainModeMask->Anonymous_0052c053_77c8_4d74_aef6_2b1225309efb
            public ERfTestSupportRxGainModeMask support_rx_gain_mode_mask;

            /// ERfTestSupportRxGain->Anonymous_ff693524_bbbc_4b4b_8778_a6da0b7a7bbe
            public ERfTestSupportRxGain support_rx_gain_mode_setting;

            /// ERfTestNccaGbgBypassCheck->Anonymous_ef678e6d_d3f8_4817_a724_8f2df980eb1a
            public ERfTestNccaGbgBypassCheck ncca_gbg_bypass_check_setting;

            /// unsigned int[64]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U4)]
            public uint[] band_mapping_table;
        }


        public enum GSM_AntDimension
        {

            /// GSM_ANT_MASK_NULL -> 0x0
            GSM_ANT_MASK_NULL = 0,

            /// GSM_ANT_MASK_RXM -> 0x1
            GSM_ANT_MASK_RXM = 1,

            /// GSM_ANT_MASK_RXD -> 0x2
            GSM_ANT_MASK_RXD = 2,

            /// GSM_ANT_MASK_BOTH -> 0x3
            GSM_ANT_MASK_BOTH = 3,
        }

        public enum CodingScheme
        {

            /// CodingSchemeNone -> 0
            CodingSchemeNone = 0,

            CodingSchemeCS1,

            CodingSchemeCS2,

            CodingSchemeCS3,

            CodingSchemeCS4,

            CodingSchemePRACh8,

            CodingSchemePRACh11,

            CodingSchemeMCS1,

            CodingSchemeMCS2,

            CodingSchemeMCS3,

            CodingSchemeMCS4,

            CodingSchemeMCS5,

            CodingSchemeMCS6,

            CodingSchemeMCS7,

            CodingSchemeMCS8,

            CodingSchemeMCS9,

            CodingSchemeCount,
        }

        public enum FrequencyBand
        {

            /// FrequencyBand400 -> 0
            FrequencyBand400 = 0,

            /// FrequencyBand850 -> 1
            FrequencyBand850 = 1,

            /// FrequencyBand900 -> 2
            FrequencyBand900 = 2,

            /// FrequencyBand1800 -> 3
            FrequencyBand1800 = 3,

            /// FrequencyBand1900 -> 4
            FrequencyBand1900 = 4,

            FrequencyBandCount,
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct Rf_NSFT_REQ_V5_T
        {

            /// FrequencyBand->Anonymous_c9728a2a_003d_462e_847a_58d0c7464c76
            public FrequencyBand band;

            /// ARFCN->short
            public short BCH_ARFCN;

            /// ARFCN->short
            public short TCH_ARFCN;

            /// Power->short
            public short BCH_DL_Power;

            /// Power->short
            public short TCH_DL_Power;

            /// TSC->char
            public byte tsc;

            /// TimeSlot->char
            public byte TCH_slot;

            /// Power->short
            public short tx_power_level;

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool is_EPSK_tx;

            /// CodingScheme->Anonymous_942aace4_7cab_4e2e_935e_fbc3602914ee
            public CodingScheme epsk_cs;

            /// GSM_AntDimension->Anonymous_44a675f9_3816_4d3b_bd65_1b5035d5a0ce
            public GSM_AntDimension Antenna;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct Rf_NSFT_REQ_T
        {

            /// FrequencyBand->Anonymous_ad827a1f_a975_4786_800c_20b53e7153c8
            public FrequencyBand band;

            /// ARFCN->short
            public short BCH_ARFCN;

            /// ARFCN->short
            public short TCH_ARFCN;

            /// Gain->short
            public short BCH_gain;

            /// Gain->short
            public short TCH_gain;

            /// TSC->char
            public byte tsc;

            /// TimeSlot->char
            public byte TCH_slot;

            /// Power->short
            public short tx_power_level;

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool is_EPSK_tx;

            /// CodingScheme->Anonymous_5da15f91_70ce_4d01_88ad_0699543f9151
            public CodingScheme epsk_cs;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct RF_NSFT_SBERResult_T
        {

            /// unsigned int
            public uint m_u4NSFTSBERSum;

            /// unsigned int
            public uint m_u4NSFTSBERCurrentCount;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct C2kMsCapability
        {

            /// unsigned short
            public ushort rfId;

            /// unsigned short
            public ushort xoType;

            /// unsigned int
            public uint bandSupport;

            /// unsigned int
            public uint rxdBandSupport;

            /// unsigned int
            public uint mipiBandSupport;

            /// unsigned int
            public uint dpdBandSupport;

            /// unsigned int
            public uint rxdPathNum;

            /// unsigned int
            public uint nsftListModeType;

            /// unsigned int
            public uint rxCalCW;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct C2kNsftCmdTestMode
        {

            /// unsigned int
            public uint reserved;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct C2kNsftResultStatus
        {

            /// unsigned int
            public uint status;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct C2kNsftCmdPowerUp
        {

            /// unsigned short
            public ushort band;

            /// unsigned short
            public ushort channel;

            /// unsigned char
            public byte walshCode;

            /// unsigned char
            public byte radioConfig;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct C2kNsftCmdSetTxPower
        {

            /// unsigned short
            public ushort band;

            /// unsigned short
            public ushort channel;

            /// double
            public double txPower;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct C2kNsftCmdFer
        {

            /// unsigned short
            public ushort band;

            /// unsigned short
            public ushort channel;

            /// unsigned short
            public ushort numFrames;

            /// unsigned char
            public byte enableAfc;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct C2kNsftResultFer
        {

            /// unsigned int
            public uint status;

            /// unsigned short
            public ushort badFrames;

            /// unsigned short
            public ushort totalFrames;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct C2kNsftCmdRssi
        {

            /// unsigned short
            public ushort band;

            /// unsigned short
            public ushort channel;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct C2kNsftResultRssi
        {

            /// unsigned int
            public uint status;

            /// unsigned short
            public ushort pnOffset;

            /// unsigned short
            public ushort strength;

            /// double
            public double mainRssi;

            /// double
            public double divRssi;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct C2K_MS_CAPABILITY
        {

            /// unsigned int
            public uint bandSupport;

            /// unsigned int
            public uint rxdBandSupport;

            /// unsigned int
            public uint rxdEnabled;

            /// unsigned int
            public uint paOctLevel;

            /// unsigned int
            public uint rfId;

            /// unsigned char
            public byte cpMajor;

            /// unsigned char
            public byte cpMinor;

            /// unsigned char
            public byte cpRev;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct C2K_NSFT_RSSI_CNF
        {

            /// unsigned char
            public byte ok;

            /// double
            public double rssiMain;

            /// double
            public double rssiRxd;

            /// unsigned short
            public ushort pnOffset;

            /// short
            public short strength;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct T_AST_RF_CAPABILITY_ITEM_SET
        {

            /// unsigned int
            public uint reserved;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct T_AST_RF_CALIBRATION_ITEM
        {

            /// is_capable : 1
            ///is_mandatory : 1
            ///parameter : 30
            public uint bitvector1;

            public uint is_capable
            {
                get
                {
                    return ((uint)((this.bitvector1 & 1u)));
                }
                set
                {
                    this.bitvector1 = ((uint)((value | this.bitvector1)));
                }
            }

            public uint is_mandatory
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 2u)
                                / 2)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 2)
                                | this.bitvector1)));
                }
            }

            public uint parameter
            {
                get
                {
                    return ((uint)(((this.bitvector1 & 4294967292u)
                                / 4)));
                }
                set
                {
                    this.bitvector1 = ((uint)(((value * 4)
                                | this.bitvector1)));
                }
            }
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct T_AST_RF_CALIBRATION_ITEM_SET
        {

            /// T_AST_RF_CALIBRATION_ITEM->_T_AST_RF_CALIBRATION_ITEM
            public T_AST_RF_CALIBRATION_ITEM tadc_cal;

            /// T_AST_RF_CALIBRATION_ITEM->_T_AST_RF_CALIBRATION_ITEM
            public T_AST_RF_CALIBRATION_ITEM multi_rat_tadc_bitmap;

            /// T_AST_RF_CALIBRATION_ITEM->_T_AST_RF_CALIBRATION_ITEM
            public T_AST_RF_CALIBRATION_ITEM multi_rat_afc_bitmap;

            /// T_AST_RF_CALIBRATION_ITEM->_T_AST_RF_CALIBRATION_ITEM
            public T_AST_RF_CALIBRATION_ITEM apc_cal;

            /// T_AST_RF_CALIBRATION_ITEM->_T_AST_RF_CALIBRATION_ITEM
            public T_AST_RF_CALIBRATION_ITEM temperature_info;

            /// T_AST_RF_CALIBRATION_ITEM->_T_AST_RF_CALIBRATION_ITEM
            public T_AST_RF_CALIBRATION_ITEM ex_temp_sensor_info;

            /// T_AST_RF_CALIBRATION_ITEM->_T_AST_RF_CALIBRATION_ITEM
            public T_AST_RF_CALIBRATION_ITEM ubin_info;

            /// T_AST_RF_CALIBRATION_ITEM->_T_AST_RF_CALIBRATION_ITEM
            public T_AST_RF_CALIBRATION_ITEM Nvram_ver_Access_Con;

            /// T_AST_RF_CALIBRATION_ITEM->_T_AST_RF_CALIBRATION_ITEM
            public T_AST_RF_CALIBRATION_ITEM elna_info;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct T_AST_RF_CAPABILITY_RESULT
        {

            /// unsigned int
            public uint capability_item_offset;

            /// unsigned int
            public uint calibration_item_offset;

            /// T_AST_RF_CAPABILITY_ITEM_SET->_T_AST_RF_CAPABILITY_ITEM_SET
            public T_AST_RF_CAPABILITY_ITEM_SET capabilityItems;

            /// T_AST_RF_CALIBRATION_ITEM_SET->_T_AST_RF_CALIBRATION_ITEM_SET
            public T_AST_RF_CALIBRATION_ITEM_SET calibrationItems;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct T_AST_RFCAL_UBIN_MODE
        {

            /// unsigned short
            public ushort ubin_tdd_mode_init;
        }
        
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct UL1D_RF_NSFT_REQ_T
        {
            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool b_afc_dac_valid;
            /// unsigned short
            public ushort u2_afc_dac;

            /// unsigned char
            public byte u1_loopbackType;

            /// unsigned char
            public byte u1_frame_shift;

            /// unsigned char
            public byte u1_rmc_type;

            /// unsigned char[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 4)]
            public string u1_ctfc;

            /// unsigned char
            public byte u1_bit_pattern;

            /// short
            public short i2_dl_freq;

            /// short
            public short i2_dl_psc;

            /// short
            public short i2_dl_ovsf;

            /// short
            public short i2_ul_freq;

            /// unsigned short
            public ushort u2_ul_tfci;

            /// unsigned int
            public uint u4_ul_sc_code;

            /// boolean
            public bool b_iq_pwr_valid;

            /// unsigned char
            public byte u1_dpcch_pwr;

            /// unsigned char
            public byte u1_dpdch_pwr;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestCmdRSSI
        {

            /// unsigned char
            public byte num_freq;

            /// unsigned short[]
            public ushort[] dl_freq;

            /// unsigned char
            public byte temperature;

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool hwAGC;

            /// unsigned char
            public byte mode;

            /// short
            public short gain;

            /// unsigned char
            public byte LNAmode;

            /// unsigned short
            public ushort pga;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestCmdLPMRSSI
        {

            /// unsigned char
            public byte num_freq;

            /// unsigned short[]
            public ushort[] dl_freq;

            /// unsigned char
            public byte temperature;

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool hwAGC;

            /// unsigned char
            public byte mode;

            /// short
            public short gain;

            /// unsigned char
            public byte LNAmode;

            /// unsigned short
            public ushort pga;

            /// unsigned char
            public byte lpm_mode;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestCmdELNARSSI
        {

            /// unsigned char
            public byte num_freq;

            /// unsigned short[]
            public ushort[] dl_freq;

            /// unsigned char
            public byte temperature;

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool hwAGC;

            /// unsigned char
            public byte mode;

            /// unsigned char
            public byte lpm_mode;

            /// short
            public short rx_gain;

            /// unsigned char
            public byte rx_LNAmode;

            /// unsigned short
            public ushort rx_pga;

            /// short
            public short rxd_gain;

            /// unsigned char
            public byte rxd_LNAmode;

            /// unsigned short
            public ushort rxd_pga;

            /// unsigned char
            public byte antenna_path;

            /// boolean
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
            public bool is_cal;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestCmdPwrToGainV3
        {

            /// unsigned char
            public byte rx_elna_bypass;

            /// unsigned char
            public byte rxd_elna_bypass;

            /// unsigned char
            public byte antenna_path;

            /// unsigned char
            public byte lpm_mode;

            /// unsigned char
            public byte rx_elna_mode;

            /// unsigned char
            public byte rxd_elna_mode;

            /// unsigned char
            public byte rx_cal_sequency;

            /// short
            public short rx_dl_power;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestCmdPwrToGainV5
        {

            /// unsigned char
            public byte rx_elna_bypass;

            /// unsigned char
            public byte rxd_elna_bypass;

            /// unsigned char
            public byte antenna_path;

            /// unsigned char
            public byte lpm_mode;

            /// unsigned char
            public byte rx_elna_mode;

            /// unsigned char
            public byte rxd_elna_mode;

            /// unsigned char
            public byte rx_cal_sequency;

            /// short
            public short rx_dl_power;

            /// unsigned char
            public byte rx_gain_table;

            /// unsigned char
            public byte rxd_gain_table;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct URfTestCmdGetMdCalInfoV3
        {

            /// unsigned char
            public byte cal_band_number;

            /// unsigned char[5]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 5)]
            public string cal_band;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct URfCalBandInfoV3
        {

            /// unsigned char
            public byte band;

            /// unsigned char
            public byte rx_elna_mode;

            /// unsigned char
            public byte rxd_elna_mode;

            /// unsigned char
            public byte cal_point_hpm;

            /// unsigned char
            public byte cal_point_lpm;

            /// unsigned char[11]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 11)]
            public string rx_cal_sequency;

            /// unsigned char[11]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 11)]
            public string rx_elna_bypass;

            /// unsigned char[11]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 11)]
            public string rxd_elna_bypass;

            /// short[11]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 11, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rx_cal_dl_power_min;

            /// short[11]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 11, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rx_cal_dl_power_max;

            /// short[11]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 11, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rx_cal_dl_power;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultGetMdCalInfoV3
        {

            /// unsigned short
            public ushort support_band_number;

            /// URfCalBandInfoV3[5]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public URfCalBandInfoV3[] cal_band_info_v3;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct URfTestCmdGetMdCalInfoV5
        {

            /// unsigned char
            public byte cal_band_number;

            /// unsigned char[8]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 8)]
            public string cal_band;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct URfCalBandInfoV5
        {

            /// unsigned char
            public byte band;

            /// unsigned char
            public byte rx_elna_mode;

            /// unsigned char
            public byte rxd_elna_mode;

            /// unsigned char
            public byte cal_point_hpm;

            /// unsigned char
            public byte cal_point_lpm;

            /// unsigned char[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 14)]
            public string rx_cal_sequency;

            /// unsigned char[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 14)]
            public string rx_elna_bypass;

            /// unsigned char[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 14)]
            public string rxd_elna_bypass;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rx_cal_dl_power_min;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rx_cal_dl_power_max;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rx_cal_dl_power;

            /// unsigned char[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 14)]
            public string rx_lna_mode;

            /// unsigned char[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 14)]
            public string rxd_lna_mode;

            /// unsigned char[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 14)]
            public string rx_location_idx;

            /// unsigned char[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 14)]
            public string rxd_location_idx;

            /// unsigned char
            public byte rx_gain_table;

            /// unsigned char
            public byte rxd_gain_table;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultGetMdCalInfoV5
        {

            /// unsigned short
            public ushort support_band_number;

            /// URfCalBandInfoV5[8]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public URfCalBandInfoV5[] cal_band_info_v5;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct URfTestCmdGetMdCalInfoV7
        {

            /// unsigned char
            public byte cal_band_number;

            /// unsigned char[8]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 8)]
            public string cal_band;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct URfCalBandInfoV7
        {

            /// unsigned char
            public byte band;

            /// unsigned char
            public byte cal_point_hpm;

            /// unsigned char
            public byte cal_point_lpm;

            /// unsigned char
            public byte cal_point_tkm;

            /// unsigned char[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 14)]
            public string rx_cal_sequency;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rx_cal_dl_power_min;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rx_cal_dl_power_max;

            /// short[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rx_cal_dl_power;

            /// unsigned char[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 14)]
            public string rx_lna_mode;

            /// unsigned char[14]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 14)]
            public string rx_location_idx;

            /// unsigned char[24]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 24)]
            public string rxloss_usage;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct URfTestResultGetMdCalInfoV7
        {

            /// unsigned short
            public ushort support_band_number;

            /// URfCalBandInfoV7[8]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public URfCalBandInfoV7[] cal_band_info_v7;
        }


        public partial class LTE_Const_V2
        {

            /// ERF_TX_TEST_COMMAND_CONFIG_TDD -> 0
            public const int ERF_TX_TEST_COMMAND_CONFIG_TDD = 0;

            /// ERF_TX_TEST_COMMAND_CONFIG_FDD -> 1
            public const int ERF_TX_TEST_COMMAND_CONFIG_FDD = 1;

            /// ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_6RB -> 0
            public const int ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_6RB = 0;

            /// ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_15RB -> 1
            public const int ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_15RB = 1;

            /// ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_25RB -> 2
            public const int ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_25RB = 2;

            /// ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_50RB -> 3
            public const int ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_50RB = 3;

            /// ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_75RB -> 4
            public const int ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_75RB = 4;

            /// ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_100RB -> 5
            public const int ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_100RB = 5;

            /// ERF_TX_TEST_COMMAND_CONFIG_PUSCH_MSC_QPSK -> 0
            public const int ERF_TX_TEST_COMMAND_CONFIG_PUSCH_MSC_QPSK = 0;

            /// ERF_TX_TEST_COMMAND_CONFIG_PUSCH_MSC_16QAM -> 1
            public const int ERF_TX_TEST_COMMAND_CONFIG_PUSCH_MSC_16QAM = 1;

            /// ERF_TX_TEST_COMMAND_CONFIG_PUSCH_MSC_64QAM -> 2
            public const int ERF_TX_TEST_COMMAND_CONFIG_PUSCH_MSC_64QAM = 2;

            /// ERF_TX_TEST_COMMAND_CONFIG_PA_MODE_HIGH -> 0
            public const int ERF_TX_TEST_COMMAND_CONFIG_PA_MODE_HIGH = 0;

            /// ERF_TX_TEST_COMMAND_CONFIG_PA_MODE_MIDDLE -> 1
            public const int ERF_TX_TEST_COMMAND_CONFIG_PA_MODE_MIDDLE = 1;

            /// ERF_TX_TEST_COMMAND_CONFIG_PA_MODE_LOW -> 2
            public const int ERF_TX_TEST_COMMAND_CONFIG_PA_MODE_LOW = 2;

            /// ERF_TX_TEST_COMMAND_CONFIG_BYPASS_NONCAA_MODE -> 0
            public const int ERF_TX_TEST_COMMAND_CONFIG_BYPASS_NONCAA_MODE = 0;

            /// ERF_TX_TEST_COMMAND_CONFIG_FILTER_NONCAA_MODE -> 1
            public const int ERF_TX_TEST_COMMAND_CONFIG_FILTER_NONCAA_MODE = 1;

            /// ERF_TX_TEST_COMMAND_CONFIG_BYPASS_CAA_MODE -> 2
            public const int ERF_TX_TEST_COMMAND_CONFIG_BYPASS_CAA_MODE = 2;

            /// ERF_TX_TEST_COMMAND_CONFIG_FILTER_CAA_MODE -> 3
            public const int ERF_TX_TEST_COMMAND_CONFIG_FILTER_CAA_MODE = 3;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestCmdPuschTxV2Param
        {

            /// unsigned char
            public byte band;

            /// unsigned char
            public byte duplexMode;

            /// unsigned char
            public byte tddConfig;

            /// unsigned char
            public byte tddSfConfig;

            /// unsigned short
            public ushort ulFrequency;

            /// unsigned short
            public ushort cellId;

            /// unsigned char
            public byte ulBandwidth;

            /// unsigned char
            public byte vrbStart;

            /// unsigned char
            public byte vrbLength;

            /// unsigned char
            public byte mcsMode;

            /// unsigned char
            public byte txPowerMode;

            /// short
            public short txPowerValue;

            /// unsigned char
            public byte networkSelection;

            /// unsigned char
            public byte txCloseLoopDisbl;

            /// unsigned char
            public byte amprEnbl;

            /// short
            public short bbBackoff;

            /// unsigned char
            public byte rfGain;

            /// unsigned char
            public byte paMode;

            /// unsigned char
            public byte paVcc;

            /// unsigned char
            public byte vm0;

            /// unsigned char
            public byte vm1;

            /// unsigned short
            public ushort txRoute;

            /// unsigned char
            public byte enableMultiCluster;

            /// unsigned char
            public byte multiClusterVrbStart;

            /// unsigned char
            public byte multiClustervrbLength;

            /// unsigned char
            public byte enableCsr;

            /// unsigned short
            public ushort dlFrequency;

            /// unsigned short
            public ushort txPathSelBitmap;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestCmdPuschTxV2
        {

            /// unsigned char
            public byte ulCCNum;

            /// short
            public short afcdac;

            /// ERfTestCmdPuschTxV2Param[]
            public ERfTestCmdPuschTxV2Param[] puschTxParam;
        }


        public partial class NativeConstants
        {

            /// ERF_TX_TEST_COMMAND_CONFIG_TDD -> 0
            public const int ERF_TX_TEST_COMMAND_CONFIG_TDD = 0;

            /// ERF_TX_TEST_COMMAND_CONFIG_FDD -> 1
            public const int ERF_TX_TEST_COMMAND_CONFIG_FDD = 1;

            /// ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_6RB -> 0
            public const int ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_6RB = 0;

            /// ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_15RB -> 1
            public const int ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_15RB = 1;

            /// ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_25RB -> 2
            public const int ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_25RB = 2;

            /// ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_50RB -> 3
            public const int ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_50RB = 3;

            /// ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_75RB -> 4
            public const int ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_75RB = 4;

            /// ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_100RB -> 5
            public const int ERF_TX_TEST_COMMAND_CONFIG_UL_BANDWIDTH_100RB = 5;

            /// ERF_TX_TEST_COMMAND_CONFIG_PUSCH_MSC_QPSK -> 0
            public const int ERF_TX_TEST_COMMAND_CONFIG_PUSCH_MSC_QPSK = 0;

            /// ERF_TX_TEST_COMMAND_CONFIG_PUSCH_MSC_16QAM -> 1
            public const int ERF_TX_TEST_COMMAND_CONFIG_PUSCH_MSC_16QAM = 1;

            /// ERF_TX_TEST_COMMAND_CONFIG_PUSCH_MSC_64QAM -> 2
            public const int ERF_TX_TEST_COMMAND_CONFIG_PUSCH_MSC_64QAM = 2;

            /// ERF_TX_TEST_COMMAND_CONFIG_PA_MODE_HIGH -> 0
            public const int ERF_TX_TEST_COMMAND_CONFIG_PA_MODE_HIGH = 0;

            /// ERF_TX_TEST_COMMAND_CONFIG_PA_MODE_MIDDLE -> 1
            public const int ERF_TX_TEST_COMMAND_CONFIG_PA_MODE_MIDDLE = 1;

            /// ERF_TX_TEST_COMMAND_CONFIG_PA_MODE_LOW -> 2
            public const int ERF_TX_TEST_COMMAND_CONFIG_PA_MODE_LOW = 2;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestCmdPuschTx
        {

            /// unsigned char
            public byte band;

            /// unsigned char
            public byte duplexMode;

            /// unsigned char
            public byte tddConfig;

            /// unsigned char
            public byte tddSfConfig;

            /// unsigned short
            public ushort ulFrequency;

            /// unsigned short
            public ushort cellId;

            /// short
            public short afcdac;

            /// unsigned char
            public byte ulBandwidth;

            /// unsigned char
            public byte vrbStart;

            /// unsigned char
            public byte vrbLength;

            /// unsigned char
            public byte mcsMode;

            /// short
            public short txPower;

            /// unsigned char
            public byte networkSelection;

            /// unsigned char
            public byte txPowerMode;

            /// unsigned char
            public byte disableCloseLoop;

            /// unsigned char
            public byte enableAmpr;

            /// short
            public short bbBackoff;

            /// unsigned char
            public byte rfGain;

            /// unsigned char
            public byte paMode;

            /// unsigned char
            public byte paVcc;

            /// unsigned char
            public byte vm0;

            /// unsigned char
            public byte vm1;

            /// unsigned char
            public byte enableCsr;

            /// unsigned short
            public ushort dlFrequency;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestCmdSetRxCommonCfg
        {

            /// unsigned char
            public byte rf_band;

            /// unsigned char
            public byte vco_lna_port_cfg_index;

            /// unsigned char
            public byte is_lna_low_power_mode;
        }


        public partial class NativeConstants
        {

            /// MAX_ERF_TEST_MIXRX_DLRX_MODE -> (0)
            public const int MAX_ERF_TEST_MIXRX_DLRX_MODE = 0;

            /// MAX_ERF_TEST_MIXRX_TXRX_MODE -> (1)
            public const int MAX_ERF_TEST_MIXRX_TXRX_MODE = 1;

            /// MAX_ERF_TEST_MIXRX_PSEUDO_TXRX_MODE -> (2)
            public const int MAX_ERF_TEST_MIXRX_PSEUDO_TXRX_MODE = 2;

            /// MAX_ERF_TEST_MIXRX_REPORT_COUNT -> 10
            public const int MAX_ERF_TEST_MIXRX_REPORT_COUNT = 10;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestCmdMixRx
        {

            /// unsigned char
            public byte mode;

            /// unsigned char
            public byte measCnt;

            /// unsigned char
            public byte measBandwidth;

            /// short
            public short afcdac;

            /// unsigned short
            public ushort dlFrequency;

            /// unsigned char
            public byte band;

            /// unsigned char
            public byte duplexMode;

            /// unsigned char
            public byte tddConfig;

            /// unsigned char
            public byte tddSfConfig;

            /// unsigned char
            public byte bandwidth;

            /// unsigned char
            public byte rxPath0Enbl;

            /// unsigned char
            public byte rxPath1Enbl;

            /// unsigned short
            public ushort ulFrequency;

            /// short
            public short txPowerValue;

            /// unsigned char
            public byte vrbStart;

            /// unsigned char
            public byte vrbLength;

            /// unsigned char
            public byte mcsMode;
        }

        
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ERfTestCmdGetMixRxRpt
        {

            /// unsigned char
            public byte measRptCnt;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rssiPath0;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rssiPath1;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrpPath0;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrpPath1;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrqPath0;

            /// short[10]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] rsrqPath1;

            /// unsigned int
            public uint crcOK_cnt;

            /// unsigned int
            public uint crcNG_cnt;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FT_NVRAM_READ_REQ
        {

            /// char*
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)]
            public string LID;

            /// unsigned short
            public ushort RID;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FT_NVRAM_READ_CNF
        {

            /// unsigned short
            public ushort LID;

            /// unsigned short
            public ushort RID;

            /// unsigned char
            public byte status;

            /// unsigned int
            public uint len;

            /// unsigned char*
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)]
            public string buf;
        }



        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FT_NVRAM_WRITE_REQ
        {

            /// char*
            [MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)]
            public string LID;

            /// unsigned short
            public ushort RID;

            /// unsigned int
            public uint len;

            /// unsigned char*
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)]
            public string buf;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FT_NVRAM_WRITE_CNF
        {

            /// unsigned short
            public ushort LID;

            /// unsigned short
            public ushort RID;

            /// unsigned char
            public byte status;
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct WIFI_RX_BANDINFO_AX
        {

            /// UINT32->unsigned int
            public UInt32 mac_rx_fcs_err_cnt;

            /// UINT32->unsigned int
            public UInt32 mac_rx_mdrdy_cnt;

            /// UINT32->unsigned int
            public UInt32 mac_rx_len_mismatch;

            /// UINT32->unsigned int
            public UInt32 mac_rx_fcs_ok_cnt;

            /// UINT32->unsigned int
            public UInt32 phy_rx_fcs_err_cnt_cck;

            /// UINT32->unsigned int
            public UInt32 phy_rx_fcs_err_cnt_ofdm;

            /// UINT32->unsigned int
            public UInt32 phy_rx_pd_cck;

            /// UINT32->unsigned int
            public UInt32 phy_rx_pd_ofdm;

            /// UINT32->unsigned int
            public UInt32 phy_rx_sig_err_cck;

            /// UINT32->unsigned int
            public UInt32 phy_rx_sfd_err_cck;

            /// UINT32->unsigned int
            public UInt32 phy_rx_sig_err_ofdm;

            /// UINT32->unsigned int
            public UInt32 phy_rx_tag_err_ofdm;

            /// UINT32->unsigned int
            public UInt32 phy_rx_mdrdy_cnt_cck;

            /// UINT32->unsigned int
            public UInt32 phy_rx_mdrdy_cnt_ofdm;

            ///// UINT32->unsigned int
            //public UInt32 DLL_AllLengthMismatchCount;

            ///// UINT32->unsigned int
            //public UInt32 DLL_AllMacMdrdy;

            ///// UINT32->unsigned int
            //public UInt32 DLL_AllFCSErr;

            ///// UINT32->unsigned int
            //public UInt32 DLL_RXOK;

            ///// UINT32->unsigned int
            //public UInt32 DLL_AllFCSPass;

            ///// UINT32->unsigned int
            //public Single DLL_PER;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct WIFI_RX_ANTINFO_AX
        {

            /// UINT32->unsigned int
            public UInt32 rcpi;

            /// UINT32->unsigned int
            public UInt32 rssi;

            /// UINT32->unsigned int
            public UInt32 fagc_ib_rssi;

            /// UINT32->unsigned int
            public UInt32 fagc_wb_rssi;

            /// UINT32->unsigned int
            public UInt32 inst_ib_rssi;

            /// UINT32->unsigned int
            public UInt32 inst_wb_rssi;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct WIFI_RX_USERINFO_AX
        {

            /// INT32->int
            public Int32 freq_offset_from_rx;

            /// UINT32->unsigned int
            public UInt32 snr;

            /// UINT32->unsigned int
            public UInt32 fcs_error_cnt;

            public float DLL_PER;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct WIFI_RX_COMMONINFO_AX
        {

            /// UINT32->unsigned int
            public UInt32 rx_fifo_full;

            /// UINT32->unsigned int
            public UInt32 aci_hit_low;

            /// UINT32->unsigned int
            public UInt32 aci_hit_high;

            /// UINT32->unsigned int
            public UInt32 mu_pkt_count;

            /// UINT32->unsigned int
            public UInt32 sig_mcs;

            /// UINT32->unsigned int
            public UInt32 sinr;

            /// UINT32->unsigned int
            public UInt32 driver_rx_count;
        }
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct WIFI_RX_ACCINFO
        {
            /*
            UINT32 DLL_AllLengthMismatchCount;
            UINT32 DLL_AllMacMdrdy;
            UINT32 DLL_AllFCSErr;
            UINT32 DLL_RXOK;
            UINT32 DLL_PreRXOK;
            UINT32 DLL_AllFCSPass;
            float DLL_PER;
             */
            public UInt32 DLL_AllLengthMismatchCount;
            public UInt32 DLL_AllMacMdrdy;
            public UInt32 DLL_AllFCSErr;
            public UInt32 DLL_RXOK;
            public UInt32 DLL_PreRXOK;
            public UInt32 DLL_AllFCSPass;
            public float DLL_PER;
        }
        
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct WIFI_RX_STASTIC_AX
        {
            /// RX_BANDINFO_AX[1]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public WIFI_RX_BANDINFO_AX[] BandInfo;

            /// RX_ANTINFO_AX[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public WIFI_RX_ANTINFO_AX[] AntInfo;

            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public WIFI_RX_USERINFO_AX[] UsrInfo;

            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public WIFI_RX_COMMONINFO_AX[] CommonInfo;

            public WIFI_RX_ACCINFO AccInfo;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct RUSettings
        {
            /// UINT32->unsigned int
            public UInt32 Seg0Cat;

            /// UINT32->unsigned int
            public UInt32 Seg0Alloc;

            /// UINT32->unsigned int
            public UInt32 Seg0StalD;

            /// UINT32->unsigned int
            public UInt32 Seg0RUIdx;

            /// UINT32->unsigned int
            public UInt32 Seg0Rate;

            /// UINT32->unsigned int
            public UInt32 Seg0LDPC;

            /// UINT32->unsigned int
            public UInt32 Seg0Nss;

            /// UINT32->unsigned int
            public UInt32 Seg0Stream;

            /// UINT32->unsigned int
            public UInt32 Seg0Length;

            /// UINT32->unsigned int
            public UInt32 Seg0TxPwr;

            /// UINT32->unsigned int
            public UInt32 Seg0MUNss;
        }
    }
}
