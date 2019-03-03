using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace ExstreamPackager
{
    class Pubbing
    {
        public string appName { get; set; }
        public string pubPath { get; set; }
        public string cntrlPath { get; set; }
        public string approvalState { get; set; }
        public string exstreamUser { get; set; }
        public string exstreamVersion { get; set; }
        public string packagerPath { get; set; }
        public string exstreamDSN { get; set; }
        public string pubName { get; set; }
        public string pubbingState { get; set; }

        public int startPubbing()
        {
            int result = 0;
            string outmsg = "";

            switch (approvalState)
            {
                case "PEER":
                    pubbingState = "-VERSIONCUSTOM=PENDING PEER APPROVAL";
                    break;
                case "INTG":
                    pubbingState = "-VERSIONCUSTOM=PENDING INTG APPROVAL";
                    break;
                case "FNST":
                    pubbingState = "-VERSIONCUSTOM=PENDING FNST APPROVAL";
                    break;
                case "USER":
                    pubbingState = "-VERSIONCUSTOM=PENDING USER APPROVAL";
                    break;
                case "URC":
                    pubbingState = "-VERSIONCUSTOM=PENDING URC/LEGAL APPROVAL";
                    break;
                case "APPROVED":
                    pubbingState = "-WIP=APPROVED";
                    break;
                case "WIP":
                    pubbingState = "-WIP=WIP";
                    break;
                default:
                    System.Console.WriteLine("*******************************************" + DateTime.Now.ToString() + "*******************************************");
                    System.Console.WriteLine("Pass valid 'Approval State' argument");
                    System.Console.WriteLine("Packaging aborted with return code 1");
                    return 1;
            }
            System.Console.WriteLine("*******************************************" + DateTime.Now.ToString() + "*******************************************");
            System.Console.WriteLine(DateTime.Now.ToString() + ": " + appName + " packaging started with approval state: " + approvalState);
            try
            {
                string[] controlLines = { "-APPLICATION=" + appName, "-DSN=" + exstreamDSN, "-EXSTREAMUSER=" + exstreamUser, "-APPLICATION_MODE=SBCS", "-MESSAGEFILE=" + pubPath + appName + "_msg.dat", "-PACKAGEFILE=" + pubPath + pubName };
                File.WriteAllLines(cntrlPath + appName + ".dat", controlLines);
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.WorkingDirectory = Path.GetDirectoryName(packagerPath);
                psi.CreateNoWindow = true;
                psi.FileName = packagerPath + "Packager.exe";
                psi.WindowStyle = ProcessWindowStyle.Normal; //Hidden
                psi.Arguments = "\"" + "-CONTROLFILE=" + cntrlPath + appName + ".dat" + "\"" + " " + "\"" + pubbingState + "\"";
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                Process exeProcess = Process.Start(psi);
                exeProcess.WaitForExit();
                result = exeProcess.ExitCode;
                outmsg = exeProcess.StandardOutput.ReadToEnd();
                //exeProcess.
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(DateTime.Now.ToString() + ":Packaging execution failed");
                System.Console.WriteLine(ex.InnerException.ToString());
                return result;
            }
            finally
            {
                System.Console.Write(DateTime.Now.ToString() + " : " + Regex.Replace(outmsg, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline));
            }

            return result;
        }

        public void readMessagefile()
        {
            string line = "";
            int errCnt = 0;
            int warnCnt = 0;
            Boolean breakflag = false;
            string patE = @"EX\d{6}E";
            string patS = @"EX\d{6}S";
            string patW = @"EX\d{6}W";
            if (!File.Exists(pubPath + appName + "_msg.dat"))
            {
                System.Console.WriteLine(DateTime.Now.ToString() + ":Packaging failure - No message file");
            }
            else
            {
                Regex rE = new Regex(patE, RegexOptions.IgnoreCase);
                Regex rS = new Regex(patS, RegexOptions.IgnoreCase);
                Regex rW = new Regex(patW, RegexOptions.IgnoreCase);
                using (StreamReader file = new StreamReader(pubPath + appName + "_msg.dat"))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.Contains("Packaging complete"))
                        {
                            breakflag = true;
                        }
                        Match mE = rE.Match(line);
                        Match mS = rS.Match(line);
                        Match mW = rW.Match(line);

                        if (mS.Success)
                        {
                            System.Console.WriteLine(DateTime.Now.ToString() + ": Packaging failure with severe errors " + mS.ToString());

                        }
                        else if (mE.Success)
                        {
                            System.Console.WriteLine(DateTime.Now.ToString() + ": Packaging complete with errors " + mE.ToString());

                        }
                        else if (mW.Success)
                        {
                            warnCnt += 1;
                        }
                    }
                    if (!breakflag)
                    {
                        System.Console.WriteLine(DateTime.Now.ToString() + ": Packaging failure");
                    }
                    if (warnCnt > 0)
                    {
                        System.Console.WriteLine(DateTime.Now.ToString() + ": Packaging complete with warnings");
                    }
                }
            }
        }
    }
}
