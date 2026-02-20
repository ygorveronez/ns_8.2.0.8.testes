var BuscarContatosPessoa = function (knout, knoutPessoa, knoutGrupoPessoas, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Contatos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Contatos", type: types.local });

        this.Nome = PropertyEntity({ col: 12, text: "Nome:", maxlength: 150 });

        this.Pessoa = PropertyEntity({ visible: false, val: ko.observable(0), def: 0 });
        this.GrupoPessoas = PropertyEntity({ visible: false, val: ko.observable(0), def: 0 });
        this.ObrigatorioPessoaOuGrupo = PropertyEntity({ visible: false, val: ko.observable(true), def: true });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = function () {
        if (knoutPessoa != null) {
            if (knoutPessoa.type == types.entity)
                knoutOpcoes.Pessoa.val(knoutPessoa.codEntity());
            else
                knoutOpcoes.Pessoa.val(knoutPessoa.val());
        }

        if (knoutGrupoPessoas != null) {
            if (knoutGrupoPessoas.type == types.entity)
                knoutOpcoes.GrupoPessoas.val(knoutGrupoPessoas.codEntity());
            else
                knoutOpcoes.GrupoPessoas.val(knoutGrupoPessoas.val());
        }
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Contato);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "PessoaContato/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Nome.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}