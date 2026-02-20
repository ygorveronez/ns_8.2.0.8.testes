using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb
{
    public class envPontosParadaViagem
    {
        public string CodigoExterno { get; set; }
        public int? Tipo { get; set; }
        public DateTime? DataHoraPrevisaoInicioViagem { get; set; }
        public DateTime? DataHoraPrevisaoFimViagem { get; set; }
        public decimal ValorTotalCarga { get; set; }
        public int? VinculoCondutor { get; set; }
        public int? VinculoVeiculoTracao { get; set; }
        public string CodigoExternoOperacao { get; set; }
        public List<envPontoControleWrapper> PontosControle { get; set; }
    }

    public class envPontoControleWrapper
    {
        public envPontoControle PontoControle { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle Tipo { get; set; }
        public DateTime? DataHoraPrevisaoInicio { get; set; }
        public DateTime? DataHoraPrevisaoFim { get; set; }
        public bool Visita { get; set; }
        public string DescricaoVisita { get; set; }
        public List<envProduto> Produtos { get; set; }
    }

    public class envProduto
    {
        public string CodigoExterno { get; set; }
        public decimal? MetragemCubica { get; set; }
        public string Nome { get; set; }
        public int? Quantidade { get; set; }
        public string NotaFiscal { get; set; }
        public string NumeroShipment { get; set; }
        public string ValorVenda { get; set; }
        public string TempoAbastecimentoSegundos { get; set; }
        public List<envReprogramacaoWrapper> Reprogramacoes { get; set; }
    }

    public class envReprogramacaoWrapper
    {
        public envReprogramacao Reprogramacao { get; set; }
    }

    public class envReprogramacao
    {
        public DateTime Data { get; set; }
        public bool? Abastecimento { get; set; }
        public string DescricaoAbastecimento { get; set; }
    }


}
