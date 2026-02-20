/// <reference path="../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../Enumeradores/EnumTipoPessoa.js" />
/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="GrupoPessoa.js" />
/// <reference path="../Configuracao/Sistema/ConfiguracaoTMS.js" />

var KnoutCadastrarCliente = function () {

    this.Nome = PropertyEntity({ text: ko.observable(Localization.Resources.Consultas.Cliente.RazaoSocial.getRequiredFieldDescription()), required: true });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoa.Juridica), options: EnumTipoPessoa.obterOpcoes(), text: Localization.Resources.Gerais.Geral.Tipo.getRequiredFieldDescription(), def: EnumTipoPessoa.Juridica });
    this.CPF_CNPJ = PropertyEntity({ text: ko.observable(Localization.Resources.Consultas.Cliente.CNPJ.getRequiredFieldDescription()), getType: typesKnockout.cnpj, required: true });

    this.Atividade = PropertyEntity({ val: ko.observable(2), options: EnumAtividade.obterOpcoes(), text: Localization.Resources.Consultas.Cliente.Atividade.getRequiredFieldDescription(), def: 2 });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Gerais.Geral.Cidade.getRequiredFieldDescription(), idBtnSearch: guid() });

    this.TelefonePrincipal = PropertyEntity({ text: Localization.Resources.Consultas.Cliente.TelefonePrincipal.getFieldDescription(), issue: 27, required: false, getType: typesKnockout.phone });
    this.TelefoneSecundario = PropertyEntity({ text: Localization.Resources.Consultas.Cliente.TelefoneSecundario.getFieldDescription(), issue: 27, required: false, getType: typesKnockout.phone });

    this.TipoLogradouro = PropertyEntity({ val: ko.observable(EnumTipoLogradouro.Rua), options: EnumTipoLogradouro.obterOpcoes(), def: EnumTipoLogradouro.Rua, text: Localization.Resources.Consultas.Cliente.TipoLogradouro.getFieldDescription(), issue: 18 });
    this.Endereco = PropertyEntity({ text: ko.observable(Localization.Resources.Consultas.Cliente.EnderecoPrincipal.getFieldDescription()), required: false, maxlength: 80, enable: ko.observable(true) });
    this.EnderecoDigitado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Consultas.Cliente.DigitarEndereco, def: ko.observable(false) });

    this.SN_Numero = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Consultas.Cliente.SemNumero, def: ko.observable(false) });
    this.Numero = PropertyEntity({ text: ko.observable(Localization.Resources.Consultas.Cliente.Numero.getFieldDescription()), val: ko.observable("S/N"), def: "S/N", maxlength: 60, enable: ko.observable(true) });

    this.Bairro = PropertyEntity({ text: ko.observable(Localization.Resources.Consultas.Cliente.Bairro.getFieldDescription()), maxlength: 40, enable: ko.observable(true) });
    this.Complemento = PropertyEntity({ text: ko.observable(Localization.Resources.Consultas.Cliente.Complemento.getFieldDescription()), maxlength: 60 });
    this.CEP = PropertyEntity({ text: ko.observable(Localization.Resources.Consultas.Cliente.CEP.getFieldDescription()), getType: typesKnockout.cep, fnFocusOut: function () { } });
    this.Email = PropertyEntity({ text: ko.observable(Localization.Resources.Consultas.Cliente.EmailPrincipal.getFieldDescription()), issue: 30, getType: typesKnockout.email, maxlength: 1000 });
    this.EnviarEmail = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: " XML" });
    this.PossuiRestricaoTrafego = PropertyEntity({ text: Localization.Resources.Consultas.Cliente.PossuiRestricaoTrafego, issue: 1458, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });

    this.ConsultarCEP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Gerais.Geral.Buscar), idBtnSearch: guid(), enable: ko.observable(true) });

    this.Buscar = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.Buscar) });
    this.CadastrarCliente = PropertyEntity({ text: ko.observable(Localization.Resources.Consultas.Cliente.CadastrarCliente) });
    this.OsDemaisDadosDoClienteSeraoComplementadosAutomaticamenteQuandoEleForLocalizadoEmUmaNFe = PropertyEntity({ text: ko.observable(Localization.Resources.Consultas.Cliente.OsDemaisDadosDoClienteSeraoComplementadosAutomaticamenteQuandoEleForLocalizadoEmUmaNFe) });

    this.NomeSocio = PropertyEntity({ text: Localization.Resources.Consultas.Cliente.NomeSocio.getFieldDescription(), maxlength: 100, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) });
    this.CPFSocio = PropertyEntity({ text: Localization.Resources.Consultas.Cliente.CPFSocio.getFieldDescription(), getType: typesKnockout.cpf, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) });
    this.DataNascimento = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.date, text: Localization.Resources.Consultas.Cliente.DataNascimento.getFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.GrupoPessoas.getFieldDescription(), issue: 58, idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) });

    this.Adicionar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });

    var $this = this;

    this.TipoPessoa.val.subscribe(function () {
        $("#" + $this.CPF_CNPJ.id).unmask();
        $this.CPF_CNPJ.val("");

        if ($this.TipoPessoa.val() === EnumTipoPessoa.Fisica) {
            $this.CPF_CNPJ.getType = typesKnockout.cpf;
            $this.CPF_CNPJ.text(Localization.Resources.Consultas.Cliente.CPF.getRequiredFieldDescription());
            $this.Nome.text(Localization.Resources.Consultas.Cliente.Nome.getRequiredFieldDescription());
            $("#" + $this.CPF_CNPJ.id).mask("000.000.000-00", { selectOnFocus: true, clearIfNotMatch: true });
        } else {
            $this.CPF_CNPJ.getType = typesKnockout.cnpj;
            $this.CPF_CNPJ.text(Localization.Resources.Consultas.Cliente.CNPJ.getRequiredFieldDescription());
            $this.CPF_CNPJ.val("");
            $this.Nome.text(Localization.Resources.Consultas.Cliente.RazaoSocial.getRequiredFieldDescription());
            $("#" + $this.CPF_CNPJ.id).mask("00.000.000/0000-00", { selectOnFocus: true, clearIfNotMatch: true });
        }
    });
};

var BuscarClientesFactory = function (knout, callbackRetorno, opcoes) {
    var _opcoes = $.extend({}, {
        permitirAddCliente: false,
        modalidades: null,
        knoutGrupoPessoas: null,
        basicGrid: null,
        knoutRaiz: null,
        knoutCodigoGrupoPessoaRaizCNPJ: null,
        knoutBaixaTituloPagar: null,
        tipoPessoa: null,
        afterDefaultCallback: null,
        knoutListaCNPJConsultar: null,
        knoutTipoCombustivel: null,
        knoutAtivo: null,
        filtrarGrupoFornecedor: false
    }, opcoes);

    return new BuscarClientes(
        knout,
        callbackRetorno,
        _opcoes.permitirAddCliente,
        _opcoes.modalidades,
        _opcoes.knoutGrupoPessoas,
        _opcoes.basicGrid,
        _opcoes.knoutRaiz,
        _opcoes.knoutCodigoGrupoPessoaRaizCNPJ,
        _opcoes.knoutBaixaTituloPagar,
        _opcoes.tipoPessoa,
        _opcoes.afterDefaultCallback,
        _opcoes.knoutListaCNPJConsultar,
        _opcoes.knoutTipoCombustivel,
        _opcoes.knoutAtivo,
        _opcoes.filtrarGrupoFornecedor
    );
}

var BuscarClientes = function (knout, callbackRetorno, permitirAddCliente, modalidades, knoutGrupoPessoas, basicGrid, knoutRaiz, knoutCodigoGrupoPessoaRaizCNPJ, knoutBaixaTituloPagar, tipoPessoa, afterDefaultCallback, knoutListaCNPJConsultar, knoutTipoCombustivel, knoutAtivo, filtrarGrupoFornecedor, somenteFilial, somenteFronteira, somenteAreaRedex, somenteArmador, somenteSupridores, filtrarPorGrupoPessoa, filtrarPorConfiguracaoOperadorLogistica, apenasVinculadosACentroDescarregamento) {

    let self = this;

    let idDiv = guid();
    let GridConsulta;
    let buscaGruposPessoas;
    let buscaLocalidades;

    self._modalWindowCadastroCliente = null;

    let multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    if (permitirAddCliente == null)
        permitirAddCliente = false;

    if (somenteFronteira == null)
        somenteFronteira = false;

    let tipoPessoaPadrao = tipoPessoa != null ? tipoPessoa : "";

    let OpcoesKnout = function () {

        this.Titulo = PropertyEntity({ text: (somenteFronteira ? Localization.Resources.Consultas.Cliente.PesquisaFronteira : Localization.Resources.Consultas.Cliente.PesquisarPessoas), type: types.local });
        this.TituloGrid = PropertyEntity({ text: (somenteFronteira ? Localization.Resources.Consultas.Cliente.Fronteiras : Localization.Resources.Consultas.Cliente.Pessoas), type: types.local });

        this.Nome = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.Cliente.RazaoSocial.getFieldDescription() });
        this.CPF_CNPJ = PropertyEntity({ col: 3, text: Localization.Resources.Consultas.Cliente.CPFCNPJ.getFieldDescription(), getType: typesKnockout.cpfCnpj });
        this.RaizCNPJ = PropertyEntity({ col: 3, text: Localization.Resources.Consultas.Cliente.RaizCNPJ.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 8, visible: !IsMobile() });
        this.NomeFantasia = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.Cliente.NomeFantasia.getFieldDescription() });
        this.Localidade = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Cidade.getFieldDescription(), idBtnSearch: guid() });
        this.GrupoPessoas = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.GrupoPessoas.getFieldDescription(), idBtnSearch: guid(), visible: !IsMobile() });
        this.CodigoIntegracao = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.Cliente.CodigoIntegracao.getFieldDescription() });

        this.CodigoGrupoPessoaRaizCNPJ = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Cliente.GrupoDePessoas.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.BaixaTituloPagar = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Cliente.BaixarTituloPagar.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.Modalidades = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(JSON.stringify(modalidades)), idGrid: guid(), visible: false });
        this.TipoPessoa = PropertyEntity({ val: ko.observable(tipoPessoaPadrao), options: EnumTipoPessoa.obterOpcoesPesquisaTexto(), text: Localization.Resources.Gerais.Geral.Tipo.getFieldDescription(), def: tipoPessoaPadrao, col: 3, visible: (tipoPessoa == null) });
        this.FiltrarGrupoFornecedor = PropertyEntity({ val: ko.observable(filtrarGrupoFornecedor), visible: false });
        this.ListaCNPJConsultar = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid(), visible: false });
        this.TipoAbastecimento = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Cliente.TipoDoCombustivel.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.Ativo = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(1), text: Localization.Resources.Gerais.Geral.Ativo.getFieldDescription(), idBtnSearch: guid(), visible: false, val: ko.observable(1) });
        this.SomenteFilial = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), visible: false });
        this.SomenteFronteira = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), visible: false });
        this.PossuiAreaRedex = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), visible: false });
        this.PossuiArmador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), visible: false });
        this.SomenteSupridores = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), visible: false });
        this.GeoLocalizacaoTipo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(-1), visible: false });
        this.ApenasVinculadosACentroDescarregamento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), visible: false });

        this.FiltrarPorConfiguracaoOperadorLogistica = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(filtrarPorConfiguracaoOperadorLogistica), visible: false });

        this.AdicionarCliente = PropertyEntity({ type: types.event, text: Localization.Resources.Consultas.Cliente.AdicionarNovaPessoa, visible: permitirAddCliente, cssClass: "btn btn-default", icon: "fal fa-plus" });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    let knoutOpcoes = new OpcoesKnout();

    var fnPreecherRetornoSelecao = function (knout, e, idDiv, knoutOpcoes) {
        knout.codEntity(e.Codigo);
        knout.val(e.Descricao);
        knoutOpcoes.Nome.val(knoutOpcoes.Nome.def);
        knoutOpcoes.CPF_CNPJ.val(knoutOpcoes.CPF_CNPJ.def);
        divBusca.CloseModal();
    };

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        knoutOpcoes.GrupoPessoas.visible = false;
    }
    if (filtrarPorGrupoPessoa === true)
        knoutOpcoes.GrupoPessoas.visible = true;

    var funcaoParametroDinamico = function () {
        if (knoutAtivo != null && knoutAtivo != undefined) {
            knoutOpcoes.Ativo.val(knoutAtivo);
            knoutOpcoes.Ativo.codEntity(knoutAtivo);
        }

        if (knoutRaiz != null)
            knoutOpcoes.RaizCNPJ.val(knoutRaiz.val());

        if (somenteFilial)
            knoutOpcoes.SomenteFilial.val(true);

        if (somenteFronteira)
            knoutOpcoes.SomenteFronteira.val(true);

        if (somenteAreaRedex)
            knoutOpcoes.PossuiAreaRedex.val(true);

        if (somenteArmador)
            knoutOpcoes.PossuiArmador.val(true);

        if (somenteSupridores)
            knoutOpcoes.SomenteSupridores.val(true);

        if (knoutGrupoPessoas != null) {
            knoutOpcoes.GrupoPessoas.codEntity(knoutGrupoPessoas.codEntity());
            knoutOpcoes.GrupoPessoas.val(knoutGrupoPessoas.val());
        }

        if (knoutCodigoGrupoPessoaRaizCNPJ != null) {
            knoutOpcoes.CodigoGrupoPessoaRaizCNPJ.codEntity(knoutCodigoGrupoPessoaRaizCNPJ.val());
            knoutOpcoes.CodigoGrupoPessoaRaizCNPJ.val(knoutCodigoGrupoPessoaRaizCNPJ.val());
        }

        if (knoutBaixaTituloPagar != null) {
            knoutOpcoes.BaixaTituloPagar.codEntity(knoutBaixaTituloPagar.Codigo.val());
            knoutOpcoes.BaixaTituloPagar.val(knoutBaixaTituloPagar.Codigo.val());
        }

        if (knoutListaCNPJConsultar != null)
            knoutOpcoes.ListaCNPJConsultar.list = knoutListaCNPJConsultar.list;

        if (knoutTipoCombustivel != null) {
            knoutOpcoes.TipoAbastecimento.codEntity(knoutTipoCombustivel.val());
            knoutOpcoes.TipoAbastecimento.val(knoutTipoCombustivel.val());
        }

        if (apenasVinculadosACentroDescarregamento)
            knoutOpcoes.ApenasVinculadosACentroDescarregamento.val(true);
    }

    if (permitirAddCliente) {

        self.fnAbrirModalAddClienteCliente = (e, knoutCadastro, modalCadastro, cliente) => {

            LimparCampos(cliente);

            cliente.Nome.val(e.Nome.val());
            cliente.Localidade.codEntity(0);
            cliente.Localidade.val(cliente.Localidade.def);
            cliente.CPF_CNPJ.val(e.CPF_CNPJ.val());

            if (e.CPF_CNPJ.val().length == 14) {
                $("#" + cliente.CPF_CNPJ.id).unmask();
                cliente.TipoPessoa.val(EnumTipoPessoa.Juridica);
                cliente.Nome.text(Localization.Resources.Consultas.Cliente.RazaoSocial.getRequiredFieldDescription());
                cliente.CPF_CNPJ.text(Localization.Resources.Consultas.Cliente.CNPJ.getRequiredFieldDescription());
                cliente.CPF_CNPJ.getType = typesKnockout.cnpj;
                $("#" + cliente.CPF_CNPJ.id).mask("00.000.000/0000-00", { selectOnFocus: true, clearIfNotMatch: true });
            } else if (e.CPF_CNPJ.val().length == 11) {
                $("#" + cliente.CPF_CNPJ.id).unmask();
                cliente.TipoPessoa.val(EnumTipoPessoa.Fisica);
                cliente.CPF_CNPJ.text(Localization.Resources.Consultas.Cliente.CPF.getRequiredFieldDescription());
                cliente.Nome.text(Localization.Resources.Consultas.Cliente.Nome.getRequiredFieldDescription());
                cliente.CPF_CNPJ.getType = typesKnockout.cpf;
                $("#" + cliente.CPF_CNPJ.id).mask("000.000.000-00", { selectOnFocus: true, clearIfNotMatch: true });
            }

            self._modalWindowCadastroCliente.show();
        };

        let fnRetornoEndereco = function (enderecoSelecionado, cliente) {
            if (enderecoSelecionado != null) {
                cliente.CEP.val(enderecoSelecionado.CEP);
                cliente.Bairro.val(enderecoSelecionado.Bairro);
                cliente.Endereco.val(enderecoSelecionado.Logradouro);
                cliente.Localidade.codEntity(enderecoSelecionado.CodigoCidade);
                cliente.Localidade.val(enderecoSelecionado.DescricaoCidadeEstado);

                if (enderecoSelecionado.TipoLogradouro != null && enderecoSelecionado.TipoLogradouro != "") {
                    if (enderecoSelecionado.TipoLogradouro == "Rua")
                        cliente.TipoLogradouro.val(1);
                    else if (enderecoSelecionado.TipoLogradouro == "Avenida")
                        cliente.TipoLogradouro.val(2);
                    else if (enderecoSelecionado.TipoLogradouro == "Rodovia")
                        cliente.TipoLogradouro.val(3);
                    else if (enderecoSelecionado.TipoLogradouro == "Estrada")
                        cliente.TipoLogradouro.val(4);
                    else if (enderecoSelecionado.TipoLogradouro == "Praca")
                        cliente.TipoLogradouro.val(5);
                    else if (enderecoSelecionado.TipoLogradouro == "Praça")
                        cliente.TipoLogradouro.val(5);
                    else if (enderecoSelecionado.TipoLogradouro == "Travessa")
                        cliente.TipoLogradouro.val(6);
                    else
                        cliente.TipoLogradouro.val(99);
                }

                cliente.EnderecoDigitado.val(false);
                cliente.SN_Numero.val(true);
            }
        };

        let fnBuscarPorCEP = function (cliente) {
            let cep = $("#" + cliente.CEP.id).val();

            if (cep.match(/\d/g).join("").length == 8) {
                let data = { CEP: cep };
                executarReST("Localidade/BuscarEnderecoPorCEP", data, function (arg) {
                    if (arg.Success) {
                        if (arg.Data != null && arg.Data.DescricaoCidadeEstado != null && arg.Data.DescricaoCidadeEstado != "" && arg.Data.CodigoCidade > 0) {
                            fnRetornoEndereco(arg.Data, cliente);
                        } else {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Consultas.Cliente.ConsultaCEP, Localization.Resources.Consultas.Cliente.CEPInformadoNaoExisteBaseDados);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                    }
                });
            }
        };

        let knoutCadastro = idDiv + "_knockoutCadastroCliente";
        let modalCadastro = idDiv + "divModalCadastrarCliente";

        $.get("Content/Static/Consultas/Cadastros/Cliente.html?dyn=" + guid(), function (data) {

            let html = data.replace(/#knockoutCadastroCliente/g, knoutCadastro).replace(/#divModalCadastrarCliente/g, modalCadastro);

            $('body #js-page-content').append(html);

            self._modalWindowCadastroCliente = new bootstrap.Modal(document.getElementById(modalCadastro), { keyboard: true, backdrop: 'static' });

            let cliente = new KnoutCadastrarCliente();

            cliente.Cancelar.eventClick = function (e) {
                self._modalWindowCadastroCliente.hide();
            };

            cliente.Adicionar.eventClick = function (e) {
                Salvar(e, "Cliente/Adicionar", function (arg) {
                    if (arg.Success) {
                        if (arg.Data != false) {
                            self._modalWindowCadastroCliente.hide();
                            fnPreecherRetornoSelecao(knout, arg.Data, idDiv, knoutOpcoes);
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                        } else {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 60000);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                    }
                });
            };

            cliente.CEP.fnFocusOut = function (cliente) {
                fnBuscarPorCEP(cliente);
            };

            KoBindings(cliente, knoutCadastro, false);

            new BuscarLocalidades(cliente.Localidade);
            new BuscarEnderecos(cliente.ConsultarCEP, null, null, fnRetornoEndereco);
            new BuscarGruposPessoas(cliente.GrupoPessoas);

            knoutOpcoes.AdicionarCliente.eventClick = function (e) {
                self.fnAbrirModalAddClienteCliente(e, knoutCadastro, modalCadastro, cliente);
            };
        });
    }

    if (knoutGrupoPessoas != null)
        knoutOpcoes.GrupoPessoas.visible = false;

    let divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha, function () {
        buscaGruposPessoas = new BuscarGruposPessoas(knoutOpcoes.GrupoPessoas, null, false, null, EnumTipoGrupoPessoas.Ambos);
        buscaLocalidades = new BuscarLocalidades(knoutOpcoes.Localidade);
    });

    let callback = function (e) {
        fnPreecherRetornoSelecao(knout, e, idDiv, knoutOpcoes);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }
    else if (afterDefaultCallback != null) {
        callback = function (e) {
            fnPreecherRetornoSelecao(knout, e, idDiv, knoutOpcoes);
            afterDefaultCallback(e);
        };
    }

    var ordenacaoPadrao = { column: 2, dir: orderDir.asc };
    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, afterDefaultCallback: afterDefaultCallback, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Cliente/Pesquisa", knoutOpcoes, null, ordenacaoPadrao, null, null, null, null, objetoBasicGrid);
    }
    else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Cliente/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback, 22), ordenacaoPadrao);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Nome.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
                Global.setarFocoProximoCampo(knout.id);
            } else {
                divBusca.OpenModal();
            }
        });
    });

    this.Destroy = function () {
        if (buscaGruposPessoas)
            buscaGruposPessoas.Destroy();

        if (buscaLocalidades)
            buscaLocalidades.Destroy();

        divBusca.Destroy();
    };
};

var BuscarClientesCarga = function (knout, callbackRetorno, knoutCarga, cargaOrigem) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Cliente.PesquisarPessoas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Cliente.Pessoas, type: types.local });
        this.Nome = PropertyEntity({ col: 8, text: Localization.Resources.Consultas.Cliente.RazaoSocial.getFieldDescription() });
        this.CPF_CNPJ = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Cliente.CPFCNPJ.getFieldDescription(), getType: typesKnockout.cpfCnpj, maxlength: 18 });
        this.CargaOrigem = PropertyEntity({ visible: false, val: ko.observable(cargaOrigem) });
        this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Consultas.Cliente.Carga.getFieldDescription(), idBtnSearch: guid() });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutCarga != null) {
        knoutOpcoes.Carga.visible = false;
        funcaoParametroDinamico = function () {
            knoutOpcoes.Carga.codEntity(knoutCarga.codEntity());
            knoutOpcoes.Carga.val(knoutCarga.val());
            knoutOpcoes.CargaOrigem.val(cargaOrigem);
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico);
    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Descricao);
        divBusca.CloseModal();
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Cliente/PesquisaCarga", knoutOpcoes, divBusca.OpcaoPadrao(callback), null, null, null, null, null, null, 10);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Nome.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {

            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });

    this.CarregarClienteCargaSelecionada = function () {
        if (knoutCarga != null) {
            LimparCampos(knoutOpcoes);
            funcaoParametroDinamico();

            GridConsulta.CarregarGrid(function (lista) {
                if (lista.data.length == 1)
                    callback(lista.data[0]);
                else
                    LimparCampo(knout);
            });
        }
    };

    this.Destroy = function () {
        divBusca.Destroy();
    };
};

var BuscarTomadoresCarga = function (knout, callbackRetorno, knoutCarga) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Cliente.PesquisarPessoas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Cliente.Pessoas, type: types.local });
        this.Nome = PropertyEntity({ col: 8, text: Localization.Resources.Consultas.Cliente.RazaoSocial.getFieldDescription() });
        this.CPF_CNPJ = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Cliente.CPFCNPJ.getFieldDescription(), getType: typesKnockout.cpfCnpj, maxlength: 14 });
        this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Consultas.Cliente.Carga.getFieldDescription(), idBtnSearch: guid() });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutCarga != null) {
        knoutOpcoes.Carga.visible = false;
        funcaoParametroDinamico = function () {
            knoutOpcoes.Carga.codEntity(knoutCarga.codEntity());
            knoutOpcoes.Carga.val(knoutCarga.val());
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico);
    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Descricao);
        divBusca.CloseModal();
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Cliente/PesquisaTomadorCarga", knoutOpcoes, divBusca.OpcaoPadrao(callback), null, null, null, null, null, null, 10);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Nome.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {

            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });

    this.CarregarTomadorCargaSelecionada = function () {
        if (knoutCarga != null) {
            LimparCampos(knoutOpcoes);
            funcaoParametroDinamico();

            GridConsulta.CarregarGrid(function (lista) {
                if (lista.data.length == 1)
                    callback(lista.data[0]);
                else
                    LimparCampo(knout);
            });
        }
    };

    this.Destroy = function () {
        divBusca.Destroy();
    };
};

var BuscarDestinatariosCarga = function (knout, callbackRetorno, knoutCarga) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Cliente.PesquisarPessoas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Cliente.Pessoas, type: types.local });
        this.Nome = PropertyEntity({ col: 8, text: Localization.Resources.Consultas.Cliente.RazaoSocial.getFieldDescription() });
        this.CPF_CNPJ = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Cliente.CPFCNPJ.getFieldDescription(), getType: typesKnockout.cpfCnpj, maxlength: 18 });
        this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Consultas.Cliente.Carga.getFieldDescription(), idBtnSearch: guid() });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutCarga != null) {
        knoutOpcoes.Carga.visible = false;
        funcaoParametroDinamico = function () {
            knoutOpcoes.Carga.codEntity(knoutCarga.codEntity());
            knoutOpcoes.Carga.val(knoutCarga.val());
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico);
    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Descricao);
        divBusca.CloseModal();
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Cliente/PesquisaDestinatarioCarga", knoutOpcoes, divBusca.OpcaoPadrao(callback), null, null, null, null, null, null, 10);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Nome.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {

            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });

    this.CarregarDestinatarioCargaSelecionada = function (entregaSelecionada) {
        if (knoutCarga != null) {
            LimparCampos(knoutOpcoes);
            funcaoParametroDinamico();

            if (entregaSelecionada)
                knoutOpcoes.CPF_CNPJ.val(entregaSelecionada);

            GridConsulta.CarregarGrid(function (lista) {
                if (lista.data.length == 1)
                    callback(lista.data[0]);
                else
                    LimparCampo(knout);
            });
        }
    };

    this.Destroy = function () {
        divBusca.Destroy();
    };
};

var BuscarRemetentesDestinatariosCarga = function (knout, callbackRetorno, knoutCarga, cargaOrigem) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Cliente.PesquisarPessoas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Cliente.Pessoas, type: types.local });
        this.Nome = PropertyEntity({ col: 8, text: Localization.Resources.Consultas.Cliente.RazaoSocial.getFieldDescription() });
        this.CPF_CNPJ = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Cliente.CPFCNPJ.getFieldDescription(), getType: typesKnockout.cpfCnpj, maxlength: 14 });
        this.CargaOrigem = PropertyEntity({ visible: false, val: ko.observable(cargaOrigem) });
        this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Consultas.Cliente.Carga.getFieldDescription(), idBtnSearch: guid() });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutCarga != null) {
        knoutOpcoes.Carga.visible = false;
        funcaoParametroDinamico = function () {
            knoutOpcoes.Carga.codEntity(knoutCarga.codEntity());
            knoutOpcoes.Carga.val(knoutCarga.val());
            knoutOpcoes.CargaOrigem.val(cargaOrigem);
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico);
    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Descricao);
        divBusca.CloseModal();
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Cliente/PesquisaRemetenteDestinatarioCarga", knoutOpcoes, divBusca.OpcaoPadrao(callback), null, null, null, null, null, null, 10);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Nome.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {

            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};