// #region Objetos Globais do Arquivo

var _aprovacaoPesagem;
var _detalheAutorizacaoPesagem;
var _gridAutorizacoesPesagem;

// #endregion Objetos Globais do Arquivo

// #region Classes

var AprovacaoPesagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.AprovacoesNecessarias = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações Necessárias:", enable: ko.observable(true) });
    this.Aprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações:", enable: ko.observable(true) });
    this.CorIconeAba = PropertyEntity({ val: ko.observable("") });
    this.PossuiRegras = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.Reprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Reprovações", enable: ko.observable(true) });
    this.Autorizacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid() });

    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasPesagemClick, type: types.event, text: "Reprocessar Regras" });
}

var DetalheAutorizacaoPesagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Regra = PropertyEntity({ text: "Regra:", val: ko.observable("") });
    this.Data = PropertyEntity({ text: "Data:", val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: "Usuário:", val: ko.observable("") });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadPesagemAprovacao() {
    _aprovacaoPesagem = new AprovacaoPesagem();
    KoBindings(_aprovacaoPesagem, "knockoutPesagemFinalAprovacao");

    _detalheAutorizacaoPesagem = new DetalheAutorizacaoPesagem();
    KoBindings(_detalheAutorizacaoPesagem, "knockoutDetalheAutorizacaoPesagem");

    loadGridAutorizacoesPesagem();
}

function loadGridAutorizacoesPesagem() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "Regra", visible: false },
        { data: "Data", visible: false },
        { data: "Motivo", visible: false },
        { data: "Usuario", title: "Usuário", width: "40%" },
        { data: "PrioridadeAprovacao", title: "Prioridade", width: "20%", className: "text-align-center" },
        { data: "Situacao", title: "Situação", width: "20%" }
    ];

    var opcaoDetalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharAutorizacaoPesagemClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };

    _gridAutorizacoesPesagem = new BasicDataTable(_aprovacaoPesagem.Autorizacoes.idGrid, header, menuOpcoes);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function detalharAutorizacaoPesagemClick(registroSelecionado) {
    _detalheAutorizacaoPesagem.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_detalheAutorizacaoPesagem, "Guarita/DetalhesAutorizacaoPesagem", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                Global.abrirModal("divModalDetalhesAutorizacaoPesagem");

                $("#divModalDetalhesAutorizacaoPesagem").one('hidden.bs.modal', function () {
                    LimparCampos(_detalheAutorizacaoPesagem);
                });
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function reprocessarRegrasPesagemClick() {
    executarReST("AutorizacaoPesagem/Reprocessar", { Codigo: _aprovacaoPesagem.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                buscarPesagemAprovacao();
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function preencherPesagemAprovacao(codigoGuarita) {
    _aprovacaoPesagem.Codigo.val(codigoGuarita);
    buscarPesagemAprovacao();
}

// #endregion Funções Públicas

// #region Funções Privadas

function buscarPesagemAprovacao() {
    executarReST("Guarita/PesquisaAutorizacaoPesagem", { Codigo: _aprovacaoPesagem.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                let possuiAbasVisiveis = Global.PossuiAbasVisiveis("container-abas-pesagem-final");

                PreencherObjetoKnout(_aprovacaoPesagem, retorno);
                _gridAutorizacoesPesagem.CarregarGrid(_aprovacaoPesagem.Autorizacoes.val());

                $("#tabPesagemFinalAprovacaoIcone").css("color", _aprovacaoPesagem.CorIconeAba.val());
                $("#liTabPesagemFinalAprovacao").show();

                if (!possuiAbasVisiveis) {
                    $("#liTabPesagemFinalAprovacao").addClass("active");
                    $("#liTabPesagemFinalAprovacao a").addClass("active");
                    $("#tabPesagemFinalAprovacao").addClass("active show");
                }
            }
            else if (retorno.Data === false)
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

// #endregion Funções Privadas
