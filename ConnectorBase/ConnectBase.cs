﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace CHQ.RD.ConnectorBase
{
    public class ConnectorBase:IConnectorBase
    {
        protected string XmlSettingFile =AppDomain.CurrentDomain.BaseDirectory+ "\\connector.xml";
        protected Thread readingThread;
        protected Thread manageThread;
        protected Thread sendingThread;
        protected Thread listenThread;
        public Dictionary<int, object> ValueList;
        protected List<ConnDriverBase> connDriverList;
        int m_id= -1;
        public int ConnectorId
        {
            get { return m_id; }
        }

        public ConnectorBase(int id)
        {
            m_id = id;
        }

        public virtual int ReadValue(int itemid)
        {
            return 1;
        }

        public virtual int InitConnDriver(ConnDriverBase conn)
        {
            int ret = -1;
            return ret;
        }
        public virtual int RunConnDriver(ConnDriverBase conn)
        {
            int ret = -1;
            return ret;
        }
        public virtual int TestConnDriver(ConnDriverBase conn)
        {
            int ret = -1;
            return ret;
        }
        public virtual int StopConnDriver(ConnDriverBase conn)
        {
            int ret = -1;
            return ret;
        }
        public virtual int PauseConnDriver(ConnDriverBase conn)
        {
            int ret = -1;
            return ret;
        }
        public virtual int CloseConnDriver(ConnDriverBase conn)
        {
            int ret = -1;
            return ret;
        }


    }
}
