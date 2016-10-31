using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    public static class CustomSocketFrameAdapterPacketExtension
    {
        public static byte[] Pack(this CustomSocketFrame frame)
        {
            #region Body

            var bodyStream = new MemoryStream();
            var bodyBw = new BinaryWriter(bodyStream, Encoding.UTF8);
            foreach (var item in frame.Body)
            {
                bodyBw.Write(item.DataType);
                bodyBw.Write(item.DataLength);
                bodyBw.Write(item.Data);
            }
            var bodyBytes = bodyStream.ToArray();
            bodyStream.Close();

            #endregion

            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms, Encoding.UTF8);

            #region Head

            bw.Write(frame.Leader);
            bw.Write(frame.Version);
            bw.Write(frame.SubVersion);
            bw.Write(frame.SyncTranCode);
            bw.Write(frame.Timestamp.Year);
            bw.Write(frame.Timestamp.Month);
            bw.Write(frame.Timestamp.Day);
            bw.Write(frame.Timestamp.Houth);
            bw.Write(frame.Timestamp.Minute);
            bw.Write(frame.Timestamp.Second);
            bw.Write(frame.Timestamp.Millisecond);
            bw.Write((UInt32)bodyBytes.Length);
            bw.Write(frame.ExtendHeadLength);
            bw.Write(frame.ExtendHead);
            bw.Write(bodyBytes);

            #endregion

            var result = ms.ToArray();
            ms.Close();
            return result;
        }

        public CustomSocketFrame UnPack(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 是否帧头
        /// </summary>
        /// <param name="bytes">4字节</param>
        /// <param name="headFlag"></param>
        /// <returns></returns>
        public bool IsLeader(byte[] bytes, UInt32 headFlag)
        {
            var num = BitConverter.ToUInt32(bytes, 0);
            return num == headFlag;
        }




    }

    

}
