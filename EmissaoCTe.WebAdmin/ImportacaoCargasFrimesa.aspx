<%@ Page Title="Importação Cargas Frimesa" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ImportacaoCargasFrimesa.aspx.cs" Inherits="EmissaoCTe.WebAdmin.ImportacaoCargasFrimesa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/Fancybox/jquery.fancybox.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-buttons.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-thumbs.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/plupload/jquery.plupload.queue.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui/ui.datepicker.min.css" rel="stylesheet" type="text/css" />
    <script defer="defer" src="Scripts/json2.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/validaCampos.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/ui/ui.datepicker.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.ui.datepicker-pt-BR.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.priceformat.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-buttons.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-thumbs.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-media.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/plupload.full.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/jquery.plupload.queue.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/pt-br.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/json2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            CarregarConsultadeClientes("btnBuscarEmbarcador", "btnBuscarEmbarcador", RetornoConsultaEmbarcador, true, false, "");

            $("#txtEmbarcador").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoEmbarcador").val("");
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtDataCarga").mask("99/99/9999");
            $("#txtDataCarga").datepicker({ changeMonth: true, changeYear: true });

            $("#btnImportar").click(function () {
                Importar();
            });

            $("#btnAtualizar").click(function () {
                Carregar();
            });

            $("#btnBaixarLoteDACTE").click(function () {
                BaixarLoteDACTE();
            });

            $("#btnBaixarLoteXML").click(function () {
                BaixarLoteXML();
            });

            $("#btnBaixarLoteDANFSE").click(function () {
                BaixarLoteDANFSE();
            });

            $("#btnBaixarLoteXMLNFSe").click(function () {
                BaixarLoteXMLNFSe();
            });

            $("#btnDeletar").click(function () {
                Deletar();
            });

            InicializarPlUpload();
            LimparCampos();
            Carregar();
        });

        function Deletar() {
            if (ValidarCamposDeletar()) {
                executarRest("/ImportacaoCargasFrimesa/DeletarCargas?callback=?", { Data: $("#txtDataCarga").val(), Embarcador: $("#hddCodigoEmbarcador").val() }, function (r) {
                    if (r.Sucesso) {
                        jAlert("Carga excluida com sucesso!", "Sucesso");
                        Carregar();
                    } else {
                        jAlert(r.Erro, "Atenção");
                    }
                });
            }
        }

        function Carregar() {
            CriarGridView("/ImportacaoCargasFrimesa/Consultar?callback=?&Data=" + $("#txtDataCarga").val() + "&Placa=" + $("#txtPlacaVeiculo").val() + "&Embarcador=" + $("#hddCodigoEmbarcador").val(), { inicioRegistros: 0 }, "tbl_cargas_table", "tbl_cargas", "tbl_paginacao_cargas", [{ Descricao: "Acessar", Evento: AcessarSistema }, { Descricao: "Emitir", Evento: Emitir }, { Descricao: "PDF", Evento: DownloadPDF }, { Descricao: "XML", Evento: DownloadXML }], [0, 1, 2, 3]);
        }

        function AcessarSistema(carga) {
            if (carga.data.StatusEmpresa == "I")
                jAlert("Transportador está Inativo, não é possível acessar.");
            else {
                executarRest("/Empresa/BuscarDadosParaAcesso?callback=?", { CodigoEmpresa: carga.data.CodigoEmpresa }, function (r) {
                    if (r.Sucesso) {
                        var win = window.open();
                        var uriAcesso = "http://" + window.location.host + "/" + r.Objeto.UriAcesso + "?x=" + r.Objeto.Login + "&y=" + r.Objeto.Senha + "&z=" + r.Objeto.Usuario;
                        win.location = uriAcesso;
                        win.focus();
                    } else {
                        jAlert(r.Erro, "Atenção");
                    }
                });
            }
        }

        function Emitir(carga) {
            if (carga.data.StatusEmpresa == "I")
                jAlert("Transportador está Inativo, não é possível emitir.");
            else
                if (carga.data.CodigoDocumento == 0)
                    jAlert("Documento não foi gerado.");
                else
                    if (carga.data.ValorFrete == "0,00" || carga.data.ValorFrete == "0,01" || carga.data.ValorFrete == "1,00")
                        jAlert("Favor Acessar a transportadora para informar o valor do frete manualmente.");
                    else {
                        executarRest("/ImportacaoCargasFrimesa/EmitirDocumentos?callback=?", { Codigo: carga.data.Codigo, AjustarGlobalizado: "SIM" }, function (r) {
                            if (r.Sucesso) {
                                jAlert("Emitido com sucesso!", "Sucesso");
                                Carregar();
                            } else {
                                jAlert(r.Erro, "Atenção");
                            }
                        });
                    }
        }

        function DownloadPDF(carga) {
            if (carga.data.CodigoDocumento == 0)
                jAlert("Documento não foi emitido");
            $("#ifrDownload").attr("src", "ImportacaoCargasFrimesa/DownloadPDF?Codigo=" + carga.data.Codigo);
        }

        function DownloadXML(carga) {
            if (carga.data.CodigoDocumento == 0)
                jAlert("Documento não foi emitido");
            $("#ifrDownload").attr("src", "ImportacaoCargasFrimesa/DownloadXML?Codigo=" + carga.data.Codigo);
        }

        function BaixarLoteDACTE() {
            var data = $("#txtDataCarga").val();

            if (data != "") {
                CampoSemErro("#txtDataCarga");

                $("#ifrDownload").attr("src", "ImportacaoCargasFrimesa/DownloadLoteDACTE?callback=?&Data=" + $("#txtDataCarga").val() + "&Placa=" + $("#txtPlacaVeiculo").val() + "&Embarcador=" + $("#hddCodigoEmbarcador").val());
            } else {
                CampoComErro("#txtDataCarga");
                jAlert("Necessário informar a data da carga.");
                valido = false;
            }
        }

        function BaixarLoteXML() {
            var data = $("#txtDataCarga").val();

            if (data != "") {
                CampoSemErro("#txtDataCarga");

                $("#ifrDownload").attr("src", "ImportacaoCargasFrimesa/DownloadLoteXML?callback=?&Data=" + $("#txtDataCarga").val() + "&Placa=" + $("#txtPlacaVeiculo").val() + "&Embarcador=" + $("#hddCodigoEmbarcador").val());
            } else {
                CampoComErro("#txtDataCarga");
                jAlert("Necessário informar a data da carga.");
                valido = false;
            }
        }

        function BaixarLoteDANFSE() {
            var data = $("#txtDataCarga").val();

            if (data != "") {
                CampoSemErro("#txtDataCarga");

                $("#ifrDownload").attr("src", "ImportacaoCargasFrimesa/DownloadLoteDANFSE?callback=?&Data=" + $("#txtDataCarga").val() + "&Placa=" + $("#txtPlacaVeiculo").val() + "&Embarcador=" + $("#hddCodigoEmbarcador").val());
            } else {
                CampoComErro("#txtDataCarga");
                jAlert("Necessário informar a data da carga.");
                valido = false;
            }
        }

        function BaixarLoteXMLNFSe() {
            var data = $("#txtDataCarga").val();

            if (data != "") {
                CampoSemErro("#txtDataCarga");

                $("#ifrDownload").attr("src", "ImportacaoCargasFrimesa/DownloadLoteXMLNFSe?callback=?&Data=" + $("#txtDataCarga").val() + "&Placa=" + $("#txtPlacaVeiculo").val() + "&Embarcador=" + $("#hddCodigoEmbarcador").val());
            } else {
                CampoComErro("#txtDataCarga");
                jAlert("Necessário informar a data da carga.");
                valido = false;
            }
        }

        function Importar() {
            if (ValidarCampos()) {

                uploader.settings.url = 'ImportacaoCargasFrimesa/EnviarCargas?callback=?&Data=' + $("#txtDataCarga").val();

                $('#divListaArquivos a').each(function () {
                    $(this).remove();
                });

                uploader.start();

                $.fancybox.close();
            };
        }

        function LimparCampos() {
            $("#hddCodigo").val('0');
            $("#txtDataCarga").val('');
            $("#txtPlacaVeiculo").val('');
            $("#divListaArquivos").html("");
        }

        function ValidarCampos() {
            var data = $("#txtDataCarga").val();
            var valido = true;

            if (data != "") {
                CampoSemErro("#txtDataCarga");
            } else {
                CampoComErro("#txtDataCarga");
                jAlert("Necessário informar a data da carga, geralmente esta data é o dia seguinte.");
                valido = false;
            }
            return valido;
        }

        function ValidarCamposDeletar() {
            var data = $("#txtDataCarga").val();
            var valido = true;

            if (data != "") {
                CampoSemErro("#txtDataCarga");
            } else {
                CampoComErro("#txtDataCarga");
                jAlert("Necessário informar a data da carga, somente será deletado se não tiver documentos emitidos.");
                valido = false;
            }
            return valido;
        }

        var uploader = null;
        var documentos = new Array();
        var erros = "";

        function InicializarPlUpload() {
            documentos = new Array();
            erros = "";
            uploader = new plupload.Uploader({
                runtimes: 'gears,html5,flash,silverlight,browserplus',
                browse_button: 'btnBuscarArquivo',
                container: 'divArquivosSelecionados',
                max_file_size: '10000kb',
                multi_selection: true,
                flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
                silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
                filters: [{ title: "Arquivos", extensions: "*" }],
            });

            uploader.init();

            uploader.bind('FilesAdded', function (up, files) {
                $('#divListaArquivos').html("");

                $.each(uploader.files, function (i, file) {
                    $('#divListaArquivos').append('<div id="' + file.id + '">Arquivo selecionado: ' + file.name + ' - <b></b><a href="javascript:void(0);" onclick="RemoverArquivo(\'' + file.id + '\');">Remover</a></div>');
                });

                up.refresh();
            });

            uploader.bind('UploadProgress', function (up, file) {
                $('#' + file.id + " b").html("   (" + file.percent + "%)");
            });

            uploader.bind('Error', function (up, err) {
                $('#divListaArquivos').html("<div>Erro: " + err.code + " - " + err.message + (err.file ? ", Arquivo: " + err.file.name : "") + "</div>");

                up.refresh();
            });

            uploader.bind('FileUploaded', function (up, file, response) {
                $('#' + file.id + " b").html("   (100%)");

                var retorno = JSON.parse(response.response.replace(");", "").replace("?(", ""));
                if (!retorno.Sucesso)
                    erros += retorno.Erro + "<br />"

            });

            uploader.bind('StateChanged', function (up) {
                if (up.state != plupload.STARTED) {
                    if (erros != "") {
                        jAlert("Ocorreram as seguintes falhas no envio dos arquivos: <br /><br />" + erros + "<br />", "Atenção", function () {
                            uploader.splice(0, uploader.files.length);
                            uploader.destroy();
                            $("#divListaArquivos").html("");
                            InicializarPlUpload();
                        });
                    }
                    else {
                        uploader.splice(0, uploader.files.length);
                        uploader.destroy();
                        $("#divListaArquivos").html("");
                        InicializarPlUpload();
                        Carregar();
                    }
                }
            });
        }

        function RemoverArquivo(id) {
            uploader.removeFile(uploader.getFile(id));
            $("#" + id).remove();
        }

        function RetornoConsultaEmbarcador(embarcador) {
            $("#hddCodigoEmbarcador").val(embarcador.CPFCNPJ);
            $("#txtEmbarcador").val(embarcador.CPFCNPJ + " - " + embarcador.Nome);
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <asp:HiddenField ID="hddCodigo" Value="0" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hddCodigoEmbarcador" Value="" runat="server" ClientIDMode="Static" />
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Importação Cargas Frimesa
                </h3>
            </div>
            <div class="content-box">
                <div class="form">
                    <div class="fields" style="margin-top: 15px;">
                        <div class="response-msg error ui-corner-all" id="divMensagemErro" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="response-msg notice ui-corner-all" id="divMensagemAlerta" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="response-msg success ui-corner-all" id="divMensagemSucesso" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldum">
                                <div class="label">
                                    <label>
                                        Data Carga*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDataCarga" />
                                </div>
                            </div>
                            <div class="buttons" style="margin-left: 5px;">
                                <input type="button" id="btnBuscarArquivo" value="Selecionar Excel" />
                                <input type="button" id="btnImportar" value="Importar" />
                                <input type="button" id="btnDeletar" value="Deletar Carga" />
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div id="divArquivosSelecionados">
                                <div id="divListaArquivos" class="fieldzao" style="overflow-y: scroll; height: 15px; margin-top: 10px;">
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Embarcador:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtEmbarcador" />
                                </div>
                            </div>
                            <div class="field fieldum" style="width: 65px;">
                                <div class="buttons">
                                    <input type="button" id="btnBuscarEmbarcador" value="Buscar" />
                                </div>
                            </div>
                            <div class="field fieldum">
                                <div class="label">
                                    <label>
                                        Placa do Veículo:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtPlacaVeiculo" />
                                </div>
                            </div>
                            <div class="buttons" style="margin-left: 5px;">
                                <input type="button" id="btnAtualizar" value="Atualizar Lista" />
                                <input type="button" id="btnBaixarLoteDACTE" value="Baixar Lote DACTE" />
                                <input type="button" id="btnBaixarLoteXML" value="Baixar Lote XML CT-e" />

                                <input type="button" id="btnBaixarLoteDANFSE" value="Baixar Lote DANFSE" />
                                <input type="button" id="btnBaixarLoteXMLNFSe" value="Baixar Lote XML NFS-e" />
                            </div>
                            <div class="table" style="margin-left: 5px;">
                                <div id="tbl_cargas">
                                </div>
                                <div id="tbl_paginacao_cargas" class="pagination">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div style="display: none;">
        <iframe id="ifrDownload" src=""></iframe>
    </div>
</asp:Content>
