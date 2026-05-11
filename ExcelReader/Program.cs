using System;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;

class Program
{
    static void Main()
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        string filePath = @"C:\project\c#\SMRS\SMRS_Technical_Project_Plan.xlsx";
        
        if (!File.Exists(filePath)) {
            Console.WriteLine("File not found");
            return;
        }

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                foreach (DataTable table in result.Tables)
                {
                    Console.WriteLine($"\n=== SHEET: {table.TableName} ===");
                    
                    // Print header
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        Console.Write(table.Columns[i].ColumnName + "\t|\t");
                    }
                    Console.WriteLine("\n" + new string('-', 80));

                    // Print rows
                    foreach (DataRow row in table.Rows)
                    {
                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            var cellVal = row[i]?.ToString()?.Replace("\n", " ").Replace("\r", " ");
                            Console.Write((cellVal ?? "") + "\t|\t");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}
