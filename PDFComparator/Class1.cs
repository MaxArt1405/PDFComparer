using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
public class Class1
{
    static string FirstFile, SecondFile;
    public static void CompareTwoPDF(string FirstPDF, string SecondPDF)
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

        List<string> File1diff;
        List<string> File2diff;
        IEnumerable<string> file1 = FirstFile.Trim().Split('\r', '\n');
        IEnumerable<string> file2 = SecondFile.Trim().Split('\r', '\n');
        File1diff = file1.ToList();
        File2diff = file2.ToList();

        if (file2.Count() > file1.Count())
        {
            Console.WriteLine("File 1 has less number of lines than File 2.");
            for (int i = 0; i < File1diff.Count; i++)
            {
                if (!File1diff[i].Equals(File2diff[i]))
                {
                    Console.WriteLine("File 1 content: " + File1diff[i] + "\r\n" + "File 2 content: " + File2diff[i]);
                }

            }

            for (int i = File1diff.Count; i < File2diff.Count; i++)
            {
                Console.WriteLine("File 2 extra content: " + File2diff[i]);
            }

        }
        else if (file2.Count() < file1.Count())
        {
            Console.WriteLine("File 2 has less number of lines than File 1.");

            for (int i = 0; i < File2diff.Count; i++)
            {
                if (!File1diff[i].Equals(File2diff[i]))
                {
                    Console.WriteLine("File 1 content: " + File1diff[i] + "\r\n" + "File 2 content: " + File2diff[i]);
                }

            }

            for (int i = File2diff.Count; i < File1diff.Count; i++)
            {
                Console.WriteLine("File 1 extra content: " + File1diff[i]);
            }
        }
        else
        {
            Console.WriteLine("File 1 and File 2, both are having same number of lines.");

            for (int i = 0; i < File1diff.Count; i++)
            {
                if (!File1diff[i].Equals(File2diff[i]))
                {
                    Console.WriteLine("File 1 content: " + File1diff[i] + "\r\n" + "File 2 Content: " + File2diff[i]);
                }

            }

        }

    }
}
