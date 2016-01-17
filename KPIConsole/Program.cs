using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using KPICore;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace KPIConsole
{
    class Program : iKPIC_subscribeHandler
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Starting agent!");

            Program p = new Program();
            p.Start();

            p.InsertData();

            Console.ReadKey();


        }

        public KPICore.KPICore core;

        public Program()
        {

        }


        public void kpic_SIBEventHandler(System.Collections.ArrayList newResults, System.Collections.ArrayList obsoleteResults, string subID)
        {
            foreach (string[] triple in newResults)
            {
                bool correction = triple[2] == "1";
                Console.WriteLine("Correction is : {0}", correction);
            }    
        }

        public void kpic_SIBEventHandlerSPARQL(SPARQLResults newResults, SPARQLResults obsoleteResults, string subID)
        {
         
        }

        public string Get(string whaaaat)
        {
            Console.Write("{0}>", whaaaat.ToUpper());
            return Console.ReadLine();
        }

        public void ClearStudentSpace(string studentUri)
        {
            foreach (string[] triple in core.queryRDF("any", "uploadedByStudent", studentUri, "uri"))
            {
                core.remove(triple[0], "any", "any", "uri");
                core.remove(triple[0], "any", "any", "literal");
            }
            core.remove(studentUri, "any", "any", "uri");
            core.remove(studentUri, "any", "any", "literal");
        }

        public void InsertData()
        {
            string student = Get("student");
            int variant = int.Parse(Get("variant"));
            string filename;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "csv user-item-rate files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    filename = openFileDialog1.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                    return;
                }
            }
            else
            {
                return;
            }            

            if (!File.Exists(filename))
            {
                Console.WriteLine("Answer file `{0}` not found", filename);
                return;
            }
            

            string studentUri = student;

            core.subscribeRDF(studentUri, "isCorrect", "any", "literal", this);

            ArrayList triples = new ArrayList();
            string[] lines = File.ReadAllLines(filename);



            //core.remove("any", "uploadedByStudent", studentUri, "uri");
            ClearStudentSpace(studentUri);

            int markNo = 1;
            foreach (string line in lines)
            {
                string markUri = String.Format("mark-{0}", markNo);

                string[] parts = line.Split(',');
                string user_ = parts[0],
                    item_ = parts[1],
                    rate_ = parts[2];

                {
                    string[] triple = new string[4];
                    triple[0] = markUri;
                    triple[1] = "hasRate";
                    triple[2] = rate_;
                    triple[3] = "literal";
                    triples.Add(triple);
                }


                {
                    string[] triple = new string[4];
                    triple[0] = markUri;
                    triple[1] = "hasUser";
                    triple[2] = user_;
                    triple[3] = "literal";
                    triples.Add(triple);
                }

                {
                    string[] triple = new string[4];
                    triple[0] = markUri;
                    triple[1] = "hasItem";
                    triple[2] = item_;
                    triple[3] = "literal";
                    triples.Add(triple);
                }

                {
                    string[] triple = new string[4];
                    triple[0] = markUri;
                    triple[1] = "uploadedByStudent";
                    triple[2] = studentUri;
                    triple[3] = "uri";
                    triples.Add(triple);
                }

                markNo++;
            }

            core.insert(triples);


            core.insert(student, "hasVariant", variant.ToString(), "literal");

        }


        public class ConnectionConfig
        {
            public string Host { get; set; }
            public int Port { get; set; }
            [YamlAlias("space-name")]
            public string SmartspaceName { get; set; }
        }

        public bool LoadYaml(out string host, out int port, out string smartspaceName)
        {
            try
            {
                using (var reader = File.OpenText("connection.yaml"))
                {
                    var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention());
                    ConnectionConfig config = deserializer.Deserialize<ConnectionConfig>(reader);

                    host = config.Host;
                    port = config.Port;
                    smartspaceName = config.SmartspaceName;
                    return true;

                }
            }
            catch (Exception e)
            {
                host = "";
                port = 0;
                smartspaceName = "";
                return false;
            }
        }



        public void Start()
        {
            string host, smartSpaceName, portString;
            int port;

            if (LoadYaml(out host, out port, out smartSpaceName))
            {
                Console.WriteLine("Loaded from YAML config: host = {0}, port = {1}, smart-space-name = {2}", host, port, smartSpaceName);
            }
            else
            {
                Console.WriteLine("Cannot load config. Please provide config from prompt");

                Console.Write("HOST>");
                host = Console.ReadLine();
                Console.Write("PORT>");
                portString = Console.ReadLine();
                Console.Write("SMART SPACE NAME>");
                smartSpaceName = Console.ReadLine();
                if (!int.TryParse(portString, out port))
                {
                    Console.Error.WriteLine("Incorrect port number");
                    return;
                }
            }

            core = new KPICore.KPICore(host, port, smartSpaceName);

            if (!core.join())
            {
                Console.Error.WriteLine("Cannot join smart space. Host: {0}, port: {1}, smart-space-name: {2}", host, port, smartSpaceName);
                return;
            }
        }
    }
}
