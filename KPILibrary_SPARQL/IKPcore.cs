/* 
 * IKPCore
 * Interface for KP (Knowledge Processor) library
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
    
    
    //public interface IKPcore
    //{
    //    //Logging
    //    event EventLogger __logger;

    //    //****************
    //    //Join and leave
    //    //****************
    //    bool join();
    //    bool leave();

    //    //*****************************************************************
    //    //Notes
    //    //Note1:
    //    //Each RDF triple is represented using a string[4] datatype, where:
    //    //string[0] = subject
    //    //string[1] = predicate
    //    //string[2] = object
    //    //string[3] = object type ["uri"|"literal"]

    //    //Note2:
    //    //If not explicity expressed, each element of an ArrayList is an RDF triple as reported in Note1 
    //    //*****************************************************************

    //    //**************
    //    // Query
    //    //*************
    //    ArrayList queryRDF(string s, string p, string o, string o_type);
    //    ArrayList queryRDF(ArrayList graph);

    //    // In WQL queries the ArrayList and string[] contains "nodes" as string[2], where:
    //    // string[0] = [URI|literal]
    //    // string[1] = ["uri"|"literal"]
    //    ArrayList queryWQL_values(string[] startNode, string path);
    //    ArrayList queryWQL_nodeTypes(string startURI);   
    //    bool queryWQL_related(string[] startNode, string[] endNode, string path);
    //    bool queryWQL_isType(string instanceURI, string classURI);
    //    bool queryWQL_isSubType(string instanceURI, string classURI);
    //    string querySPARQL(string query);

    //    //*****************************
    //    // Insert, remove and update
    //    //*****************************
    //    bool insert(string s, string p, string o, string o_type);
    //    bool insert(ArrayList insert_graph);
    //    bool remove(string s, string p, string o, string o_type);
    //    bool remove(ArrayList remove_graph);
    //    bool update(string s_new, string p_new, string o_new, string o_new_type, string s_old, string p_old, string o_old, string o_old_type);
    //    bool update(ArrayList insert_graph, ArrayList remove_graph);
        
    //    //*****************
    //    // Subscriptions
    //    //*****************
    //    // The returned string represents the subscription ID to be used to call the unsubscribe
    //    string subscribeRDF(string s, string p, string o, string o_type, iKPIC_subscribeHandler f_eh);
    //    string subscribeRDF(ArrayList triples, iKPIC_subscribeHandler f_eh);
    //    string subscribeWQL_VALUE(string[] startNode, string path, iKPIC_subscribeHandler f_eh);
    //    bool unsubscribe(string subscriptionID);
    //    bool unsubscribeAll();
        
        ////The following methods should be deprecated because the subject is always a URI
        //bool insertTriple(string s, string p, string o, string s_type, string o_type);
        //ArrayList queryRDF(string s, string p, string o, string s_type, string o_type);
        //bool removeTriple(string s, string p, string o, string s_type, string o_type);
        //bool updateTriple(string sn, string pn, string on, string sn_type, string on_type, string so, string po, string oo, string so_type, string oo_type);
    //}
}
