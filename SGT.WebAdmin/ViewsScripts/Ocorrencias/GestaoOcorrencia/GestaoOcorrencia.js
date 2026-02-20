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
/// <reference path="../../Consultas/CargaEntrega.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/NotaFiscal.js" />
/// <reference path="../../Consultas/TiposCausadoresOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumTipoColetaEntregaDevolucao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoChamado.js" />
/// <reference path="GestaoOcorrenciaEtapas.js" />
/// <reference path="GestaoOcorrenciaAtendimento.js" />
/// <reference path="GestaoOcorrenciaAnexo.js" />
/// <reference path="GestaoOcorrenciaNotaFiscal.js" />
/// <reference path="../../Cargas/ControleEntregaDevolucao/ControleEntregaDevolucao.js" />

var _gestaoOcorrencia,
    _pesquisaGestaoOcorrencia,
    _gridPesquisaGestaoOcorrencia,
    _controleEntregaDevolucao,
    _sobras,
    _gridSobras;

var _buscouPorCodigo = false;

var PesquisaGestaoOcorrencia = function () {
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "", text: "Data Inicial:" });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "", text: "Data Final:" });

    this.DataFinal.dateRangeInit = this.DataInicial;
    this.DataInicial.dateRangeLimit = this.DataFinal;

    this.NotaFiscal = PropertyEntity({ text: "Nota Fiscal da Entrega:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroCarga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: "Número Carga:" });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Pesquisar = PropertyEntity({ eventClick: pesquisarGestaoOcorrencia, type: types.event, text: "Pesquisar", visible: ko.observable(true), idGrid: guid() });
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaGestaoOcorrencia.Visible.visibleFade()) {
                _pesquisaGestaoOcorrencia.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaGestaoOcorrencia.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus")
    });
}

var GestaoOcorrencia = function () {
    this.Codigo = PropertyEntity({ def: 0, getType: typesKnockout.int, val: ko.observable(0) });
    this.CodigoChamado = PropertyEntity({ def: 0, getType: typesKnockout.int, val: ko.observable(0) });
    this.SituacaoChamado = PropertyEntity({ type: typesKnockout.options, options: EnumSituacaoChamado.obterOpcoes(), val: ko.observable(EnumSituacaoChamado.Aberto), def: EnumSituacaoChamado.Aberto });

    this.Carga = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: "*Carga:", required: true, idBtnSearch: guid(), enable: ko.observable(true) });
    this.CargaEntrega = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: "*Entrega:", required: true, idBtnSearch: guid(), enable: ko.observable(true) });

    this.CargaEntrega.codEntity.subscribe((valor) => {
        _controleEntregaDevolucao.limpar();
    });

    this.Carga.codEntity.subscribe((valor) => {
        LimparCampoEntity(_gestaoOcorrencia.CargaEntrega);
        _controleEntregaDevolucao.limpar();
    });
    this.DataPrevisaoEntrega = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), def: "", text: "Previsão de Entrega:", enable: ko.observable(true), visible: ko.observable(false) });

    this.TipoOcorrencia = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: "*Tipo da Ocorrência:", required: true, idBtnSearch: guid(), enable: ko.observable(true) });

    this.TipoOcorrencia.codEntity.subscribe((valor) => {
        if (valor <= 0) {
            _gestaoOcorrencia.TipoDevolucao.visible(false);
        }
        if (valor > 0) {
            var codigoEntrega = _gestaoOcorrencia.CargaEntrega.codEntity();
            executarReST("GestaoOcorrencia/ObterPermissaoAlterarDataPrevisaoEntrega", { CodigoTipoOcorrencia: valor, CodigoEntrega: codigoEntrega }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        _gestaoOcorrencia.DataPrevisaoEntrega.visible(true);

                        if (retorno.Data.DataPrevisaoEntregaAtual != "") {
                            _gestaoOcorrencia.DataPrevisaoEntrega.val(retorno.Data.DataPrevisaoEntregaAtual);
                        }
                        else {
                            _gestaoOcorrencia.DataPrevisaoEntrega.val("");
                        }
                    }
                    else {
                        _gestaoOcorrencia.DataPrevisaoEntrega.val("");
                        _gestaoOcorrencia.DataPrevisaoEntrega.visible(false);
                    }
                } else {
                    _gestaoOcorrencia.DataPrevisaoEntrega.val("");
                    _gestaoOcorrencia.DataPrevisaoEntrega.visible(false);
                }
            });
        }
    });

    this.TipoDevolucao = PropertyEntity({ val: ko.observable(EnumTipoColetaEntregaDevolucao.Total), options: ko.observable(EnumTipoColetaEntregaDevolucao.obterOpcoes()), def: EnumTipoColetaEntregaDevolucao.Total, text: "Tipo de Devolução:", visible: ko.observable(false), enable: ko.observable(true), idFade: guid() });
    this.Anexo = PropertyEntity({ eventClick: gerenciarAnexosGestaoOcorrenciaClick, type: types.event, text: "Anexos", visible: ko.observable(true) });
    this.Observacoes = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Observações:", maxlength: 1000, enable: ko.observable(true) });
    this.TiposCausadoresOcorrencia = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: "Causador da Ocorrência:", required: false, idBtnSearch: guid(), enable: ko.observable(true) });

    this.NotasFiscais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid(), required: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarOcorrenciaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar", visible: ko.observable(true) });

    this.Sobras = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
}

var Sobras = function () {
    this.Codigo = PropertyEntity({ def: 0, getType: typesKnockout.int, val: ko.observable(0) });

    this.PermiteSobras = PropertyEntity({ val: ko.observable(false) });

    this.PermiteSobras.val.subscribe((novoValor) => {
        if (_buscouPorCodigo)
            _sobras.MostrarCamposSobra.visible(false);
        else
            _sobras.MostrarCamposSobra.visible(true);
    })

    this.MostrarCamposSobra = PropertyEntity({ eventClick: exibirCamposSobra, type: types.event, text: "Adicionar Sobra", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), icon: ko.observable("fal fa-plus") });
    this.GridSobra = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.CodigoSobra = PropertyEntity({ text: "Código do Produto:", val: ko.observable(""), def: "", visible: ko.observable(true), required: true });
    this.QuantidadeSobra = PropertyEntity({ getType: typesKnockout.int, text: "Quantidade:", val: ko.observable(""), def: "", visible: ko.observable(true), required: true });
    this.AdicionarSobra = PropertyEntity({ eventClick: adicionarSobraClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

function loadGestaoOcorrencia(){
    _pesquisaGestaoOcorrencia = new PesquisaGestaoOcorrencia();
    KoBindings(_pesquisaGestaoOcorrencia, "knockoutPesquisaGestaoOcorrencia");

    loadGridPesquisaGestaoOcorrencia();
    loadHtmlGestaoOcorrencia("gestaoOcorrencia", false, null);
}

function loadHtmlGestaoOcorrencia(idDivConteudo, filtrarTiposOcorrenciaPermitidasNoPortalDoCliente, callback) {
    $.get("Content/Static/Ocorrencia/GestaoOcorrencia.html?dyn=" + guid(), function (content) {
        $("#" + idDivConteudo).html(content);

        _gestaoOcorrencia = new GestaoOcorrencia();
        KoBindings(_gestaoOcorrencia, "knockoutOcorrencia");

        _sobras = new Sobras();
        KoBindings(_sobras, "knockoutSobras");

        _controleEntregaDevolucao = new ControleEntregaDevolucaoContainer("controle-entrega-devolucao-container");

        new BuscarCargas(_gestaoOcorrencia.Carga);
        new BuscarXMLNotaFiscal(_pesquisaGestaoOcorrencia.NotaFiscal)
        new BuscarCargaEntrega(_gestaoOcorrencia.CargaEntrega, retornoCargaEntrega, _gestaoOcorrencia.Carga);
        new BuscarTipoOcorrencia(_gestaoOcorrencia.TipoOcorrencia, retornoTipoOcorrencia, null, null, null, null, null, null, true, null, null, null, null, filtrarTiposOcorrenciaPermitidasNoPortalDoCliente);
        new BuscarTiposCausadoresOcorrencia(_gestaoOcorrencia.TiposCausadoresOcorrencia);

        loadGestaoOcorrenciaEtapas();
        loadGestaoOcorrenciaAtendimento();
        loadGestaoOcorrenciaAnexo();
        loadGridSobras();
        loadGestaoOcorrenciaNotaFiscal();

        if (callback)
            callback();
    });
}

function loadGridPesquisaGestaoOcorrencia() {
    var opcaoCarregar = {
        descricao: "Carregar",
        id: guid(),
        evento: "onclick",
        metodo: carregarOcorrenciaClick,
        tamanho: "5",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [opcaoCarregar]
    };

    var configExportacao = {
        url: "GestaoOcorrencia/ExportarPesquisa",
        titulo: "Gestão de Ocorrência"
    };

    _gridPesquisaGestaoOcorrencia = new GridViewExportacao(_pesquisaGestaoOcorrencia.Pesquisar.idGrid, "GestaoOcorrencia/Pesquisa", _pesquisaGestaoOcorrencia, menuOpcoes, configExportacao, null, 5);
    _gridPesquisaGestaoOcorrencia.CarregarGrid();
}

function recarregarGridNotasFiscaisOcorrencia() {
    _gridNotasFiscaisOcorrencia.CarregarGrid(_gestaoOcorrencia.NotasFiscais.val());
}

function pesquisarGestaoOcorrencia() {
    _gridPesquisaGestaoOcorrencia.CarregarGrid();
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function carregarOcorrenciaClick(registroSelecionado) {
    executarReST("GestaoOcorrencia/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                limparTodosCamposGestaoOcorrencia();
                limparCamposSobras();
                limparCamposNotaFiscal();

                _buscouPorCodigo = true;
                _sobras.PermiteSobras.val(retorno.Data.Ocorrencia.PermiteSobras);
                _notaFiscal.OcorrenciaPorNotaFiscal.val(retorno.Data.Ocorrencia.OcorrenciaPorNotaFiscal);


                PreencherObjetoKnout(_gestaoOcorrencia, { Data: retorno.Data.Ocorrencia });
                PreencherObjetoKnout(_gestaoOcorrenciaAtendimento, { Data: retorno.Data.Atendimento });
                setarStatusEtapaAtendimento(retorno.Data.Atendimento.Situacao);
                _anexosGestaoOcorrencia.Anexos.val(retorno.Data.Ocorrencia.Anexos);

                if (_gestaoOcorrencia.TipoDevolucao.val() === EnumTipoColetaEntregaDevolucao.Parcial)
                    _controleEntregaDevolucao.preencher(retorno.Data.Ocorrencia.CargaEntrega.Codigo, false, retorno.Data.Atendimento.Codigo);

                RecarregarGridSobras();
                setarVisibilidadeFaixaTabAtendimento();

                preencherXMLNotasFiscais(retorno.Data.NotasFiscais);
                if (_notaFiscal.OcorrenciaPorNotaFiscal.val()) {
                    $("#liTabNotaFiscal").show();
                }
                else
                {
                    if ($('#tabNotaFiscal').hasClass('active')) {
                        $("#tabNotaFiscal").removeClass("active");
                        $("#tabNotaFiscal").removeClass("show");

                        $("#tabKnockoutOcorrencia").addClass("active show");
                        $("#liTabOcorrencia li:first-child a").addClass("active");
                        $("#liTabNotaFiscal a").removeClass("active");

                    }
                    $("#liTabNotaFiscal").hide();
                }

                bloquearSelecionarNotasFiscais();
                _gridNotaFiscal.DesabilitarOpcoes();
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", "Ocorreu uma falha ao buscar o registro.");
        }
    });
}

function adicionarOcorrenciaClick() {
    if (!ValidarCamposObrigatorios(_gestaoOcorrencia)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    var arraySobras = new Array();
    if (_gestaoOcorrencia.Sobras.list.length > 0)
        _gestaoOcorrencia.Sobras.list.forEach(function (sobra) {
            arraySobras.push({ Codigo: sobra.Codigo.val, CodigoSobra: sobra.CodigoSobra.val, QuantidadeSobra: sobra.QuantidadeSobra.val });
        });

    var dados = {
        ItensDevolver: _controleEntregaDevolucao.obter(),
        TipoOcorrencia: _gestaoOcorrencia.TipoOcorrencia.codEntity(),
        CargaEntrega: _gestaoOcorrencia.CargaEntrega.codEntity(),
        TipoDevolucao: _gestaoOcorrencia.TipoDevolucao.val(),
        Observacoes: _gestaoOcorrencia.Observacoes.val(),
        TiposCausadoresOcorrencia: _gestaoOcorrencia.TiposCausadoresOcorrencia.codEntity(),
        Sobras: JSON.stringify(arraySobras),
        DataPrevisaoEntrega: _gestaoOcorrencia.DataPrevisaoEntrega.val(),
        NotasFiscais: obterNotasFiscais()
    };

    executarReST("GestaoOcorrencia/AdicionarOcorrencia", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Ocorrência adicionada com sucesso!");
                if (_anexosGestaoOcorrencia.Anexos.val().length > 0) {
                    EnviarArquivosAnexadosChamado(function () {
                        _anexosGestaoOcorrencia.Anexos.val([]);
                    }, retorno.Data.Codigo);
                }

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor) {
                    PreencherObjetoKnout(_gestaoOcorrenciaAtendimento, { Data: retorno.Data.Atendimento });
                    setarStatusEtapaAtendimento(retorno.Data.Atendimento.SituacaoAtendimento);
                } else {
                    limparTodosCamposGestaoOcorrencia();
                    limparCamposSobras();
                    _gridPesquisaGestaoOcorrencia.CarregarGrid();
                } 
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", "Ocorreu uma falha ao adicionar a ocorrência.");
        }
    });
}

function retornoTipoOcorrencia(retorno) {
    _sobras.PermiteSobras.val(retorno.PermiteInformarSobras)

    _gestaoOcorrencia.TipoOcorrencia.val(retorno.Descricao);
    _gestaoOcorrencia.TipoOcorrencia.codEntity(retorno.Codigo);
    _gestaoOcorrencia.TipoDevolucao.visible(retorno.TipoOcorrenciaMotivoRejeicao);
    _notaFiscal.OcorrenciaPorNotaFiscal.val(retorno.OcorrenciaPorNotaFiscal);

    if (_notaFiscal.OcorrenciaPorNotaFiscal)
        $("#liTabNotaFiscal").show();
}

function retornoCargaEntrega(entrega) {
    _gestaoOcorrencia.CargaEntrega.val(entrega.Codigo);
    _gestaoOcorrencia.CargaEntrega.codEntity(entrega.Codigo);
    permitirSelecionarNotasFiscais();

    executarReST("ControleEntregaEntrega/BuscarDetalhesEntrega", { Codigo: entrega.Codigo }, function (arg) {
        if (arg.Success) {
            var data = arg.Data;
            if (data !== false) {
                _controleEntregaDevolucao.preencher(entrega.Codigo, true);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
        }
    });
}

function limparClick() {
    etapaOcorrenciaAguardando();
    limparTodosCamposGestaoOcorrencia();
    limparCamposSobras();
    limparCamposNotaFiscal()
}

function limparTodosCamposGestaoOcorrencia() {
    LimparCampos(_gestaoOcorrencia);
    _controleEntregaDevolucao.limpar();
    _pesquisaGestaoOcorrencia.ExibirFiltros.visibleFade(false);
    _gestaoOcorrencia.DataPrevisaoEntrega.visible(false);
}

function adicionarSobraClick() {
    if (!ValidarCamposObrigatorios(_sobras)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    _sobras.Codigo.val(guid());
    _gestaoOcorrencia.Sobras.list.push(SalvarListEntity(_sobras));

    limparCamposCadastroSobras();
    RecarregarGridSobras();
}

function loadGridSobras() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: excluirSobraClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoSobra", title: "Código", width: "40%", className: "text-align-left" },
        { data: "QuantidadeSobra", title: "Quantidade", width: "25%", className: "text-align-left" }
    ];

    _gridSobras = new BasicDataTable(_sobras.GridSobra.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridSobras.CarregarGrid([]);
}

function excluirSobraClick(data) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o registro?", function () {
        for (var i = 0; i < _gestaoOcorrencia.Sobras.list.length; i++) {
            if (data.Codigo == _gestaoOcorrencia.Sobras.list[i].Codigo.val) {
                _gestaoOcorrencia.Sobras.list.splice(i, 1);
                break;
            }
        }

        RecarregarGridSobras();
    });
}

function exibirCamposSobra(e) {
    if (e.MostrarCamposSobra.visibleFade()) {
        e.MostrarCamposSobra.icon("fal fa-plus");
    } else {
        e.MostrarCamposSobra.icon("fal fa-minus");
    }
    e.MostrarCamposSobra.visibleFade(!e.MostrarCamposSobra.visibleFade());
}

function RecarregarGridSobras() {
    var data = new Array();

    $.each(_gestaoOcorrencia.Sobras.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.CodigoSobra = item.CodigoSobra.val;
        itemGrid.QuantidadeSobra = item.QuantidadeSobra.val;

        data.push(itemGrid);
    });

    _gridSobras.CarregarGrid(data, !_gestaoOcorrencia.Codigo.val() > 0);
}

function limparCamposSobras() {
    limparCamposCadastroSobras();
    _buscouPorCodigo = false;
    _sobras.PermiteSobras.val(false);
    _sobras.MostrarCamposSobra.visibleFade(false);
    _gridSobras.CarregarGrid([]);
}

function limparCamposCadastroSobras() {
    _sobras.Codigo.val("");
    _sobras.CodigoSobra.val("");
    _sobras.QuantidadeSobra.val("");
}