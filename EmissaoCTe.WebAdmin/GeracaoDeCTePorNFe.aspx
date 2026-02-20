<%@ Page Title="Geração de CT-e por NF-e" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="GeracaoDeCTePorNFe.aspx.cs" Inherits="EmissaoCTe.WebAdmin.GeracaoDeCTePorNFe" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/Fancybox/jquery.fancybox.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-buttons.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-thumbs.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/plupload/jquery.plupload.queue.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui/ui.datepicker.min.css" rel="stylesheet" type="text/css" />
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/validaCampos.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.priceformat.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-buttons.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-thumbs.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-media.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/ui/ui.datepicker.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.ui.datepicker-pt-BR.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/plupload.full.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/jquery.plupload.queue.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/pt-br.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/json2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $("#txtValorFrete").priceFormat({ prefix: '' });
            $("#txtValorTotalMercadoria").priceFormat({ prefix: '' });

            $("#txtDataEmissao").mask("99/99/9999 99:99");
            $("#txtDataEmissaoFiltro").mask("99/99/9999");
            $("#txtDataEmissaoFiltro").datepicker({ changeMonth: true, changeYear: true });

            CarregarCTes();

            $("#btnBuscarCTes").click(function () {
                CarregarCTes();
            });

            $("#btnFiltrar").click(function () {
                CarregarCTes();
            });

            LimparCampos();
        });

        function CarregarCTes() {
            CriarGridView("/UsuarioCTe/Consultar?callback=?", { inicioRegistros: 0, NumeroCTe: $("#txtNumeroFiltro").val(), SerieCTe: $("#txtSerieFiltro").val(), DataEmissao: $("#txtDataEmissaoFiltro").val(), Empresa: $("#txtEmpresaFiltro").val() }, "tbl_ctes_table", "tbl_ctes", "tbl_paginacao_ctes", [{ Descricao: "Reemitir", Evento: Reemitir }, { Descricao: "PDF", Evento: DownloadDACTE }, { Descricao: "XML", Evento: DownloadXML }], [0, 1]);
        }

        function DownloadDACTE(doc) {
            if (doc.data.Tipo == "CT-e")
                $("#ifrDownload").attr("src", "ConhecimentoDeTransporteEletronico/DownloadDacte?CodigoCTe=" + doc.data.Codigo + "&CodigoEmpresa=" + doc.data.CodigoEmpresa);
            else if (doc.data.Tipo == "NFs-e")
                $("#ifrDownload").attr("src", "NotaFiscalDeServicosEletronica/DownloadDANFSE?CodigoNFSe=" + doc.data.Codigo + "&CodigoEmpresa=" + doc.data.CodigoEmpresa);
        }

        function DownloadXML(doc) {
            if (doc.data.Tipo == "CT-e")
                $("#ifrDownload").attr("src", "ConhecimentoDeTransporteEletronico/DownloadXML?CodigoCTe=" + doc.data.Codigo + "&CodigoEmpresa=" + doc.data.CodigoEmpresa);
            else if (doc.data.Tipo == "NFs-e")
                $("#ifrDownload").attr("src", "NotaFiscalDeServicosEletronica/DownloadXMLAutorizacao?CodigoNFSe=" + doc.data.Codigo + "&CodigoEmpresa=" + doc.data.CodigoEmpresa);
        }

        function Reemitir(doc) {
            if (doc.data.Tipo == "CT-e") {
                if (doc.data.DescricaoStatus != "Rejeição")
                    jAlert("Status não permite reemitir CT-e.");
                else {
                    executarRest("/ConhecimentoDeTransporteEletronico/Emitir?callback=?", { CodigoCTe: doc.data.Codigo }, function (r) {
                        if (r.Sucesso) {
                            jAlert("CT-e reemitido com sucesso!", "Sucesso");
                            CarregarCTes();
                        } else {
                            jAlert(r.Erro, "Atenção");
                        }
                    });
                }
            }
            else if (doc.data.Tipo == "NFs-e") {
                if (doc.data.DescricaoStatus != "Rejeição")
                    jAlert("Status não permite reemitir NFs-e.");
                else {
                    executarRest("/NotaFiscalDeServicosEletronica/Emitir?callback=?", { Codigo: doc.data.Codigo }, function (r) {
                        if (r.Sucesso) {
                            jAlert("NFs-e reemitida com sucesso!", "Sucesso");
                            CarregarCTes();
                        } else {
                            jAlert(r.Erro, "Atenção");
                        }
                    });
                }
            }

        }

        var uploader = null;
        var documentos = new Array();
        var erros = "";

        function InicializarPlUpload() {
            documentos = new Array();
            erros = "";
            uploader = new plupload.Uploader({
                runtimes: 'gears,html5,flash,silverlight,browserplus',
                browse_button: 'btnSelecionarArquivos',
                container: 'divArquivosSelecionados',
                max_file_size: '500kb',
                multi_selection: true,
                url: 'XMLNotaFiscalEletronica/ObterDocumento?callback=?',
                flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
                silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
                filters: [{ title: "Arquivos XML", extensions: "xml" }],
            });

            $('#btnGerarCTe').click(function (e) {
                if (ValidarDados()) {
                    if (uploader.files.length > 0) {

                        $('#divListaArquivos a').each(function () {
                            $(this).remove();
                        });

                        uploader.start();

                        e.preventDefault();

                    } else {
                        ExibirMensagemAlerta("O total de NF-es selecionadas é inválido para a geração do CT-e.", "Atenção!");
                    }
                }
            });

            uploader.init();

            uploader.bind('FilesAdded', function (up, files) {
                $('#divListaArquivos').html("");

                $.each(uploader.files, function (i, file) {
                    $('#divListaArquivos').append('<div id="' + file.id + '">NF-e selecionada: ' + file.name + ' - <b></b><a href="javascript:void(0);" onclick="RemoverArquivo(\'' + file.id + '\');">Remover</a></div>');
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

                if (retorno.Sucesso) {
                    documentos.push({
                        NFe2: retorno.Objeto.versao == "2.00" ? retorno.Objeto : null,
                        NFe3: retorno.Objeto.versao == "3.10" ? retorno.Objeto : null,
                        NFe4: retorno.Objeto.versao == "4.00" ? retorno.Objeto : null
                    });
                } else {
                    erros += "<br/>" + file.name + ": " + retorno.Erro;
                }
            });

            uploader.bind('StateChanged', function (up) {
                if (up.state != plupload.STARTED) {
                    ValidarNFes();
                    if (erros != "") {
                        jAlert("Ocorreram as seguintes falhas no envio dos arquivos xml: <br /><br />" + erros + "<br />", "Atenção", function () {
                            uploader.splice(0, uploader.files.length);
                            uploader.destroy();
                            LimparCampos();
                            CarregarCTes();
                        });
                    } else {
                        GerarCTe();
                    }
                }
            });
        }

        function GerarCTe() {
            executarRest("/UsuarioCTe/GerarCTePorListaNFe?callback=?", { DataEmissao: $("#txtDataEmissao").val(), ValorFrete: $("#txtValorFrete").val(), ValorTotalMercadoria: $("#txtValorTotalMercadoria").val(), Observacao: $("#txtObservacao").val(), Documentos: JSON.stringify(documentos) }, function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("CT-e gerado com sucesso!", "Sucesso!");
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }

                uploader.splice(0, uploader.files.length);
                uploader.destroy();

                LimparCampos();
                CarregarCTes();

            });
        }

        function RemoverArquivo(id) {
            uploader.removeFile(uploader.getFile(id));
            $("#" + id).remove();
        }

        function ValidarDados() {
            var valor = Globalize.parseFloat($("#txtValorFrete").val());
            var valido = true;

            if (isNaN(valor) || valor <= 0) {
                CampoComErro("#txtValorFrete");
                valido = false;
            } else {
                CampoSemErro("#txtValorFrete");
            }

            return valido;
        }

        function ValidarNFes() {
            if (documentos != null && documentos.length >= 0) {
                var cpfCnpjTransportador = "";
                for (var i = 0; i < documentos.length; i++) {
                    var nfe = documentos[i].NFe2 != null ? documentos[i].NFe2 : documentos[i].NFe3 != null ? documentos[i].NFe3 : documentos[i].NFe4;

                    if (nfe.NFe.infNFe.transp != null && nfe.NFe.infNFe.transp.transporta != null) {
                        cpfCnpjTransportador = cpfCnpjTransportador == "" ? nfe.NFe.infNFe.transp.transporta.Item : cpfCnpjTransportador;

                        if (cpfCnpjTransportador != nfe.NFe.infNFe.transp.transporta.Item) {
                            erros += "As NF-es são de mais de um transportador.<br/>";
                            return;
                        }
                    } else {
                        erros += "Alguma das NF-es não possui um transportador.<br/>";
                        return;
                    }
                }
            }
        }

        function LimparCampos() {
            $("#txtValorFrete").val("0,00");
            $("#txtValorTotalMercadoria").val("0,00");
            $("#txtObservacao").val("");
            $("#divListaArquivos").html("");
            $("#txtDataEmissao").val(Globalize.format(new Date(), "dd/MM/yyyy HH:mm"));

            InicializarPlUpload();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Geração de CT-e por NF-e
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
                        <div class="field fieldum">
                            <div class="label">
                                <label>
                                    Número:
                                </label>
                            </div>
                            <div class="input">
                                <input type="text" id="txtNumeroFiltro" />
                            </div>
                        </div>
                        <div class="field fieldum">
                            <div class="label">
                                <label>
                                    Série:
                                </label>
                            </div>
                            <div class="input">
                                <input type="text" id="txtSerieFiltro" />
                            </div>
                        </div>
                        <div class="field fieldum">
                            <div class="label">
                                <label>
                                    Data Emissão:
                                </label>
                            </div>
                            <div class="input">
                                <input type="text" id="txtDataEmissaoFiltro" />
                            </div>
                        </div>
                        <div class="field fielddois">
                            <div class="label">
                                <label>
                                    Empresa:
                                </label>
                            </div>
                            <div class="input">
                                <input type="text" id="txtEmpresaFiltro" />
                            </div>
                        </div>
                        <div class="buttons" style="margin-left: 5px; margin-bottom: 15px;">
                            <input type="button" id="btnFiltrar" value="Filtrar" />
                        </div>
                        <div class="table" style="margin-left: 5px;">
                            <div id="tbl_ctes">
                            </div>
                            <div id="tbl_paginacao_ctes" class="pagination">
                            </div>
                        </div>
                        <div class="buttons" style="margin-left: 5px; margin-bottom: 15px;">
                            <input type="button" id="btnBuscarCTes" value="Atualizar Lista" />
                        </div>
                        <div class="fields">
                            <div class="fieldzao">
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Valor do Frete*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtValorFrete" value="0,00" class="maskedInput" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Valor Total da Mercadoria*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtValorTotalMercadoria" value="0,00" class="maskedInput" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Data de Emissão*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtDataEmissao" value="0,00" class="maskedInput" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldseis">
                                    <div class="label">
                                        <label>
                                            Observação:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <textarea id="txtObservacao" rows="3" cols="10" style="width: 99.5%"></textarea>
                                    </div>
                                </div>
                            </div>
                            <div id="divArquivosSelecionados" class="fieldzao">
                                <div id="divListaArquivos" style="margin-top: 10px; margin-left: 10px;">
                                </div>
                                <div class="buttons" style="margin-left: 5px;">
                                    <input type="button" id="btnSelecionarArquivos" value="Selecionar NF-e" />
                                    <input type="button" id="btnGerarCTe" value="Gerar" />
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
