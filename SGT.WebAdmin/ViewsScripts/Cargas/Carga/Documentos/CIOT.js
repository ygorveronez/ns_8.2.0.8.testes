/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="CTe.js" />
/// <reference path="PreCTe.js" />

var _HTML_CARGA_CIOT_LOADED = false;

var _pesquisaHistoricoIntegracaoCIOT;
var _gridHistoricoIntegracaoCIOT;

var PesquisaHistoricoIntegracaoCIOT = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCargaCIOT = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function buscarCargasCIOT(e) {
    var abrir = { descricao: Localization.Resources.Cargas.Carga.Abrir, id: guid(), metodo: abrirModalCIOTClick, icone: "" };
    var imprimir = { descricao: Localization.Resources.Cargas.Carga.Imprimir, id: guid(), metodo: ImpressaoCIOTClick, icone: "", visibilidade: VisibilidadeImpressao };
    var reenviar = { descricao: Localization.Resources.Cargas.Carga.Reenviar, id: guid(), metodo: reemitirCIOT, icone: "", visibilidade: VisibilidadeReenviar };
    var historico = { descricao: Localization.Resources.Cargas.Carga.HistoricoDeIntegracao, id: guid(), metodo: ExibirHistoricoIntegracaoCIOT, icone: "", visibilidade: VisibilidadeDetalhes };
    var detalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), metodo: DetalhesCIOTClick, icone: "" };
    var gerarRegistroDesembarque = { descricao: Localization.Resources.Cargas.Carga.GerarRegistroDesembarque, id: guid(), metodo: GerarRegistroDesembarque, visibilidade: VisibilidadeGerarRegistroDesembarque, icone: "" };
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: EditarCIOTClick, visibilidade: VisibilidadeEditar, icone: ""};

    //var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("CargaIntegracaoValePedagio"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };

    _cargaCTe.SituacaoIntegracaoValePedagio.visible(true);

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [reenviar, imprimir, historico, detalhes, gerarRegistroDesembarque, editar] };

    var isProblemaCIOT = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && (_cargaAtual.ProblemaIntegracaoCIOT.val() || _cargaAtual.LiberadoComProblemaCIOT.val());

    if (_isPreCte) {
        _gridCargaCIOTPreCTe = new GridView(_cargaCTe.PesquisarCIOTPreCte.idGrid, "CargaCIOT/Pesquisa", _cargaCTe, menuOpcoes);
        _gridCargaCIOTPreCTe.CarregarGrid(function () {
            if (_gridCargaCIOTPreCTe.NumeroRegistros() > 0) {
                $("#tabCIOTPreCTes_" + e.DadosCTes.id + "_li").show();
                LoadCIOTCarga();
            } else {
                $("#tabCIOTPreCTes_" + e.DadosCTes.id + "_li").hide();
            }
        });

        if (isProblemaCIOT) {
            _cargaCTe.LiberarCargaComCIOTRejeitadosPreCte.visible(true);

            if (_cargaCTe.LiberarCargaComCIOTRejeitadosPreCte.val())
                _cargaCTe.LiberarCargaComCIOTRejeitadosPreCte.enable(false);
            else
                _cargaCTe.LiberarCargaComCIOTRejeitadosPreCte.enable(true);
        } else {
            _cargaCTe.LiberarCargaComCIOTRejeitadosPreCte.visible(false);
        }
    } else {
        _gridCargaCIOT = new GridView(_cargaCTe.PesquisarCIOT.idGrid, "CargaCIOT/Pesquisa", _cargaCTe, menuOpcoes);
        _gridCargaCIOT.CarregarGrid(function () {
            if (_gridCargaCIOT.NumeroRegistros() > 0 || (_cargaAtual.ProblemaIntegracaoCIOT.val() === true && _gridCargaCIOT.NumeroRegistros() <= 0)) {
                $("#tabCIOT_" + e.DadosCTes.id + "_li").show();
                LoadCIOTCarga();
            } else {
                $("#tabCIOT_" + e.DadosCTes.id + "_li").hide();
            }
        });

        if (isProblemaCIOT) {
            _cargaCTe.LiberarCargaComCIOTRejeitados.visible(true);

            if (_cargaAtual.LiberadoComProblemaCIOT.val())
                _cargaCTe.LiberarCargaComCIOTRejeitados.enable(false);
            else
                _cargaCTe.LiberarCargaComCIOTRejeitados.enable(true);
        } else {

            if (_cargaAtual.ProblemaIntegracaoCIOT.val() === true && _cargaAtual.MotivoPendencia.val() != "" && _gridCargaCIOT.NumeroRegistros() <= 0)
                exibirMotivoPendenciaEtapaDocumentosCarga(_cargaAtual, 'DivMensagemCIOT');

            _cargaCTe.LiberarCargaComCIOTRejeitados.visible(false);
        }
    }
}

function EditarCIOTClick(row) {
    const payload = {
        codigoCargaCiot: row.CodigoCargaCIOT || row.Codigo,
        carga: _cargaAtual.Codigo.val()
    };
    
    executarReST("CargaCTe/ObterDadosGeracaoCIOT", payload, function (arg) {
        if (arg.Data != null) {

            _cargaCTe.ValorAntigoCIOT.val(row.Numero);
            _cargaCTe.CIOT.val(arg.Data.Numero);
            _cargaCTe.FormaPagamento.val(arg.Data.TipoPagamento);
            _cargaCTe.ValorAdiantamento.val(arg.Data.ValorAdiantamento);
            _cargaCTe.ValorFrete.val(arg.Data.ValorFrete);
            _cargaCTe.DataVencimento.val(arg.Data.DataAbertura);
            ObterCampoPixCte(_cargaCTe, arg.Data.TipoPagamento, arg.Data.TipoChavePIX, arg.Data.ChavePIX);
            _cargaCTe.TipoChavePIX.val(arg.Data.TipoChavePIX);
            _cargaCTe.TipoPagamento.val(arg.Data.TipoInformacaoBancaria);
            _cargaCTe.ContaCIOT.val(arg.Data.Conta);
            _cargaCTe.AgenciaCIOT.val(arg.Data.Agencia);
            _cargaCTe.CNPJInstituicaoPagamento.val(arg.Data.Ipef);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });

    $("#DadosGeracaoCIOT").show();
}

function VisibilidadeEditar(row) {
    if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador && row.RegistradoPeloEmbarcador) || (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe)) {
        if (!row.StatusMDFe) 
            return true;
    }
    return false;
}


function CarregarGridCIOT() {
    _gridCargaCIOT.CarregarGrid();
}

function DetalhesCIOTClick(integracao) {
    BuscarDetalhesCIOT(integracao);
    Global.abrirModal("divModalDetalhesCIOT");
}

function BuscarDetalhesCIOT(integracao) {
    _pesquisaHistoricoIntegracaoCIOT = new PesquisaHistoricoIntegracaoCIOT();
    _pesquisaHistoricoIntegracaoCIOT.Codigo.val(integracao.Codigo);

    _gridDetalhesCIOT = new GridView("tblDetalhesCIOT", "CargaCIOT/ConsultarDetalhes", _pesquisaHistoricoIntegracaoCIOT, null, { column: 1, dir: orderDir.desc });
    _gridDetalhesCIOT.CarregarGrid();
}

function ExibirHistoricoIntegracaoCIOT(integracao) {
    BuscarHistoricoIntegracaoCIOT(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoCTe");
}

function BuscarHistoricoIntegracaoCIOT(integracao) {
    _pesquisaHistoricoIntegracaoCIOT = new PesquisaHistoricoIntegracaoCIOT();
    _pesquisaHistoricoIntegracaoCIOT.Codigo.val(integracao.Codigo);
    _pesquisaHistoricoIntegracaoCIOT.CodigoCargaCIOT.val(integracao.CodigoCargaCIOT);

    var download = { descricao: Localization.Resources.Cargas.Carga.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoCIOT, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoCIOT = new GridView("tblHistoricoIntegracaoCTe", "CargaCIOT/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoCIOT, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoCIOT.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoCIOT(historicoConsulta) {
    executarDownload("CargaCIOT/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo, CodigoCargaCIOTArquivo: historicoConsulta.CodigoCargaCIOTArquivo });
}

function VisibilidadeReenviar(datagrid) {
    return (datagrid.Situacao === EnumSituacaoCIOT.Pendencia || datagrid.Situacao === EnumSituacaoCIOT.AgIntegracao);
}

function VisibilidadeDetalhes(datagrid) {
    return (datagrid.TipoCIOT === "Por Período");
}

function VisibilidadeGerarRegistroDesembarque(datagrid) {
    return ((datagrid.Situacao === EnumSituacaoCIOT.Aberto || datagrid.Situacao == EnumSituacaoCIOT.Encerrado) && _CONFIGURACAO_TMS.PermitirGerarRegistroDeDesembarqueNoCIOT);
}

function VisibilidadeImpressao(datagrid) {
    return (datagrid.Situacao === EnumSituacaoCIOT.Aberto || datagrid.Situacao == EnumSituacaoCIOT.Encerrado || datagrid.Situacao == EnumSituacaoCIOT.PagamentoAutorizado);
}

//*******METODOS*******

function reemitirCIOT(datagrid) {
    var data = {
        Codigo: datagrid.Codigo,
        Carga: _cargaAtual.Codigo.val()
    };

    executarReST("CargaCIOT/ReenviarIntegracao", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_isPreCte)
                    _gridCargaCIOTPreCTe.CarregarGrid();
                else
                    _gridCargaCIOT.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function GerarRegistroDesembarque(datagrid) {
    var data = {
        Codigo: datagrid.Codigo,
        Carga: _cargaAtual.Codigo.val()
    };

    executarReST("CargaCIOT/GerarRegistroDeDesembarque", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_isPreCte)
                    _gridCargaCIOTPreCTe.CarregarGrid();
                else
                    _gridCargaCIOT.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}


function LoadCIOTCarga() {
    if (_HTML_CARGA_CIOT_LOADED) return;

    //$.get("CargaCIOT/CIOT", function (html) {
    //    var $page = $(html);
    //    var $container = $page.find(".content-external");
    //    $("#divModalCargaCIOT .load").html($container.html());

    //    //LoadCIOT();

    //    _HTML_CARGA_CIOT_LOADED = true;
    //});
}

function abrirModalCIOTClick(dataRow) {
    var data = {
        Codigo: dataRow.Codigo
    }
    EditarCIOT(data, function () {
        Global.abrirModal("divModalCargaCIOT");
    });
}

function ImpressaoCIOTClick(data) {
    executarDownload("CargaCIOT/Imprimir", { Codigo: data.Codigo, Carga: _cargaCTe.Carga.val() });
}

function LiberarCargaComCIOTRejeitadosClick(e) {
    var data = {
        Carga: _cargaAtual.Codigo.val()
    }
    exibirConfirmacao(Localization.Resources.Cargas.Carga.AvancarEtapa, Localization.Resources.Cargas.Carga.VoceTemCertezaQueDesejaAvancarEtapaMesmoQueTenhaCIOTComRejeicao, function () {
        executarReST("CargaCTe/LiberarComProblemaCIOT", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _cargaCTe.LiberarCargaComAverbacoesRejeitados.enable(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}