/// <reference path="../../Enumeradores/EnumResponsavelAvaria.js" />
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
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Filial.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaTiposDetalhe;
var _gridTiposDetalhe;

var _tipo = [
    { text: "Detalhe Entrega", value: 5 },
    { text: "Horário Entrega", value: 2 },
    { text: "Período Entrega", value: 4 },
    { text: "Processamento Especial", value: 1 },
    { text: "Tipo Palete", value: 6 },
    { text: "Zona Transporte", value: 3 }
];


function obterTipoPesquisa() {
    return [{ text: "Todos", value: 0 }].concat(_tipo);
}

var _tipoPaleteCliente = [
    { text: "Chep", value: 1 },
    { text: "Batido", value: 2 },
    { text: "Palete Retorno", value: 3 }
];

function obterTipoPaleteClientePesquisa() {
    return [{ text: "Não Definido", value: 0 }].concat(_tipoPaleteCliente);
}

var PesquisaTiposDetalhe = function () {

    this.CodigoEmbarcador = PropertyEntity({ text: "Código Integração: ", getType: typesKnockout.string, type: types.string, required: ko.observable(false), visible: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: "Descrição: ", getType: typesKnockout.string, type: types.string, required: ko.observable(false), visible: ko.observable(true) });
    this.Tipo = PropertyEntity({ text: "Tipo de detalhe: ", val: ko.observable(0), options: obterTipoPesquisa(), def: 0 });
    this.ExibirTipoPaleteCliente = PropertyEntity({ val: ko.observable(true), def: true });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            pesquisarClick();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var TipoDetalhe = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração:", issue: 15, getType: typesKnockout.string, val: ko.observable(""), maxlength: 50 });
    this.Tipo = PropertyEntity({ text: "Tipo de detalhe: ", val: ko.observable(0), options: _tipo, def: 0 });
    this.TipoPaleteCliente = PropertyEntity({ text: "Tipo Palete Cliente: ", val: ko.observable(0), options: _tipoPaleteCliente, def: 0, visible: ko.observable(false), required: ko.observable(false) });
    this.Valor = PropertyEntity({ text: "Peso:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true }, maxlength: 10, visible: ko.observable(false), required: ko.observable(false) });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.Tipo.val.subscribe(function (novoValor) {
        if (_tipoDetalhe) {
            var habilita = novoValor == 6; //Tipo de Palete
            _tipoDetalhe.TipoPaleteCliente.visible(habilita);
            _tipoDetalhe.TipoPaleteCliente.required(habilita);
            _tipoDetalhe.Valor.visible(habilita);
            _tipoDetalhe.Valor.required(habilita);
        }
    });
}

//*******EVENTOS*******
function loadTipoDetalhePedido() {
    // Instancia pesquisa
    _pesquisaTiposDetalhe = new PesquisaTiposDetalhe();
    KoBindings(_pesquisaTiposDetalhe, "knockoutPesquisaTipoDetalhe", false, _pesquisaTiposDetalhe.Pesquisar.id);

    // Inicia busca
    loadGridTipoDetalhe();

    _tipoDetalhe = new TipoDetalhe();
    KoBindings(_tipoDetalhe, "knockoutTipoDetalhe");

}

function pesquisarClick() {
    if (ValidarCamposObrigatorios(_pesquisaTiposDetalhe)) {
        _gridTiposDetalhe.CarregarGrid();
    }
}

//*******MÉTODOS*******
function loadGridTipoDetalhe() {

    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoDetalheClick, tamanho: "10", icone: "" };
    
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 15,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridTiposDetalhe = new GridView(_pesquisaTiposDetalhe.Pesquisar.idGrid, "TipoDetalhe/Pesquisa", _pesquisaTiposDetalhe, menuOpcoes);
    _gridTiposDetalhe.CarregarGrid();
}

function adicionarClick(e, sender) {
    Salvar(_tipoDetalhe, "TipoDetalhe/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTiposDetalhe.CarregarGrid();
                limparCamposTipoDetalhe();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tipoDetalhe, "TipoDetalhe/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTiposDetalhe.CarregarGrid();
                limparCamposTipoDetalhe();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_tipoDetalhe, "TipoDetalhe/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTiposDetalhe.CarregarGrid();
                    limparCamposTipoDetalhe();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTipoDetalhe();
}

function editarTipoDetalheClick(itemGrid) {
    // Limpa os campos
    limparCamposTipoDetalhe();

    // Seta o codigo do ProdutoAvaria
    _tipoDetalhe.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_tipoDetalhe, "TipoDetalhe/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaTiposDetalhe.ExibirFiltros.visibleFade(false);
                // Alternas os campos de CRUD
                _tipoDetalhe.Atualizar.visible(true);
                _tipoDetalhe.Excluir.visible(true);
                _tipoDetalhe.Cancelar.visible(true);
                _tipoDetalhe.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);

}

function limparCamposTipoDetalhe() {
    _tipoDetalhe.Atualizar.visible(false);
    _tipoDetalhe.Cancelar.visible(false);
    _tipoDetalhe.Excluir.visible(false);
    _tipoDetalhe.Adicionar.visible(true);
    LimparCampos(_tipoDetalhe);
}