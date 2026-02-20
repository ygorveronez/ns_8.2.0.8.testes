using System;
using ReportApi.DTO;

namespace ReportApi.Extensions;

public static class EnumExtensions
{


    public static string GetExtension(this FileType fileType)
    {
        switch (fileType)
        {
            case FileType.PDF:
                return ".pdf";
            case FileType.CSV:
                return ".csv";
            case FileType.EXCEL:
                return ".xls";
            default:
                throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
        }
    }
    
}