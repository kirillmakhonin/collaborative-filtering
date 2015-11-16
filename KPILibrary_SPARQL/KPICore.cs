/* 
 * KPCore
 * KP (Knowledge Processor) library implementation
 * http://sourceforge.net/projects/smart-m3/
 
    Copyright (C) 2012  Luca Roffia, lroffia@arces.unibo.it

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

//Open the socket on JOIN and close on LEAVE
//#define KEEP_SOCKET_OPENED

#define OSGi_SUPPORT

//Uncomment for compiling on .NET CF
//#define KPI_CF

//Async socket connection support
#define ASYNC_CONNECT

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections;

namespace KPICore
{
    //Logging
    public delegate void EventLogger(object sender, string msg);

    public class SPARQLResults
    {
        public List<string> variables;
        public List<SPARQLResult> results;
        public string xmlResponce;

        public class SPARQLResult
        {
            public List<SPARQLBinding> bindings;
        }

        public class SPARQLBinding
        {
            public string name;
            public string value;
            public SPARQLValueType type;
        }

        public enum SPARQLValueType { URI, LITERAL, BNODE };
    }

    //Callback definition for subscribe notifications
    public interface iKPIC_subscribeHandler
    {
        void kpic_SIBEventHandler(ArrayList newResults, ArrayList obsoleteResults, string subID);
        void kpic_SIBEventHandlerSPARQL(SPARQLResults newResults, SPARQLResults obsoleteResults, string subID);
    }

    public class KPICore
    {
        // Event logging
        private static bool LOG_SSAP_MSG = false;
        public event EventLogger __logger;
        public void logger(object sender, string msg)
        {
            if (__logger == null) return;
            DateTime now = DateTime.Now;
            msg = now.ToLongTimeString() + "."+ now.Millisecond+" KPI:" + msg;
            __logger(sender,msg);
        }
        public void SSAPLogging(bool enabled) {
            LOG_SSAP_MSG = enabled;   
        }

        //Main KP socket
        private Socket kpSocket = null;

#if KPI_CF
        //Not supported in .NET CF
#else
         int SOCKET_TIMEOUT = 10000;
#endif
        //Local members defaults
        private string HOST = "127.0.0.1";
        private int PORT = 10010;
        private string SMART_SPACE_NAME = "X";
        private string nodeID = "00000000-0000-0000-0000-000000000000";

        //Used for SSAP messages managment
        private SSAP_XMLTools xmlTools = null;

        #region CONSTRUCTOR

        private void initDefaultNamespaces() {
            /* W3C Namespaces
             * prefix rdf:, namespace URI: http://www.w3.org/1999/02/22-rdf-syntax-ns#
             * prefix rdfs:, namespace URI: http://www.w3.org/2000/01/rdf-schema#
             * prefix dc:, namespace URI: http://purl.org/dc/elements/1.1/
             * prefix owl:, namespace URI: http://www.w3.org/2002/07/owl#
             * prefix xsd:, namespace URI: http://www.w3.org/2001/XMLSchema#
            */
            if (sibNamespaces.Count == 0)
            {
                sibNamespaces.Add("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                sibNamespaces.Add("rdfs", "http://www.w3.org/2000/01/rdf-schema#");
                sibNamespaces.Add("dc", "http://purl.org/dc/elements/1.1/");
                sibNamespaces.Add("owl", "http://www.w3.org/2002/07/owl#");
                sibNamespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema#");
            }
        }

        public KPICore(string HOST, int PORT, string SMART_SPACE_NAME, string nodeID)
        {
            this.HOST = HOST;
            this.PORT = PORT;
            this.SMART_SPACE_NAME = SMART_SPACE_NAME;
            this.nodeID = nodeID;
            this.xmlTools = new SSAP_XMLTools(this.nodeID, this.SMART_SPACE_NAME);

#if DEBUG
            this.xmlTools.__logger += new EventLogger(xmlTools___logger);
#endif
            //Load default namespaces
            initDefaultNamespaces();
        }

#if DEBUG
        void xmlTools___logger(object sender, string msg)
        {
            logger(sender, msg);
        }
#endif
        public KPICore(string HOST, int PORT, string SMART_SPACE_NAME)
        {
            this.HOST = HOST;
            this.PORT = PORT;
            this.SMART_SPACE_NAME = SMART_SPACE_NAME;
            
            //Create a random NODE ID
            this.nodeID = Guid.NewGuid().ToString();
            this.xmlTools = new SSAP_XMLTools(this.nodeID, this.SMART_SPACE_NAME);

#if DEBUG
            this.xmlTools.__logger += new EventLogger(xmlTools___logger);
#endif

            //Load default namespaces
            initDefaultNamespaces();
        }

        #endregion

        #region LOW LEVEL SOCKET MANAGEMENT

        private bool openConnection(ref Socket kpSocket) {
            return openConnection(ref kpSocket, SOCKET_TIMEOUT);
        }
        
        private bool openConnection(ref Socket kpSocket,int timeout)
        {
            logger(this, "Socket:OpenConnection@begin");

            try{
                //Use DNS to retrive the SIB IP address
                EndPoint SIBEndPoint = null;
                logger(this, "Socket:Resolve host address " + HOST);
                IPAddress[] ipAddr = Dns.GetHostAddresses(HOST);

                if (ipAddr.Length > 0)
                {
                    SIBEndPoint = new IPEndPoint(ipAddr[0], PORT);
                    logger(this, "Socket:SIB EndPoint: " + SIBEndPoint.ToString());

                    //Create a new socket
                    if (kpSocket == null) kpSocket = new Socket(SIBEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    logger(this, "Socket:Open socket...(timeout " + connectTimeOut.ToString() + ")");
                    kpSocket.ReceiveTimeout = timeout;
                    kpSocket.SendTimeout = timeout;

#if ASYNC_CONNECT
                    //Async connection
                    allDone.Reset();
                    kpSocket.BeginConnect(SIBEndPoint, new AsyncCallback(ConnectCallback), kpSocket);
                    allDone.WaitOne(connectTimeOut);
                    if (kpSocket.Connected) {
                        logger(this, "Socket:Connected " + kpSocket.LocalEndPoint.ToString() + "-->" + kpSocket.RemoteEndPoint.ToString());
                        logger(this, "Socket:OpenConnection@end");
                        return true;
                    }

                    logger(this, "Socket:Connect timeout");
                    return false;
#else
                    kpSocket.Connect(SIBEndPoint);
                    logger(this, "Socket:Connected " + kpSocket.LocalEndPoint.ToString() + "-->" + kpSocket.RemoteEndPoint.ToString());
                    logger(this, "Socket:OpenConnection@end");
                    return true;
#endif
                }
                logger(this, "Socket:Resolve host address failed");
                return false;
            }
            catch (Exception e) {
                logger(this, "EXP:Socket:OpenConnection exception ("+e.Message+")");
                return false;
            }
        }

#if ASYNC_CONNECT
        //To manage async socket connection
        private const int connectTimeOut = 5000;
        private ManualResetEvent allDone = new ManualResetEvent(false);
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                logger(this, "Asyn connect callback");
                allDone.Set();
                Socket s = (Socket)ar.AsyncState;
                s.EndConnect(ar);
            }
            catch (Exception e){
                logger(this, "Async callback exception due to timeout...");
            }
        }
#endif

        private bool closeConnection(ref Socket kpSocket)
        {
            logger(this, "Socket:CloseConnection@begin");

            try
            {
#if OSGi_SUPPORT
                kpSocket.Shutdown(SocketShutdown.Receive);
#else
                kpSocket.Shutdown(SocketShutdown.Both);
#endif
                // False: OSGi (Alfredo)
                kpSocket.Disconnect(false);
                kpSocket.Close();
                kpSocket = null;
                
                logger(this, "Socket:CloseConnection:Socket closed");
                logger(this, "Socket:CloseConnection@end");
                return true;

            }
            catch (Exception e)
            {
                logger(this, "Socket:CloseConnection exception (" + e.Message + ")");
                logger(this, "Socket:CloseConnection@end");
                return false;
            }
        }
        private bool send(ref Socket kpSocket, string msg)
        {

            //byte[] dataToSend = System.Text.Encoding.ASCII.GetBytes(msg);
            byte[] dataToSend = System.Text.Encoding.UTF8.GetBytes(msg);

            logger(this, "Socket:Send (" + dataToSend.Length.ToString() + " bytes)");

            if (LOG_SSAP_MSG)
            {
                logger(this, "SSAP:MESSAGE SENT\r\n------------------\r\n" + msg + "\r\n------------------");
            }

            try
            {
                if (kpSocket.Send(dataToSend) == dataToSend.Length)
                {
                    //OSGi SIB
#if OSGi_SUPPORT
                    kpSocket.Shutdown(SocketShutdown.Send);
#endif
                    logger(this, "Socket:Message sent");
                    return true;
                }
            }
            catch (Exception e)
            {
                logger(this, "Socket:Send exception (" + e.Message + ")");
            }
            return false;
        }
        private string receive(ref Socket kpSocket)
        {
            int attempt = 0;
            string msg = "";
            
            //Receive buffer size
            byte[] byteReceived = new byte[4096];
            
            int len = 0;

            try
            {
                int i = 0;
                while (true)
                {
                    try
                    {
                        len = kpSocket.Receive(byteReceived);
                    }
                    catch (SocketException e)
                    {
                        //Timeout exception to be handle or socket has been closed
                        logger(this, "Socket:Receive SocketException (" + e.Message + ")");
                        return "";
                    }

                    if (len > 0)
                    {
                        attempt++;

                        //msg += System.Text.Encoding.ASCII.GetString(byteReceived, 0, len);
                        msg += System.Text.Encoding.UTF8.GetString(byteReceived, 0, len);

                        //OSGi SIB
                        if (msg.Contains("<SSAP_message>") && msg.Contains("</SSAP_message>"))
                        //if (msg.StartsWith("<SSAP_message>") && msg.EndsWith("</SSAP_message>"))
                        {
                            logger(this, "Socket:Receive SSAP response (" + msg.Length.ToString() + " bytes) in "+attempt.ToString()+ " slot(s)");
                            
                            if (LOG_SSAP_MSG)
                            {
                                logger(this, "SSAP:MESSAGE RECEIVED\r\n------------------\r\n" + msg + "\r\n------------------");
                            }
                            
                            return msg;
                        }
                    }
                    else {
                        if (i < 3) {
                            i++;
                            Thread.Sleep(500);
                            continue;
                        }
                        logger(this, "Socket:Receive ERROR. No answer from SIB after "+ i.ToString() +" attempts");
                        return "";
                    }
                }
            }
            catch (ObjectDisposedException e)
            {
                //Timeout exception to be handle or socket has been closed
                logger(this, "Socket:Receive ObjectDisposedException (" + e.Message + ")");
                return "";
            }
            catch (System.Security.SecurityException e)
            {
                //Timeout exception to be handle or socket has been closed
                logger(this, "Socket:Receive SecurityException (" + e.Message + ")");
                return "";
            }
            catch (Exception e) {
                logger(this, "Socket:Receive Exception (" + e.Message + ")");
                return "";
            }
        }
        
        private string sendSSAPMsg(string msg) {
            return sendSSAPMsg(msg, SOCKET_TIMEOUT);
        }
        private string sendSSAPMsg(string msg,int timeout)
        {
            logger(this, "Socket:SSAP request@begin");

#if KEEP_SOCKET_OPENED
            //Open the socket on JOIN and close on LEAVE
#else
            if (!openConnection(ref kpSocket,timeout)) return null;
#endif
            if (!kpSocket.Connected) {
                logger(this, "Socket:Socket not connected...exit :(");
                logger(this, "Socket:SSAP request@end");
                return null;
            }

            kpSocket.SendTimeout = 60000;
            logger(this, "Socket:Send with timeout " + kpSocket.SendTimeout.ToString() + " msec");
            if (!send(ref kpSocket, msg))
            {
                logger(this, "Socket:Data not sent :(");
                logger(this, "Socket:SSAP request@end");

#if KEEP_SOCKET_OPENED
                //Open the socket on JOIN and close on LEAVE
                return null;
#else
                if (!closeConnection(ref kpSocket)) return null;
                return "";
#endif              
            }

            if (msg.IndexOf("<transaction_type>UNSUBSCRIBE</transaction_type>") < 0)
            {
                try
                {
                    kpSocket.ReceiveTimeout = 120000;
                    logger(this, "Socket:Receive with timeout " + kpSocket.ReceiveTimeout.ToString() + " msec");
                    string ret = receive(ref kpSocket); 
                    logger(this, "Socket:SSAP request@end");

#if KEEP_SOCKET_OPENED
                    //Open the socket on JOIN and close on LEAVE
#else
                    if (!closeConnection(ref kpSocket)) return null;                  
#endif      
                    return ret;
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    logger(this, "Socket:SSAP request exception (" + e.Message + ")");
                    logger(this, "EXP:Socket:SSAP request@end");

#if KEEP_SOCKET_OPENED
                    //Open the socket on JOIN and close on LEAVE
#else
                    if (!closeConnection(ref kpSocket)) return null;
#endif
                    return "";
                }
            }
            else
            {
                logger(this, "Socket:UnSubscription request...no answer expected");
                logger(this, "Socket:SSAP request@end");

#if KEEP_SOCKET_OPENED
                //Open the socket on JOIN and close on LEAVE
#else
                if (!closeConnection(ref kpSocket)) return null;
#endif
                return "";
            }
        }

        #endregion

        #region SSAP_API
        public bool join()
        {
            logger(this, "[JOIN]");
            logger(this, "SIB EndPoint ** " + HOST + ":" + PORT + " SS: " + SMART_SPACE_NAME + " KP ID: " + nodeID+ " **");

#if KEEP_SOCKET_OPENED
            //Open the socket on JOIN and close on LEAVE
            if (!openConnection(ref kpSocket)) return false;
#endif         
            
            return xmlTools.isJoinConfirmed(sendSSAPMsg(this.xmlTools.join(),2000));
        }
        public bool leave()
        {
            logger(this, "[LEAVE]");
            logger(this, "SIB EndPoint ** " + HOST + ":" + PORT + " SS: " + SMART_SPACE_NAME + " KP ID: " + nodeID+ " **");

            bool ret = xmlTools.isLeaveConfirmed(sendSSAPMsg(this.xmlTools.leave(),2000));

#if KEEP_SOCKET_OPENED
            //Open the socket on JOIN and close on LEAVE
            bool ret1 = closeConnection(ref kpSocket);
            return ret && ret1;
#endif
            return ret;
        }

        //Graph as Arraylist of string[4]={s,p,o,o_type}
        public bool insert(ArrayList insert_graph)
        {
            logger(this, "[INSERT GRAPH]");
            return xmlTools.isInsertConfirmed(sendSSAPMsg(this.xmlTools.insert(insert_graph)));
        }
        public bool remove(ArrayList remove_graph)
        {
            logger(this, "[REMOVE GRAPH]");
            return xmlTools.isRemoveConfirmed(sendSSAPMsg(this.xmlTools.remove(remove_graph)));
        }
        public bool update(ArrayList insert_graph, ArrayList remove_graph)
        {
            logger(this, "[UPDATE GRAPH]");
            return xmlTools.isUpdateConfirmed(sendSSAPMsg(this.xmlTools.update(insert_graph, remove_graph)));
        }
        public ArrayList queryRDF(ArrayList graph)
        {
            logger(this, "[QUERY RDF GRAPH]");
            string responce = sendSSAPMsg(this.xmlTools.queryRDFGraph(graph));
            if (xmlTools.isQueryConfirmed(responce)) return xmlTools.getQueryTriple(responce);
            else return null;
        }

        public string subscribeRDF(ArrayList triples, iKPIC_subscribeHandler f_eh) {
            ArrayList results = new ArrayList();
            return subscribeRDF(triples, f_eh, ref results);
        }
        public string subscribeRDF(ArrayList triples, iKPIC_subscribeHandler f_eh,ref ArrayList results)
        {
            logger(this, "[SUBSCRIBE RDF GRAPH]");

            if (f_eh == null)
            {
                logger(this, "KPI:SubscribeRDF:Subscription event handler is null");
                return "";
            }

            Socket subSocket = null;

            logger(this, "KPI:SubscribeRDF:Create a new socket for subscription");

            if (!openConnection(ref subSocket)) return "";

            if (!send(ref subSocket, this.xmlTools.subscribeRDF_XML(triples)))
            {
                closeConnection(ref subSocket);
                return "";
            }

            string msg = receive(ref subSocket);

            if (this.xmlTools.isSubscriptionConfirmed(msg))
            {
                SubscriptionDescriptor info = new SubscriptionDescriptor();
#if KPI_CF
                threadParam = info;
#endif
                string subID = this.xmlTools.getSubscriptionID(msg);
                logger(this, "KPI:SubscribeRDF:New subscription ID " + subID);
                
                info.subID = subID;
                info.type = SUBSCRITPION_TYPE.RDF;
                info.subSocket = subSocket;

                results = this.xmlTools.getQueryTriple(msg);
#if KPI_CF
                // NOT SUPPORTED IN .NET CF 3.5
#else
                info.subSocket.ReceiveTimeout = -1;
#endif
                info.subHandler = f_eh;
#if KPI_CF              
                info.subThread = new Thread(new ThreadStart(run));
                info.subThread.IsBackground = true;
#else
                info.subThread = new Thread(new ParameterizedThreadStart(run));
                info.subThread.Name = "SUB THREAD " + subID;
#endif
                activeSubscribtions.Add(subID, info);
#if KPI_CF
                info.subThread.Start();
#else
                info.subThread.Start(info);
#endif
                return subID;
            }
            else
            {
                closeConnection(ref subSocket);
            }

            return "";
        }
        public bool unsubscribe(string subscriptionID)
        {
            return unsubscribe(subscriptionID, false);
        }
        public bool unsubscribe(string subscriptionID, bool force)
        {
            logger(this, "[UNSUBSCRIBE] SubID: " + subscriptionID + " forced: " + force.ToString());

            if (!force)
            {
                //Send UNSUBSCRIBE request and wait for indication in the subscription thread
                //The thread will exit and the socket will be closed if the SUBSCRIPTION indication is received
                SubscriptionDescriptor descr = (SubscriptionDescriptor)activeSubscribtions[(string)subscriptionID];

                sendSSAPMsg(this.xmlTools.unsubscribeRDF(subscriptionID));

                if (descr == null) return false;
                while (descr.subThread.ThreadState != ThreadState.Stopped) { }
                descr.subSocket = null;
                activeSubscribtions.Remove(subscriptionID);

                logger(this, "Thread " + descr.subThread.Name + " stopped");

                return true;
            }
            else
            {
                //FORCE: remove subscription if thread is terminated (e.g. connection lost)
                SubscriptionDescriptor descr = (SubscriptionDescriptor)activeSubscribtions[(string)subscriptionID];

                logger(this, "Remove subscription...");

                if (descr.subThread.ThreadState == ThreadState.Stopped)
                {
                    logger(this, "Thread " + descr.subThread.ManagedThreadId + " stopped");
                    descr.subSocket = null;
                    return true;
                }

                logger(this, "Thread is running...");

                return false;
            }
        }
        public bool unsubscribeAll()
        {
            return unsubscribeAll(false);
        }
        public bool unsubscribeAll(bool force)
        {
            bool ret = true;
            if (activeSubscribtions == null) return true;
            if (activeSubscribtions.Count == 0) return true;
            Array keys = new string[activeSubscribtions.Count];
            activeSubscribtions.Keys.CopyTo(keys, 0);
            for (int i = keys.GetLowerBound(0); i < keys.GetUpperBound(0) + 1; i++)
            {
                ret = ret && unsubscribe(keys.GetValue(i).ToString(), force);
            }
            return ret;
        }
        
        //SPARQL Subscription
        public string subscribeSPARQL(string query, iKPIC_subscribeHandler f_eh, ref SPARQLResults results)
        {
            logger(this, "[SUBSCRIBE SPARQL]");

            if (f_eh == null)
            {
                logger(this, "KPI:SubscribeSPARQL:Subscription event handler is null");
                return "";
            }

            Socket subSocket = null;

            logger(this, "KPI:SubscribePARQL:Create a new socket for subscription");

            if (!openConnection(ref subSocket)) return "";

            if (!send(ref subSocket, this.xmlTools.subscribeSPARQL(query)))
            {
                closeConnection(ref subSocket);
                return "";
            }

            string msg = receive(ref subSocket);

            if (this.xmlTools.isSubscriptionConfirmed(msg))
            {
                SubscriptionDescriptor info = new SubscriptionDescriptor();
#if KPI_CF
                threadParam = info;
#endif
                string subID = this.xmlTools.getSubscriptionID(msg);
                logger(this, "KPI:SubscribeRDF:New subscription ID " + subID);

                info.subID = subID;
                info.type = SUBSCRITPION_TYPE.SPARQL;
                info.subSocket = subSocket;

                results = this.xmlTools.getSPARQLResults(msg);
#if KPI_CF
                // NOT SUPPORTED IN .NET CF 3.5
#else
                info.subSocket.ReceiveTimeout = -1;
#endif
                info.subHandler = f_eh;
#if KPI_CF              
                info.subThread = new Thread(new ThreadStart(run));
                info.subThread.IsBackground = true;
#else
                info.subThread = new Thread(new ParameterizedThreadStart(run));
                info.subThread.Name = "SUB THREAD " + subID;
#endif
                activeSubscribtions.Add(subID, info);
#if KPI_CF
                info.subThread.Start();
#else
                info.subThread.Start(info);
#endif
                return subID;
            }
            else
            {
                closeConnection(ref subSocket);
            }

            return "";
        }

        //Insert an OWL RDF-XML description
        public bool insertOWL(string owl) {
            logger(this, "[INSERT OWL]");
            return xmlTools.isInsertConfirmed(sendSSAPMsg(this.xmlTools.insertOWL(owl)));
        }

        // Single triple operations
        public bool insert(string s, string p, string o, string o_type)
        {
            ArrayList triples = new ArrayList();
            string[] triple = new string[4];
            triple[0] = s;
            triple[1] = p;
            triple[2] = o;
            triple[3] = o_type;
            triples.Add(triple);
            return insert(triples);
        }             
        public bool remove(string s, string p, string o, string o_type)
        {
            ArrayList triples = new ArrayList();
            string[] triple = new string[4];
            triple[0] = s;
            triple[1] = p;
            triple[2] = o;
            triple[3] = o_type;
            triples.Add(triple);
            return remove(triples);
        }      
        public bool update(string s_new, string p_new, string o_new, string o_new_type, string s_old, string p_old, string o_old, string o_old_type)
        {
            ArrayList oldTriples = new ArrayList();
            ArrayList newTriples = new ArrayList();
            string[] newTriple = new string[4];
            string[] oldTriple = new string[4];
            newTriple[0] = s_new;
            newTriple[1] = p_new;
            newTriple[2] = o_new;
            newTriple[3] = o_new_type;
            newTriples.Add(newTriple);
            oldTriple[0] = s_old;
            oldTriple[1] = p_old;
            oldTriple[2] = o_old;
            oldTriple[3] = o_old_type;
            oldTriples.Add(oldTriple);
            return update(newTriples,oldTriples);
        }     
        public ArrayList queryRDF(string s, string p, string o, string o_type)
        {
            ArrayList triples = new ArrayList();
            string[] triple = new string[4];
            triple[0] = s;
            triple[1] = p;
            triple[2] = o;
            triple[3] = o_type;
            triples.Add(triple);
            return queryRDF(triples);
        }      
        public string subscribeRDF(string s, string p, string o, string o_type, iKPIC_subscribeHandler f_eh)
        {
            ArrayList triples = new ArrayList();
            string[] triple = new string[4];
            triple[0] = s;
            triple[1] = p;
            triple[2] = o;
            triple[3] = o_type;
            triples.Add(triple);
            return subscribeRDF(triples, f_eh); 
        }      

        //OLD API SUPPORT
        public bool insert(string s, string p, string o, string s_type, string o_type)
        {
            return insert(s, p, o, o_type);
        }
        public bool remove(string s, string p, string o, string s_type, string o_type)
        {
            return remove(s, p, o, o_type);
        }
        public bool update(string sn, string pn, string on, string sn_type, string on_type, string so, string po, string oo, string so_type, string oo_type)
        {
            return update(sn, pn, on, on_type, so, po, oo, oo_type);
        }
        public ArrayList queryRDF(string s, string p, string o, string s_type, string o_type)
        {
            return queryRDF(s, p, o, o_type);
        }

        public ArrayList queryWQL_values(string[] startNode, string path)
        {
            logger(this, "[QUERY WQL-VALUES]");
            string responce = sendSSAPMsg(this.xmlTools.queryWQL_values(startNode[0], startNode[1], path));
            if (xmlTools.isQueryConfirmed(responce)) return xmlTools.getValuesQueryNodeList(responce);
            return null;
        }
        public ArrayList queryWQL_nodeTypes(string startURI)
        {
            logger(this, "[QUERY WQL-NODETYPES]");
            string responce = sendSSAPMsg(this.xmlTools.queryWQL_nodeTypes(startURI));
            if (xmlTools.isQueryConfirmed(responce)) return xmlTools.getQueryTriple(responce);
            return null;
        }
        public bool queryWQL_related(string[] startNode, string[] endNode, string path)
        {
            logger(this, "[QUERY WQL-RELATED]");
            string responce = sendSSAPMsg(this.xmlTools.queryWQL_related(startNode[0], startNode[1], endNode[0], endNode[1], path));
            if (xmlTools.isQueryConfirmed(responce)) return xmlTools.getBoleanResultsFromWQLquery(responce);
            return false;
        }
        public bool queryWQL_isType(string instanceURI, string classURI)
        {
            logger(this, "[QUERY WQL-ISTYPE]");
            string responce = sendSSAPMsg(this.xmlTools.queryWQL_isType(instanceURI, classURI));
            if (xmlTools.isQueryConfirmed(responce)) return xmlTools.getBoleanResultsFromWQLquery(responce);
            return false;
        }
        public bool queryWQL_isSubType(string instanceURI, string classURI)
        {
            logger(this, "[QUERY WQL-ISSUBTYPE]");
            string responce = sendSSAPMsg(this.xmlTools.queryWQL_isSubType(instanceURI, classURI));
            if (xmlTools.isQueryConfirmed(responce)) return xmlTools.getBoleanResultsFromWQLquery(responce);
            return false;
        }
        public string subscribeWQL_VALUE(string[] startNode, string path, iKPIC_subscribeHandler f_eh)
        {
            logger(this, "[SUBSCRIBE WQL VALUE]");

            if (f_eh == null)
            {
                logger(this, "Subscribe WQL_VALUE:Subscription event handler is null");
                return "";
            }

            Socket subSocket = null;

            logger(this, "Subscribe WQL_VALUE:Create new socket for subscription");

            if (!openConnection(ref subSocket)) return "";

            if (!send(ref subSocket, this.xmlTools.subscribeWQL_VALUE_XML(startNode, path))) return "";

            string msg = receive(ref subSocket);

            if (this.xmlTools.isSubscriptionConfirmed(msg))
            {
                //this.KP_ERROR_ID = ERR_Subscription_DONE;

                SubscriptionDescriptor info = new SubscriptionDescriptor();
#if KPI_CF
                //syncParam.WaitOne();
                threadParam = info;
#endif
                string subID = this.xmlTools.getSubscriptionID(msg);
                info.subID = subID;

                info.type = SUBSCRITPION_TYPE.WQL;
                info.subSocket = subSocket;
#if KPI_CF
                // NOT SUPPORTED IN .NET CF 3.5
#else
                info.subSocket.ReceiveTimeout = -1;
#endif
                info.subHandler = f_eh;
#if KPI_CF
                // NOT SUPPORTED IN .NET CF 3.5               
                info.subThread = new Thread(new ThreadStart(run));
#else
                info.subThread = new Thread(new ParameterizedThreadStart(run));
#endif

                activeSubscribtions.Add(subID, info);

#if KPI_CF
                // NOT SUPPORTED IN .NET CF 3.5
                info.subThread.Start();
#else
                info.subThread.Start(info);
#endif
                return subID;
            }
            else
            {
                //this.KP_ERROR_ID = ERR_Subscription_NOT_DONE;
            }

            return "";
        }

        #endregion

        #region SUBSCRIPTIONS

        //Active subscriptions
        private Hashtable activeSubscribtions = new Hashtable();

        //Data structure for subscriptions management
        private enum SUBSCRITPION_TYPE { RDF, WQL, SPARQL };
        private class SubscriptionDescriptor
        {
            public string subID = "";
            public Socket subSocket = null;
            public iKPIC_subscribeHandler subHandler = null;
            public Thread subThread = null;
            public SUBSCRITPION_TYPE type = SUBSCRITPION_TYPE.RDF;
        }

#if KPI_CF
        // NOT SUPPORTED IN .NET CF 3.5
        SubscriptionDescriptor threadParam;

        private void run()
        {
            //Retrieve subscription parameters
            SubscriptionDescriptor descr = new SubscriptionDescriptor();
            descr.subHandler = threadParam.subHandler;
            descr.subID = threadParam.subID;
            descr.subSocket = threadParam.subSocket;
            descr.subThread = threadParam.subThread;
            descr.type = threadParam.type;

            //Wait to receive a notification
            string msg_event = "";
            byte[] byteReceived = new byte[1024];
            int len;

            while (true)
            {
                //Receive from socket or throw Timeout exception
                //Store received buffer into msg_event string
                try
                {
                    if (descr.subSocket != null)
                    {
                        if (!descr.subSocket.Connected) {
                            logger(this, "KPI:SubscriptionThread:Socket has been closed...exit");
                            //string[] triple={"any","any","any","uri"};
                            //ArrayList graph = new ArrayList();
                            //graph.Add(triple);
                            //descr.subID = subscribeRDF(graph, descr.subHandler);
                            descr.subHandler.kpic_SIBEventHandler(null, null, descr.subID);
                            return; 
                        }
                        if (descr.subSocket.Available == 0)
                        {
                            logger(this, "KPI:SubscriptionThread:No data received...wait");
                            Thread.Sleep(250);
                            continue;
                        }
                    }
                    else
                    {
                        logger(this, "KPI:SubscriptionThread:Socket is null...exit");
                        descr.subHandler.kpic_SIBEventHandler(null, null, descr.subID);
                        return;
                    }

                        len = descr.subSocket.Receive(byteReceived);
                }
                catch (Exception e)
                {
                    logger(this, "KPI:SubscriptionThread " + descr.subID + " exception (" + e.Message + ")...exit");
                    return;
                }

                msg_event += System.Text.Encoding.ASCII.GetString(byteReceived, 0, len);

                //Wait to receive a complete SSAP message
                if (msg_event.StartsWith("<SSAP_message>") && msg_event.EndsWith("</SSAP_message>"))
                {
                    logger(this, "EventHandlerThread:New notification");

                    ArrayList newResults = null;
                    ArrayList obsoleteResult = null;

                    if (descr.type == SUBSCRITPION_TYPE.RDF)
                    {
                        newResults = xmlTools.getNewResultsTripleFromRdfSubscribeIndication(msg_event);
                        obsoleteResult = xmlTools.getObsoleteResultsTripleFromRdfSubscribeIndication(msg_event);
                    }
                    else
                    {
                        newResults = xmlTools.getNewResultsNodeFromWqlValueSubscribeIndication(msg_event);
                        obsoleteResult = xmlTools.getObsoleteResultsNodeFromWqlValueSubscribeIndication(msg_event);
                    }

                    //Prepare to receive a new notification
                    msg_event = "";
                    //Send a notify
                    descr.subHandler.kpic_SIBEventHandler(newResults, obsoleteResult, descr.subID);
                }
            }
        }
#else
        private void run(object info){
            SubscriptionDescriptor descr = (SubscriptionDescriptor)info;

            //Wait to receive a notification
            string msg_event = "";
            byte[] byteReceived = new byte[1024];
            int len;
            
            while (true)
            {
                try
                {
                    if (descr.subSocket == null) return;

                    len = descr.subSocket.Receive(byteReceived);

                    msg_event += System.Text.Encoding.ASCII.GetString(byteReceived, 0, len);

                    if (msg_event.Contains("<SSAP_message>") && msg_event.Contains("</SSAP_message>"))
                    {

                        //UNSUBSCRIBE
                        if (msg_event.Contains("<transaction_type>UNSUBSCRIBE</transaction_type>"))
                        {
                            if (xmlTools.isUnSubscriptionConfirmed(msg_event))
                            {
                                //Close socket and exit                                
                                descr.subSocket.Close();
                                logger(this, "Unsubscription " + descr.subID + " confirmed (subscription socket closed)");
                                return;
                            }
                        }

                        logger(this, "Subscription " + descr.subID + " received (" + msg_event.Length.ToString() + " bytes)");

                        ArrayList newResults = null;
                        ArrayList obsoleteResult = null;

                        switch (descr.type) { 
                            case SUBSCRITPION_TYPE.RDF:
                                newResults = xmlTools.getNewResultsTripleFromRdfSubscribeIndication(msg_event);
                                obsoleteResult = xmlTools.getObsoleteResultsTripleFromRdfSubscribeIndication(msg_event);
                                descr.subHandler.kpic_SIBEventHandler(newResults, obsoleteResult, descr.subID);
                                break;
                            case SUBSCRITPION_TYPE.WQL:
                                newResults = xmlTools.getNewResultsNodeFromWqlValueSubscribeIndication(msg_event);
                                obsoleteResult = xmlTools.getObsoleteResultsNodeFromWqlValueSubscribeIndication(msg_event);
                                descr.subHandler.kpic_SIBEventHandler(newResults, obsoleteResult, descr.subID);
                                break;
                            case SUBSCRITPION_TYPE.SPARQL:
                                SPARQLResults newBindings = new SPARQLResults();
                                SPARQLResults oldBindings = new SPARQLResults();
                                newBindings = xmlTools.getNewSPARQLResults(msg_event);
                                oldBindings = xmlTools.getOldSPARQLResults(msg_event);
                                descr.subHandler.kpic_SIBEventHandlerSPARQL(newBindings, oldBindings, descr.subID);
                                break;
                        }

                        msg_event = "";
                        

                        logger(this, "Subscription ==> NOTIFY");
                    }
                    else
                    {
                        logger(this, "Subscription " + descr.subID + " received (" + len.ToString() + " bytes)");
                        if (len == 1 && msg_event[0] == ' ')
                        {
                            msg_event = "";
                            logger(this, "Subscription " + descr.subID + " PING (" + msg_event + ")");
                        }
                    }
                }
                catch (SocketException ex)
                {
                    logger(this, "Subscription " + descr.subID + " exception on Receive (buffer size " + byteReceived.Length.ToString() + " bytes)");
                    logger(this, "Thread " + descr.subThread.Name + " has exited");
                    return;
                }
            }
        }
#endif

        #endregion

        #region QNAME_SUPPORT

        /* Current SIB version does not support Qnames (e.g. foaf:person)
         * Because SIB allows to store triple where subject and predicate are not URIs 
         * the following "special" RDF statement (e.g. <http://www.w3.org/1999/02/22-rdf-syntax-ns,http://sibnamespaces/hasPrefix,"rdf">
         * can be used to store and retrieve namespaces
         * Default namespaces:
            prefix rdf:, namespace URI: http://www.w3.org/1999/02/22-rdf-syntax-ns#
            prefix rdfs:, namespace URI: http://www.w3.org/2000/01/rdf-schema#
            prefix dc:, namespace URI: http://purl.org/dc/elements/1.1/
            prefix owl:, namespace URI: http://www.w3.org/2002/07/owl#
            prefix xsd:, namespace URI: http://www.w3.org/2001/XMLSchema#
         */

        private static string sibNamespacesURI = "http://sibnamespaces/hasPrefix";

        public enum RDFStatementObjectType { URI, LITERAL };

        public class RDFStatement
        {
            private string _subject;
            private string _predicate;
            private string _object;
            private RDFStatementObjectType _type;

            public RDFStatement(string sub, string pred, string obj, RDFStatementObjectType type)
            {
                _subject = sub;
                _predicate = pred;
                _object = obj;
                _type = type;
            }

            public string getSubject() { return _subject; }
            public string getPredicate() { return _predicate; }
            public string getObject() { return _object; }
            public RDFStatementObjectType getType() { return _type; }
            public bool isObjectLiteral() { return _type == RDFStatementObjectType.LITERAL; }
        }

        private Hashtable sibNamespaces = new Hashtable();

        public int retrieveSibNamespaces() {
            logger(this, "Retrive SIB Namespaces@begin");
            ArrayList triples = queryRDF("any", sibNamespacesURI, "any", "uri");
            foreach(string[] triple in triples){
                logger(this, "Retrieved <rdf:RDF xmlns:" + triple[2] + "=" + triple[0] + ">");
                if (!sibNamespaces.Contains(triple[0])){                
                    sibNamespaces.Add(triple[0], triple[2]);
                }
            }
            logger(this, "Retrive SIB Namespaces@end");
            return sibNamespaces.Count;
        }
        public bool storeSibNamespace(string prefix, string suffix) {
            logger(this, "Store SIB namespaces@begin");
            int ret = retrieveSibNamespaces();

            if (sibNamespaces.Contains(prefix))
            {
                //Namespace to be updated
                logger(this, "Update <"+prefix+":"+(string)sibNamespaces[prefix]+"> with <"+prefix+":"+suffix+">");

                string[] oldTriple = new string[4];
                oldTriple[0] = (string)sibNamespaces[prefix];
                oldTriple[1] = sibNamespacesURI;
                oldTriple[2] = prefix;
                oldTriple[3] = "literal";
                
                string[] newTriple = new string[4];
                newTriple[0] = suffix;
                newTriple[1] = sibNamespacesURI;
                newTriple[2] = prefix;
                newTriple[3] = "literal";

                if (update(newTriple[0], newTriple[1], newTriple[2], newTriple[3], oldTriple[0], oldTriple[1], oldTriple[2], oldTriple[3]))
                {
                    sibNamespaces.Remove(suffix);
                    sibNamespaces.Add(suffix, prefix);
                    logger(this, "Store SIB namespaces@end");
                    return true;
                }
            }
            else { 
                //New namespace to be added
                logger(this, "Add <" + prefix + ":" + suffix + ">");

                string[] triple = new string[4];
                triple[0] = suffix;
                triple[1] = sibNamespacesURI;
                triple[2] = prefix;
                triple[3] = "literal";
                if (insert(triple[0], triple[1], triple[2], triple[3]))
                {
                    sibNamespaces.Remove(prefix);
                    sibNamespaces.Add(prefix, suffix);
                    logger(this, "Store SIB namespaces@end");
                    return true;
                }
            }

            logger(this, "Store SIB namespaces@end");
            return false;
        }
        public string getNamespace(string prefix) {
            foreach (string key in sibNamespaces.Keys) if (sibNamespaces[key].ToString() == prefix) return key;
            return "";
        }
        public ArrayList getAllPrefixes() {
            ArrayList ret = new ArrayList();
            foreach (string ns in sibNamespaces.Keys) {
                ret.Add(sibNamespaces[ns]);
            }
            return ret;
        }
        public bool resolveQNames(string qName,ref string URI) {
            if (qName.CompareTo("any") == 0)
            {
                URI = qName;
                return true;
            }
            string[] prefix = qName.Split(':');
            foreach (string ns in sibNamespaces.Keys) {
                if (((string)sibNamespaces[ns]) == prefix[0]) {
                    URI = ns + prefix[1];
                    return true;
                }
            }

            return false;
        }
        public bool reduceToQNames(string URI, ref string qName) {
            if (URI.CompareTo("any") == 0)
            {
                qName = URI;
                return true;
            }
            string suffix = "";
            string nameSpace = "";
            if (URI.Contains("#")) {
                nameSpace = URI.Substring(0, URI.IndexOf('#')+1);
                suffix = URI.Substring(URI.IndexOf('#') + 1);
            }
            else{
                nameSpace = URI.Substring(0, URI.LastIndexOf('/') + 1);
                suffix = URI.Substring(URI.LastIndexOf('/') + 1);
            }

            if (sibNamespaces.Contains(nameSpace)) {
                qName = sibNamespaces[nameSpace] + ":" + suffix;
                return true;
            }

            return false;

        }

        public bool insert(RDFStatement triple) {
            string sub ="", pred ="", obj= "";
            resolveQNames(triple.getSubject(), ref sub);
            resolveQNames(triple.getPredicate(), ref pred);
            if (!triple.isObjectLiteral()) resolveQNames(triple.getObject(), ref obj);
            else obj = triple.getObject();

            return insert(sub, pred, obj, triple.isObjectLiteral() ? "literal" : "uri");
        }

        public bool remove(RDFStatement triple)
        {
            string sub ="", pred="", obj="";
            resolveQNames(triple.getSubject(), ref sub);
            resolveQNames(triple.getPredicate(), ref pred);
            if (!triple.isObjectLiteral()) resolveQNames(triple.getObject(), ref obj);
            else obj = triple.getObject();

            return remove(sub, pred, obj, triple.isObjectLiteral() ? "literal" : "uri");
        }

        public bool update(RDFStatement newTriple,RDFStatement oldTriple)
        {
            string sub="", pred="", obj="";
            resolveQNames(newTriple.getSubject(), ref sub);
            resolveQNames(newTriple.getPredicate(), ref pred);
            if (!newTriple.isObjectLiteral()) resolveQNames(newTriple.getObject(), ref obj);
            else obj = newTriple.getObject();

            string sub1="", pred1="", obj1="";
            resolveQNames(oldTriple.getSubject(), ref sub1);
            resolveQNames(oldTriple.getPredicate(), ref pred1);
            if (!oldTriple.isObjectLiteral()) resolveQNames(oldTriple.getObject(), ref obj1);
            else obj1 = oldTriple.getObject();

            return update(sub, pred, obj, newTriple.isObjectLiteral() ? "literal" : "uri", sub1, pred1, obj1, oldTriple.isObjectLiteral() ? "literal" : "uri");
        }

        public ArrayList query(RDFStatement triple) {
            string sub="", pred="", obj="";
            resolveQNames(triple.getSubject(), ref sub);
            resolveQNames(triple.getPredicate(), ref pred);
            if (!triple.isObjectLiteral()) resolveQNames(triple.getObject(), ref obj);
            else obj = triple.getObject();

            ArrayList result = queryRDF(sub, pred, obj, triple.isObjectLiteral() ? "literal" : "uri");

            ArrayList resultAsQNames = new ArrayList();

            foreach (string[] tuple in result) {
                reduceToQNames(tuple[0], ref sub);
                reduceToQNames(tuple[1], ref pred);
                if (tuple[3].CompareTo("uri") == 0) reduceToQNames(tuple[2], ref obj);
                else obj = tuple[2];
                RDFStatementObjectType type = (tuple[3].CompareTo("literal") == 0) ? RDFStatementObjectType.LITERAL : RDFStatementObjectType.URI;
                resultAsQNames.Add(new RDFStatement(sub,pred,obj,type));
            }

            return resultAsQNames;

        }

        #endregion

        #region SPARQL

        public SPARQLResults getSPARQLResults(string query)
        {
            logger(this, "[QUERY SPARQL]");
            string responce = sendSSAPMsg(this.xmlTools.querySPARQL(query));
            if (xmlTools.isQueryConfirmed(responce)) return xmlTools.getSPARQLResults(responce);
            return null;

        }
        public string querySPARQL(string query)
        {
            logger(this, "[QUERY SPARQL]");
            string responce = sendSSAPMsg(this.xmlTools.querySPARQL(query));
            if (xmlTools.isQueryConfirmed(responce)) return xmlTools.getSPARQLXMLResult(responce);
            return null;
        }

        #endregion
    }
}

