﻿using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf.parser;
using System.Linq;

namespace PDFComparator
{
    class Program
    {
        static void Main(string[] args)
        {
            string FirstPDF = "ICC16Old.pdf";
            string SecondPDF = "ICC16New.pdf";
            CompareTwoPDF(FirstPDF, SecondPDF);
        }

        private static void CompareTwoPDF(string FirstPDF, string SecondPDF)
        {
            PdfReader reader1 = new PdfReader(SecondPDF);
            PdfReader reader = new PdfReader(FirstPDF);
            string firstcopy = "First.pdf";
            string secondcopy = "Second.pdf";
            Dictionary<string, string> file1 = GetFormFieldValues(reader);
            Dictionary<string, string> file2 = GetFormFieldValues(reader1);
            
            Dictionary<string, string> mergeResult = MergeDictionary(file1, file2);
            Logger(GetFormFieldValues(reader), GetFormFieldValues(reader1), mergeResult);

            HighLighter(FirstPDF, firstcopy, reader, mergeResult);
            HighLighter(SecondPDF, secondcopy, reader1, mergeResult);

            
        }
        private static void HighLighter(string path, string copypath, PdfReader reader, Dictionary<string,string> merge)
        {
            using (MemoryStream pdfStream = new MemoryStream())
            {
                Document doc = new Document();
                PdfCopy copy = new PdfCopy(doc, new FileStream(copypath, FileMode.Create));
                PdfStamper stamp = new PdfStamper(reader, pdfStream);
                doc.Open();
                foreach (var item in merge)
                {
                    HighLightFields(item.Key, stamp);
                }
                copy.AddDocument(reader);
                doc.Close();
                stamp.FormFlattening = true;
                stamp.Close();    
                
                pdfStream.Flush();
                pdfStream.Close();
            }
        }
        private static Dictionary<string, string> GetFormFieldValues(PdfReader pdfReader)
        {
            AcroFields form = pdfReader.AcroFields;
            var dict = new Dictionary<string, string>();
            foreach(var item in form.Fields)
            {
                dict.Add(item.Key, form.GetField(item.Key));
            }
            return dict;
        }
        private static Dictionary<string, string> MergeDictionary(Dictionary<string, string> dict1, Dictionary<string, string> dict2)
        {
            var dict3 = new Dictionary<string, string>();

            foreach (var item in dict2)
            {
                if (!dict1.ContainsKey(item.Key))
                {
                    if (!dict3.ContainsKey(item.Key))
                    {
                        dict3.Add(item.Key, item.Value);
                    }
                }
                else
                {
                    dict1.TryGetValue(item.Key, out string value);
                    if (!item.Value.Equals(value))
                    {
                        if (!dict3.ContainsKey(item.Key))
                        {
                            dict3.Add(item.Key, item.Value);
                        }
                    }
                }
            }
            return dict3;
        }
        private static void HighLightFields(string field, PdfStamper stamp)
        {
            var fieldPositions = stamp.AcroFields.GetFieldPositions(field);
            if (fieldPositions == null)
                return;

            var positions = fieldPositions.ToArray();

            for (int i = 0; i < positions.Length; i++)
            {
                PdfContentByte contentByte = stamp.GetOverContent(positions[i].page);
                contentByte.SetRGBColorFill(100, 100, 100);
                contentByte.Fill();
            }
        }
        private static void Logger(Dictionary<string, string> file1, Dictionary<string, string> file2, Dictionary<string, string> mergeResult)
        {
            using (var file = new StreamWriter("Merge.json", false))
            {
                foreach (var item in mergeResult)
                {
                    file.WriteLine("\r\n" + " " + item.Value);
                }
            }
            using (var file = new StreamWriter("FirstPDF.json", false))
            {
                foreach (var item in file1)
                {
                    file.WriteLine("\r\n" + " " + item.Value);
                }
            }
            using (var file = new StreamWriter("SecondPDF.json", false))
            {
                foreach (var item in file2)
                {
                    file.WriteLine("\r\n" + " " + item.Value);
                }
            }
        }
    }
}
