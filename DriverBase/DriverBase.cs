﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CHQ.RD.DataContract;
using GeneralOPs;
namespace CHQ.RD.DriverBase
{
    public class DriverBase:IDriverBase
    {
        #region 变量和属性
        protected Thread m_thread;
        protected object m_host;
        protected object m_datalist;
        protected Dictionary<int, object> m_valuelist;
        int m_transmode=0;
        int m_readmode=0;
        int m_readinterval=2000;
        Timer m_reader;
        string errorfile = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\DriverBaseError.log";
        string logfile = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\DriverBase.log";
        public string ErrorFile
        {
            get { return errorfile; }
        }
        public string LogFile
        {
            get { return logfile; }
        }
        Type m_hosttype = typeof(S7TCPHost);
        Type m_addresstype = typeof(S7Address);
        Dictionary<int, int> m_errorCount;
        protected Timer m_datareader;
        protected Timer m_errortransact;
        DriverStatus m_status = DriverStatus.None;
        public int TransMode
        {
            get { return m_transmode; }
            set { m_transmode = value; }
        }
        public int ReadMode
        {
            get { return m_readmode; }
            set { m_readmode = value; }
        }
        public int ReadInterval
        {
            get { return m_readinterval; }
            set { m_readinterval = value; }
        }
        public DriverStatus Status
        {
            get { return m_status; }
            set { SetStatus(value); }
        }
        public Type HostType
        {
            get { return m_hosttype; }
            set { m_hosttype = value; }
        }
        public Type AddressType
        {
            get { return m_addresstype; }
            set { m_addresstype = value; }
        }
        #endregion
        public DriverBase()
        {
            
        }


        #region Operations
        public virtual int SetStatus(DriverStatus status)
        {
            int ret = -1;
            /*状态设置规则 
             * 驱动的状态应该只有几种
             * 未初始-None
             * 已初始-Inited
             * 错误中-Error
             * 主动读取的驱动--Running,Stoped
             * 
             */
            try
            {
                switch (Status)
                {
                    //未初始状态下，只能置于错误
                    case DriverStatus.None:
                        if (status != DriverStatus.Error)
                        {
                            throw new Exception("None=>Error Only!");
                        }
                        else
                        {
                            m_status = DriverStatus.Error;
                        }
                        break;
                    case DriverStatus.Inited:
                        if (status == DriverStatus.Running || status == DriverStatus.Stoped)
                        {
                            if (ReadMode != 1)
                            {
                                throw new Exception("非主动读取模式中，不能设置运行或停止！");
                            }
                            else
                            {
                                //如果是运行，则设置开始运行，否则设置为stoped
                                m_status = status;
                            }
                        }
                        else
                        {
                            m_status = status;
                        }
                        break;
                    case DriverStatus.Error:
                        //根据状态设置其运行
                        break;
                    case DriverStatus.Running:
                        //错误、停止
                        break;
                    case DriverStatus.Stoped:
                        //启动、错误
                        break;
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "DriverBase.SetStatus(" + status.ToString() + "):" + ex.Message);
            }
            return ret;
        }
        public virtual int Init()
        {
            int ret = -1;
            try
            {
                ret = AcceptSetting(m_host, m_datalist);
                if (ret != 0)
                {
                    throw new Exception("加载设置应用失败！");
                }
                m_status = DriverStatus.Inited;
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "DriverBase.Init(" + m_host.ToString() + "):" + ex.Message);
            }
            return ret;
        }
        public virtual int Start()
        {
            int ret = -1;
            try
            {
                if (m_readmode != 1)
                {
                    throw new Exception("非主动读取模式不能启动读取！");
                }
                else
                {
                    if (m_status == DriverStatus.Stoped || m_status == DriverStatus.Inited)
                    {
                        //TODO:启动自动读取
                    }
                    else
                    {
                        throw new Exception(m_status.ToString()+"=>"+DriverStatus.Running.ToString()+" 错误，不支持的状态");
                    }
                }
                m_status = DriverStatus.Running;
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "DriverBase.Start(" + m_host.ToString() + "):" + ex.Message);
            }
            return ret;
        }
        public virtual int Stop()
        {
            int ret = -1;
            return ret;
        }
        public virtual int Restart()
        {
            int ret = -1;
            return ret;
        }
        #endregion

        public virtual void Dispose()
        {

        }
        public virtual int AcceptSetting(object host,object list)
        {
            int ret = -1;
            return ret;
        }

        public virtual int TryConnectToDevice()
        {
            int ret = -1;
            return ret;
        }
        public virtual object ReadData(int ItemId)
        {
            object ret = -1;
            return ret;
        }

        public virtual object ReadDeviceData(object Item)
        {
            object ret = -1;
            return ret;
        }
        public virtual int SetStatus(object status)
        {
            int ret = -1;
            return ret;
        }

        public virtual int SendData(object value)
        {
            int ret = -1;
            return ret;
        }
        public virtual object ParsingHost(string host)
        {
            object ret = null;
            return ret;
        }
        public virtual object ParsingAddress(string address)
        {
            object ret = null;
            return ret;
        }
    }
}
