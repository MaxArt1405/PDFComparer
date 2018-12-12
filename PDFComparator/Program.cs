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
        static void Main(string[] args)
        {
            string FirstPDF = "ICC16New.pdf";
            string SecondPDF = "ICC16Edit.pdf";
            CompareTwoPDF(FirstPDF, SecondPDF);
            Console.ReadLine();
        }

        private static void CompareTwoPDF(string FirstPDF, string SecondPDF)
        {

            PdfReader reader1 = new PdfReader(SecondPDF);
            PdfReader reader = new PdfReader(FirstPDF);

            Dictionary<string, string> file1 = GetFormFieldValues(reader);
            Dictionary<string, string> file2 = GetFormFieldValues(reader1);

            Dictionary<string, string> mergeResult = MergeDictionary(file1, file2);
            using (MemoryStream pdfStream = new MemoryStream())
            {
                PdfStamper stamp = new PdfStamper(reader, pdfStream);
                foreach (var item in mergeResult)
                {
                    HighLightFields(item.Key, stamp);
                }
                stamp.FormFlattening = false;
                stamp.Close();
                pdfStream.Flush();
                pdfStream.Close();
            }

            using (MemoryStream pdfStream = new MemoryStream())
            {
                PdfStamper stamp = new PdfStamper(reader1, pdfStream);
                foreach (var item in mergeResult)
                {
                    HighLightFields(item.Key, stamp);
                }
                stamp.FormFlattening = false;
                stamp.Close();
                pdfStream.Flush();
                pdfStream.Close();
            }
            using (var file = new StreamWriter("Merge.json", false))
            {
                foreach (var item in mergeResult)
                {
                    file.WriteLine("\r\n" + item.Key + "----" + item.Value);
                }
            }
            using (var file = new StreamWriter("FirstPDF.json", false))
            {
                foreach(var item in file1)
                {
                    file.WriteLine("\r\n" + item.Key + "----" + item.Value);
                }
            }
            using (var file = new StreamWriter("SecondPDF.json", false))
            {
                foreach (var item in file2)
                {
                    file.WriteLine("\r\n" + item.Key + "----" + item.Value);
                }
            }
        }
        private static Dictionary<string, string> GetFormFieldValues(PdfReader pdfReader)
        {
            var dict = new Dictionary<string, string>
            {
                { string.Join("\r\n", pdfReader.AcroFields.Fields.Select(x => x.Key).ToArray()), string.Join("\r\n",pdfReader.AcroFields.Fields.Select(y => pdfReader.AcroFields.GetField(y.Key)).ToArray()) }
            };
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

}
