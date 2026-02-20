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
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Cargas/Carga/DadosCarga/Carga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _atendimentoCliente;
var _pesquisaAtendimentoCliente;
var _gridAtendimentoCliente;
var _gridCargasDocumento;
var _gridManifestosDocumento;
var _gridFaturasDocumento;
var _gridOcorrenciasEntrega;
var _gridOcorrenciasDocumento;
var _gridCanhotos;
var _gridGuaritas;
var _gridTitulos;
var _modalOcorrenciaAtendimentoCliente;
var _modalFaturaAtendimentoCliente;
var _modalGuaritaAtendimentoCliente;

var AtendimentoCliente = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });

    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });
    this.CodigoCargaEmbarcador = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string, idFade: guid(), visibleFade: ko.observable(false) });

    this.Numero = PropertyEntity({ text: "Número: " });
    this.TipoDocumento = PropertyEntity({ text: "Tip. Doc.: " });
    this.NumeroNotas = PropertyEntity({ text: "Nº Notas: " });
    this.NumerosSolicitacoes = PropertyEntity({ text: "Nº Solicitação(ões): " });
    this.NumeroPedidoCF = PropertyEntity({ text: "Nº Pedido(s) Cliente: " });
    this.DataEmissao = PropertyEntity({ text: "Data Emissão: " });
    this.DataPrevisao = PropertyEntity({ text: "Data Previsão Entrega: " });
    this.Remetente = PropertyEntity({ text: "Remetente: " });
    this.Destinatario = PropertyEntity({ text: "Destinatário: " });
    this.Transportador = PropertyEntity({ text: "Transportador: " });
    this.Observacao = PropertyEntity({ text: "Observação: " });
    this.RecebedorNF = PropertyEntity({ text: "Recebedor(es): " });
    this.DestinatarioNF = PropertyEntity({ text: "Destinatário(s): " });

    this.Cargas = PropertyEntity({ text: "Cargas: ", idGrid: guid() });
    this.Manifestos = PropertyEntity({ text: "Manifestos: ", idGrid: guid() });
    this.Faturas = PropertyEntity({ text: "Faturas: ", idGrid: guid() });
    this.Canhotos = PropertyEntity({ text: "Canhotos: ", idGrid: guid() });
    this.Guaritas = PropertyEntity({ text: "Guaritas: ", idGrid: guid() });
    this.Titulos = PropertyEntity({ text: "Títulos: ", idGrid: guid() });

    this.OcorrenciasEntrega = PropertyEntity({ text: "Ocorrências de Entrega: ", idGrid: guid() });
    this.Ocorrencias = PropertyEntity({ text: "Ocorrências: ", idGrid: guid() });
    this.NovaOcorrencia = PropertyEntity({ eventClick: NovaOcorrenciaClick, type: types.event, text: "Nova Ocorrência", visible: ko.observable(true) });

    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar", visible: ko.observable(false) });
    this.GerarImpressao = PropertyEntity({ eventClick: gerarImpressaoClick, type: types.event, text: "Gerar Impressão", visible: ko.observable(false) });
};

var PesquisaAtendimentoCliente = function () {
    this.NumeroNotaDe = PropertyEntity({ text: "Núm. NF-e:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.string });
    //this.NumeroNotaAte = PropertyEntity({ text: "Até:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.SerieNota = PropertyEntity({ text: "Série NF:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.DataEmissaoNotaDe = PropertyEntity({ text: "Emissão NF de:", getType: typesKnockout.date });
    this.DataEmissaoNotaAte = PropertyEntity({ text: "Até:", getType: typesKnockout.date });
    this.DataEmissaoNotaDe.dateRangeLimit = this.DataEmissaoNotaAte;
    this.DataEmissaoNotaAte.dateRangeInit = this.DataEmissaoNotaDe;
    this.EmpresaOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa Origem:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.NumeroCTeDe = PropertyEntity({ text: "Nº CT-e de:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.NumeroCTeAte = PropertyEntity({ text: "Até:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.SerieCTe = PropertyEntity({ text: "Série CT-e:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.DataEmissaoCTeDe = PropertyEntity({ text: "Emissão CT-e de:", getType: typesKnockout.date });
    this.DataEmissaoCTeAte = PropertyEntity({ text: "Até:", getType: typesKnockout.date });
    this.DataEmissaoCTeDe.dateRangeLimit = this.DataEmissaoCTeAte;
    this.DataEmissaoCTeAte.dateRangeInit = this.DataEmissaoCTeDe;
    this.EmpresaDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa Destino:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.CidadeOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cidade Origem:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CidadeDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cidade Destino:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroFaturaDe = PropertyEntity({ text: "Nº Fatura de:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.NumeroFaturaAte = PropertyEntity({ text: "Até:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.NumeroPedidoDe = PropertyEntity({ text: "Nº Pedido:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });

    this.NumeroSolicitacao = PropertyEntity({ text: "Nº Solicitação:", maxlength: 100, enable: ko.observable(true), getType: typesKnockout.string });
    this.NumeroPedidoCF = PropertyEntity({ text: "Nº Pedido Cliente:", maxlength: 100, enable: ko.observable(true), getType: typesKnockout.string });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Recebedor:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.NumeroCarga = PropertyEntity({ text: "Nº Carga:", maxlength: 100, enable: ko.observable(true), getType: typesKnockout.string });
    this.NumeroPreFatura = PropertyEntity({ text: "Nº Pré-Fatura:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Nº Pedido Embarcador:", maxlength: 100, enable: ko.observable(true), getType: typesKnockout.string });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas da Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo da Carga:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            limparCamposAtendimentoCliente();
            _gridAtendimentoCliente.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal  fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var DetalheCarga = function () {
    this.Pedido = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.Origem = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.PesoTotal = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.NumeroPaletes = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.ProdutoPredominante = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });
    this.PesoTotalPaletes = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.ValorFrete = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
};

//*******EVENTOS*******
function loadAtendimentoCliente() {
    //-- Knouckout
    // Instancia pesquisa
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {

            _pesquisaAtendimentoCliente = new PesquisaAtendimentoCliente();
            KoBindings(_pesquisaAtendimentoCliente, "knockoutPesquisaAtendimentoCliente", false, _pesquisaAtendimentoCliente.Pesquisar.id);

            new BuscarEmpresa(_pesquisaAtendimentoCliente.EmpresaOrigem);
            new BuscarEmpresa(_pesquisaAtendimentoCliente.EmpresaDestino);
            new BuscarLocalidades(_pesquisaAtendimentoCliente.CidadeOrigem);
            new BuscarLocalidades(_pesquisaAtendimentoCliente.CidadeDestino);
            new BuscarClientes(_pesquisaAtendimentoCliente.Cliente);
            new BuscarClientes(_pesquisaAtendimentoCliente.Remetente);
            new BuscarClientes(_pesquisaAtendimentoCliente.Destinatario);
            new BuscarClientes(_pesquisaAtendimentoCliente.Recebedor);
            new BuscarGruposPessoas(_pesquisaAtendimentoCliente.GrupoPessoa);
            new BuscarTiposOperacao(_pesquisaAtendimentoCliente.TipoOperacao);
            new BuscarVeiculos(_pesquisaAtendimentoCliente.Veiculo);
            new BuscarMotoristas(_pesquisaAtendimentoCliente.Motorista);
            new BuscarTiposdeCarga(_pesquisaAtendimentoCliente.TipoCarga);

            // Instancia objeto principal
            _atendimentoCliente = new AtendimentoCliente();
            KoBindings(_atendimentoCliente, "knockoutAtendimentoCliente");

            // Inicia busca
            buscarAtendimentoCliente();

            //Carregar grids
            GridFaturasDocumento();
            GridOcorrenciasDocumento();
            GridOcorrenciasEntrega();
            GridCargasDocumento();
            GridManifestosDocumento();
            GridCanhoto();
            GridGuarita();
            GridTitulo();

            _pesquisaOcorrencia = new PesquisaOcorrencia();
            carregarLancamentoOcorrencia("conteudoOcorrencia", "modaisOcorrencia");
            carregarGuaritaTMS();

            _modalOcorrenciaAtendimentoCliente = new bootstrap.Modal(document.getElementById("divModalOcorrencia"), { backdrop: 'static', keyboard: true });
            $('#divModalOcorrencia').on('hidden.bs.modal', function () {
                _gridOcorrenciasDocumento.CarregarGrid();
            });

            _pesquisaFatura = new PesquisaFatura();
            carregarLancamentoFatura("conteudoFatura", function () {
                _modalFaturaAtendimentoCliente = new bootstrap.Modal(document.getElementById("divModalFatura"), { backdrop: 'static', keyboard: true });
            });

            _modalGuaritaAtendimentoCliente = new bootstrap.Modal(document.getElementById("divModalGuarita"), { backdrop: 'static', keyboard: true });
        });
    });
}

function carregarGuaritaTMS() {
    $.get("Content/Static/Logistica/GuaritaTMS.html?dyn=" + guid(), function (dataConteudo) {
        $("#conteudoGuarita").html(dataConteudo);

        _guaritaTMS = new GuaritaTMS();
        KoBindings(_guaritaTMS, "knockoutCadastroGuaritaTMS");

        _CRUDGuaritaTMS = new CRUDGuaritaTMS();
        KoBindings(_CRUDGuaritaTMS, "knockoutCRUDGuaritaTMS");

        loadGuaritaTMSReboque();
    });
}

function NovaOcorrenciaClick(e, sender) {
    if (_atendimentoCliente.CodigoCarga.val() > 0) {
        limparCamposOcorrencia();
        var data = { Codigo: _atendimentoCliente.CodigoCarga.val(), CodigoCargaEmbarcador: _atendimentoCliente.CodigoCargaEmbarcador.val() };
        _ocorrencia.Conhecimento.val(_atendimentoCliente.Codigo.val());
        _ocorrencia.Conhecimento.codEntity(_atendimentoCliente.Codigo.val());
        _ocorrencia.Chamado.codEntity(0);
        _ocorrencia.Chamado.val("0");
        _ocorrencia.Chamado.codEntity(0);
        _ocorrencia.Chamado.visible(false);
        _ocorrencia.NaoLimparCarga.val(true);
        _ocorrencia.Carga.enable(false);

        retornoCarga(data, function () {
            _modalOcorrenciaAtendimentoCliente.show();
        });
    } else {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Nenhuma carga vinculada ao conhecimento");
    }
}

function gerarImpressaoClick(e, sender) {
    executarDownload("AtendimentoCliente/Imprimir", { Codigo: _atendimentoCliente.Codigo.val() });
}

function limparClick(e) {
    limparCamposAtendimentoCliente();
}

function editarAtendimentoClienteClick(itemGrid) {
    // Limpa os campos
    limparCamposAtendimentoCliente();

    // Seta o codigo do objeto
    _atendimentoCliente.Codigo.val(itemGrid.CodigoCTe);

    // Busca informacoes para edicao
    BuscarPorCodigo(_atendimentoCliente, "AtendimentoCliente/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaAtendimentoCliente.ExibirFiltros.visibleFade(false);
                _atendimentoCliente.Codigo.visibleFade(true);

                // Alternas os campos de CRUD
                _atendimentoCliente.GerarImpressao.visible(true);
                _atendimentoCliente.Limpar.visible(true);

                _gridFaturasDocumento.CarregarGrid();
                _gridOcorrenciasDocumento.CarregarGrid();
                _gridOcorrenciasEntrega.CarregarGrid();
                _gridCargasDocumento.CarregarGrid();
                _gridManifestosDocumento.CarregarGrid();
                _gridCanhotos.CarregarGrid();
                _gridGuaritas.CarregarGrid();
                _gridTitulos.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function DetalheFaturaSACClick(data) {
    editarFatura(data);
    _modalFaturaAtendimentoCliente.show();
}

function DetalheCargaClick(carga) {
    var data = { Carga: carga.Codigo };
    executarReST("Carga/BuscarCargaPorCodigo", data, function (r) {
        if (r.Success) {
            if (r.Data) {

                $("#fdsCargaSAC").html('<button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true" style="position: absolute; z-index: 9999; right: 18px;">×</button>');
                _ocorrenciaAtual = null;
                var knoutCarga = GerarTagHTMLDaCarga("fdsCargaSAC", r.Data, false);
                $("#" + knoutCarga.DivCarga.id).attr("class", "p-2");
                _cargaAtual = knoutCarga;

                Global.abrirModal("divModalDetalhesCarga");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function DetalheOcorrenciaClick(data) {
    limparCamposOcorrencia();
    _ocorrencia.Codigo.val(data.Codigo);
    buscarOcorrenciaPorCodigo(function () {
        _modalOcorrenciaAtendimentoCliente.show();
    });
}

//*******MÉTODOS*******
function buscarAtendimentoCliente() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Selecionar", id: guid(), evento: "onclick", metodo: editarAtendimentoClienteClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridAtendimentoCliente = new GridView(_pesquisaAtendimentoCliente.Pesquisar.idGrid, "AtendimentoCliente/Pesquisa", _pesquisaAtendimentoCliente, menuOpcoes, null);
    _gridAtendimentoCliente.CarregarGrid();
}

function VisibilidadeFatura(dataRow) {
    return dataRow.PossuiPermissaoFatura;
}

function GridFaturasDocumento() {
    //-- Grid
    // Opcoes
    var detalhe = { descricao: "Detalhe", id: guid(), evento: "onclick", metodo: DetalheFaturaSACClick, tamanho: "10", icone: "", visibilidade: VisibilidadeFatura };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        tamanho: 7,
        opcoes: [detalhe]
    };

    _gridFaturasDocumento = new GridView(_atendimentoCliente.Faturas.idGrid, "AtendimentoCliente/FaturasDocumento", _atendimentoCliente, menuOpcoes);
}

function VisibilidadeOcorrencia(dataRow) {
    return dataRow.PossuiPermissaoOcorrencia;
}

function GridOcorrenciasEntrega() {
    //-- Grid
    // Opcoes
    _gridOcorrenciasEntrega = new GridView(_atendimentoCliente.OcorrenciasEntrega.idGrid, "AtendimentoCliente/OcorrenciasEntrega", _atendimentoCliente, null);
}

function GridOcorrenciasDocumento() {
    //-- Grid
    // Opcoes
    var detalhe = { descricao: "Detalhe", id: guid(), evento: "onclick", metodo: DetalheOcorrenciaClick, tamanho: "10", icone: "", visibilidade: VisibilidadeOcorrencia };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        tamanho: 7,
        opcoes: [detalhe]
    };

    _gridOcorrenciasDocumento = new GridView(_atendimentoCliente.Ocorrencias.idGrid, "AtendimentoCliente/OcorrenciasDocumento", _atendimentoCliente, menuOpcoes);
}

function VisibilidadeCarga(dataRow) {
    return dataRow.PossuiPermissaoCarga;
}

function GridCargasDocumento() {
    //-- Grid
    // Opcoes
    var detalhe = { descricao: "Detalhe", id: guid(), evento: "onclick", metodo: DetalheCargaClick, tamanho: "10", icone: "", visibilidade: VisibilidadeCarga };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        tamanho: 7,
        opcoes: [detalhe]
    };

    _gridCargasDocumento = new GridView(_atendimentoCliente.Cargas.idGrid, "AtendimentoCliente/CargasDocumento", _atendimentoCliente, menuOpcoes);
}

function GridManifestosDocumento() {
    _gridManifestosDocumento = new GridView(_atendimentoCliente.Manifestos.idGrid, "AtendimentoCliente/ManifestosDocumento", _atendimentoCliente);
}

function GridCanhoto() {
    //-- Grid
    // Opcoes
    var imagem = { descricao: "Imagem", id: guid(), evento: "onclick", metodo: DownloadImagemCanhotoClick, tamanho: "10", icone: "", visibilidade: VisibilidadeImagemCanhoto };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        tamanho: 7,
        opcoes: [imagem]
    };

    _gridCanhotos = new GridView(_atendimentoCliente.Canhotos.idGrid, "AtendimentoCliente/CanhotosDocumento", _atendimentoCliente, menuOpcoes);
}

function VisibilidadeImagemCanhoto(dataRow) {
    return dataRow.GuidNomeArquivo !== "";
}

function DownloadImagemCanhotoClick(canhoto) {
    if (canhoto.Codigo > 0 && canhoto.GuidNomeArquivo !== "") {
        var dados = {
            Codigo: canhoto.CodigoCanhoto
        };
        executarDownload("Canhoto/DownloadCanhoto", dados);
    } else {
        exibirMensagem(tipoMensagem.aviso, "Conhoto não enviado", "Não foi enviado o canhoto para este documento.");
    }
}

function GridGuarita() {
    //-- Grid
    // Opcoes
    var detalhe = { descricao: "Detalhe", id: guid(), evento: "onclick", metodo: DetalheGuaritaClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        tamanho: 7,
        opcoes: [detalhe]
    };

    _gridGuaritas = new GridView(_atendimentoCliente.Guaritas.idGrid, "AtendimentoCliente/GuaritasDocumento", _atendimentoCliente, menuOpcoes);
}

function GridTitulo() {
    _gridTitulos = new GridView(_atendimentoCliente.Titulos.idGrid, "AtendimentoCliente/TitulosDocumento", _atendimentoCliente);
}

function DetalheGuaritaClick(guarita) {
    limparCamposguaritaTMS();
    _guaritaTMS.Codigo.val(guarita.Codigo);
    buscarGuaritaPorCodigo(function () {
        _modalGuaritaAtendimentoCliente.show();
    });
}

function buscarGuaritaPorCodigo(callback) {
    callback = callback || function () { };

    BuscarPorCodigo(_guaritaTMS, "GuaritaTMS/BuscarPorCodigo", function (arg) {
        SetarEnableCamposKnockout(_guaritaTMS, false);
        SetarEnableCamposKnockout(_guaritaTMSReboque, false);

        _CRUDGuaritaTMS.Atualizar.visible(false);
        _CRUDGuaritaTMS.Cancelar.visible(false);
        _CRUDGuaritaTMS.Excluir.visible(false);
        _CRUDGuaritaTMS.Adicionar.visible(false);

        RecarregarGridGuaritaTMSReboque();
        _gridGuaritaTMSReboque.DesabilitarOpcoes();
        _guaritaTMS.AlterarReboquesVeiculo.val(false);

        callback();
    }, function (arg) {
        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        limparCamposguaritaTMS();
    });
}

function limparCamposAtendimentoCliente() {
    _pesquisaAtendimentoCliente.ExibirFiltros.visibleFade(true);
    _atendimentoCliente.Codigo.visibleFade(false);
    _atendimentoCliente.GerarImpressao.visible(false);
    _atendimentoCliente.Limpar.visible(false);
    LimparCampos(_atendimentoCliente);
}