/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />

var BuscarNaturezasOperacoes = function (knout, tituloOpcional, tituloGridOpcional, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: tituloOpcional != null ? tituloOpcional : Localization.Resources.Consultas.NaturezaOperacao.ConsultaDeNaturezasDeOperacoes, type: types.local });
        this.TituloGrid = PropertyEntity({ text: tituloGridOpcional != null ? tituloGridOpcional : Localization.Resources.Consultas.NaturezaOperacao.NaturezasDeOperacoes, type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.NaturezaOperacao.Descricao.getFieldDescription() });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Consultas.NaturezaOperacao.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);


    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "NaturezaOperacao/Consultar", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
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

var BuscarNaturezasOperacoesNotaFiscal = function (knout, tituloOpcional, tituloGridOpcional, callbackRetorno, knoutPessoa, knoutEmpresa, knoutIndicadorPresenca, knoutDentroForaEstado, knoutTipoEntradaSaida, knoutLocalidadeTerminoPrestacao, permitirAdicionarNaturezaOperacao) {

    var idDiv = guid();
    var GridConsulta;

    if (permitirAdicionarNaturezaOperacao == null)
        permitirAdicionarNaturezaOperacao = false;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: tituloOpcional != null ? tituloOpcional : Localization.Resources.Consultas.NaturezaOperacao.ConsultaDeNaturezasDeOperacoes, type: types.local });
        this.TituloGrid = PropertyEntity({ text: tituloGridOpcional != null ? tituloGridOpcional : Localization.Resources.Consultas.NaturezaOperacao.NaturezasDeOperacoes, type: types.local });

        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.NaturezaOperacao.Descricao.getFieldDescription() });

        this.Status = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.NaturezaOperacao.Status.getFieldDescription(), visible: false, val: ko.observable("A"), def: ko.observable("A") });
        this.Pessoa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.NaturezaOperacao.Pessoas.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });
        this.IndicadorPresenca = PropertyEntity({ type: types.entity, codEntity: ko.observable(9), idBtnSearch: guid(), visible: false });
        this.DentroForaEstado = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.NaturezaOperacao.DentroFora.getFieldDescription(), visible: false, val: ko.observable(""), def: ko.observable("") });
        this.Tipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(-1), val: ko.observable("-1"), def: ko.observable("-1"), idBtnSearch: guid(), visible: false });
        this.LocalidadeTerminoPrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });

        this.AdicionarProduto = PropertyEntity({ type: types.event, text: Localization.Resources.Consultas.NaturezaOperacao.CadastrarNaturezaOperacao, visible: permitirAdicionarNaturezaOperacao, cssClass: "btn btn-default", icon: " " });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Consultas.NaturezaOperacao.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutPessoa != null || knoutEmpresa != null || knoutIndicadorPresenca != null || knoutDentroForaEstado != null || knoutTipoEntradaSaida != null || knoutLocalidadeTerminoPrestacao != null) {
        knoutOpcoes.Pessoa.visible = knoutPessoa == null;
        funcaoParamentroDinamico = function () {
            if (knoutPessoa != null) {
                knoutOpcoes.Pessoa.codEntity(knoutPessoa.codEntity());
                knoutOpcoes.Pessoa.val(knoutPessoa.val());
            }
            if (knoutEmpresa != null) {
                knoutOpcoes.Empresa.codEntity(knoutEmpresa.codEntity());
                knoutOpcoes.Empresa.val(knoutEmpresa.val())
            }
            if (knoutIndicadorPresenca != null) {
                knoutOpcoes.IndicadorPresenca.codEntity(knoutIndicadorPresenca.val());
                knoutOpcoes.IndicadorPresenca.val(knoutIndicadorPresenca.val());
            }
            if (knoutDentroForaEstado != null) {
                knoutOpcoes.DentroForaEstado.val(knoutDentroForaEstado)
            }
            if (knoutTipoEntradaSaida != null) {
                knoutOpcoes.Tipo.codEntity(knoutTipoEntradaSaida.val());
                knoutOpcoes.Tipo.val(knoutTipoEntradaSaida.val());
            }
            if (knoutLocalidadeTerminoPrestacao != null) {
                knoutOpcoes.LocalidadeTerminoPrestacao.codEntity(knoutLocalidadeTerminoPrestacao.codEntity());
                knoutOpcoes.LocalidadeTerminoPrestacao.val(knoutLocalidadeTerminoPrestacao.val());
            }
        }
    }

    if (permitirAdicionarNaturezaOperacao) {

        var fnAbrirModalAdicionarNaturezaOperacao = function (e, knoutCadastro, modalCadastro, produto) {

            LimparCampos(produto);

            Global.abrirModal(modalCadastro);
        };

        var knoutCadastro = idDiv + "knockoutCadastroNaturezaOperacao";
        var modalCadastro = idDiv + "divModalCadastrarNaturezaDaOperacao";
        $.get("Content/Static/Consultas/NaturezaOperacao/NaturezaOperacao.html?dyn=" + guid(), function (data) {
            var html = data.replace(/#knockoutCadastroNaturezaOperacao/g, knoutCadastro).replace(/#divModalCadastrarNaturezaDaOperacao/g, modalCadastro);
            $('#js-page-content').append(html);
            var produto = new knockoutCadastroNaturezaOperacao();

            produto.Cancelar.eventClick = function (e) {
                Global.fecharModal(modalCadastro);
            };

            produto.Adicionar.eventClick = function (e) {
                if (produto.Descricao.val() == "") {
                    valido = false;
                    exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe a descrição da Natureza da Operação!");
                    produto.Descricao.requiredClass("form-control is-invalid");
                    return;
                }

                Salvar(e, "NaturezaDaOperacao/AdicionarNaturezaOperacaoSimplificada", function (arg) {
                    if (arg.Success) {
                        if (arg.Data != false) {
                            Global.fecharModal(modalCadastro);
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                        } else {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 60000);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                    }
                });
            };

            KoBindings(produto, knoutCadastro, false);

            knoutOpcoes.AdicionarProduto.eventClick = function (e) {
                fnAbrirModalAdicionarNaturezaOperacao(e, knoutCadastro, modalCadastro, produto);
            };
        });
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "NaturezaDaOperacao/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
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

var _statusPesquisaNaturezaOperacao = [
    { text: "Todos", value: "" },
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

var knockoutCadastroNaturezaOperacao = function () {

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 60 });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumEntradaSaida.Todos), options: EnumEntradaSaida.obterOpcoesPesquisa(), def: EnumEntradaSaida.Todos, text: "*Tipo: ", required: true });
    this.Status = PropertyEntity({ val: ko.observable("A"), options: _statusPesquisaNaturezaOperacao, def: "A", text: "*Status: ", required: true });
    this.DentroEstado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Dentro do Estado", def: false, visible: ko.observable(true) });
    this.GeraTitulo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Gera Título", def: false });
    this.ControlaEstoque = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Controla Estoque", def: false });
    this.Garantia = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Garantia", def: false });
    this.Demonstracao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Demonstração", def: false });
    this.Bonificacao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Bonificação", def: false });
    this.Outras = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Outras", def: false });
    this.DesconsideraICMSEfetivo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Desconsidera ICMS Efetivo", def: false });
    this.Devolucao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Devolução", def: false });

    this.Adicionar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });

    var $this = this;
};