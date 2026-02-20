using System;
using System.IO;

namespace Dominio.ObjetosDeValor.Relatorios;

public class ReportResult
{
    
    public Guid Id { get; set; }
    public ExecutionStatus ExecutionStatus { get; set; }
    public string FullPath { get; set; }
    
    public string FileName { get; set; }
    public string ErrorMessage { get; set; }


    public byte[] GetContentFile()
    {
        if (Utilidades.IO.FileStorageService.Storage.Exists(FullPath))
        {
            return Utilidades.IO.FileStorageService.Storage.ReadAllBytes(FullPath);
        }        

        return null;
    }
}