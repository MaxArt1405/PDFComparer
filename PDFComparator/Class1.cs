using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
public class Class1
{
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
