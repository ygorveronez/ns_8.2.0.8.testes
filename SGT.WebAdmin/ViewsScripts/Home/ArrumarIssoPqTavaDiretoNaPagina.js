/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/Rest.js" />
/// <reference path="../../js/Global/Mensagem.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/bootstrap/bootstrap.js" />
/// <reference path="../../js/libs/jquery.blockui.js" />
/// <reference path="../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../js/app.config.js" />
/// <reference path="../../js/Global/Charts.js" />
/// <reference path="../Financeiros/LancamentoConta/LancamentoConta.js" />
/// <reference path="ConfiguracaoCertificado.js" />

var _graficoFaturamento;
var _gridContasReceber;
var _gridContasPagar;
var _gridAlertaMultiNFe;

//*******MAPEAMENTO KNOUCKOUT*******

var _contaPagarReceber;
var _termoUsoSistema;
var _legendaTooltipGrafico = "<b>Faturamento:</b> Valor total dos títulos a receber gerados durante o mês " +
    "<br/><b>Receita:</b> Valor total dos títulos a receber com vencimento no mês" +
    "<br/><b>Recebido:</b> Total dos valores de títulos recebidos durante o mês" +
    "<br/><b>Liquidado:</b> Total dos valores recebidos de títulos com vencimento no mês" +
    "<br/><b>Despesa:</b> Valor total dos títulos a pagar com vencimento no mês" +
    "<br/><b>Pago:</b> Valor total dos títulos pagos com vencimento no mês";

var ContaPagarReceber = function () {
    this.GridContaPagarReceber = PropertyEntity({ type: types.local, idGrid: guid() });
};

var TermoUsoSistema = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoAnexo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NecessarioNovoAceite = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.DataVigenciaContrato = PropertyEntity({ text: "Vigência:", val: ko.observable(""), def: "" });

    this.AceitarTermo = PropertyEntity({ eventClick: confirmarAceiteTermoUsoClick, type: types.event, text: "Li e Concordo com os Termos de Uso", visible: ko.observable(false) });
    this.Fechar = PropertyEntity({ eventClick: fecharTermoUsoClick, type: types.event, text: "Fechar", visible: ko.observable(true) });
    this.BaixarContrato = PropertyEntity({ eventClick: baixarContratoClick, type: types.event, text: "Baixar Contrato", visible: ko.observable(false) });
};

//*******EVENTOS*******

function ArrumarIssoPqTavaDiretoNaPagina() {
    _contaPagarReceber = new ContaPagarReceber();
    KoBindings(_contaPagarReceber, "knockoutContaPagarReceber");

    _termoUsoSistema = new TermoUsoSistema();
    KoBindings(_termoUsoSistema, "knockoutTermoUsoSistemaHome");

    $("#btnTermosDeUso").click(function () {
        Global.abrirModal('divModalTermoUso');
    });

    /*$("#btnNovosTitulos").click(function () {
        new LancarContas();
    });

    $("#btnDownloadAssinador").click(function () {
        executarDownload("NotaFiscalEletronica/DownloadAssinador", { Codigo: 0 });
    });

    $("#btnCertificadoDigital").click(function () {
        loadConfiguracaoCertificado();
    });*/

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFeAdmin || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {

        VerificarCertificadosVencidos(VerificaPermissaoUsuario);

        /*if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe && _CONFIGURACAO_TMS.UsuarioAdministrador) {

            $("#btnAcessoNotaFiscal").show();
            $("#btnAcessoNFCe").show();
            $("#btnAcessoNFSe").show();
            $("#btnAcessoContasPagar").show();
            $("#btnAcessoContasReceber").show();
            $("#btnAcessoAgendaTArefas").show();
            $("#btnNovosTitulos").show();
            $("#btnDownloadAssinador").show();
            $("#btnCertificadoDigital").show();
        } else {
            $("#divAcessoRapido").hide();
        }*/
    }
    else {
        $("#divAcessoRapido").hide();
        $("#divCertificado").hide();
    }

    if (_CONFIGURACAO_TMS.ExigirAceiteTermoUsoSistema) {
        CarregaTermoDeUsoSistema();
    }
}

function VerificarCertificadosVencidos(callback) {
    var dataEnvio = {
        TipoAcesso: _CONFIGURACAO_TMS.TipoServicoMultisoftware
    };
    executarReST("Home/VerificarCertificadosVencidos", dataEnvio, function (arg) {
        if (arg.Success) {
            if (arg.Data && arg.Data != null && arg.Data.length > 0) {
                var header = [{ data: "CNPJ", title: "CNPJ", width: "18%" },
                { data: "Nome", title: "Razão Social", width: "35%" },
                { data: "Fone", title: "Telefone", width: "10%" },
                { data: "DataVencimento", title: "Vencimento", width: "15%" }];

                var _gridCertificados = new BasicDataTable("gridCertificado", header, null, { column: 3, dir: orderDir.asc });
                var dataVencimentoMultiNFe = "";
                var data = new Array();
                $.each(arg.Data, function (i, listaEmpresa) {
                    var listaItemEmpresa = new Object();

                    listaItemEmpresa.CNPJ = listaEmpresa.CNPJ;
                    listaItemEmpresa.Nome = listaEmpresa.Nome;
                    listaItemEmpresa.Fone = listaEmpresa.Fone;
                    listaItemEmpresa.DataVencimento = listaEmpresa.DataVencimento;
                    dataVencimentoMultiNFe = listaEmpresa.DataVencimento;

                    data.push(listaItemEmpresa);
                });

                _gridCertificados.CarregarGrid(data);

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe && dataVencimentoMultiNFe != "")
                    exibirMensagem(tipoMensagem.aviso, "Aviso", "Seu certificado digital possui o vencimento em: " + dataVencimentoMultiNFe);
                else {
                    $("#wid-id-2").show();
                    $("#divCertificado").show();
                    $("#divAcessoRapido").show();
                }
            } else {
                $("#wid-id-2").hide();
                $("#divCertificado").hide();
            }
            callback();
        } else {
            $("#wid-id-2").hide();
            $("#divCertificado").hide();
            exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
        }
    });
}

function VerificaPermissaoUsuario() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        var dataEnvio = {
            TipoAcesso: _CONFIGURACAO_TMS.TipoServicoMultisoftware
        };
        executarReST("Home/VerificaPermissaoUsuario", dataEnvio, function (arg) {
            if (arg.Success) {
                if (arg.Data != null && arg.Data) {
                    if (arg.Data.VisualizarGraficosIniciais) {
                        $("#divAcessoRapido").show();
                        CarregarGraficoFaturamento(CarregarContas);
                    }
                    else
                        $("#divAcessoRapido").hide();
                }
                CarregaTermoDeUsoSistema();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            }
        });
    }
}

function CarregaTermoDeUsoSistema() {
    var dataEnvio = {
        TipoAcesso: _CONFIGURACAO_TMS.TipoServicoMultisoftware
    };
    executarReST("Home/CarregaTermoDeUsoSistema", dataEnvio, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var data = arg.Data;

                $("#btnTermosDeUso").show();
                $('#contentTermoUso').html("");
                $('#contentTermoUso').html(data.TermoUso);

                _termoUsoSistema.NecessarioNovoAceite.val(data.NecessarioNovoAceite);
                _termoUsoSistema.DataVigenciaContrato.val(data.DataVigenciaContrato);
                _termoUsoSistema.Codigo.val(data.Codigo);

                if (data.CodigoAnexo > 0) {
                    _termoUsoSistema.CodigoAnexo.val(data.CodigoAnexo);
                    _termoUsoSistema.BaixarContrato.visible(true);
                }

                if (!data.AceitouTermosUso || data.NecessarioNovoAceite) {
                    _termoUsoSistema.AceitarTermo.visible(true);

                    if (_CONFIGURACAO_TMS.ExigirAceiteTermoUsoSistema) {
                        _termoUsoSistema.Fechar.visible(false);
                        $("#btnTermosDeUso").click();
                    }
                }
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
        }
    });
}

function CarregarGraficoFaturamento(callback) {

    var options = {
        type: ChartType.BarHorizontal,
        idContainer: "divGraficoBarras",
        properties: {
            x: 'Valor',
            xType: ChartPropertyType.decimal,
            xText: 'DescricaoValor',
            y: 'Descricao',
            yType: ChartPropertyType.string,
            color: 'Cor'
        },
        margin: {
            top: 70,
            right: 100,
            left: 150,
            bottom: 0
        },
        title: "Gráfico de Faturamento",
        yTitle: "",
        xTitle: "Valor (R$)",
        fileName: "Gráfico de Faturamento",
        url: "Home/ObterValoresFaturamento",
        knockoutParams: null,
        breadcumbTitle: "Dados Gerais",
        drillDownSettings: null,
        width: 0, //0 = auto
        height: 0 //0 = auto//90 fica bom
    };

    _graficoFaturamento = new Chart(options);

    _graficoFaturamento.init();

    $("#wid-id-Grafico").show();

    $('#divGraficoBarras').popover({
        title: 'Informações do Gráfico',
        html: true,
        content: _legendaTooltipGrafico,
        container: 'body',
        placement: 'right',
        trigger: "hover"
    });

    carregarGraficosMicrocharts(callback);
}

function carregarGraficosMicrocharts(callback) {
    executarReST("Home/BuscarGraficosGerais", null, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                html = "";
                for (var i = 0; i < arg.Data.length; i++) {
                    var situacao = arg.Data[i];

                    if (situacao.Icone == "") {
                        var total = situacao.Quantidade;
                        var percentual = (Globalize.parseFloat(situacao.Valor) * 100) / total;
                        html += '<div class="col-12 col-md-2">';
                        html += '<div class="sparks-info" style="text-align: center;"><div class="easy-pie-chart" style="color: ' + situacao.Cor + '" data-percent="' + parseInt(percentual) + '" data-pie-size="50">';
                        html += '<span class="percent percent-sign">' + parseInt(percentual) + '</span>';
                        html += '</div></div>';
                        //html += '<span class="easy-pie-title">' + situacao.DescricaoTipo + '</span>';
                        html += '       <br/><div class="sparks-info" style="text-align: center;"><h7>' + situacao.DescricaoTipo + '</h7></div>';
                        html += '<ul class="smaller-stat hidden-sm pull-right">';
                        html += '<li> ';
                        html += '<span class="label" style="background: ' + situacao.Cor + ';"><i ></i>' + Globalize.parseInt(situacao.Valor) + '</span>';
                        html += '</li>';
                        html += '</ul>';
                        html += '</div>';
                    } else {
                        html += '<div class="col-12 col-md-2">';
                        html += '   <div class="sparks-info" style="text-align: center;"><span><img src="img/' + situacao.Imagem + '" alt="SmartAdmin" /></span></div>';
                        html += '   <div class="sparks-info" style="text-align: center;">';
                        html += '       <br/><h7>' + situacao.DescricaoTipo + '</h7><br/>';
                        html += '       <span style="color: ' + situacao.Cor + '; font-size:14px;"> <i class="fa ' + situacao.Icone + '"></i> R$ ' + situacao.Valor + '</span>';
                        html += '   </div>';
                        html += '</div>';
                    }
                }
                $("#divMicrocharts").html(html);

                $('.easy-pie-chart').each(function () {
                    var $this = $(this),
                        barColor = $this.css('color') || $this.data('pie-color'),
                        trackColor = $this.data('pie-track-color') || 'rgba(0,0,0,0.04)',
                        size = parseInt($this.data('pie-size')) || 25;
                    $this.easyPieChart({
                        barColor: barColor,
                        trackColor: trackColor,
                        scaleColor: false,
                        lineCap: 'butt',
                        lineWidth: parseInt(size / 8.5),
                        animate: 1500,
                        rotate: -90,
                        size: size,
                        onStep: function (from, to, percent) {
                            $(this.el).find('.percent').text(Math.round(percent));
                        }
                    });
                    $this = null;
                });
                if (callback != undefined)
                    callback();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                if (callback != undefined)
                    callback();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            if (callback != undefined)
                callback();
        }
    });
}

function CarregarContas() {
    _gridContasReceber = new GridView(_contaPagarReceber.GridContaPagarReceber.idGrid, "Home/ContasEmpresa", _contaPagarReceber, null, { column: 2, dir: orderDir.asc }, null, null);
    _gridContasReceber.CarregarGrid();
    CarregarAlertasLicencasMultiNFe();
}

function CarregarAlertasLicencasMultiNFe() {
    executarReST("ControleAlerta/CarregarAlertasLicencas", null, function (arg) {
        if (arg.Success) {
            if (arg.Data) {

                var ocultar = {
                    descricao: "Ocultar", id: guid(), evento: "onclick", metodo: function (data) {
                        OcultarLicencaMultiNFeClick(data);
                    }, tamanho: "10", icone: "fa-check-square-o"
                };
                var menuOpcoes = new Object();
                menuOpcoes.tipo = TypeOptionMenu.link;
                menuOpcoes.opcoes = new Array();
                menuOpcoes.opcoes.push(ocultar);

                var header = [
                    { data: "Codigo", visible: false },
                    { data: "Descricao", title: "Descrição", width: "60%", className: "text-align-left" },
                    { data: "Data", title: "Data", width: "20%", className: "text-align-center" }
                ];

                _gridAlertaMultiNFe = new BasicDataTable("gridAlertas", header, menuOpcoes, null, null, 3);

                var data = new Array();
                $.each(arg.Data, function (i, listaAlerta) {
                    var tarefa = new Object();

                    tarefa.Codigo = listaAlerta.Codigo;
                    tarefa.Descricao = listaAlerta.Descricao;
                    tarefa.Data = listaAlerta.Data;

                    data.push(tarefa);
                });

                _gridAlertaMultiNFe.CarregarGrid(data);

            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
        }
    });
}

function OcultarLicencaMultiNFeClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja ocultar o alerta " + data.Descricao + "?", function () {
        var dataEnvio = {
            Codigo: data.Codigo
        };
        executarReST("ControleAlerta/OcultarAlerta", dataEnvio, function (arg) {
            if (!arg.Success) {
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            } else {
                CarregarAlertasLicencasMultiNFe();
            }
        });
    });
}

function confirmarAceiteTermoUsoClick() {
    executarReST("Home/AceitarTermoDeUsoSistema", { NecessarioNovoAceite: _termoUsoSistema.NecessarioNovoAceite.val(), Codigo: _termoUsoSistema.Codigo.val() }, function (arg) {
        if (!arg.Success) {
            exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
        } else {
            _termoUsoSistema.AceitarTermo.visible(false);
            _termoUsoSistema.Fechar.visible(true);
            fecharTermoUsoClick();
        }
    });
}

function fecharTermoUsoClick() {
    Global.fecharModal("divModalTermoUso");
}

function baixarContratoClick() {
    executarDownload("Home/DownloadContrato", { CodigoAnexo: _termoUsoSistema.CodigoAnexo.val() });
}