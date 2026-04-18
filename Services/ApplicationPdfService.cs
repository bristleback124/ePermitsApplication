using ePermitsApp.DTOs;
using ePermitsApp.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

namespace ePermitsApp.Services
{
    public class ApplicationPdfService : IApplicationPdfService
    {
        private readonly IApplicationService _applicationService;
        private readonly IDocumentDownloadService _documentDownloadService;
        private readonly IFileStorageService _fileStorageService;

        public ApplicationPdfService(
            IApplicationService applicationService,
            IDocumentDownloadService documentDownloadService,
            IFileStorageService fileStorageService)
        {
            _applicationService = applicationService;
            _documentDownloadService = documentDownloadService;
            _fileStorageService = fileStorageService;
        }

        public async Task<byte[]> GenerateApplicationPdfAsync(int applicationId, string type)
        {
            var documents = await _documentDownloadService.GetDocumentPathsAsync(applicationId, type);

            byte[] questPdfBytes;
            if (type == "BuildingPermit")
            {
                var app = await _applicationService.GetApplicationBuildingPermitById(applicationId);
                if (app == null) throw new InvalidOperationException("Application not found.");
                questPdfBytes = GenerateBuildingPermitPdf(app, documents);
            }
            else
            {
                var app = await _applicationService.GetApplicationCoOById(applicationId);
                if (app == null) throw new InvalidOperationException("Application not found.");
                questPdfBytes = GenerateCoOPdf(app, documents);
            }

            // Phase B: Merge PDF uploads using PdfSharpCore
            return MergePdfUploads(questPdfBytes, documents, _fileStorageService);
        }

        private byte[] GenerateBuildingPermitPdf(ApplicationBuildingPermitDetailDto app, List<(string Folder, string FileName, string FilePath)>? documents)
        {
            var doc = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    SetupPage(page);
                    page.Content().Column(col =>
                    {
                        RenderHeader(col, "Building Permit Application Summary", app.BasicInformation.FormattedId);
                        RenderBasicInformation(col, app.BasicInformation);
                        RenderProjectDetails(col, app.ProjectDetails);
                        RenderOwnerInformation(col, app.OwnerInformation);
                        RenderBuildingPermitDocChecklist(col, app.RequiredDocs, app.BuildingPermitTechDocs);
                        RenderReviewOffices(col, app.ReviewOffices);
                    });
                });

                // Render non-PDF uploaded documents as additional pages
                if (documents != null)
                    RenderNonPdfDocuments(container, documents, _fileStorageService);
            });

            return doc.GeneratePdf();
        }

        private byte[] GenerateCoOPdf(ApplicationCoODetailDto app, List<(string Folder, string FileName, string FilePath)>? documents)
        {
            var doc = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    SetupPage(page);
                    page.Content().Column(col =>
                    {
                        RenderHeader(col, "Certificate of Occupancy Application Summary", app.BasicInformation.FormattedId);
                        RenderBasicInformation(col, app.BasicInformation);
                        RenderCoOProjectDetails(col, app.ProjectDetails);
                        RenderOwnerInformation(col, app.OwnerInformation);
                        RenderProfessionalInfo(col, app.ProfessionalInfo);
                        RenderCoODocChecklist(col, app.RequiredDocs);
                        RenderReviewOffices(col, app.ReviewOffices);
                    });
                });

                if (documents != null)
                    RenderNonPdfDocuments(container, documents, _fileStorageService);
            });

            return doc.GeneratePdf();
        }

        // ── Page Setup ──

        private static void SetupPage(PageDescriptor page)
        {
            page.Size(PageSizes.A4);
            page.Margin(40);
            page.DefaultTextStyle(x => x.FontSize(10));
        }

        // ── Shared Sections ──

        private static void RenderHeader(ColumnDescriptor col, string title, string formattedId)
        {
            col.Item().PaddingBottom(10).Column(inner =>
            {
                inner.Item().Text(title).FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                inner.Item().Text($"Application ID: {formattedId}").FontSize(11).FontColor(Colors.Grey.Darken1);
                inner.Item().Text($"Generated: {DateTime.Now:MMMM dd, yyyy hh:mm tt}").FontSize(9).FontColor(Colors.Grey.Medium);
            });
            col.Item().PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        }

        private static void RenderBasicInformation(ColumnDescriptor col, BasicInformationDto info)
        {
            RenderSectionTitle(col, "Basic Information");
            col.Item().PaddingBottom(10).Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(1); c.RelativeColumn(2); });
                AddFieldRow(table, "Application ID", info.ApplicationId.ToString());
                AddFieldRow(table, "Formatted ID", NaIfEmpty(info.FormattedId));
                AddFieldRow(table, "Project Description", NaIfEmpty(info.ProjectDescription));
                AddFieldRow(table, "Status", NaIfEmpty(info.Status));
                AddFieldRow(table, "Applicant", NaIfEmpty(info.Applicant));
                AddFieldRow(table, "Phone", NaIfEmpty(info.Phone));
            });
        }

        private static void RenderOwnerInformation(ColumnDescriptor col, OwnerInformationDto info)
        {
            RenderSectionTitle(col, "Owner Information");
            col.Item().PaddingBottom(10).Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(1); c.RelativeColumn(2); });
                AddFieldRow(table, "Owner Name", NaIfEmpty(info.OwnerName));
                AddFieldRow(table, "Location", NaIfEmpty(info.Location));
                AddFieldRow(table, "Phone", NaIfEmpty(info.Phone));
                AddFieldRow(table, "Email", NaIfEmpty(info.Email));
            });
        }

        private static void RenderReviewOffices(ColumnDescriptor col, List<ApplicationDepartmentReviewDto> reviews)
        {
            RenderSectionTitle(col, "Review Offices");
            if (reviews == null || reviews.Count == 0)
            {
                col.Item().PaddingBottom(10).Text("No review offices assigned.").Italic().FontColor(Colors.Grey.Medium);
                return;
            }

            col.Item().PaddingBottom(10).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(2);
                    c.RelativeColumn(1);
                    c.RelativeColumn(2);
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Department").Bold().FontSize(9);
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Status").Bold().FontSize(9);
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Assigned Reviewer").Bold().FontSize(9);
                });

                foreach (var r in reviews)
                {
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4).Text(NaIfEmpty(r.DepartmentName)).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4).Text(NaIfEmpty(r.Status)).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4).Text(NaIfEmpty(r.AssignedReviewerName)).FontSize(9);
                }
            });
        }

        // ── Building Permit Specific ──

        private static void RenderProjectDetails(ColumnDescriptor col, ProjectDetailsDto details)
        {
            RenderSectionTitle(col, "Project Details");
            col.Item().PaddingBottom(10).Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(1); c.RelativeColumn(2); });
                AddFieldRow(table, "Project Type", NaIfEmpty(details.ProjectType));
                AddFieldRow(table, "Project Value", $"₱{details.ProjectValue:N2}");
                AddFieldRow(table, "Lot Area", $"{details.LotArea:N2} sq.m.");
                AddFieldRow(table, "Number of Stories", details.NumberOfStories.ToString());
                AddFieldRow(table, "Permit Type", NaIfEmpty(details.PermitType));
                AddFieldRow(table, "Occupancy Type", NaIfEmpty(details.OccupancyType));
                AddFieldRow(table, "Floor Area", $"{details.FloorArea:N2} sq.m.");
                AddFieldRow(table, "Date Filed", details.CreatedAt.ToString("MMMM dd, yyyy"));
            });
        }

        private static void RenderBuildingPermitDocChecklist(ColumnDescriptor col, RequiredDocs reqDocs, BuildingPermitTechDocDto techDocs)
        {
            RenderSectionTitle(col, "Document Checklist");
            col.Item().PaddingBottom(5).Text("Required Documents:").Bold().FontSize(9);
            col.Item().PaddingBottom(3).Text($"  • Proof of Ownership: {FileLabel(reqDocs.ReqDocProofOwnership)}").FontSize(9);
            col.Item().PaddingBottom(3).Text($"  • Barangay Clearance: {FileLabel(reqDocs.ReqDocBarangayClearance)}").FontSize(9);
            col.Item().PaddingBottom(3).Text($"  • Tax Declaration: {FileLabel(reqDocs.ReqDocTaxDeclaration)}").FontSize(9);
            col.Item().PaddingBottom(3).Text($"  • Real Property Tax Receipt: {FileLabel(reqDocs.ReqDocRealPropTaxReceipt)}").FontSize(9);
            col.Item().PaddingBottom(3).Text($"  • ECC or CNC: {FileLabel(reqDocs.ReqDocECCorCNC)}").FontSize(9);
            col.Item().PaddingBottom(5).Text($"  • Special Clearances: {FileLabel(reqDocs.ReqDocSpecialClearances)}").FontSize(9);

            col.Item().PaddingBottom(5).Text("Technical Documents:").Bold().FontSize(9);
            RenderFileList(col, "IoC Plans", techDocs.TechDocIoCPlans);
            RenderFileList(col, "SE Plans", techDocs.TechDocSEPlans);
            RenderFileList(col, "EE Plans", techDocs.TechDocEEPlans);
            RenderFileList(col, "SP Plans", techDocs.TechDocSPPlans);
            RenderFileList(col, "BOM/Cost", techDocs.TechDocBOMCost);
            RenderFileList(col, "Scope of Work", techDocs.TechDocSoW);
            RenderFileList(col, "ME Plans", techDocs.TechDocMEPlans);
            RenderFileList(col, "ECE Plans", techDocs.TechDocECEPlans);
            col.Item().PaddingBottom(10);
        }

        // ── CoO Specific ──

        private static void RenderCoOProjectDetails(ColumnDescriptor col, CoOProjectDetailsDto details)
        {
            RenderSectionTitle(col, "Project Details");
            col.Item().PaddingBottom(10).Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(1); c.RelativeColumn(2); });
                AddFieldRow(table, "Project Type", NaIfEmpty(details.ProjectType));
                AddFieldRow(table, "Building Permit No.", NaIfEmpty(details.BldgPermitNo));
                AddFieldRow(table, "Occupancy Type", NaIfEmpty(details.OccupancyType));
                AddFieldRow(table, "Floor Area", $"{details.FloorArea:N2} sq.m.");
                AddFieldRow(table, "Number of Stories", details.NumberOfStories.ToString());
                AddFieldRow(table, "Completion Date", details.CompletionDate.ToString("MMMM dd, yyyy"));
                AddFieldRow(table, "Date Filed", details.CreatedAt.ToString("MMMM dd, yyyy"));
            });
        }

        private static void RenderProfessionalInfo(ColumnDescriptor col, CoOProfessionalInfoDto info)
        {
            RenderSectionTitle(col, "Professional Information");

            col.Item().PaddingBottom(5).Text("Inspector of Construction (IoC)").Bold().FontSize(9);
            col.Item().PaddingBottom(10).Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(1); c.RelativeColumn(2); });
                AddFieldRow(table, "Full Name", NaIfEmpty(info.IoCFullName));
                AddFieldRow(table, "PRC No.", NaIfEmpty(info.IoCPRCNo));
                AddFieldRow(table, "PTR No.", NaIfEmpty(info.IoCPTRNo));
                AddFieldRow(table, "Validity", info.IOCValidity.ToString("MMMM dd, yyyy"));
            });

            col.Item().PaddingBottom(5).Text("Engineer of Record (EoR)").Bold().FontSize(9);
            col.Item().PaddingBottom(10).Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(1); c.RelativeColumn(2); });
                AddFieldRow(table, "Full Name", NaIfEmpty(info.EoRFullName));
                AddFieldRow(table, "PRC/PTR No.", NaIfEmpty(info.EoRPRCorPTRNo));
                AddFieldRow(table, "Validity", info.EoRValidity.ToString("MMMM dd, yyyy"));
                AddFieldRow(table, "Specialization", NaIfEmpty(info.EoRSpecialization));
            });
        }

        private static void RenderCoODocChecklist(ColumnDescriptor col, CoORequiredDocsDto reqDocs)
        {
            RenderSectionTitle(col, "Document Checklist");
            col.Item().PaddingBottom(3).Text($"  • Building Permit & Signed Plans: {FileLabel(reqDocs.ReqDocBldgPermitSPlans)}").FontSize(9);
            col.Item().PaddingBottom(3).Text($"  • As-Built Plans: {FileLabel(reqDocs.ReqDocAsBuiltPlans)}").FontSize(9);
            col.Item().PaddingBottom(3).Text($"  • Construction Logbook: {FileLabel(reqDocs.ReqDocConsLogbook)}").FontSize(9);
            col.Item().PaddingBottom(3).Text($"  • Construction Photos: {FileLabel(reqDocs.ReqDocConsPhotos)}").FontSize(9);
            col.Item().PaddingBottom(3).Text($"  • Barangay Clearance: {FileLabel(reqDocs.ReqDocBrgyClearance)}").FontSize(9);
            col.Item().PaddingBottom(3).Text($"  • FSIC: {FileLabel(reqDocs.ReqDocFSIC)}").FontSize(9);
            col.Item().PaddingBottom(10).Text($"  • Others: {FileLabel(reqDocs.ReqDocOthers)}").FontSize(9);
        }

        // ── Non-PDF Document Rendering (QuestPDF) ──

        private static void RenderNonPdfDocuments(
            IDocumentContainer container,
            List<(string Folder, string FileName, string FilePath)> documents,
            IFileStorageService fileStorageService)
        {
            foreach (var (folder, fileName, filePath) in documents)
            {
                var ext = Path.GetExtension(fileName).ToLowerInvariant();
                if (ext == ".pdf") continue; // PDFs handled in Phase B

                container.Page(page =>
                {
                    SetupPage(page);
                    page.Content().Column(col =>
                    {
                        // Separator header
                        col.Item().PaddingBottom(10).Column(inner =>
                        {
                            inner.Item().Background(Colors.Blue.Lighten5).Padding(10).Column(sep =>
                            {
                                sep.Item().Text($"Folder: {folder}").FontSize(10).FontColor(Colors.Grey.Darken1);
                                sep.Item().Text(fileName).FontSize(14).Bold();
                            });
                        });

                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                        {
                            try
                            {
                                using var stream = fileStorageService.DownloadAsync(filePath).GetAwaiter().GetResult();
                                using var buffer = new MemoryStream();
                                stream.CopyTo(buffer);
                                var imageBytes = buffer.ToArray();
                                col.Item().Image(imageBytes).FitArea();
                            }
                            catch
                            {
                                col.Item().Padding(20).Text($"Error loading image: {fileName}").Italic().FontColor(Colors.Red.Medium);
                            }
                        }
                        else if (ext == ".docx")
                        {
                            try
                            {
                                using var stream = fileStorageService.DownloadAsync(filePath).GetAwaiter().GetResult();
                                var text = ExtractDocxText(stream);
                                col.Item().Padding(10).Text(string.IsNullOrWhiteSpace(text) ? "(Empty document)" : text).FontSize(9);
                            }
                            catch
                            {
                                col.Item().Padding(20).Text($"Error reading DOCX: {fileName}").Italic().FontColor(Colors.Red.Medium);
                            }
                        }
                        else
                        {
                            // Unsupported: .doc, .dwg, .dxf, etc.
                            col.Item().Padding(40).AlignCenter().AlignMiddle().Column(inner =>
                            {
                                inner.Item().AlignCenter().Text("Cannot embed this file type").FontSize(14).Bold().FontColor(Colors.Grey.Darken1);
                                inner.Item().AlignCenter().Text(fileName).FontSize(11).FontColor(Colors.Grey.Medium);
                                inner.Item().AlignCenter().Text($"File type: {ext}").FontSize(9).FontColor(Colors.Grey.Medium);
                            });
                        }
                    });
                });
            }
        }

        private static string ExtractDocxText(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            using var doc = WordprocessingDocument.Open(stream, false);
            var body = doc.MainDocumentPart?.Document?.Body;
            if (body == null) return string.Empty;

            var paragraphs = body.Descendants<OpenXmlParagraph>();
            return string.Join(Environment.NewLine, paragraphs.Select(p => p.InnerText));
        }

        // ── Phase B: Merge PDF uploads with PdfSharpCore ──

        private static byte[] MergePdfUploads(
            byte[] questPdfBytes,
            List<(string Folder, string FileName, string FilePath)>? documents,
            IFileStorageService fileStorageService)
        {
            var pdfDocuments = documents?.Where(d => Path.GetExtension(d.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase)).ToList();

            if (pdfDocuments == null || pdfDocuments.Count == 0)
                return questPdfBytes;

            using var outputDoc = PdfReader.Open(new MemoryStream(questPdfBytes), PdfDocumentOpenMode.Import);
            using var finalDoc = new PdfDocument();

            // Copy all pages from QuestPDF output
            for (int i = 0; i < outputDoc.PageCount; i++)
                finalDoc.AddPage(outputDoc.Pages[i]);

            // Add each PDF upload with a separator page
            foreach (var (folder, fileName, filePath) in pdfDocuments)
            {
                // Separator page
                var sepPage = finalDoc.AddPage();
                sepPage.Width = PdfSharpCore.Drawing.XUnit.FromPoint(595); // A4 width
                sepPage.Height = PdfSharpCore.Drawing.XUnit.FromPoint(842); // A4 height
                using (var gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(sepPage))
                {
                    var fontBold = new PdfSharpCore.Drawing.XFont("Arial", 16, PdfSharpCore.Drawing.XFontStyle.Bold);
                    var fontNormal = new PdfSharpCore.Drawing.XFont("Arial", 12, PdfSharpCore.Drawing.XFontStyle.Regular);
                    var brush = PdfSharpCore.Drawing.XBrushes.DarkSlateGray;

                    gfx.DrawString($"Folder: {folder}", fontNormal, PdfSharpCore.Drawing.XBrushes.Gray,
                        new PdfSharpCore.Drawing.XRect(40, 80, sepPage.Width - 80, 30), PdfSharpCore.Drawing.XStringFormats.TopLeft);
                    gfx.DrawString(fileName, fontBold, brush,
                        new PdfSharpCore.Drawing.XRect(40, 110, sepPage.Width - 80, 30), PdfSharpCore.Drawing.XStringFormats.TopLeft);
                }

                // Import pages from the PDF file
                try
                {
                    using var uploadedStream = fileStorageService.DownloadAsync(filePath).GetAwaiter().GetResult();
                    using var uploadedPdf = PdfReader.Open(uploadedStream, PdfDocumentOpenMode.Import);
                    for (int i = 0; i < uploadedPdf.PageCount; i++)
                        finalDoc.AddPage(uploadedPdf.Pages[i]);
                }
                catch
                {
                    // If PDF can't be opened, add an error page
                    var errorPage = finalDoc.AddPage();
                    errorPage.Width = PdfSharpCore.Drawing.XUnit.FromPoint(595);
                    errorPage.Height = PdfSharpCore.Drawing.XUnit.FromPoint(842);
                    using var gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(errorPage);
                    var font = new PdfSharpCore.Drawing.XFont("Arial", 14, PdfSharpCore.Drawing.XFontStyle.Regular);
                    gfx.DrawString($"Error: Could not read PDF file '{fileName}'", font, PdfSharpCore.Drawing.XBrushes.Red,
                        new PdfSharpCore.Drawing.XRect(40, 100, errorPage.Width - 80, 30), PdfSharpCore.Drawing.XStringFormats.TopLeft);
                }
            }

            using var ms = new MemoryStream();
            finalDoc.Save(ms, false);
            return ms.ToArray();
        }

        // ── Helpers ──

        private static void RenderSectionTitle(ColumnDescriptor col, string title)
        {
            col.Item().PaddingBottom(5).Text(title).FontSize(13).Bold().FontColor(Colors.Blue.Darken1);
        }

        private static void AddFieldRow(TableDescriptor table, string label, string value)
        {
            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4)
                .Text(label).Bold().FontSize(9).FontColor(Colors.Grey.Darken1);
            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4)
                .Text(value).FontSize(9);
        }

        private static string NaIfEmpty(string? value) =>
            string.IsNullOrWhiteSpace(value) ? "N/A" : value;

        private static string FileLabel(List<FileMetadataDto>? files)
        {
            if (files == null || files.Count == 0)
            {
                return "N/A";
            }

            if (files.Count == 1)
            {
                return NaIfEmpty(files[0].Name);
            }

            return $"{files.Count} files";
        }

        private static void RenderFileList(ColumnDescriptor col, string label, List<FileMetadataDto>? files)
        {
            if (files == null || files.Count == 0)
            {
                col.Item().PaddingBottom(3).Text($"  • {label}: N/A").FontSize(9);
                return;
            }

            col.Item().PaddingBottom(1).Text($"  • {label}:").FontSize(9);
            foreach (var f in files)
                col.Item().PaddingBottom(1).Text($"      - {NaIfEmpty(f.Name)}").FontSize(8);
            col.Item().PaddingBottom(3);
        }
    }
}
