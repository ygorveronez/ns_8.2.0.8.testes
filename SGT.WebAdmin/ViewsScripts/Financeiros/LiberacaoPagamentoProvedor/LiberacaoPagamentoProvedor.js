/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumEtapaLiberacaoPagamentoProvedor.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLiberacaoPagamentoProvedor.js" />
/// <reference path="LiberacaoPagamentoEtapa.js" />
/// <reference path="../../Configuracao/EmissaoCTe/EmissaoCTe.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDLiberacaoPagamentoProvedor;
var _documentosEmpresaFilial;
var _gridLiberacaoPagamentoProvedor;
var _pesquisaLiberacaoPagamentoProvedor;
var _gridDetalhesCarga;

/*
 * Declaração das Classes
 */

var PesquisaLiberacaoPagamentoProvedor = function () {
    this.Provedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Provedor:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", getType: typesKnockout.selectMultiple, val: ko.observable([EnumSituacaoLiberacaoPagamentoProvedor.Aberto]), options: EnumSituacaoLiberacaoPagamentoProvedor.obterOpcoes(), def: [EnumSituacaoLiberacaoPagamentoProvedor.Aberto] });
    this.Etapa = PropertyEntity({ text: "Etapa: ", val: ko.observable(1), options: EnumEtapaLiberacaoPagamentoProvedor.obterOpcoes(), def: "", visible: ko.observable(true), getType: typesKnockout.selectMultiple });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            recarregarGridPesquisaLiberacaoPagamentoProvedor();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var DocumentosEmpresaFilial = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador (Empresa/Filial):", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TransportadorProvedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador (Provedor OS):", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor) });
    this.NumeroCarga = PropertyEntity({ val: ko.observable(""), def: "", type: types.entity, codEntity: ko.observable(0), text: "Número da Carga:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoDocumentoProvedor = PropertyEntity({ val: ko.observable(EnumTipoDocumentoProvedor.CTe), options: EnumTipoDocumentoProvedor.obterOpcoes(), def: EnumTipoDocumentoProvedor.CTe, text: "Tipo de Documento:", enable: ko.observable(true) });
    this.LocalidadePrestacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "*Localidade Prestação:", idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false), enable: ko.observable(true) });

    this.PesquisaDetalhesCarga = PropertyEntity({ eventClick: recarregarDetalhesCarga, type: types.event, text: "Pesquisar", visible: ko.observable(true), enable: ko.observable(true) });
    this.DetalhesCarga = PropertyEntity({ idGrid: guid(), visible: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(true), enable: ko.observable(true) });

    this.ConfirmarDocumentosEmpresaFilial = PropertyEntity({ eventClick: confirmarDocumentosEmpresaFilial, type: types.event, text: "Confirmar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar / Novo", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.TipoDocumentoProvedor.val.subscribe(function (novoValor) {
        let tipoDocumentoProvedorDiferenteNFSe = novoValor != EnumTipoDocumentoProvedor.NFSe

        visibilidadeCampoCRUDAprovacao(tipoDocumentoProvedorDiferenteNFSe);
        visibilidadeGRIDDetalhesCarga(tipoDocumentoProvedorDiferenteNFSe);
        visibilidadeDocumentosProvedor(tipoDocumentoProvedorDiferenteNFSe);
        visibilidadeDocumentosEmpresaFilial(tipoDocumentoProvedorDiferenteNFSe);

        _gridDetalhesCarga.CarregarGrid();
    });

    this.LocalidadePrestacao.multiplesEntities.subscribe(recarregarDetalhesCarga);
}

function loadGridLiberacaoPagamentoProvedor() {
    const opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "15", icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] }

    _gridLiberacaoPagamentoProvedor = new GridView(_pesquisaLiberacaoPagamentoProvedor.Pesquisar.idGrid, "LiberacaoPagamentoProvedor/Pesquisa", _pesquisaLiberacaoPagamentoProvedor, menuOpcoes);
    _gridLiberacaoPagamentoProvedor.CarregarGrid();
}

function loadLiberacaoPagamentoProvedor() {
    _documentosEmpresaFilial = new DocumentosEmpresaFilial();
    KoBindings(_documentosEmpresaFilial, "knockoutDocumentosEmpresaFilial");

    _pesquisaLiberacaoPagamentoProvedor = new PesquisaLiberacaoPagamentoProvedor();
    KoBindings(_pesquisaLiberacaoPagamentoProvedor, "knockoutPesquisaLiberacaoPagamentoProvedor", false, _pesquisaLiberacaoPagamentoProvedor.Pesquisar.id);

    BuscarCargas(_pesquisaLiberacaoPagamentoProvedor.Carga, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
    BuscarClientes(_pesquisaLiberacaoPagamentoProvedor.Provedor);
    BuscarEmpresa(_documentosEmpresaFilial.Tomador);
    BuscarClientes(_documentosEmpresaFilial.TransportadorProvedor);
    BuscarCargas(_documentosEmpresaFilial.NumeroCarga, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
    BuscarLocalidades(_documentosEmpresaFilial.LocalidadePrestacao);

    $("#importacaoDocumentosProvedor").show();

    loadLiberacaoPagamentoProvedorEtapa();
    loadGridLiberacaoPagamentoProvedor();
    loadLiberacao();
    loadDocumentosProvedor();
    loadAprovacao();
    loadRegrasAprovacao();
    loadDelegar();
    BuscarDetalhesCarga();

    HeaderAuditoria("PagamentoProvedor", _documentosProvedor, "CodigoPagamentoProvedor");

    //Inicia a grid como seleção unica
    _gridDetalhesCarga.SetarPermissaoSelecionarSomenteUmRegistro(true);
    _documentosEmpresaFilial.SelecionarTodos.visible(false);
}

function editarClick(registroSelecionado) {
    _pesquisaLiberacaoPagamentoProvedor.ExibirFiltros.visibleFade(false);

    buscarLiberacaoPagamentoPorCodigo(registroSelecionado.Codigo);
}

function confirmarDocumentosEmpresaFilial() {
    exibirConfirmacao("Confirmar Documentos da Empresa Filial", "Deseja confirmar os Documentos da Empresa Filial?", function () {
        var cargasSelecionadas = _gridDetalhesCarga.ObterMultiplosSelecionados();

        var codigosCargasSelecionadas = new Array();
        var codigosLocaisPrestacao = new Array();

        for (var i = 0; i < cargasSelecionadas.length; i++) {
            codigosCargasSelecionadas.push(cargasSelecionadas[i].Codigo);
        }

        for (var i = 0; i < _documentosEmpresaFilial.LocalidadePrestacao.multiplesEntities().length; i++) {
            codigosLocaisPrestacao.push(_documentosEmpresaFilial.LocalidadePrestacao.multiplesEntities()[i].Codigo);
        }

        var data = {
            CodigosCargaPedido: JSON.stringify(codigosCargasSelecionadas),
            Codigo: _documentosProvedor.CodigoPagamentoProvedor.val(),
            TipoDocumentoProvedor: _documentosEmpresaFilial.TipoDocumentoProvedor.val(),
            LocalidadePrestacao: JSON.stringify(codigosLocaisPrestacao),
        }

        executarReST("LiberacaoPagamentoProvedor/ConfirmarDocumentosEmpresaFilial", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Confirmado com sucesso");
                    
                    SetarEtapasPagamentoProvedor(EnumEtapaLiberacaoPagamentoProvedor.DocumentoProvedor, arg.Data.Situacao);
                    PreencherObjetoDocumentosProvedor(arg.Data);

                    validarEtapa(arg.Data.Status);

                    carregarCargasSelecionadas(arg.Data.Codigo);
                    _gridDetalhesCarga.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function BuscarDetalhesCarga() {
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _documentosEmpresaFilial.SelecionarTodos,
        callbackNaoSelecionado: null,
        callbackSelecionado: null,
        callbackSelecionarTodos: null,
        somenteLeitura: false
    }

    _gridDetalhesCarga = new GridView(_documentosEmpresaFilial.DetalhesCarga.idGrid, "LiberacaoPagamentoProvedor/PesquisaDetalhesCarga", _documentosEmpresaFilial, null, null, null, null, null, null, multiplaescolha);
    _gridDetalhesCarga.CarregarGrid();
}

function buscarLiberacaoPagamentoPorCodigo(codigo) {
    _documentosEmpresaFilial.Codigo.val(codigo);
    BuscarPorCodigo(_documentosEmpresaFilial, "LiberacaoPagamentoProvedor/BuscarPorCodigo", function (arg) {
        PreencherObjetoDocumentosProvedor(arg.Data.ListaDocumentosProvedor, arg.Data);

        preencherAprovacao(arg);
        SetarEtapasPagamentoProvedor(arg.Data.Status, arg.Data.Situacao);
        ValidarCampos(arg.Data.Situacao, arg.Data.Status);
        _anexo.Anexos.val(arg.Data.Anexos.slice());

        carregarCargasSelecionadas(arg.Data.Codigo);

        AtualizarGridRegrasAutorizacaoLiberacaoPagamentoProvedor();

        _regraPagamentoProvedor.Codigo.val(arg.Data.CodigoPagamentoProvedor);
        _regraPagamentoProvedor.CodigoAprovacaoAlcadaRegra.val(arg.Data.CodigoAprovacaoAlcadaRegra);
        _gridRegrasLiberacaoPagamentoProvedor.CarregarGrid();

        _gridDetalhesCarga.CarregarGrid();
    }, null);
}

function AtualizarGridRegrasAutorizacaoLiberacaoPagamentoProvedor() {
    _gridRegrasLiberacaoPagamentoProvedor.CarregarGrid(function (arg) {
        var exibeBtn = false;
        arg.data.forEach(function (row) {
            if (row.PodeAprovar)
                exibeBtn = true;
        });

        _justificativaAprovacaoRegra.Aprovar.visible(exibeBtn);
    });
}

function recarregarGridPesquisaLiberacaoPagamentoProvedor() {
    _gridLiberacaoPagamentoProvedor.CarregarGrid();
}

function cancelarClick() {
    exibirConfirmacao("Confirmar Cancelamento", "Deseja realmente cancelar o pagamento selecionado?", function () {

        var data = {            
            Codigo: _documentosProvedor.CodigoPagamentoProvedor.val()
        }
        executarReST("LiberacaoPagamentoProvedor/CancelarPagamentoProvedor", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelado com sucesso");

                    _gridDetalhesCarga.CarregarGrid();
                    _gridDetalhesCarga.SetarPermissaoSelecionarSomenteUmRegistro(true);
                    _documentosEmpresaFilial.SelecionarTodos.visible(false);

                    limparCamposDocumentoEmpresaFilial();
                    SetarCamposPadroes();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function limparClick() {
    _gridDetalhesCarga.CarregarGrid();
    _gridDetalhesCarga.SetarPermissaoSelecionarSomenteUmRegistro(true);
    _documentosEmpresaFilial.SelecionarTodos.visible(false);

    limparCamposDocumentoEmpresaFilial();
    SetarCamposPadroes();
}

function limparCamposDocumentoEmpresaFilial() {
    LimparCampos(_documentosEmpresaFilial);
    LimparCampos(_documentosProvedor);

    _importarXML.Arquivo.val("");
    _importarPDF.ArquivoPDF.val("");

    SetarEtapasPagamentoProvedor(EnumEtapaLiberacaoPagamentoProvedor.DetalhesCarga, 0);
}

function ValidarCampos(situacao, etapa) {
    validarEtapa(etapa);

    if (situacao == EnumSituacaoLiberacaoPagamentoProvedor.Finalizada || situacao == EnumSituacaoLiberacaoPagamentoProvedor.Rejeitada || situacao == EnumSituacaoLiberacaoPagamentoProvedor.Cancelada) {
        _documentosEmpresaFilial.TipoDocumentoProvedor.enable(false);
        _documentosEmpresaFilial.LocalidadePrestacao.visible(false);
        _documentosEmpresaFilial.ConfirmarDocumentosEmpresaFilial.visible(false);
        _documentosEmpresaFilial.Tomador.visible(false);
        _documentosEmpresaFilial.TransportadorProvedor.visible(false);
        _documentosEmpresaFilial.NumeroCarga.visible(false);

        _documentosProvedor.DataEmissao.enable(false);
        _documentosProvedor.NumeroNFS.enable(false);
        _documentosProvedor.ValorTotalProvedor.enable(false);
        _documentosProvedor.ConfirmarDocumentosProvedor.visible(false);
        _documentosProvedor.ArquivoXML.visible(false);
        _documentosProvedor.ArquivoPDF.visible(false);
        _importarXML.Arquivo.val("");
        _importarPDF.ArquivoPDF.val("");

        $("#importacaoDocumentosProvedor").hide();
    }
    if (situacao == EnumSituacaoLiberacaoPagamentoProvedor.Cancelada || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor) {
        _documentosEmpresaFilial.Cancelar.visible(false);
        _CRUDAprovacao.Cancelar.visible(false);
        _documentosProvedor.Cancelar.visible(false);
        _liberacaoPagamentoProvedor.Cancelar.visible(false);
    }
    else {
        _documentosEmpresaFilial.Cancelar.visible(true);
        _CRUDAprovacao.Cancelar.visible(true);
        _documentosProvedor.Cancelar.visible(true);
        _liberacaoPagamentoProvedor.Cancelar.visible(true);
    }
}

function SetarCamposPadroes() {
    _documentosEmpresaFilial.ConfirmarDocumentosEmpresaFilial.enable(true);
    _documentosEmpresaFilial.ConfirmarDocumentosEmpresaFilial.visible(true);
    _documentosEmpresaFilial.Tomador.visible(true);
    _documentosEmpresaFilial.TransportadorProvedor.visible(true);
    _documentosEmpresaFilial.NumeroCarga.visible(true);
    _documentosEmpresaFilial.Tomador.enable(true);
    _documentosEmpresaFilial.TransportadorProvedor.enable(true);
    _documentosEmpresaFilial.NumeroCarga.enable(true);
    _documentosEmpresaFilial.TipoDocumentoProvedor.enable(true);
    _documentosEmpresaFilial.LocalidadePrestacao.enable(true);

    _documentosProvedor.DataEmissao.enable(true);
    _documentosProvedor.NumeroNFS.enable(true);
    _documentosProvedor.ValorTotalProvedor.enable(true);
    _documentosProvedor.ArquivoPDF.visible(true);
    _documentosProvedor.ArquivoXML.visible(true);
    _documentosProvedor.ArquivoPDF.enable(true);
    _documentosProvedor.ArquivoXML.enable(true);
    _documentosProvedor.Anexo.enable(true);
    _documentosProvedor.ConfirmarDocumentosProvedor.enable(true);
    _documentosProvedor.ConfirmarDocumentosProvedor.visible(true);
    _documentosEmpresaFilial.Cancelar.visible(false);
    _CRUDAprovacao.Cancelar.visible(false);
    _documentosProvedor.Cancelar.visible(false);
    _liberacaoPagamentoProvedor.Cancelar.visible(false);

    $("#importacaoDocumentosProvedor").show();
}

function validarEtapa(etapa) {
    if (etapa != EnumEtapaLiberacaoPagamentoProvedor.DetalhesCarga) {
        _documentosEmpresaFilial.ConfirmarDocumentosEmpresaFilial.enable(false);
        _documentosEmpresaFilial.TipoDocumentoProvedor.enable(false);
        _documentosEmpresaFilial.LocalidadePrestacao.enable(false);
        _documentosEmpresaFilial.Tomador.enable(false);
        _documentosEmpresaFilial.TransportadorProvedor.enable(false);
        _documentosEmpresaFilial.NumeroCarga.enable(false);

    }

    if (etapa != EnumEtapaLiberacaoPagamentoProvedor.DocumentoProvedor) {
        _documentosProvedor.ConfirmarDocumentosProvedor.enable(false);
        _documentosProvedor.DataEmissao.enable(false);
        _documentosProvedor.NumeroNFS.enable(false);
        _documentosProvedor.ValorTotalProvedor.enable(false);
        _documentosProvedor.ArquivoPDF.enable(false);
        _documentosProvedor.ArquivoXML.enable(false);
        _documentosProvedor.Anexo.enable(false);
    }
}

function preencherAprovacao(arg) {
    if (arg.Data.EtapaLiberacaoPagamentoProvedor == 3 || arg.Data.EtapaLiberacaoPagamentoProvedor == 4) {
        PreencherObjetoKnout(_detalheDocumentoRecebido, { Data: arg.Data.ListaAprovacao.DetalhesDocumentoRecebido });
        PreencherObjetoKnout(_detalheDocumento, { Data: arg.Data.ListaAprovacao.DetalhesDocumento });
        verificarTratarDivergencias(arg.Data.ListaAprovacao.DetalhesDocumento, _detalheDocumento, _detalheDocumentoRecebido, true);
    }
}

function visibilidadeGRIDDetalhesCarga(tipoDocumentoProvedorDiferenteNFSe) {
    _gridDetalhesCarga.SetarPermissaoSelecionarSomenteUmRegistro(tipoDocumentoProvedorDiferenteNFSe);
    _gridDetalhesCarga.AtualizarRegistrosSelecionados([]);
}

function visibilidadeDocumentosProvedor(tipoDocumentoProvedorDiferenteNFSe) {
    if (!tipoDocumentoProvedorDiferenteNFSe) {
        _documentosProvedor.DataEmissao.visible(true);
        _documentosProvedor.DataEmissao.required(true);

        _documentosProvedor.NumeroNFS.visible(true);
        _documentosProvedor.NumeroNFS.required(true);

        _documentosProvedor.ValorTotalProvedor.visible(true);
        _documentosProvedor.ValorTotalProvedor.required(true);
    } else {
        _documentosProvedor.DataEmissao.visible(false);
        _documentosProvedor.DataEmissao.required(false);

        _documentosProvedor.NumeroNFS.visible(false);
        _documentosProvedor.NumeroNFS.required(false);

        _documentosProvedor.ValorTotalProvedor.visible(false);
        _documentosProvedor.ValorTotalProvedor.required(false);
    }
}

function visibilidadeDocumentosEmpresaFilial(tipoDocumentoProvedorDiferenteNFSe) {
    _documentosEmpresaFilial.SelecionarTodos.visible(!tipoDocumentoProvedorDiferenteNFSe);

    _documentosEmpresaFilial.LocalidadePrestacao.visible(!tipoDocumentoProvedorDiferenteNFSe);
    _documentosEmpresaFilial.LocalidadePrestacao.required(!tipoDocumentoProvedorDiferenteNFSe);
    _documentosEmpresaFilial.LocalidadePrestacao.codEntity(0);
    _documentosEmpresaFilial.LocalidadePrestacao.val("");
}

function carregarCargasSelecionadas(codigo) {
    var listaCodigos = new Array();

    for (var i = 0; i < codigo.length; i++) {
        listaCodigos.push(codigo[i]);
    }

    _documentosEmpresaFilial.Codigo.val(JSON.stringify(listaCodigos));
}

function recarregarDetalhesCarga() {
    if (_documentosEmpresaFilial.Codigo.val().length == 0)
        _gridDetalhesCarga.CarregarGrid();
}