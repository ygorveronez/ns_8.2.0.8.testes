using System;

namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public class NFSeManual
    {
        public int Numero { get; set; }
        public int Serie { get; set; }
        public bool IncluirISSBC { get; set; }
        public decimal AliquotaISS { get; set; }
        public decimal PercentualRetencao { get; set; }
        public decimal ValorBaseCalculo { get; set; }
        public decimal ValorRetido { get; set; }
        public decimal ValorISS { get; set; }
        public decimal ValorFrete { get; set; }
        public string Observacao { get; set; }
        public DateTime? DataEmissao { get; set; }
        public Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }
    }
}
