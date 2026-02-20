/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="AutorizacaoTaxaDescarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _delegarSelecionados;
var _delegar;

/*
 * Declaração das Classes
 */

var Delegar = function () {
    this.UsuarioDelegado = PropertyEntity({ text: "Usuário Responsável:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.Delegar = PropertyEntity({ eventClick: delegarClick, type: types.event, text: "Delegar", visible: ko.observable(true) });
}

var DelegarSelecionados = function () {
    this.UsuarioDelegado = PropertyEntity({ text: "Usuário Responsável:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.Delegar = PropertyEntity({ eventClick: delegarSelecionadosClick, type: types.event, text: "Delegar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarDelegarSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadDelegar() {
    _delegar = new Delegar();
    KoBindings(_delegar, "knockoutDelegar");

    _delegarSelecionados = new DelegarSelecionados();
    KoBindings(_delegarSelecionados, "knockoutDelegarSelecionados");

    new BuscarFuncionario(_delegar.UsuarioDelegado);
    new BuscarFuncionario(_delegarSelecionados.UsuarioDelegado);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function delegarClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja delegar a taxa de descarga?", function () {
        var dados = {
            Codigo: _taxaDescarga.Codigo.val(),
            UsuarioDelegado: _delegar.UsuarioDelegado.codEntity(),
        };

        executarReST("AutorizacaoTaxaDescarga/Delegar", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    atualizarGrid_taxaDescarga();
                    atualizar_taxaDescarga()
                    LimparCampos(_delegar);
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        })
    });
}

function delegarSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja delegar todas as taxas de descarga selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisa_taxaDescarga);

        dados.SelecionarTodos = _pesquisaTaxaDescarga.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridTaxaDescarga.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridTaxaDescarga.ObterMultiplosNaoSelecionados());
        dados.UsuarioDelegado = _delegarSelecionados.UsuarioDelegado.codEntity();

        executarReST("AutorizacaoTaxaDescarga/DelegarMultiplas", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                    atualizarGridTaxaDescarga();
                    cancelarDelegarSelecionadosClick();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        })
    });
}

function cancelarDelegarSelecionadosClick() {
    LimparCampos(_delegarSelecionados);
    Global.fecharModal("divModalDelegarSelecionados");
}