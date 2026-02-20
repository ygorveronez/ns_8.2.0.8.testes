using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.INTPFAR
{
    public class INTPFAR //000
    {
        public string TipoRegistro { get; set; }
        public string NomeInterface { get; set; }
        public string Versao { get; set; }
        public string NomeArquivo { get; set; }

        public string NomeArquivoSemExtencao { get; set; }
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public CabecalhoFatura CabecalhoFatura { get; set; }
        public List<CabecalhoFatura> CabecalhosFatura { get; set; }
        public List<Fatura> Fatura { get; set; }
        public RodapeFatura RodapeFatura { get; set; }
        public DateTime DataGeracao { get; set; }
        public string NumeroControle { get; set; }
        public int TotalLancamentos { get; set; }
        public string Contadores { get; set; }
        public string Somatorios { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
