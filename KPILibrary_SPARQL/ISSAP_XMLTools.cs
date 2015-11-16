/* 
 * ISSAP_XMLTools
 * Interface for the Smart-M3 SSAP protocol implementation
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace KPICore
{
    public interface ISSAP_XMLTools
    {
        //*****************************************************************
        //Notes
        //Note1:
        //Each RDF triple is represented using a string[4] datatype, where:
        //string[0] = subject
        //string[1] = predicate
        //string[2] = object
        //string[3] = object type ["uri"|"literal"]
        
        //Note2:
        //If not explicity expressed, each element of an ArrayList is an RDF triple as reported in Note1 
        //*****************************************************************

        //********************
        //Logging SSAP events
        //********************
        event EventLogger __logger;

        //**********************
        //SSAP primitive: all these methods return an XML description of a SSAP message
        //**********************
        string join();
        string leave();

        string insert(ArrayList insert_graph);
        string update(ArrayList insert_graph, ArrayList remove_graph);
        string remove(ArrayList remove_graph);
        
        string subscribeRDF_XML(ArrayList triples);
        string subscribeWQL_VALUE_XML(string[] startNode, string path);
        string unsubscribeRDF(string subscriptionID);
        
        string queryRDFGraph(ArrayList graph);
        string querySPARQL(string query);

        //WQL Support (deprecated)
        string queryWQL_values(string startNode, string type, string path);
        string queryWQL_related(string startNode, string startNodeType, string endNode, string endNodeType, string path);
        string queryWQL_nodeTypes(string startURI);
        string queryWQL_isType(string instanceURI, string classURI);
        string queryWQL_isSubType(string subType, string superType);

        //************************
        //Check confirm message
        //************************
        bool isJoinConfirmed(string xml);
        bool isLeaveConfirmed(string xml);
        bool isInsertConfirmed(string xml);
        bool isUpdateConfirmed(string xml);
        bool isRemoveConfirmed(string xml);
        bool isQueryConfirmed(string xml);
        bool isSubscriptionConfirmed(string xml);
        bool isUnSubscriptionConfirmed(string xml);
        
        //***************************************
        //Get query results or subscription ID
        //***************************************
        string getSubscriptionID(string xml);
        bool getBoleanResultsFromWQLquery(string xml);

        ArrayList getQueryTriple(string xml);
        ArrayList getNewResultsTripleFromRdfSubscribeIndication(string xml);
        ArrayList getObsoleteResultsTripleFromRdfSubscribeIndication(string xml);
        
        //The return ArrayList contains elements as string[2] where:
        //string[0] = URI
        //string[1] = ["uri"|"literal"]
        ArrayList getNewResultsNodeFromWqlValueSubscribeIndication(string xml);
        ArrayList getObsoleteResultsNodeFromWqlValueSubscribeIndication(string xml);
        ArrayList getValuesQueryNodeList(string xml);

        string getSPARQLXMLResult(string xml);
    }
}
