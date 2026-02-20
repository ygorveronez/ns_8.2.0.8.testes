using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding.ImportacaoRota
{
    public class Rota
    {
        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public string FlagOrigem { get; set; }

        public string FlagDestino { get; set; }

        public List<CEP> CEPOrigem { get; set; }

        public List<CEP> CEPDestino { get; set; }

        public List<CodigoDescricao> CidadeOrigem { get; set; }

        public List<CodigoDescricao> CidadeDestino { get; set; }

        public List<CodigoDescricao> ClienteOrigem { get; set; }

        public List<CodigoDescricao> ClienteDestino { get; set; }

        public List<CodigoDescricao> EstadoOrigem { get; set; }

        public List<CodigoDescricao> EstadoDestino { get; set; }

        public decimal QuilometragemMedia { get; set; }

        public string Frequencia { get; set; }

        public decimal Peso { get; set; }

        public int AdicionalAPartirDaEntregaNumero { get; set; }

        public int NumeroEntrega { get; set; }

        public string Observacao { get; set; }

        public List<CodigoDescricao> PaisOrigem { get; set; }

        public List<CodigoDescricao> PaisDestino { get; set; }

        public List<CodigoDescricao> RegiaoOrigem { get; set; }

        public List<CodigoDescricao> RegiaoDestino { get; set; }

        public List<CodigoDescricao> RotaOrigem { get; set; }

        public List<CodigoDescricao> RotaDestino { get; set; }

        public decimal ValorCargaMes { get; set; }

        public string Volume { get; set; }

        public List<CodigoDescricao> ModeloVeicular { get; set; }

        public List<CodigoDescricao> FiliaisParticipante { get; set; }

        public List<CodigoDescricao> TipoCarga { get; set; }

        public List<Baseline> Baseline { get; set; }

        public Tomador Tomador { get; set; }

        public CodigoDescricao GrupoModeloVeicular { get; set; }

        public CodigoDescricao ModeloCarroceria { get; set; }

        public int FrequenciaMensalComAjudante { get; set; }

        public int QuantidadeAjudantePorVeiculo { get; set; }

        public int MediaEntregasFracionada { get; set; }

        public int MaximaEntregasFacionada { get; set; }

        public string Inconterm { get; set; }

        public int QuantidadeViagensPorAno { get; set; }

        public int VolumeTonAno { get; set; }

        public int VolumeTonViagem { get; set; }

        public decimal ValorMedioNFe { get; set; }

        public string TempoColeta { get; set; }

        public string TempoDescarga { get; set; }

        public string Compressor { get; set; }
    }
}
