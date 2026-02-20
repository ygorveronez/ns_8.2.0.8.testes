namespace Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete
{
    public sealed class ImportacaoTabelaFreteParametro
    {
        public int CodigoDestino { get; set; }

        public int CodigoEmpresa { get; set; }

        public int CodigoOrigem { get; set; }

        public int CodigoTabelaFrete { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int CodigoVigencia { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao? TipoPagamento { get; set; }

        public bool FreteValidoParaQualquerDestino { get; set; }

        public bool FreteValidoParaQualquerOrigem { get; set; }

        public int IndiceColunaCepDestino { get; set; }

        public int IndiceColunaCepDestinoDiasUteis { get; set; }

        public int IndiceColunaTomadorDestino { get; set; }

        public int IndiceColunaLeadTime { get; set; }

        public int IndiceColunaCepOrigem { get; set; }

        public int IndiceColunaClienteDestino { get; set; }

        public int IndiceColunaClienteOrigem { get; set; }

        public int IndiceColunaCodigoIntegracao { get; set; }

        public int IndiceColunaDestino { get; set; }

        public int IndiceColunaEstadoDestino { get; set; }

        public int IndiceColunaEstadoOrigem { get; set; }

        public int IndiceColunaOrigem { get; set; }

        public int IndiceColunaParametroBase { get; set; }

        public int IndiceColunaRegiaoDestino { get; set; }

        public int IndiceColunaRegiaoOrigem { get; set; }

        public int IndiceColunaRotaDestino { get; set; }

        public int IndiceColunaRotaOrigem { get; set; }

        public int IndiceColunaTransportador { get; set; }

        public int IndiceColunaPrioridadeUso { get; set; }

        public int IndiceColunaFronteira { get; set; }
        public int IndiceColunaKMSistema { get; set; }
        public int IndiceLinhaIniciarImportacao { get; set; }
        public int IndiceColunaCanalEntrega { get; set; }

        public bool NaoAtualizarValoresZerados { get; set; }

        public bool NaoValidarTabelasExistentes { get; set; }

        public dynamic Parametros { get; set; }

        public int IndiceColunaSeg { get; set; }
        public int IndiceColunaTer { get; set; }
        public int IndiceColunaQua { get; set; }
        public int IndiceColunaQui { get; set; }
        public int IndiceColunaSex { get; set; }
        public int IndiceColunaSab { get; set; }
        public int IndiceColunaDom { get; set; }
        public int IndiceColunaTipoOperacao { get; set; }
        public int IndiceColunaTipoDeCarga { get; set; }
        public int IndiceColunaLeadTimeDias { get; set; }

        public int IndiceColunaContratoTransportador { get; set; }
        public int IndiceColunaGrupoCarga { get; set; }
        public int IndiceColunaGerenciarCapacidade { get; set; }
        public int IndiceColunaEstruturaTabela { get; set; }
        public int IndiceColunaCapacidadeOTM { get; set; }
        public int IndiceColunaQuantidadeEntregas { get; set; }
        public int IndiceColunaPercentualRota { get; set; }
    }
}
