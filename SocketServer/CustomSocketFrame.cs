using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    /// <summary>
    /// 数据帧报文格式
    /// </summary>
    public class CustomSocketFrame
    {
        #region Header

        /// <summary>
        ///LEADER：数据帧引导符 数据帧开始符， 4byte ，十六进制数值为EEEEEEEE
        /// </summary>
        public UInt32 Leader { get; set; }

        /// <summary>
        /// VER 协议版本号, uint8
        /// </summary>
        /// <remarks>
        /// --VER：协议版本号UINT8, SubVersion 子版本号UINT8，2个UINT8类型整数构成，形式为xx.yy
        /// ，xx表示主版本号，yy表示修订序列号，当前版本为01.00
        /// </remarks>
        public byte Version { get; set; }

        /// <summary>
        /// 协议版本号, uint8
        /// </summary>
        public byte SubVersion { get; set; }

        /// <summary>
        /// STC 同步传输编码，唯一标识一路数据传输通路，UINT32类型整数，4个字节
        /// </summary>
        public UInt32 SyncTranCode { get; set; }

        /// <summary>
        /// TS：数据帧时间戳，9字节
        /// </summary>
        public Timestamp Timestamp { get; set; }

        ///// <summary>
        ///// PL：载荷长度，UINT32型整数，4个字节
        ///// </summary>
        //public UInt32 PlayloadLength { get; set; }

        /// <summary>
        /// EL：扩展帧头长度，UINT8型整数，1个字节，最多可以指示扩展255个字节长度，默认为0
        /// </summary>
        public byte ExtendHeadLength 
        { 
            get 
            { 
                return ExtendHead == null ? (byte)0 : (byte)ExtendHead.Length;
            } 
        }

        /// <summary>
        /// ExHeader：扩展帧头
        /// </summary>
        public byte[] ExtendHead { get; set; }

        #endregion

        #region PayLoad Body 
        /// <summary>
        /// 数据帧体
        /// </summary>
        public List<SubBody> Body { get; set; }

        #endregion


        public CustomSocketFrame()
        {
            Leader = 0xEEEEEEEE;
        }
    }


    public struct Timestamp
    {
        /// <summary>
        /// 年，取值范围：[2000,2100]
        /// </summary>
        public UInt16 Year { get; set; }

        /// <summary>
        /// 月，取值范围：[1,12]
        /// </summary>
        public byte Month { get; set; }

        /// <summary>
        /// 日，取值范围：[1,31]
        /// </summary>
        public byte Day { get; set; }

        /// <summary>
        /// 时，取值范围：[0,23]
        /// </summary>
        public byte Houth { get; set; }

        /// <summary>
        /// 分，取值范围：[0,59]
        /// </summary>
        public byte Minute { get; set; }

        /// <summary>
        /// 秒，取值范围：[0,59]
        /// </summary>
        public byte Second { get; set; }

        /// <summary>
        /// 毫秒， 取值范围：[0,999]
        /// </summary>
        public UInt16 Millisecond { get; set; }
    }

    public struct SubBody
    {
        /// <summary>
        /// 数据结果类型，1字节，UINT8型整数
        /// </summary>
        public byte DataType { get; set; }

        /// <summary>
        /// DL：数据长度，UINT32型整数，4个字节
        /// </summary>
        public UInt32 DataLength
        {
            get 
            { 
                return (UInt32)Data.Length;
            }
        }

        /// <summary>
        /// DATA	：子体有效数据
        /// </summary>
        public byte[] Data { get; set; }
    
    }
}
