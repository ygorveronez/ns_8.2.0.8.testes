<%@ Page Title="Carta de Correção Eletrônica" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="CartaDeCorrecaoEletronica.aspx.cs" Inherits="EmissaoCTe.WebApp.CartaDeCorrecaoEletronica" %>

<%@ Import Namespace="System.Web.Optimization" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845DCD8080CC91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker") %>
        <%: Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datetimepicker",
                           "~/bundle/scripts/priceFormat",
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        $(document).ready(function () {
            ObterInformacoesCTe();

            $("#btnVoltar").click(function () {
                RedirecionarParaVoltar();
            });


        });

        function ObterInformacoesCTe() {
            executarRest("/CartaDeCorrecaoEletronica/ObterInformacoesCTe?callback=?", { CodigoCTe: GetUrlParam("x") }, function (r) {
                if (r.Sucesso) {
                    $("#txtNumeroCTe").val(r.Objeto.Numero + " - " + r.Objeto.Serie);
                    $("#txtDataEmissaoCTe").val(r.Objeto.DataEmissao);
                    $("#txtValorCTe").val(Globalize.format(r.Objeto.ValorFrete, "n2"));
                    $("#txtChaveCTe").val(r.Objeto.Chave);
                    $("#txtRemetenteCTe").val(r.Objeto.Remetente);
                    $("#txtDestinatarioCTe").val(r.Objeto.Destinatario);

                    $("#divEmissaoCCe .modal-header h4").append(" (CT-e " + r.Objeto.Numero + " - " + r.Objeto.Serie + ")");

                    $("body").data("CodigoCTe", r.Objeto.Codigo);

                    CarregarCCes();
                } else {
                    jAlert(r.Erro + "<br /><br />Você será redirecionado de volta.", "Atenção!", function () {
                        RedirecionarParaVoltar();
                    });
                }
            });
        }

        function RedirecionarParaVoltar() {
            var voltar = GetUrlParam("voltar");

            if (voltar == "" || voltar == null)
                voltar = "EmissaoCTe";

            location.href = voltar + ".aspx";
        }

        function GetUrlParam(name) {
            var url = window.location.search.replace("?", "");
            var itens = url.split("&");
            for (n in itens) {
                if (itens[n].match(name)) {
                    return itens[n].replace(name + "=", "");
                }
            }
            return null;
        }
    </script>
    <script id="ScriptItensCCe" type="text/javascript">
        $(document).ready(function () {
            $("#txtSequenciaItem").priceFormat({ limit: 3, centsLimit: 0, centsSeparator: '' });

            CarregarConsultaDeCamposDaCartaDeCorrecaoEletronica("btnConsultarCamposCCe", "btnConsultarCamposCCe", RetornoConsultaCampoCCe, true, false);

            $("#btnNovaCCe").click(function () {
                AbrirTelaEmissaoCCe();
            });

            $("#btnSalvarValor").click(function () {
                SalvarItemCCe();
            });

            $("#btnCancelarValor").click(function () {
                LimparCamposItemCCe();
            });

            $("#btnExcluirValor").click(function () {
                ExcluirItemCCe();
            });

            $("#txtCampoCCe").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("CampoCCe", null);
                        $("#txtCampoCCe").val("");
                        LimparCamposCCe();
                    } else {
                        e.preventDefault();
                    }
                }
            });
        });

        function AbrirTelaEmissaoCCe() {
            $("#txtDataEvento").val(Globalize.format(new Date(), "dd/MM/yyyy HH:mm"));
            $("#divEmissaoCCe").modal({ keyboard: false, backdrop: 'static' });
        }

        function FecharTelaEmissaoCCe() {
            LimparCCe();

            $("#divEmissaoCCe").modal('hide');
        }

        function RetornoConsultaCampoCCe(campo) {
            LimparCamposCCe();

            $("#txtCampoCCe").val(campo.Descricao);

            ObterDadosCampoCCe(campo.Codigo);
        }

        function ObterDadosCampoCCe(codigoCampo) {
            executarRest("/CampoDaCartaDeCorrecaoEletronica/ObterDetalhes?callback=?", { CodigoCampoCCe: codigoCampo }, function (r) {
                if (r.Sucesso) {
                    RenderizarCampoCCe(r.Objeto);
                    $("body").data("CampoCCe", r.Objeto);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção", "placeholder-msgEmissaoCCe");
                }
            });
        }

        function LimparCamposItemCCe() {
            $("body").data("CodigoItemCCe", 0);
            $("#btnExcluirValor").hide();

            LimparCamposCCe();
        }

        function LimparCamposCCe() {
            $("#txtCampoCCe").val("");
            $("body").data("CampoCCe", null);

            $("#txtValor").val("");

            if ($("#txtValor").data("DateTimePicker") != null)
                $('#txtValor').data("DateTimePicker").destroy();

            $("#txtValor").unbind();
            $("#txtValor").unmask();

            $("#txtSequenciaItem").val("0");

            $("#selValor").html("");

            $("#divSequencia").hide();
            $("#divValor").hide();
            $("#divValorSelecao").hide();
        }

        function RenderizarCampoCCe(campo, valor) {
            switch (campo.TipoCampo) {
                case 0: //Texto
                    FormatarCampoTexto(campo.QuantidadeCaracteres);
                    break;
                case 1: //Inteiro
                    FormatarCampoInteiro(campo.QuantidadeInteiros);
                    break;
                case 2: //Decimal
                    FormatarCampoDecimal(campo.QuantidadeInteiros, campo.QuantidadeDecimais);
                    break;
                case 3: //Selecao
                    FormataCampoSelecao(campo, valor);
                    break;
                case 4: //Data
                    FormatarCampoData();
                    break;
            }

            if (campo.IndicadorRepeticao)
                $("#divSequencia").show();
        }

        function FormataCampoSelecao(campo, valor) {
            $("#divValor").hide();
            $("#divValorSelecao").show();
            switch (campo.NomeCampo) {
                case "serie":
                    CarregarSeries(valor);
                    break;
                case "CFOP":
                    CarregarCFOPs(valor);
                    break;

                default:
                    $("#divValor").show();
                    $("#divValorSelecao").hide();
                    break;
            }
        }

        function CarregarCFOPs(valor) {
            executarRest("/CFOP/BuscarTodos?callback=?", { Tipo: 1 }, function (r) {
                if (r.Sucesso) {
                    RenderizarCampoSelecao(r.Objeto, "Numero", valor);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção", "placeholder-msgEmissaoCCe");
                }
            });
        }

        function CarregarSeries(valor) {
            executarRest("/EmpresaSerie/BuscarSeriesDoUsuario?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    RenderizarCampoSelecao(r.Objeto, "Numero", valor);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção", "placeholder-msgEmissaoCCe");
                }
            });
        }

        function RenderizarCampoSelecao(valores, campoValor, valorSelecionado) {
            var selValores = document.getElementById("selValor");

            selValores.options.length = 0;

            for (var i = 0; i < valores.length; i++) {

                var optn = document.createElement("option");
                optn.text = valores[i][campoValor];
                optn.value = valores[i][campoValor];

                if (valorSelecionado != null && valorSelecionado == valores[i][campoValor])
                    optn.setAttribute("selected", "selected");

                selValores.options.add(optn);
            }
        }

        function FormatarCampoData() {
            $("#divValor").show();
            FormatarCampoDate("txtValor");
        }

        function FormatarCampoDecimal(inteiros, decimais) {
            $("#divValor").show();
            $("#txtValor").priceFormat({ limit: inteiros, centsLimit: decimais });
            $("#txtValor").val(Globalize.format(0, ("n" + decimais.toString())));
        }

        function FormatarCampoInteiro(tamanho) {
            $("#divValor").show();
            $("#txtValor").priceFormat({ limit: tamanho, centsLimit: 0, centsSeparator: '' });
            $("#txtValor").val("0");
        }

        function FormatarCampoTexto(tamanho) {
            $("#divValor").show();
            $("#txtValor").attr("maxlength", tamanho);
        }

        function ValidarItemCCe() {
            var campoCCe = $("body").data("CampoCCe");
            var valor = campoCCe != null ? campoCCe.TipoCampo != 3 ? $("#txtValor").val() : $("#selValor").val() : "";
            var sequencial = Globalize.parseInt($("#txtSequenciaItem").val());
            var valido = true;

            if (campoCCe != null) {
                CampoSemErro("#txtCampoCCe");
            } else {
                CampoComErro("#txtCampoCCe");
                valido = false;
            }

            if (valor != "") {
                CampoSemErro("#txtValor");
                CampoSemErro("#selValor");
            } else {
                CampoComErro("#txtValor");
                CampoComErro("#selValor");
                valido = false;
            }

            if (campoCCe != null && campoCCe.IndicadorRepeticao) {
                if (sequencial > 0) {
                    CampoSemErro("#txtSequenciaItem");
                } else {
                    CampoComErro("#txtSequenciaItem");
                    valido = false;
                }
            }

            return valido;
        }

        function SalvarItemCCe() {
            if (ValidarItemCCe()) {

                var itemCCe = {
                    Codigo: $("body").data("CodigoItemCCe"),
                    Campo: $("body").data("CampoCCe"),
                    Valor: $("body").data("CampoCCe").TipoCampo != 3 ? $("#txtValor").val() : $("#selValor").val(),
                    Sequencial: Globalize.parseInt($("#txtSequenciaItem").val()),
                    Excluir: false
                };

                var itensCCe = $("body").data("ItensCCe") == null ? new Array() : $("body").data("ItensCCe");

                if (!ValidarItemSequencial(itemCCe, itensCCe)) {

                    if (itemCCe.Campo.IndicadorRepeticao)
                        ExibirMensagemAlerta("O mesmo número sequencial já foi utilizado para este campo.", "Atenção!", "placeholder-msgEmissaoCCe");
                    else
                        ExibirMensagemAlerta("Este campo já foi utilizado e não pode ser utilizado mais de uma vez na mesma Carta de Correção Eletrônica.", "Atenção!", "placeholder-msgEmissaoCCe");

                    return;
                }

                if (itemCCe.Codigo == 0)
                    itemCCe.Codigo = -(itensCCe.length + 1);

                for (var i = 0; i < itensCCe.length; i++) {
                    if (itensCCe[i].Codigo == itemCCe.Codigo) {
                        itensCCe.splice(i, 1);
                        break;
                    }
                }

                itensCCe.push(itemCCe);

                itensCCe.sort(function (a, b) {
                    if (a.Campo.Descricao != b.Campo.Descricao)
                        return a.Campo.Descricao < b.Campo.Descricao ? -1 : 1;
                    else if (a.Valor != b.Valor)
                        return a.Valor < b.Valor ? -1 : 1;
                    else
                        return 0;

                });

                $("body").data("ItensCCe", itensCCe);
                RenderizarItensCCe();
                LimparCamposItemCCe();

            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-msgEmissaoCCe");
            }
        }

        function ValidarItemSequencial(item, itens) {
            if (item.Campo.IndicadorRepeticao) {
                for (var i = 0; i < itens.length; i++) {
                    if (itens[i].Campo.IndicadorRepeticao && item.Campo.GrupoCampo == itens[i].Campo.GrupoCampo && item.Campo.NomeCampo == itens[i].Campo.NomeCampo && itens[i].Sequencial == item.Sequencial)
                        return false;
                }
            } else {
                for (var i = 0; i < itens.length; i++) {
                    if (item.Campo.GrupoCampo == itens[i].Campo.GrupoCampo && item.Campo.NomeCampo == itens[i].Campo.NomeCampo && itens[i].Codigo != item.Codigo)
                        return false;
                }
            }

            return true;
        }

        function EditarItemCCe(item) {
            LimparCamposCCe();

            RenderizarCampoCCe(item.Campo, item.Valor);

            $("body").data("CodigoItemCCe", item.Codigo);
            $("body").data("CampoCCe", item.Campo);
            $("#txtCampoCCe").val(item.Campo.Descricao);
            $("#txtSequenciaItem").val(item.Sequencial);

            if (item.Campo.TipoCampo != 3)
                $("#txtValor").val(item.Valor);

            $("#btnExcluirValor").show();
        }

        function ExcluirItemCCe() {
            var codigo = $("body").data("CodigoItemCCe");

            var itensCCe = $("body").data("ItensCCe") == null ? new Array() : $("body").data("ItensCCe");

            for (var i = 0; i < itensCCe.length; i++) {
                if (itensCCe[i].Codigo == codigo) {
                    if (codigo <= 0)
                        itensCCe.splice(i, 1);
                    else
                        itensCCe[i].Excluir = true;
                    break;
                }
            }

            $("body").data("ItensCCe", itensCCe);
            RenderizarItensCCe();
            LimparCamposItemCCe();
        }

        function LimparCCe() {
            $("body").data("ItensCCe", null);
            $("body").data("CodigoCCe", null);

            RenderizarItensCCe();
            LimparCamposItemCCe();

            $("#divEmissaoCCe .modal-body button").attr("disabled", false);
        }

        function RenderizarItensCCe() {
            var itensCCe = $("body").data("ItensCCe") == null ? new Array() : $("body").data("ItensCCe");

            $("#tblItensCCe tbody").html("");

            for (var i = 0; i < itensCCe.length; i++) {
                if (!itensCCe[i].Excluir)
                    $("#tblItensCCe tbody").append("<tr><td>" + itensCCe[i].Campo.Descricao + "</td><td>" + itensCCe[i].Valor + "</td><td>" + (itensCCe[i].Sequencial > 0 ? itensCCe[i].Sequencial : "") + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarItemCCe(" + JSON.stringify(itensCCe[i]) + ")'>Editar</button></td></tr>");
            }

            if ($("#tblItensCCe tbody").html() == "")
                $("#tblItensCCe tbody").html("<tr><td colspan='4'>Nenhum registro encontrado.</td></tr>");
        }
    </script>
    <script id="ScriptEmissaoCCe" type="text/javascript">
        $(document).ready(function () {
            FormatarCampoDateTime("txtDataEvento");

            $("#btnEmitirCCe").click(function () {
                SalvarCCe(true);
            });
            $("#btnSalvarCCe").click(function () {
                SalvarCCe(false);
            });
            $("#btnCancelarCCe").click(function () {
                FecharTelaEmissaoCCe();
            });
        });
        function SalvarCCe(emitir) {
            if (ValidarCCe()) {

                var dados = {
                    ItensCCe: JSON.stringify($("body").data("ItensCCe")),
                    CodigoCCe: $("body").data("CodigoCCe"),
                    CodigoCTe: $("body").data("CodigoCTe"),
                    Emitir: emitir,
                    DataEvento: $("#txtDataEvento").val()
                };

                executarRest("/CartaDeCorrecaoEletronica/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {

                        jAlert("Carta de Correção Eletrônica " + (emitir ? "emitida" : "salva") + " com sucesso!", "Sucesso", function (r) {
                            FecharTelaEmissaoCCe();
                        });

                        CarregarCCes();

                    } else {

                        if (r.Objeto != null && r.Objeto.Codigo > 0)
                            $("body").data("CodigoCCe", r.Objeto.Codigo);

                        ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgEmissaoCCe");

                    }
                });
            }
        }

        function ValidarCCe() {
            var itensCCe = $("body").data("ItensCCe") == null ? new Array() : $("body").data("ItensCCe");

            if (itensCCe.length <= 0) {
                ExibirMensagemAlerta("É necessário inserir ao menos um campo e seu respectivo valor para salvar a Carta de Correção Eletrônica.", "Atenção!", "placeholder-msgEmissaoCCe");
                return false;
            }

            return true;
        }
    </script>
    <script id="ScriptGridCCe" type="text/javascript">
        $(document).ready(function () {
            $("#btnConsultarCCe").click(function () {
                CarregarCCes();
            });
        });
        function CarregarCCes() {
            var opcoes = new Array();
            opcoes.push({ Descricao: "Editar", Evento: EditarCCe });
            opcoes.push({ Descricao: "Baixar XML", Evento: DownloadXML });            
            opcoes.push({ Descricao: "Baixar PDF", Evento: DownloadPDF });
            opcoes.push({ Descricao: "Visualizar", Evento: GerarRelartorio });

            CriarGridView("/CartaDeCorrecaoEletronica/Consultar?callback=?", { CodigoCTe: $("body").data("CodigoCTe"), inicioRegistros: 0 }, "tbl_cces_table", "tbl_cces", "tbl_paginacao_cces", opcoes, [0, 1], null);
        }
        function EditarCCe(cce) {
            executarRest("/CartaDeCorrecaoEletronica/ObterDetalhes?callback=?", { CodigoCCe: cce.data.Codigo, CodigoCTe: $("body").data("CodigoCTe") }, function (r) {
                if (r.Sucesso) {

                    AbrirTelaEmissaoCCe();

                    $("body").data("CodigoCCe", r.Objeto.Codigo);
                    $("body").data("ItensCCe", r.Objeto.Itens);

                    RenderizarItensCCe();

                    if (r.Objeto.Status != 9 && r.Objeto.Status != 0) {
                        $("#divEmissaoCCe .modal-body button").attr("disabled", true);
                    }
                } else {

                    ExibirMensagemErro(r.Erro, "Atenção!");

                }
            });
        }
        function DownloadXML(cce) {
            executarDownload("/CartaDeCorrecaoEletronica/DownloadXML", { CodigoCCe: cce.data.Codigo, CodigoCTe: $("body").data("CodigoCTe") });
        }
        function DownloadPDF(cce) {
            executarDownload("/CartaDeCorrecaoEletronica/DownloadPDF", { CodigoCCe: cce.data.Codigo, CodigoCTe: $("body").data("CodigoCTe") });
        }
        function GerarRelartorio(cce) {
            if (cce.data.Status == 3) {
                $("#hddCodigoCCe").val(cce.data.Codigo);
                $("#btnGerarRelatorio").trigger("click");
                $("#divRelatorio").modal({ keyboard: false, backdrop: 'static' });
            } else {
                ExibirMensagemAlerta("A Carta de Correção Eletrônica deve estar autorizada para ser visualizada.", "Atenção");
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <asp:Button ID="btnGerarRelatorio" runat="server" OnClick="btnGerarRelatorio_Click" ClientIDMode="Static" />
        <asp:HiddenField ID="hddCodigoCCe" runat="server" Value="0" ClientIDMode="Static" />
    </div>
    <div class="page-header">
        <h2>Carta de Correção Eletrônica
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">CT-e:
                </span>
                <input type="text" id="txtNumeroCTe" class="form-control" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Data Emissão:
                </span>
                <input type="text" id="txtDataEmissaoCTe" class="form-control" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Valor:
                </span>
                <input type="text" id="txtValorCTe" class="form-control" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Chave:
                </span>
                <input type="text" id="txtChaveCTe" class="form-control" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Remetente:
                </span>
                <input type="text" id="txtRemetenteCTe" class="form-control" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Destinatário:
                </span>
                <input type="text" id="txtDestinatarioCTe" class="form-control" disabled />
            </div>
        </div>
    </div>

    <div class="panel-group" id="panelInformacoes" style="margin-bottom: 10px; margin-top: 10px;">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#panelInformacoes" href="#infoArtigo58">Condições de Uso
                    </a>
                </h4>
            </div>
            <div id="infoArtigo58" class="panel-collapse collapse out">
                <div class="panel-body">
                    <p>
                        De acordo com o <a href="http://www1.fazenda.gov.br/confaz/confaz/Convenios/SINIEF/CVSINIEF_006_89.htm" target="_blank">Artigo 58-B do ajuste SINIEF 02/08</a>, com efeito a partir de 02/06/2008, fica permitida a utilização de carta de correção, para regularização de erro ocorrido na emissão de documentos fiscais relativos à prestação de serviço de transporte, desde que o erro não esteja relacionado com:       
                        <br />
                        <br />
                        I - as variáveis que determinam o valor do imposto tais como: base de cálculo, alíquota, diferença de preço, quantidade, valor da prestação;       
                        <br />
                        II - a correção de dados cadastrais que implique mudança do emitente, tomador, remetente ou do destinatário;       
                        <br />
                        III - a data de emissão ou de saída.
                    </p>
                </div>
            </div>
        </div>
    </div>

    <button type="button" id="btnVoltar" class="btn btn-default"><span class="glyphicon glyphicon-chevron-left"></span>&nbsp;Voltar</button>
    <button type="button" id="btnNovaCCe" class="btn btn-default"><span class="glyphicon glyphicon-new-window"></span>&nbsp;Nova Carta de Correção</button>
    <button type="button" id="btnConsultarCCe" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar / Buscar CC-e</button>
    <div id="tbl_cces" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_cces">
    </div>
    <div class="modal fade" id="divEmissaoCCe" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Emissão de Carta de Correção Eletrônica</h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgEmissaoCCe"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-5">
                            <div class="input-group">
                                <span class="input-group-addon">Data*:
                                </span>
                                <input type="text" id="txtDataEvento" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <hr />
                    <div class="row">
                        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Campo*:
                                </span>
                                <input type="text" id="txtCampoCCe" class="form-control" />
                                <div class="input-group-btn">
                                    <button type="button" id="btnConsultarCamposCCe" class="btn btn-primary">Buscar</button>
                                </div>
                            </div>
                        </div>
                        <div id="divValorSelecao" class="col-xs-12 col-sm-8 col-md-6 col-lg-6" style="display: none;">
                            <div class="input-group">
                                <span class="input-group-addon">Novo Valor*:
                                </span>
                                <select id="selValor" class="form-control">
                                </select>
                            </div>
                        </div>
                        <div id="divValor" class="col-xs-12 col-sm-8 col-md-6 col-lg-6" style="display: none;">
                            <div class="input-group">
                                <span class="input-group-addon">Novo Valor*:
                                </span>
                                <input type="text" id="txtValor" class="form-control" />
                            </div>
                        </div>
                        <div id="divSequencia" class="col-xs-12 col-sm-4 col-md-4 col-lg-4" style="display: none;">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Número sequêncial do item alterado">Nº Seq.</abbr>*:
                                </span>
                                <input type="text" id="txtSequenciaItem" class="form-control" />
                            </div>
                        </div>
                    </div>

                    <button type="button" id="btnSalvarValor" class="btn btn-primary">Salvar</button>
                    <button type="button" id="btnExcluirValor" class="btn btn-danger" style="display: none;">Excluir</button>
                    <button type="button" id="btnCancelarValor" class="btn btn-default">Cancelar</button>

                    <div class="table-responsive" style="margin-top: 10px; max-height: 400px; overflow-y: scroll;">
                        <table id="tblItensCCe" class="table table-bordered table-condensed table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 30%;">Campo
                                    </th>
                                    <th style="width: 45%;">Valor
                                    </th>
                                    <th style="width: 15%;">Nº Item
                                    </th>
                                    <th style="width: 10%;">Opções
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td colspan="4">Nenhum registro encontrado!
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnEmitirCCe" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Emitir CC-e</button>
                    <button type="button" id="btnSalvarCCe" class="btn btn-primary"><span class="glyphicon glyphicon-floppy-disk"></span>&nbsp;Salvar CC-e</button>
                    <button type="button" id="btnCancelarCCe" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
    <asp:ScriptManager ID="ScriptManager" EnableScriptGlobalization="true" runat="server">
    </asp:ScriptManager>
    <div class="modal fade" id="divRelatorio" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true" style="margin-right: 8px; margin-top: 5px;">&times;</button>
                <div class="modal-body" style="overflow-x: scroll;">
                    <asp:UpdatePanel ID="uppRelatorio" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <rsweb:ReportViewer ID="rvwRelatorioCCe" runat="server" AsyncRendering="true" ExportContentDisposition="AlwaysAttachment" EnableTheming="true" PromptAreaCollapsed="false"
                                ShowFindControls="false" ShowWaitControlCancelLink="false" ZoomMode="Percent" ZoomPercent="100" SizeToReportContent="true" ShowZoomControl="false">
                                <LocalReport ReportPath="Relatorios/RelatorioCCe.rdlc">
                                </LocalReport>
                            </rsweb:ReportViewer>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="rvwRelatorioCCe" />
                            <asp:AsyncPostBackTrigger ControlID="btnGerarRelatorio" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
