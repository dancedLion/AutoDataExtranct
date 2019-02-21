﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverBase;
using DataContract;
using GeneralOPs;
namespace S7Sharp7Driver
{
    

    public class S7Sharp7Driver:DriverBase.DriverBase
    {
        List<S7Sharp7AddressSetting> l_items;
        List<S7DataItem> m_dataitem;
        S7Sharp7Host m_host;
        S7Client m_client;
        Dictionary<string, int> m_valuetype;
        Dictionary<string, int> m_dbtype;
        Dictionary<string, int> m_datalen;
        public S7Sharp7Driver()
        {
            //初始化列表
            l_items = new List<S7Sharp7AddressSetting>();
            m_items = new List<IAddressSetting>();
            m_items = l_items.Cast<IAddressSetting>().ToList();
            m_host = new S7Sharp7Host();
            m_client = new S7Client();
            m_dbtype = new Dictionary<string, int>();
            m_dbtype.Add("DB", 0x84);
            m_dbtype.Add("MB", 0x83);
            m_dbtype.Add("PE", 0x81);   //IB

            m_valuetype = new Dictionary<string, int>();
            m_valuetype.Add("BIT", 0x02);
            m_valuetype.Add("BYTE", 0x02);
            m_valuetype.Add("REAL", 0x08);
            m_valuetype.Add("TEXT", 0x02);
            m_valuetype.Add("INT16", 0x04);
            m_valuetype.Add("INT", 0x06);
            m_valuetype.Add("UINT16", 0x04);
            m_valuetype.Add("UINT32", 0x06);

            m_datalen = new Dictionary<string, int>();
            m_datalen.Add("BIT", 1);
            m_datalen.Add("BYTE", 1);
            m_datalen.Add("REAL", 4);
            m_datalen.Add("TEXT", 1);
            m_datalen.Add("INT16", 2);
            m_datalen.Add("UINT16", 2);
            m_datalen.Add("UINT32", 4);
            m_datalen.Add("INT", 4);

            m_dataitem = new List<S7DataItem>();
        }

        public int AcceptSetting(object host,object list)
        {
            int ret = 1;
            try
            {
                m_host = (S7Sharp7Host)host;
                l_items = (List<S7Sharp7AddressSetting>)list;
                ret = m_client.ConnectTo(m_host.IpAddress, m_host.RackNo, m_host.SlotNo);
                if (ret != 0)
                {
                    throw new Exception("Connect to Host(" + m_host.IpAddress + ") error,error code=" + ret.ToString());
                }
                //每个值的试读
                ret = SettingAddress();
                //地址设置的错误不影响整 体的使用
                if (ret != 1)
                {
                    ret = 1;
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(this.ToString() + ":AcceptSetting(" + host.ToString() + "," + list.ToString() + "):" + ex.Message);
                ret = -1;
            }
            return ret;
        }
        public override int SettingAddress()
        {
            //return base.SettingAddress();
            //测试每个地址的设置是否OK，如果错误就返回错误值 
            int result = 1;
            try
            {
                foreach (IAddressSetting add in l_items)
                {
                    object t = ReadDeviceData(add.Id);
                    if (t == null)
                    {
                        throw new Exception("Testing Reading Data ERROR,Item Id=" + add.Id.ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(this.ToString() + ":SettingAddress ERROR:" + ex.Message);
                result = -1;
            }
            return result;
        }
        public override object ReadDeviceData(int ItemId)
        {
            object ret=null;
            int i = 0;
            try
            {
                S7Sharp7AddressSetting add = l_items.Find((S7Sharp7AddressSetting addset) => { return addset.Id == ItemId; });
                S7DataAddress address = (S7DataAddress)add.Address;
                S7Client.S7DataItem item = new S7Client.S7DataItem();
                //S7Client client = new S7Client();
                //ret=client.ConnectTo(m_host.IpAddress, m_host.RackNo, m_host.SlotNo);
                byte[] buffer = new byte[12];
                i = m_client.ReadArea(
                        m_dbtype[address.BlockType.ToString()],
                        address.BlockAddress,
                        address.ByteAddress,
                        address.DataType.ToString() == "BIT" ? 1 : (address.DataLength / S7.DataSizeByte(m_valuetype[address.DataType.ToString()])),
                        m_valuetype[address.DataType.ToString()],
                        buffer);
                if (i != 0)
                {
                    throw new Exception("read data error, item id=" + ItemId.ToString() + "!");
                }
                else
                {
                    switch (address.DataType)
                    {
                        case "BIT":
                            ret = S7ByteConvert.ToBit(buffer[0], address.DataLength, 0);
                            break;
                        case "BYTE":
                            ret = buffer[0];
                            break;
                        case "INT":
                            ret = S7ByteConvert.ToInt(buffer, 0);
                            break;
                        case "INT16":
                            ret = S7ByteConvert.ToInt16(buffer, 0);
                            break;
                        case "REAL":
                            ret = S7ByteConvert.ToFloat(buffer, 0);
                            break;
                        case "UINT16":
                            ret = S7ByteConvert.ToUInt16(buffer, 0);
                            break;
                        case "UINT32":
                            ret = S7ByteConvert.ToUInt32(buffer, 0);
                            break;
                        case "TEXT":
                            ret = Encoding.Default.GetString(buffer, 2, address.DataLength - 2);
                            break;
                        default:
                            ret = BitConverter.ToString(buffer);
                            break;

                    }
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(this.ToString() + ":ReadDeviceData error:" + ex.Message);
            }
            return ret;
            //return base.ReadDeviceData(ItemId);
        }
    }
}