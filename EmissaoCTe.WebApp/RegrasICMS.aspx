<%@ Page Title="Cadastro de Regras de ICMS" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RegrasICMS.aspx.cs" Inherits="EmissaoCTe.WebApp.RegrasICMS" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker","~/bundle/styles/plupload") %>
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
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/priceformat") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaDeRegraICMS("default-search", "default-search", RetornoConsultaRegraICMS, "", true, false);

            CarregarConsultadeClientes("btnBuscarRemetente", "btnBuscarRemetente", RetornoConsultaCliente, true, false);
            CarregarConsultadeClientes("btnBuscarDestinatario", "btnBuscarDestinatario", RetornoConsultaCliente, true, false);
            CarregarConsultadeClientes("btnBuscarTomador", "btnBuscarTomador", RetornoConsultaCliente, true, false);

            CarregarConsultaDeAtividades("btnBuscarAtividadeRemetente", "btnBuscarAtividadeRemetente", RetornoConsultaAtividade, true, false);
            CarregarConsultaDeAtividades("btnBuscarAtividadeDestinatario", "btnBuscarAtividadeDestinatario", RetornoConsultaAtividade, true, false);
            CarregarConsultaDeAtividades("btnBuscarAtividadeTomador", "btnBuscarAtividadeTomador", RetornoConsultaAtividade, true, false);

            CarregarConsultaDeAliquotasDeICMS("btnBuscarAliquota", "btnBuscarAliquota", "A", RetornoConsultaAliquota, true, false);
            CarregarConsultaDeCFOPs("btnBuscarCFOP", "btnBuscarCFOP", "1", RetornoConsultaCFOP, true, false);

            RemoveConsulta(".text-consulta", function ($this) {
                $this.val('');
                $this.data('codigo', 0);
            });

            $("#txtPercentualReducaoBC").priceFormat();
            $("#txtAliquotaSimples").priceFormat();

            $("#selCST").change(function () {
                CSTModificado();
            });

            $("button[data-tagon]").click(function () {
                var $this = $(this);
                InserirTag($this.data('tagon').replace('#', ''), $this.data('tagval'));
            });

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            $("#btnAnexo").click(function () {
                EnviarAnexo();
            });

            $("#btnDownloadAnexo").click(function () {
                executarDownload("/RegraICMS/DownloadArquivo", { Codigo: IdRegraEmEdicao }, null, null, null);
            });


            CarregaEstados();
            LimparCampos();
        });
        var IdRegraEmEdicao = 0;

        function EnviarAnexo() {
            if (IdRegraEmEdicao == 0)
                return Salvar(EnviarAnexo);

            Upload = AbrirUploadPadrao({
                url: "/RegraICMS/InserirArquivo?Codigo=" + IdRegraEmEdicao,
                multiple: false,
                max_file_size: '10000kb',
                onFinish: function (arquivos, erros) {
                    if (erros.legth > 0)
                        ExibirMensagemErro(erros[0], "Anexo");
                    else {
                        $("#btnDownloadAnexo").show();
                        $("#btnAnexo").text("Substituir Anexo");
                    }
                }
            });
        }

        function RetornoConsultaCliente(cliente, evt) {
            var container = $(evt.currentTarget).parent().siblings('input');
            container.val(cliente.CPFCNPJ + " " + cliente.Nome).data('codigo', cliente.CPFCNPJ.replace(/[^0-9]/g, ''));
        }

        function RetornoConsultaAtividade(atividade, evt) {
            var container = $(evt.currentTarget).parent().siblings('input');
            container.val(atividade.Descricao).data('codigo', atividade.Codigo);
        }

        function RetornoConsultaAliquota(aliquota, evt) {
            var container = $(evt.currentTarget).parent().siblings('input');
            container.val(aliquota.Aliquota).data('codigo', aliquota.Aliquota);
        }

        function RetornoConsultaCFOP(cfop, evt) {
            var container = $(evt.currentTarget).parent().siblings('input');
            container.val(cfop.CFOP).data('codigo', cfop.Codigo);
        }

        function ValidarDados() {
            var valido = true;

            return valido;
        }

        function CSTModificado() {
            var cst = $("#selCST").val();

            if (cst == "20" || cst == "60" || cst == "90" || cst == "91") {
                $("#txtPercentualReducaoBC").prop('disabled', false);
            } else {
                $("#txtPercentualReducaoBC").val('0,00').prop('disabled', true);
            }

            if (cst == "40" || cst == "41" || cst == "51" || cst == "") {
                $("#txtAliquota, #btnBuscarAliquota").val('').data('codigo', '').prop('disabled', true);
            } else {
                $("#txtAliquota, #btnBuscarAliquota").prop('disabled', false);
            }
        }

        function InserirTag(id, text) {
            if (id != null && id.trim() != "") {
                var txtarea = document.getElementById(id);
                var scrollPos = txtarea.scrollTop;
                var strPos = 0;
                var br = ((txtarea.selectionStart || txtarea.selectionStart == '0') ? "ff" : (document.selection ? "ie" : false));
                if (br == "ie") {
                    txtarea.focus();
                    var range = document.selection.createRange();
                    range.moveStart('character', -txtarea.value.length);
                    strPos = range.text.length;
                } else if (br == "ff") {
                    strPos = txtarea.selectionStart;
                }
                var front = (txtarea.value).substring(0, strPos);
                var back = (txtarea.value).substring(strPos, txtarea.value.length);
                txtarea.value = front + text + back;
                strPos = strPos + text.length;
                if (br == "ie") {
                    txtarea.focus();
                    var range = document.selection.createRange();
                    range.moveStart('character', -txtarea.value.length);
                    range.moveStart('character', strPos);
                    range.moveEnd('character', 0);
                    range.select();
                } else if (br == "ff") {
                    txtarea.selectionStart = strPos;
                    txtarea.selectionEnd = strPos;
                    txtarea.focus();
                }
                txtarea.scrollTop = scrollPos;
            }
        }

        function Salvar(cbsucesso) {
            if (ValidarDados()) {
                var regra = {
                    Codigo: IdRegraEmEdicao,
                    Status: $("#selStatus").val() == 1,

                    EstadoEmitente: $("#selEstadoEmitente").val(),
                    EstadoOrigem: $("#selEstadoOrigem").val(),
                    UFOrigemDiferente: $("#chkUFOrigemDiferente").prop('checked'),
                    EstadoDestino: $("#selEstadoDestino").val(),
                    UFDestinoDiferente: $("#chkUFDestinoDiferente").prop('checked'),
                    EstadoTomador: $("#selEstadoTomador").val(),
                    EstadoEmitenteDiferente: $("#selEstadoEmitenteDiferente").val(),
                    Remetente: $("#txtRemetente").data('codigo'),
                    AtividadeRemetente: $("#txtAtividadeRemetente").data('codigo'),
                    Destinatario: $("#txtDestinatario").data('codigo'),
                    AtividadeDestinatario: $("#txtAtividadeDestinatario").data('codigo'),
                    Tomador: $("#txtTomador").data('codigo'),
                    AtividadeTomador: $("#txtAtividadeTomador").data('codigo'),

                    CST: $("#selCST").val(),
                    CFOP: $("#txtCFOP").data('codigo'),
                    Aliquota: $("#txtAliquota").data('codigo'),
                    PercentualReducaoBC: $("#txtPercentualReducaoBC").val(),
                    AliquotaSimples: $("#txtAliquotaSimples").val(),
                    DescricaoRegra: $("#txtDescricaoRegra").val(),
                    ImprimeLeiNoCTe: $("#chkImprimeLeiNoCTe").prop('checked'),
                    ZerarValorICMS: $("#chkZerarValorICMS").prop('checked'),
                };

                executarRest("/RegraICMS/Salvar?callback=?", regra, function (r) {
                    if (r.Sucesso) {
                        if (cbsucesso != null) {
                            BuscarPorCodigo(r.Objeto.Codigo);
                            IdRegraEmEdicao = r.Objeto.Codigo;
                            cbsucesso();
                        } else {
                            ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                            LimparCampos();
                        }
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });

            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!");
            }
        }

        function LimparCampos() {
            $(".preenche-estados").val('');
            $(".text-consulta").val('');
            $(".text-consulta").data('codigo', 0);

            $("#selStatus").val('1');
            $("#selCST").val('').trigger('change');
            $("#txtPercentualReducaoBC").val('0,00');
            $("#txtAliquotaSimples").val('0,00');
            $("#txtDescricaoRegra").val("");
            $("#chkImprimeLeiNoCTe").prop('checked', false);
            $("#chkZerarValorICMS").prop('checked', false);

            $("#chkUFOrigemDiferente").prop('checked', false);
            $("#chkUFDestinoDiferente").prop('checked', false);

            $("#btnAnexo").text("Inserir Anexo");
            $("#btnDownloadAnexo").hide();

            $("#txtLog").val("");
            IdRegraEmEdicao = 0;
        }

        function RetornoConsultaRegraICMS(regra) {
            BuscarPorCodigo(regra.Codigo);
        }

        function BuscarPorCodigo(codigo) {
            executarRest("/RegraICMS/ObterDetalhes?callback=?", { Codigo: codigo }, function (r) {
                if (r.Sucesso) {
                    IdRegraEmEdicao = r.Objeto.Codigo;
                    $("#selStatus").val(r.Objeto.Status ? "1" : "0");
                    $("#selEstadoEmitente").val(r.Objeto.EstadoEmitente);
                    $("#chkUFOrigemDiferente").prop('checked', r.Objeto.UFOrigemDiferente);
                    $("#chkUFDestinoDiferente").prop('checked', r.Objeto.UFDestinoDiferente);
                    $("#selEstadoOrigem").val(r.Objeto.EstadoOrigem);
                    $("#selEstadoDestino").val(r.Objeto.EstadoDestino);
                    $("#selEstadoTomador").val(r.Objeto.EstadoTomador);
                    $("#selEstadoEmitenteDiferente").val(r.Objeto.EstadoEmitenteDiferente);
                    if (r.Objeto.Remetente != null) {
                        $("#txtRemetente").val(r.Objeto.Remetente.Codigo + " " + r.Objeto.Remetente.Descricao);
                        $("#txtRemetente").data('codigo', r.Objeto.Remetente.Codigo);
                    }
                    if (r.Objeto.AtividadeRemetente != null) {
                        $("#txtAtividadeRemetente").val(r.Objeto.AtividadeRemetente.Descricao);
                        $("#txtAtividadeRemetente").data('codigo', r.Objeto.AtividadeRemetente.Codigo);
                    }
                    if (r.Objeto.Destinatario != null) {
                        $("#txtDestinatario").val(r.Objeto.Destinatario.Codigo + " " + r.Objeto.Destinatario.Descricao);
                        $("#txtDestinatario").data('codigo', r.Objeto.Destinatario.Codigo);
                    }
                    if (r.Objeto.AtividadeDestinatario != null) {
                        $("#txtAtividadeDestinatario").val(r.Objeto.AtividadeDestinatario.Descricao);
                        $("#txtAtividadeDestinatario").data('codigo', r.Objeto.AtividadeDestinatario.Codigo);
                    }
                    if (r.Objeto.Tomador != null) {
                        $("#txtTomador").val(r.Objeto.Tomador.Codigo + " " + r.Objeto.Tomador.Descricao);
                        $("#txtTomador").data('codigo', r.Objeto.Tomador.Codigo);
                    }
                    if (r.Objeto.AtividadeTomador != null) {
                        $("#txtAtividadeTomador").val(r.Objeto.AtividadeTomador.Descricao);
                        $("#txtAtividadeTomador").data('codigo', r.Objeto.AtividadeTomador.Codigo);
                    }
                    $("#selCST").val(r.Objeto.CST).trigger('change');
                    if (r.Objeto.CFOP != null) {
                        $("#txtCFOP").val(r.Objeto.CFOP.Descricao);
                        $("#txtCFOP").data('codigo', r.Objeto.CFOP.Codigo);
                    }
                    if (r.Objeto.Aliquota != null) {
                        $("#txtAliquota").val(r.Objeto.Aliquota.Descricao);
                        $("#txtAliquota").data('codigo', r.Objeto.Aliquota.Codigo);
                    }
                    $("#txtPercentualReducaoBC").val(r.Objeto.PercentualReducaoBC);
                    $("#txtAliquotaSimples").val(r.Objeto.AliquotaSimples);
                    $("#txtDescricaoRegra").val(r.Objeto.DescricaoRegra);
                    $("#chkImprimeLeiNoCTe").prop('checked', r.Objeto.ImprimeLeiNoCTe);
                    $("#chkZerarValorICMS").prop('checked', r.Objeto.ZerarValorICMS);
                    $("#txtLog").val(r.Objeto.Log);

                    if (r.Objeto.PossuiArquivo) {
                        $("#btnDownloadAnexo").show();
                        $("#btnAnexo").text("Substituir Anexo");
                    } else {
                        $("#btnDownloadAnexo").hide();
                        $("#btnAnexo").text("Inserir Anexo");
                    }
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }

        function CarregaEstados() {
            var estados = [
                { value: "", text: "Não Selecionado" },
                { value: "AC", text: "Acre" },
                { value: "AL", text: "Alagoas" },
                { value: "AP", text: "Amapá" },
                { value: "AM", text: "Amazonas" },
                { value: "BA", text: "Bahia" },
                { value: "CE", text: "Ceará" },
                { value: "DF", text: "Distrito Federal" },
                { value: "ES", text: "Espírito Santo" },
                { value: "GO", text: "Goiás" },
                { value: "MA", text: "Maranhão" },
                { value: "MT", text: "Mato Grosso" },
                { value: "MS", text: "Mato Grosso do Sul" },
                { value: "MG", text: "Minas Gerais" },
                { value: "PA", text: "Pará" },
                { value: "PB", text: "Paraíba" },
                { value: "PR", text: "Paraná" },
                { value: "PE", text: "Pernambuco" },
                { value: "PI", text: "Piauí" },
                { value: "RJ", text: "Rio de Janeiro" },
                { value: "RN", text: "Rio Grande do Norte" },
                { value: "RS", text: "Rio Grande do Sul" },
                { value: "RO", text: "Rondônia" },
                { value: "RR", text: "Roraima" },
                { value: "SC", text: "Santa Catarina" },
                { value: "SP", text: "São Paulo" },
                { value: "SE", text: "Sergipe" },
                { value: "TO", text: "Tocantins" },
                { value: "EX", text: "Exterior" }
            ];

            var _htmlOptions = estados.map(function (est) { return '<option value="' + est.value + '">' + est.text + '</option>'; })

            $(".preenche-estados").html(_htmlOptions.join(''));
        }
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro de Regras de ICMS</h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-12">
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-5">
                    <div class="input-group">
                        <span class="input-group-addon">Status:</span>
                        <select id="selStatus" class="form-control">
                            <option value="1">Ativo</option>
                            <option value="0">Inativo</option>
                        </select>
                    </div>
                </div>
            </div>
        </div>

        <!--<div class="col-xs-12 col-sm-12 col-md-4 hide">
            <div class="input-group">
                <span class="input-group-addon">Estado Emitente:</span>
                <select id="selEstadoEmitente" class="form-control preenche-estados"></select>
            </div>
        </div>-->

        <div class="col-xs-12 col-sm-12 col-md-5">
            <div class="input-group">
                <span class="input-group-addon">Estado Origem:</span>
                <select id="selEstadoOrigem" class="form-control preenche-estados"></select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-7">
            <div class="checkbox" style="margin-bottom: 11px; margin-top: 8px;">
                <label>
                    <input type="checkbox" id="chkUFOrigemDiferente">
                    Estado Origem diferente do selecionado.
                </label>
            </div>
        </div>

        <div class="col-xs-12 col-sm-12 col-md-5">
            <div class="input-group">
                <span class="input-group-addon">Estado Destino:</span>
                <select id="selEstadoDestino" class="form-control preenche-estados"></select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-7">
            <div class="checkbox" style="margin-bottom: 11px; margin-top: 8px;">
                <label>
                    <input type="checkbox" id="chkUFDestinoDiferente">
                    Estado Destino diferente do selecionado.
                </label>
            </div>
        </div>

        <div class="col-xs-12 col-sm-12 col-md-12">
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-5">
                    <div class="input-group">
                        <span class="input-group-addon">Estado Tomador:</span>
                        <select id="selEstadoTomador" class="form-control preenche-estados"></select>
                    </div>
                </div>
            </div>
        </div>

        <!-- <div class="col-xs-12 col-sm-12 col-md-6 hide">
            <div class="input-group">
                <span class="input-group-addon">Estado Emitente Diferente de:</span>
                <select id="selEstadoEmitenteDiferente" class="form-control preenche-estados"></select>
            </div>
        </div> -->

        <div class="col-xs-12 col-sm-12 col-md-7">
            <div class="input-group">
                <span class="input-group-addon">Remetente:</span>
                <input type="text" id="txtRemetente" class="text-consulta form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarRemetente" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>

        <div class="col-xs-12 col-sm-12 col-md-5">
            <div class="input-group">
                <span class="input-group-addon">Atividade Remetente:</span>
                <input type="text" id="txtAtividadeRemetente" class="text-consulta form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarAtividadeRemetente" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>

        <div class="col-xs-12 col-sm-12 col-md-7">
            <div class="input-group">
                <span class="input-group-addon">Destinatário:</span>
                <input type="text" id="txtDestinatario" class="text-consulta form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarDestinatario" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>

        <div class="col-xs-12 col-sm-12 col-md-5">
            <div class="input-group">
                <span class="input-group-addon">Atividade Destinatário:</span>
                <input type="text" id="txtAtividadeDestinatario" class="text-consulta form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarAtividadeDestinatario" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>

        <div class="col-xs-12 col-sm-12 col-md-7">
            <div class="input-group">
                <span class="input-group-addon">Tomador:</span>
                <input type="text" id="txtTomador" class="text-consulta form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTomador" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>

        <div class="col-xs-12 col-sm-12 col-md-5">
            <div class="input-group">
                <span class="input-group-addon">Atividade Tomador:</span>
                <input type="text" id="txtAtividadeTomador" class="text-consulta form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarAtividadeTomador" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>

        <div class="col-sm-12">
            <br />
            <br />
        </div>

        <div class="col-xs-12 col-sm-12 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">CFOP:</span>
                <input type="text" id="txtCFOP" class="text-consulta form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarCFOP" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">CST:</span>
                <select id="selCST" class="form-control">
                    <option value="">Não Informado</option>
                    <option value="00">00 - tributação normal ICMS</option>
                    <option value="20">20 - tributação com BC reduzida do ICMS</option>
                    <option value="40">40 - ICMS isenção</option>
                    <option value="41">41 - ICMS não tributada</option>
                    <option value="51">51 - ICMS diferido</option>
                    <option value="60">60 - ICMS cobrado anteriormente por substituição tributária</option>
                    <option value="91">90 - ICMS outros</option>
                    <option value="90">90 - ICMS devido à UF de origem da prestação, quando diferente da UF do emitente</option>
                    <option value="SN">Simples Nacional</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Aliquota:</span>
                <input type="text" id="txtAliquota" class="text-consulta form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarAliquota" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Percentual Redução BC:</span>
                <input type="text" id="txtPercentualReducaoBC" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">Aliquota Simples</span>
                <input type="text" id="txtAliquotaSimples" class="form-control" />
            </div>
        </div>
        <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
            <button type="button" id="btnAnexo" class="btn btn-primary">Anexo</button>
            <button type="button" id="btnDownloadAnexo" class="btn btn-default">Download</button>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12">
            <div class="input-group">
                <span class="input-group-addon">Descrição da Regra (lei):</span>
                <textarea rows="3" id="txtDescricaoRegra" class="form-control"></textarea>
            </div>
            <span>Tags: </span>
            <button type="button" class="btn btn-default btn-xs" data-tagon="#txtDescricaoRegra" data-tagval="#Aliquota#">Aliquota</button>
            <button type="button" class="btn btn-default btn-xs" data-tagon="#txtDescricaoRegra" data-tagval="#ValorICMS#">Valor ICMS</button>
            <button type="button" class="btn btn-default btn-xs" data-tagon="#txtDescricaoRegra" data-tagval="#ValorFrete#">Valor Frete</button>
            <button type="button" class="btn btn-default btn-xs" data-tagon="#txtDescricaoRegra" data-tagval="#BaseCalculo#">Base de Cálculo</button>
            <button type="button" class="btn btn-default btn-xs" data-tagon="#txtDescricaoRegra" data-tagval="#Transportadora#">Transportador</button>
            <button type="button" class="btn btn-default btn-xs" data-tagon="#txtDescricaoRegra" data-tagval="#Tomador#">Tomador</button>
            <button type="button" class="btn btn-default btn-xs" data-tagon="#txtDescricaoRegra" data-tagval="#Rementente#">Rementente</button>
            <button type="button" class="btn btn-default btn-xs" data-tagon="#txtDescricaoRegra" data-tagval="#UFOrigem#">UF Origem</button>
            <button type="button" class="btn btn-default btn-xs" data-tagon="#txtDescricaoRegra" data-tagval="#UFDestino#">UF Destino</button>
            <button type="button" class="btn btn-default btn-xs" data-tagon="#txtDescricaoRegra" data-tagval="#Produto#">Produto Predominante</button>
            <button type="button" class="btn btn-default btn-xs" data-tagon="#txtDescricaoRegra" data-tagval="#PercentualICMSIncluirNoFrete#">% Inclusão ICMS</button>
            <button type="button" class="btn btn-default btn-xs" data-tagon="#txtDescricaoRegra" data-tagval="#AliquotaSimples#">Aliquota Simples</button>
            <button type="button" class="btn btn-default btn-xs" data-tagon="#txtDescricaoRegra" data-tagval="#ValorFreter*AliquotaSimples#">Frete x Aliquota Simples</button>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12">
            <div class="checkbox" style="margin-bottom: 5px; margin-top: 0;">
                <label>
                    <input type="checkbox" id="chkImprimeLeiNoCTe">
                    Imprimir lei (Descrição da Regra) no CT-e?
                </label>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12">
            <div class="checkbox" style="margin-bottom: 5px; margin-top: 0;">
                <label>
                    <input type="checkbox" id="chkZerarValorICMS">
                    Zerar base de calculo do ICMS?
                </label>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Log das Alterações">Log</abbr>:
                </span>
                <textarea id="txtLog" class="form-control taggedInput" rows="5" readonly=""></textarea>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
