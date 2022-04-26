﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Systems
{
    public enum RecordType
    {
        AKTVEJ = 001,
        BOLIG = 002,
        BYNAVN = 003,
        POSTVEJ = 004,
        NOTATVEJ = 005,
        BYFORNYDIST = 006,
    }

    public class RecordHandler
    {
        private Dictionary<string, int[]> RecordTypeDict = new Dictionary<string, int[]>();

        public RecordHandler()
        {
            RecordTypeDict.Add("ATKVEJ", POSTDISTArr);
            recordList.Add(tempRecordType);
            recordList.Add(tempKOMKOD);
            recordList.Add(tempvejkod);
            recordList.Add(temptimestamp);
            recordList.Add(temptilkomkod);
            recordList.Add(temptilvejkod);
            recordList.Add(tempfrakomkod);
            recordList.Add(tempfravejkod);
            recordList.Add(temphaenstart);
            recordList.Add(tempvejadrnvn);
            recordList.Add(tempvejadrnvn);
            ReadRecordFromFile();
        }

        private string tempRecordType = string.Empty;
        private string tempKOMKOD = string.Empty;
        private string tempvejkod = string.Empty;
        private string temptimestamp = string.Empty;
        private string temptilkomkod = string.Empty;
        private string temptilvejkod = string.Empty;
        private string tempfrakomkod = string.Empty;
        private string tempfravejkod = string.Empty;
        private string temphaenstart = string.Empty;
        private string tempvejadrnvn = string.Empty;
        private string tempvejnvn = string.Empty;



        List<string> recordList = new List<string>();

        int[] POSTDISTArr = new int[9] { 3, 4, 4, 4, 4, 1, 12, 4, 20 };

        private void ReadRecordFromFile()
        {
            string rootPath = Directory.GetCurrentDirectory();
            string currentLine = string.Empty;
            using (StreamReader sr = File.OpenText(rootPath + @"\tempRecords.txt"))
            {
                while ((currentLine = sr.ReadLine()) != null)
                {
                    switch (currentLine.Substring(0, 3))
                    {
                        case "001":
                            break;
                        case "002":
                            break;
                        case "003":
                            break;
                        case "004":
                            Debug.WriteLine(currentLine);
                            SpliceRecord(currentLine, POSTDISTArr);
                            break;
                        case "005":
                            break;
                        case "006":
                            break;
                        case "007":
                            break;
                        case "008":
                            break;
                        default:
                            break;
                    }
                }
                
            }
        }

        private void SpliceRecord(string currentRecord, int[] recordType)
        {
            int currentCol = 0;
            for (int i = 0; i < recordType.Length; i++)
            {
                if (currentRecord != null)
                {
                    recordList[i] = currentRecord.Substring(currentCol, recordType[i]);
                    currentCol += recordType[i];
                }
            }
            foreach (var item in recordList)
            {
                Debug.WriteLine(item);
            }
            Debug.WriteLine("\n\n");
        }
    }
}
