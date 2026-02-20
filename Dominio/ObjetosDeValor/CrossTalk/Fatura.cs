using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.CrossTalk
{
    public class Fatura
    {
        public string CNPJTransportador { get; set; }

        public string NumeroFatura { get; set; }

        public DateTime DataEmissao { get; set; }

        public string SerieFatura { get; set; }

        public DateTime DataVencimento { get; set; }

        public string CNPJAvon { get; set; }

        public decimal ValorTotal { get; set; }

        public List<string> CTes { get; set; }

        public string ObterArquivo()
        {
            StringBuilder registro = new StringBuilder();

            registro.Append("0|");
            registro.Append(this.CNPJTransportador).Append("|");
            registro.Append(this.NumeroFatura).Append("|");
            registro.Append(this.DataEmissao.ToString("yyyyMMdd")).Append("|");
            registro.Append(this.SerieFatura).Append("|");
            registro.Append(this.DataVencimento.ToString("yyyyMMdd")).Append("|");
            registro.Append(this.CNPJAvon).Append("|");
            registro.Append(this.ValorTotal.ToString("n2").Replace(".", "").Replace(",", ""));

            registro.AppendLine();

            foreach (string cte in this.CTes)
                registro.Append("1|").Append(cte).AppendLine();

            return registro.ToString();
        }
    }
}
