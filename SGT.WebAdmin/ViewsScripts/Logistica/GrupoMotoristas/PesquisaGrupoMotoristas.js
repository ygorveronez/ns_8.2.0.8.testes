/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="Constantes.js" />

var PesquisaGrupoMotoristas = function () {
    this.Descricao = PropertyEntity({ text: "Descrição" });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração", maxlength: 50 });
    this.Situacao = PropertyEntity({ val: ko.observable(3), options: EnumSituacaoGrupoMotoristas.obterOpcoesPesquisa(), def: "", text: "Situação" });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: EnumSimNao.obterOpcoesPesquisa(), def: true, text: "Ativo", visible: true });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridGrupoMotoristas.CarregarGrid();
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
