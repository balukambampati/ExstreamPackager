using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ExstreamPackager
{
    public class Program
    {
        static int Main(string[] args)
        {
            int returncode=1;
            int PubbingStatus = 0;
            string newPubContentStatus;
            string oldPubContentStatus;
            Pubbing pubbing = new Pubbing();
            Content pubContent = new Content();
            Comparison cp = new Comparison();


            if (args.Length < 7)
            {
                System.Console.WriteLine("Pass valid 7 arguments");
                System.Console.WriteLine("Packaging aborted with return code 1");
                return 1;
            }
            
            if (!string.IsNullOrEmpty(args[0]))
            {
                pubbing.appName = args[0];
            }
            if (!string.IsNullOrEmpty(args[1]))
            {
                pubbing.pubPath = args[1];
            }
            if (!string.IsNullOrEmpty(args[2]))
            {
                pubbing.cntrlPath = args[2];
            }
            if (!string.IsNullOrEmpty(args[3]))
            {
                pubbing.approvalState = args[3];
            }
            if (!string.IsNullOrEmpty(args[4]))
            {
                pubbing.exstreamUser = args[4];
            }
            if (!string.IsNullOrEmpty(args[5]))
            {
                pubbing.exstreamVersion = args[5];
            }
            if (!string.IsNullOrEmpty(args[6]))
            {
                pubbing.pubName = args[6];
            }
            string[] confValues = ConfigurationManager.AppSettings.Get(pubbing.exstreamVersion).Split(',');
            pubbing.packagerPath = confValues[0];
            pubbing.exstreamDSN = confValues[1];
            Console.WriteLine("*******************************************"+DateTime.Now+"*******************************************");
            Console.WriteLine("*******************Pulling Baseline contents*******************************");
            oldPubContentStatus = pubContent.movePubContents(pubbing.pubPath, pubbing.pubName, pubbing.packagerPath, "Baseline_");
            Console.WriteLine("Pubbing is in progress");
            //PubbingStatus = pubbing.startPubbing();
            Console.WriteLine("Pulling New pub contents");
            newPubContentStatus = pubContent.movePubContents(pubbing.pubPath, pubbing.pubName, pubbing.packagerPath, "New_");
            //pubbing.readMessagefile();
            cp.compareFiles(oldPubContentStatus, newPubContentStatus); 
            return PubbingStatus;
        }
    }
}