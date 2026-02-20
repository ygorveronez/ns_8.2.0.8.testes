/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/TipoMovimentoArquivoContabil.js" />
/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Enumeradores/EnumTipoMovimentoArquivoContabilEuro.js" />
/// <reference path="../../Enumeradores/EnumTipoMovimentoArquivoContabilQuestor.js" />
/// <reference path="../../Enumeradores/EnumTipoMovimentoArquivoContabilPadraoTransben.js" />
/// <reference path="../../Enumeradores/EnumTipoArquivoContabilQuestor.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _arquivoContabil;
var _arquivoContabilEuro;
var _arquivoContabilJB;
var _arquivoContabilQuestor;
var _arquivoContabilPH;
var _arquivoESocial;
var _arquivoExactus;
var _arquivoMercante;
var _PermissoesPersonalizadas = null
var _arquivoContaOeste;
var _arquivoAlterdata;


var _tipoMovimentoEuro = [
    { text: "Contas pagas de Documentos", value: EnumTipoMovimentoArquivoContabilEuro.ContasPagarNotaFiscal },
    { text: "Contas recebidas de Documentos", value: EnumTipoMovimentoArquivoContabilEuro.ContasReceberNotaFiscal },
    { text: "Demais pagamentos", value: EnumTipoMovimentoArquivoContabilEuro.ContasPagar },
    { text: "Demais recebimentos", value: EnumTipoMovimentoArquivoContabilEuro.ContasReceber },
    { text: "NFS-es de Entrada", value: EnumTipoMovimentoArquivoContabilEuro.NFSeEntrada },
    { text: "Despesas Acerto de Viagem", value: EnumTipoMovimentoArquivoContabilEuro.DesepesasAcertoViagem },
    { text: "Pagamento do Motorista", value: EnumTipoMovimentoArquivoContabilEuro.PagamentoMotorista },
    { text: "Opções pré-cadastradas", value: EnumTipoMovimentoArquivoContabilEuro.OpcoesPreCadastradas }
];

var _tipoMovimentoQuestor = [
    { text: "Contas a Pagar", value: EnumTipoMovimentoArquivoContabilQuestor.ContasPagar },
    { text: "Contas a Receber", value: EnumTipoMovimentoArquivoContabilQuestor.ContasReceber },
    { text: "Demais Movimentos", value: EnumTipoMovimentoArquivoContabilQuestor.DemaisMovimentos },
    { text: "Juros e Descontos Recebidos", value: EnumTipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos },
    { text: "Juros e Descontos Pagos", value: EnumTipoMovimentoArquivoContabilQuestor.JurosDescontosPagos },
    { text: "Todos Movimentos", value: EnumTipoMovimentoArquivoContabilQuestor.Todos },
    { text: "NFS-es de Entrada", value: EnumTipoMovimentoArquivoContabilQuestor.NFSeEntrada },
    { text: "Por Tipo de Movimento", value: EnumTipoMovimentoArquivoContabilQuestor.TipoMovimento },
    { text: "Opções pré-cadastradas", value: EnumTipoMovimentoArquivoContabilQuestor.OpcoesPreCadastradas }
];

var _tipoMovimentoPH = [
    { text: "Contas a Pagar", value: EnumTipoMovimentoArquivoContabilQuestor.ContasPagar },
    { text: "Contas a Receber", value: EnumTipoMovimentoArquivoContabilQuestor.ContasReceber },
    { text: "Juros e Descontos Recebidos", value: EnumTipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos },
    { text: "Juros e Descontos Pagos", value: EnumTipoMovimentoArquivoContabilQuestor.JurosDescontosPagos },
    { text: "Todos Movimentos", value: EnumTipoMovimentoArquivoContabilQuestor.Todos },
    { text: "Documento de Entrada", value: EnumTipoMovimentoArquivoContabilQuestor.NFeEntrada },
    { text: "Opções pré-cadastradas", value: EnumTipoMovimentoArquivoContabilQuestor.OpcoesPreCadastradas }
];

var _tipoMovimentoAlterdata = [
    { text: "Contas a Pagar", value: EnumTipoMovimentoArquivoContabilQuestor.ContasPagar },
    { text: "Contas a Receber", value: EnumTipoMovimentoArquivoContabilQuestor.ContasReceber },
    { text: "Juros e Descontos Recebidos", value: EnumTipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos },
    { text: "Juros e Descontos Pagos", value: EnumTipoMovimentoArquivoContabilQuestor.JurosDescontosPagos },
    { text: "Todos Movimentos", value: EnumTipoMovimentoArquivoContabilQuestor.Todos },
    { text: "Documento de Entrada", value: EnumTipoMovimentoArquivoContabilQuestor.NFeEntrada },
    { text: "Opções pré-cadastradas", value: EnumTipoMovimentoArquivoContabilQuestor.OpcoesPreCadastradas }
];

var _tipoCargaPerigosa = [
    { text: "Todas", value: 0 },
    { text: "Somente Carga Perigosa", value: 1 },
    { text: "Sem Carga Perigosa", value: 2 }
];

var _tipoTransbordo = [
    { text: "Gerar Manifesto M3 (Todos)", value: 0 },
    { text: "Gerar Manifesto M3 (Sem número do manifesto absorvido)", value: 2 },
    { text: "Gerar Baldeação M4", value: 1 }
];

var _tipoMovimentoPadraoTransben = [
    { text: "Contas pagas de Documentos", value: EnumTipoMovimentoArquivoContabilPadraoTransben.ConstasPagasDocumento },
    { text: "Contas recebidas de Documentos", value: EnumTipoMovimentoArquivoContabilPadraoTransben.ContasRecebidasDocumento },
    { text: "Demais pagamentos", value: EnumTipoMovimentoArquivoContabilPadraoTransben.DemaisPagamentos },
    { text: "Demais recebimentos", value: EnumTipoMovimentoArquivoContabilPadraoTransben.DemaisRecebimentos }
];


var ArquivoContabil = function () {
    this.DataInicial = PropertyEntity({ text: "*Data Inicial: ", getType: typesKnockout.date, required: true });
    this.DataFinal = PropertyEntity({ text: "*Data Final: ", getType: typesKnockout.date, required: true });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });

    this.GerarArquivoEContab = PropertyEntity({ eventClick: gerarArquivoEContabClick, type: types.event, text: "Gerar Arquivo E-Contab", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var ArquivoContabilEuro = function () {
    this.DataInicial = PropertyEntity({ text: "*Data Inicial: ", getType: typesKnockout.date, required: true });
    this.DataFinal = PropertyEntity({ text: "*Data Final: ", getType: typesKnockout.date, required: true });
    this.TipoMovimento = PropertyEntity({ val: ko.observable(EnumTipoMovimentoArquivoContabilEuro.ContasPagarNotaFiscal), options: _tipoMovimentoEuro, def: EnumTipoMovimentoArquivoContabilEuro.ContasPagarNotaFiscal, text: "*Tipo Movimento: " });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo de Documento:", idBtnSearch: guid(), issue: 0, visible: ko.observable(false) });
    this.TipoMovimentoArquivoContabil = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento para Arquivo Contábil:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.GerarComCodigoHistoricoTipoMovimento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: "Gerar com o código do histórico do tipo de Movimento?" });
    
    this.GerarArquivoEuro = PropertyEntity({ eventClick: gerarArquivoEuroClick, type: types.event, text: "Gerar Arquivo Euro", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });

    this.TipoMovimento.val.subscribe(function (novoValor) {
        LimparCampoEntity(_arquivoContabilEuro.ModeloDocumentoFiscal);
        _arquivoContabilEuro.ModeloDocumentoFiscal.visible(false);

        LimparCampoEntity(_arquivoContabilEuro.TipoMovimentoArquivoContabil);
        _arquivoContabilEuro.TipoMovimentoArquivoContabil.required(false);
        _arquivoContabilEuro.TipoMovimentoArquivoContabil.visible(false);

        if (novoValor === EnumTipoMovimentoArquivoContabilEuro.NFSeEntrada) {
            _arquivoContabilEuro.ModeloDocumentoFiscal.visible(true);
        } else if (novoValor === EnumTipoMovimentoArquivoContabilEuro.OpcoesPreCadastradas) {
            _arquivoContabilEuro.TipoMovimentoArquivoContabil.required(true);
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe)
                _arquivoContabilEuro.TipoMovimentoArquivoContabil.visible(true);
        }
    });
};

var ArquivoContabilJB = function () {
    this.DataInicial = PropertyEntity({ text: "*Data Inicial: ", getType: typesKnockout.date, required: true });
    this.DataFinal = PropertyEntity({ text: "*Data Final: ", getType: typesKnockout.date, required: true });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });

    this.GerarArquivoJB = PropertyEntity({ eventClick: GerarArquivoJBClick, type: types.event, text: "Gerar Arquivo JB", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var ArquivoContabilQuestor = function () {
    this.DataInicial = PropertyEntity({ text: ko.observable("*Data Emissão Inicial:"), getType: typesKnockout.date, required: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: ko.observable("*Data Emissão Final:"), getType: typesKnockout.date, required: ko.observable(true) });
    this.DataEntradaInicial = PropertyEntity({ text: ko.observable("Data Entrada Inicial:"), getType: typesKnockout.date, required: ko.observable(false) });
    this.DataEntradaFinal = PropertyEntity({ text: ko.observable("Data Entrada Final:"), getType: typesKnockout.date, required: ko.observable(false) });

    this.TipoMovimento = PropertyEntity({ text: "*Tipo Movimento: ", val: ko.observable(EnumTipoMovimentoArquivoContabilQuestor.ContasPagar), options: _tipoMovimentoQuestor, def: EnumTipoMovimentoArquivoContabilQuestor.ContasPagar });
    this.TipoArquivo = PropertyEntity({ text: "*Tipo do Arquivo: ", val: ko.observable(EnumTipoArquivoContabilQuestor.Padrao), options: EnumTipoArquivoContabilQuestor.obterOpcoes(), def: EnumTipoArquivoContabilQuestor.Padrao });
    
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoMovimentoArquivoContabil = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento para Arquivo Contábil:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.PlanoConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Plano de Conta:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });

    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo de Documento:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.TiposMovimentos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Movimento:", idBtnSearch: guid(), issue: 0, visible: ko.observable(false) });
    this.ExtensaoCFOP = PropertyEntity({ text: "Gerar com a extensão da CFOP?", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.GerarArquivoQuestor = PropertyEntity({ eventClick: gerarArquivoQuestorClick, type: types.event, text: "Gerar Arquivo Questor", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });

    this.TipoArquivo.val.subscribe(function (novoValor) {
        if (novoValor === EnumTipoArquivoContabilQuestor.Contabil)
            _arquivoContabilQuestor.PlanoConta.visible(true);
        else {
            _arquivoContabilQuestor.PlanoConta.visible(false);
            LimparCampoEntity(_arquivoContabilQuestor.PlanoConta);
        }

        atualizarElementoAbaQuestor(novoValor);
    });

    this.TipoMovimento.val.subscribe(function (novoValor) {
        _arquivoContabilQuestor.DataInicial.required(true);
        _arquivoContabilQuestor.DataFinal.required(true);
        _arquivoContabilQuestor.DataInicial.text("*Data Emissão Inicial:");
        _arquivoContabilQuestor.DataFinal.text("*Data Emissão Final:");

        _arquivoContabilQuestor.TiposMovimentos.visible(false);
        LimparCampoEntity(_arquivoContabilQuestor.TiposMovimentos);
        LimparCampoEntity(_arquivoContabilQuestor.ModeloDocumentoFiscal);
        _arquivoContabilQuestor.ExtensaoCFOP.val(false);
        _arquivoContabilQuestor.DataEntradaInicial.val("");
        _arquivoContabilQuestor.DataEntradaFinal.val("");

        _arquivoContabilQuestor.TipoMovimentoArquivoContabil.required(false);
        _arquivoContabilQuestor.TipoMovimentoArquivoContabil.visible(false);
        LimparCampoEntity(_arquivoContabilQuestor.TipoMovimentoArquivoContabil);

        $("#idQuestorNFSEntrada").hide();

        if (novoValor === EnumTipoMovimentoArquivoContabilQuestor.NFSeEntrada) {
            _arquivoContabilQuestor.DataInicial.required(false);
            _arquivoContabilQuestor.DataFinal.required(false);
            _arquivoContabilQuestor.DataInicial.text("Data Emissão Inicial:");
            _arquivoContabilQuestor.DataFinal.text("Data Emissão Final:");

            $("#idQuestorNFSEntrada").show();
        } else if (novoValor === EnumTipoMovimentoArquivoContabilQuestor.TipoMovimento) {
            _arquivoContabilQuestor.TiposMovimentos.visible(true);
        } else if (novoValor === EnumTipoMovimentoArquivoContabilQuestor.OpcoesPreCadastradas) {
            _arquivoContabilQuestor.TipoMovimentoArquivoContabil.required(true);
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe)
                _arquivoContabilQuestor.TipoMovimentoArquivoContabil.visible(true);
        }
    });
};

var ArquivoContaOeste = function () {
    this.DataInicial = PropertyEntity({ text: ko.observable("*Data Emissão Inicial:"), getType: typesKnockout.date, required: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: ko.observable("*Data Emissão Final:"), getType: typesKnockout.date, required: ko.observable(true) });
    this.DataEntradaInicial = PropertyEntity({ text: ko.observable("Data Entrada Inicial:"), getType: typesKnockout.date, required: ko.observable(false) });
    this.DataEntradaFinal = PropertyEntity({ text: ko.observable("Data Entrada Final:"), getType: typesKnockout.date, required: ko.observable(false) });

    this.TipoMovimento = PropertyEntity({ val: ko.observable(EnumTipoMovimentoArquivoContabilQuestor.ContasPagar), options: _tipoMovimentoQuestor, def: EnumTipoMovimentoArquivoContabilQuestor.ContasPagar, text: "*Tipo Movimento: " });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoMovimentoArquivoContabil = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento para Arquivo Contábil:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });

    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo de Documento:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.ExtensaoCFOP = PropertyEntity({ text: "Gerar com a extensão da CFOP?", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.GerarRegistroRetNovaEspecificacao = PropertyEntity({ text: "Gerar registros reti com a nova especificação", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false) });

    this.GerarComCodigoHistoricoTipoMovimento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: "Gerar com o código do histórico do tipo de Movimento?" });
    this.NaoRemoverCaracteresEspeciaisAcentuados = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: "Não remover caracteres Especiais ou Acentuados?" });

    this.GerarArquivoContaOeste = PropertyEntity({ eventClick: gerarArquivoContaOesteClick, type: types.event, text: "Gerar Arquivo Conta Oeste", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });

    this.TipoMovimento.val.subscribe(function (novoValor) {
        _arquivoContaOeste.DataInicial.required(true);
        _arquivoContaOeste.DataFinal.required(true);
        _arquivoContaOeste.DataInicial.text("*Data Emissão Inicial:");
        _arquivoContaOeste.DataFinal.text("*Data Emissão Final:");

        _arquivoContaOeste.GerarRegistroRetNovaEspecificacao.visible(false);
        _arquivoContaOeste.GerarRegistroRetNovaEspecificacao.val(false);

        LimparCampoEntity(_arquivoContaOeste.ModeloDocumentoFiscal);
        _arquivoContaOeste.ExtensaoCFOP.val(false);
        _arquivoContaOeste.DataEntradaInicial.val("");
        _arquivoContaOeste.DataEntradaFinal.val("");

        _arquivoContaOeste.TipoMovimentoArquivoContabil.required(false);
        _arquivoContaOeste.TipoMovimentoArquivoContabil.visible(false);
        LimparCampoEntity(_arquivoContaOeste.TipoMovimentoArquivoContabil);

        $("#idContaOesteNFSEntrada").hide();

        if (novoValor === EnumTipoMovimentoArquivoContabilQuestor.NFSeEntrada) {
            _arquivoContaOeste.DataInicial.required(false);
            _arquivoContaOeste.DataFinal.required(false);
            _arquivoContaOeste.DataInicial.text("Data Emissão Inicial:");
            _arquivoContaOeste.DataFinal.text("Data Emissão Final:");
            _arquivoContaOeste.GerarRegistroRetNovaEspecificacao.visible(true);

            $("#idContaOesteNFSEntrada").show();
        } else if (novoValor === EnumTipoMovimentoArquivoContabilQuestor.OpcoesPreCadastradas) {
            _arquivoContaOeste.TipoMovimentoArquivoContabil.required(true);
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe)
                _arquivoContaOeste.TipoMovimentoArquivoContabil.visible(true);
        }
    });
};

var ArquivoAlterdata = function () {
    this.DataInicial = PropertyEntity({ text: ko.observable("*Data Emissão Inicial:"), getType: typesKnockout.date, required: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: ko.observable("*Data Emissão Final:"), getType: typesKnockout.date, required: ko.observable(true) });
    this.TipoMovimento = PropertyEntity({ val: ko.observable(EnumTipoMovimentoArquivoContabilQuestor.ContasPagar), options: _tipoMovimentoAlterdata, def: EnumTipoMovimentoArquivoContabilQuestor.ContasPagar, text: "*Tipo Movimento: " });
    this.TipoMovimentoArquivoContabil = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento para Arquivo Contábil:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.GerarFormatoTXT = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: "Gerar em formato txt" });

    this.GerarArquivoAlterdata = PropertyEntity({ eventClick: gerarArquivoAlterdataClick, type: types.event, text: "Gerar Arquivo Alterdata", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });

    this.TipoMovimento.val.subscribe(function (novoValor) {
        _arquivoAlterdata.TipoMovimentoArquivoContabil.required(false);
        _arquivoAlterdata.TipoMovimentoArquivoContabil.visible(false);
        LimparCampoEntity(_arquivoAlterdata.TipoMovimentoArquivoContabil);

        if (novoValor === EnumTipoMovimentoArquivoContabilQuestor.OpcoesPreCadastradas) {
            _arquivoAlterdata.TipoMovimentoArquivoContabil.required(true);
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe)
                _arquivoAlterdata.TipoMovimentoArquivoContabil.visible(true);
        }
    });
};

var ArquivoESocial = function () {
    this.DataInicial = PropertyEntity({ text: "*Data Inicial: ", getType: typesKnockout.date, required: true });
    this.DataFinal = PropertyEntity({ text: "*Data Final: ", getType: typesKnockout.date, required: true });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });

    this.GerarArquivo = PropertyEntity({ eventClick: derarArquivoESocialClick, type: types.event, text: "Gerar Arquivo E-Social", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var ArquivoExactus = function () {
    this.DataInicial = PropertyEntity({ text: "*Data Inicial: ", getType: typesKnockout.date, required: true });
    this.DataFinal = PropertyEntity({ text: "*Data Final: ", getType: typesKnockout.date, required: true });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });

    this.GerarArquivo = PropertyEntity({ eventClick: gerarArquivoExactusClick, type: types.event, text: "Gerar Arquivo Exactus", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var ArquivoContabilPH = function () {
    this.DataInicial = PropertyEntity({ text: ko.observable("*Data Movimento Inicial:"), getType: typesKnockout.date, required: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: ko.observable("*Data Movimento Final:"), getType: typesKnockout.date, required: ko.observable(true) });
    this.TipoMovimento = PropertyEntity({ val: ko.observable(EnumTipoMovimentoArquivoContabilQuestor.ContasPagar), options: _tipoMovimentoPH, def: EnumTipoMovimentoArquivoContabilQuestor.ContasPagar, text: "*Tipo Movimento: " });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoMovimentoArquivoContabil = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento para Arquivo Contábil:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.PlanoConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Plano de Conta:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.GerarArquivoPH = PropertyEntity({ eventClick: gerarArquivoPHClick, type: types.event, text: "Gerar Arquivo PH", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });

    this.TipoMovimento.val.subscribe(function (novoValor) {
        _arquivoContabilPH.TipoMovimentoArquivoContabil.required(false);
        _arquivoContabilPH.TipoMovimentoArquivoContabil.visible(false);
        LimparCampoEntity(_arquivoContabilPH.TipoMovimentoArquivoContabil);

        if (novoValor === EnumTipoMovimentoArquivoContabilQuestor.OpcoesPreCadastradas) {
            _arquivoContabilPH.TipoMovimentoArquivoContabil.required(true);
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe)
                _arquivoContabilPH.TipoMovimentoArquivoContabil.visible(true);
        }
    });
};

var ArquivoMercante = function () {
    this.PedidoNavioViagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Viagem:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Porto Origem:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto de Atracação:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Terminal Origem:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Terminal de Atracação:"), idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.ComConhecimentosCancelados = PropertyEntity({ text: "Deseja gerar com os CT-es cancelados?", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.TipoCargaPerigosa = PropertyEntity({ val: ko.observable(0), options: _tipoCargaPerigosa, def: 0, text: "*Tipo Carga Perigosa: " });
    this.TipoTransbordo = PropertyEntity({ val: ko.observable(0), options: _tipoTransbordo, def: 0, text: "*Tipo do Arquivo: " });

    this.GerarArquivoMercante = PropertyEntity({ eventClick: GerarArquivoMercanteClick, type: types.event, text: "Gerar Arquivo Mercante", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });

    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo Mercante:", val: ko.observable(""), visible: ko.observable(true) });
    this.Enviar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadArquivoContabil() {
    _arquivoContabil = new ArquivoContabil();
    KoBindings(_arquivoContabil, "tabEContab");

    _arquivoContabilEuro = new ArquivoContabilEuro();
    KoBindings(_arquivoContabilEuro, "tabEuro");

    _arquivoContabilJB = new ArquivoContabilJB();
    KoBindings(_arquivoContabilJB, "tabJB");

    _arquivoContabilQuestor = new ArquivoContabilQuestor();
    KoBindings(_arquivoContabilQuestor, "tabQuestor");

    _arquivoContaOeste = new ArquivoContaOeste();
    KoBindings(_arquivoContaOeste, "tabContaOeste");

    _arquivoESocial = new ArquivoESocial();
    KoBindings(_arquivoESocial, "tabESocial");

    _arquivoExactus = new ArquivoExactus();
    KoBindings(_arquivoExactus, "tabExactus");

    _arquivoContabilPH = new ArquivoContabilPH();
    KoBindings(_arquivoContabilPH, "tabPH");

    _arquivoMercante = new ArquivoMercante();
    KoBindings(_arquivoMercante, "tabMercante");

    _arquivoAlterdata = new ArquivoAlterdata();
    KoBindings(_arquivoAlterdata, "tabAlterdata");

    new BuscarEmpresa(_arquivoContabilJB.Empresa);
    new BuscarEmpresa(_arquivoContabilQuestor.Empresa);
    new BuscarEmpresa(_arquivoContaOeste.Empresa);
    new BuscarEmpresa(_arquivoContabilPH.Empresa);
    new BuscarEmpresa(_arquivoContabil.Empresa);
    new BuscarEmpresa(_arquivoESocial.Empresa);
    new BuscarEmpresa(_arquivoExactus.Empresa);

    new BuscarModeloDocumentoFiscal(_arquivoContabilEuro.ModeloDocumentoFiscal);
    new BuscarModeloDocumentoFiscal(_arquivoContabilQuestor.ModeloDocumentoFiscal);
    new BuscarModeloDocumentoFiscal(_arquivoContaOeste.ModeloDocumentoFiscal);

    new BuscarTipoMovimento(_arquivoContabilQuestor.TiposMovimentos);
    new BuscarTipoMovimentoArquivoContabil(_arquivoAlterdata.TipoMovimentoArquivoContabil);
    new BuscarTipoMovimentoArquivoContabil(_arquivoContaOeste.TipoMovimentoArquivoContabil);
    new BuscarTipoMovimentoArquivoContabil(_arquivoContabilPH.TipoMovimentoArquivoContabil);
    new BuscarTipoMovimentoArquivoContabil(_arquivoContabilQuestor.TipoMovimentoArquivoContabil);
    new BuscarTipoMovimentoArquivoContabil(_arquivoContabilEuro.TipoMovimentoArquivoContabil);

    new BuscarPlanoConta(_arquivoContabilPH.PlanoConta, null, null, null, EnumAnaliticoSintetico.Analitico);
    new BuscarPlanoConta(_arquivoContabilQuestor.PlanoConta, null, null, null, EnumAnaliticoSintetico.Analitico);
    new BuscarEmpresa(_arquivoMercante.Empresa);
    new BuscarPedidoViagemNavio(_arquivoMercante.PedidoNavioViagem);
    new BuscarPorto(_arquivoMercante.PortoOrigem);
    new BuscarPorto(_arquivoMercante.PortoDestino);
    new BuscarTipoTerminalImportacao(_arquivoMercante.TerminalOrigem);
    new BuscarTipoTerminalImportacao(_arquivoMercante.TerminalDestino);

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ArquivoContabil_RemoverObrigatoriedadeTeminalAtracacao, _PermissoesPersonalizadas)) {
        _arquivoMercante.TerminalDestino.required(false);
        _arquivoMercante.TerminalDestino.text("Terminal de Atracação:");
    } else {
        _arquivoMercante.TerminalDestino.required(true);
        _arquivoMercante.TerminalDestino.text("*Terminal de Atracação:");
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _arquivoContabil.Empresa.required(false);
        _arquivoContabil.Empresa.visible(false);

        _arquivoContabilJB.Empresa.required(false);
        _arquivoContabilJB.Empresa.visible(false);

        _arquivoContabilQuestor.Empresa.required(false);
        _arquivoContabilQuestor.Empresa.visible(false);

        _arquivoContaOeste.Empresa.required(false);
        _arquivoContaOeste.Empresa.visible(false);

        _arquivoContabilPH.Empresa.required(false);
        _arquivoContabilPH.Empresa.visible(false);

        _arquivoESocial.Empresa.required(false);
        _arquivoESocial.Empresa.visible(false);

        _arquivoExactus.Empresa.required(false);
        _arquivoExactus.Empresa.visible(false);

        $("#liTabESocial").hide();
    }

    $("#idQuestorNFSEntrada").hide();
    $("#idContaOesteNFSEntrada").hide();
}

function gerarArquivoEContabClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_arquivoContabil);

    if (Globalize.parseDate(_arquivoContabil.DataInicial.val()) > Globalize.parseDate(_arquivoContabil.DataFinal.val())) {
        valido = false;
        _arquivoContabil.DataFinal.requiredClass("form-control");
        _arquivoContabil.DataFinal.requiredClass("form-control is-invalid");
    }

    if (valido) {
        var dados = {
            DataInicial: _arquivoContabil.DataInicial.val(),
            DataFinal: _arquivoContabil.DataFinal.val(),
            Empresa: _arquivoContabil.Empresa.codEntity()
        };
        executarDownload("ArquivoContabil/GerarArquivoEContab", dados, null, function (arg) {
            var retorno = JSON.parse(arg.replace("(", "").replace(");", ""));
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function gerarArquivoEuroClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_arquivoContabilEuro);

    if (Globalize.parseDate(_arquivoContabilEuro.DataInicial.val()) > Globalize.parseDate(_arquivoContabilEuro.DataFinal.val())) {
        valido = false;
        _arquivoContabilEuro.DataFinal.requiredClass("form-control");
        _arquivoContabilEuro.DataFinal.requiredClass("form-control is-invalid");
    }

    if (valido) {
        var dados = {
            DataInicial: _arquivoContabilEuro.DataInicial.val(),
            DataFinal: _arquivoContabilEuro.DataFinal.val(),
            TipoMovimento: _arquivoContabilEuro.TipoMovimento.val(),
            ModeloDocumentoFiscal: JSON.stringify(recursiveMultiplesEntities(_arquivoContabilEuro.ModeloDocumentoFiscal)),
            TipoMovimentoArquivoContabil: _arquivoContabilEuro.TipoMovimentoArquivoContabil.codEntity(),
            GerarComCodigoHistoricoTipoMovimento: _arquivoContabilEuro.GerarComCodigoHistoricoTipoMovimento.val()
        };
        executarDownload("ArquivoContabil/GerarArquivoEuro", dados, async function (retorno) {

            if (retorno.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function GerarArquivoJBClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_arquivoContabilJB);

    if (Globalize.parseDate(_arquivoContabilJB.DataInicial.val()) > Globalize.parseDate(_arquivoContabilJB.DataFinal.val())) {
        valido = false;
        _arquivoContabilJB.DataFinal.requiredClass("form-control");
        _arquivoContabilJB.DataFinal.requiredClass("form-control is-invalid");
    }

    if (valido) {
        var dados = {
            DataInicial: _arquivoContabilJB.DataInicial.val(),
            DataFinal: _arquivoContabilJB.DataFinal.val(),
            Empresa: _arquivoContabilJB.Empresa.codEntity()
        };
        executarDownload("ArquivoContabil/GerarArquivoJB", dados, null, function (arg) {
            var retorno = JSON.parse(arg.replace("(", "").replace(");", ""));
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function gerarArquivoExactusClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_arquivoExactus);

    if (Globalize.parseDate(_arquivoExactus.DataInicial.val()) > Globalize.parseDate(_arquivoExactus.DataFinal.val())) {
        valido = false;
        _arquivoExactus.DataFinal.requiredClass("form-control");
        _arquivoExactus.DataFinal.requiredClass("form-control is-invalid");
    }

    if (valido) {
        var dados = {
            DataInicial: _arquivoExactus.DataInicial.val(),
            DataFinal: _arquivoExactus.DataFinal.val(),
            Empresa: _arquivoExactus.Empresa.codEntity()
        };
        executarDownload("ArquivoContabil/GerarArquivoExactus", dados, null, function (arg) {
            var retorno = JSON.parse(arg.replace("(", "").replace(");", ""));
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function derarArquivoESocialClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_arquivoESocial);

    if (Globalize.parseDate(_arquivoESocial.DataInicial.val()) > Globalize.parseDate(_arquivoESocial.DataFinal.val())) {
        valido = false;
        _arquivoESocial.DataFinal.requiredClass("form-control");
        _arquivoESocial.DataFinal.requiredClass("form-control is-invalid");
    }

    if (valido) {
        var dados = {
            DataInicial: _arquivoESocial.DataInicial.val(),
            DataFinal: _arquivoESocial.DataFinal.val(),
            Empresa: _arquivoESocial.Empresa.codEntity()
        };
        executarDownload("ArquivoContabil/GerarArquivoESocial", dados, null, function (arg) {
            var retorno = JSON.parse(arg.replace("(", "").replace(");", ""));
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function gerarArquivoQuestorClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_arquivoContabilQuestor);

    if (Globalize.parseDate(_arquivoContabilQuestor.DataInicial.val()) > Globalize.parseDate(_arquivoContabilQuestor.DataFinal.val())) {
        valido = false;
        _arquivoContabilQuestor.DataFinal.requiredClass("form-control");
        _arquivoContabilQuestor.DataFinal.requiredClass("form-control is-invalid");
    }

    if (_arquivoContabilQuestor.TipoMovimento.val() === EnumTipoMovimentoArquivoContabilQuestor.NFSeEntrada) {
        if (_arquivoContabilQuestor.TipoArquivo.val() === EnumTipoArquivoContabilQuestor.Contabil)
            return exibirMensagem(tipoMensagem.atencao, "Atenção", "Não é possível utilizar esse Tipo de Movimento para o Tipo Arquivo Contábil! Somente pode ser utilizado no Padrão!");

        if (Globalize.parseDate(_arquivoContabilQuestor.DataEntradaInicial.val()) > Globalize.parseDate(_arquivoContabilQuestor.DataEntradaFinal.val())) {
            valido = false;
            _arquivoContabilQuestor.DataEntradaFinal.requiredClass("form-control");
            _arquivoContabilQuestor.DataEntradaFinal.requiredClass("form-control is-invalid");
        }

        if (_arquivoContabilQuestor.DataInicial.val() === "" && _arquivoContabilQuestor.DataEntradaInicial.val() === "") {
            exibirMensagem(tipoMensagem.atencao, "Filtros de Datas", "Favor aplicar um filtro de data, ou Data Emissão, ou Data Entrada");
            return;
        }
        else if ((_arquivoContabilQuestor.DataInicial.val() !== "" || _arquivoContabilQuestor.DataFinal.val() !== "") &&
            (_arquivoContabilQuestor.DataEntradaInicial.val() !== "" || _arquivoContabilQuestor.DataEntradaFinal.val() !== "")) {
            exibirMensagem(tipoMensagem.atencao, "Filtros de Datas", "Favor utilizar apenas um filtro de data, ou Data Emissão, ou Data Entrada");
            return;
        }
    }

    if (valido) {
        var dados = {
            DataInicial: _arquivoContabilQuestor.DataInicial.val(),
            DataFinal: _arquivoContabilQuestor.DataFinal.val(),
            TipoMovimento: _arquivoContabilQuestor.TipoMovimento.val(),
            Empresa: _arquivoContabilQuestor.Empresa.codEntity(),
            ExtensaoCFOP: _arquivoContabilQuestor.ExtensaoCFOP.val(),
            ModeloDocumentoFiscal: JSON.stringify(recursiveMultiplesEntities(_arquivoContabilQuestor.ModeloDocumentoFiscal)),
            TiposMovimentos: JSON.stringify(recursiveMultiplesEntities(_arquivoContabilQuestor.TiposMovimentos)),
            DataEntradaInicial: _arquivoContabilQuestor.DataEntradaInicial.val(),
            DataEntradaFinal: _arquivoContabilQuestor.DataEntradaFinal.val(),
            TipoMovimentoArquivoContabil: _arquivoContabilQuestor.TipoMovimentoArquivoContabil.codEntity(),
            TipoArquivo: _arquivoContabilQuestor.TipoArquivo.val(),
            PlanoConta: _arquivoContabilQuestor.PlanoConta.codEntity()
        };
        executarDownload("ArquivoContabil/GerarArquivoQuestor", dados, null, function (arg) {
            var retorno = JSON.parse(arg.replace("(", "").replace(");", ""));
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function gerarArquivoAlterdataClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_arquivoAlterdata);

    if (Globalize.parseDate(_arquivoAlterdata.DataInicial.val()) > Globalize.parseDate(_arquivoAlterdata.DataFinal.val())) {
        valido = false;
        _arquivoAlterdata.DataFinal.requiredClass("form-control");
        _arquivoAlterdata.DataFinal.requiredClass("form-control is-invalid");
    }

    if (valido) {
        var dados = {
            DataInicial: _arquivoAlterdata.DataInicial.val(),
            DataFinal: _arquivoAlterdata.DataFinal.val(),
            TipoMovimento: _arquivoAlterdata.TipoMovimento.val(),
            TipoMovimentoArquivoContabil: _arquivoAlterdata.TipoMovimentoArquivoContabil.codEntity(),
            GerarFormatoTXT: _arquivoAlterdata.GerarFormatoTXT.val()
        };
        executarDownload("ArquivoContabil/GerarArquivoAlterdata", dados, null, function (arg) {
            var retorno = JSON.parse(arg.replace("(", "").replace(");", ""));
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function gerarArquivoContaOesteClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_arquivoContaOeste);

    if (Globalize.parseDate(_arquivoContaOeste.DataInicial.val()) > Globalize.parseDate(_arquivoContaOeste.DataFinal.val())) {
        valido = false;
        _arquivoContaOeste.DataFinal.requiredClass("form-control");
        _arquivoContaOeste.DataFinal.requiredClass("form-control is-invalid");
    }

    if (_arquivoContaOeste.TipoMovimento.val() === EnumTipoMovimentoArquivoContabilQuestor.NFSeEntrada) {
        if (Globalize.parseDate(_arquivoContaOeste.DataEntradaInicial.val()) > Globalize.parseDate(_arquivoContaOeste.DataEntradaFinal.val())) {
            valido = false;
            _arquivoContaOeste.DataEntradaFinal.requiredClass("form-control");
            _arquivoContaOeste.DataEntradaFinal.requiredClass("form-control is-invalid");
        }

        if (_arquivoContaOeste.DataInicial.val() === "" && _arquivoContaOeste.DataEntradaInicial.val() === "") {
            exibirMensagem(tipoMensagem.atencao, "Filtros de Datas", "Favor aplicar um filtro de data, ou Data Emissão, ou Data Entrada");
            return;
        }
        else if ((_arquivoContaOeste.DataInicial.val() !== "" || _arquivoContaOeste.DataFinal.val() !== "") &&
            (_arquivoContaOeste.DataEntradaInicial.val() !== "" || _arquivoContaOeste.DataEntradaFinal.val() !== "")) {
            exibirMensagem(tipoMensagem.atencao, "Filtros de Datas", "Favor utilizar apenas um filtro de data, ou Data Emissão, ou Data Entrada");
            return;
        }
    }

    if (valido) {
        var dados = {
            DataInicial: _arquivoContaOeste.DataInicial.val(),
            DataFinal: _arquivoContaOeste.DataFinal.val(),
            TipoMovimento: _arquivoContaOeste.TipoMovimento.val(),
            Empresa: _arquivoContaOeste.Empresa.codEntity(),
            ExtensaoCFOP: _arquivoContaOeste.ExtensaoCFOP.val(),
            GerarRegistroRetNovaEspecificacao: _arquivoContaOeste.GerarRegistroRetNovaEspecificacao.val(),
            ModeloDocumentoFiscal: JSON.stringify(recursiveMultiplesEntities(_arquivoContaOeste.ModeloDocumentoFiscal)),
            DataEntradaInicial: _arquivoContaOeste.DataEntradaInicial.val(),
            DataEntradaFinal: _arquivoContaOeste.DataEntradaFinal.val(),
            TipoMovimentoArquivoContabil: _arquivoContaOeste.TipoMovimentoArquivoContabil.codEntity(),
            GerarComCodigoHistoricoTipoMovimento: _arquivoContaOeste.GerarComCodigoHistoricoTipoMovimento.val(),
            NaoRemoverCaracteresEspeciaisAcentuados: _arquivoContaOeste.NaoRemoverCaracteresEspeciaisAcentuados.val()
        };
        executarDownload("ArquivoContabil/GerarArquivoContaOeste", dados, null, function (arg) {
            var retorno = JSON.parse(arg.replace("(", "").replace(");", ""));
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function gerarArquivoPHClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_arquivoContabilPH);

    if (Globalize.parseDate(_arquivoContabilPH.DataInicial.val()) > Globalize.parseDate(_arquivoContabilPH.DataFinal.val())) {
        valido = false;
        _arquivoContabilPH.DataFinal.requiredClass("form-control");
        _arquivoContabilPH.DataFinal.requiredClass("form-control is-invalid");
    }

    if ((_arquivoContabilPH.TipoMovimento.val() == EnumTipoMovimentoArquivoContabilQuestor.NFeEntrada) && _arquivoContabilPH.PlanoConta.val())
        return exibirMensagem(tipoMensagem.atencao, "Atenção", "Não é possível buscar por Plano de Conta ao utilizar esse Tipo de Movimento!");

    if (valido) {
        var dados = {
            DataInicial: _arquivoContabilPH.DataInicial.val(),
            DataFinal: _arquivoContabilPH.DataFinal.val(),
            TipoMovimento: _arquivoContabilPH.TipoMovimento.val(),
            Empresa: _arquivoContabilPH.Empresa.codEntity(),
            TipoMovimentoArquivoContabil: _arquivoContabilPH.TipoMovimentoArquivoContabil.codEntity(),
            PlanoConta: _arquivoContabilPH.PlanoConta.codEntity()
        };
        executarDownload("ArquivoContabil/GerarArquivoPH", dados, null, function (arg) {
            var retorno = JSON.parse(arg.replace("(", "").replace(");", ""));
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function importarClick(e, sender) {
    var file = document.getElementById(_arquivoMercante.Arquivo.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);

    enviarArquivo("ArquivoContabil/ImportarArquivoMercante?callback=?", null, formData, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
            LimparCampo(_arquivoMercante.Arquivo);
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });
}

function GerarArquivoMercanteClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_arquivoMercante);
    if (valido) {
        data = RetornarObjetoPesquisa(_arquivoMercante);
        executarDownload("ArquivoContabil/GerarArquivoMercante", data, function (arg) {

            Salvar(_arquivoMercante, "ArquivoContabil/ConsultarDocumentacaoMercantePendente", function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo gerado com sucesso e sem pendências.");
                    } else {
                        $("#tabMercante").before('<p class="alert alert-info no-margin"><button class="close" data-dismiss="alert">×</button><i class="fa-fw fa fa-info"></i><strong>Atenção!</strong> Retorno da geração do Arquivo Mercante:<br/>' + arg.Msg.replace(/\n/g, "<br />") + '</p>');
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            }, sender);

        }, null);
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function gerarArquivoPadraoTransben(e, sender) {
    var valido = ValidarCamposObrigatorios(_arquivoContabilQuestor);

    if (Globalize.parseDate(_arquivoContabilQuestor.DataInicial.val()) > Globalize.parseDate(_arquivoContabilQuestor.DataFinal.val())) {
        valido = false;
        _arquivoContabilQuestor.DataFinal.requiredClass("form-control");
        _arquivoContabilQuestor.DataFinal.requiredClass("form-control is-invalid");
    }

    if (valido) {
        var dados = {
            DataInicial: _arquivoContabilQuestor.DataInicial.val(),
            DataFinal: _arquivoContabilQuestor.DataFinal.val(),
            TipoMovimento: _arquivoContabilQuestor.TipoMovimento.val(),
            ModeloDocumentoFiscal: JSON.stringify(recursiveMultiplesEntities(_arquivoContabilQuestor.ModeloDocumentoFiscal)),
            TipoMovimentoArquivoContabil: _arquivoContabilQuestor.TipoMovimentoArquivoContabil.codEntity(),
        };
        executarDownload("ArquivoContabil/GerarArquivoPadraoTransben", dados, async function (retorno) {

            if (retorno.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function cancelarClick(e) {
    limparCamposArquivoContabil();
}

//*******MÉTODOS*******

function limparCamposArquivoContabil() {
    LimparCampos(_arquivoContabil);
    LimparCampos(_arquivoContabilEuro);
    LimparCampos(_arquivoContabilJB);
    LimparCampos(_arquivoESocial);
    LimparCampos(_arquivoContabilQuestor);
    LimparCampos(_arquivoContaOeste);
    LimparCampos(_arquivoContabilPH);
    LimparCampos(_arquivoExactus);
    LimparCampos(_arquivoMercante);
    LimparCampos(_arquivoAlterdata);
}

function atualizarElementoAbaQuestor(tipoArquivo) {

    var elemento = document.getElementById("tabQuestor");

    if (tipoArquivo === EnumTipoArquivoContabilQuestor.PadraoTransben) {
        _arquivoContabilQuestor.TipoMovimento.options = _tipoMovimentoPadraoTransben;
        _arquivoContabilQuestor.TipoMovimento.def = EnumTipoMovimentoArquivoContabilPadraoTransben.ConstasPagasDocumento;
        _arquivoContabilQuestor.TipoMovimento.val(EnumTipoMovimentoArquivoContabilPadraoTransben.ConstasPagasDocumento);
        _arquivoContabilQuestor.GerarArquivoQuestor.eventClick = gerarArquivoPadraoTransben;

        _arquivoContabilQuestor.Empresa.required(false);
        _arquivoContabilQuestor.Empresa.enable(false);
        _arquivoContabilQuestor.Empresa.visible(false);
    } else {
       
        _arquivoContabilQuestor.TipoMovimento.options = _tipoMovimentoQuestor;
        _arquivoContabilQuestor.TipoMovimento.def = EnumTipoMovimentoArquivoContabilQuestor.ContasPagar;
        _arquivoContabilQuestor.TipoMovimento.val(EnumTipoMovimentoArquivoContabilQuestor.ContasPagar);
        _arquivoContabilQuestor.GerarArquivoQuestor.eventClick = gerarArquivoQuestorClick;

        _arquivoContabilQuestor.Empresa.required(true);
        _arquivoContabilQuestor.Empresa.enable(true);
        _arquivoContabilQuestor.Empresa.visible(true);
    }
}

