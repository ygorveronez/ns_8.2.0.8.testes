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
/// <reference path="Analise.js" />

var _gridInformacoesFechamento;
var _motivosProcessoInformacoesFechamento = [];

var InformacoesFechamentoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });

    this.CodigoNotaFiscal = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.CodigoMotivoProcesso = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.NotaFiscal = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.MotivoProcesso = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.QuantidadeDivergencia = PropertyEntity({ type: types.map, val: ko.observable("") });
};

function loadInformacoesFechamento() {
    ObterGeneroAreaEnvolvida()
        .then((possuiGeneroAreaEnvolvida) => { if (possuiGeneroAreaEnvolvida && _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe) $("#liTabAnaliseInformacoesFechamento").show(); })
        .then(ObterMotivoProcesso)
        .then(function () {
            _analise.MotivoProcesso.options(_motivosProcessoInformacoesFechamento);
            _analise.MotivoProcesso.val(_motivosProcessoInformacoesFechamento[0].value);
        });

    new BuscarXMLNotaFiscal(_analise.NotaFiscalInformacaoFechamento, null, null, null, null, _abertura?.CargaEntrega);

    loadGridInformacoesFechamento();
}

function ObterGeneroAreaEnvolvida() {
    return new Promise((resolve) => {
        executarReST("ChamadoOcorrencia/ObterGeneroAreaEnvolvida", {}, function (r) {
            if (r.Success) {
                resolve(r.Data);
            }
        })
    });
}

function ObterMotivoProcesso() {
    return new Promise((resolve) => {
        executarReST("ChamadoOcorrencia/ObterMotivoProcesso", {}, function (r) {
            if (r.Success) {
                if (r.Data != undefined && r.Data != null && r.Data.length > 0) {
                    r.Data.forEach(tipo => _motivosProcessoInformacoesFechamento.push({ text: tipo.Descricao, value: tipo.Codigo }));
                } else {
                    _motivosProcessoInformacoesFechamento = [{ text: "Selecione um registro", value: 0 }];
                }
            } else {
                _motivosProcessoInformacoesFechamento = [{ text: "Selecione um registro", value: 0 }];
            }

            resolve();
        })
    });
}

function salvarInformacoesFechamentoEtapaAnaliseClick() {
    var analise = RetornarObjetoPesquisa(_analise);

    executarReST("ChamadoAnalise/SalvarInformacoesFechamentoEtapaAnalise", analise, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Chamado.ChamadoOcorrencia.Sucesso, retorno.Msg);
                buscarChamadoPorCodigo(_chamado.Codigo.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function ValidarCamposVisiveisEtapaAnalise() {
    var analise = RetornarObjetoPesquisa(_analise);

    executarReST("ChamadoAnalise/ValidarCamposVisiveisEtapaAnalise", analise, function (retorno) {
        if (retorno.Success)
            if (retorno.Data !== false) {
                _analise.SalvarInformacoesFechamento.visible(retorno.Data.SalvarInformacoesFechamento);
                _analise.SalvarInformacoesFechamento.val(retorno.Data.SalvarInformacoesFechamento);
                _CRUDIntegracao.ReenviarIntegracaoInformacoesFechamento.visible(retorno.Data.PermiteReenviarIntegracoesInformacoesFechamento);
                if (retorno.Data.SalvarInformacoesFechamento)
                    _analise.InformarDadosChamadoFinalizadoComCusto.enable(true);
            }
    });
}

function loadGridInformacoesFechamento() {
    if (_gridInformacoesFechamento != null && _gridInformacoesFechamento.Destroy)
        _gridInformacoesFechamento.Destroy();

    var excluir = { descricao: "Excluir", id: guid(), metodo: excluirInformacoesFechamentoClick, icone: "", visibilidade: visibilidadeExcluirInformacoesFechamento };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoNotaFiscal", visible: false },
        { data: "CodigoMotivoProcesso", visible: false },
        { data: "NotaFiscal", title: "Nota Fiscal", width: "20%" },
        { data: "MotivoProcesso", title: "Motivo do Processo", width: "40%" },
        { data: "QuantidadeDivergencia", title: "Quantidade", width: "20%" }
    ];

    _gridInformacoesFechamento = new BasicDataTable(_analise.GridInformacoesFechamento.id, header, menuOpcoes);
}

function visibilidadeExcluirInformacoesFechamento() {
    return _chamado.Situacao.val() === EnumSituacaoChamado.Aberto || _chamado.Situacao.val() === EnumSituacaoChamado.EmTratativa;
}

function adicionarInformacoesFechamentoClick(e, sender) {
    if (_analise.NotaFiscalInformacaoFechamento.codEntity() == 0 || _analise.QuantidadeDivergencia.val() == 0)
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");

    var notaExistente = false;
    $.each(_analise.InformacoesFechamento.list, function (i, lista) {
        if (_analise.NotaFiscalInformacaoFechamento.codEntity() == lista.CodigoNotaFiscal.val) {
            notaExistente = true;
            return false;
        }
    });
    if (notaExistente)
        return exibirMensagem(tipoMensagem.atencao, "Atenção", "Nota fiscal já informada!");

    var lista = new InformacoesFechamentoMap();

    lista.Codigo.val = guid();
    lista.CodigoNotaFiscal.val = _analise.NotaFiscalInformacaoFechamento.codEntity();
    lista.NotaFiscal.val = _analise.NotaFiscalInformacaoFechamento.val();
    lista.CodigoMotivoProcesso.val = _analise.MotivoProcesso.val();
    lista.MotivoProcesso.val = $("#" + _analise.MotivoProcesso.id + " option:selected").text();
    lista.QuantidadeDivergencia.val = _analise.QuantidadeDivergencia.val();

    _analise.InformacoesFechamento.list.push(lista);

    limparCamposInformacoesFechamento();

    $("#" + _analise.NotaFiscalInformacaoFechamento.id).focus();
    recarregarGridInformacoesFechamento();
}

function excluirInformacoesFechamentoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a informação de fechamento?", function () {
        $.each(_analise.InformacoesFechamento.list, function (i, lista) {
            if (data.Codigo == lista.Codigo.val) {
                _analise.InformacoesFechamento.list.splice(i, 1);
                return false;
            }
        });

        recarregarGridInformacoesFechamento();
    });
}

function recarregarGridInformacoesFechamento() {
    var data = new Array();

    $.each(_analise.InformacoesFechamento.list, function (i, lista) {
        var listaGrid = new Object();

        listaGrid.Codigo = lista.Codigo.val;
        listaGrid.CodigoNotaFiscal = lista.CodigoNotaFiscal.val;
        listaGrid.CodigoMotivoProcesso = lista.CodigoMotivoProcesso.val;
        listaGrid.NotaFiscal = lista.NotaFiscal.val;
        listaGrid.MotivoProcesso = lista.MotivoProcesso.val;
        listaGrid.QuantidadeDivergencia = lista.QuantidadeDivergencia.val;

        data.push(listaGrid);
    });

    _gridInformacoesFechamento.CarregarGrid(data);
}

function controleCamposInformacoesFechamento(novoValor) {
    if (novoValor) {
        _analise.NotaFiscalInformacaoFechamento.visible(true);
        _analise.MotivoProcesso.visible(true);
        _analise.QuantidadeDivergencia.visible(true);
        _analise.AdicionarInformacaoFechamento.visible(true);

        if (_analise.SalvarInformacoesFechamento.val())
            _analise.SalvarInformacoesFechamento.visible(true);
    } else {
        _analise.NotaFiscalInformacaoFechamento.visible(false);
        _analise.MotivoProcesso.visible(false);
        _analise.QuantidadeDivergencia.visible(false);
        _analise.AdicionarInformacaoFechamento.visible(false);
        _analise.SalvarInformacoesFechamento.visible(false);
    }
}

function controleStatusCamposInformacoesFechamento(status) {
    _analise.InformarDadosChamadoFinalizadoComCusto.enable(status);
    _analise.QuantidadeDivergencia.enable(status);
    _analise.MotivoProcesso.enable(status);
    _analise.NotaFiscalInformacaoFechamento.enable(status);
    _analise.AdicionarInformacaoFechamento.enable(status);
}

function limparCamposInformacoesFechamento() {
    LimparCampo(_analise.QuantidadeDivergencia);
    LimparCampoEntity(_analise.NotaFiscalInformacaoFechamento);
}