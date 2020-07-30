using EtherCATSeries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IMICNC_WPF
{

    public struct Axis<T>
    {
        public T X, Y, Z, U, V, W, A, B;
        //public T X { get; set; }
        //public T Y { get; set; }
        //public T Z { get; set; }

        public static TDestination Convert<TSource, TDestination>(TSource value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(TSource));

            TDestination result = default(TDestination);

            // 判斷能不能轉型
            if (converter.CanConvertTo(typeof(TDestination)))
            {
                result = (TDestination)(converter.ConvertTo(value, typeof(TDestination)));
            }
            return result;
        }
    }

    static class MCCLManager
    {
        // Coordinate System Variable
        public static Axis<double> machanicalCoordinate = new Axis<double>();
        public static Axis<double> absoluteCoordinate = new Axis<double>();
        public static Axis<double> relativeCoordinate = new Axis<double>();

        public static bool isToolHolder = false;
        public static bool isSpindleOn = false;
        public static double rpm;
        //public static Axis<double> machasbSub = new Axis<double>();
        //private static List<Axis<double>> QQ = new List<Axis<double>>();
        //public static CNCParameter[] xx = new CNCParameter[1];
        //public static Axis<double> relativeSub = new Axis<double>();

        // MCCL Get Data Variable
        public static Axis<double> CurRefPos = new Axis<double>();
        public static Axis<double> CurPos = new Axis<double>();
        public static Axis<double> speed = new Axis<double>();
        public static Axis<int> ENCValue = new Axis<int>();
        public static Axis<int> PulsePos = new Axis<int>();
        public static Axis<ushort> PositiveLimitSwitchStatus = new Axis<ushort>();
        public static Axis<ushort> NegativeLimitSwitchStatus = new Axis<ushort>();
        public static Axis<ushort> HomeSensorStatus = new Axis<ushort>();
        public static double CurFeedSpeed;
        public static ushort EmgcStopStatus = 0;
        public static string Error;

        /// <summary>
        /// 這函式是使用非同步方式檢查 MCCLManager 內，所有的 MCCL Function 回傳值
        /// </summary>
        /// <param name="returnValue"></param>

        #region 控制器
        public static void ConnectEMPZ()
        {
            string ip = "192.168.0.2";
            byte[] tmp = Encoding.ASCII.GetBytes(ip);
            int cardInx = 0;
            int QQ = MCCL.MCC_Init(ref tmp[0], ref cardInx, 8);
            if (QQ < 0)
            {
                MessageBox.Show("Connect Error!");
                return;
            }
            else
            {
                MessageBox.Show(tmp[0].ToString());
            }
        }

        public static void DisConnectEMPZ()
        {
            try { MCCL.MCC_Close(); }
            catch { }
        }

        public static void SetMachineParameter()
        {
            // MotorBox
            /*
            //MCCL使用所需物件
            MCCL.SYS_MAC_PARAM stMacParam = new MCCL.SYS_MAC_PARAM();
            MCCL.SYS_ENCODER_CONFIG stENCConfig = new MCCL.SYS_ENCODER_CONFIG();
            MCCL.SYS_HOME_CONFIG stHomeConfig = new MCCL.SYS_HOME_CONFIG();

            //MCCL物件參數初始化_機構參數
            stMacParam.wPosToEncoderDir = 0;
            stMacParam.dwPPR = 8192;
            stMacParam.wRPM = 6000;
            stMacParam.dfPitch = 1.0;
            stMacParam.dfGearRatio = 1.0;
            stMacParam.dfHighLimit = 50000.0;
            stMacParam.dfLowLimit = -50000.0;
            stMacParam.dfHighLimitOffset = 0;
            stMacParam.dfLowLimitOffset = 0;
            stMacParam.wPulseMode = MCCL.DDA_FMT_PD;
            stMacParam.wPulseWidth = 100;
            stMacParam.wCommandMode = 0;
            stMacParam.wOverTravelUpSensorMode = 0;
            stMacParam.wOverTravelDownSensorMode = 0;

            //MCCL物件參數初始化_編碼器參數
            stENCConfig.wType = MCCL.ENC_TYPE_AB;
            stENCConfig.wAInverse = MCCL._NO_;
            stENCConfig.wBInverse = MCCL._NO_;
            stENCConfig.wCInverse = MCCL._NO_;
            stENCConfig.wABSwap = MCCL._NO_;
            stENCConfig.wInputRate = 4;
            //set encoder input rate : x4

            //MCCL物件參數初始化_原點復歸參數
            stHomeConfig.wMode = 3;
            stHomeConfig.wDirection = 1;
            stHomeConfig.wSensorMode = MCCL.SL_NORMAL_CLOSE;
            stHomeConfig.nIndexCount = 2;
            stHomeConfig.dfAccTime = 300;
            stHomeConfig.dfDecTime = 300;
            stHomeConfig.dfHighSpeed = 10;
            stHomeConfig.dfLowSpeed = 2;
            stHomeConfig.dfOffset = 0;
            stHomeConfig.wPaddel0 = 0;
            stHomeConfig.nPaddel1 = 0;
            */
            //---------------------------------------------------------------------------------------------
            // CNC

            MCCL.SYS_MAC_PARAM stMacParam = new MCCL.SYS_MAC_PARAM();
            MCCL.SYS_ENCODER_CONFIG stENCConfig = new MCCL.SYS_ENCODER_CONFIG();
            MCCL.SYS_HOME_CONFIG stHomeConfig = new MCCL.SYS_HOME_CONFIG();

            //MCCL物件參數初始化_機構參數
            stMacParam.wPosToEncoderDir = 0;
            stMacParam.dwPPR = 1048576;
            stMacParam.wRPM = 3000;
            stMacParam.dfPitch = 8.23;
            stMacParam.dfGearRatio = 1.0;
            stMacParam.dfHighLimit = 50000.0;
            stMacParam.dfLowLimit = -50000.0;
            stMacParam.dfHighLimitOffset = 0;
            stMacParam.dfLowLimitOffset = 0;
            stMacParam.wPulseMode = MCCL.DDA_FMT_PD;
            stMacParam.wPulseWidth = 100;
            stMacParam.wCommandMode = 0;
            stMacParam.wOverTravelUpSensorMode = 0;
            stMacParam.wOverTravelDownSensorMode = 0;

            //MCCL物件參數初始化_編碼器參數
            stENCConfig.wType = MCCL.ENC_TYPE_AB;
            stENCConfig.wAInverse = MCCL._NO_;
            stENCConfig.wBInverse = MCCL._NO_;
            stENCConfig.wCInverse = MCCL._NO_;
            stENCConfig.wABSwap = MCCL._NO_;
            stENCConfig.wInputRate = 4;
            //set encoder input rate : x4

            //MCCL物件參數初始化_原點復歸參數
            stHomeConfig.wMode = 3;
            stHomeConfig.wDirection = 1;
            stHomeConfig.wSensorMode = MCCL.SL_NORMAL_CLOSE;
            stHomeConfig.nIndexCount = 2;
            stHomeConfig.dfAccTime = 300;
            stHomeConfig.dfDecTime = 300;
            stHomeConfig.dfHighSpeed = 0.1;
            stHomeConfig.dfLowSpeed = 0.0001;
            stHomeConfig.dfOffset = 0;
            stHomeConfig.wPaddel0 = 0;
            stHomeConfig.nPaddel1 = 0;

            for (ushort wChannel = 0; wChannel <= 7; wChannel++)
            {
                MCCL.MCC_SetMacParam(ref stMacParam, wChannel, 0);
                //MCCL.MCC_SetEncoderConfig(ref stENCConfig, wChannel, 0);
                //MCCL.MCC_SetHomeConfig(ref stHomeConfig, wChannel, 0);
            }

            // 設定 Group
            MCCL.MCC_CloseAllGroups();
            nGroupIndex = (ushort)MCCL.MCC_CreateGroup(0, 1, 2, -1, -1, -1, -1, -1, MCCL.CARDINDEX0);

            if (nGroupIndex < 0)
            {
                MessageBox.Show("Create GroupIndex Fail");
                MCCL.MCC_CloseAllGroups();
                return;
            }

            //設定最大速度 
            MCCL.MCC_SetSysMaxSpeed(100); // set max. feed rate

        }

        const int MAX_NUM_OF_GROUP = 1;
        //private int MAX_NUM_OF_DRIVERS;
        private static ushort nGroupIndex = 0;
        public static void StartEMPZ()
        {
            //設定使用Card
            MCCL.SYS_CARD_CONFIG stCardConfig = new MCCL.SYS_CARD_CONFIG();
            stCardConfig.wCardType = 4;
            MCCL.ErrorCode eRet;
            eRet = MCCL.MCC_InitSystem(1, ref stCardConfig, 1);
            if (eRet < 0)
            {
                MessageBox.Show("MCC_InitSystem Error :" + eRet.ToString());
                return;
            }
            else
            {
                // CmdQueueSize設定
                MCCL.MCC_SetCmdQueueSize(50000, 0);
                int sixe = MCCL.MCC_GetCmdQueueSize(0);
                MessageBox.Show("" + sixe.ToString());

                //座標軸設定
                //MCCL.MCC_SetIncrease(nGroupIndex);    //相對
                MCCL.MCC_SetAbsolute(nGroupIndex);  //絕對
                int eerocode = MCCL.MCC_GetCoordType(nGroupIndex);
                if (eerocode == 0)
                {
                    MessageBox.Show("IncreaseOpen");
                }
                else if (eerocode == 1)
                {
                    MessageBox.Show("AbsoluteOpen");
                }

                //DryRun設定  DryRun可
                //MCCL.MCC_EnableDryRun();
                MCCL.MCC_DisableDryRun();
                eerocode = MCCL.MCC_CheckDryRun();
                if (eerocode == 0)
                {
                    MessageBox.Show("dryrunopen");
                }
                else if (eerocode == 1)
                {
                    MessageBox.Show("dryrunclose");
                }

                //其他設定
                //MCCL.MCC_SetUnit(MCCL.UNIT_MM, 0); // coordinate unit : mm

                // 4.過形成保護
                //MCCL.MCC_EnableLimitSwitchCheck();
                //MCCL.MCC_DisableLimitSwitchCheck();
                //MCCL.MCC_SetOverTravelCheck();
                //MCCL.MCC_GetOverTravelCheck();

                //硬體極限
                //MCCL.MCC_EnableLimitSwitchCheck(0);
                //開關平滑模式
                MCCL.MCC_EnableBlend(nGroupIndex);
                //MCCL.MCC_DisableBlend(nGroupIndex);
                MCCL.MCC_CheckBlend(nGroupIndex);

                // set line, arc and circle motion's accleration time
                MCCL.MCC_SetAccType('S', nGroupIndex); //ST兩種模式而已
                MCCL.MCC_GetAccType(nGroupIndex);
                MCCL.MCC_SetAccTime(200, nGroupIndex); // set accleration time to be 300 ms
                MCCL.MCC_GetAccTime(nGroupIndex);
                // set line, arc and circle motion's deceleration time
                MCCL.MCC_SetDecType('S', nGroupIndex);
                MCCL.MCC_GetDecType(0);
                MCCL.MCC_SetDecTime(200, nGroupIndex); // set decleration time to be 300 ms
                MCCL.MCC_GetDecTime(nGroupIndex);

                MCCL.MCC_SetFeedSpeed(10, nGroupIndex); //  set line, arc and circle motion's feed rate (unit : mm/sec)

                MCCL.MCC_SetPtPAccType('S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', nGroupIndex);
                //MCCL.MCC_GetPtPAccType();
                //MCCL.MCC_SetPtPAccTime();
                //MCCL.MCC_GetPtPAccTime();
                MCCL.MCC_SetPtPDecType('S', 'S', 'S', 'S', 'S', 'S', 'S', 'S', nGroupIndex);
                //MCCL.MCC_GetPtPDecType();
                //MCCL.MCC_SetPtPDecTime();
                //MCCL.MCC_GetPtPDecTime();

                MCCL.MCC_SetPtPSpeed(10, nGroupIndex); //  set line, arc and circle motion's feed rate (unit : mm/sec)
                MCCL.MCC_GetPtPSpeed(nGroupIndex);

                //MCCL.MCC_SetPGain(60, 60, 60, 60, 60, 60, 60, 60, 0); // set appropriate P Gain

            }
        }

        public static void EndEMPZ()
        {
            MCCL.MCC_CloseAllGroups();
            MCCL.MCC_CloseSystem();
        }

        public static void ServoOn()
        {
            for (ushort i = 0; i <= 7; i++)
            {
                MCCL.MCC_SetServoOn(i, 0);
            }
        }

        public static void ServoOff()
        {
            for (ushort i = 0; i <= 7; i++)
            {
                MCCL.MCC_SetServoOff(i, 0);
            }
        }
        #endregion

        #region 主軸相關
        /// <summary>
        /// 主軸夾刀
        /// </summary>
        public static void ClampTool()
        {
            short setValue = 1;
            uint Size = 2;
            IntPtr nTargetValue = Marshal.AllocHGlobal(2);
            Marshal.WriteInt16(nTargetValue, setValue);
            MCCL.MCC_EcatCoeSdoDownload(0, 0x2213, 0, nTargetValue, Size);
        }

        /// <summary>
        /// 主軸鬆刀
        /// </summary>
        public static void UnClampTool()
        {
            short setValue = 0;
            uint Size = 2;
            IntPtr nTargetValue = Marshal.AllocHGlobal(2);
            Marshal.WriteInt16(nTargetValue, setValue);
            MCCL.MCC_EcatCoeSdoDownload(0, 0x2213, 0, nTargetValue, Size);
        }

        /// <summary>
        /// 主軸ON
        /// </summary>
        public static void SpindleOn()
        {
            short setValue = 0;
            uint Size = 2;
            IntPtr nTargetValue = Marshal.AllocHGlobal(2);
            Marshal.WriteInt16(nTargetValue, setValue);
            MCCL.MCC_EcatCoeSdoDownload(0, 0x2215, 0, nTargetValue, Size);
            isSpindleOn = true;
        }

        /// <summary>
        /// 主軸OFF
        /// </summary>
        public static void SpindleOff()
        {
            short setValue = 1;
            uint Size = 2;
            IntPtr nTargetValue = Marshal.AllocHGlobal(2);
            Marshal.WriteInt16(nTargetValue, setValue);
            MCCL.MCC_EcatCoeSdoDownload(0, 0x2215, 0, nTargetValue, Size);
            isSpindleOn = false;
        }

        /// <summary>
        /// 主軸轉速改變
        /// </summary>
        /// <param name="num">改變參數</param>
        public static void ToolRPMChange(float num)
        {
            //0~10?
            MCCL.MCC_EcatSetDacOutputValue(4, 1, num);
        }
        #endregion

        public static void JogSpace(sbyte axis, sbyte direction)
        {
            MCCL.MCC_JogSpace(direction, 10, (char)axis, 0);
        }

        public static void JogConti(sbyte axis, sbyte direction)
        {
            MCCL.MCC_JogConti(direction, 10, (char)axis, 0);
        }

        public static void MCC_AbortMotionEx()
        {
            MCCL.MCC_AbortMotionEx(200, 0);
        }

        public static void MCC_GoHome()
        {
            //只有XYZ需GOHOME
            MCCL.MCC_EcatSetHomeAxis(1, 1, 1, 0, 0, 0, 0, 0);

            for (int i = 0; i < 8; i++)
            {
                MCCL.MCC_EcatSetHomeMode(28, i);
                MCCL.MCC_EcatSetHomeSwitchSpeed(1000, i);
                MCCL.MCC_EcatSetHomeZeroSpeed(50, i);
            }

            MCCL.MCC_EcatHome();
        }

        public static void MCC_DefineMachining()
        {
            MCCL.MCC_DefineOrigin(0, nGroupIndex);
            MCCL.MCC_DefineOrigin(1, nGroupIndex);
            MCCL.MCC_DefineOrigin(2, nGroupIndex);
        }

        /// <summary>
        /// 從MCCL獲取資訊
        /// </summary>
        /// <returns></returns>
        public static void GetMCCLData()
        {
            MCCL.MCC_GetCurRefPos(ref CurRefPos.X, ref CurRefPos.Y, ref CurRefPos.Z,
                                          ref CurRefPos.U, ref CurRefPos.V, ref CurRefPos.W,
                                          ref CurRefPos.A, ref CurRefPos.B, 0);

            MCCL.MCC_GetCurPos(ref CurPos.X, ref CurPos.Y, ref CurPos.Z,
                               ref CurPos.U, ref CurPos.V, ref CurPos.W,
                               ref CurPos.A, ref CurPos.B, 0);

            MCCL.MCC_GetSpeed(ref speed.X, ref speed.Y, ref speed.Z,
                              ref speed.U, ref speed.V, ref speed.W,
                              ref speed.A, ref speed.B, 0);


            MCCL.MCC_GetENCValue(ref ENCValue.X, 0, MCCL.CARDINDEX0);
            MCCL.MCC_GetENCValue(ref ENCValue.Y, 1, MCCL.CARDINDEX0);
            MCCL.MCC_GetENCValue(ref ENCValue.Z, 2, MCCL.CARDINDEX0);
            MCCL.MCC_GetENCValue(ref ENCValue.U, 3, MCCL.CARDINDEX0);
            MCCL.MCC_GetENCValue(ref ENCValue.V, 4, MCCL.CARDINDEX0);
            MCCL.MCC_GetENCValue(ref ENCValue.W, 5, MCCL.CARDINDEX0);
            MCCL.MCC_GetENCValue(ref ENCValue.A, 6, MCCL.CARDINDEX0);
            MCCL.MCC_GetENCValue(ref ENCValue.B, 7, MCCL.CARDINDEX0);

            MCCL.MCC_GetPulsePos(ref PulsePos.X, ref PulsePos.Y, ref PulsePos.Z,
                                 ref PulsePos.U, ref PulsePos.V, ref PulsePos.W,
                                 ref PulsePos.A, ref PulsePos.B, 0);

            MCCL.MCC_GetLimitSwitchStatus(ref PositiveLimitSwitchStatus.X, 0, 0, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref PositiveLimitSwitchStatus.Y, 0, 1, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref PositiveLimitSwitchStatus.Z, 0, 2, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref PositiveLimitSwitchStatus.U, 0, 3, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref PositiveLimitSwitchStatus.V, 0, 4, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref PositiveLimitSwitchStatus.W, 0, 5, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref PositiveLimitSwitchStatus.A, 0, 6, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref PositiveLimitSwitchStatus.B, 0, 7, MCCL.CARDINDEX0);

            MCCL.MCC_GetLimitSwitchStatus(ref NegativeLimitSwitchStatus.X, 1, 0, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref NegativeLimitSwitchStatus.Y, 1, 1, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref NegativeLimitSwitchStatus.Z, 1, 2, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref NegativeLimitSwitchStatus.U, 1, 3, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref NegativeLimitSwitchStatus.V, 1, 4, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref NegativeLimitSwitchStatus.W, 1, 5, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref NegativeLimitSwitchStatus.A, 1, 6, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref NegativeLimitSwitchStatus.B, 1, 7, MCCL.CARDINDEX0);

            MCCL.MCC_GetHomeSensorStatus(ref HomeSensorStatus.X, 0, MCCL.CARDINDEX0);
            MCCL.MCC_GetHomeSensorStatus(ref HomeSensorStatus.Y, 1, MCCL.CARDINDEX0);
            MCCL.MCC_GetHomeSensorStatus(ref HomeSensorStatus.Z, 2, MCCL.CARDINDEX0);
            MCCL.MCC_GetHomeSensorStatus(ref HomeSensorStatus.U, 3, MCCL.CARDINDEX0);
            MCCL.MCC_GetHomeSensorStatus(ref HomeSensorStatus.V, 4, MCCL.CARDINDEX0);
            MCCL.MCC_GetHomeSensorStatus(ref HomeSensorStatus.W, 5, MCCL.CARDINDEX0);
            MCCL.MCC_GetHomeSensorStatus(ref HomeSensorStatus.A, 6, MCCL.CARDINDEX0);
            MCCL.MCC_GetHomeSensorStatus(ref HomeSensorStatus.B, 7, MCCL.CARDINDEX0);


            //進給率
            CurFeedSpeed = MCCL.MCC_GetCurFeedSpeed(0);

            MCCL.MCC_GetEmgcStopStatus(ref EmgcStopStatus, MCCL.CARDINDEX0);
            Error = MCCL.MCC_GetErrorCode(0).ToString();
            //MCCL.MCC_GetErrorCount();

            UpdateCoordinateSystem();
        }

        private static void UpdateCoordinateSystem()
        {
            absoluteCoordinate = CurPos;
            //
            machanicalCoordinate = ChangeType(ENCValue);
            relativeCoordinate = CurPos;

            Debug.WriteLine(ENCValue.X);
            Debug.WriteLine(ENCValue.Y);
            Debug.WriteLine(ENCValue.Z);
            Debug.WriteLine(machanicalCoordinate.X);
            Debug.WriteLine(machanicalCoordinate.Y);
            Debug.WriteLine(machanicalCoordinate.Z);
        }

        private static Axis<double> ChangeType(Axis<int> Source)
        {
            Axis<double> END = new Axis<double>();
            END.X = (double)Source.X / 100000 * 0.78484;
            END.Y = (double)Source.Y / 100000 * 0.78484;
            END.Z = (double)Source.Z / 100000 * 0.78484;
            END.U = (double)Source.U / 100000 * 0.78484;
            END.V = (double)Source.V / 100000 * 0.78484;
            END.W = (double)Source.W / 100000 * 0.78484;
            END.A = (double)Source.A / 100000 * 0.78484;
            END.B = (double)Source.B / 100000 * 0.78484;
            return END;
        }
    }
}
