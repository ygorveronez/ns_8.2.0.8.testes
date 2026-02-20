namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EtapaGestaoDevolucao
    {
        GestaoDeDevolucao = 0,
        DefinicaoTipoDevolucao = 1,
        AprovacaoTipoDevolucao = 2,
        OrdemeRemessa = 3,
        GeracaoOcorrenciaDebito = 4,
        DefinicaoLocalColeta = 5,
        GeracaoCargaDevolucao = 6,
        AgendamentoParaDescarga = 7,
        GestaoCustoContabil = 8,
        Agendamento = 9,
        AprovacaoDataDescarga = 10,
        Monitoramento = 11,
        GeracaoLaudo = 12,
        AprovacaoLaudo = 13,
        IntegracaoLaudo = 14,
        CenarioPosEntrega = 15,
        RegistroDocumentosPallet = 16,
        DocumentacaoEntradaFiscal = 17,
        AprovacaoCenarioPosEntrega = 18
    }

    public static class EtapaGestaoDevolucaoHelper
    {
        public static string ObterDescricao(this EtapaGestaoDevolucao EtapaGestaoDevolucao)
        {
            switch (EtapaGestaoDevolucao)
            {
                case EtapaGestaoDevolucao.GestaoDeDevolucao: return "Gestão de Devolução";
                case EtapaGestaoDevolucao.DefinicaoTipoDevolucao: return "Definir Tipo de Devolução";
                case EtapaGestaoDevolucao.AprovacaoTipoDevolucao: return "Aprovação";
                case EtapaGestaoDevolucao.OrdemeRemessa: return "Ordem e Remessa";
                case EtapaGestaoDevolucao.GeracaoOcorrenciaDebito: return "Geração Ocorrência Débito";
                case EtapaGestaoDevolucao.DefinicaoLocalColeta: return "Definição Local de Coleta";
                case EtapaGestaoDevolucao.GeracaoCargaDevolucao: return "Geração Carga de Devolução";
                case EtapaGestaoDevolucao.AgendamentoParaDescarga: return "Agendamento para Descarga";
                case EtapaGestaoDevolucao.GestaoCustoContabil: return "Gestão de Custo e Contábil";
                case EtapaGestaoDevolucao.Agendamento: return "Agendamento";
                case EtapaGestaoDevolucao.AprovacaoDataDescarga: return "Aprovação Data Descarga";
                case EtapaGestaoDevolucao.Monitoramento: return "Monitoramento";
                case EtapaGestaoDevolucao.GeracaoLaudo: return "Geração de Laudo";
                case EtapaGestaoDevolucao.AprovacaoLaudo: return "Aprovação Laudo";
                case EtapaGestaoDevolucao.IntegracaoLaudo: return "Integração Laudo";
                case EtapaGestaoDevolucao.CenarioPosEntrega: return "Pós Entrega";
                case EtapaGestaoDevolucao.RegistroDocumentosPallet: return "Registros dos documentos";
                case EtapaGestaoDevolucao.DocumentacaoEntradaFiscal: return "Documentação de entrada fiscal";
                case EtapaGestaoDevolucao.AprovacaoCenarioPosEntrega: return "Aprovação Pós Entrega";  //Avaliação e Liberação
                default: return string.Empty;
            }
        }
    }
}