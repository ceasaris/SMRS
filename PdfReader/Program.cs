using System;
using System.IO;
using UglyToad.PdfPig;

namespace PdfReaderApp {
    class Program {
        static void Main(string[] args) {
            string[] pdfFiles = {
                @"C:\project\c#\SMRS\SMRS_Proposal_v2.pdf",
                @"C:\project\c#\SMRS\SMRS_DB_Diagram.pdf"
            };

            foreach (var file in pdfFiles) {
                Console.WriteLine($"\n--- START OF {Path.GetFileName(file)} ---\n");
                if (File.Exists(file)) {
                    try {
                        using (PdfDocument document = PdfDocument.Open(file)) {
                            foreach (var page in document.GetPages()) {
                                Console.WriteLine(page.Text);
                            }
                        }
                    } catch (Exception ex) {
                        Console.WriteLine($"Error reading PDF: {ex.Message}");
                    }
                } else {
                    Console.WriteLine("File not found.");
                }
                Console.WriteLine($"\n--- END OF {Path.GetFileName(file)} ---\n");
            }
        }
    }
}
