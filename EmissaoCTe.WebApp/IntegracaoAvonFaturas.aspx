<%@ Page Title="Emissão de Faturas Avon" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="IntegracaoAvonFaturas.aspx.cs" Inherits="EmissaoCTe.WebApp.IntegracaoAvonFaturas" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker") %>
        <%: Scripts.Render("~/bundle/scripts/json",
                           "~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaDeManifestosAvon("btnAdicionarManifesto", "btnAdicionarManifesto", "2", RetornoConsultaManifesto, false, false);

            $("#btnNovaFatura").click(function () {
                AbrirTelaEmissaoFatura();
            });

            $("#btnCancelar").click(function () {
                FecharTelaEmissaoFatura();
            });

            $("#btnEmitirFatura").click(function () {
                EmitirFatura();
            });

            $("#btnAtualizarGridFaturas").click(function () {
                AtualizarGridFaturas();
            });

            $("#txtDataVencimento").datepicker();
            $("#txtDataVencimento").mask("99/99/9999");

            $("#txtDataInicial").datepicker();
            $("#txtDataInicial").mask("99/99/9999");

            $("#txtDataFinal").datepicker();
            $("#txtDataFinal").mask("99/99/9999");

            $("#txtNumeroInicial").mask("9?9999999999");
            $("#txtNumeroFinal").mask("9?9999999999");

            AtualizarGridFaturas();
        });

        function AtualizarGridFaturas() {
            var dados = {
                NumeroInicial: $("#txtNumeroInicial").val(),
                NumeroFinal: $("#txtNumeroFinal").val(),
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                inicioRegistros: 0
            };

            var opcoes = new Array();
            opcoes.push({ Descricao: "Consultar", Evento: ConsultarStatus });
            opcoes.push({ Descricao: "Enviar Novamente", Evento: EnviarNovamente });
            opcoes.push({ Descricao: "Baixar Lote CT-e", Evento: DownloadLoteCTe });
            opcoes.push({ Descricao: "Baixar Requisição", Evento: DownloadArquivosFatura });
            opcoes.push({ Descricao: "Quitar Fatura", Evento: QuitarFatura });
            opcoes.push({ Descricao: "Cancelar Fatura", Evento: CancelarFatura });
            opcoes.push({ Descricao: "Download Detalhes", Evento: DownloadDetalhesFatura });

            CriarGridView("/IntegracaoAvonFaturas/Consultar?callback=?", dados, "tbl_faturas_table", "tbl_faturas", "tbl_paginacao_faturas", opcoes, [0], null);
        }

        function EmitirFatura() {
            if (ValidarDados()) {

                var manifestos = $("body").data("manifestos");
                var codigosManifestos = new Array();

                for (var i = 0; i < manifestos.length; i++)
                    codigosManifestos.push(manifestos[i].Codigo);

                var dados = {
                    DataVencimento: $("#txtDataVencimento").val(),
                    Manifestos: JSON.stringify(codigosManifestos)
                };

                executarRest("/IntegracaoAvonFaturas/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        AtualizarGridFaturas();
                        FecharTelaEmissaoFatura();
                        LimparCampos();
                        ExibirMensagemSucesso("Fatura emitida com sucesso!", "Sucesso!");
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderFatura");
                    }
                });
            }
        }

        function ValidarDados() {
            var dataVencimento = $("#txtDataVencimento").val();
            var manifestos = $("body").data("manifestos");
            var valido = true;

            if (dataVencimento != null && dataVencimento != "") {
                CampoSemErro("#txtDataVencimento");
            } else {
                CampoComErro("#txtDataVencimento");
                valido = false;
            }

            if (!valido) {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!", "messages-placeholderFatura");
            } else if (manifestos == null || manifestos.length == 0) {
                ExibirMensagemAlerta("Adicione um ou mais manifestos para emitir a fatura.", "Atenção!", "messages-placeholderFatura");
                valido = false;
            }

            return valido;
        }

        function RetornoConsultaManifesto(manifesto) {
            executarRest("/IntegracaoAvon/ObterDetalhes?callback=?", { CodigoManifesto: manifesto.Codigo }, function (r) {
                if (r.Sucesso) {

                    var manifestos = $("body").data("manifestos") == null ? new Array() : $("body").data("manifestos");

                    for (var i = 0; i < manifestos.length; i++)
                        if (manifesto.Codigo == manifestos[i].Codigo)
                            return;

                    manifestos.push(r.Objeto);
                    $("body").data("manifestos", manifestos);

                    RenderizarManifestos(manifestos);
                    AtualizarValores(manifestos);

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderFatura");
                }
            });
        }

        function ExcluirManifesto(manifesto) {
            jConfirm("Deseja realmente excluir este manifesto?", "Atenção!", function (r) {
                if (r) {
                    var manifestos = $("body").data("manifestos") == null ? new Array() : $("body").data("manifestos");

                    for (var i = 0; i < manifestos.length; i++)
                        if (manifesto.Codigo == manifestos[i].Codigo)
                            manifestos.splice(i, 1);

                    $("body").data("manifestos", manifestos);

                    RenderizarManifestos(manifestos);
                    AtualizarValores(manifestos);
                }
            });
        }

        function AtualizarValores(manifestos) {
            var quantidade = 0, valor = 0;

            for (var i = 0; i < manifestos.length; i++) {
                quantidade += manifestos[i].QuantidadeCTe;
                valor += manifestos[i].ValorCTe;
            }

            $("#txtValorFatura").val(Globalize.format(valor, "n2"));
            $("#txtQuantidadeCTes").val(Globalize.format(quantidade, "n0"));
        }

        function RenderizarManifestos(manifestos) {
            $("#tblManifestos tbody").html("");

            for (var i = 0; i < manifestos.length; i++)
                $("#tblManifestos tbody").append("<tr><td>" + manifestos[i].Numero + "</td><td>" + Globalize.format(manifestos[i].ValorCTe, "n2") + "</td><td>" + manifestos[i].QuantidadeCTe + "</td><td><button type='button' onclick='ExcluirManifesto(" + JSON.stringify(manifestos[i]) + ");' class='btn btn-default btn-xs btn-block'>Excluir</button></td></tr>");

            if ($("#tblManifestos tbody").html() == "")
                $("#tblManifestos tbody").append("<tr><td colspan='4'>Nenhum registro encontrado!</td></tr>");
        }

        function LimparCampos() {
            $("#txtValorFatura").val("0,00");
            $("#txtQuantidadeCTes").val("0");

            $("body").data("manifestos", new Array());
            RenderizarManifestos(new Array());

            var hoje = new Date();
            var dataVencimento = new Date(hoje);
            dataVencimento.setDate(hoje.getDate() + 100);
            $("#txtDataVencimento").val(Globalize.format(dataVencimento, "dd/MM/yyyy"));
        }

        function AbrirTelaEmissaoFatura() {
            LimparCampos();

            $("#divEmissaoFatura").modal({ keyboard: false, backdrop: 'static' });
        }

        function FecharTelaEmissaoFatura() {
            $("#divEmissaoFatura").modal('hide');
        }

        function ConsultarStatus(fatura) {
            jConfirm("Deseja consultar a fatura " + fatura.data.Numero + "?", "Atenção!", function (confirma) {
                if (confirma) {
                    executarRest("/IntegracaoAvonFaturas/ConsultaFatura?callback=?", { CodigoFatura: fatura.data.Codigo }, function (r) {
                        if (r.Sucesso) {
                            AtualizarGridFaturas();
                            ExibirMensagemSucesso("Consulta realizada com sucesso!", "Sucesso!");
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!");
                        }
                    });
                }
            });
        }

        function EnviarNovamente(fatura) {
            jConfirm("Deseja enviar novamente a fatura " + fatura.data.Numero + "?", "Atenção!", function (confirma) {
                if (confirma) {
                    executarRest("/IntegracaoAvonFaturas/EnviarFatura?callback=?", { CodigoFatura: fatura.data.Codigo }, function (r) {
                        if (r.Sucesso) {
                            AtualizarGridFaturas();
                            ExibirMensagemSucesso("Envio realizado com sucesso!", "Sucesso!");
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!");
                        }
                    });
                }
            });
        }

        function DownloadLoteCTe(fatura) {
            executarDownload("/IntegracaoAvonFaturas/DownloadLoteCTe", { CodigoFatura: fatura.data.Codigo });
        }

        function DownloadArquivosFatura(fatura) {
            executarDownload("/IntegracaoAvonFaturas/DownloadArquivosFatura", { CodigoFatura: fatura.data.Codigo });
        }
    </script>
    <script id="CancelarFatura" type="text/javascript">
        function CancelarFatura(fatura) {
            jConfirm("Deseja realmente cancelar a fatura <b>" + fatura.data.Numero + "</b>?", "Atenção!", function (retorno) {
                if (retorno) {
                    executarRest("/IntegracaoAvonFaturas/CancelarFatura?callback=?", { CodigoFatura: fatura.data.Codigo }, function (r) {
                        if (r.Sucesso) {
                            AtualizarGridFaturas();
                            ExibirMensagemSucesso("Fatura cancelada com sucesso!", "Sucesso!");
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!");
                        }
                    });
                }
            });
        }
    </script>
    <script id="QuitarFaturas" type="text/javascript">
        function QuitarFatura(fatura) {
            jConfirm("Deseja realmente quitar a fatura <b>" + fatura.data.Numero + "</b>?", "Atenção!", function (retorno) {
                if (retorno) {
                    executarRest("/IntegracaoAvonFaturas/QuitarFatura?callback=?", { CodigoFatura: fatura.data.Codigo }, function (r) {
                        if (r.Sucesso) {
                            AtualizarGridFaturas();
                            ExibirMensagemSucesso("A fatura está em processo quitação. Isto pode levar alguns minutos...", "Quitando!");
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!");
                        }
                    });
                }
            });
        }
    </script>
    <script id="DownloadDetalhesFatura" type="text/javascript">
        function DownloadDetalhesFatura(fatura) {
            executarDownload("/IntegracaoAvonFaturas/DownloadDetalhesFatura", { CodigoFatura: fatura.data.Codigo });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Emissão de Faturas para a Avon
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <button type="button" id="btnNovaFatura" class="btn btn-default"><span class="glyphicon glyphicon-new-window"></span>&nbsp;Emitir Fatura</button>
    <div class="row" style="margin-top: 10px; margin-bottom: 5px;">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Nº Inicial:
                </span>
                <input type="text" id="txtNumeroInicial" class="form-control" maxlength="10" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Nº Final:
                </span>
                <input type="text" id="txtNumeroFinal" class="form-control" maxlength="10" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtDataInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtDataFinal" class="form-control" />
            </div>
        </div>
    </div>
    <button type="button" id="btnAtualizarGridFaturas" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Buscar / Atualizar Faturas</button>
    <div id="tbl_faturas" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_faturas">
    </div>
    <div class="clearfix"></div>
    <div class="modal fade" id="divEmissaoFatura" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Emissão de Fatura</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholderFatura">
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Data Vcto.*:
                                </span>
                                <input type="text" id="txtDataVencimento" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Vl. Fatura:
                                </span>
                                <input type="text" id="txtValorFatura" class="form-control" disabled="disabled" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Qtd. CT-e:
                                </span>
                                <input type="text" id="txtQuantidadeCTes" class="form-control" disabled="disabled" />
                            </div>
                        </div>
                    </div>
                    <button type="button" id="btnAdicionarManifesto" class="btn btn-primary">Adicionar Manifesto</button>
                    <table id="tblManifestos" class="table table-bordered table-condensed table-hover table-responsive" style="margin-top: 10px;">
                        <thead>
                            <tr>
                                <th style="width: 20%;">Número
                                </th>
                                <th style="width: 20%;">Valor
                                </th>
                                <th style="width: 50%;">Qtd. CT-e
                                </th>
                                <th>Opções
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnEmitirFatura" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Emitir Fatura</button>
                    <button type="button" id="btnCancelar" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
