<%@ Page Title="AtendimentoEmissao" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AtendimentoEmissao.aspx.cs" Inherits="EmissaoCTe.WebAdmin.AtendimentoEmissao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .pneu, .pneuEixo, .pneuManutencao {
            height: 100px;
            width: 101px;
            float: left;
            border: none;
            margin-right: 5px;
            text-align: center;
            cursor: move;
            background: transparent url('Images/pneu.png') no-repeat bottom;
            z-index: 999;
        }

            .pneu header, .pneuEixo header, .pneuManutencao header {
                padding: 5px;
                overflow: hidden;
                height: 35px;
                line-height: 1;
            }

        .droppableActive {
            border: 2px dashed #000 !important;
        }

        .droppableHover {
            background-color: rgba(209, 209, 209, 0.50);
        }
    </style>

    <link href="Styles/ui/ui.datepicker.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/plupload/jquery.plupload.queue.min.css" rel="stylesheet" type="text/css" />

    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/ui/ui.datepicker.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.ui.datepicker-pt-BR.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/plupload.full.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/jquery.plupload.queue.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/pt-br.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/validaCampos.min.js" type="text/javascript"></script>
    <script defer="defer" type="text/javascript">
        var IdAtendimentoEmEdicao = 0;
        var PodeEditar = false;
        var Anexos = [];
        var TipoAtendimentoPadrao = null;
        $(document).ready(function () {
            CarregarConsultaDeAtendimentosEmissao("default-search", "default-search", RetornoConsultaAtendimento, true);

            CarregarConsultaDeEmpresas("btnEmpresa", "btnEmpresa", "A", RetornoConsultaTransportador, true);
            CarregarConsultaDeTipoAtendimentoEmissao("btnTipo", "btnTipo", RetornoConsultaTipo, true);
            CarregarConsultaDeUsuariosAdminAtivos("btnUsuarioErro", "btnUsuarioErro", RetornoConsultaUsuarioErro, true);

            RemoveConsulta("#txtEmpresa, #txtTipo", function ($this) {
                $this.val('');
                $this.data('codigo', 0);
            });

            $("#txtData").mask("99/99/9999 99:99");
            SetaDataAtual();

            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
            $("#btnExcluir").click(function () {
                Excluir();
            });

            InicializarPlUpload();
            LimparCampos();
            BuscarTipoAtendimentoPadrao();
            HeaderAuditoria("Atendimento");
        });

        function SetaDataAtual() {
            var date = new Date();
            $("#txtData").val(Globalize.format(date, "dd/MM/yyyy HH:mm"));
        }

        function RetornoConsultaTransportador(transportador) {
            $("#txtEmpresa").data('codigo', transportador.Codigo).val(transportador.RazaoSocial);
        }

        function RetornoConsultaUsuarioErro(usuario) {
            $("#txtUsuarioErro").data('codigo', usuario.Codigo).val(usuario.Nome);
        }

        function RetornoConsultaTipo(tipo) {
            $("#txtTipo").data('codigo', tipo.Codigo).val(tipo.Descricao);
        }

        function BuscarTipoAtendimentoPadrao() {
            executarRest("/AtendimentoEmissao/TipoAtendimentoPadraoEmissao?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    TipoAtendimentoPadrao = r.Objeto;

                    if (TipoAtendimentoPadrao != null)
                        $("#txtTipo").data('codigo', TipoAtendimentoPadrao.Codigo).val(TipoAtendimentoPadrao.Descricao);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function RetornoConsultaAtendimento(atendimento) {
            LimparCampos();
            executarRest("/AtendimentoEmissao/ObterDetalhes?callback=?", atendimento, function (r) {
                if (r.Sucesso) {
                    var dados = r.Objeto;

                    IdAtendimentoEmEdicao = dados.Codigo;
                    HeaderAuditoriaCodigo(dados.Codigo);

                    $("#txtNumero").val(dados.Numero);
                    $("#txtEmpresa").data("codigo", dados.Empresa.Codigo).val(dados.Empresa.Descricao);
                    $("#txtData").val(dados.Data);
                    $("#txtDescricao").val(dados.Descricao);
                    $("#txtObservacao").val(dados.Observacao);
                    $("#spnAutor").text(dados.Autor);

                    if (dados.Tipo != null)
                        $("#txtTipo").data('codigo', dados.Tipo.Codigo).val(dados.Tipo.Descricao);

                    if (dados.UsuarioErro != null)
                        $("#txtUsuarioErro").data('codigo', dados.UsuarioErro.Codigo).val(dados.UsuarioErro.Nome);

                    $("#selTipoContato").val(dados.TipoContato);
                    $("#txtContato").val(dados.Contato);

                    $("#filecontainer, #autor").show();
                    Anexos = dados.Anexos;
                    RenderizarArquivosAnexados();
                    PodeEditar = dados.PodeEditar;

                    if (!PodeEditar) {
                        $("#btnSelecionarArquivos").hide();
                        $("#btnSalvar").hide();
                    }
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function LimparCampos() {
            IdAtendimentoEmEdicao = 0;
            Anexos = [];

            SetaDataAtual();
            $("#txtNumero").val("Automático");
            $("#txtEmpresa").data("codigo", 0).val("");
            $("#txtUsuarioErro").data("codigo", 0).val("");
            $("#txtDescricao").val("");
            $("#txtObservacao").val("");

            if (TipoAtendimentoPadrao != null)
                $("#txtTipo").data('codigo', TipoAtendimentoPadrao.Codigo).val(TipoAtendimentoPadrao.Descricao);
            else
                $("#txtTipo").data("codigo", 0).val("");

            $("#selTipoContato").val("3");
            $("#txtContato").val("");

            $("#btnExcluir").hide();
            $("#btnSalvar").show();
            $("#btnSelecionarArquivos").show();
            $("#autor").hide("");
            $.each(uploader.files, function (i, file) {
                uploader.removeFile(file);
            });
            RenderizarArquivosAnexados();
        }
        function ValidarCampos() {
            var valido = true;

            if ($("#txtEmpresa").data('codigo') > 0) {
                CampoSemErro("#txtEmpresa");
            } else {
                CampoComErro("#txtEmpresa");
                valido = false;
            }

            if ($("#txtData").val() != "") {
                CampoSemErro("#txtData");
            } else {
                CampoComErro("#txtData");
                valido = false;
            }

            if ($("#txtDescricao").val() != "") {
                CampoSemErro("#txtDescricao");
            } else {
                CampoComErro("#txtDescricao");
                valido = false;
            }

            return valido;
        }
        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: IdAtendimentoEmEdicao,
                    Empresa: $("#txtEmpresa").data('codigo'),
                    Tipo: $("#txtTipo").data('codigo'),
                    Sistema: "1",
                    Data: $("#txtData").val(),
                    Situacao: "4",
                    Satisfacao: "4",
                    TipoContato: $("#selTipoContato").val(),
                    Contato: $("#txtContato").val(),
                    Descricao: $("#txtDescricao").val(),
                    Observacao: $("#txtObservacao").val(),
                    UsuarioErro: $("#txtUsuarioErro").data('codigo'),
                };

                executarRest("/AtendimentoEmissao/SalvarEmissao?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        if (uploader.files.length > 0) {
                            uploader.settings.url = 'AtendimentoEmissao/Anexos?callback=?&Codigo=' + r.Objeto.Codigo;
                            $('#divListaArquivos a').remove();
                            uploader.start();
                        } else {
                            SalvoComSucesso();
                        }
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção");
            }
        }
        function Excluir() {
            jConfirm("Tem certeza que deseja excluir", "Confirmar exclusão", function (r) {
                if (r) {
                    executarRest("/AtendimentoEmissao/Excluir?callback=?", Impressora, function (r) {
                        if (r.Sucesso) {
                            ExibirMensagemSucesso("Excluído com sucesso.", "Sucesso");
                            LimparCampos();
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção");
                        }
                    });
                }
            });
        }

        function SalvoComSucesso() {
            ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso");
            LimparCampos();
        }

        function InicializarPlUpload() {
            documentos = new Array();
            erros = false;
            uploader = new plupload.Uploader({
                runtimes: 'gears,html5,flash,silverlight,browserplus',
                browse_button: 'btnSelecionarArquivos',
                container: 'divArquivosSelecionados',
                max_file_size: '10000kb',
                multi_selection: true,
                flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
                silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
                filters: [{ title: "Arquivos", extensions: "*" }],
            });

            uploader.init();

            uploader.bind('FilesAdded', function (up) {
                up.refresh();
                RenderizarArquivosAnexados();
            });

            uploader.bind('UploadProgress', function (up, file) {
                $('#' + file.id + " b").html("  (" + file.percent + "%)");
            });

            uploader.bind('Error', function (up, err) {
                $('#' + err.file.id + " b").html("<div>Erro: " + err.code + " - " + err.message + (err.file ? ", Arquivo: " + err.file.name : "") + "</div>");

                up.refresh();
                erros = true;
            });

            uploader.bind('FileUploaded', function (up, file, response) {
                $('#' + file.id + " b").html("(100%)");
            });

            uploader.bind("UploadComplete", function () {
                if (!erros) {
                    jAlert("Dados salvos com sucesso!", "Sucesso!");
                }
            });

            uploader.bind('StateChanged', function (up) {
                if (up.state != plupload.STARTED) {
                    if (erros) {
                        jAlert("Ocorreram algumas falhas no retorno dos arquivos", "Atenção");
                    } else {
                        SalvoComSucesso();
                    }
                }
            });
        }

        function DownloadArquivo(id) {
            $("#ifrDownload").attr("src", "AtendimentoEmissao/DownloadAnexo?Codigo=" + id);
        }

        function RenderizarArquivosAnexados() {
            $('#divListaArquivos').html("");

            var rows = [];

            for (var i in uploader.files) {
                var file = uploader.files[i];
                rows.push('<div id="' + file.id + '">Arquivo selecionado: ' + file.name + ' - <b></b><a href="javascript:void(0);" onclick="RemoverArquivo(\'' + file.id + '\');">Remover</a></div>');
            }

            for (var i in Anexos) {
                var file = Anexos[i];
                rows.push('<div id="' + file.Codigo + '"><a href="javascript:void(0);" onclick="DownloadArquivo(\'' + file.Codigo + '\');">' + file.NomeArquivo + '</a>' + (PodeEditar ? ' - <a href="javascript:void(0);" onclick="DeletarArquivo(\'' + file.Codigo + '\');">Remover</a>' : '') + '</div>');
            }

            if (rows.length == 0) {
                rows.push('<div style="text-aling: center">Nenhum arquivo selecionado</div>');
            }

            $('#divListaArquivos').append(rows.join(''));
        }

        function DeletarArquivo(id) {
            executarRest("/AtendimentoEmissao/AnexoExcluir?callback=?", { Codigo: id }, function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("Excluído com sucesso.", "Sucesso");
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function RemoverArquivo(id) {
            uploader.removeFile(uploader.getFile(id));
            RenderizarArquivosAnexados();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Atendimento Emissão</h3>
            </div>
            <div class="content-box">
                <div class="form">
                    <div id="default-search" class="default-search">
                        Pesquisar
                    </div>
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
                                        Número:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" disabled id="txtNumero" />
                                </div>
                            </div>
                            <div class="field fieldum" id="autor" style="display: none">
                                <div class="label">
                                    <label>
                                        Autor:
                                    </label>
                                </div>
                                <div class="input">
                                    <span id="spnAutor"></span>
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Data*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtData" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="field fieldnove">
                                    <div class="label">
                                        <label>
                                            Empresa*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtEmpresa" />
                                    </div>
                                </div>
                                <div class="buttons">
                                    <input type="button" id="btnEmpresa" value="Buscar" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="field fieldnove">
                                    <div class="label">
                                        <label>
                                            Usuário do Erro*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtUsuarioErro" />
                                    </div>
                                </div>
                                <div class="buttons">
                                    <input type="button" id="btnUsuarioErro" value="Buscar" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="field fieldnove">
                                    <div class="label">
                                        <label>
                                            Tipo*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTipo" />
                                    </div>
                                </div>
                                <div class="buttons">
                                    <input type="button" id="btnTipo" value="Buscar" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Tipo do Contato*:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selTipoContato">
                                        <option value="3">E-mail</option>
                                        <option value="2">Telefone</option>
                                        <option value="1">Skype</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Contato*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtContato" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldseis">
                                <div class="label">
                                    <label>
                                        Descrição*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDescricao" />
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
                                    <textarea rows="6" id="txtObservacao" cols="" style="width: 99.5%"></textarea>
                                </div>
                            </div>
                        </div>

                        <div class="fieldzao" id="filecontainer" style="margin: 25px 0 0 4px;">
                            <div class="field fielddoze">
                                <input type="button" id="btnSelecionarArquivos" value="Selecionar Arquivos" />
                                <div id="lblArquivos" class="label">
                                    <label>
                                        <b>Anexos:</b>
                                    </label>
                                </div>
                            </div>
                            <div id="divArquivosSelecionados">
                                <div id="divListaArquivos" class="fieldzao" style="overflow-y: scroll; max-height: 100px;">
                                </div>
                            </div>
                        </div>

                        <div class="buttons" style="margin-left: 5px;">
                            <input type="button" id="btnSalvar" value="Salvar" />
                            <input type="button" id="btnCancelar" value="Cancelar" />
                            <input type="button" id="btnExcluir" value="Excluir" style="display: none;" />
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
