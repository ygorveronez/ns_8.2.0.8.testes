//*******MAPEAMENTO KNOUCKOUT*******

var _gridBordero, _bordero, _CRUDBordero, _pesquisaBordero, _liquidacaoBordero, _contatoClienteBordero;

var _situacaoPesquisaBordero = [
    { text: "Todas", value: "" },
    { text: "Em Andamento", value: EnumSituacaoBordero.EmAndamento },
    { text: "Finalizado", value: EnumSituacaoBordero.Finalizado },
    { text: "Liquidado", value: EnumSituacaoBordero.Quitado },
    { text: "Cancelado", value: EnumSituacaoBordero.Cancelado }
];

var _tipoPessoa = [
    { text: "Pessoa (CNPJ/CPF)", value: 1 },
    { text: "Grupo de Pessoas", value: 2 }
];

var PesquisaBordero = function () {
    this.Numero = PropertyEntity({ text: "Nº Borderô:", val: ko.observable(""), def: "", maxlength: 15, getType: typesKnockout.int });
    this.Carga = PropertyEntity({ text: "Nº Carga:", maxlength: 15 });
    this.CTe = PropertyEntity({ text: "Nº CT-e:", val: ko.observable(""), def: "", maxlength: 15, getType: typesKnockout.int });
    this.Titulo = PropertyEntity({ text: "Nº Título:", val: ko.observable(""), def: "", maxlength: 15, getType: typesKnockout.int });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataVencimentoInicial = PropertyEntity({ text: "Data Vencto. Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataVencimentoFinal = PropertyEntity({ text: "Data Vencto. Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.ValorACobrarInicial = PropertyEntity({ text: "Valor a Cobrar Inicial:", getType: typesKnockout.decimal, val: ko.observable(""), def: "" });
    this.ValorACobrarFinal = PropertyEntity({ text: "Valor a Cobrar Final:", getType: typesKnockout.decimal, val: ko.observable(""), def: "" });
    this.ValorTotalACobrarInicial = PropertyEntity({ text: "Total a Cobrar Inicial:", getType: typesKnockout.decimal, val: ko.observable(""), def: "" });
    this.ValorTotalACobrarFinal = PropertyEntity({ text: "Total a Cobrar Final:", getType: typesKnockout.decimal, val: ko.observable(""), def: "" });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoPesquisaBordero, def: "", text: "Situação:" });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(1), options: _tipoPessoa, def: "", text: "Tipo de Pessoa:" });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;
    this.DataVencimentoInicial.dateRangeLimit = this.DataVencimentoFinal;
    this.DataVencimentoFinal.dateRangeInit = this.DataVencimentoInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridBordero.CarregarGrid();
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

    this.TipoPessoa.val.subscribe(function (novoValor) {
        if (novoValor == 1) {
            _pesquisaBordero.Pessoa.visible(true);
            _pesquisaBordero.GrupoPessoas.visible(false);
            _pesquisaBordero.GrupoPessoas.codEntity(0);
            _pesquisaBordero.GrupoPessoas.val('');
        } else {
            _pesquisaBordero.GrupoPessoas.visible(true);
            _pesquisaBordero.Pessoa.visible(false);
            _pesquisaBordero.Pessoa.codEntity(0);
            _pesquisaBordero.Pessoa.val('');
        }
    });
}

var Bordero = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataEmissao = PropertyEntity({ text: "*Data de Emissão:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), required: true, enable: ko.observable(true) });
    this.DataVencimento = PropertyEntity({ text: "*Data de Vencimento:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), required: true, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 1000, enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoBordero.EmAndamento), def: EnumSituacaoBordero.EmAndamento, getType: typesKnockout.int });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(1), options: _tipoPessoa, def: "", text: "*Tipo de Pessoa:", enable: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: true });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true), required: false });

    this.ImprimirObservacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: "Imprimir Observação?", enable: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Banco:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Agencia = PropertyEntity({ text: "Agência:", maxlength: 10, enable: ko.observable(true) });
    this.DigitoAgencia = PropertyEntity({ text: "Dígito:", maxlength: 1, enable: ko.observable(true) });
    this.NumeroConta = PropertyEntity({ text: "Conta:", maxlength: 10, enable: ko.observable(true) });
    this.TipoConta = PropertyEntity({ val: ko.observable(EnumTipoConta.Corrente), options: EnumTipoConta.obterOpcoesBordero(), def: EnumTipoConta.Corrente, text: "Tipo de Conta:", enable: ko.observable(true) });

    this.TipoPessoa.val.subscribe(function (novoValor) {
        if (novoValor == 1) {
            _bordero.Pessoa.visible(true);
            _bordero.Pessoa.required = true;
            _bordero.GrupoPessoas.required = false;
            _bordero.GrupoPessoas.visible(false);
            _bordero.GrupoPessoas.codEntity(0);
            _bordero.GrupoPessoas.val('');
        } else {
            _bordero.GrupoPessoas.visible(true);
            _bordero.GrupoPessoas.required = true;
            _bordero.Pessoa.required = false;
            _bordero.Pessoa.visible(false);
            _bordero.Pessoa.codEntity(0);
            _bordero.Pessoa.val('');
        }
    });

    this.Salvar = PropertyEntity({ eventClick: SalvarClick, type: types.event, text: ko.observable("Gerar Borderô"), icon: "fa fa-save", visible: ko.observable(true), enable: ko.observable(true) });
}

var LiquidacaoBordero = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataBase = PropertyEntity({ text: "*Data Base:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), required: true, visible: true });
    this.DataBaixa = PropertyEntity({ text: "*Data da Baixa:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), required: true, visible: true});
    this.TipoPagamento = PropertyEntity({ text: "*Tipo Pagamento:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true, visible: true });

    this.Liquidar = PropertyEntity({ eventClick: LiquidarBorderoClick, type: types.event, text: "Liquidar", icon: "fa fa-chevron-down", visible: ko.observable(true), enable: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharTelaLiquidacaoBorderoClick, type: types.event, text: "Fechar", icon: "fa fa-window-close", visible: ko.observable(true), enable: ko.observable(true) });
}

var CRUDBordero = function () {
    this.GerarNovoBordero = PropertyEntity({ eventClick: GerarNovoBorderoClick, type: types.event, text: "Limpar / Gerar Novo Borderô", visible: ko.observable(false), icon: "fa fa-history" });
    this.Finalizar = PropertyEntity({ eventClick: FinalizarBorderoClick, type: types.event, text: "Finalizar", visible: ko.observable(false), icon: "fa fa-chevron-down" });
    this.Liquidar = PropertyEntity({ eventClick: AbrirTelaLiquidacaoBorderoClick, type: types.event, text: "Liquidar", visible: ko.observable(false), icon: "fa fa-chevron-down" });
    this.Cancelar = PropertyEntity({ eventClick: CancelarBorderoClick, type: types.event, text: "Cancelar", visible: ko.observable(false), icon: "fa fa-ban" });
    this.Imprimir = PropertyEntity({ eventClick: ImprimirBorderoClick, type: types.event, text: "Imprimir", visible: ko.observable(false), icon: "fa fa-print" });
}

//*******EVENTOS*******

function LoadBordero() {

    _bordero = new Bordero();
    KoBindings(_bordero, "knockoutCadastroBordero");

    HeaderAuditoria("Bordero", _bordero);

    _CRUDBordero = new CRUDBordero();
    KoBindings(_CRUDBordero, "knockoutCRUDBordero");

    _pesquisaBordero = new PesquisaBordero();
    KoBindings(_pesquisaBordero, "knockoutPesquisaBordero", false, _pesquisaBordero.Pesquisar.id);

    _liquidacaoBordero = new LiquidacaoBordero();
    KoBindings(_liquidacaoBordero, "knockoutLiquidarBordero");

    HeaderAuditoria("Bordero", _bordero);

    new BuscarClientes(_pesquisaBordero.Pessoa);
    new BuscarGruposPessoas(_pesquisaBordero.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);

    new BuscarClientes(_bordero.Pessoa, RetornoConsultaPessoaBordero);
    new BuscarGruposPessoas(_bordero.GrupoPessoas, RetornoConsultaGrupoPessoasBordero, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarBanco(_bordero.Banco);
    new BuscarClientes(_bordero.Tomador);
    new BuscarEmpresa(_bordero.Empresa);

    new BuscarTipoPagamento(_liquidacaoBordero.TipoPagamento);

    LoadEtapaBordero();
    LoadResumoBordero();
    LoadBorderoTitulo();
    LoadBorderoTituloDocumento();
    LoadAcrescimoDescontoBorderoTituloDocumento();
    //LoadSignalRBordero();

    BuscarBorderos();

    _contatoClienteBordero = new ContatoCliente("btnContatoCliente", _bordero.Codigo, EnumTipoDocumentoContatoCliente.Bordero);
}

function SalvarClick(e, sender) {
    Salvar(_bordero, "Bordero/Salvar", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Borderô salvo com sucesso!");
                PreecherCamposEdicaoBordero(r.Data);
                _gridBordero.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function GerarNovoBorderoClick(e) {
    LimparCamposBordero();
    SetarEtapaBordero();
}

function FinalizarBorderoClick(e) {
    exibirConfirmacao("Atenção!", "Deseja realmente finalizar o borderô?", function () {
        executarReST("Bordero/Finalizar", { Codigo: _bordero.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Borderô finalizado com sucesso!");
                    PreecherCamposEdicaoBordero(r.Data);
                    _gridBordero.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function CancelarBorderoClick(e) {

    executarReST("Bordero/ValidarCancelamentoBordero", { Codigo: _bordero.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                if (!r.Data.Valido) {
                    if (r.Data.PermiteCancelarBaixa) {
                        exibirConfirmacao("Confirmação", r.Data.Mensagem, function () {
                            FinalizarCancelamentoBordero();
                        }, null, "Confirmar", "Cancelar");
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", r.Data.Mensagem, 30000);
                    }
                } else {
                    exibirConfirmacao("Confirmação", "Deseja realmente cancelar este borderô?", function () {
                        FinalizarCancelamentoBordero();
                    });
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function FinalizarCancelamentoBordero() {
    executarReST("Bordero/Cancelar", { Codigo: _bordero.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Borderô cancelado com sucesso!");
                PreecherCamposEdicaoBordero(r.Data);
                _gridBordero.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}



function LiquidarBorderoClick(e) {
    exibirConfirmacao("Atenção!", "Deseja realmente liquidar o borderô?", function () {
        Salvar(_liquidacaoBordero, "Bordero/Liquidar", function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Borderô liquidado com sucesso!");
                    PreecherCamposEdicaoBordero(r.Data);
                    FecharTelaLiquidacaoBorderoClick();
                    _gridBordero.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function ImprimirBorderoClick(e) {
    executarDownload("Bordero/Imprimir", { Codigo: _bordero.Codigo.val() });
}

function FecharTelaLiquidacaoBorderoClick() {
    LimparCampos(_liquidacaoBordero);
    Global.fecharModal('knockoutLiquidarBordero');
}

function AbrirTelaLiquidacaoBorderoClick() {
    LimparCampos(_liquidacaoBordero);
    _liquidacaoBordero.Codigo.val(_bordero.Codigo.val());
    Global.abrirModal('knockoutLiquidarBordero');
}

//*******MÉTODOS*******

function RetornoConsultaPessoaBordero(dados) {
    _bordero.Pessoa.codEntity(dados.Codigo);
    _bordero.Pessoa.val(dados.Nome + " (" + dados.CPF_CNPJ + ")");

    ObterInformacoesPagamentoBordero(dados.Codigo, null);
}

function RetornoConsultaGrupoPessoasBordero(dados) {
    _bordero.GrupoPessoas.codEntity(dados.Codigo);
    _bordero.GrupoPessoas.val(dados.Descricao);

    ObterInformacoesPagamentoBordero(null, dados.Codigo);
}

function ObterInformacoesPagamentoBordero(cpfCnpjPessoa, codigoGrupoPessoas) {
    executarReST("Bordero/ObterDadosPagamentoTomador", { CPFCNPJ: cpfCnpjPessoa, GrupoPessoas: codigoGrupoPessoas }, function (r) {
        if (r.Success) {
            if (r.Data) {

                _bordero.Agencia.val(r.Data.Agencia);
                _bordero.DigitoAgencia.val(r.Data.DigitoAgencia);
                _bordero.NumeroConta.val(r.Data.NumeroConta);
                _bordero.TipoConta.val(r.Data.TipoConta);

                _bordero.Banco.val(r.Data.Banco.Descricao);
                _bordero.Banco.codEntity(r.Data.Banco.Codigo);

                _bordero.Tomador.val(r.Data.Tomador.Descricao);
                _bordero.Tomador.codEntity(r.Data.Tomador.Codigo);

            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function BuscarBorderos() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarBordero, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridBordero = new GridView(_pesquisaBordero.Pesquisar.idGrid, "Bordero/Pesquisa", _pesquisaBordero, menuOpcoes, { column: 1, dir: orderDir.desc }, null);

    _gridBordero.CarregarGrid();
}

function EditarBordero(borderoGrid) {
    LimparCamposBordero();

    _bordero.Codigo.val(borderoGrid.Codigo);

    BuscarBorderoPorCodigo();
}

function BuscarBorderoPorCodigo() {
    BuscarPorCodigo(_bordero, "Bordero/BuscarPorCodigo", function (arg) {
        PreecherCamposEdicaoBordero(arg.Data);
    });
}

function PreecherCamposEdicaoBordero(dados) {
    PreencherObjetoKnout(_bordero, { Data: dados });
    PreecherResumoBordero(dados);

    _pesquisaBordero.ExibirFiltros.visibleFade(false);

    _bordero.Salvar.visible(false);
    _CRUDBordero.Liquidar.visible(false);
    _CRUDBordero.Finalizar.visible(false);
    _CRUDBordero.Cancelar.visible(false);
    _CRUDBordero.Imprimir.visible(false);

    _CRUDBordero.GerarNovoBordero.visible(true);

    if (dados.Situacao == EnumSituacaoBordero.EmAndamento) {
        _CRUDBordero.Cancelar.visible(true);
        _CRUDBordero.Imprimir.visible(true);
        _CRUDBordero.Finalizar.visible(true);
        _CRUDBordero.GerarNovoBordero.visible(true);

        _bordero.Salvar.visible(true);
        _bordero.Salvar.text("Salvar Dados");
        _bordero.TipoPessoa.enable(false);
        _bordero.Pessoa.enable(false);
        _bordero.GrupoPessoas.enable(false);
    } else if (dados.Situacao == EnumSituacaoBordero.Finalizado) {
        _bordero.Salvar.visible(false);
        _CRUDBordero.Cancelar.visible(true);
        _CRUDBordero.Imprimir.visible(true);
        _CRUDBordero.Liquidar.visible(true);
        _CRUDBordero.GerarNovoBordero.visible(true);

        SetarEnableCamposKnockout(_bordero, false);
    } else if (dados.Situacao == EnumSituacaoBordero.Quitado) {
        _bordero.Salvar.visible(false);
        _CRUDBordero.Cancelar.visible(true);
        _CRUDBordero.Imprimir.visible(true);
        _CRUDBordero.GerarNovoBordero.visible(true);

        SetarEnableCamposKnockout(_bordero, false);
    } else if (dados.Situacao == EnumSituacaoBordero.AgIntegracao || dados.Situacao == EnumSituacaoBordero.IntegracaoRejeitada) {
        _bordero.Salvar.visible(false);
        _CRUDBordero.Cancelar.visible(true);
        _CRUDBordero.Imprimir.visible(true);
        _CRUDBordero.GerarNovoBordero.visible(true);

        SetarEnableCamposKnockout(_bordero, false);
    } else if (dados.Situacao == EnumSituacaoBordero.Cancelado) {
        _CRUDBordero.GerarNovoBordero.visible(true);

        SetarEnableCamposKnockout(_bordero, false);
    }

    SetarEtapaBordero();

    _contatoClienteBordero.ShowButton();
}

function LimparCamposBordero() {
    _CRUDBordero.GerarNovoBordero.visible(false);
    _CRUDBordero.Finalizar.visible(false);
    _CRUDBordero.Liquidar.visible(false);
    _CRUDBordero.Cancelar.visible(false);

    _bordero.Salvar.text("Gerar Borderô");
    _bordero.Salvar.visible(true);

    SetarEnableCamposKnockout(_bordero, true);

    LimparCampos(_bordero);
    LimparResumoBordero();

    _contatoClienteBordero.HideButton();
}