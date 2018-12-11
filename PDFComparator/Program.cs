using System;
using iTextSharp;
using Baseline;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using System.Text;
using iTextSharp.text.pdf.parser;
using System.Linq;

namespace PDFComparator
{
    class Program
    {
        static void Main(string[] args)
        {

            CompareTwoPDF("TablesSource.pdf", "TablesTarget.pdf");
            Console.ReadLine();
        }
        public static string ReadPdfFile(string fileName)
        {
            StringBuilder text = new StringBuilder();

            if (File.Exists(fileName))
            {
                PdfReader pdfReader = new PdfReader(fileName);

                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                    text.Append(currentText);
                }
                pdfReader.Close();
            }
            return text.ToString();
        }
        static string FirstFile, SecondFile;
        public static void CompareTwoPDF(string FirstPDF, string SecondPDF)
        {
            IsExist(FirstPDF, SecondPDF);

            List<string> File1diff;
            List<string> File2diff;

            IEnumerable<string> file1 = FirstFile.Trim().Split('\r', '\n');
            IEnumerable<string> file2 = SecondFile.Trim().Split('\r', '\n');

            File1diff = file1.ToList();
            File2diff = file2.ToList();

            if (file2.Count() > file1.Count())
            {
                Console.WriteLine("The FirstPDF has less number of lines than the SecondPDF.");
                for (int i = 0; i < File1diff.Count; i++)
                {
                    if (!File1diff[i].Equals(File2diff[i]))
                    {
                        Console.WriteLine("The FirstPDF content: " + File1diff[i] + "\r\n" + "The SecondPDF content: " + File2diff[i]);
                    }

                }

                for (int i = File1diff.Count; i < File2diff.Count; i++)
                {
                    Console.WriteLine("File 2 extra content: " + File2diff[i]);
                }

            }
            else if (file2.Count() < file1.Count())
            {
                Console.WriteLine("The SecondPDF has less number of lines than File 1.");

                for (int i = 0; i < File2diff.Count; i++)
                {
                    if (!File1diff[i].Equals(File2diff[i]))
                    {
                        Console.WriteLine("The FirstPDF content: " + File1diff[i] + "\r\n" + "The SecondPDF content: " + File2diff[i]);
                    }

                }

                for (int i = File2diff.Count; i < File1diff.Count; i++)
                {
                    Console.WriteLine("The FirstPDF extra content: " + File1diff[i]);
                }
            }
            else
            {
                Console.WriteLine("The FirstPDF and the SecondPDF, both are having same number of lines.");

                for (int i = 0; i < File1diff.Count; i++)
                {
                    if (!File1diff[i].Equals(File2diff[i]))
                    {
                        Console.WriteLine("The FirstPDF content: " + File1diff[i] + "\r\n" + "The SecondPDF content: " + File2diff[i]);
                    }

                }

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
    }
}
