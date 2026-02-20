
/// <reference path="../../../ViewsScripts/Enumeradores/EnumAcaoTratativaIrregularidade.js" />
var EnumAcaoTratativaIrregularidade = function () {
    this.Todas = "",
    this.EfetuarCadastrosNecessarios = 1;
    this.PagarConformeFRS = 2;
    this.PagarConformeCTeNFS = 3;
    this.PagarConformeOutroValor = 4;
    this.CampoInserirPesoReal = 5;
    this.SubstituirDocumento = 6;
    this.RejeitarSubstituicao = 7;
    this.CancelarFRSCancelarCarga = 8;
    this.CampoInserirNumeroOcorrencia = 9;
    this.AutorizadoSubstituicao = 10;
    this.NaoAutorizadoSubstituicao = 11;
    this.RetornarFluxo = 12;
    this.NaoProcedePagamento = 13;
    this.ProcedePagamento = 14;
    this.CriarDCContingencia = 15;
    this.ErroCalculo = 16;
    this.SemTarifa = 17;
    this.TarifaCadastrada = 18;
    this.InserirChaveAcessoNFeValida = 19;
    this.AnexarComplementar = 20;
    this.AplicarEventoDesacordo = 21;
    this.CTeCorretoCadastrosCorrigidos = 22;
    this.EmitirCCeRealizarUpload = 23;
    this.ProblemaOperacional = 24;
    this.ProblemaComercial = 25;
    this.DesacordoSubstituicaoDocumento = 26;
    this.ErroFaturamento = 27;
    this.FormacaoCorreta = 28;
    this.FormacaoIncorretaInformarFormacaoCorreta = 29;
    this.NFSCorretaCadastrosCorrigidos = 30;
    this.Aprovado = 31;
    this.Rejeitado = 32;
    this.NecessarioCartaCorrecao = 33;
};


EnumAcaoTratativaIrregularidade.prototype = {
    obterOpcoesPorDescricao: function (gatilhoIrregularidade) {
        let listaPermitida = [];

        switch (gatilhoIrregularidade) {
      

            case EnumGatilhoIrregularidade.MIROBloqueioR:
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["PagarConformeFRS"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["PagarConformeCTeNFS"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["PagarConformeOutroValor"]));
                break;

            case EnumGatilhoIrregularidade.PendenteSubstituicaoDocumento:
            case EnumGatilhoIrregularidade.CTeCancelado:
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["SubstituirDocumento"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["RejeitarSubstituicao"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["CancelarFRSCancelarCarga"]));
                break;

            //case "Documento duplicado"://Rever
            //case "Emitidos fora do Multi Embarcador (Transp. com Certificado cedido)": //Rever
            //    listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AutorizadoSubstituicao"]));
            //    listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["NaoAutorizadoSubstituicao"]));
            //    listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["RetornarFluxo"]));
            //    break;

            /*case "Ocorrência Rejeitada/Cancelada": //rever*/
            case EnumGatilhoIrregularidade.SemLink:
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["NaoProcedePagamento"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["ProcedePagamento"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AutorizadoSubstituicao"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["NaoAutorizadoSubstituicao"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["RetornarFluxo"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["CampoInserirNumeroOcorrencia"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["CriarDCContingencia"]));
                break;

            case EnumGatilhoIrregularidade.SemCalculo:
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["ErroCalculo"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["SemTarifa"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["TarifaCadastrada"]));
                break;

            case EnumGatilhoIrregularidade.NFeCancelada:
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AutorizadoSubstituicao"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["NaoAutorizadoSubstituicao"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["InserirChaveAcessoNFeValida"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["ProcedePagamento"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["RetornarFluxo"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["CancelarFRSCancelarCarga"]));
                break;

            case EnumGatilhoIrregularidade.AliquotaICMSValorICMS:
            case EnumGatilhoIrregularidade.CNPJTransportadora:
            case EnumGatilhoIrregularidade.TomadorFreteUnilever:
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AnexarComplementar"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AplicarEventoDesacordo"]));
                break;

            case EnumGatilhoIrregularidade.CSTICMS:
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AutorizadoSubstituicao"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["NaoAutorizadoSubstituicao"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["RetornarFluxo"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["CTeCorretoCadastrosCorrigidos"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["CriarDCContingencia"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["SubstituirDocumento"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["NecessarioCartaCorrecao"]));
                break;

            case EnumGatilhoIrregularidade.NFeVinculadaAoFrete:
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AplicarEventoDesacordo"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["EmitirCCeRealizarUpload"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["NecessarioCartaCorrecao"]));
                break;
                
            case EnumGatilhoIrregularidade.ValorPrestacaoServico:
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["ProblemaOperacional"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["ProblemaComercial"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["DesacordoSubstituicaoDocumento"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["ErroFaturamento"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["PagarConformeFRS"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["PagarConformeCTeNFS"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["PagarConformeOutroValor"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AplicarEventoDesacordo"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["FormacaoCorreta"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["FormacaoIncorretaInformarFormacaoCorreta"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["SubstituirDocumento"]));
                break;
                

            case EnumGatilhoIrregularidade.MunicipioPrestacaoServico:
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AutorizadoSubstituicao"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["NaoAutorizadoSubstituicao"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["RetornarFluxo"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["NFSCorretaCadastrosCorrigidos"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["CriarDCContingencia"]));
                break;

            case EnumGatilhoIrregularidade.ValidarDadosNFSe:
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["Aprovado"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["Rejeitado"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["NaoAutorizadoSubstituicao"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["RetornarFluxo"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["CriarDCContingencia"]));
                break;

            case EnumGatilhoIrregularidade.ValorTotalReceber:
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["ProblemaOperacional"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["ProblemaComercial"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["DesacordoSubstituicaoDocumento"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["ErroFaturamento"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["PagarConformeFRS"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["PagarConformeCTeNFS"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["PagarConformeOutroValor"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AplicarEventoDesacordo"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["FormacaoCorreta"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["FormacaoIncorretaInformarFormacaoCorreta"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["SubstituirDocumento"]));
                break;

            case EnumGatilhoIrregularidade.CFOP:
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AnexarComplementar"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AutorizadoSubstituicao"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["NaoAutorizadoSubstituicao"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AplicarEventoDesacordo"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["RetornarFluxo"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["CTeCorretoCadastrosCorrigidos"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["CriarDCContingencia"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["SubstituirDocumento"]));
                break;

            case EnumGatilhoIrregularidade.Participantes:
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AutorizadoSubstituicao"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["NaoAutorizadoSubstituicao"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["RetornarFluxo"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AplicarEventoDesacordo"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["CriarDCContingencia"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["SubstituirDocumento"]));
                break;

            case EnumGatilhoIrregularidade.AliquotaISSValorISS:
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AnexarComplementar"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AutorizadoSubstituicao"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["NaoAutorizadoSubstituicao"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["AplicarEventoDesacordo"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["RetornarFluxo"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["CTeCorretoCadastrosCorrigidos"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["CriarDCContingencia"]));
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["SubstituirDocumento"]));
                break;

            default:
                listaPermitida.push(ObterObjetoPadrao(DADOS_GATILHOS["EfetuarCadastrosNecessarios"]));
                break;
        }

        return listaPermitida
    }
};

var EnumAcaoTratativaIrregularidade = Object.freeze(new EnumAcaoTratativaIrregularidade());

const DADOS_GATILHOS = {
    EfetuarCadastrosNecessarios: { valor: EnumAcaoTratativaIrregularidade.EfetuarCadastrosNecessarios, text: "Efetuar os cadastros necessários" },
    PagarConformeFRS: { valor: EnumAcaoTratativaIrregularidade.PagarConformeFRS, text: "Pagar conforme FRS" },
    PagarConformeCTeNFS: { valor: EnumAcaoTratativaIrregularidade.PagarConformeCTeNFS, text: "Pagar conforme CT-e/NFS" },
    PagarConformeOutroValor: { valor: EnumAcaoTratativaIrregularidade.PagarConformeOutroValor, text: "Pagar conforme outro Valor" },
    CampoInserirPesoReal: { valor: EnumAcaoTratativaIrregularidade.CampoInserirPesoReal, text: "Campo para Inserir Peso real" },
    SubstituirDocumento: { valor: EnumAcaoTratativaIrregularidade.SubstituirDocumento, text: "Substituir Documento" },
    RejeitarSubstituicao: { valor: EnumAcaoTratativaIrregularidade.RejeitarSubstituicao, text: "Rejeitar Substituição" },
    CancelarFRSCancelarCarga: { valor: EnumAcaoTratativaIrregularidade.CancelarFRSCancelarCarga, text: "Cancelar FRS/Cancelar Carga" },
    CampoInserirNumeroOcorrencia: { valor: EnumAcaoTratativaIrregularidade.CampoInserirNumeroOcorrencia, text: "Campo para inserir número da ocorrência" },
    AutorizadoSubstituicao: { valor: EnumAcaoTratativaIrregularidade.AutorizadoSubstituicao, text: "Autorizado Substituição" },
    NaoAutorizadoSubstituicao: { valor: EnumAcaoTratativaIrregularidade.NaoAutorizadoSubstituicao, text: "Não Autorizado Substituição" },
    RetornarFluxo: { valor: EnumAcaoTratativaIrregularidade.RetornarFluxo, text: "Retornar Fluxo" },
    NaoProcedePagamento: { valor: EnumAcaoTratativaIrregularidade.NaoProcedePagamento, text: "Não Procede o Pagamento" },
    ProcedePagamento: { valor: EnumAcaoTratativaIrregularidade.ProcedePagamento, text: "Procede o Pagamento" },
    CriarDCContingencia: { valor: EnumAcaoTratativaIrregularidade.CriarDCContingencia, text: "Criar DC Contingência" },
    ErroCalculo: { valor: EnumAcaoTratativaIrregularidade.ErroCalculo, text: "Erro de Cálculo" },
    SemTarifa: { valor: EnumAcaoTratativaIrregularidade.SemTarifa, text: "Sem Tarifa" },
    TarifaCadastrada: { valor: EnumAcaoTratativaIrregularidade.TarifaCadastrada, text: "Tarifa Cadastrada" },
    InserirChaveAcessoNFeValida: { valor: EnumAcaoTratativaIrregularidade.InserirChaveAcessoNFeValida, text: "Inserir chave de acesso NF-e Válida" },
    AnexarComplementar: { valor: EnumAcaoTratativaIrregularidade.AnexarComplementar, text: "Anexar Complementar" },
    AplicarEventoDesacordo: { valor: EnumAcaoTratativaIrregularidade.AplicarEventoDesacordo, text: "Aplicar Evento Desacordo" },
    CTeCorretoCadastrosCorrigidos: { valor: EnumAcaoTratativaIrregularidade.CTeCorretoCadastrosCorrigidos, text: "CT-e Correto - Cadastros Corrigidos" },
    NecessarioCartaCorrecao: { valor: EnumAcaoTratativaIrregularidade.NecessarioCartaCorrecao, text: "Carta de Correção" },
    EmitirCCeRealizarUpload: { valor: EnumAcaoTratativaIrregularidade.EmitirCCeRealizarUpload, text: "Emitir CC-e e realizar Upload" },
    ProblemaOperacional: { valor: EnumAcaoTratativaIrregularidade.ProblemaOperacional, text: "Problema Operacional (Divergência na Carga)" },
    ProblemaComercial: { valor: EnumAcaoTratativaIrregularidade.ProblemaComercial, text: "Problema Comercial" },
    DesacordoSubstituicaoDocumento: { valor: EnumAcaoTratativaIrregularidade.DesacordoSubstituicaoDocumento, text: "Desacordo - Substituição do Documento" },
    ErroFaturamento: { valor: EnumAcaoTratativaIrregularidade.ErroFaturamento, text: "Erro de faturamento" },
    FormacaoCorreta: { valor: EnumAcaoTratativaIrregularidade.FormacaoCorreta, text: "Formação Correta" },
    FormacaoIncorretaInformarFormacaoCorreta: { valor: EnumAcaoTratativaIrregularidade.FormacaoIncorretaInformarFormacaoCorreta, text: "Formação Incorreta - Informar a formação correta" },
    NFSCorretaCadastrosCorrigidos: { valor: EnumAcaoTratativaIrregularidade.NFSCorretaCadastrosCorrigidos, text: "NFS-e Correta - Cadastros Corrigidos" },
    Aprovado: { valor: EnumAcaoTratativaIrregularidade.Aprovado, text: "Aprovado" },
    Rejeitado: { valor: EnumAcaoTratativaIrregularidade.Rejeitado, text: "Rejeitado" },
}

function ObterObjetoPadrao(dadosGatilho) {
    return PropertyEntity({ getType: typesKnockout.bool, text: dadosGatilho.text, val: ko.observable(false), ValorEnum: dadosGatilho.valor })
}