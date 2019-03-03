using System;
using System.Diagnostics;
using System.IO;

namespace ExstreamPackager
{
    class Content
    {
        public string oldPubPath;
        public string newPubPath;
        public string ContentStatus;
        public string pubnamewithoutext;

        public string movePubContents(string pubPath, string pubName, string packagerPath, string state)
        {
            oldPubPath = pubPath + pubName;
            string contentresult;
            pubnamewithoutext = pubName.Replace(".pub", "");
            pubnamewithoutext = state + pubnamewithoutext;
            try
            {
                string targetPath = Directory.GetParent(Directory.GetParent(pubPath).ToString()).ToString();
                targetPath = Path.Combine(targetPath, "Contents");
                if (!File.Exists(oldPubPath))
                {
                    ContentStatus = "Pub file NOT available";
                    return ContentStatus;
                }
                else
                {
                    if (Directory.Exists(Path.Combine(targetPath)))
                    {
                        newPubPath = Path.Combine(targetPath, state + pubName);
                        File.Copy(oldPubPath, newPubPath, true);
                        contentresult = pubContents(packagerPath, targetPath, state + pubName);
                        Console.WriteLine(state + "contents pulled");

                        //File.Copy(Path.Combine(packagerPath,"ExstreamReport.dat"), Path.Combine(targetPath, state + pubnamewithoutext + "_Content.dat"), true);
                        return contentresult;
                    }
                    else
                    {
                        Directory.CreateDirectory(targetPath);
                        newPubPath = Path.Combine(targetPath, state + pubName);
                        File.Copy(oldPubPath, newPubPath, true);
                        contentresult = pubContents(packagerPath, targetPath, state + pubName);
                        Console.WriteLine(state + "contents pulled");
                        //File.Copy(Path.Combine(packagerPath, "ExstreamReport.dat"), Path.Combine(targetPath, state + pubnamewithoutext + "_Content.dat"), true);
                        return contentresult;
                    }

                }
            }
            catch (Exception ex)
            {
                ContentStatus = ex.ToString();
                System.Console.WriteLine(ex);
                return "Exception" + ContentStatus;
            }
        }
        public string pubContents(string enginePath, string pubPath, string pubName)
        {
            int result = 1;
            string outmsg;
            try
            {
                Console.WriteLine("Pulling content for" + pubName);
                string controlLines = "-PACKAGEFILE=" + Path.Combine(pubPath, pubName) + " " + "-RUN=CONTENT" + " " + "-REPORTFILE=" + Path.Combine(pubPath, pubnamewithoutext + "_content.dat") + " " + "-MESSAGEFILE=" + Path.Combine(pubPath, pubnamewithoutext + "_content_msg.dat");

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.WorkingDirectory = Path.GetDirectoryName(enginePath);
                psi.CreateNoWindow = true;
                psi.FileName = enginePath + "Engine.exe";
                psi.WindowStyle = ProcessWindowStyle.Normal; //Hidden
                psi.Arguments = controlLines;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                Process exeProcess = Process.Start(psi);
                // exeProcess.BeginErrorReadLine();
                exeProcess.WaitForExit();
                result = exeProcess.ExitCode;
                outmsg = exeProcess.StandardOutput.ReadToEnd();
                //exeProcess.
                //Console.WriteLine("Contents file cretated with {0} for {1}" + Path.Combine(pubPath, pubnamewithoutext + "_content.dat"), pubName);
                if (result != 12)
                {
                    return Path.Combine(pubPath, pubnamewithoutext + "_content.dat");
                }
                else
                {
                    return "Error while pulling the contents";
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(DateTime.Now.ToString() + ":Packaging execution failed");
                System.Console.WriteLine(ex.InnerException.ToString());
                return "Exception at contents" + ex;
            }
            finally
            {
                //System.Console.Write(DateTime.Now.ToString() + " : " + Regex.Replace(outmsg, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline));

            }


        }
    }
}
