using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Dominio.ObjetosDeValor.Grid;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Servicos.Embarcador.Exportacao
{
    public class ExcelExport
    {
        public byte[] GerarExcelGenerico<T>(List<T> dados, List<HeaderExportacao> headers, Dictionary<string, Color> coresFixas = null, string nomePlanilha = "Exportacao")
        {
            if (dados == null || dados.Count == 0 || headers == null)
                return null;

            using ExcelPackage package = new ExcelPackage();
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(nomePlanilha);

            coresFixas ??= new Dictionary<string, Color>();

            int col = 1, row = 1;

            List<string> colunasComDados = new List<string>();
            IDictionary<string, object> primeiraLinha = dados.FirstOrDefault() as IDictionary<string, object>;

            if (primeiraLinha != null)
            {
                foreach (var headerItem in headers)
                {
                    if (primeiraLinha.ContainsKey(headerItem.name))
                    {
                        colunasComDados.Add(headerItem.name);

                        string nomeHeader = !string.IsNullOrEmpty(headerItem.title)
                            ? headerItem.title
                            : headerItem.name;
                        worksheet.Cells[row, col].Value = nomeHeader;
                        worksheet.Cells[row, col].Style.Font.Bold = true;
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.White);
                        worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.Black);

                        if (coresFixas.TryGetValue(headerItem.name, out var corHeader))
                        {
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(corHeader);
                        }

                        col++;
                    }
                }
            }

            row = 2;
            foreach (T linha in dados)
            {
                IDictionary<string, object> linhaDicionario = linha as IDictionary<string, object>;
                if (linhaDicionario == null)
                    continue;

                col = 1;
                foreach (var coluna in colunasComDados)
                {
                    ExcelRange cell = worksheet.Cells[row, col];

                    if (linhaDicionario.TryGetValue(coluna, out var valor))
                    {
                        cell.Value = valor?.ToString() ?? "N/A";
                    }
                    else
                    {
                        cell.Value = "N/A";
                    }

                    if (coresFixas.TryGetValue(coluna, out var cor))
                    {
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(cor);
                    }

                    col++;
                }
                row++;
            }

            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }
    }
}
