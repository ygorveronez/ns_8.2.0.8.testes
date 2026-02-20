/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoContratoFinanciamento.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />
/// <reference path="ContratoFinanciamentoValor.js" />
/// <reference path="ContratoFinanciamentoParcela.js" />
/// <reference path="ContratoFinanciamentoVeiculo.js" />
/// <reference path="ContratoFinanciamentoDocumentoEntrada.js" />
/// <reference path="ContratoFinanciamentoAnexo.js" />
/// <reference path="ContratoFinanciamentoModalidades.js" />
/// <reference path="../RateioDespesaVeiculo/RateioDespesaVeiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridContratoFinanciamento;
var _contratoFinanciamento;
var _pesquisaContratoFinanciamento;
var _crudContratoFinanciamento;
var _PermissoesPersonalizadas;

var PesquisaContratoFinanciamento = function () {
    this.NumeroDocumento = PropertyEntity({ text: "Nº Documento: " });
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoContratoFinanciamento.Todos), options: EnumSituacaoContratoFinanciamento.obterOpcoesPesquisa(), def: EnumSituacaoContratoFinanciamento.Todos });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ text: "Veículo: ", getType: typesKnockout.string });
    this.DocumentoEntrada = PropertyEntity({ text: "Nº Documento Entrada: ", getType: typesKnockout.string });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridContratoFinanciamento.CarregarGrid();
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

var ContratoFinanciamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "Número: ", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false) });
    this.Situacao = PropertyEntity({ text: "*Situação: ", val: ko.observable(EnumSituacaoContratoFinanciamento.Aberto), options: EnumSituacaoContratoFinanciamento.obterOpcoes(), def: EnumSituacaoContratoFinanciamento.Aberto, required: ko.observable(true), enable: ko.observable(true) });

    this.NumeroDocumento = PropertyEntity({ text: "*Número Documento: ", required: ko.observable(true), maxlength: 200, enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ text: "*Data Emissão: ", getType: typesKnockout.date, required: ko.observable(true), enable: ko.observable(true) });
    this.ValorTotal = PropertyEntity({ text: "*Valor Capital:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(true), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 5000, val: ko.observable(""), enable: ko.observable(true) });

    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Movimento para o Valor Capital:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.ValorAcrescimo = PropertyEntity({ text: "Valor Acréscimo:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(false), enable: ko.observable(true) });
    this.TipoMovimentoAcrescimo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Movimento para o Acréscimo:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Outros), options: EnumFormaTitulo.obterOpcoes(), text: "*Forma do Título: ", def: EnumFormaTitulo.Outros, required: true, enable: ko.observable(true) });

    this.Valores = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Parcelamento = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.Veiculos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Modalidades = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.DocumentosEntrada = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaAnexos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosNovos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosExcluidos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDContratoFinanciamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Estornar = PropertyEntity({ eventClick: estornarClick, type: types.event, text: "Estornar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.SalvarObservacao = PropertyEntity({ eventClick: salvarObservacaoClick, type: types.event, text: "Salvar Observação", visible: ko.observable(false) });
    this.SalvalVeiculos = PropertyEntity({ eventClick: salvarVeiculoClick, type: types.event, text: "Salvar Veiculos", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadContratoFinanciamento() {
    _contratoFinanciamento = new ContratoFinanciamento();
    KoBindings(_contratoFinanciamento, "knockoutCadastroContratoFinanciamento");

    HeaderAuditoria("ContratoFinanciamento", _contratoFinanciamento);

    _crudContratoFinanciamento = new CRUDContratoFinanciamento();
    KoBindings(_crudContratoFinanciamento, "knockoutCRUDContratoFinanciamento");

    _pesquisaContratoFinanciamento = new PesquisaContratoFinanciamento();
    KoBindings(_pesquisaContratoFinanciamento, "knockoutPesquisaContratoFinanciamento", _pesquisaContratoFinanciamento.Pesquisar.id);

    new BuscarClientes(_pesquisaContratoFinanciamento.Fornecedor);
    new BuscarEmpresa(_contratoFinanciamento.Empresa);
    new BuscarClientes(_contratoFinanciamento.Fornecedor);
    new BuscarTipoMovimento(_contratoFinanciamento.TipoMovimento);
    new BuscarTipoMovimento(_contratoFinanciamento.TipoMovimentoAcrescimo);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _contratoFinanciamento.Empresa.visible(false);
        _contratoFinanciamento.Empresa.required(false);
    }

    carregarDespesaVeiculo("conteudoDespesaVeiculo", buscarContratoFinanciamento);

    loadContratoFinanciamentoValor();
    loadContratoFinanciamentoParcela();
    loadContratoFinanciamentoVeiculo();
    loadContratoFinanciamentoDocumentoEntrada();
    loadContratoFinanciamentoAnexo();
    loadContratoFinanciamentoModalidade();
}

function adicionarClick(e, sender) {
    resetarTabs();
    var valido = true;

    if (!validaCamposObrigatoriosContratoFinanciamentoParcela()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, Informe todos os campos obrigatórios do Parcelamento");
        return;
    }

    if (valido) {
        _contratoFinanciamento.Parcelamento.val(JSON.stringify(RetornarObjetoPesquisa(_contratoFinanciamentoParcela)));
        preencherListasSelecaoContratoFinanciamentoVeiculo();
        preencherListasSelecaoContratoFinanciamentoDocumentoEntrada();
        preencherListasSelecaoContratoFinanciamentoModalidades();

        Salvar(_contratoFinanciamento, "ContratoFinanciamento/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if (_contratoFinanciamento.ListaAnexosNovos.list.length > 0)
                        EnviarContratoFinanciamentoAnexos(arg.Data.Codigo, true);
                    else {
                        if (_CONFIGURACAO_TMS.AbrirRateioDespesaVeiculoAutomaticamente && _contratoFinanciamento.Situacao.val() === EnumSituacaoContratoFinanciamento.Finalizado) {

                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                            _gridContratoFinanciamento.CarregarGrid();
                            limparCamposContratoFinanciamento();

                            AbrirTelaReteioDespesaVeiculo(arg.Data);

                        } else {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                            _gridContratoFinanciamento.CarregarGrid();
                            limparCamposContratoFinanciamento();

                            editarContratoFinanciamento({ Codigo: arg.Data.Codigo }, false);
                        }
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }
}

function atualizarClick(e, sender) {
    resetarTabs();
    var valido = true;

    if (!validaCamposObrigatoriosContratoFinanciamentoParcela()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, Informe todos os campos obrigatórios do Parcelamento");
        return;
    }

    if (valido) {
        _contratoFinanciamento.Parcelamento.val(JSON.stringify(RetornarObjetoPesquisa(_contratoFinanciamentoParcela)));
        preencherListasSelecaoContratoFinanciamentoVeiculo();
        preencherListasSelecaoContratoFinanciamentoDocumentoEntrada();
        preencherListasSelecaoContratoFinanciamentoModalidades();

        if (_contratoFinanciamento.ListaAnexosExcluidos.list.length > 0)
            _contratoFinanciamento.ListaAnexosExcluidos.text = JSON.stringify(_contratoFinanciamento.ListaAnexosExcluidos.list);

        Salvar(_contratoFinanciamento, "ContratoFinanciamento/Atualizar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if (_contratoFinanciamento.ListaAnexosNovos.list.length > 0)
                        EnviarContratoFinanciamentoAnexos(_contratoFinanciamento.Codigo.val(), false);
                    else {
                        if (_CONFIGURACAO_TMS.AbrirRateioDespesaVeiculoAutomaticamente && _contratoFinanciamento.Situacao.val() === EnumSituacaoContratoFinanciamento.Finalizado) {

                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                            _gridContratoFinanciamento.CarregarGrid();
                            limparCamposContratoFinanciamento();

                            AbrirTelaReteioDespesaVeiculo(arg.Data);

                        } else {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                            _gridContratoFinanciamento.CarregarGrid();
                            limparCamposContratoFinanciamento();
                        }
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }
}

function estornarClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente estornar o Contrato de Financiamento " + _contratoFinanciamento.Numero.val() + "?", function () {
        executarReST("ContratoFinanciamento/Estornar", { Codigo: _contratoFinanciamento.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Estornado com sucesso.");
                    _gridContratoFinanciamento.CarregarGrid();
                    editarContratoFinanciamento({ Codigo: _contratoFinanciamento.Codigo.val() }, false);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposContratoFinanciamento();
}

function salvarObservacaoClick(e) {
    exibirConfirmacao("Atenção!", "Deseja realmente alterar a observação do Contrato de Financiamento " + _contratoFinanciamento.Numero.val() + "?", function () {
        executarReST("ContratoFinanciamento/SalvarObservacao", {
            Codigo: _contratoFinanciamento.Codigo.val(),
            Observacao: _contratoFinanciamento.Observacao.val()
        }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Observação alterada com sucesso.");
                    _gridContratoFinanciamento.CarregarGrid();
                    editarContratoFinanciamento({ Codigo: _contratoFinanciamento.Codigo.val() }, false);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

//*******MÉTODOS*******

function EnviarContratoFinanciamentoAnexos(codigoContratoFinanciamento, adicionar) {
    var formData = obterFormDataContratoFinanciamentoAnexo();

    if (formData) {
        enviarArquivo("ContratoFinanciamento/EnviarAnexos?callback=?", { Codigo: codigoContratoFinanciamento }, formData, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if (_CONFIGURACAO_TMS.AbrirRateioDespesaVeiculoAutomaticamente && _contratoFinanciamento.Situacao.val() === EnumSituacaoContratoFinanciamento.Finalizado) {

                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                        _gridContratoFinanciamento.CarregarGrid();
                        limparCamposContratoFinanciamento();

                        AbrirTelaReteioDespesaVeiculo(arg.Data);
                    } else {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso");
                        _gridContratoFinanciamento.CarregarGrid();
                        limparCamposContratoFinanciamento();

                        if (adicionar)
                            editarContratoFinanciamento({ Codigo: arg.Data.Codigo }, false);
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function buscarContratoFinanciamento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarContratoFinanciamento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridContratoFinanciamento = new GridView(_pesquisaContratoFinanciamento.Pesquisar.idGrid, "ContratoFinanciamento/Pesquisa", _pesquisaContratoFinanciamento, menuOpcoes);
    _gridContratoFinanciamento.CarregarGrid();
}

function editarContratoFinanciamento(contratoGrid, deixarCamposEditaveis) {
    limparCamposContratoFinanciamento();
    _contratoFinanciamento.Codigo.val(contratoGrid.Codigo);
    BuscarPorCodigo(_contratoFinanciamento, "ContratoFinanciamento/BuscarPorCodigo", function (arg) {
        _pesquisaContratoFinanciamento.ExibirFiltros.visibleFade(false);
        _crudContratoFinanciamento.Atualizar.visible(true);
        _crudContratoFinanciamento.Cancelar.visible(true);
        _crudContratoFinanciamento.Adicionar.visible(false);

        RecarregarGridValorContratoFinanciamento();
        RecarregarGridContratoFinanciamentoVeiculo();
        RecarregarGridContratoFinanciamentoDocumentoEntrada();
        RecarregarGridContratoFinanciamentoAnexo();
        RecarregarGridContratoFinanciamentoModalidades();

        if (arg.Data.Parcelamento !== null && arg.Data.Parcelamento !== undefined) {
            PreencherObjetoKnout(_contratoFinanciamentoParcela, { Data: arg.Data.Parcelamento });
        }

        _contratoFinanciamentoParcela.Parcelas.visible(true);
        _gridContratoFinanciamentoParcelas.CarregarGrid();

        _contratoFinanciamento.ValorTotal.enable(false);
        _contratoFinanciamento.ValorAcrescimo.enable(false);
        if (_contratoFinanciamento.Situacao.val() === EnumSituacaoContratoFinanciamento.Cancelado || _contratoFinanciamento.Situacao.val() === EnumSituacaoContratoFinanciamento.Finalizado) {
            SetarEnableCamposKnockout(_contratoFinanciamento, false);
            SetarEnableCamposKnockout(_contratoFinanciamentoParcelaValor, false);
            SetarEnableCamposKnockout(_contratoFinanciamentoVeiculo, false);
            SetarEnableCamposKnockout(_contratoFinanciamentoDocumentoEntrada, false);
            SetarEnableCamposKnockout(_contratoFinanciamentoAnexo, false);
            _gridVeiculosContratoFinanciamento.DesabilitarOpcoes();

            if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFinanciamento_PermitirAlterarVeiculos, _PermissoesPersonalizadas)) {
                _gridVeiculosContratoFinanciamento.HabilitarOpcoes();
                _crudContratoFinanciamento.SalvalVeiculos.visible(true);
                _contratoFinanciamentoVeiculo.Veiculo.enable(true);
            }

            _gridDocumentoEntradasContratoFinanciamento.DesabilitarOpcoes();
            //_gridContratoFinanciamentoAnexo.DesabilitarOpcoes();
            _crudContratoFinanciamento.Atualizar.visible(false);
            desativarEditarGridContratoFinanciamentoParcelas();
        }

        if (_contratoFinanciamento.Situacao.val() === EnumSituacaoContratoFinanciamento.Finalizado) {
            _crudContratoFinanciamento.Estornar.visible(true);
        }

        _gridContratoFinanciamentoValor.DesabilitarOpcoes();
    
        SetarEnableCamposKnockout(_contratoFinanciamentoValor, false);
        SetarEnableCamposKnockout(_contratoFinanciamentoParcela, false);

        if (_contratoFinanciamento.Situacao.val() === EnumSituacaoContratoFinanciamento.Aberto) {
            _contratoFinanciamentoParcela.VencimentoPrimeiraParcela.enable(true);
            _contratoFinanciamentoParcela.Provisao.enable(true);
            _contratoFinanciamentoParcela.LimparParcela.visible(true);
            _contratoFinanciamentoParcela.LimparParcela.enable(true);
        }
        else {
            _contratoFinanciamentoParcela.LimparParcela.visible(false);
            _contratoFinanciamentoParcela.LimparParcela.enable(false);
        }

        if (_contratoFinanciamentoParcela.Dividir.val() === true || _contratoFinanciamentoParcela.Repetir.val() === true) {
            _contratoFinanciamentoParcela.Periodicidade.visible(true);
            _contratoFinanciamentoParcela.NumeroOcorrencia.visible(true);
            _contratoFinanciamentoParcela.VencimentoPrimeiraParcela.visible(true);
            _contratoFinanciamentoParcela.GerarParcela.visible(true);
            _contratoFinanciamentoParcela.DiaOcorrencia.visible(true);
        }

        if (deixarCamposEditaveis != null && deixarCamposEditaveis != undefined && deixarCamposEditaveis === true) {
            _contratoFinanciamento.ValorTotal.enable(true);
            _contratoFinanciamento.ValorAcrescimo.enable(true);
            SetarEnableCamposKnockout(_contratoFinanciamentoParcela, true);
            _contratoFinanciamentoParcela.LimparParcela.visible(false);
        }

        if (_contratoFinanciamento.Situacao.val() === EnumSituacaoContratoFinanciamento.Finalizado) {
            _crudContratoFinanciamento.SalvarObservacao.visible(true);
            _contratoFinanciamento.Observacao.enable(true);
        }

    }, null);
}

function AbrirTelaReteioDespesaVeiculo(data) {
    LimparCamposRateioDespesa();
    Global.abrirModal('divModalDespesaVeiculo');
    _rateioDespesa.ContratoFinanciamento.val(data.Codigo);
    _rateioDespesa.NumeroDocumento.val(data.NumeroDocumento);
    _rateioDespesa.TipoDocumento.val(data.TipoDocumento);
    _rateioDespesa.Valor.val(data.Valor);
    _rateioDespesa.Pessoa.val(data.Pessoa.Descricao);
    _rateioDespesa.Pessoa.codEntity(data.Pessoa.Codigo);

}

function limparCamposContratoFinanciamento() {
    _crudContratoFinanciamento.SalvarObservacao.visible(false);
    _crudContratoFinanciamento.Atualizar.visible(false);
    _crudContratoFinanciamento.Cancelar.visible(false);
    _crudContratoFinanciamento.Estornar.visible(false);
    _crudContratoFinanciamento.Adicionar.visible(true);
    LimparCampos(_contratoFinanciamento);

    LimparCamposContratoFinanciamentoValor();
    limparCamposContratoFinanciamentoParcela();
    limparCamposContratoFinanciamentoVeiculo();
    limparCamposContratoFinanciamentoDocumentoEntrada();
    LimparCamposContratoFinanciamentoAnexo();

    RecarregarGridContratoFinanciamentoAnexo();
    _contratoFinanciamentoParcela.Parcelas.visible(false);
    _gridContratoFinanciamentoParcelas.CarregarGrid();

    SetarEnableCamposKnockout(_contratoFinanciamento, true);
    SetarEnableCamposKnockout(_contratoFinanciamentoValor, true);
    SetarEnableCamposKnockout(_contratoFinanciamentoParcela, true);
    SetarEnableCamposKnockout(_contratoFinanciamentoParcelaValor, true);
    SetarEnableCamposKnockout(_contratoFinanciamentoVeiculo, true);
    SetarEnableCamposKnockout(_contratoFinanciamentoDocumentoEntrada, true);
    SetarEnableCamposKnockout(_contratoFinanciamentoAnexo, true);
    _contratoFinanciamento.Numero.enable(false);

    _gridContratoFinanciamentoValor.HabilitarOpcoes();
    _gridVeiculosContratoFinanciamento.HabilitarOpcoes();
    _gridDocumentoEntradasContratoFinanciamento.HabilitarOpcoes();
    //_gridContratoFinanciamentoAnexo.HabilitarOpcoes();
    habilitarEditarGridContratoFinanciamentoParcelas();

    $("#liTabContratoFinanciamento a").click();
    resetarTabs();
}

function resetarTabs() {
    $(".nav-tabs").each(function () {
        $(this).find("a:first").tab("show");
    });
}

function salvarVeiculoClick() {
    const Veiculos = obterSomenteVeiculosGrid();
    const Codigo = _contratoFinanciamento.Codigo.val();
    executarReST('ContratoFinanciamento/AtualizarSomenteVeiculos', { Veiculos, Codigo }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", 'Veiculos Atualizados');
        limparCamposContratoFinanciamento();
    })
}