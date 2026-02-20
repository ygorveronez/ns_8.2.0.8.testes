using System;

namespace Dominio.ObjetosDeValor.Embarcador.PreCarga
{
    public class PreCargaAlteracaoImportacao
    {
        public int CodigoPreCarga { get; set; }
        public string NumeroPreCarga { get; set; }

        public DateTime? DataPrevisaoInicioViagem { get; set; }
        public DateTime? DataPrevisaoInicioViagemAnterior { get; set; }

        public DateTime? PrevisaoChegadaDestinatario { get; set; }
        public DateTime? PrevisaoChegadaDestinatarioAnterior { get; set; }

        public DateTime? PrevisaoChegadaDoca { get; set; }
        public DateTime? PrevisaoChegadaDocaAnterior { get; set; }

        public DateTime? DataSaidaLojaPrevista { get; set; }
        public DateTime? PrevisaoSaidaDestinatario { get; set; }
        
        public DateTime? DataPrevisaoFimViagem { get; set; }
        public DateTime? DataPrevisaoFimViagemAnterior { get; set; }
        


        public string DocaCarregamento { get; set; }
        public string DocaCarregamentoAnterior { get; set; }

        public string CargaRetorno { get; set; }
        public string CargaRetornoAnterior { get; set; }

        public string NumeroPedido { get; set; }
        public string Observacao { get; set; }
        public decimal PesoPedido { get; set; }
        public decimal PesoPlanejamento { get; set; }
        public decimal QuantidadePallets { get; set; }
        public int TipoCarga { get; set; }
        public int ModeloVeicularCarga { get; set; }
        public int TipoOperacao { get; set; }
        public long Destinatario { get; set; }
        public long Remetente { get; set; }
        public int Filial { get; set; }
        public int Empresa { get; set; }
    }
}
