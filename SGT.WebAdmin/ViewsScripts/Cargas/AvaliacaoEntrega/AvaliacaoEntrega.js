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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEntrega.js" />
/// <reference path="../../Enumeradores/EnumAvaliacaoEntrega.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaEntrega;
var _pesquisaCargaEntrega;
var _avaliacaoEntrega;
var _CRUDAvaliacaoEntrega;
var _perguntas;
var _habilitarAvaliacao;
var _habilitarAvaliacaoQuestionario;
var _motivosAvaliacao = [];
var _modalGerenciarAnexosPergunta;



var PesquisaCargaEntrega = function () {
    this.NumeroCarga = PropertyEntity({ text: Localization.Resources.Cargas.AvaliacaoEntrega.NumCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), idBtnSearch: guid() });
    this.NumeroNotaFiscal = PropertyEntity({ text: Localization.Resources.Cargas.AvaliacaoEntrega.NumNF.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroTransporte = PropertyEntity({ text: Localization.Resources.Cargas.AvaliacaoEntrega.NumTransporte.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Respondida = PropertyEntity({ text: Localization.Resources.Cargas.AvaliacaoEntrega.SituacaoAvaliacao.getFieldDescription(), options: ko.observable(EnumRespondida.obterOpcoesPesquisa()), val: ko.observable(0), def: 0 });
    this.SituacaoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.AvaliacaoEntrega.SituacaoEntrega.getFieldDescription(), options: ko.observable(EnumSituacaoEntrega.obterOpcoesPesquisa()), val: ko.observable(EnumSituacaoEntrega.Entregue), def: EnumSituacaoEntrega.Entregue });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCargaEntrega.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Pergunta = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Pergunta = PropertyEntity({ val: ko.observable(""), def: "" });
    this.DescricaoPergunta = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Ordem = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Resposta = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.BotaoAnexos = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Anexos, visible: ko.observable(false) });
    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });
};

var AvaliacaoEntrega = function () {
    this.CodigoCargaEntrega = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.NumeroCarga = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Carga, val: ko.observable(""), def: "" });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Destinatario, val: ko.observable(""), def: "" });
    this.SituacaoAvaliacao = PropertyEntity({ text: Localization.Resources.Cargas.AvaliacaoEntrega.SituacaoAvaliacao, val: ko.observable(""), def: "" });

    this.Perguntas = ko.observableArray();
    this.MotivoAvaliacao = PropertyEntity({ text: Localization.Resources.Cargas.AvaliacaoEntrega.MotivoAvaliacao, options: _motivosAvaliacao, val: ko.observable(_motivosAvaliacao[0].value), def: _motivosAvaliacao[0].value, visible: ko.observable(false), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao, val: ko.observable(""), def: "", enable: ko.observable(true) });
};

var CRUDAvaliacao = function () {
    this.Atualizar = PropertyEntity({ type: types.event, eventClick: atualizarAvaliacaoEntregaClick, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ type: types.event, eventClick: limparAvaliacaoEntregaClick, text: Localization.Resources.Cargas.AvaliacaoEntrega.Limpar, visible: ko.observable(true) });
};

function loadAvaliacaoEntrega() {
    BuscarPerguntas().then(function () {

        _pesquisaCargaEntrega = new PesquisaCargaEntrega();
        KoBindings(_pesquisaCargaEntrega, "knoutPesquisaCargaEntrega");

        _avaliacaoEntrega = new AvaliacaoEntrega();
        KoBindings(_avaliacaoEntrega, "knoutAvaliacaoEntrega");

        _CRUDAvaliacaoEntrega = new CRUDAvaliacao();
        KoBindings(_CRUDAvaliacaoEntrega, "knoutCRUDAvaliacaoEntrega");

        new BuscarClientes(_pesquisaCargaEntrega.Destinatario);

        _avaliacaoEntrega.Perguntas(_perguntas);

        if (!_habilitarAvaliacaoQuestionario) {
            $("input[name=avaliacao-0]").on("change", ValidarCampoMotivoAvaliacao);
        }

        _CRUDAvaliacaoEntrega.Atualizar.visible(false);

        BuscarCargaEntrega();
    });
}

function BuscarCargaEntrega() {
    var editar = { descricao: Localization.Resources.Cargas.AvaliacaoEntrega.Avaliacao, id: "clasEditar", evento: "onclick", metodo: editarAvaliacaoEntregaClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCargaEntrega = new GridView(_pesquisaCargaEntrega.Pesquisar.idGrid, "AvaliacaoEntrega/Pesquisa", _pesquisaCargaEntrega, menuOpcoes);
    _gridCargaEntrega.CarregarGrid();
}

function editarAvaliacaoEntregaClick(data) {
    LimparAvaliacaoEntrega();
    executarReST("AvaliacaoEntrega/BuscarPorCodigo", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherObjetoAvaliacaoEntrega(r.Data);
                visibilidadeBotaoAnexosPerguntas(_modalGerenciarAnexosPergunta);
            } else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
    });
}

function atualizarAvaliacaoEntregaClick() {
    if (!ValidarRespostaAvaliacaoEntrega()) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.AvaliacaoEntrega.NecessarioInformarAvaliacaoCorretamente);
        return;
    }

    let data = {
        CodigoCargaEntrega: _avaliacaoEntrega.CodigoCargaEntrega.val(),
        AvaliacaoGeral: $("input[name=avaliacao-geral]:checked").val() || null,
        Questionario: JSON.stringify(ObterRespostaPerguntas()),
        MotivoAvaliacao: !_habilitarAvaliacaoQuestionario ? _avaliacaoEntrega.MotivoAvaliacao.val() : null,
        Observacao: _avaliacaoEntrega.Observacao.val()
    };

    executarReST("AvaliacaoEntrega/InserirAvaliacao", data, function (r) {
        if (r.Success) {
            if (r.Data) {

                r.Data.forEach(avaliacao => {
                    _avaliacaoEntrega.Perguntas().forEach(pergunta => {
                        if (pergunta.Codigo.val() == avaliacao.CodigoPergunta) {
                            enviarAnexos(avaliacao.CodigoAvaliacaoEntrega, pergunta.Anexos.val());
                        }
                    })
                });

                LimparAvaliacaoEntrega();
                _gridCargaEntrega.CarregarGrid();

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.AvaliacaoEntrega.AvalicaoSalvaSucesso);
            } else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
    });
}

function limparAvaliacaoEntregaClick() {
    LimparAvaliacaoEntrega();
}

function LimparAvaliacaoEntrega() {
    LimparCampos(_avaliacaoEntrega);
    _avaliacaoEntrega.Perguntas(_perguntas);

    if (_modalGerenciarAnexosPergunta) {
        limparCamposAnexo();
        removerAnexosPerguntas();
    }

    $("input[name=avaliacao-geral]").prop("disabled", false).prop("checked", false); //Desbloqueia e desmarca o campo

    for (let i = 0; i < _avaliacaoEntrega.Perguntas().length; i++)
        $("input[name=avaliacao-" + _avaliacaoEntrega.Perguntas()[i].Codigo.val() + "]").prop("disabled", false).prop("checked", false); //Desbloqueia e desmarca o campo

    if (!_habilitarAvaliacaoQuestionario) {
        $("input[name=avaliacao-0]").on("change", ValidarCampoMotivoAvaliacao);
    }

    _avaliacaoEntrega.MotivoAvaliacao.visible(false);
    _avaliacaoEntrega.MotivoAvaliacao.enable(true);
    _avaliacaoEntrega.Observacao.enable(true);

    _CRUDAvaliacaoEntrega.Atualizar.visible(false);
    _CRUDAvaliacaoEntrega.Limpar.visible(true);
}

function BuscarPerguntas() {
    let p = new promise.Promise();

    executarReST("AvaliacaoEntrega/BuscarConfiguracaoPerguntasAvaliacaoEntrega", {}, function (r) {
        if (r.Success) {
            if (r.Data) {

                _habilitarAvaliacao = r.Data.HabilitarAvaliacao;
                _habilitarAvaliacaoQuestionario = r.Data.HabilitarAvaliacaoQuestionario;

                _motivosAvaliacao = [];
                _perguntas = [];

                r.Data.MotivosAvaliacao.forEach(function (motivo) {
                    _motivosAvaliacao.push({ text: motivo.Descricao, value: motivo.Valor });
                });

                if (_motivosAvaliacao.length <= 0)
                    _motivosAvaliacao.push({ text: "--selecione--", value: 1 });

                r.Data.Perguntas.forEach(function (pergunta) {
                    _perguntas.push(mapearPerguntas(pergunta));
                });

                if (r.Data.HabilitarAnexos) {
                    $.get("Content/Static/Carga/ModaisAvaliacaoEntrega.html?dyn=" + guid(), function (htmlModaisAvaliacaoEntrega) {
                        $("#ModaisAvaliacaoEntrega").html(htmlModaisAvaliacaoEntrega);

                        loadAnexo();
                        _modalGerenciarAnexosPergunta = new bootstrap.Modal(document.getElementById("divModalGerenciarAnexosPergunta"), { backdrop: 'static' });

                        visibilidadeBotaoAnexosPerguntas(true);
                    });
                }

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
}

function ValidarCampoMotivoAvaliacao() {
    let numeroEstrelas = $("input[name=avaliacao-0]:checked").val();
    if (numeroEstrelas <= 3) {
        _avaliacaoEntrega.MotivoAvaliacao.visible(true);
    }
    else {
        _avaliacaoEntrega.MotivoAvaliacao.visible(false);
    }
}

function ObterRespostaPerguntas() {
    let respostas = [];

    for (let i = 0; i < _avaliacaoEntrega.Perguntas().length; i++) {
        let codigoPergunta = _avaliacaoEntrega.Perguntas()[i].Codigo.val();
        let resposta = $(`input[name=avaliacao-${codigoPergunta}]:checked`).val() || null;
        respostas.push({ Codigo: codigoPergunta, Resposta: resposta });
    }

    return respostas;
}

function PreencherObjetoAvaliacaoEntrega(data) {
    _avaliacaoEntrega.CodigoCargaEntrega.val(data.Codigo);
    _avaliacaoEntrega.NumeroCarga.val(data.Carga);
    _avaliacaoEntrega.Destinatario.val(data.Destinatario);
    _avaliacaoEntrega.SituacaoAvaliacao.val(data.SituacaoAvaliacao);

    if (data.AvaliacaoRespondida) {
        if (data.AvaliacaoGeral > 0)
            $(`input[id=avaliacao-geral-${data.AvaliacaoGeral}]`).prop("checked", true); //Seleciona a opção que foi avaliada
        else
            $("input[name=avaliacao-geral]").prop("checked", false);

        $("input[name=avaliacao-geral]").prop("disabled", true); //Desabilita a avaliação geral

        _avaliacaoEntrega.Perguntas([]);
        data.Avaliacoes.forEach(function (pergunta) {
            _avaliacaoEntrega.Perguntas.push(mapearPerguntas(pergunta));
        });

        for (let i = 0; i < _avaliacaoEntrega.Perguntas().length; i++) {
            $(`input[name=avaliacao-${_avaliacaoEntrega.Perguntas()[i].Codigo.val()}]`).prop("disabled", true); //Desabilita todas as perguntas

            if (_avaliacaoEntrega.Perguntas()[i].Resposta.val() > 0)
                $(`input[id=star-rate-${_avaliacaoEntrega.Perguntas()[i].Resposta.val()}-${_avaliacaoEntrega.Perguntas()[i].Codigo.val()}]`).prop("checked", true); //Marca a opção que foi avaliado
            else
                $(`input[name=avaliacao-${_avaliacaoEntrega.Perguntas()[i].Codigo.val()}]`).prop("checked", false);
        }

        _avaliacaoEntrega.MotivoAvaliacao.val(data.MotivoAvaliacao);
        _avaliacaoEntrega.MotivoAvaliacao.enable(false);
        _avaliacaoEntrega.Observacao.val(data.Observacao);
        _avaliacaoEntrega.Observacao.enable(false);

        _CRUDAvaliacaoEntrega.Atualizar.visible(false);
        _CRUDAvaliacaoEntrega.Limpar.visible(true);
    } else if ((_habilitarAvaliacao && data.Entregue) && !data.AvaliacaoRespondida) {
        $("input[name=avaliacao-geral]").prop("disabled", false).prop("checked", false);

        _avaliacaoEntrega.Perguntas(_perguntas);

        for (let i = 0; i < _avaliacaoEntrega.Perguntas().length; i++)
            $(`input[name=avaliacao-${_avaliacaoEntrega.Perguntas()[i].Codigo.val()}]`).prop("disabled", false).prop("checked", false);

        _avaliacaoEntrega.MotivoAvaliacao.val(_motivosAvaliacao[0].value);
        _avaliacaoEntrega.Observacao.val("");

        _CRUDAvaliacaoEntrega.Atualizar.visible(true);
        _CRUDAvaliacaoEntrega.Limpar.visible(true);
    } else {
        $("input[name=avaliacao-geral]").prop("disabled", true).prop("checked", false);

        _avaliacaoEntrega.Perguntas(_perguntas);

        for (let i = 0; i < _avaliacaoEntrega.Perguntas().length; i++)
            $(`input[name=avaliacao-${_avaliacaoEntrega.Perguntas()[i].Codigo.val()}]`).prop("disabled", true).prop("checked", false);

        _avaliacaoEntrega.MotivoAvaliacao.val(_motivosAvaliacao[0].value);
        _avaliacaoEntrega.MotivoAvaliacao.enable(false);
        _avaliacaoEntrega.Observacao.val("");
        _avaliacaoEntrega.Observacao.enable(false);

        _CRUDAvaliacaoEntrega.Atualizar.visible(false);
        _CRUDAvaliacaoEntrega.Limpar.visible(true);
    }
}

function mapearPerguntas(pergunta) {
    let objetoPergunta = new Pergunta();

    objetoPergunta.Codigo.val(pergunta.Codigo);
    objetoPergunta.Ordem.val(pergunta.Ordem);
    objetoPergunta.Pergunta.val(pergunta.Titulo);
    objetoPergunta.DescricaoPergunta.val(pergunta.Conteudo.replaceAll("\n", "<br />"));
    objetoPergunta.Resposta.val(pergunta.Resposta);

    if (pergunta.Anexos)
        objetoPergunta.Anexos.val(pergunta.Anexos);

    return objetoPergunta;
}

function ValidarRespostaAvaliacaoEntrega() {
    let avaliacaoGeral = $("input[name=avaliacao-geral]:checked").val();

    if (avaliacaoGeral == undefined)
        return false;

    for (let i = 0; i < _avaliacaoEntrega.Perguntas().length; i++) {
        let resposta = $(`input[name=avaliacao-${_avaliacaoEntrega.Perguntas()[i].Codigo.val()}]:checked`).val();
        if (resposta == undefined)
            return false;
    }

    return true;
}

function gerenciarAnexosPerguntaClick(codigoPergunta) {
    _cabecalhoModal.CodigoPergunta.val(codigoPergunta);

    _gridAnexo.CarregarGrid(obterAnexos());

    _modalGerenciarAnexosPergunta.show();
}

function removerAnexosPerguntas() {
    _avaliacaoEntrega.Perguntas().forEach(pergunta => {
        pergunta.Anexos.val([]);
    })
}

function visibilidadeBotaoAnexosPerguntas(isVisivel) {

    _avaliacaoEntrega.Perguntas().forEach(function (pergunta) {
        pergunta.BotaoAnexos.visible(isVisivel);
    });
}