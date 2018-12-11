using System;
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
        static string FirstFile, SecondFile;
        static void Main(string[] args)
        {
            CompareTwoPDF("ICC16Edit.pdf", "ICC16New.pdf");
            Console.ReadLine();
        }
        public static void CompareTwoPDF(string FirstPDF, string SecondPDF)
        {
            IsExist(FirstPDF, SecondPDF);

            List<string> File1diff;
            List<string> File2diff;

            IEnumerable<string> file1 = FirstFile.Trim().Split('\n', '\r');
            IEnumerable<string> file2 = SecondFile.Trim().Split('\n', '\r');

            File1diff = file1.ToList();
            File2diff = file2.ToList();

            if (file2.Count() > file1.Count())
            {
                Console.WriteLine("The FirstPDF has less number of lines than the SecondPDF.");

                IsContain(File1diff, File2diff);

                for (int i = File1diff.Count; i < File2diff.Count; i++)
                {
                    Console.WriteLine("SecondPDF extra content: " + File2diff[i]);
                }
            }
            else if (file2.Count() < file1.Count())
            {
                Console.WriteLine("The SecondPDF has less number of lines than FirstPDF.");

                IsContain(File2diff, File1diff);

                for (int i = File2diff.Count; i < File1diff.Count; i++)
                {
                    Console.WriteLine("The FirstPDF extra content: " + File1diff[i]);
                }
            }
            else
            {
                Console.WriteLine("The FirstPDF and the SecondPDF, both are having same number of lines.");
                IsContain(File1diff, File2diff);
            }
        }
        private static void IsContain(List<string> File1, List<string> File2)
        {
            List<string> differencies = new List<string>(){"\n"};
            if (!File1.Equals(File2)&& !File2.Equals(File1))
            {
                for (int i = 0; i < File1.Count; i++)
                {
                    if (!File1[i].Equals(File2[i]))
                    {
                        //Console.WriteLine("The FirstPDF content: " + File1[i].ToUpper() + "\n" + "The SecondPDF content: " + File2[i].ToUpper());
                        differencies.Add(File1[i] + "   ---   " + File2[i] +"\n");
                    }
                }
            }
            foreach(var item in differencies)
            {
                Console.WriteLine(item.ToString());
            }
        }
        private static void IsExist(String FirstPDF, String SecondPDF)
        {
            if (File.Exists(FirstPDF) && File.Exists(SecondPDF))
            {
                PdfReader reader = new PdfReader(FirstPDF);
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    FirstFile += PdfTextExtractor.GetTextFromPage(reader, page, strategy);
                }

                PdfReader reader1 = new PdfReader(SecondPDF);
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    SecondFile += PdfTextExtractor.GetTextFromPage(reader1, page, strategy);
                }
            }
            else
            {
                Console.WriteLine("Files does not exist.");
            } 
        }



        #region
        struct CurrentRes
        {
            public Dictionary<string, string> AcroFields { get; set; }
            public byte[] PdfData { get; set; }
        }
        struct BaseFields
        {
            public Dictionary<string, string> AcroFields { get; set; }
            public byte[] PdfData { get; set; }
        }
        public void MergePdfs()
        {
            CurrentRes Current = new CurrentRes();
            CurrentRes BaseFields = new CurrentRes();
            Dictionary<string, string> currentFields = Current.AcroFields;
            Dictionary<string, string> mergeResult = MergeCaseWithBaseLine(currentFields, BaseFields.AcroFields);

            if (mergeResult.Count > 0)
            {
                PdfReader baseLinePdfReader = new PdfReader(BaseFields.PdfData);
                using (MemoryStream pdfStream = new MemoryStream())
                {
                    PdfStamper stamp = new PdfStamper(baseLinePdfReader, pdfStream);
                    foreach (var item in mergeResult)
                    {
                        HighLightFields(item.Key, stamp);
                    }
                    stamp.FormFlattening = false;
                    stamp.Close();
                    baseLinePdfReader.Close();
                    pdfStream.Flush();
                    pdfStream.Close();
                }

                PdfReader currPdfReader = new PdfReader(Current.PdfData);
                using (MemoryStream pdfStream = new MemoryStream())
                {
                    PdfStamper stamp = new PdfStamper(currPdfReader, pdfStream);
                    foreach (var item in mergeResult)
                    {
                        HighLightFields(item.Key, stamp);
                    }
                    stamp.FormFlattening = false;
                    stamp.Close();
                    baseLinePdfReader.Close();
                    pdfStream.Flush();
                    pdfStream.Close();
                }
            }
            else
            {
                return;
            }
        }
        private Dictionary<string, string> MergeCaseWithBaseLine(Dictionary<string, string> dict1, Dictionary<string, string> dict2)
        {
            var dict3 = new Dictionary<string, string>();
            var first = MergeDictionary(dict1, dict2);
            foreach (var item in first)
            {
                if (!dict3.ContainsKey(item.Key))
                    dict3.Add(item.Key, item.Value);
            }
            var second = MergeDictionary(dict2, dict1);
            foreach (var item in second)
            {
                if (!dict3.ContainsKey(item.Key))
                    dict3.Add(item.Key, item.Value);
            }
            return dict3;//FilterIgnoredNodes(dict3); // ability to ignore some fields, which will be always different
        }
        private Dictionary<string, string> MergeDictionary(Dictionary<string, string> dict1, Dictionary<string, string> dict2)
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
        private void HighLightFields(string field, PdfStamper stamp)
        {
            var fieldPositions = stamp.AcroFields.GetFieldPositions(field);
            if (fieldPositions == null)
                return;

            var positions = fieldPositions.ToArray();

            for (int i = 0; i < positions.Length; i++)
            {
                int pageNum = positions[i].page;
                float left = (float)Math.Round(positions[i].position.Left);
                float right = (float)Math.Round(positions[i].position.Right);
                float top = (float)Math.Round(positions[i].position.Top);
                float bottom = (float)Math.Round(positions[i].position.Bottom);
                PdfContentByte contentByte = stamp.GetOverContent(pageNum);
                contentByte.SetColorFill(BaseColor.ORANGE);
                contentByte.Rectangle(left, top, right - left, bottom - top);
                contentByte.Fill();
            }
        }
    }
    #endregion
}
