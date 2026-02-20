/// <reference path="AnexoConvite.js" />
/// <reference path="ConvidadosConvite.js" />
/// <reference path="BiddingChecklist.js" />
/// <reference path="BiddingOfertas.js" />
/// <reference path="EtapaBiddingConvite.js" />
/// <reference path="../../Consultas/TipoDeBidding.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoBiddingConvite.js" />

/*Declaração Objetos*/
var _CRUDBiddingConvite;
var _biddingConvite;
var _pesquisaBiddingConvite;
var _gridBiddingConvite;

var _situacaoOptions = [
    { text: "Inativo", value: 0 },
    { text: "Ativo", value: 1 }
];

var CRUDBiddingConvite = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Salvar Bidding", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
};

var PesquisaBiddingConvite = function () {
    this.Descricao = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.string, text: "Descrição:" });
    this.DataInicio = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.date, text: "Data Inicio:" });
    this.DataLimite = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.date, text: "Data Limite" });
    this.TipoBidding = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Bidding:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Solicitante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Solicitante: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Comprador = PropertyEntity({ codEntity: ko.observable(0), type: types.multiplesEntities, text: "Comprador: ", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: "Situação:", options: EnumSituacaoBiddingConvite.ObterOpcoesPesquisa() });
    this.Pesquisar = PropertyEntity({ eventClick: PesquisarConvite, type: types.event, text: "Pesquisar", visible: ko.observable(true), idGrid: guid() });
    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.NumeroBidding = PropertyEntity({ val: ko.observable(""), required: false, getType: typesKnockout.int, text: "Número Bidding:" });
}

var BiddingConvite = function () {
    this.Codigo = PropertyEntity({ text: "Número Bidding", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false) });
    this.Situacao = PropertyEntity({ text: "*Situação:", required: true, getType: typesKnockout.dynamic, options: _situacaoOptions, val: ko.observable("1"), def: "1", enable: ko.observable(true) });
    this.SituacaoAprovacao = PropertyEntity({ val: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "*Data Início Bidding:", required: true, getType: typesKnockout.dateTime });
    this.DataLimite = PropertyEntity({ text: "*Data Limite Bidding:", required: true, getType: typesKnockout.dateTime });
    this.PrazoAceiteConvite = PropertyEntity({ text: ko.observable("*Prazo para aceite do convite:"), required: ko.observable(true), getType: typesKnockout.dateTime, val: ko.observable(""), enable: ko.observable(true) });
    this.DescritivoConvite = PropertyEntity({ text: "*Descritivo do convite para avaliação do Embarcador:", required: true, getType: typesKnockout.string, maxlength: 5000, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.DescritivoTransportador = PropertyEntity({ text: "*Descritivo do convite para o Transportador:", required: true, getType: typesKnockout.string, maxlength: 5000, enable: ko.observable(true) });
    this.TipoFrete = PropertyEntity({ text: "Tipo de frete:", required: false, getType: typesKnockout.dynamic, options: EnumTipoFrete.ObterOpcoes(), val: ko.observable(EnumTipoFrete.CIFFOB), def: EnumTipoFrete.CIFFOB, enable: ko.observable(true) });
    this.ExigirPreenchimentoChecklistConvitePeloTransportador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true) });
    this.DataInicioVigencia = PropertyEntity({ text: "Data Início Vigência Tabela:", required: false, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataFimVigencia = PropertyEntity({ text: "Data Final Vigência Tabela:", required: false, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataInicioVigencia.dateRangeLimit = this.DataFimVigencia;
    this.DataFimVigencia.dateRangeLimit = this.DataInicioVigencia;
    this.TipoBidding = PropertyEntity({ text: "*Tipo de Bidding: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(false) });

    this.DataInicio.val.subscribe(function () {
        if (_biddingConvite.DataInicio.val() != "") {
            _biddingConvite.PrazoAceiteConvite.enable(true);
            _biddingConvite.PrazoAceiteConvite.minDate(_biddingConvite.DataInicio.val());
        }
        else {
            _biddingConvite.PrazoAceiteConvite.enable(false);
            _biddingConvite.PrazoAceiteConvite.val("");
        }
    });
}

//Métodos Globais
function LoadBidding() {
    _biddingConvite = new BiddingConvite();
    KoBindings(_biddingConvite, "knockoutBidding");

    HeaderAuditoria("BiddingConvite", _biddingConvite);

    _CRUDBiddingConvite = new CRUDBiddingConvite();
    KoBindings(_CRUDBiddingConvite, "knockoutCRUDBiddingConvite");

    _pesquisaBiddingConvite = new PesquisaBiddingConvite();
    KoBindings(_pesquisaBiddingConvite, "knockoutPesquisaBiddingConvite");

    BuscarTipoDeBidding(_biddingConvite.TipoBidding, null, null, callbackTipoBidding)
    BuscarTipoDeBidding(_pesquisaBiddingConvite.TipoBidding);
    BuscarFuncionario(_pesquisaBiddingConvite.Solicitante);
    BuscarFuncionario(_pesquisaBiddingConvite.Comprador);

    loadGridBidding();
    loadAnexo();
    loadConvidados();
    loadChecklist();
    loadOfertas();
    loadBaseline();
    LoadEtapaBiddingConvite();
    loadAnexoConviteTipoBidding();
    verificarExistenciaRegraTipoBidding();
    LoadAprovacaoBiddingConvite();
    LoadValidacaoObrigatoriedadeDatas();

    $("#liTabAnexos").show();
}

//Métodos Privados
function loadGridBidding() {
    const opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (registroSelecionado) { EditarClick(registroSelecionado, false); }, tamanho: "10", icone: "" };
    const duplicar = { descricao: "Duplicar", id: guid(), evento: "onclick", metodo: function (registroSelecionado) { EditarClick(registroSelecionado, true); }, tamanho: "10", icone: "" };

    const menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoEditar, duplicar], tamanho: "10" };
    _gridBiddingConvite = new GridViewExportacao(_pesquisaBiddingConvite.Pesquisar.idGrid, "BiddingConvite/Pesquisar", _pesquisaBiddingConvite, menuOpcoes);
    _gridBiddingConvite.CarregarGrid();
}

// Eventos de Clique

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function PesquisarConvite() {
    _gridBiddingConvite.CarregarGrid();
}

function EditarClick(registroSelecionado, duplicar) {
    LimparCamposBiddingConvite();
    _CRUDBiddingConvite.Adicionar.visible(duplicar);
    _CRUDBiddingConvite.Atualizar.visible(!duplicar);

    _biddingConvite.Descricao.val(registroSelecionado.Descricao);
    _biddingConvite.DataInicio.val(registroSelecionado.DataInicio);
    _biddingConvite.DataLimite.val(registroSelecionado.DataLimite);

    executarReST("BiddingConvite/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo, Duplicar: duplicar }, function (retorno) {

        if (retorno.Success) {
            if (retorno.Data) {
                _biddingConvite.Codigo.val(retorno.Data.Codigo);
                _pesquisaBiddingConvite.ExibirFiltros.visibleFade(false);
                _listaAnexo.Anexos.val(retorno.Data.Anexos.slice());
                _listaAnexoTipoBidding.Anexos.val(retorno.Data.AnexosTipoBidding.slice());
                _convidados = retorno.Data.Convidados.slice();
                _biddingConvite.PrazoAceiteConvite.val(retorno.Data.DadosBiddingConvite.PrazoAceiteConvite);
                _biddingConvite.DescritivoConvite.val(retorno.Data.DadosBiddingConvite.DescritivoConvite);
                _biddingConvite.DescritivoTransportador.val(retorno.Data.DadosBiddingConvite.DescritivoTransportador);
                _biddingConvite.Situacao.val(retorno.Data.DadosBiddingConvite.Situacao);
                _biddingConvite.SituacaoAprovacao.val(retorno.Data.DadosBiddingConvite.SituacaoAprovacao);
                _biddingConvite.DataInicioVigencia.val(retorno.Data.DadosBiddingConvite.DataInicioVigencia);
                _biddingConvite.DataFimVigencia.val(retorno.Data.DadosBiddingConvite.DataFimVigencia);
                _biddingConvite.TipoFrete.val(retorno.Data.DadosBiddingConvite.TipoFrete);
                _biddingConvite.TipoBidding.codEntity(retorno.Data.DadosBiddingConvite.TipoBidding.Codigo);
                _biddingConvite.TipoBidding.val(retorno.Data.DadosBiddingConvite.TipoBidding.Descricao);
                _convidado.Convidado.basicTable.CarregarGrid(_convidados);
                _checklist.PrazoChecklist.val(retorno.Data.Checklist.Prazo);
                _checklist.PreenchimentoChecklist.val(retorno.Data.Checklist.TipoPreenchimentoChecklist);
                _checklist.Codigo.val(retorno.Data.Checklist.Codigo);
                const _questionarios = retorno.Data.Questionarios.slice();
                questionario = _questionarios;
                _gridChecklistQuestionarios.CarregarGrid(_questionarios);
                _ofertas.PrazoOferta.val(retorno.Data.Oferta.PrazoOferta);
                _ofertas.PermitirInformarVeiculosVerdes.val(retorno.Data.Oferta.PermitirInformarVeiculosVerdes);
                _ofertas.TipoLance.val(retorno.Data.Oferta.TipoLance);                
                rota = retorno.Data.Rotas.slice();
                _gridRotasBasicTable.CarregarGrid(rota);
                _gridRotas.Rotas.val(rota);
                _gridRotas.Excluir.visible(false);
                _convidado.Convidado.visible(_CONFIGURACAO_TMS.TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding);

                if (duplicar)
                    return;

                if (retorno.Data.DadosBiddingConvite.Iniciado) {
                    _CRUDBiddingConvite.Atualizar.visible(false);
                    _adicionarRotaBotoes.Adicionar.visible(false);
                    _gridRotas.Adicionar.visible(false);
                    _gridQuestionario.Adicionar.visible(false);
                    _listaAnexo.Adicionar.visible(false);
                    _gridRotasBasicTable.DesabilitarOpcoes();
                    _gridChecklistQuestionarios.DesabilitarOpcoes();
                    _gridAnexo.DesabilitarOpcoes();
                    _gridConvidado.DesabilitarOpcoes();
                }

                if (retorno.Data.DadosBiddingConvite.Etapa == "Aguardando Aprovação") {
                    _biddingConvite.Situacao.enable(true);
                    _biddingConvite.Descricao.enable(true);
                    _biddingConvite.DescritivoConvite.enable(true);
                    _biddingConvite.DescritivoTransportador.enable(true);
                    _biddingConvite.PrazoAceiteConvite.enable(true);
                    _biddingConvite.TipoFrete.enable(true);
                    _biddingConvite.TipoBidding.enable(true);
                    _biddingConvite.DataInicioVigencia.enable(true);
                    _biddingConvite.PrazoAceiteConvite.enable(true);
                    _checklist.PrazoChecklist.enable(true);
                    _biddingConvite.DataFimVigencia.enable(true);
                    _CRUDBiddingConvite.Atualizar.visible(true);
                    SetarEnableCamposKnockout(_ofertas, true);
                    _ofertas.PrazoOferta.enable(true);
                    _gridRotas.Adicionar.visible(true);
                    _gridRotasBasicTable.HabilitarOpcoes();
                    _adicionarRotaBotoes.Adicionar.visible(true);
                    _convidado.Convidado.visible(true);

                    $("#liTabAnexos").hide();
                }
                else if (retorno.Data.DadosBiddingConvite.Etapa != "Finalizado") {
                    _biddingConvite.Situacao.enable(false);
                    _biddingConvite.Descricao.enable(false);
                    _biddingConvite.DescritivoConvite.enable(false);
                    _biddingConvite.DescritivoTransportador.enable(false);
                    _biddingConvite.DataInicioVigencia.enable(false);
                    _biddingConvite.PrazoAceiteConvite.enable(false);
                    _ofertas.PrazoOferta.enable(false);
                    _checklist.PrazoChecklist.enable(false);
                    _biddingConvite.DataFimVigencia.enable(false);
                    _CRUDBiddingConvite.Atualizar.visible(true);
                    SetarEnableCamposKnockout(_ofertas, false);
                    _biddingConvite.TipoBidding.enable(false);
                } else {
                    _biddingConvite.Situacao.enable(true);
                    _biddingConvite.Descricao.enable(true);
                    _biddingConvite.DescritivoConvite.enable(true);
                    _biddingConvite.DescritivoTransportador.enable(true);
                    _biddingConvite.DataInicioVigencia.enable(true);
                    _biddingConvite.PrazoAceiteConvite.enable(true);
                    _ofertas.PrazoOferta.enable(true);
                    _checklist.PrazoChecklist.enable(true);
                    _biddingConvite.DataFimVigencia.enable(true);
                    _biddingConvite.TipoBidding.enable(true);
                    _CRUDBiddingConvite.Atualizar.visible(false);

                    $("#liTabAnexos").hide();
                }
                return SetarEtapaBiddingConvite();
            }
            return exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        return exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);

}

function adicionarClick(e, sender) {
    if (validarCamposObrigatoriosBiddingConvite())
        adicionarDadosBiddingConvite();
}

function limparClick() {
    LimparCamposBiddingConvite();
}

function atualizarClick() {

    if (!validarCamposObrigatoriosBiddingConvite())
        return;

    executarReST("BiddingConvite/Atualizar", obterDados(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Bidding Atualizado com sucesso");
                enviarArquivosAnexadosQuestionario(retorno.Data.questionarioCodigos.slice());
                _gridBiddingConvite.CarregarGrid();
                LimparCamposBiddingConvite();
                limparGridConvidados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

// Funções privadas
function obterDados() {
    const dadosBidding = RetornarObjetoPesquisa(_biddingConvite);
    preencherConvite(dadosBidding);
    preencherChecklist(dadosBidding); 
    preencherOferta(dadosBidding);

    return dadosBidding;
}

function preencherConvite(dadosBidding) {
    dadosBidding["Convite"] = ObterConviteSalvar();
}

function ObterConviteSalvar() {
    let listaConvidados = obterConvidados();

    return JSON.stringify(listaConvidados);
}

function adicionarDadosBiddingConvite() {
    executarReST("BiddingConvite/Adicionar", obterDados(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Bidding adicionado com sucesso");

                _gridBiddingConvite.CarregarGrid();
                enviarArquivosAnexados(retorno.Data.Codigo);
                enviarArquivosAnexadosQuestionario(retorno.Data.questionarioCodigos.slice());
                LimparCamposBiddingConvite();
                limparGridConvidados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);

}

function LimparCamposBiddingConvite() {
    _gridRotasBasicTable.HabilitarOpcoes();
    _gridChecklistQuestionarios.HabilitarOpcoes();
    _gridAnexo.HabilitarOpcoes();
    _gridConvidado.HabilitarOpcoes();
    _adicionarRotaBotoes.Adicionar.visible(true);
    _gridRotas.Adicionar.visible(true);
    _gridQuestionario.Adicionar.visible(true);
    _listaAnexo.Adicionar.visible(true);
    $("#liTabAnexos").show();
    _convidado.Convidado.visible(true);
    _CRUDBiddingConvite.Adicionar.visible(true);
    _CRUDBiddingConvite.Atualizar.visible(false);
    LimparCampos(_biddingConvite);
    _listaAnexo.Anexos.val([]);
    _listaAnexoTipoBidding.Anexos.val([]);
    LimparCampos(_checklist);
    LimparCampos(_ofertas);
    limparCamposChecklist();
    rota = [];
    _gridRotasBasicTable.CarregarGrid([]);
    limparGridConvidados();
    SetarEnableCamposKnockout(_ofertas, true);
    SetarEnableCamposKnockout(_biddingConvite, true);
    _biddingConvite.Codigo.enable(false);
    SetarEtapaInicioBiddingConvite();
}

function obterBaselineFinalizar() {

    let baselineFinalizar = RetornarObjetoPesquisa(_dadosFechamento);

    preencherBaselineSalvar(baselineFinalizar);

    return baselineFinalizar;
}

function verificarExistenciaRegraTipoBidding() {
    executarReST("BiddingConvite/VerificarExistenciaRegraBiddingConvite", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            if (retorno.Data.Regras) {
                _biddingConvite.TipoBidding.visible(true);
                _pesquisaBiddingConvite.TipoBidding.visible(true);
            } else {
                _biddingConvite.TipoBidding.visible(false);
                _pesquisaBiddingConvite.TipoBidding.visible(false);
            }
        }
    });
}

function callbackTipoBidding(tipoBidding) {
    executarReST("TipoBidding/VerificarAnexoTipoBidding", { CodigoTipoBidding: tipoBidding.Codigo }, function (retorno) {
        if (retorno.Success && retorno.Data) {
            _biddingConvite.TipoBidding.val(tipoBidding.Descricao);
            _biddingConvite.TipoBidding.codEntity(tipoBidding.Codigo);

            _listaAnexoTipoBidding.Anexos.val(retorno.Data.Anexos.slice());
        }
    });

    executarReST("TipoBidding/VerificarQuestionarioTipoBidding", { CodigoTipoBidding: tipoBidding.Codigo }, function (retorno) {
        if (retorno.Success && retorno.Data) {
            const retornoQuestionario = retorno.Data.Questionarios.slice();
            for (let i = 0; i < retornoQuestionario.length; i++) {
                const questao = retornoQuestionario[i];
                questionario.push({
                    Codigo: questao.Codigo,
                    Descricao: questao.Descricao,
                    Requisito: questao.Requisito,
                    ChecklistAnexo: questao.ChecklistAnexo
                });
            }
            _gridChecklistQuestionarios.CarregarGrid(questionario);
        }
    });
}

function validarCamposObrigatoriosBiddingConvite() {
    if (!ValidarCamposObrigatorios(_biddingConvite) || !ValidarCamposObrigatorios(_checklist) || !ValidarCamposObrigatorios(_ofertas)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return false;
    }

    if (verificarSeExistePerguntaQuestionario()) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe ao menos uma pergunta no checklist!");
        return false;
    }

    if (_ofertas.TipoLance.val() == EnumTipoLanceBidding.NaoSelecionado) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Selecione um Tipo Lance!");
        return false;
    }

    return true;
}

function LoadValidacaoObrigatoriedadeDatas() {
    if (_CONFIGURACAO_TMS.PermiteRemoverObrigatoriedadeDatas) {
        _biddingConvite.PrazoAceiteConvite.required(false);
        _biddingConvite.PrazoAceiteConvite.text("Prazo para aceite do convite");

        _checklist.PrazoChecklist.required(false);
        _checklist.PrazoChecklist.text("Prazo Preenchimento Checklist");

        _ofertas.PrazoOferta.required(false);
        _ofertas.PrazoOferta.text("Prazo Preenchimento Oferta");
    }
}