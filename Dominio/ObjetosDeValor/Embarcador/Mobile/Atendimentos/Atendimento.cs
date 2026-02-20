using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos
{
    public class Atendimento
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public string Descricao { get; set; }
        public int ProtocoloCarga { get; set; }
        public string DescricaoCarga { get; set; }
        public int CodigoMotivo { get; set; }
        public string DescricaoMotivo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento Tipo { get; set; }
        public Dominio.Enumeradores.TipoTomador? TipoCliente { get; set; }
        public string QrCodeFuncionario { get; set; }
        public double CNPJCliente { get; set; }
        public string DescricaoCliente { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado Situacao { get; set; }
        public string DescricaoSituacao { get; set; }
        public string DataCriacao { get; set; }
        public string DataReentrega { get; set; }
        public string DataRetencaoInicio { get; set; }
        public string DataRetencaoFim { get; set; }
        public decimal TempoRetencao { get; set; }
        public bool RetencaoBau { get; set; }
        public string Observacao { get; set; }
        public string Analises { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Ocorrencia> Ocorrencias { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Analise> HistoricoAnalises { get; set; }
        public string NumeroOcorrencia { get; set; }
        public string DataEntradaRaio { get; set; }
        public string DataSaidaRaio { get; set; }
        public string PlacaReboque { get; set; }
        public string Filial { get; set; }
    }
}
