namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaJanelaCarregamentoAdicional
    {
        DocumentosEmitidos = 1,
        DadosTransporteInformados = 2,
        SemDadosTransporte = 3,
        AtrasoChegadaVeiculo = 4,
        ForaPeriodoCarregamento = 5,
        NotaFiscalEmitida = 6
    }

    public static class SituacaoCargaJanelaCarregamentoAdicionalHelper
    {
        public static string ObterDescricao(this SituacaoCargaJanelaCarregamentoAdicional situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaJanelaCarregamentoAdicional.AtrasoChegadaVeiculo: return "Atraso na Chegada do Veículo";
                case SituacaoCargaJanelaCarregamentoAdicional.DadosTransporteInformados: return "Dados de Transporte Informados";
                case SituacaoCargaJanelaCarregamentoAdicional.DocumentosEmitidos: return "Documentos Emitidos";
                case SituacaoCargaJanelaCarregamentoAdicional.ForaPeriodoCarregamento: return "Fora do Período de Carregamento";
                case SituacaoCargaJanelaCarregamentoAdicional.SemDadosTransporte: return "Sem Dados de Transporte";
                case SituacaoCargaJanelaCarregamentoAdicional.NotaFiscalEmitida: return "Nota Fiscal Emitida";
                default: return string.Empty;
            }
        }
    }
}
