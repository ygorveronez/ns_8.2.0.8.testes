/// <reference path="../../Enumeradores/EnumSituacaoCompraPallets.js" />
/// <reference path="../../../js/Global/CRUD.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _compraPallets;
var _pesquisaCompraPallets;
var _gridCompraPallets;
var _fileImportacao;
var _isTMS = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS;
/*
 * Declaração das Classes
 */

var CompraPallets = function () {
    var self = this;

    /* 
     * Campos da compra de pallets
     */
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid(), enable: ko.observable(true), required: !_isTMS, visible: !_isTMS });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", idBtnSearch: guid(), enable: ko.observable(true), required: true });
    this.Numero = PropertyEntity({ text: "*Número:", getType: typesKnockout.int, enable: ko.observable(true), required: true });
    this.Numero.configInt.thousands = '';
    this.Quantidade = PropertyEntity({ text: "*Quantidade:", enable: ko.observable(true), getType: typesKnockout.int, enable: ko.observable(true), required: true });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Setor:", idBtnSearch: guid(), enable: ko.observable(true), visible: !_isTMS });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa/Filial:", idBtnSearch: guid(), enable: ko.observable(true), required: _isTMS, visible: _isTMS });
    this.Valor = PropertyEntity({ text: "*Valor Unitário:", getType: typesKnockout.decimal, enable: ko.observable(true), required: true });

    /* 
     * Campos do resumo da compra de pallets
     */
    this.Resumo = PropertyEntity({ visible: ko.observable(false), type: types.local });
    this.FilialResumo = PropertyEntity({ text: "Filial:", val: ko.observable(""), visible: !_isTMS });
    this.FornecedorResumo = PropertyEntity({ text: "Fornecedor:", val: ko.observable("") });
    this.QuantidadeResumo = PropertyEntity({ text: "Quantidade:", val: ko.observable("") });
    this.SetorResumo = PropertyEntity({ text: "Setor:", val: ko.observable(""), visible: !_isTMS });
    this.SituacaoResumo = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.TransportadorResumo = PropertyEntity({ text: "Empresa/Filial:", val: ko.observable(""), visible: _isTMS });
    this.ValorUnitarioResumo = PropertyEntity({ text: "Valor Unitário:", val: ko.observable("") });
    this.ValorTotalResumo = PropertyEntity({ text: "Valor Total:", val: ko.observable("") });

    /* 
     * Eventos para atualizar os campos do resumo quando os campos da compra de pallets são alterados
     */
    this.Filial.val.subscribe(function (valor) { self.FilialResumo.val(valor); });
    this.Fornecedor.val.subscribe(function (valor) { self.FornecedorResumo.val(valor); });
    this.Quantidade.val.subscribe(function (valor) { self.QuantidadeResumo.val(valor); atualizarValorTotalResumo(); });
    this.Setor.val.subscribe(function (valor) { self.SetorResumo.val(valor); });
    this.Situacao.val.subscribe(function (valor) { self.SituacaoResumo.val(EnumSituacaoCompraPallets.obterDescricao(valor)); });
    this.Transportador.val.subscribe(function (valor) { self.TransportadorResumo.val(valor); });
    this.Valor.val.subscribe(function (valor) { self.ValorUnitarioResumo.val(valor); atualizarValorTotalResumo(); });

    /*
     * Botões
     */
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.DownloadXML = PropertyEntity({ eventClick: downloadXmlClick, type: types.event, text: "Download XML", visible: ko.observable(false) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar / Nova", visible: ko.observable(false) });
    this.ImportarXML = PropertyEntity({ eventClick: abrirImportacaoXmlClick, eventChange: importarXMLOnChange, type: types.event, text: "Importar XML NF-e" });
}

var PesquisaCompraPallets = function () {
    this.DataInicio = PropertyEntity({ text: "Data Início:", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Fim:", getType: typesKnockout.date });
    this.DataFim.dateRangeInit = this.DataInicio;
    this.DataInicio.dateRangeLimit = this.DataFim;

    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int });
    this.Numero.configInt.thousands = '';
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: !_isTMS });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoCompraPallets.obterOpcoes(), def: "", text: "Situação:" });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), visible: _isTMS });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCompraPallets.CarregarGrid();
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
}

/*
 * Declaração das Funções de Inicialização
 */

function loadCompraPallets() {
    _pesquisaCompraPallets = new PesquisaCompraPallets();
    KoBindings(_pesquisaCompraPallets, "knockoutPesquisaCompraPallets", false, _pesquisaCompraPallets.Pesquisar.id);

    _compraPallets = new CompraPallets();
    KoBindings(_compraPallets, "knockoutCompraPallets");

    HeaderAuditoria("CompraPallets", _compraPallets);

    new BuscarClientes(_pesquisaCompraPallets.Fornecedor);
    new BuscarClientes(_compraPallets.Fornecedor);
    new BuscarFilial(_pesquisaCompraPallets.Filial);
    new BuscarFilial(_compraPallets.Filial);
    new BuscarSetorFuncionario(_compraPallets.Setor);
    new BuscarTransportadores(_pesquisaCompraPallets.Transportador);
    new BuscarTransportadores(_compraPallets.Transportador);

    loadGridCompraPallets();

    _fileImportacao = _compraPallets.ImportarXML.get$()[0];
}

function loadGridCompraPallets() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCompraPalletsClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar]};

    _gridCompraPallets = new GridView(_pesquisaCompraPallets.Pesquisar.idGrid, "CompraPallets/Pesquisa", _pesquisaCompraPallets, menuOpcoes, null);
    _gridCompraPallets.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function abrirImportacaoXmlClick() {
    _compraPallets.ImportarXML.get$().click();
}

function adicionarClick(e, sender) {
    Salvar(_compraPallets, "CompraPallets/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridCompraPallets.CarregarGrid();
                limparCamposCompraPallets();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar a compra?", function () {
        Salvar(_compraPallets, "CompraPallets/CancelarPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCompraPallets.CarregarGrid();
                    limparCamposCompraPallets();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function downloadXmlClick() {
    executarDownload("CompraPallets/DownloadXML", { Codigo: e.Codigo.val() });
}

function editarCompraPalletsClick(itemGrid) {
    editarCompraPallets(itemGrid.Codigo);
}

function finalizarClick(e, sender) {
    Salvar(_compraPallets, "CompraPallets/Finalizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Finalizado com sucesso");
                _gridCompraPallets.CarregarGrid();
                limparCamposCompraPallets();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function limparClick() {
    limparCamposCompraPallets();
}

function importarXMLOnChange() {
    var count = _fileImportacao.files.length;
    if (count > 0)
        exibirConfirmacao("Importar XML", "Tem certeza que deseja importar " + count + " arquivo(s)?", importarXmlCompras);
}

/*
 * Declaração das Funções
 */

function atualizarValorTotalResumo() {
    var valorTotal = _compraPallets.ValorUnitarioResumo.val().replace(/\./g, '').replace(",", ".");

    if (isNaN(valorTotal))
        valorTotal = 0;

    _compraPallets.ValorTotalResumo.val(Globalize.format(_compraPallets.QuantidadeResumo.val() * valorTotal, "n2"));
}

function controlarCampos() {
    if (_compraPallets.Situacao.val() == 0) {
        _compraPallets.Numero.enable(true);
        _compraPallets.Fornecedor.enable(true);
        _compraPallets.Quantidade.enable(true);
        _compraPallets.Valor.enable(true);
        _compraPallets.Filial.enable(true);
        _compraPallets.Setor.enable(true);
        _compraPallets.Transportador.enable(true);

        _compraPallets.DownloadXML.visible(false);
        _compraPallets.Finalizar.visible(false);
        _compraPallets.Limpar.visible(false);
        _compraPallets.Cancelar.visible(false);
        _compraPallets.Adicionar.visible(true);
    } else {
        if (_compraPallets.Situacao.val() != EnumSituacaoCompraPallets.AgFinalizacao) {
            _compraPallets.Fornecedor.enable(false);
            _compraPallets.Filial.enable(false);
            _compraPallets.Quantidade.enable(false);
            _compraPallets.Setor.enable(false);
            _compraPallets.Transportador.enable(false);
            _compraPallets.Valor.enable(false);
        }

        if (_compraPallets.Situacao.val() == EnumSituacaoCompraPallets.AgFinalizacao) {
            _compraPallets.Finalizar.visible(true);
        }

        if (_compraPallets.Situacao.val() != EnumSituacaoCompraPallets.Cancelado) {
            _compraPallets.Cancelar.visible(true);
        }
        
        _compraPallets.Numero.enable(false);
        _compraPallets.Limpar.visible(true);
        _compraPallets.Adicionar.visible(false);
    }
}

function editarCompraPallets(codigo) {
    limparCamposCompraPallets();

    _compraPallets.Codigo.val(codigo);

    BuscarPorCodigo(_compraPallets, "CompraPallets/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaCompraPallets.ExibirFiltros.visibleFade(false);

                controlarCampos();

                _compraPallets.DownloadXML.visible(arg.Data.PossuiXML);
                _compraPallets.Resumo.visible(true);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function importarXmlCompras() {
    var fmData = new FormData();
    var count = s = _fileImportacao.files.length;
    for (var i = 0; i < count; i++)
        fmData.append("XML", _fileImportacao.files[i]);

    enviarArquivo("CompraPallets/ImportacaoXML", {}, fmData, function (arg) {
        limparFileImportacao();
        if (arg.Success) {
            if (arg.Data != null) {
                if (arg.Data.Erros == 0)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Documentos importado(s) com sucesso.");
                else
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (count - arg.Data.Erros) + " de " + count + " importado(s).");
                _gridCompraPallets.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function limparCamposCompraPallets() {
    LimparCampos(_compraPallets);
    limparFileImportacao();
    controlarCampos();
    _compraPallets.Resumo.visible(false);
}

function limparFileImportacao() {
    _fileImportacao.value = null;
}