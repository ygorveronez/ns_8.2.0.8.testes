namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum AcaoTratativaIrregularidade
    {
        EfetuarCadastrosNecessarios = 1,
        PagarConformeFRS = 2,
        PagarConformeCTeNFS = 3,
        PagarConformeOutroValor = 4,
        CampoInserirPesoReal = 5,
        SubstituirDocumento = 6,
        RejeitarSubstituicao = 7,
        CancelarFRSCancelarCarga = 8,
        CampoInserirNumeroOcorrencia = 9,
        AutorizadoSubstituicao = 10,
        NaoAutorizadoSubstituicao = 11,
        RetornarFluxo = 12,
        NaoProcedePagamento = 13,
        ProcedePagamento = 14,
        CriarDCContingencia = 15,
        ErroCalculo = 16,
        SemTarifa = 17,
        TarifaCadastrada = 18,
        InserirChaveAcessoNFeValida = 19,
        AnexarComplementar = 20,
        AplicarEventoDesacordo = 21,
        CTeCorretoCadastrosCorrigidos = 22,
        EmitirCCeRealizarUpload = 23,
        ProblemaOperacional = 24,
        ProblemaComercial = 25,
        DesacordoSubstituicaoDocumento = 26,
        ErroFaturamento = 27,
        FormacaoCorreta = 28,
        FormacaoIncorretaInformarFormacaoCorreta = 29,
        NFSCorretaCadastrosCorrigidos = 30,
        Aprovado = 31,
        Rejeitado = 32,
        NecessarioCartaCorrecao = 33
    }

    public static class AcaoTratativaIrregularidadeHelper
    {
        public static string ObterDescricao(this AcaoTratativaIrregularidade o)
        {
            switch (o)
            {
                case AcaoTratativaIrregularidade.EfetuarCadastrosNecessarios: return "Efetuar os cadastros necessários";
                case AcaoTratativaIrregularidade.PagarConformeFRS: return "Pagar conforme FRS";
                case AcaoTratativaIrregularidade.PagarConformeCTeNFS: return "Pagar conforme CT-e/NFS";
                case AcaoTratativaIrregularidade.PagarConformeOutroValor: return "Pagar conforme outro Valor";
                case AcaoTratativaIrregularidade.CampoInserirPesoReal: return "Campo para Inserir Peso real";
                case AcaoTratativaIrregularidade.SubstituirDocumento: return "Substituir Documento";
                case AcaoTratativaIrregularidade.RejeitarSubstituicao: return "Rejeitar Substituição";
                case AcaoTratativaIrregularidade.CancelarFRSCancelarCarga: return "Cancelar FRS/Cancelar Carga";
                case AcaoTratativaIrregularidade.CampoInserirNumeroOcorrencia: return "Campo para inserir número da ocorrência";
                case AcaoTratativaIrregularidade.AutorizadoSubstituicao: return "Autorizado Substituição";
                case AcaoTratativaIrregularidade.NaoAutorizadoSubstituicao: return "Não Autorizado Substituição";
                case AcaoTratativaIrregularidade.RetornarFluxo: return "Retornar Fluxo";
                case AcaoTratativaIrregularidade.NaoProcedePagamento: return "Não Procede o Pagamento";
                case AcaoTratativaIrregularidade.ProcedePagamento: return "Procede o Pagamento";
                case AcaoTratativaIrregularidade.CriarDCContingencia: return "Criar DC Contingência";
                case AcaoTratativaIrregularidade.ErroCalculo: return "Erro de Cálculo";
                case AcaoTratativaIrregularidade.SemTarifa: return "Sem Tarifa";
                case AcaoTratativaIrregularidade.TarifaCadastrada: return "Tarifa Cadastrada";
                case AcaoTratativaIrregularidade.InserirChaveAcessoNFeValida: return "Inserir chave de acesso NF-e Válida";
                case AcaoTratativaIrregularidade.AnexarComplementar: return "Anexar Complementar";
                case AcaoTratativaIrregularidade.AplicarEventoDesacordo: return "Aplicar Evento Desacordo";
                case AcaoTratativaIrregularidade.CTeCorretoCadastrosCorrigidos: return "CT-e Correto - Cadastros Corrigidos";
                case AcaoTratativaIrregularidade.EmitirCCeRealizarUpload: return "Emitir CC-e e realizar Upload";
                case AcaoTratativaIrregularidade.ProblemaOperacional: return "Problema Operacional (Divergência na Carga)";
                case AcaoTratativaIrregularidade.ProblemaComercial: return "Problema Comercial";
                case AcaoTratativaIrregularidade.DesacordoSubstituicaoDocumento: return "Desacordo - Substituição do Documento";
                case AcaoTratativaIrregularidade.ErroFaturamento: return "Erro de faturamento";
                case AcaoTratativaIrregularidade.FormacaoCorreta: return "Formação Correta";
                case AcaoTratativaIrregularidade.FormacaoIncorretaInformarFormacaoCorreta: return "Formação Incorreta - Informar a formação correta";
                case AcaoTratativaIrregularidade.NFSCorretaCadastrosCorrigidos: return "NFS-e Correta - Cadastros Corrigidos";
                case AcaoTratativaIrregularidade.Aprovado: return "Aprovado";
                case AcaoTratativaIrregularidade.Rejeitado: return "Rejeitado";
                case AcaoTratativaIrregularidade.NecessarioCartaCorrecao: return "Necessário Carta de Correação";
                default: return string.Empty;
            }
        }
    }
}
