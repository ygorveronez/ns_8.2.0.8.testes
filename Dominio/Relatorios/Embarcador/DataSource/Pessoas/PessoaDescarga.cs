using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pessoas
{
    public class PessoaDescarga
    {
        public int Codigo { get; set; }
        public string PessoaOrigem { get; set; }
        public string PessoaDestino { get; set; }
        public string HoraInicio { get; set; }
        public string TipoPessoaOrigem { get; set; }
        public string TipoPessoaDestino { get; set; }
        public double CnpjOrigem { get; set; }
        public double CnpjDestino { get; set; }
        public string HoraFim { get; set; }
        public decimal ValorPorPallet { get; set; }
        public decimal ValorPorVolume { get; set; }
        private bool DeixaReboqueParaDescarga { get; set; }

        public string DeixaReboqueParaDescargaFormatada
        {
            get
            {
                return DeixaReboqueParaDescarga == true ? "Sim" : "Não";
            }
        }

        public virtual string CnpjOrigemFormatado
        {
            get
            {
                if (this.TipoPessoaOrigem == null)
                    return "";
                if (this.TipoPessoaOrigem.Equals("E"))
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    return this.TipoPessoaOrigem.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CnpjOrigem) : String.Format(@"{0:000\.000\.000\-00}", this.CnpjOrigem);
                }
            }
        }
        public virtual string CnpjDestinoFormatado
        {
            get
            {
                if (this.TipoPessoaDestino == null)
                    return "";
                if (this.TipoPessoaDestino.Equals("E"))
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    return this.TipoPessoaDestino.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CnpjDestino) : String.Format(@"{0:000\.000\.000\-00}", this.CnpjDestino);
                }
            }
        }

    }
}