using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Frete;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest
{
    public class DocumentoTransporte
    {
        public string NumeroCarga { get; set; }
        public bool DocumentoGlobalizado { get; set; }
        public string DataCriacaoCarga { get; set; }
        public string DataInicioCarregamento { get; set; }
        public string DataTerminoCarregamento { get; set; }
        public Dominio.ObjetosDeValor.Filial Filial { get; set; }
        public string[] Lacres { get; set; }
        public List<Motorista> Motoristas { get; set; }
        public TipoCargaEmbarcador TipoCargaEmbarcador { get; set; }
        public Embarcador.Carga.TipoOperacao TipoOperacao { get; set; }
        public Veiculo Veiculo { get; set; }
        public Embarcador.Pessoas.Empresa TransportadoraEmitente { get; set; }
        public List<Dominio.ObjetosDeValor.WebService.Rest.Unilever.Pedido> Pedido { get; set; }
        public InformacoesAdicionais InformacoesAdicionais { get; set; }
        public UnidadeNegocio UnidadeNegocio { get; set; }
        public TipoDT TipoDT { get; set; }
        public string ExternalID1 { get; set; }
        public string ExternalID2 { get; set; }

        [JsonIgnore]
        public string ControleIntegracaoEmbarcador { get; set; }

        public StatusEmbarque StatusEmbarque { get; set; }
        public decimal DistanciaTotal { get; set; }
        public FreteRota FreteRota { get; set; }
        public string DataAgendamento { get; set; }
        public string DataPreparacaoPosCarregamento { get; set; }
        public string StatusLoger { get; set; }
        public string DataRealFaturamento { get; set; }
        public string ProcessamentoEspecial { get; set; }
        public string DataPrevisaoInicioViagem { get; set; }
        public List<Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.DocumentoTransporte.EventosDT> EventosDT { get; set; }
    }
}