using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao
{
    public class CargaProvisao
    {
        #region atributos
        public string? NumeroCarga { get; set; }
        public string? SituacaoCarga { get; set; }
        public string? Filial { get; set; }
        public string? CnpjTransportador { get; set; }
        public string? CodigoIntegracaoTransportador { get; set; }
        public string? Transportador { get; set; }
        public string? TabelaFrete { get; set; }
        public string? TipoCarga { get; set; }
        public string? TipoOperacao { get; set; }
        public string? CargaAgrupamento { get; set; }
        public DateTime? DataCriacaoCarga { get; set; }
        public DateTime? DataCriacaoCargaAgrupada { get; set; }
        public string? Carregamento { get; set; }
        public string? ModeloVeicular { get; set; }
        public string? Placa { get; set; }
        public string? Rota { get; set; }
        public List<LoteProvisao> LotesProvisao { get; set; }
        public List<CancelamentoCarga> CancelamentosCarga { get; set; }

        #endregion 
    }
}
