/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />

var BuscarEnderecos = function (knout, TituloOpcional, TituloGridOpcional, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: TituloOpcional != null ? TituloOpcional : Localization.Resources.Consultas.Localidade.PesquisaDeEndereco, type: types.local });
        this.TituloGrid = PropertyEntity({ text: TituloGridOpcional != null ? TituloGridOpcional : Localization.Resources.Consultas.Localidade.Enderecos, type: types.local });
        this.Localidade = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Localidade.DescricaoLocalidade.getRequiredFieldDescription(), idBtnSearch: guid(), visible: true, required: true });
        //this.CEP = PropertyEntity({ col: 4, text: "CEP: ", getType: typesKnockout.string, maxlength: 8 });
        this.CEP = PropertyEntity({ col: 0, text: Localization.Resources.Consultas.Localidade.CEP.getFieldDescription(), getType: typesKnockout.string, visible: false });
        this.Bairro = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Localidade.Bairro.getFieldDescription() });
        this.Logradouro = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.Localidade.Logradouro.getRequiredFieldDescription(), required: true });
        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.Localidade.Cidade.getFieldDescription(), visible: false });
        this.CodigoIBGE = PropertyEntity({ col: 4, getType: typesKnockout.string, maxlength: 7, text: Localization.Resources.Consultas.Localidade.CodigoIBGE.getFieldDescription(), visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                var obj = RetornarObjetoPesquisa(knoutOpcoes);
                var valido = true;

                if (obj.Localidade == 0) {
                    valido = false;
                    knoutOpcoes.Localidade.requiredClass("form-control is-invalid");
                }
                else
                    knoutOpcoes.Localidade.requiredClass("form-control");

                if (obj.Logradouro == "") {
                    valido = false;
                    knoutOpcoes.Logradouro.requiredClass("form-control is-invalid");
                }
                else
                    knoutOpcoes.Logradouro.requiredClass("form-control");

                if (valido)
                    GridConsulta.CarregarGrid();
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Consultas.Localidade.Pesquisa, Localization.Resources.Consultas.Localidade.ObrigatorioInformarUmaLocalidadeUmLogradouro);
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    $("#" + knoutOpcoes.CodigoIBGE.id).mask("0000000", { selectOnFocus: true, clearIfNotMatch: true });
    $("#" + knoutOpcoes.CEP.id).mask("00000000", { selectOnFocus: true, clearIfNotMatch: true });

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, null, function () {
        new BuscarLocalidades(knoutOpcoes.Localidade, null);
    });


    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Localidade/PesquisaEnderecoPorCEP", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
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
    })
}