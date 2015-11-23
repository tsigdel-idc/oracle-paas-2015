using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using IDC.Common;
using IDC.LeadCapture.Models;

namespace IDC.LeadCapture.BLL
{
    public class PdfReport
    {
        private string imagePath = "~/Content/images/pdf/";

        iTextSharp.text.Font bodyText = FontFactory.GetFont("ms-sans-serif", 10f);
        iTextSharp.text.Font bodyText11 = FontFactory.GetFont("ms-sans-serif", 11f);
        iTextSharp.text.Font bodyTextBold = FontFactory.GetFont("ms-sans-serif", 10f, iTextSharp.text.Font.BOLD, new BaseColor(0, 0, 0));
        iTextSharp.text.Font serifGray9 = FontFactory.GetFont("ms-sans-serif", 9f, new BaseColor(128, 128, 128));
        iTextSharp.text.Font serifGray12 = FontFactory.GetFont("ms-sans-serif", 12f, new BaseColor(0, 0, 0));
        iTextSharp.text.Font serifBlueBold10 = FontFactory.GetFont("ms-sans-serif", 10f, iTextSharp.text.Font.BOLD, new BaseColor(31, 73, 125));

        iTextSharp.text.Font titleMain = FontFactory.GetFont("calibri", 32f, iTextSharp.text.Font.ITALIC, new BaseColor(79, 129, 189));
        iTextSharp.text.Font titleSub = FontFactory.GetFont("calibri", 15f, new BaseColor(64, 64, 64));
        iTextSharp.text.Font calBlue14 = FontFactory.GetFont("calibri", 14f, new BaseColor(79, 129, 189));
        iTextSharp.text.Font calGray10 = FontFactory.GetFont("calibri", 10f, new BaseColor(64, 64, 64));
        
        iTextSharp.text.Font treb24 = FontFactory.GetFont("trebuchet", 24f, new BaseColor(0, 0, 0));
        iTextSharp.text.Font trebBlueBold12 = FontFactory.GetFont("trebuchet", 12f, iTextSharp.text.Font.BOLD, new BaseColor(31, 73, 125));
        iTextSharp.text.Font trebBlueBold13 = FontFactory.GetFont("trebuchet", 12f, iTextSharp.text.Font.BOLD, new BaseColor(31, 73, 125));
        iTextSharp.text.Font trebBold12 = FontFactory.GetFont("trebuchet", 12f, iTextSharp.text.Font.BOLD, new BaseColor(0, 0, 0));

        Color color_orange = Color.FromArgb(255, 247, 163, 67);
        Color color_gray = Color.FromArgb(255, 68, 68, 68);
        Color color_white_transparent = Color.FromArgb(0, 255, 255, 255);
        Color color_white_opaque = Color.FromArgb(255, 255, 255, 255);

        // constants for slide images
        const float imageScale = 76.2f;

        public PdfReport()
        {
            // Create fonts
            FontFactory.Register(HttpContext.Current.Server.MapPath("~/Content/fonts/arial.ttf"), "arial");
            FontFactory.Register(HttpContext.Current.Server.MapPath("~/Content/fonts/arialbd.ttf"), "arial-bold");
            FontFactory.Register(HttpContext.Current.Server.MapPath("~/Content/fonts/calibri.ttf"), "calibri");
            FontFactory.Register(HttpContext.Current.Server.MapPath("~/Content/fonts/calibrib.ttf"), "calibri-bold");
            FontFactory.Register(HttpContext.Current.Server.MapPath("~/Content/fonts/trebuc.ttf"), "trebuchet");
            FontFactory.Register(HttpContext.Current.Server.MapPath("~/Content/fonts/trebucbd.ttf"), "trebuchet-bold");
            FontFactory.Register(HttpContext.Current.Server.MapPath("~/Content/fonts/sserife.fon"), "ms-sans-serif");
        }

        public void GenerateReport(Models.Scoring report, MemoryStream ms, List<string> templates = null)
        {
            // Create document and writer objects
            var tempStream = new MemoryStream();
            CreateDocument(report, tempStream);

            // convert pdf document to byte array and create a PdfReader
            byte[] pdfContent = tempStream.ToArray();
            var tempReader = new PdfReader(pdfContent);

            int numberOfFiles = 0;
            if (templates != null) numberOfFiles = templates.Count;

            // final document
            var document = new Document();
            var writer = PdfWriter.GetInstance(document, ms);
            document.Open();
            var content = writer.DirectContent;
            var readers = new List<PdfReader>();

            // iterate through all documents starting from generated pdf
            for (int fileCounter = -1; fileCounter < numberOfFiles; fileCounter++)
            {
                // PdfReader of each document (generated in memory or loaded from file)
                PdfReader reader = null;

                if (fileCounter < 0)
                {
                    // generated pfd document
                    reader = tempReader;
                }
                else
                {
                    // load pdf document from file system
                    string path = templates[fileCounter];
                    var fi = new FileInfo(path);
                    if (!fi.Exists) continue;
                    reader = new PdfReader(path);
                }

                readers.Add(reader);

                int numberOfPages = reader.NumberOfPages;

                // iterate through all pages
                for (int currentPageIndex = 1; currentPageIndex <= numberOfPages; currentPageIndex++)
                {
                    // determine page size for the current page
                    document.SetPageSize(reader.GetPageSizeWithRotation(currentPageIndex));

                    // create page
                    document.NewPage();

                    // determine page orientation
                    var importedPage = writer.GetImportedPage(reader, currentPageIndex);
                    int pageOrientation = reader.GetPageRotation(currentPageIndex);

                    if ((pageOrientation == 90) || (pageOrientation == 270))
                    {
                        content.AddTemplate(importedPage, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(currentPageIndex).Height);
                    }
                    else
                    {
                        content.AddTemplate(importedPage, 1f, 0, 0, 1f, 0, 0);
                    }

                    // copy hyperlinks
                    var links = reader.GetLinks(currentPageIndex);
                    foreach (var link in links) content.AddAnnotation(link.CreateAnnotation(writer), true);
                }
            }

            writer.CloseStream = false;
            document.Close();
            foreach (var reader in readers) reader.Close();

            // set Position to 0 - important!
            ms.Position = 0;
        }

        private void CreateDocument(Models.Scoring report, MemoryStream ms)
        {
            var pagesize = GetPageSize();
            float marginRight = GetFloatResource("pdf_margin_right"); // en-US: 64f, de-DE: 32f
            var doc = new Document(pagesize, 64f, marginRight, 32f, 40f);
            var writer = PdfWriter.GetInstance(doc, ms);
            writer.CompressionLevel = 9;  // set PDF compression to max level
            writer.PageEvent = new ITextEvents();

            // page 1 **************************************************************************
            doc.Open();

            // Create Slide 1 - Cover page
            float p1_img1_X = GetFloatResource("p1_img1_X"); //-25; //-10
            float p1_img1_Y = GetFloatResource("p1_img1_Y"); //765; // 715f;

            float p1_img2_Y = GetFloatResource("p1_img2_Y"); //400f; // 350f;

            float p1_img3_Y = GetFloatResource("p1_img3_Y"); //190f; //120f
            float p1_img3_X = GetFloatResource("p1_img3_X"); //454f; //442f

            float p1_txt2_Y = GetFloatResource("p1_txt2_Y"); //32f; // 0
            float p1_txt3_Y = GetFloatResource("p1_txt3_Y"); //40f; // 30f

            AddImage(ResourceCache.Localize("p1_img1"), p1_img1_X, p1_img1_Y, imageScale, doc);
            AddImage(ResourceCache.Localize("p1_img2"), 0, p1_img2_Y, imageScale, doc);

            AddText(new string[] { 
                ResourceCache.Localize("p1_txt1_1"), 
                ResourceCache.Localize("p1_txt1_2"), 
                ResourceCache.Localize("p1_txt1_3") },
                titleMain, 440, 0, Element.ALIGN_RIGHT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p1_txt2") },
                titleSub, p1_txt2_Y, 0, Element.ALIGN_RIGHT, 1.5f, doc); 

            AddImage(ResourceCache.Localize("p1_img3"), p1_img3_X, p1_img3_Y, 13, doc);
            
            AddText(new string[] { ResourceCache.Localize("p1_txt3") },
                calBlue14, p1_txt3_Y, 0, Element.ALIGN_RIGHT, 1.5f, doc);

            AddText(new string[] { 
                ResourceCache.Localize("p1_txt4_1"),
                ResourceCache.Localize("p1_txt4_2") },
                calGray10, 0, 0, Element.ALIGN_RIGHT, 1.5f, doc);

            // page 2 **************************************************************************
            doc.NewPage();

            float spacing = GetFloatResource("pdf_spacing"); //5;  // 10
            float p2_txt2_Y = GetFloatResource("p2_txt2_Y"); //30; // 50

            AddText(new string[] { ResourceCache.Localize("p2_txt1") },
                treb24, 0, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p2_txt2") },
                trebBlueBold12, p2_txt2_Y, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddLineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1, doc);

            AddText(new string[] { ResourceCache.Localize("p2_txt3") },
                bodyText, spacing, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p2_txt4") },
                bodyText, spacing, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p2_txt5") },
                bodyText, spacing, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p2_txt6") },
                bodyText, spacing, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddList(new string[] { 
                ResourceCache.Localize("p2_txt7-1"),
                ResourceCache.Localize("p2_txt7-2"),
                ResourceCache.Localize("p2_txt7-3"),
                ResourceCache.Localize("p2_txt7-4") },
                bodyText, 0, 0, spacing, Element.ALIGN_LEFT, 1.5f, "\u2022", 5, 10, doc);

            AddText(new string[] { ResourceCache.Localize("p2_txt8") },
                bodyText, 0, 0, Element.ALIGN_LEFT, 1.5f, doc);

            // page 3 **************************************************************************
            doc.NewPage();

            float p3_img1_Y = GetFloatResource("p3_img1_Y"); //280f; // 245f

            AddText(new string[] { ResourceCache.Localize("p3_txt1") },
                trebBlueBold13, 0, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p3_txt2") },
                bodyText, 10, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p3_txt3") },
                trebBlueBold12, spacing, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddLineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1, doc);

            AddText(new string[] { ResourceCache.Localize("p3_txt4") },
                trebBold12, 0, 0, Element.ALIGN_LEFT, 1, doc);

            AddImage(ResourceCache.Localize("p3_img1"), 50f, p3_img1_Y, 33.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p3_txt5") },
                serifGray9, 320, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p3_txt6") },
                bodyText, 20, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p3_txt7") },
                bodyText, spacing, 0, Element.ALIGN_LEFT, 1.5f, doc);

            // page 4 **************************************************************************
            doc.NewPage();

            float p4_img1_Y = GetFloatResource("p4_img1_Y"); //155f; // 165f
            float p4_txt6_Y = GetFloatResource("p4_txt6_Y"); //10; // 320;

            AddText(new string[] { ResourceCache.Localize("p4_txt1") },
                trebBlueBold13, 0, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p4_txt2") },
                bodyText, spacing, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddList(new string[] { 
                ResourceCache.Localize("p4_txt3_1"), 
                ResourceCache.Localize("p4_txt3_2") },
                bodyText, 0, 0, spacing, Element.ALIGN_LEFT, 1.5f, "\u2022", 5, 10, doc);

            AddText(new string[] { ResourceCache.Localize("p4_txt4") },
                trebBlueBold12, spacing, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddLineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1, doc);

            AddText(new string[] { ResourceCache.Localize("p4_txt5") },
                trebBold12, 0, 0, Element.ALIGN_LEFT, 1, doc);

            AddImage(ResourceCache.Localize("p4_img1"), 60f, p4_img1_Y, imageScale, doc);

            // German only
            AddText(new string[] { ResourceCache.Localize("p4_txt5a"), ResourceCache.Localize("p4_txt5b") },
                serifGray9, 290, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p4_txt6") },
                bodyText, p4_txt6_Y, 0, Element.ALIGN_LEFT, 1.5f, doc);

            // page 5 **************************************************************************
            doc.NewPage();

            float p5_txt5_Y = GetFloatResource("p5_txt5_Y"); //0; //370
            float p5_img1_Y = GetFloatResource("p5_img1_Y"); //275; //290
            float p5_barchart_Y = GetFloatResource("p5_barchart_Y"); //250f; //265f

            AddText(new string[] { ResourceCache.Localize("p5_txt1") },
                trebBlueBold13, 0, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { 
                string.Format(ResourceCache.Localize("p5_txt2"),
                               report.AltLevelName, 
                               report.LevelName, 
                               report.PeerScoreOverall_Top10Pct) },
                bodyText, spacing, 0, Element.ALIGN_LEFT, 1.5f, doc);

            // German only
            AddText(new string[] { ResourceCache.Localize("p5_txt2a") },
                bodyText, spacing, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p5_txt3") },
                trebBlueBold12, spacing, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddLineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1, doc);

            AddText(new string[] { string.Format(ResourceCache.Localize("p5_txt4"), report.CompanyName) },
                trebBold12, 0, 0, Element.ALIGN_LEFT, 1, doc);

            AddImage(ResourceCache.Localize("p5_img1"), 60f, p5_img1_Y, imageScale, doc);

            AddBarChart(new decimal[] {
                report.ScoreOverall,
                report.PeerScoreOverall_Top10Pct,
                report.PeerScoreIndustry_Top10Pct, 
                report.PeerScoreOrgSize_Top10Pct, 
                report.PeerScoreRegion_Top10Pct},
                465, 460, 190f, p5_barchart_Y, imageScale, doc);

            // German only
            AddText(new string[] { ResourceCache.Localize("p5_txt4a"), ResourceCache.Localize("p5_txt4b") },
                serifGray9, 340, 10, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p5_txt5") },
                bodyText, p5_txt5_Y, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddList(new string[] {
                ResourceCache.Localize("p5_txt6_1"),
                ResourceCache.Localize("p5_txt6_2"),
                ResourceCache.Localize("p5_txt6_3") },
                bodyText, 0, 0, spacing, Element.ALIGN_LEFT, 1.5f, "\u2022", 5, 10, doc);

            AddText(new string[] { ResourceCache.Localize("p5_txt7") },
                bodyText, 10, 0, Element.ALIGN_LEFT, 1.5f, doc);

            // page 6 **************************************************************************
            doc.NewPage();

            float p6_txt1a_Y = GetFloatResource("p6_txt1a_Y"); //5f; //0
            float bodyText_size = GetFloatResource("bodyText_size"); //9.5f; //10f

            AddText(new string[] { ResourceCache.Localize("p6_txt1"), ResourceCache.Localize("p6_txt1a") },
                bodyText, p6_txt1a_Y, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p6_txt2") },
                trebBlueBold12, spacing, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddLineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1, doc);

            // saving some more space
            bodyText = FontFactory.GetFont("ms-sans-serif", bodyText_size);

            AddText(new string[] { ResourceCache.Localize("p6_txt3") },
                bodyText, spacing, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p6_txt4") },
                bodyTextBold, spacing, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddList(new string[] {
                ResourceCache.Localize("p6_txt5_1"),
                ResourceCache.Localize("p6_txt5_2"),
                ResourceCache.Localize("p6_txt5_3"),
                ResourceCache.Localize("p6_txt5_4"),
                ResourceCache.Localize("p6_txt5_5") },
                bodyText, 0, 0, spacing, Element.ALIGN_LEFT, 1.5f, "\u2022", 5, 10, doc);

            AddText(new string[] { ResourceCache.Localize("p6_txt6") },
                serifBlueBold10, spacing, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddLineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1, doc);

            AddList(new string[] {
                ResourceCache.Localize("p6_txt7_1"),
                ResourceCache.Localize("p6_txt7_2"),
                ResourceCache.Localize("p6_txt7_3"),
                ResourceCache.Localize("p6_txt7_4"),
                ResourceCache.Localize("p6_txt7_5"),
                ResourceCache.Localize("p6_txt7_6"),
                ResourceCache.Localize("p6_txt7_7"),
                ResourceCache.Localize("p6_txt7_8"),
                ResourceCache.Localize("p6_txt7_9"),
                ResourceCache.Localize("p6_txt7_10"),
                ResourceCache.Localize("p6_txt7_11"),
                ResourceCache.Localize("p6_txt7_12"),
                ResourceCache.Localize("p6_txt7_13") },
                bodyText, 0, 0, 3, Element.ALIGN_LEFT, 1.5f, "\u2022", 5, 10, doc);

            // page 7 **************************************************************************
            doc.NewPage();

            float p7_img1_Y = GetFloatResource("p7_img1_Y"); //560f; //500f
            float p7_img2_Y = GetFloatResource("p7_img2_Y"); //400f; //350f
            float p7_txt1_Y = GetFloatResource("p7_txt1_Y"); //250; //270,

            AddImage(ResourceCache.Localize("p7_img1"), 0, p7_img1_Y, imageScale, doc);
            AddImage(ResourceCache.Localize("p7_img2"), 40, p7_img2_Y, 100, doc);

            AddList(new string[] {
                ResourceCache.Localize("p7_txt1_1"),
                ResourceCache.Localize("p7_txt1_2"),
                ResourceCache.Localize("p7_txt1_3"),
                ResourceCache.Localize("p7_txt1_4"),
                ResourceCache.Localize("p7_txt1_5")},
                bodyText11, p7_txt1_Y, 0, spacing, Element.ALIGN_LEFT, 0, string.Empty, 0, 150, doc);

            AddImage(ResourceCache.Localize("p7_img3"), 40, 30, 50, doc);

            // page 8 **************************************************************************
            doc.NewPage();

            AddText(new string[] { ResourceCache.Localize("p8_txt1") },
                trebBlueBold12, 0, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p8_txt2"), ResourceCache.Localize("p8_txt2a") },
                bodyText, 10, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p8_txt3") },
                trebBlueBold12, 10, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] {
                ResourceCache.Localize("p8_txt4_1"),
                ResourceCache.Localize("p8_txt4_2"),
                ResourceCache.Localize("p8_txt4_3"),
                ResourceCache.Localize("p8_txt4_4"),
                ResourceCache.Localize("p8_txt4_5"),
                ResourceCache.Localize("p8_txt4_6"),
                ResourceCache.Localize("p8_txt4_7") },
                bodyText, 10, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddLineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1, doc);

            AddText(new string[] { ResourceCache.Localize("p8_txt5") },
                bodyText, 10, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p8_txt6") },
                serifGray9, 10, 0, Element.ALIGN_LEFT, 1.5f, doc);

            // German only
            AddText(new string[] { ResourceCache.Localize("p8_txt6a"), ResourceCache.Localize("p8_txt6b") },
                serifGray9, 10, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddText(new string[] { ResourceCache.Localize("p8_txt7") },
                serifGray9, 10, 0, Element.ALIGN_LEFT, 1.5f, doc);

            AddImage(ResourceCache.Localize("p8_img1"), 0, 10f, imageScale, doc);

            // close document BEFORE passing object reference to PdfReader - important!
            doc.Close();
        }

        #region helper methods

        private float AddText(string[] chunks, iTextSharp.text.Font font, float spacingBefore, float spacingAfter, int alignment, float lineHightMultiplier, Document doc)
        {
            int count = 0;
            float height = 0;
            var paragraph = new Paragraph();            
            float fontSize = font.Size * lineHightMultiplier;
            var phrase = new Phrase();

            foreach(string item in chunks)
            {
                if (string.IsNullOrEmpty(item)) continue;
                var chunk = new Chunk(item + Environment.NewLine, font);
                chunk.setLineHeight(fontSize);
                phrase.Add(chunk);
                count++;
            }

            if (count > 0)
            {
                paragraph.Add(phrase);
                paragraph.Alignment = alignment;
                paragraph.SpacingBefore = spacingBefore;
                paragraph.SpacingAfter = spacingAfter;
                doc.Add(paragraph);
                height = spacingBefore + spacingAfter + count * fontSize;
            }

            return height;
        }

        private float AddList(string[] chunks, iTextSharp.text.Font font, float spacingBefore, float spacingAfter, float spacingBetween, int alignment, float lineHightMultiplier, string listSymbol, int padding, float indentationLeft, Document doc)
        {
            int count = 0;
            float height = 0;
            float fontSize = font.Size * lineHightMultiplier;
            var paragraph = new Paragraph();

            var list = new List(List.UNORDERED);
            list.SetListSymbol(listSymbol.PadRight(padding));
            list.IndentationLeft = indentationLeft;

            foreach (string item in chunks)
            {
                if (string.IsNullOrEmpty(item)) continue;
                var chunk = new Chunk(item, font);
                chunk.setLineHeight(font.Size * lineHightMultiplier);
                var listItem = new ListItem(chunk);
                listItem.SpacingBefore = spacingBetween;
                list.Add(listItem);
                count++;
            }

            if (count > 0)
            {
                paragraph.Add(list);
                paragraph.Alignment = alignment;
                paragraph.SpacingBefore = spacingBefore;
                paragraph.SpacingAfter = spacingAfter;
                doc.Add(paragraph);
                height = spacingBefore + spacingAfter + count * (fontSize + spacingBetween) - spacingBetween;
            }

            return height;
        }

        private void AddLineSeparator(float lineWidth, float percentage, BaseColor lineColor, int align, float offset, Document doc)
        {
            doc.Add(new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(lineWidth, percentage, lineColor, align, offset))));
        }

        private void AddImage(string imgFileName, float absoluteX, float absoluteY, float scale, Document doc)
        {
            var img = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(Path.Combine(imagePath, imgFileName)));
            img.ScalePercent(scale);
            img.Alignment = iTextSharp.text.Image.TEXTWRAP;
            img.SetAbsolutePosition(absoluteX, absoluteY);
            doc.Add(img);
        }

        // Custom properties list: http://msdn.microsoft.com/en-us/library/dd456764(v=vs.110).aspx
        private void AddBarChart(decimal[] score, int width, int height, float absoluteX, float absoluteY, float scale, Document doc)
        {
            var chart = new Chart
            {
                Width = width,
                Height = height,
                RenderType = RenderType.BinaryStreaming,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };

            // Set chart area properties
            chart.BackColor = Color.Transparent;
            chart.ChartAreas.Add("Overall");
            chart.ChartAreas[0].BackColor = Color.Transparent;

            // Set X-axis properties
            chart.ChartAreas[0].AxisX.TitleForeColor = color_gray;
            chart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 9f);
            chart.ChartAreas[0].AxisX.LineColor = Color.Transparent;
            chart.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = color_white_transparent;
            chart.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;

            // Set Y-axis properties
            chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;
            chart.ChartAreas[0].AxisY.LineColor = Color.Transparent;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Transparent;
            chart.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
            chart.ChartAreas[0].AxisY.Minimum = 0.0;
            chart.ChartAreas[0].AxisY.Interval = 1.0;
            chart.ChartAreas[0].AxisY.Maximum = 5.0;

            // Add data series to chart
            chart.Series.Add("Digital Transformation");
            chart.Series[0].ChartType = SeriesChartType.Bar;
            chart.Series[0]["PixelPointWidth"] = "20";
            chart.Series[0]["BarLabelStyle"] = "Right";
            chart.Series[0].IsValueShownAsLabel = false;

            chart.Series[0].Color = color_orange;

            // Add data to the chart IN REVERSE ORDER to display in the correct order on screen!!
            for (int i = score.Length - 1; i >= 0; i--)
            {
                chart.Series[0].Points.AddY(score[i]);
            }

            // Render chart in png format
            byte[] buffer = null;

            using (var chartimage = new MemoryStream())
            {
                chart.SaveImage(chartimage, ChartImageFormat.Png);
                buffer = chartimage.GetBuffer();
            }

            if (buffer != null)
            {
                var img = iTextSharp.text.Image.GetInstance(buffer);
                img.SetAbsolutePosition(absoluteX, absoluteY);
                img.ScalePercent(scale);
                doc.Add(img);
            }
        }

        private iTextSharp.text.Rectangle GetPageSize()
        {
            string pageSize = ResourceCache.Localize("pdf_page_size");
            if (!string.IsNullOrEmpty(pageSize) && pageSize == "A4") return PageSize.A4;
            else return PageSize.LETTER;
        }

        private float GetFloatResource(string key)
        {
            string strVal = ResourceCache.Localize(key);
            return Helper.FloatValue(strVal);
        }

        #endregion
    }

    #region iTextSharp events

    public class ITextEvents : PdfPageEventHelper
    {
        PdfContentByte cb;
        BaseFont arial;
        BaseColor gray;

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            cb = writer.DirectContent;
            arial = FontFactory.GetFont("arial", 10f, iTextSharp.text.Font.NORMAL, new BaseColor(137, 137, 137)).BaseFont;
            gray = new BaseColor(137, 137, 137);
        }

        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);

            if (writer.PageNumber > 1)
            {
                float leftMargin = 64f;
                cb.BeginText();
                cb.SetFontAndSize(arial, 10f);
                cb.SetColorFill(gray);
                cb.SetTextMatrix(document.PageSize.GetLeft(leftMargin), document.PageSize.GetBottom(25f));
                cb.ShowText("© " + DateTime.Today.Year.ToString() + " IDC");
                cb.SetTextMatrix(document.PageSize.GetRight(50f), document.PageSize.GetBottom(25f));
                cb.ShowText(writer.PageNumber.ToString());
                cb.EndText();
            }
        }
    }

    #endregion
}