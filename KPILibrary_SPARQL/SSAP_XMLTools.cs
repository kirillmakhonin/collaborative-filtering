/* 
 * SSAP_XMLTools
 * Implementation of the Smart-M3 SSAP protocol Version 1.0
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

//If defined the SSAP Message of saap_insert_graph is saved to the disk (debugging)
//#define SAVE_TO_DISK

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;

namespace KPICore
{
    public class SSAP_XMLTools:ISSAP_XMLTools 
    {
        //The default ID of a KP
        private string nodeID = "00000000-0000-0000-0000-000000000000";
        
        //The URI representing the "any" concept
        private string ANYURI = "http://www.nokia.com/NRC/M3/sib#any";
        
        //Default Smart Space name
        private string SMART_SPACE_NAME = "X";

        //Used to track each single messages
        private int transaction_id = 0;

        //Constants
        private const string ANY = "any";
        private const string WQL_VALUES = "WQL-VALUES";
        private const string WQL_RELATED = "WQL-RELATED";
        private const string WQL_NODETYPES = "WQL-NODETYPES";
        private const string WQL_ISTYPE = "WQL-ISTYPE";
        private const string WQL_ISSUBTYPE = "WQL-ISSUBTYPE";

        //Constructor
        public SSAP_XMLTools(string nodeID, string SMART_SPACE_NAME, string ANYURI)
        {
            this.nodeID = nodeID;
            this.SMART_SPACE_NAME = SMART_SPACE_NAME;
            this.ANYURI = ANYURI;
        }
        public SSAP_XMLTools(string nodeID, string SMART_SPACE_NAME)
        {
            this.nodeID = nodeID;
            this.SMART_SPACE_NAME = SMART_SPACE_NAME;
        }
        public SSAP_XMLTools(string nodeID)
        {
            this.nodeID = nodeID;
        }
        public SSAP_XMLTools()
        {
        }

        //Logging
        public event EventLogger __logger;       
        public void logger(object sender, string msg)
        {
            if (__logger == null) return;
            __logger(sender, "SSAP:"+msg);
        }

        //****************************************************************
        //SSAP PRIMITIVES REQUEST XML MESSAGES (Interface implementation)
        //****************************************************************
        public string join()
        {
            return ssap_message("JOIN", "");
        }
        public bool isJoinConfirmed(string xml)
        {
            return CheckMessageConfirm(xml, "JOIN");
        }

        public string leave()
        {
            return ssap_message("LEAVE", "");
        }
        public bool isLeaveConfirmed(string xml)
        {
            return CheckMessageConfirm(xml, "LEAVE");
        }

        public string insertOWL(string owl) {
            return ssap_message("INSERT", ssap_owl_insert(owl));
        }

        public string insert(ArrayList insert_graph)
        {
            return ssap_message("INSERT", ssap_insert_graph(insert_graph));
        }
        public bool isInsertConfirmed(string xml)
        {
            return CheckMessageConfirm(xml, "INSERT");
        }

        public string remove(ArrayList remove_graph)
        {
            return ssap_message("REMOVE", ssap_remove_graph(remove_graph));
        }
        public bool isRemoveConfirmed(string xml)
        {
            return CheckMessageConfirm(xml, "REMOVE");
        }

        public string update(ArrayList insert_graph, ArrayList remove_graph)
        {
            return ssap_message("UPDATE", ssap_update(insert_graph, remove_graph));
        }
        public bool isUpdateConfirmed(string xml)
        {
            return CheckMessageConfirm(xml, "UPDATE");
        }

        public string queryRDFGraph(ArrayList graph) 
        {
            return ssap_message("QUERY", ssap_RDF_query_graph(graph));
        }
        public bool isQueryConfirmed(string xml)
        {
            String[] id = { "transaction_type", "message_type" };
            String[] rif = { "QUERY", "CONFIRM" };
            return autoCheckSibMessage(xml, id, rif)
                && getParameterElement(xml, "status").InnerText.Equals("m3:Success");
        }

        public string subscribeRDF_XML(ArrayList triples)
        {
            return ssap_message("SUBSCRIBE", ssap_RDF_subscribe(triples));
        }
        public string unsubscribeRDF(string subscriptionID)
        {
            return "<SSAP_message><transaction_type>UNSUBSCRIBE</transaction_type>"
            + "<message_type>REQUEST</message_type>"
            + "<node_id>" + nodeID + "</node_id>"
            + "<space_id>" + SMART_SPACE_NAME + "</space_id>"
            + "<transaction_id>" + ++transaction_id + "</transaction_id>"
            + "<parameter name =\"subscription_id\">" + subscriptionID + "</parameter></SSAP_message>";

            //return ssap_message("UNSUBSCRIBE", ssap_RDF_unsubscribe(subscriptionID));
        }
        public bool isSubscriptionConfirmed(string xml)
        {
            return CheckMessageConfirm(xml, "SUBSCRIBE");
        }
        public bool isUnSubscriptionConfirmed(string xml)
        {
            return CheckMessageConfirm(xml, "UNSUBSCRIBE");
        }

        //SPARQL Subscription
        public string subscribeSPARQL(string query) {
            return ssap_message("SUBSCRIBE", ssap_sparql_query(preProcessSPARQLQuery(query)));
        }


        public string querySPARQL(string query)
        {
            return ssap_message("QUERY", ssap_sparql_query(preProcessSPARQLQuery(query)));
        }

        //WQL Support (deprecated)
        public string queryWQL_values(string startNode, string type, string path)
        {
            return ssap_message("QUERY", ssap_WQL_values(startNode, type, path));
        }
        public string queryWQL_related(string startNode, string startNodeType, string endNode, string endNodeType, string path)
        {
            return ssap_message("QUERY", ssap_WQL_related(startNode, startNodeType, endNode, endNodeType, path));
        }
        public string queryWQL_nodeTypes(string startURI)
        {
            return ssap_message("QUERY", ssap_WQL_nodetypes(startURI));
        }
        public string queryWQL_isType(string instanceURI, string classURI)
        {
            return ssap_message("QUERY", ssap_WQL_isType(instanceURI, classURI));
        }
        public string queryWQL_isSubType(string subType, string superType)
        {
            return ssap_message("QUERY", ssap_WQL_isSubType(subType, superType));
        }
        public string subscribeWQL_VALUE_XML(string[] startNode, string path)
        {
            return ssap_message("SUBSCRIBE", ssap_WQL_subscribe_values(startNode, path));
        }

        //*************************************************
        //SSAP PRIMITIVES CONFIRM XML MESSAGES GET RESULTS
        //*************************************************
        public ArrayList getValuesQueryNodeList(string xml)
        {
            return getNodes(getXMLParameter(xml, "results", "node_list"));
        }
        public ArrayList getNewResultsNodeFromWqlValueSubscribeIndication(string xml)
        {
            return getNodes(getXMLParameter(xml, "new_results", "node_list"));
        }
        public ArrayList getObsoleteResultsNodeFromWqlValueSubscribeIndication(string xml)
        {
            return getNodes(getXMLParameter(xml, "obsolete_results", "node_list"));
        }
        public ArrayList getNewResultsTripleFromRdfSubscribeIndication(string xml)
        {
            return getTriples(getXMLParameter(xml, "new_results", "triple_list"));
        }
        public ArrayList getObsoleteResultsTripleFromRdfSubscribeIndication(string xml)
        {
            return getTriples(getXMLParameter(xml, "obsolete_results", "triple_list"));
        }       
        public ArrayList getQueryTriple(string xml)
        {
            return getTriples(getXMLParameter(xml, "results", "triple_list"));
        }
        public string getSubscriptionID(string xml)
        {
            return getParameterElement(xml, "subscription_id").InnerText;
        }
        public bool getBoleanResultsFromWQLquery(string xml)
        {
            return (getParameterElement(xml, "results").InnerText == "TRUE" ? true : false);  
        }
        public string getSPARQLXMLResult(string xml)
        {
            int x = 0;
            if (xml == null)
            {
                logger(this, "getSPARQLXMLResult: XML message is null");
                return null;
            }

            if (xml.CompareTo("") == 0)
            {
                logger(this, "getSPARQLXMLResult: XML message is empty");
                return null;
            }

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(xml);
            }
            catch
            {
                logger(this, "getSPARQLXMLResult:Exception failed to load XML");
                return null;
            }

            if (doc == null)
            {
                logger(this, "getSPARQLXMLResult:doc is null");
                return null;
            }

            string result = "";

            XmlNodeList list = doc.GetElementsByTagName("boolean");
            if (list == null) return xml;
            if (list.Count != 0)
            {
                x = 1;
                foreach (XmlNode node in list)
                {
                    result += node.InnerText;
                }
            }

            list = doc.GetElementsByTagName("rdf:Description");
            if (list == null) return xml;
            if (list.Count != 0)
            {
                x = 1;
                foreach (XmlNode node in list)
                {
                    result += node.OuterXml + "\r\n";
                }
            }

            if (x == 0)
            {
                list = doc.GetElementsByTagName("head");

                if (list == null) return xml;
                if (list.Count == 0) return xml;

                result += list.Item(0).OuterXml + "\r\n\r\n";

                list = doc.GetElementsByTagName("result");

                if (list == null) return result;
                if (list.Count == 0) return result;

                foreach (XmlNode node in list)
                {
                    result += node.OuterXml + "\r\n";
                }
            }
            return result;

            //return xml;
        }
        public SPARQLResults getNewSPARQLResults(string xml) {
            if (xml == null)
            {
                logger(this, "getXMLParameter: XML message is null");
                return null;
            }

            if (xml.CompareTo("") == 0)
            {
                logger(this, "getXMLParameter: XML message is empty");
                return null;
            }

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(xml);
            }
            catch
            {
                logger(this, "getXMLParameter:Exception failed to load XML");
                return null;
            }

            if (doc == null)
            {
                logger(this, "getXMLParameter:doc is null");
                return null;
            }

            ArrayList output = new ArrayList();

            XmlElement root = doc.DocumentElement;

            XmlNodeList parameters = root.GetElementsByTagName("parameter");

            foreach (XmlNode parameter in parameters) {
                if (parameter.Attributes["name"].Value == "new_results")
                {
                    XmlDocument doc1 = new XmlDocument();
                    doc1.LoadXml(parameter.OuterXml);

                    return getSPARQLTag(doc1.DocumentElement);
                }
            }

            return null;
        }
        public SPARQLResults getOldSPARQLResults(string xml) {
            if (xml == null)
            {
                logger(this, "getXMLParameter: XML message is null");
                return null;
            }

            if (xml.CompareTo("") == 0)
            {
                logger(this, "getXMLParameter: XML message is empty");
                return null;
            }

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(xml);
            }
            catch
            {
                logger(this, "getXMLParameter:Exception failed to load XML");
                return null;
            }

            if (doc == null)
            {
                logger(this, "getXMLParameter:doc is null");
                return null;
            }

            ArrayList output = new ArrayList();

            XmlElement root = doc.DocumentElement;

            XmlNodeList parameters = root.GetElementsByTagName("parameter");

            foreach (XmlNode parameter in parameters)
            {
                if (parameter.Attributes["name"].Value == "obsolete_results")
                {
                    XmlDocument doc1 = new XmlDocument();
                    doc1.LoadXml(parameter.OuterXml);

                    return getSPARQLTag(doc1.DocumentElement);
                }
            }

            return null;   
        }
        private SPARQLResults getSPARQLTag(XmlElement root)
        {

            if (root == null) return null;

            //Get all the variables
            SPARQLResults results = new SPARQLResults();
            results.variables = new List<string>();
            XmlNodeList tagResults = root.GetElementsByTagName("sparql");
            if (tagResults == null) return null;
            if (tagResults.Count != 1) return null;
            results.xmlResponce = root.GetElementsByTagName("sparql")[0].OuterXml;

            XmlNodeList parameters = root.GetElementsByTagName("head");

            foreach (XmlElement p in parameters)
            {
                XmlNodeList var = p.GetElementsByTagName("variable");
                foreach (XmlElement v in var)
                {
                    results.variables.Add(v.GetAttribute("name"));
                }
                XmlNodeList link = p.GetElementsByTagName("link");
                foreach (XmlElement l in link)
                {
                    results.variables.Add(l.GetAttribute("href"));
                }
            }

            //Get all the results
            results.results = new List<SPARQLResults.SPARQLResult>();

            XmlNodeList res = root.GetElementsByTagName("result");
            if (res == null) return results;
            if (res.Count == 0) return results;

            foreach (XmlElement rt in res)
            {
                SPARQLResults.SPARQLResult result = new SPARQLResults.SPARQLResult();
                result.bindings = new List<SPARQLResults.SPARQLBinding>();
                results.results.Add(result);

                XmlNodeList bind = rt.GetElementsByTagName("binding");
                foreach (XmlElement b in bind)
                {
                    string nome = b.GetAttribute("name");
                    string tipo = b.FirstChild.Name;

                    SPARQLResults.SPARQLBinding binding = new SPARQLResults.SPARQLBinding();

                    if (tipo == "unbound") continue;
                    if (b.GetElementsByTagName(tipo)[0].FirstChild.InnerText == null) continue;

                    binding.name = nome;
                    binding.value = b.GetElementsByTagName(tipo)[0].FirstChild.Value;

                    switch (tipo)
                    {
                        case "uri":
                            binding.type = SPARQLResults.SPARQLValueType.URI;
                            break;
                        case "bnode":
                            binding.type = SPARQLResults.SPARQLValueType.BNODE;
                            break;
                        case "literal":
                            binding.type = SPARQLResults.SPARQLValueType.LITERAL;
                            break;
                    }

                    result.bindings.Add(binding);
                }
            }

            return results; 
        }
        public SPARQLResults getSPARQLResults(string xml)
        {
            if (xml == null)
            {
                logger(this, "getSPARQLResuts: XML message is null");
                return null;
            }

            if (xml.CompareTo("") == 0)
            {
                logger(this, "getSPARQLResuts: XML message is empty");
                return null;
            }

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(xml);
            }
            catch
            {
                logger(this, "getSPARQLResuts:Exception failed to load XML");
                return null;
            }

            if (doc == null)
            {
                logger(this, "getSPARQLResuts:doc is null");
                return null;
            }

            XmlElement root = doc.DocumentElement;

            return getSPARQLTag(root);  
        }

        //*********************************************
        //Utility functions to parse CONFIRM messages
        //*********************************************
        private bool CheckMessageConfirm(string xml, string type)
        {
            string[] id = { "transaction_type", "message_type" };
            string[] rif = { type, "CONFIRM" };

            if (xml == null)
            {
                logger(this, "CheckMessageConfirm: XML message is null");
                return false;
            }
            if (xml.CompareTo("") == 0)
            {
                logger(this, "CheckMessageConfirm: XML message is empty");
                return false;
            }

            bool ret1 = autoCheckSibMessage(xml, id, rif);
            bool ret2 = false;
            try
            {
                ret2 = getParameterElement(xml, "status").InnerText.Equals("m3:Success");
            }
            catch (XmlException e) {
                logger(this, "CheckMessageConfirm exception (" + e.Message + ")");
                return false;
            }
            return ret1 && ret2;
        }
        private bool autoCheckSibMessage(string xml, string[] id, string[] rif)
        {
            Hashtable hashtable = SibXMLMessageParser(xml);

            if (hashtable == null) return false;

            if (hashtable.Count < id.GetLength(0)) return false;

            for (int i = 0; i < id.GetLength(0); i++)
                if (!rif[i].Equals((String)hashtable[id[i]])) return false;

            return true;
        }
        private Hashtable SibXMLMessageParser(string xml)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(xml);
            }
            catch
            {
                logger(this, "SibXMLMessageParser: LoadXml exception");
                return null;
            }

            if (doc == null)
            {
                logger(this, "SibXMLMessageParser:DOM is null");
                return null;
            }

            Hashtable hashtable = new Hashtable();

            XmlElement root = doc.DocumentElement;
            if (root == null) return null;

            XmlNodeList list = root.ChildNodes;
            if (list == null) return null;

            foreach (XmlNode x in list)
            {
                if (!x.Name.Equals("parameter")) hashtable.Add(x.Name, x.InnerText);
            }

            return hashtable;
        }
        private XmlElement getXMLParameter(string xml, string value, string attribute)
        {
            if (xml == null)
            {
                logger(this, "getXMLParameter: XML message is null");
                return null;
            }

            if (xml.CompareTo("") == 0)
            {
                logger(this, "getXMLParameter: XML message is empty");
                return null;
            }

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(xml);
            }
            catch
            {
                logger(this, "getXMLParameter:Exception failed to load XML");
                return null;
            }

            if (doc == null)
            {
                logger(this, "getXMLParameter:doc is null");
                return null;
            }

            ArrayList output = new ArrayList();

            XmlElement root = doc.DocumentElement;
            XmlElement nodeList = null;

            XmlNodeList parameters = root.GetElementsByTagName("parameter");

            foreach (XmlElement ele in parameters)
            {
                XmlAttributeCollection coll = ele.Attributes;

                foreach (XmlAttribute y in coll)
                {
                    if (y.Value.Equals(value))
                    {
                        return (XmlElement)ele.SelectSingleNode(attribute);
                    }
                }
            }
            return nodeList;
        }
        private XmlElement getParameterElement(string xml, string attribute)
        {
            if (xml == null)
            {
                logger(this, "getParameterElement: XML message is null");
                return null;
            }

            if (xml.CompareTo("") == 0)
            {
                logger(this, "getParameterElement: XML message is empty");
                return null;
            }

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(xml);
            }
            catch
            {
                logger(this, "getParameterElement:Exception LoadXml");	 
            }

            if (doc == null)
            {
                logger(this, "getParameterElement doc is null");
                return null;
            }

            XmlElement output = null;

            XmlElement root = null;

            try
            {
                root = doc.DocumentElement;
            }
            catch (XmlException e) {
                logger(this, "getParameterElement exception ("+e.Message+")");
            }

            XmlNodeList list = null;

            try
            {
                list = root.GetElementsByTagName("parameter");
            }
            catch (XmlException e) {
                logger(this, "getParameterElement exception ("+e.Message+")");
            }
            
            foreach (XmlNode x in list)
            {
                XmlAttributeCollection coll = x.Attributes;

                foreach (XmlAttribute y in coll)
                {
                    if (y.Value.Equals(attribute)) return (XmlElement)x;
                }
            }

            return output;
        }
        private ArrayList getNodes(XmlElement nodeList)
        {
            if (nodeList == null) return null;

            ArrayList output = new ArrayList();

            try
            {
                XmlNodeList list = nodeList.GetElementsByTagName("uri");
                foreach (XmlNode n in list)
                {
                    string[] nodo = new string[2];
                    nodo[0] = n.InnerText;
                    nodo[1] = "uri";

                    output.Add(nodo);
                }
            }
            catch
            {
                logger(this, "GetNodes:Exception URI");
            }

            try
            {
                XmlNodeList list = nodeList.GetElementsByTagName("literal");

                foreach (XmlNode n in list)
                {
                    string[] nodo = new string[2];
                    nodo[0] = n.InnerText;
                    nodo[1] = "literal";

                    output.Add(nodo);
                }
            }
            catch
            {
                logger(this, "GetNodes:Exception literal");
            }

            return output;
        }
        private ArrayList getTriples(XmlElement tripleList)
        {
            if (tripleList == null) return null;

            ArrayList output = new ArrayList();
            XmlNodeList list;

            try
            {
                list = tripleList.GetElementsByTagName("triple");
            }
            catch (XmlException e) {
                logger(this, "GetTriples exception ("+e.Message+")");
                return output;
            }
            
            if (list == null) return output;

            foreach (XmlNode n in list)
            {
                string[] tripla = new string[4];
                tripla[0] = n.SelectSingleNode("subject").InnerText;
                tripla[1] = n.SelectSingleNode("predicate").InnerText;
                tripla[2] = n.SelectSingleNode("object").InnerText;
                tripla[3] = n.SelectSingleNode("object").Attributes.GetNamedItem("type").Value;

                output.Add(tripla);
            }

            return output;
        }
        private string preProcessSPARQLQuery(string query)
        {
            string t1 = query.Replace("&", "&amp;");
            string t2 = t1.Replace("<", "&lt;");
            string t3 = t2.Replace(">", "&gt;");
            string t4 = t3.Replace("\"", "&quot;");
            string t5 = t4.Replace("'", "&apos;");
            return t5;
        }

        //******************************************
        // LOW LEVEL SSAP REQUEST MESSAGE COMPOSERS
        //******************************************
        private string ssap_message(string transaction_type, string body)
        {
            string ssap = "<SSAP_message>"
                + "<node_id>" + nodeID + "</node_id>"
                + "<space_id>" + SMART_SPACE_NAME + "</space_id>"
                + "<transaction_type>" + transaction_type + "</transaction_type>" 
                + "<message_type>REQUEST</message_type>"
                + "<transaction_id>" + ++transaction_id + "</transaction_id>"
                + body
                + "<parameter name=\"confirm\">TRUE</parameter>"
                + "</SSAP_message>";

            return ssap;
        }
        private string ssap_join(string credentials)
        {
            return "<parameter name = \"credentials\">" + credentials + "</parameter>";
        }
        private string ssap_RDF_triple(string s, string p, string o, string o_type)
        {
            return "<triple>"
                + "<subject type=\"" + "uri" + "\">" + (s == ANY ? ANYURI : s) + "</subject>"
                + "<predicate>" + (p == ANY ? ANYURI : p) + "</predicate>"
                + "<object type=\"" + (o == ANY ? "uri" : o_type) + "\">" + (o == ANY ? ANYURI : (o_type == "uri" ? o : "<![CDATA[" + o + "]]>")) + "</object>"
                //+ "<object type=\"" + (o == ANY ? "uri" : o_type) + "\">" + (o == ANY ? ANYURI : (o_type == "uri" ? o : o)) + "</object>"
                + "</triple>";
        }

        private string ssap_insert_graph(ArrayList insert_graph)
        {
#if SAVE_TO_DISK
            //SAVE TO DISK...
            string insertMsg = "";
            if (System.IO.File.Exists("insert.xml")) System.IO.File.Delete("insert.xml");
            System.IO.StreamWriter temp = System.IO.File.CreateText("insert.xml");
            temp.Write("<parameter name=\"insert_graph\"  encoding=\"RDF-M3\"><triple_list>");
#else
            string insertMsg = "<parameter name=\"insert_graph\"  encoding=\"RDF-M3\">"
                + "<triple_list>";
#endif
            foreach (string[] triple in insert_graph)
            {
#if SAVE_TO_DISK
                temp.Write(ssap_RDF_triple(triple[0], triple[1], triple[2], triple[3]));
#else
                insertMsg += ssap_RDF_triple(triple[0], triple[1], triple[2], triple[3]);
#endif
            }
#if SAVE_TO_DISK
            temp.Write("</triple_list></parameter>");
            temp.Close();
            System.IO.StreamReader read = System.IO.File.OpenText("insert.xml");
            insertMsg = read.ReadToEnd();
            read.Close();
#else
            insertMsg += "</triple_list></parameter>";
#endif
            

            return insertMsg;
        }
        private string ssap_remove_graph(ArrayList remove_graph)
        {
            string removeMsg = "<parameter name=\"remove_graph\"  encoding=\"RDF-M3\">"
                + "<triple_list>";

            foreach (string[] triple in remove_graph)
            {
                removeMsg += ssap_RDF_triple(triple[0], triple[1], triple[2], triple[3]);
            }

            removeMsg += "</triple_list></parameter>";

            return removeMsg;
        }
        private string ssap_RDF_query_graph(ArrayList graph) {
            string ret = "<parameter name = \"type\">RDF-M3</parameter>"
                + "<parameter name = \"query\">"
                + "<triple_list>";

            foreach (string[] triple in graph) ret += ssap_RDF_triple(triple[0], triple[1], triple[2], triple[3]);

            ret += "</triple_list></parameter>";

            return ret;
        }
        private string ssap_RDF_subscribe(ArrayList triples) {
            string msg = "<parameter name = \"type\">RDF-M3</parameter>"
                   + "<parameter name = \"query\"><triple_list>";

            foreach (string[] triple in triples) {
                msg += ssap_RDF_triple(triple[0], triple[1], triple[2], triple[3]);
            }

            msg += "</triple_list></parameter>";

            return msg; 
        }
        private string ssap_RDF_unsubscribe(string subscriptionID)
        {
            return "<parameter name = \"subscription_id\">" + subscriptionID + "</parameter>";
        }
        private string ssap_update(ArrayList insert_graph, ArrayList remove_graph)
        {
            string updateMsg = "<parameter name=\"insert_graph\"  encoding=\"RDF-M3\">"
                + "<triple_list>";

            foreach (string[] triple in insert_graph)
            {
                updateMsg += ssap_RDF_triple(triple[0], triple[1], triple[2], triple[3]);
            }

            updateMsg += "</triple_list></parameter>"
            
            + "<parameter name = \"remove_graph\" encoding = \"" + "RDF-M3" + "\">"
            + "<triple_list>";

            foreach (string[] triple in remove_graph)
            {
                updateMsg += ssap_RDF_triple(triple[0], triple[1], triple[2], triple[3]);
            }

            updateMsg += "</triple_list></parameter>";

            return updateMsg;
        }

        private string ssap_sparql_query(string query)
        {
            return "<parameter name = \"type\">sparql</parameter>"
                + "<parameter name = \"query\">"
                + query
                + "</parameter>";
        }

        private string ssap_owl_insert(string owl)
        {
            return "<parameter name=\"insert_graph\" encoding=\"RDF-XML\">" + owl + "</parameter>";
        }

        //WQL Support (deprecated)
        private string ssap_WQL_values(string startNode, string type, string path)
        {
            return "<parameter name = \"type\">" + WQL_VALUES + "</parameter>"
                    + "<parameter name = \"query\">"
                    + "<wql_query>"
                    + "<node name=\"start\" type=\"" + type + "\" >" + startNode + "</node>"
                    + "<path_expression>" + path + "</path_expression>"
                    + "</wql_query>"
                    + "</parameter>";
        }
        private string ssap_WQL_related(string startNode, string startNodeType, string endNode, string endNodeType, string path) {
            return "<parameter name = \"type\">" + WQL_RELATED + "</parameter>"
                + "<parameter name = \"query\">"
                + "<wql_query>"
                + "<node name=\"start\" type=\"" + startNodeType + "\" >" + startNode + "</node>"
                + "<node name=\"end\" type=\"" + endNodeType + "\" >" + endNode + "</node>"
                + "<path_expression>" + path + "</path_expression>"
                + "</wql_query>"
                + "</parameter>";
        }
        private string ssap_WQL_nodetypes(string node)
        {
            return "<parameter name = \"type\">" + WQL_NODETYPES + "</parameter>"
                + "<parameter name = \"query\">"
                + "<wql_query>"
                + "<node>" + node + "</node>"
                + "</wql_query>"
                + "</parameter>";
        }
        private string ssap_WQL_isType(string instanceURI, string classURI)
        {
            return "<parameter name = \"type\">" + WQL_ISTYPE + "</parameter>"
                + "<parameter name = \"query\">"
                + "<wql_query>"
                + "<node>" + instanceURI + "</node>"
                + "<node name=\"type\">" + classURI + "</node>"
                + "</wql_query>"
                + "</parameter>";
        }
        private string ssap_WQL_isSubType(string subType, string superType)
        {
            return "<parameter name = \"type\">" + WQL_ISSUBTYPE + "</parameter>"
                + "<parameter name = \"query\">"
                + "<wql_query>"
                + "<node name=\"subtype\">" + subType + "</node>"
                + "<node name=\"supertype\">" + superType + "</node>"
                + "</wql_query>"
                + "</parameter>";
        }
        private string ssap_WQL_subscribe_values(string[] startNode, string path)
        {
            return "<parameter name = \"type\">" + WQL_VALUES + "</parameter>"
            + "<parameter name = \"query\">"
            + "<wql_query>"
            + "<node name=\"start\" type=\"" + startNode[1] + "\" >" + startNode[0] + "</node>"
            + "<path_expression>" + path + "</path_expression>"
            + "</wql_query>"
            + "</parameter>";
        }
    }
}
