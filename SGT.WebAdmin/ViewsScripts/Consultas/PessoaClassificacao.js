/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/Globais.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/Rest.js" />
/// <reference path="../Enumeradores/EnumPessoaClasse.js" />

var BuscarPessoaClassificacao = function (knout, callbackRetorno) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.PessoaClassificacao.BuscarClassificacaoPessoa, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.PessoaClassificacao.ClassificacoesPessoas, type: types.local });
        this.Descricao = PropertyEntity({ text: Localization.Resources.Consultas.PessoaClassificacao.Descricao.getFieldDescription(), col: 8 });
        this.Classe = PropertyEntity({ text: Localization.Resources.Consultas.PessoaClassificacao.Classe.getFieldDescription(), col: 4, val: ko.observable(EnumPessoaClasse.Todas), options: EnumPessoaClasse.obterOpcoesPesquisa(), def: EnumPessoaClasse.Todas });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.PessoaClassificacao.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "PessoaClassificacao/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}
