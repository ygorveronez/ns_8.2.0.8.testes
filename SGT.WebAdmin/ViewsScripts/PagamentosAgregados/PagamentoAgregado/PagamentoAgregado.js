/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPagamentoAgregado.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="Etapa.js" />
/// <reference path="Pagamento.js" />
/// <reference path="Documento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPagamentos;
var _CRUDPagamento;
var _pesquisaPagamentos;

var _situacao = [
    { text: "Todas", value: "" },
    { text: "Iniciado", value: EnumSituacaoPagamentoAgregado.Iniciado },
    { text: "Ag. Aprovação", value: EnumSituacaoPagamentoAgregado.AgAprovacao },
    { text: "Finalizado", value: EnumSituacaoPagamentoAgregado.Finalizado },
    { text: "Rejeitada", value: EnumSituacaoPagamentoAgregado.Rejeitada },
    { text: "Sem Regra", value: EnumSituacaoPagamentoAgregado.SemRegra }
];

var CRUDPagamento = function () {
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: "Adicionar", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.Finalizar = PropertyEntity({ eventClick: FinalizarClick, type: types.event, text: "Finalizar", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.Estornar = PropertyEntity({ eventClick: EstornarClick, type: types.event, text: "Estornar", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparLancamentoClick, type: types.event, text: "Limpar", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasClick, type: types.event, text: "Reprocessar Regras", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true) });

    this.ImprimirReciboPagamento = PropertyEntity({ eventClick: ImprimirReciboClick, type: types.event, text: "Recibo", idGrid: guid(), visible: ko.observable(false) });
    this.ImprimirExtratoPagamento = PropertyEntity({ eventClick: ImprimirExtratoClick, type: types.event, text: "Extrato", idGrid: guid(), visible: ko.observable(true) });
    this.ImprimirFaturaPagamento = PropertyEntity({ eventClick: ImprimirFaturaClick, type: types.event, text: "Fatura", idGrid: guid(), visible: ko.observable(_CONFIGURACAO_TMS.HabilitarLayoutFaturaPagamentoAgregado) });
};

var PesquisaPagamentos = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true, val: ko.observable() });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Numero = PropertyEntity({ text: "Número:", val: ko.observable(""), def: "", getType: typesKnockout.int });
    this.NumeroContrato = PropertyEntity({ text: "Número Contrato:", val: ko.observable(""), def: "", getType: typesKnockout.int });

    this.Situacao = PropertyEntity({ val: ko.observable(""), def: "", options: _situacao, text: "Situação:" });

    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Agregado:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.LimitarOperacaoPorEmpresa) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPagamentos.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadPagamentoAgregado() {

    _CRUDPagamento = new CRUDPagamento();
    KoBindings(_CRUDPagamento, "knockoutCRUD");

    _pesquisaPagamentos = new PesquisaPagamentos();
    KoBindings(_pesquisaPagamentos, "knockoutPesquisaPagamentos", false, _pesquisaPagamentos.Pesquisar.id);

    loadEtapaPagamentoAgregado();
    loadPagamento();
    loadDocumento();
    loadAprovacao();

    LimparCamposLancamento();

    // Inicia as buscas
    new BuscarClientes(_pesquisaPagamentos.Cliente);
    new BuscarTransportadores(_pesquisaPagamentos.Empresa, null, null, null, null, null, null, null, null, null, true);

    BuscarPagamentos();
}

function salvarClick(e, sender) {
    if (!ValidarDadosPagamento())
        return false;

    Salvar(_pagamento, "PagamentoAgregado/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_pagamento.ListaAnexosNovos.list.length > 0)
                    EnviarPagamentoAgregadoAnexos(arg.Data.Codigo);
                else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Pagamento criado com sucesso");

                    _CRUDPagamento.Adicionar.visible(false);
                    _CRUDPagamento.Finalizar.visible(true);
                    _CRUDPagamento.ImprimirReciboPagamento.visible(true);

                    _pagamento.Situacao.val(EnumSituacaoPagamentoAgregado.Iniciado);
                    _pagamento.Codigo.val(arg.Data.Codigo);
                    _pagamento.Numero.val(arg.Data.Numero);
                    _documento.Codigo.val(arg.Data.Codigo);

                    Etapa2Liberada();
                    $("a[href='#knockoutDocumento']").click();
                }

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function AtualizarClick(e, sender) {
    if (!ValidarDadosPagamento())
        return false;
    preencherListasSelecao();

    if (_pagamento.ListaAnexosExcluidos.list.length > 0)
        _pagamento.ListaAnexosExcluidos.text = JSON.stringify(_pagamento.ListaAnexosExcluidos.list);

    Salvar(_pagamento, "PagamentoAgregado/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_pagamento.ListaAnexosNovos.list.length > 0)
                    EnviarPagamentoAgregadoAnexos(_pagamento.Codigo.val());
                else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Pagamento atualizado com sucesso");
                    LimparCamposLancamento();
                    BuscarPagamentos();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function EstornarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja estornar o pagamento de agregado selecionado?", function () {
        Salvar(_pagamento, "PagamentoAgregado/Estornar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Pagamento estornado com sucesso");
                    LimparCamposLancamento();
                    BuscarPagamentos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        }, sender);
    });
}

function CancelarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar o pagamento de agregado?", function () {
        Salvar(_pagamento, "PagamentoAgregado/Cancelar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Pagamento cancelado com sucesso");
                    LimparCamposLancamento();
                    BuscarPagamentos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function FinalizarClick(e, sender) {
    if (!ValidarDadosPagamento())
        return false;
    preencherListasSelecao();
    Salvar(_pagamento, "PagamentoAgregado/Finalizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Pagamento criado com sucesso");

                _pagamento.Codigo.val(arg.Data.Codigo);
                _pagamento.Numero.val(arg.Data.Numero);
                _documento.Codigo.val(arg.Data.Codigo);
                _aprovacao.Codigo.val(arg.Data.Codigo);

                BuscarPagamentoPorCodigo(arg.Data.Codigo);

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function ImprimirExtratoClick(e, sender) {
    var data = { Codigo: _pagamento.Codigo.val() };
    executarReST("PagamentoAgregado/GerarExtratoPagamentoAgregado", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    })
}

function ImprimirReciboClick(e, sender) {
    var data = { Codigo: _pagamento.Codigo.val() };
    executarReST("PagamentoAgregado/GerarReciboPagamentoAgregado", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    })
}

function ImprimirFaturaClick() {
    executarDownload("PagamentoAgregado/GerarFaturaPagamentoAgregado", { PagamentoAgregado: _pagamento.Codigo.val() });
}

function reprocessarRegrasClick(e, sender) {
    executarReST("PagamentoAgregado/ReprocessarRegras", { Codigo: _pagamento.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.PossuiRegra || arg.Data.Finalizado) {
                    _gridPagamentos.CarregarGrid();
                    LimparCamposLancamento();
                    BuscarPagamentoPorCodigo(arg.Data.Codigo);
                } else
                    exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function limparLancamentoClick(e, sender) {
    LimparCamposLancamento();
}


//*******MÉTODOS*******

function EnviarPagamentoAgregadoAnexos(codigoPagamentoAgregado) {
    var formData = new FormData();
    for (var i = 0; i < _pagamento.ListaAnexosNovos.list.length; i++) {
        formData.append("Arquivo", _pagamento.ListaAnexosNovos.list[i].Arquivo);
        formData.append("Descricao", _pagamento.ListaAnexosNovos.list[i].DescricaoAnexo.val);
    }

    var data = {
        CodigoPagamentoAgregado: codigoPagamentoAgregado
    };
    enviarArquivo("PagamentoAgregado/EnviarAnexos?callback=?", data, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso");
                LimparCamposLancamento();
                BuscarPagamentos();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function BuscarPagamentos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPagamentos, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridPagamentos = new GridView(_pesquisaPagamentos.Pesquisar.idGrid, "PagamentoAgregado/Pesquisa", _pesquisaPagamentos, menuOpcoes);
    _gridPagamentos.CarregarGrid();
}

function editarPagamentos(itemGrid) {
    // Limpa os campos
    LimparCamposLancamento();

    // Esconde filtros
    _pesquisaPagamentos.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarPagamentoPorCodigo(itemGrid.Codigo);
}

function BuscarPagamentoPorCodigo(codigo, cb) {
    executarReST("PagamentoAgregado/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data !== null) {
            $("#fdsDocumentos").show();
            $("#fdsPercentualDocumentos").hide();

            // -- Pagamento
            EditarPagamento(arg.Data);

            // -- Dados Pagamento
            EditarDadosPagamento(arg.Data);

            // -- Dados Documento
            EditarDadosDocumento(arg.Data);

            // -- Aprovação
            ListarAprovacoes(arg.Data);

            SetarEtapaDocumento();
            RecarregarGridPagamentoAgregadoAnexo();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function EditarPagamento(data) {
    _pagamento.Situacao.val(data.Situacao);

    _CRUDPagamento.Adicionar.visible(false);
    _CRUDPagamento.Finalizar.visible(true);

    if (data.Situacao === EnumSituacaoPagamentoAgregado.Iniciado) {
        _CRUDPagamento.Atualizar.visible(true);
        _CRUDPagamento.Cancelar.visible(true);
    }
    else if (data.Situacao === EnumSituacaoPagamentoAgregado.SemRegra) {
        _CRUDPagamento.ReprocessarRegras.visible(true);
        _CRUDPagamento.Finalizar.visible(false);
    }
    else if (data.Situacao === EnumSituacaoPagamentoAgregado.Finalizado) {
        _CRUDPagamento.Estornar.visible(true);
    }

    _CRUDPagamento.ImprimirReciboPagamento.visible(true);
    _CRUDPagamento.Limpar.visible(true);
}

function LimparCamposLancamento() {
    LimparCampos(_pagamento);

    _pagamento.DataInicial.requiredClass("form-control");
    _pagamento.DataFinal.requiredClass("form-control");
    _pagamento.DataInicialOcorrencia.requiredClass("form-control");
    _pagamento.DataFinalOcorrencia.requiredClass("form-control");

    _CRUDPagamento.Atualizar.visible(false);
    _CRUDPagamento.Cancelar.visible(false);
    _CRUDPagamento.Adicionar.visible(true);
    _CRUDPagamento.Limpar.visible(true);
    _CRUDPagamento.ReprocessarRegras.visible(false);
    _CRUDPagamento.Finalizar.visible(false);
    _CRUDPagamento.Estornar.visible(false);
    _CRUDPagamento.ImprimirReciboPagamento.visible(false);

    SetarEtapaInicioLancamento();

    LimparCamposDadosPagamento();
    LimparCamposDadosDocumento();

    LimparCamposPagamentoAgregadoAnexo();
    RecarregarGridPagamentoAgregadoAnexo();

    $("#knockoutPagamentoAgregado").click();
    Global.ResetarAbas();
}

function ValidarDadosPagamento() {
    var tudoCerto = true;

    _pagamento.DataInicial.requiredClass("form-control");
    _pagamento.DataFinal.requiredClass("form-control");
    _pagamento.DataInicialOcorrencia.requiredClass("form-control");
    _pagamento.DataFinalOcorrencia.requiredClass("form-control");

    if ((_pagamento.DataInicial.val() === "") && (_pagamento.DataFinal.val() === "") && (_pagamento.DataInicialOcorrencia.val() === "") && (_pagamento.DataFinalOcorrencia.val() === "")) {
        tudoCerto = false;
        _pagamento.DataInicial.requiredClass("form-control is-invalid");
        _pagamento.DataFinal.requiredClass("form-control is-invalid");
        _pagamento.DataInicialOcorrencia.requiredClass("form-control is-invalid");
        _pagamento.DataFinalOcorrencia.requiredClass("form-control is-invalid");
    }
    if ((_pagamento.DataInicial.val() !== "") && (_pagamento.DataFinal.val() === "") && tudoCerto === true) {
        tudoCerto = false;
        _pagamento.DataInicial.requiredClass("form-control is-invalid");
        _pagamento.DataFinal.requiredClass("form-control is-invalid");
    }
    if ((_pagamento.DataInicial.val() === "") && (_pagamento.DataFinal.val() !== "") && tudoCerto === true) {
        tudoCerto = false;
        _pagamento.DataInicial.requiredClass("form-control is-invalid");
        _pagamento.DataFinal.requiredClass("form-control is-invalid");
    }
    if ((_pagamento.DataInicialOcorrencia.val() !== "") && (_pagamento.DataFinalOcorrencia.val() === "") && tudoCerto === true) {
        tudoCerto = false;
        _pagamento.DataInicialOcorrencia.requiredClass("form-control is-invalid");
        _pagamento.DataFinalOcorrencia.requiredClass("form-control is-invalid");
    }
    if ((_pagamento.DataInicialOcorrencia.val() === "") && (_pagamento.DataFinalOcorrencia.val() !== "") && tudoCerto === true) {
        tudoCerto = false;
        _pagamento.DataInicialOcorrencia.requiredClass("form-control is-invalid");
        _pagamento.DataFinalOcorrencia.requiredClass("form-control is-invalid");
    }

    if (!tudoCerto) {
        exibirMensagem(tipoMensagem.aviso, "Período", "Favor informe o período correto (inicio e fim) para o pagamento ao agregado.");
    }

    return tudoCerto;
}