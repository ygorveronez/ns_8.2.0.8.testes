/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPedagio.js" />
/// <reference path="../../Enumeradores/EnumTipoPedagio.js" />
/// <reference path="../../Consultas/Motorista.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPedagio;
var _pedagio;
var _pesquisaPedagio;

var _pesquisaTipoPedagio = [{ text: "Todos", value: EnumTipoPedagio.Todos }, { text: "Débito", value: EnumTipoPedagio.Debito }, { text: "Crédito", value: EnumTipoPedagio.Credito }];
var _tipoPedagio = [{ text: "Débito", value: EnumTipoPedagio.Debito }, { text: "Crédito", value: EnumTipoPedagio.Credito }];
var _sitaucaoPedagio = [{ text: "Todos", value: EnumSituacaoPedagio.Todos }, { text: "Inconsistente", value: EnumSituacaoPedagio.Inconsistente }, { text: "Lançado", value: EnumSituacaoPedagio.Lancado }, { text: "Fechado", value: EnumSituacaoPedagio.Fechado }];

var PesquisaPedagio = function () {
    this.Veiculo = PropertyEntity({ text: "Veículo:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.TipoPedagio = PropertyEntity({ text: "Tipo: ", val: ko.observable(EnumTipoPedagio.Todos), options: _pesquisaTipoPedagio, def: EnumTipoPedagio.Todos });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _sitaucaoPedagio, def: 0 });
    this.DataImportacao = PropertyEntity({ text: "Data Importação:", required: false, enable: ko.observable(true), getType: typesKnockout.date });

    this.DataInicial = PropertyEntity({ text: "Data Passagem de:", required: false, enable: ko.observable(true), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até:", required: false, enable: ko.observable(true), getType: typesKnockout.date });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPedagio.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Pedagio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Data = PropertyEntity({ text: "*Data Passagem:", required: true, enable: ko.observable(true), getType: typesKnockout.dateTime });
    this.Veiculo = PropertyEntity({ text: "*Veículo:", textAviso: ko.observable(""), type: types.entity, codEntity: ko.observable(0), required: true, enable: ko.observable(true), idBtnSearch: guid(), issue: 143 });
    this.Valor = PropertyEntity({ text: "*Valor:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, enable: ko.observable(true), val: ko.observable(0.00), maxlength: 10 });
    this.Praca = PropertyEntity({ text: "Praça:", enable: ko.observable(true), maxlength: 150 });
    this.Rodovia = PropertyEntity({ text: "Rodovia:", enable: ko.observable(true), maxlength: 150 });
    this.TipoMovimento = PropertyEntity({ text: "*Movimento Financeiro:", type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true), issue: 364 });
    this.TipoPedagio = PropertyEntity({ text: "*Tipo: ", val: ko.observable(EnumTipoPedagio.Debito), options: _tipoPedagio, def: EnumTipoPedagio.Debito, enable: ko.observable(true), required: true });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", enable: ko.observable(true) });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false) });

    // Upload de arquivo
    this.PercentualProcessado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(false) });
    this.MensagemFalhaGeracao = PropertyEntity({ visible: ko.observable(false) });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), visible: ko.observable(true) });
    this.Enviar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });

    this.MoedaCotacaoBancoCentral.val.subscribe(function (novoValor) {
        CalcularMoedaEstrangeira();
    });

    this.DataBaseCRT.val.subscribe(function (novoValor) {
        CalcularMoedaEstrangeira();
    });

    this.ValorMoedaCotacao.val.subscribe(function (novoValor) {
        ConverterValor();
    });

    this.ValorOriginalMoedaEstrangeira.val.subscribe(function (novoValor) {
        ConverterValor();
    });

    this.Valor.val.subscribe(function (novoValor) {
        ConverterValorOriginal();
    });

    //CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Reabrir = PropertyEntity({ eventClick: reabrirClick, type: types.event, text: "Reabrir", visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "ImportacaoDePedagio/Importar",
        UrlConfiguracao: "ImportacaoDePedagio/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O038_Pedagio,
        ParametrosRequisicao: function () {
            return {
                Inserir: true,
                Atualizar: false
            };
        },
        CallbackImportacao: function () {
            _gridPedagio.CarregarGrid();
        }
    });
};

//*******EVENTOS*******

function loadPedagio() {

    _pesquisaPedagio = new PesquisaPedagio();
    KoBindings(_pesquisaPedagio, "knockoutPesquisaPedagio", false, _pesquisaPedagio.Pesquisar.id);

    _pedagio = new Pedagio();
    KoBindings(_pedagio, "knockoutCadastroPedagio");

    HeaderAuditoria("Pedagio", _pedagio);

    new BuscarVeiculos(_pesquisaPedagio.Veiculo, retornoSelecaoVeiculoPesquisa);
    new BuscarVeiculos(_pedagio.Veiculo, retornoSetarMotorista, null, null, null, null, null, null, null, null, null, null, _pedagio.Data);
    //new BuscarVeiculos(_pedagio.Veiculo, retornoSelecaoVeiculo);

    new BuscarTipoMovimento(_pedagio.TipoMovimento);
    new BuscarMotoristas(_pedagio.Motorista);

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        _pedagio.MoedaCotacaoBancoCentral.visible(true);
        _pedagio.DataBaseCRT.visible(true);
        _pedagio.ValorMoedaCotacao.visible(true);
        _pedagio.ValorOriginalMoedaEstrangeira.visible(true);
    }

    carregaSubscribes();
    buscarPedagio();
}

function retornoSelecaoVeiculoPesquisa(data) {
    if (data != null & data.Placa != null & data.Placa != "" & data.Codigo > 0) {
        _pesquisaPedagio.Veiculo.val(data.Placa);
        _pesquisaPedagio.Veiculo.codEntity(data.Codigo);
    }
}

function adicionarClick(e, sender) {
    resetarTabs();
    Salvar(_pedagio, "Pedagio/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridPedagio.CarregarGrid();
                limparCamposPedagio();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function atualizarClick(e, sender) {
    resetarTabs();
    Salvar(_pedagio, "Pedagio/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridPedagio.CarregarGrid();
                limparCamposPedagio();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function excluirClick(e, sender) {
    resetarTabs();
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse pedágio?", function () {
        ExcluirPorCodigo(_pedagio, "Pedagio/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridPedagio.CarregarGrid();
                    limparCamposPedagio();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function reabrirClick(e, sender) {
    resetarTabs();
    exibirConfirmacao("Confirmação", "Tem certeza que deseja reabrir o pedágio?", function () {
        ExcluirPorCodigo(_pedagio, "Pedagio/Reabrir", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Reaberto com sucesso");
                    _gridPedagio.CarregarGrid();

                    _pedagio.Atualizar.visible(true);
                    _pedagio.Excluir.visible(true);
                    _pedagio.Reabrir.visible(false);

                    AlternarCampos(true);
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
    limparCamposPedagio();
}

function retornoSetarMotorista(data) {
    if (_CONFIGURACAO_TMS.BuscarMotoristaDaCargaLancamentoAbastecimentoAutomatico && _pedagio.Data.val() != "" && _pedagio.Data.val() != null && (_pedagio.Motorista.codEntity() == 0 || _pedagio.Motorista.codEntity == null)) {
        executarReST("Infracao/BuscarMotorista", { Veiculo: data.Codigo, Data: _pedagio.Data.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _pedagio.Motorista.codEntity(retorno.Data.Codigo);
                    _pedagio.Motorista.val(retorno.Data.Nome);
                }
                else if (data != null & data.Motorista != null & data.Motorista != "" & data.CodigoMotorista > 0) {
                    _pedagio.Motorista.val(data.Motorista);
                    _pedagio.Motorista.codEntity(data.CodigoMotorista);
                }
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    }

    if (!_CONFIGURACAO_TMS.BuscarMotoristaDaCargaLancamentoAbastecimentoAutomatico && data != null & data.Motorista != null & data.Motorista != "" & data.CodigoMotorista > 0) {
        _pedagio.Motorista.val(data.Motorista);
        _pedagio.Motorista.codEntity(data.CodigoMotorista);
    }

    if (data != null & data.Placa != null & data.Placa != "" & data.Codigo > 0) {
        // Exibe movimento financeiro quando veiculo for de terceiro
        if (data.TipoPropriedade == "T") {
            _pedagio.TipoMovimento.visible(true);
            _pedagio.TipoMovimento.required(true);
        }
        else {
            _pedagio.TipoMovimento.visible(false);
            _pedagio.TipoMovimento.required(false);
        }

        _pedagio.Veiculo.val(data.Placa);
        _pedagio.Veiculo.codEntity(data.Codigo);
    }
}

//*******MÉTODOS*******

function VerificarImportacaoPedagioEstaSelecionada() {

    exibirMensagem(tipoMensagem.ok, "Sucesso", "Importação dos pedágios realizada com sucesso.");

    SetarPercentualProcessamento(0);
    _pedagio.PercentualProcessado.visible(false);
    _pedagio.MensagemFalhaGeracao.visible(false);
    _pedagio.Arquivo.visible(true);

    _pedagio.Arquivo.val("");
    _gridPedagio.CarregarGrid();
    resetarTabs();
}

function AtualizarProgressImportacaoPedagio(percentual) {
    finalizarRequisicao();
    SetarPercentualProcessamento(percentual);
}

function SetarPercentualProcessamento(percentual) {
    var strPercentual = parseInt(percentual) + "%";
    _pedagio.PercentualProcessado.val(strPercentual);
    $("#" + _pedagio.PercentualProcessado.id).css("width", strPercentual)
}

function carregaSubscribes() {
    // Exibe movimento financeiro quando veiculo por de terceiro
    _pedagio.Veiculo.codEntity.subscribe(function (novoValor) {
        if (novoValor == 0) {
            _pedagio.TipoMovimento.visible(false);
            _pedagio.TipoMovimento.required(false);
        }
    });
    _pedagio.Veiculo.val.subscribe(function (novoValor) {
        if (novoValor == "") _pedagio.Veiculo.codEntity(0);
    });
}

function editarPedagio(pedagioGrid) {
    limparCamposPedagio();
    _pedagio.Codigo.val(pedagioGrid.Codigo);
    BuscarPorCodigo(_pedagio, "Pedagio/BuscarPorCodigo", function (arg) {
        _pesquisaPedagio.ExibirFiltros.visibleFade(false);

        // Veiculo nao cadastrado
        if (arg.Data.Placa != null) {
            var placa = arg.Data.Placa;
            _pedagio.Veiculo.textAviso(" Placa (" + placa + ") não cadastrada");
            _pedagio.Veiculo.val(placa);
        } else {
            _pedagio.Veiculo.textAviso("");
            if (arg.Data.Veiculo.Tipo == "T") {
                _pedagio.TipoMovimento.visible(true);
                _pedagio.TipoMovimento.required(true);
            } else {
                _pedagio.TipoMovimento.visible(false);
                _pedagio.TipoMovimento.required(false);
            }
        }

        // Acoes de acordo com a situacao
        if (arg.Data.Situacao == EnumSituacaoPedagio.Fechado) {
            _pedagio.Atualizar.visible(false);
            _pedagio.Excluir.visible(false);
            _pedagio.Reabrir.visible(true);
            AlternarCampos(false);
        } else {
            _pedagio.Atualizar.visible(true);
            _pedagio.Excluir.visible(true);
            _pedagio.Reabrir.visible(false);
        }
        AbaImportar(false);
        _pedagio.Adicionar.visible(false);
        _pedagio.Cancelar.visible(true);

        resetarTabs();
        _pedagio.ValorMoedaCotacao.val(arg.Data.ValorMoedaCotacao);
    }, null);
}

function importarClick(e, sender) {
    var file = document.getElementById(_pedagio.Arquivo.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);

    SetarPercentualProcessamento(0);
    _pedagio.PercentualProcessado.visible(true);
    _pedagio.MensagemFalhaGeracao.visible(false);
    _pedagio.Arquivo.visible(false);

    enviarArquivo("ImportacaoDePedagio/ImportarPedagios?callback=?", { Codigo: 0 }, formData, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Importação dos pedágios realizada com sucesso.");
            SetarPercentualProcessamento(0);
            _pedagio.PercentualProcessado.visible(false);
            _pedagio.MensagemFalhaGeracao.visible(false);
            _pedagio.Arquivo.visible(true);

            _pedagio.Arquivo.val("");
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function buscarPedagio() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPedagio, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "Pedagio/ExportarPesquisa",
        titulo: "Pedágios"
    };

    _gridPedagio = new GridViewExportacao(_pesquisaPedagio.Pesquisar.idGrid, "Pedagio/Pesquisa", _pesquisaPedagio, menuOpcoes, configExportacao);
    _gridPedagio.CarregarGrid();
}


function limparCamposPedagio() {
    _pedagio.Adicionar.visible(true);
    _pedagio.Atualizar.visible(false);
    _pedagio.Cancelar.visible(false);
    _pedagio.Excluir.visible(false);
    _pedagio.Reabrir.visible(false);
    LimparCampos(_pedagio);
    resetarTabs();
    AlternarCampos(true);
    AbaImportar(true);
    _pedagio.PercentualProcessado.visible(false);
    _pedagio.MensagemFalhaGeracao.visible(false);
    _pedagio.Arquivo.visible(true);
    SetarPercentualProcessamento(0);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function AbaImportar(status) {
    if (status)
        $("#liTabImportacao").show();
    else
        $("#liTabImportacao").hide();
}

function AlternarCampos(status) {
    SetarEnableCamposKnockout(_pedagio, status);
}

function CalcularMoedaEstrangeira() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _pedagio.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _pedagio.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                _pedagio.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValor();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValor() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_pedagio.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_pedagio.ValorOriginalMoedaEstrangeira.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _pedagio.Valor.val(Globalize.format(valorOriginal * valorMoedaCotacao, "n2"));
        }
    }
}

function ConverterValorOriginal() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_pedagio.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_pedagio.Valor.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _pedagio.ValorOriginalMoedaEstrangeira.val(Globalize.format(valorOriginal / valorMoedaCotacao, "n2"));
        }
    }
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}