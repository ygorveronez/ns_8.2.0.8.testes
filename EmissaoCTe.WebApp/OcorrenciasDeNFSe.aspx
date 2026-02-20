<%@ Page Title="Cadastros de Ocorrências de NFS-es" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="OcorrenciasDeNFSe.aspx.cs" Inherits="EmissaoCTe.WebApp.OcorrenciasDeNFSe" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker",
                           "~/bundle/styles/plupload") %>

        <%: Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/datetimepicker",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/statecreator",
                           "~/bundle/scripts/validaCampos") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        $(document).ready(function () {
            FormatarCampoDateTime("txtDataDaOcorrencia");

            CarregarConsultaDeTiposDeOcorrenciasDeCTes("btnBuscarOcorrencia", "btnBuscarOcorrencia", RetornoConsultaTipoOcorrenciaCTe, true, false);
            CarregarConsultaDeNFSe("btnBuscarNFSe", "btnBuscarNFSe", "", RetornoConsultaNFSe, true, false);

            CarregarConsultaDeOcorrenciasDeNFSes("default-search", "default-search", RetornoConsultaOcorrenciaNFSe, true, false);

            RemoveConsulta($("#txtOcorrencia, #txtNFSe"), function ($this) {
                $this.val("");
                $this.data("Codigo", 0);
            });

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            $("#btnAnexar").click(function () {
                Anexar();
            });

            StateAnexos = new State({
                name: "anexos",
                id: "Codigo",
                render: RenderizarAnexos
            });

            LimparCampos();
        });

        var StateAnexos;
        var IdOcorrenciaNFSe;
        var UploadAnexos;

        function RetornoConsultaOcorrenciaNFSe(ocorrencia) {
            executarRest("/OcorrenciaDeNFSe/ObterDetalhes?callback=?", { Codigo: ocorrencia.Codigo }, function (r) {
                if (r.Sucesso) {
                    IdOcorrenciaNFSe = ocorrencia.Codigo;
                    $("#txtOcorrencia").data("Codigo", r.Objeto.CodigoTipoOcorrencia);
                    $("#txtOcorrencia").val(r.Objeto.DescricaoTipoOcorrencia);

                    $("#txtNFSe").data("Codigo", r.Objeto.CodigoNFSe);
                    $("#txtNFSe").val(r.Objeto.DescricaoNFSe);

                    $("#txtObservacao").val(r.Objeto.Observacao);
                    $("#txtDataDaOcorrencia").val(r.Objeto.DataDaOcorrencia);
                    $("#btnSalvar").attr("disabled", true);

                    // Anexos
                    StateAnexos.set(r.Objeto.Anexos);
                    $("#btnAnexar").removeClass("disabled");
                    $("#btnEmail").removeClass("disabled");
                    
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function RetornoConsultaTipoOcorrenciaCTe(tipoOcorrencia) {
            $("#txtOcorrencia").data("Codigo", tipoOcorrencia.Codigo);
            $("#txtOcorrencia").val(tipoOcorrencia.Descricao);
        }

        function RetornoConsultaNFSe(nfs) {
            $("#txtNFSe").data("Codigo", nfs.Codigo);
            $("#txtNFSe").val(nfs.Numero + "-" + nfs.Serie);
        }

        function SetarDadosPadrao() {
            $("#txtDataDaOcorrencia").val(Globalize.format(new Date(), "dd/MM/yyyy HH:mm"));
        }

        function LimparCampos() {
            $("#txtOcorrencia").data("Codigo", 0);
            $("#txtOcorrencia").val("");

            $("#txtNFSe").data("Codigo", 0);
            $("#txtNFSe").val("");

            $("#txtDataDaOcorrencia").val("");
            $("#txtObservacao").val("");
            $("#btnSalvar").attr("disabled", false);
            $("#btnAnexar").addClass("disabled");
            $("#btnEmail").addClass("disabled");            

            SetarDadosPadrao();
            StateAnexos.clear();
            IdOcorrenciaNFSe = 0;
        }

        function ValidarCampos() {
            var codigoNFSe = $("#txtNFSe").data("Codigo");
            var codigoTipoOcorrencia = $("#txtOcorrencia").data("Codigo");
            var dataOcorrencia = $("#txtDataDaOcorrencia").val();

            var valido = true;
            if (dataOcorrencia != "") {
                CampoSemErro("#txtDataDaOcorrencia");
            } else {
                CampoComErro("#txtDataDaOcorrencia");
                valido = false;
            }
            if (codigoNFSe > 0) {
                CampoSemErro("#txtNFSe");
            } else {
                CampoComErro("#txtNFSe");
                valido = false;
            }
            if (codigoTipoOcorrencia > 0) {
                CampoSemErro("#txtOcorrencia");
            } else {
                CampoComErro("#txtOcorrencia");
                valido = false;
            }
            return valido;
        }

        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    CodigoTipoOcorrencia: $("#txtOcorrencia").data("Codigo"),
                    CodigoNFSe: $("#txtNFSe").data("Codigo"),
                    DataDaOcorrencia: $("#txtDataDaOcorrencia").val(),
                    Observacao: $("#txtObservacao").val()
                };
                executarRest("/OcorrenciaDeNFSe/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        BuscarOcorrencia(r.Objeto.Codigo);//LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!");
            }
        }

        function BuscarOcorrencia(codigo) {
            executarRest("/OcorrenciaDeNFSe/ObterDetalhes?callback=?", { Codigo: codigo }, function (r) {
                if (r.Sucesso) {
                    IdOcorrenciaNFSe = codigo;
                    $("#txtOcorrencia").data("Codigo", r.Objeto.CodigoTipoOcorrencia);
                    $("#txtOcorrencia").val(r.Objeto.DescricaoTipoOcorrencia);

                    $("#txtNFSe").data("Codigo", r.Objeto.CodigoNFSe);
                    $("#txtNFSe").val(r.Objeto.DescricaoNFSe);

                    $("#txtObservacao").val(r.Objeto.Observacao);
                    $("#txtDataDaOcorrencia").val(r.Objeto.DataDaOcorrencia);
                    $("#btnSalvar").attr("disabled", true);

                    // Anexos
                    StateAnexos.set(r.Objeto.Anexos);
                    $("#btnAnexar").removeClass("disabled");
                    $("#btnEmail").removeClass("disabled");

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function Anexar() {
            if (IdOcorrenciaNFSe == 0)
                return jAlert("É preciso salvar a ocorrência antes de enviar anexos.", "Anexos indisponíveis")
            UploadAnexos = AbrirUploadPadrao({
                title: "Anexar arquivos",
                url: "/OcorrenciaDeNFSe/Anexar?callback=?&Codigo=" + IdOcorrenciaNFSe,
                onFinish: function (arquivos, erros) {
                    if (arquivos.length > 0) {
                        for (var i in arquivos)
                            StateAnexos.insert(arquivos[i]);
                    }

                    if (erros.length > 0) {
                        var uuErros = [];
                        for (var i in erros)
                            if ($.inArray(erros[i], uuErros) < 0)
                                uuErros.push(erros[i]); 
                        ExibirMensagemErro(uuErros.join("<br>"), "Erro no envio de anexo:<br>");
                    }
                }
            });
        }

        function ExcluirAnexo(anexo) {
            jConfirm("Tem certeza que deseja excluir esse anexo?", "Excluir Anexo", function (res) {
                if (res) {
                    executarRest("/OcorrenciaDeNFSe/ExcluirAnexo?callback=?", anexo, function (r) {
                        if (r.Sucesso) {
                            StateAnexos.remove({ Codigo: anexo.Codigo });
                            ExibirMensagemSucesso("Excluído com sucesso.", "Sucesso!");
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção");
                        }
                    });
                }
            });
        }

        function DownloadAnexo(anexo) {
            executarDownload("/OcorrenciaDeNFSe/DownloadAnexo", anexo);
        }

        function RenderizarAnexos() {
            var itens = StateAnexos.get();
            var $tabela = $("#tblAnexos");

            $tabela.find("tbody").html("");

            itens.forEach(function (info) {
                if (!info.Excluir) {
                    var $row = $("<tr>" +
                        "<td><a href='#' class='download'>" + info.Nome + "</button></td>" +
                        "<td><button type='button' class='btn btn-default btn-xs btn-block'>Excluir</button></td>" +
                    "</tr>");

                    $row.on("click", "button", function () {
                        ExcluirAnexo(info);
                    });

                    $row.on("click", ".download", function (e) {
                        if (e && e.preventDefault) e.preventDefault();
                        DownloadAnexo(info);
                    });

                    $tabela.find("tbody").append($row);
                }
            });

            if ($tabela.find("tbody tr").length == 0)
                $tabela.find("tbody").html("<tr><td class='text-center' colspan='" + $tabela.find("thead th").length + "'>Nenhum registro encontrado.</td></tr>");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro de Ocorrências de NFS-es
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Data Ocorrência*:
                </span>
                <input type="text" id="txtDataDaOcorrencia" class="form-control" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">NFS-e / Série*:
                </span>
                <input type="text" id="txtNFSe" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarNFSe" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Ocorrência*:
                </span>
                <input type="text" id="txtOcorrencia" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarOcorrencia" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Observação:
                </span>
                <textarea id="txtObservacao" class="form-control" rows="3"></textarea>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="table-responsive" style="margin-top: 10px;">
                <table id="tblAnexos" class="table table-bordered table-condensed table-hover">
                    <thead>
                        <tr>
                            <th style="width: 85%;" colspan="1" rowspan="1">Arquivo</th>
                            <th style="width: 15%;" colspan="1" rowspan="1">Opções</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>

    <button type="button" id="btnAnexar" class="btn btn-primary">
        <span class="glyphicon glyphicon-file"></span>&nbsp;Anexar
    </button>
</asp:Content>
