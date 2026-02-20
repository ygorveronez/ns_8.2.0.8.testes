/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPallet.js" />
/// <reference path="../../Enumeradores/EnumEntradaSaida.js" />
/// <reference path="../../Enumeradores/EnumRegraPallet.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumCodigoFiltroPesquisa.js" />

var _pesquisaControlePallet;
var _gridControlePallet;
var _gridSaldoControlePallet;
var _cabecalhoControlePallet;
var _detalhesControlePallet;
var _adicionarPallet;
var _alterarResponsavelControlePallet;

function inicializarControlePallet() {
    _pesquisaControlePallet = new PesquisaControlePallet();
    KoBindings(_pesquisaControlePallet, "knockoutPesquisaControlePallet", false, _pesquisaControlePallet.Pesquisar.id);

    _cabecalhoControlePallet = new TotalizadoresControlePallet();
    KoBindings(_cabecalhoControlePallet, "knoutCabecalhoControlePallet");

    _detalhesControlePallet = new DetalhesControlePallet();
    KoBindings(_detalhesControlePallet, "knoutDetalhesControlePallet");

    _adicionarPallet = new AdicionarPallet();
    KoBindings(_adicionarPallet, "knoutAdicionarPallet");

    BuscarFilial(_pesquisaControlePallet.Filial);
    BuscarTransportadores(_pesquisaControlePallet.Transportador);
    BuscarClientes(_pesquisaControlePallet.Cliente);
    BuscarEstados(_pesquisaControlePallet.UFOrigem);
    BuscarEstados(_pesquisaControlePallet.UFDestino);
    BuscarCargas(_adicionarPallet.Carga);
    BuscarXMLNotaFiscal(_adicionarPallet.NotaFiscal);
    BuscarFilial(_adicionarPallet.Filial);
    BuscarTransportadores(_adicionarPallet.Transportador);
    BuscarClientes(_adicionarPallet.Cliente);

    loadAnexos();
    loadValePallet();
    carregarGridControlePallet();
}

var PesquisaControlePallet = function () {
    this.NotaFiscal = PropertyEntity({ text: "Nota Fiscal:", getType: typesKnockout.int, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Carga = PropertyEntity({ text: "Carga:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente", idBtnSearch: guid(), visible: ko.observable(true) });
    this.UFOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "UF Origem", idBtnSearch: guid(), visible: ko.observable(true) });
    this.UFDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "UF Destino", idBtnSearch: guid(), visible: ko.observable(true) });
    this.EscritorioVendas = PropertyEntity({ text: "Escritório Vendas:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação Pallet", val: ko.observable(EnumSituacaoPallet.Todos), options: EnumSituacaoPallet.obterOpcoesPesquisa(), def: EnumSituacaoPallet.Todos });
    this.ResponsavelPallet = PropertyEntity({ text: "Responsável Pallet", val: ko.observable(EnumResponsavelPallet.Todos), options: EnumResponsavelPallet.obterOpcoesPesquisa(), def: EnumResponsavelPallet.Todos });
    this.DataInicialCriacaoCarga = PropertyEntity({ text: "Dt. Inicial Criação da Carga", getType: typesKnockout.date, val: ko.observable(""), visible: ko.observable(true) });
    this.DataFinalCriacaoCarga = PropertyEntity({ text: "Dt. Final Criação da Carga", getType: typesKnockout.date, val: ko.observable(""), visible: ko.observable(true) });
    this.DataInicialNotaFiscal = PropertyEntity({ text: "Dt. Inicial Nota Fiscal", getType: typesKnockout.date, val: ko.observable(""), visible: ko.observable(true) });
    this.DataFinalNotaFiscal = PropertyEntity({ text: "Dt. Final Nota Fiscal", getType: typesKnockout.date, val: ko.observable(""), visible: ko.observable(true) });
    this.RegraPallet = PropertyEntity({ text: "Regra Pallet", val: ko.observable(EnumRegraPallet.Nenhuma), options: EnumRegraPallet.obterOpcoesPesquisa(), def: EnumRegraPallet.Nenhuma });

    this.DataInicialCriacaoCarga.dateRangeLimit = this.DataFinalCriacaoCarga;
    this.DataFinalCriacaoCarga.dateRangeInit = this.DataInicialCriacaoCarga;

    this.DataInicialNotaFiscal.dateRangeLimit = this.DataFinalNotaFiscal;
    this.DataFinalNotaFiscal.dateRangeInit = this.DataInicialNotaFiscal;

    this.ModeloFiltrosPesquisa = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloDeFiltroDePesquisa, idBtnSearch: guid(),
        tipoFiltroPesquisa: EnumCodigoFiltroPesquisa.ControlePallet,
    });
    this.ConfiguracaoModeloFiltroPesquisa = PropertyEntity({ eventClick: function (e) { abrirConfiguracaoModeloFiltroPesquisa(EnumCodigoFiltroPesquisa.ControlePallet, _pesquisaControlePallet) }, type: types.event, text: Localization.Resources.Gerais.Geral.SalvarFiltro, visible: ko.observable(true) });
    this.CarregarFiltrosPesquisa = PropertyEntity({
        eventClick: function (e) {
            abrirBuscaFiltrosManual(e);
        }, type: types.event, text: "Carregar Filtro", idFade: guid(), visible: ko.observable(true)
    });
    this.Pesquisar = PropertyEntity({
        type: types.event, eventClick: function (e) {
            fecharFiltrosControlePallet();
            _gridControlePallet.CarregarGrid();
            ObterTotaisControlePallet();
        }, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

var AlterarResponsavelControlePallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.ResponsavelPallet = PropertyEntity({ text: "Responsável Pallet", val: ko.observable(EnumResponsavelPallet.Todos), options: EnumResponsavelPallet.obterOpcoesPesquisa(), def: EnumResponsavelPallet.Todos, required: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente", idBtnSearch: guid(), visible: ko.observable(false) });

    this.ResponsavelPallet.val.subscribe(function (newValue) {
        _alterarResponsavelControlePallet.Cliente.visible(newValue == EnumResponsavelPallet.Cliente);
        _alterarResponsavelControlePallet.Filial.visible(newValue == EnumResponsavelPallet.Filial);

        LimparCampo(_alterarResponsavelControlePallet.Cliente);
        LimparCampo(_alterarResponsavelControlePallet.Filial);
    });

    this.Adicionar = PropertyEntity({ eventClick: alterarResponsavelPalletClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

var TotalizadoresControlePallet = function () {
    this.Todos = PropertyEntity({ text: "Todos", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true) });
    this.Pendente = PropertyEntity({ text: "Pendentes", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true) });
    this.Concluido = PropertyEntity({ text: "Concluidos", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true) });
    this.Cancelado = PropertyEntity({ text: "Cancelados", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true) });
    this.Reserva = PropertyEntity({ text: "Reserva", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true) });
    this.AguardandoAvaliacao = PropertyEntity({ text: "Aguardando Avaliação", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true) });
};

var DetalhesControlePallet = function () {
    this.Situacao = PropertyEntity({ text: "Situação", val: ko.observable(""), def: "" });
    this.Origem = PropertyEntity({ text: "Origem", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Destino = PropertyEntity({ text: "Destino", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Carga = PropertyEntity({ text: "Carga", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.QuantidadePallets = PropertyEntity({ text: "Quantidade de Pallets", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroNotaFiscalPermuta = PropertyEntity({ text: "Número NF Permuta", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.SerieNotaFiscalPermuta = PropertyEntity({ text: "Série NF Permuta", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroNotaFiscalDevolucao = PropertyEntity({ text: "Número NFD", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.SerieNotaFiscalDevolucao = PropertyEntity({ text: "Série NFD", val: ko.observable(""), def: "", visible: ko.observable(true) });
};

var AdicionarPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NotaFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Nota Fiscal", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cliente", idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial", idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Transportador", idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false) });
    this.QuantidadePallets = PropertyEntity({ text: "*Quantidade de Paletts", val: ko.observable(""), def: "", visible: ko.observable(true), required: ko.observable(true) });
    this.ResponsavelPallet = PropertyEntity({ text: "*Responsável Pallet", val: ko.observable(EnumResponsavelPallet.Todos), options: EnumResponsavelPallet.obterOpcoes(), def: EnumResponsavelPallet.Todos, required: ko.observable(true) });
    this.TipoEntradaSaida = PropertyEntity({ text: "*Tipo Movimentação", val: ko.observable(EnumEntradaSaida.Entrada), options: EnumEntradaSaida.obterOpcoes(), def: EnumEntradaSaida.Entrada, required: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação", val: ko.observable(""), def: "", visible: ko.observable(true) });


    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable("Selecione um arquivo para anexar"), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _adicionarPallet.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.AdicionarAnexo = PropertyEntity({ eventClick: adicionarAnexoModalClick, type: types.event, text: Localization.Resources.Gerais.Geral.AdicionarAnexo, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: fecharModalAdicionarPallet, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });

    this.ResponsavelPallet.val.subscribe(exibirCampos);
};

// #endregion Knouts

function exibirCampos(newValue) {
    _adicionarPallet.Cliente.visible(newValue == EnumResponsavelPallet.Cliente);
    _adicionarPallet.Cliente.required(newValue == EnumResponsavelPallet.Cliente);

    _adicionarPallet.Filial.visible(newValue == EnumResponsavelPallet.Filial);
    _adicionarPallet.Filial.required(newValue == EnumResponsavelPallet.Filial);

    _adicionarPallet.Transportador.visible(newValue == EnumResponsavelPallet.Transportador);
    _adicionarPallet.Transportador.required(newValue == EnumResponsavelPallet.Transportador);

    LimparCampo(_adicionarPallet.Cliente);
    LimparCampo(_adicionarPallet.Filial);
    LimparCampo(_adicionarPallet.Transportador);
}

function carregarGridControlePallet() {
    const configuracaoExportacao = {
        url: "ControlePallet/ExportarPesquisa",
        titulo: "Controle de Pallet"
    };

    const detalhes = {
        descricao: "Detalhes",
        id: guid(),
        evento: "onclick",
        metodo: detalhesControlePalletClick,
        tamanho: "10",
        icone: "",
        visibilidade: true
    };

    const auditoria = {
        descricao: "Auditar",
        id: guid(),
        evento: "onclick",
        metodo: exibirAuditoria,
        tamanho: "10",
        icone: "",
        visibilidade: VisibilidadeOpcaoAuditoria
    }

    const reverter = {
        descricao: "Reverter",
        id: guid(),
        evento: "onclick",
        metodo: reverterMovimentacao,
        tamanho: "10",
        icone: "",
        visibilidade: exibirOpcaoReverter
    }

    const anexos = {
        descricao: "Anexos",
        id: guid(),
        evento: "onclick",
        metodo: adicionarAnexoModalClick,
        tamanho: "10",
        icone: "",
        visibilidade: true
    }

    const laudo = {
        descricao: "Informar Recebimento",
        id: guid(),
        evento: "onclick",
        metodo: exibirModalLaudo,
        tamanho: "10",
        icone: "",
        visibilidade: exibirOpcaoLaudo
    }

    const valePallet = {
        descricao: "Informar Recebimento Vale Pallet",
        id: guid(),
        evento: "onclick",
        metodo: exibirModalAdicionarPallet,
        tamanho: "10",
        icone: "",
        visibilidade: exibirOpcaoRecebimentoValePallet
    }

    const alterarResponsavelPallet = {
        descricao: "Alterar Responsável Pallet",
        id: guid(),
        evento: "onclick",
        metodo: abrirModalAlterarResponsavelPalletClick,
        tamanho: "10",
        icone: "",
        visibilidade: exibirOpcaoAlterarResponsavelPallet
    }

    const menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        opcoes: [
            detalhes,
            anexos,
            reverter,
            auditoria,
            laudo,
            valePallet,
            alterarResponsavelPallet
        ]
    };

    _gridControlePallet = new GridViewExportacao("grid-gestao-pallet-controle-pallet", "ControlePallet/Pesquisa", _pesquisaControlePallet, menuOpcoes, configuracaoExportacao, null, 25, null, null, null, callbackColumnDefaultControlePallet);
    _gridControlePallet.SetPermitirEdicaoColunas(true);
    _gridControlePallet.SetSalvarPreferenciasGrid(true);
    _gridControlePallet.SetHabilitarScrollHorizontal(true, 200);

    _gridControlePallet.CarregarGrid();

    ObterTotaisControlePallet();
}

function callbackColumnDefaultControlePallet(cabecalho, valorColuna, dadosLinha) {
    if (cabecalho.name == "LeadTimeDevolucaoFormatado") {
        if (dadosLinha.SituacaoDevolucao == "Pendente") {
            setTimeout(function () {
                $('#' + cabecalho.name + '-' + dadosLinha.DT_RowId)
                    .countdown(moment(valorColuna, "DD/MM/YYYY HH:mm:ss").format("YYYY/MM/DD HH:mm:ss"), { elapse: true, precision: 1000 })
                    .on('update.countdown', function (event) {
                        if (event.offset.totalDays > 0)
                            $(this).text(event.strftime('%-Dd %H:%M:%S'));
                        else
                            $(this).text(event.strftime('%H:%M:%S'));
                    })
                    .on('finish.countdown', function (event) {
                        $(this).text("Tempo limite atingido");
                    });
            }, 1000);
        }

        return '<span id="' + cabecalho.name + '-' + dadosLinha.DT_RowId + '"></span>';
    }
}

function carregarGridSaldoControlePallet() {
    _gridSaldoControlePallet = new GridView("grid-gestao-pallet-saldo-controle-pallet", "ControlePallet/PesquisaSaldo", _pesquisaControlePallet, null, null, 15);
    _gridSaldoControlePallet.CarregarGrid();
}

function detalhesControlePalletClick(registroSelecionado) {
    const dados = { Codigo: registroSelecionado.Codigo }

    executarReST("ControlePallet/BuscarPorCodigo", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_detalhesControlePallet, retorno);
                _detalhesControlePallet.Origem.val(retorno.Data.Carga.DadosSumarizados.Origem);
                _detalhesControlePallet.Destino.val(retorno.Data.Carga.DadosSumarizados.Destino);
                _detalhesControlePallet.Carga.val(retorno.Data.Carga.CodigoCargaEmbarcador);

                exibirModalDetalhesControlePallet();

                $("#modalDetalhePedido").one('hidden.bs.modal', function () {
                    LimparCampos(_detalhesControlePallet);
                });
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function ObterTotaisControlePallet() {
    const dados = RetornarObjetoPesquisa(_pesquisaControlePallet);

    executarReST("ControlePallet/ObterTotais", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                PreencherObjetoKnout(_cabecalhoControlePallet, arg);

                //_cabecalhoControlePallet.TotalSaldo.val(arg.Data.TotalSaldo);
                //_cabecalhoControlePallet.TotalSaldo.requiredClass(arg.Data.TotalSaldo >= 0 ? 'text-success' : 'text-danger');
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function abrirBuscaFiltrosManual(e) {
    const buscaFiltros = new BuscarModeloFiltroPesquisa(e.ModeloFiltrosPesquisa, function (retorno) {
        if (retorno.Codigo == 0) return;

        e.ModeloFiltrosPesquisa.codEntity(retorno.Codigo);
        e.ModeloFiltrosPesquisa.val(retorno.ModeloDescricao);

        PreencherJsonFiltroPesquisa(_pesquisaControlePallet, retorno.Dados);
    }, EnumCodigoFiltroPesquisa.ControlePallet);

    buscaFiltros.AbrirBusca();
}

function exibirFiltrosControlePallet() {
    Global.abrirModal('divModalFiltroPesquisaControlePallet');
}

function fecharFiltrosControlePallet() {
    Global.fecharModal('divModalFiltroPesquisaControlePallet');
}

function exibirModalDetalhesControlePallet() {
    Global.abrirModal('divModalDetalhesControlePallet');
}

function fecharModalDetalhesControlePallet() {
    Global.fecharModal('divModalDetalhesControlePallet');
}

function exibirModalAdicionarPalletControle() {
    Global.abrirModal('divModalAdicionarPallet');
}

function fecharModalAdicionarPallet() {
    LimparCampos(_adicionarPallet);
    Global.fecharModal('divModalAdicionarPallet');
}

function fecharModalSaldoControlePallet() {
    Global.fecharModal('divModalSaldoControlePallet');
}

function exibirModalSaldoControlePallet() {
    carregarGridSaldoControlePallet();
    Global.abrirModal('divModalSaldoControlePallet');
}

function abrirModalAlterarResponsavelPalletClick(movimentacao) {
    _alterarResponsavelControlePallet = new AlterarResponsavelControlePallet();
    KoBindings(_alterarResponsavelControlePallet, "knockoutAlterarResponsavelControlePallet");

    _alterarResponsavelControlePallet.Codigo.val(movimentacao.Codigo);

    BuscarFilial(_alterarResponsavelControlePallet.Filial);
    BuscarClientes(_alterarResponsavelControlePallet.Cliente);
    
    Global.abrirModal('divModalAlterarResponsavel');
}

function fecharModalAlterarResponsavel() {
    LimparCampos(_alterarResponsavelControlePallet);
    Global.fecharModal('divModalAlterarResponsavel');
}

function AdicionarClick() {
    if (!ValidarCamposObrigatorios(_adicionarPallet)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja adicionar esses pallets?", function () {
        const data = RetornarObjetoPesquisa(_adicionarPallet);
        executarReST("ControlePallet/Adicionar", data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                //_adicionarPallet.Codigo.val(retorno.Data.Codigo)
                enviarArquivosAnexados(retorno.Data.Codigo);
                fecharModalAdicionarPallet();
                carregarGridControlePallet();
                } else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    });
}

function alterarResponsavelPalletClick() {
    if (!ValidarCamposObrigatorios(_alterarResponsavelControlePallet)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja alterar o responsável?", function () {
        const data = RetornarObjetoPesquisa(_alterarResponsavelControlePallet);
        executarReST("ControlePallet/AlterarResponsavelManual", data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    fecharModalAlterarResponsavel();
                    carregarGridControlePallet();
                } else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    });
}

function exibirAuditoria(movimentacao) {
    const data = { Codigo: movimentacao.Codigo };
    const closureAuditoria = OpcaoAuditoria("MovimentacaoPallet", null, movimentacao);
    closureAuditoria(data);
}

function reverterMovimentacao(movimentacao) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja reverte essa movimentação?", function () {
        const data = { Codigo: movimentacao.Codigo };
        executarReST("ControlePallet/Reverter", data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Movimentação revertida!");
                    carregarGridControlePallet();
                } else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    });
}

function exibirOpcaoReverter(movimentacao) {
    return movimentacao.TipoLancamento === EnumTipoLancamento.Manual;
}

function exibirOpcaoLaudo(movimentacao) {
    return movimentacao.RegraPallet == EnumRegraPallet.Transferencia && movimentacao.TipoMovimentacao == EnumEntradaSaida.Entrada;
}

function exibirOpcaoAlterarResponsavelPallet(movimentacao) {
    return movimentacao.CodigoCargaPedido != 0 && movimentacao.Situacao != EnumSituacaoPallet.Cancelado;
}

function exibirOpcaoRecebimentoValePallet(movimentacao) {
    return movimentacao.RegraPallet == EnumRegraPallet.ValePallet && movimentacao.Situacao != EnumSituacaoPallet.Cancelado;
}