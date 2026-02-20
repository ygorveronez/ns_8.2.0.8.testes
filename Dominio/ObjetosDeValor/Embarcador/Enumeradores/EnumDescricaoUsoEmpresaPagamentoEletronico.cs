namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum DescricaoUsoEmpresaPagamentoEletronico
    {
        Nenhum = 0,
        PagamentoSalario = 1,
        PagamentoFerias = 2,
        AdiantamentoSalario = 3,
        DecimoTerceiroSalario = 4,
        AdiantamentoDecimoTerceiroSalario = 5,
        BonusMetaVenda = 6,
        BonusPerformance = 7,
        ComissaoSobreVendas = 8,
        ParticipacaoLucrosDaEmpresa = 9,
        BonusPorProducao = 10,
        DecimoQuartoSalario = 11,
        HoraExtra = 12,
        GratificacaoPremio = 13,
        RescisaoContratual = 14,
        AdiantamentoDiarias = 15
    }

    public static class DescricaoUsoEmpresaPagamentoEletronicoHelper
    {
        public static string ObterDescricao(this DescricaoUsoEmpresaPagamentoEletronico finalidadePagamento)
        {
            switch (finalidadePagamento)
            {
                case DescricaoUsoEmpresaPagamentoEletronico.Nenhum: return "Nenhum";
                case DescricaoUsoEmpresaPagamentoEletronico.PagamentoSalario: return "PAGAMENTO DE SALARIO";
                case DescricaoUsoEmpresaPagamentoEletronico.PagamentoFerias: return "PAGAMENTO DE FERIAS";
                case DescricaoUsoEmpresaPagamentoEletronico.AdiantamentoSalario: return "ADIANTAMENTO DE SALARIO";
                case DescricaoUsoEmpresaPagamentoEletronico.DecimoTerceiroSalario: return "13o. SALARIO";
                case DescricaoUsoEmpresaPagamentoEletronico.AdiantamentoDecimoTerceiroSalario: return "ADIANTAMENTO 13o. SALARIO";
                case DescricaoUsoEmpresaPagamentoEletronico.BonusMetaVenda: return "BONUS POR METAS DE VENDAS";
                case DescricaoUsoEmpresaPagamentoEletronico.BonusPerformance: return "BONUS POR PERFORMANCE";
                case DescricaoUsoEmpresaPagamentoEletronico.ComissaoSobreVendas: return "COMISSAO SOBRE VENDAS";
                case DescricaoUsoEmpresaPagamentoEletronico.ParticipacaoLucrosDaEmpresa: return "PARTICIPACAO LUCROS DA EMPRESA";
                case DescricaoUsoEmpresaPagamentoEletronico.BonusPorProducao: return "BONUS POR PRODUCAO / SERVICO";
                case DescricaoUsoEmpresaPagamentoEletronico.DecimoQuartoSalario: return "14o. SALARIO";
                case DescricaoUsoEmpresaPagamentoEletronico.HoraExtra: return "HORA EXTRA";
                case DescricaoUsoEmpresaPagamentoEletronico.GratificacaoPremio: return "GRATIFICACAO / PREMIO";
                case DescricaoUsoEmpresaPagamentoEletronico.RescisaoContratual: return "RESCISAO CONTRATUAL";
                case DescricaoUsoEmpresaPagamentoEletronico.AdiantamentoDiarias: return "ADIANTAMENTO DE DIÁRIAS";
                default: return string.Empty;
            }
        }
    }
}
