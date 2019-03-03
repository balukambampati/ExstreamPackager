using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace ExstreamPackager
{
    class Comparison
    {

        public void compareFiles(string oldpath, string newpath)
        {

            string oldReportpath = Directory.GetParent(oldpath).ToString();
            string newReportPath = Directory.GetParent(newpath).ToString();
            //starting position followed by length
            int[] positions = {2, 12, 16, 56, 75, 10, 87, 19};
            PubContents pc = new PubContents();
            List<string> oldDiffContents = new List<string>();
            List<string> newDiffContents = new List<string>();
            var oldDataTable = new DataTable("OldPubContent");
            var newDataTable = new DataTable("NewPubContent");
            var oldDataTable_diff = new DataTable("OldPubContent_Diff");
            var newDataTable_diff = new DataTable("NewPubContent_Diff");
            oldDataTable_diff.Columns.Add("Compare report", typeof(System.String));
            newDataTable_diff.Columns.Add("Compare report", typeof(System.String));
            //string subString = "";
            bool isHeader = false;


            try
            {
                if (File.Exists(oldpath) && File.Exists(newpath))
                {
                    var oldPubContent = File.ReadAllLines(oldpath);
                    var newPubContent = File.ReadAllLines(newpath);
                    var tempContent = File.ReadAllLines(oldpath);
                    var oldMatchContent = new List<string>();
                    var newMatchContent = new List<string>();
                    var headerListContent = new List<string>();


                    foreach (string line in oldPubContent)
                    {
                        //header rows old pub content
                        if (line.Length <= 80)
                        {
                            headerListContent.Add(line);
                            isHeader = true;
                            continue;
                        }
                        else
                        {

                            if (!newPubContent.Any(m => m.Equals(line)))
                            {
                                //oldDiffContents.Add(line);
                                oldDataTable_diff.NewRow();
                                oldDataTable_diff.Rows.Add(line);
                                oldDiffContents.Add(line.Substring(positions[0], positions[4] - 3));
                            }
                            else
                            {
                                oldMatchContent.Add(line);
                            }
                        }
                    }
                    foreach (string line in newPubContent)
                    {

                        //header rows new pub content
                        if (line.Length <= 80)
                        {
                            headerListContent.Add(line);
                            isHeader = true;
                            continue;
                        }
                        else
                        {
                            if (!oldPubContent.Any(m => m.Equals(line)))
                            {
                                newDataTable_diff.NewRow();
                                newDataTable_diff.Rows.Add(line);
                                newDiffContents.Add(line.Substring(positions[0], positions[4] - 3));
                            }
                            else
                            {
                                newMatchContent.Add(line);
                            }

                        }
                    }



                    if (oldDataTable_diff.Select().Length > 1 || newDataTable_diff.Select().Length > 1)
                    {
                        StreamWriter streamWriter = new StreamWriter(Path.Combine(oldReportpath, "PubCompareReport.dat"));
                        streamWriter.WriteLine("Header rows: ");
                        foreach (string item in headerListContent)
                        {
                            streamWriter.WriteLine(item.ToString());
                        }

                        if (oldDataTable_diff.Select().Length > 1)
                        {
                            streamWriter.WriteLine("Objects  removed in the New Pub against Baseline are: ");
                            foreach (DataRow row in oldDataTable_diff.Rows)
                            {
                                foreach (DataColumn column in oldDataTable_diff.Columns)
                                {
                                    streamWriter.WriteLine(row[column].ToString());
                                }
                            }
                        }
                        if (newDataTable_diff.Select().Length > 1)
                        {
                            streamWriter.WriteLine("Objects are recently modified and present in the new Pub are: ");
                            foreach (DataRow row in newDataTable_diff.Rows)
                            {
                                foreach (DataColumn column in newDataTable_diff.Columns)
                                {
                                    if(oldDiffContents.Any(word => row[column].ToString().Contains(word))) { 
                                        
                                            streamWriter.WriteLine(row[column].ToString());
                                            Console.WriteLine(row[column].ToString());
                                    }
                                }

                            }
                        }
                        if (newDataTable_diff.Select().Length > 1)
                        {

                            streamWriter.WriteLine("Objects are added in the new Pub are: ");
                            Console.WriteLine("Objects are added in the new Pub are: ");
                            foreach (DataRow row in newDataTable_diff.Rows)
                            {
                                foreach (DataColumn column in newDataTable_diff.Columns)
                                {
                                    //for loop list
                                    if (!oldDiffContents.Any(word => row[column].ToString().Contains(word)))
                                    {
                                        streamWriter.WriteLine(row[column].ToString());
                                            Console.WriteLine(row[column].ToString());
                                      }
                                }

                            }
                        }
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                    Console.WriteLine("Exist from Comparison");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

    }
}
