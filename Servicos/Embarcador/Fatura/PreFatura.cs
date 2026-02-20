using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;

namespace Servicos.Embarcador.Fatura
{
    public class PreFatura : ServicoBase
    {
        public PreFatura(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.ObjetosDeValor.Embarcador.Fatura.PreFatura ProcessarArquivoPreFatura(ExcelPackage package, Repositorio.UnitOfWork unitOfWork)
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.First();

            Dominio.ObjetosDeValor.Embarcador.Fatura.PreFatura retornoPreFatura = new Dominio.ObjetosDeValor.Embarcador.Fatura.PreFatura();
            List<Dominio.ObjetosDeValor.Embarcador.Fatura.CTePreFatura> listaCTePreFatura = new List<Dominio.ObjetosDeValor.Embarcador.Fatura.CTePreFatura>();

            var cellValueDoct = "";
            var cellValueValor = "";
            var MsgRetorno = "";
            int numeroCTe = 0;
            decimal valorCTe = 0;

            for (var i = 1; i <= worksheet.Dimension.End.Row; i++)
            {
                cellValueDoct = Utilidades.String.RemoveDiacritics(worksheet.Cells[1, 1].Text);
                cellValueValor = Utilidades.String.RemoveDiacritics(worksheet.Cells[1, 3].Text);
                if (cellValueDoct != "Dcto" && cellValueValor != "Valor")
                {
                    MsgRetorno = "A primeira linha e a primeira coluna deve estar como 'Dcto'<br/>" +
                        "A primeira linha e a segunda coluna deve estar como 'Valor'<br/>";
                    break;
                }
                if (i > 1)
                {
                    try
                    {
                        cellValueDoct = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, 1].Text);
                        cellValueValor = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, 3].Text);
                        cellValueValor = cellValueValor.Replace("R$", "");
                        cellValueValor = cellValueValor.Trim();
                        Dominio.ObjetosDeValor.Embarcador.Fatura.CTePreFatura cte = new Dominio.ObjetosDeValor.Embarcador.Fatura.CTePreFatura();

                        numeroCTe = 0;
                        valorCTe = 0;
                        int.TryParse(cellValueDoct, out numeroCTe);
                        decimal.TryParse(cellValueValor, out valorCTe);
                        if (numeroCTe > 0 && valorCTe > 0)
                        {
                            cte.NumeroCTe = numeroCTe;
                            cte.ValorCTe = valorCTe;

                            listaCTePreFatura.Add(cte);
                        }
                        else
                        {
                            MsgRetorno = MsgRetorno + "CTe: " + cellValueDoct + " Valor: " + cellValueValor + " estão com valores zerados.<br/>";
                        }
                    }
                    catch (Exception)
                    {
                        cellValueDoct = "";
                        cellValueValor = "";
                    }
                    if (i >= worksheet.Dimension.End.Row)
                    {
                        break;
                    }
                }
            }

            retornoPreFatura.CTePreFatura = listaCTePreFatura;
            retornoPreFatura.MsgRetorno = MsgRetorno;
            return retornoPreFatura;
        }
    }
}
