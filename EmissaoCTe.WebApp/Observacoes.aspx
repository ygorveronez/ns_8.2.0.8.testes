<%@ Page Title="Cadastro de Observações CTe" Language="C#" MasterPageFile="Site.Master"
    AutoEventWireup="true" CodeBehind="Observacoes.aspx.cs" Inherits="EmissaoCTe.WebApp.Observacoes" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/priceformat") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        var idTextArea;
        $(document).ready(function () {
            CarregarConsultaDeObservacoes("default-search", "default-search", Editar, true, false);
            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
            $("#btnExcluir").click(function () {
                Excluir();
            });
            $("#selOperacao").change(function () {
                ControlarCamposOperacao();
            });

            ObterEstados();
            $("#btnExcluir").hide();
            $("#txtLog").prop("readonly", "true");

            //$(".taggedInput").focus(function () {
            //    idTextArea = this.id;
            //});
            idTextArea = "txtObservacao";
            $("#lnkSerieCTe").click(function () {
                InserirTag(idTextArea, "#SerieCTe#");
            });
            $("#lnkCPFCNPJRemetente").click(function () {
                InserirTag(idTextArea, "#CPFCNPJRemetente#");
            });
            $("#lnkNomeRemetente").click(function () {
                InserirTag(idTextArea, "#NomeRemetente#");
            });
            $("#lnkFantasiaRemetente").click(function () {
                InserirTag(idTextArea, "#FantasiaRemetente#");
            });
            $("#lnkCidadeRemetente").click(function () {
                InserirTag(idTextArea, "#CidadeRemetente#");
            });
            $("#lnkEstadoRemetente").click(function () {
                InserirTag(idTextArea, "#EstadoRemetente#");
            });
            $("#lnkCPFCNPJDestinatario").click(function () {
                InserirTag(idTextArea, "#CPFCNPJDestinatario#");
            });
            $("#lnkNomeDestinatario").click(function () {
                InserirTag(idTextArea, "#NomeDestinatario#");
            });
            $("#lnkFantasiaDestinatario").click(function () {
                InserirTag(idTextArea, "#FantasiaDestinatario#");
            });
            $("#lnkCidadeDestinatario").click(function () {
                InserirTag(idTextArea, "#CidadeDestinatario#");
            });
            $("#lnkEstadoDestinatario").click(function () {
                InserirTag(idTextArea, "#EstadoDestinatario#");
            });
            $("#lnkCPFCNPJProprietarioVeiculo").click(function () {
                InserirTag(idTextArea, "#CPFCNPJProprietario#");
            });
            $("#lnkNomeProprietarioVeiculo").click(function () {
                InserirTag(idTextArea, "#NomeProprietario#");
            });
            $("#lnkRNTRCProprietario").click(function () {
                InserirTag(idTextArea, "#RNTRCProprietario#");
            });
            $("#lnkPlaca").click(function () {
                InserirTag(idTextArea, "#PlacaVeiculo#");
            });
            $("#lnkRENAVAMVeiculo").click(function () {
                InserirTag(idTextArea, "#RENAVAMVeiculo#");
            });
            $("#lnkUFVeiculo").click(function () {
                InserirTag(idTextArea, "#UFVeiculo#");
            });
            $("#lnkMarcaVeiculo").click(function () {
                InserirTag(idTextArea, "#MarcaVeiculo#");
            });
            $("#lnkPlacasVinculadas").click(function () {
                InserirTag(idTextArea, "#PlacasVinculadas#");
            });
            $("#lnkPlacasRenavamVinculadas").click(function () {
                InserirTag(idTextArea, "#PlacasRenavamVinculadas#");
            });
            $("#lnkNomeMotorista").click(function () {
                InserirTag(idTextArea, "#NomeMotorista#");
            });
            $("#lnkCPFMotorista").click(function () {
                InserirTag(idTextArea, "#CPFMotorista#");
            });
            $("#lnkNumeroNFe").click(function () {
                InserirTag(idTextArea, "#NumeroNFe#");
            });
        });

        function ControlarCamposOperacao() {
            if ($("#selOperacao").val() == "0") {
                $("#selUfInicio").attr("disabled", false);
                $("#selUfFim").attr("disabled", false);
            }
            else {
                $("#selUfInicio").val($("#selUfInicio option:first").val());
                $("#selUfFim").val($("#selUfFim option:first").val());

                $("#selUfInicio").attr("disabled", true);
                $("#selUfFim").attr("disabled", true);
            }
        }

        function ObterEstados() {
            executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
                if (r.Sucesso) {

                    var selUfInicio = document.getElementById("selUfInicio");

                    selUfInicio.options.length = 0;

                    var optnTodos = document.createElement("option");
                    optnTodos.text = "Todos";
                    optnTodos.value = "";

                    selUfInicio.options.add(optnTodos);

                    for (var i = 0; i < r.Objeto.length; i++) {
                        var optn = document.createElement("option");
                        optn.text = r.Objeto[i].Nome;
                        optn.value = r.Objeto[i].Sigla;

                        selUfInicio.options.add(optn);
                    }

                    $("#selUfInicio").val("");


                    var selUfFim = document.getElementById("selUfFim");

                    selUfFim.options.length = 0;

                    var optnTodos = document.createElement("option");
                    optnTodos.text = "Todos";
                    optnTodos.value = "";

                    selUfFim.options.add(optnTodos);

                    for (var i = 0; i < r.Objeto.length; i++) {
                        var optn = document.createElement("option");
                        optn.text = r.Objeto[i].Nome;
                        optn.value = r.Objeto[i].Sigla;

                        selUfFim.options.add(optn);
                    }

                    $("#selUfFim").val("");

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function ValidarCampos() {
            var obs = $("#txtObservacao").val();
            var valido = true;
            if (obs != "") {
                var tamanhoValido = true;
                switch ($("#selTipo").val()) {
                    case '0':
                        if (obs.length > 160) {
                            tamanhoValido = false;
                            ExibirMensagemAlerta("A observação do contribuinte deve ser menor ou igual a 160 caracteres.", "Atenção");
                        }
                        break;
                    case '1':
                        if (obs.length > 60) {
                            tamanhoValido = false;
                            ExibirMensagemAlerta("A observação do fisco deve ser menor ou igual a 60 caracteres.", "Atenção");
                        }
                        break;
                    case '2':
                        if (obs.length > 2000) {
                            tamanhoValido = false;
                            ExibirMensagemAlerta("A observação geral deve ser menor ou igual a 2000 caracteres.", "Atenção");
                        }
                        break;
                }
                if (tamanhoValido) {
                    CampoSemErro("#txtObservacao");
                } else {
                    CampoComErro("#txtObservacao");
                    valido = false;
                }
            } else {
                CampoComErro("#txtObservacao");
                valido = false;
            }
            return valido;
        }

        function LimparCampos() {
            $("#txtObservacao").val("");
            $("#hddCodigo").val("0");
            $("#selTipo").val($("#selTipo option:first").val());
            $("#selStatus").val($("#selStatus option:first").val());
            $("#selUfInicio").val($("#selUfInicio option:first").val());
            $("#selUfFim").val($("#selUfFim option:first").val());
            $("#selCSTCTe").val($("#selCSTCTe option:first").val());
            $("#txtLog").val("");
            $("#selTipoCTe").val($("#selTipoCTe option:first").val());
            $("#selOperacao").val($("#selOperacao option:first").val());
            $("#chkAutomatica").prop('checked', false);
            $("#btnExcluir").hide();
        }
        automa
        function Salvar() {
            if (ValidarCampos()) {
                executarRest("/Observacao/Salvar?callback=?", {
                    Codigo: $("#hddCodigo").val(),
                    Descricao: $("#txtObservacao").val(),
                    Tipo: $("#selTipo").val(),
                    UfInicio: $("#selUfInicio").val(),
                    UfFim: $("#selUfFim").val(),
                    CST: $("#selCSTCTe").val(),
                    Status: $("#selStatus").val(),
                    Operacao: $("#selOperacao").val(),
                    TipoCTe: $("#selTipoCTe").val(),
                    Automatica: $("#chkAutomatica")[0].checked,
                }, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });
            }
        }

        function Excluir() {
            jConfirm("Deseja realmente excluir esta observação?", "Atenção", function (r) {
                if (r) {
                    executarRest("/Observacao/Excluir?callback=?", { Codigo: $("#hddCodigo").val() }, function (r) {
                        if (r.Sucesso) {
                            ExibirMensagemSucesso("Exclusão realizada com sucesso.", "Sucesso!");
                            LimparCampos();
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção!");
                        }
                    });
                }
            });
        }

        function Editar(observacao) {
            $("#selTipo").val(observacao.Tipo);
            $("#selStatus").val(observacao.Status);
            $("#txtObservacao").val(observacao.Descricao);
            $("#hddCodigo").val(observacao.Codigo);
            $("#selUfInicio").val(observacao.UF);
            $("#selUfFim").val(observacao.UFFim);
            $("#selCSTCTe").val(observacao.CST);
            $("#selOperacao").val(observacao.Operacao);
            $("#txtLog").val(observacao.Log);
            $("#chkAutomatica").prop('checked', observacao.Automatica);
            $("#selTipoCTe").val(observacao.TipoCTe);
            //$("#btnExcluir").show();
        }

        function InserirTag(id, text) {
            if (!$("#chkAutomatica")[0].checked)
                ExibirMensagemAlerta("Tags disponíveis apenas para Observação Automática.", "Atenção!");

            if (id != null && id.trim() != "" && $("#chkAutomatica")[0].checked) {
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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Observações CTe
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Tipo Obs.*:
                </span>
                <select id="selTipo" class="form-control">
                    <option value="2">Geral</option>
                    <option value="0">Contribuinte</option>
                    <option value="1">Fisco</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="A">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-6 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <div class="checkbox">
                    <label>
                        <input type="checkbox" id="chkAutomatica" />
                        Observação Automática
                    </label>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Operação
                </span>
                <select id="selOperacao" class="form-control">
                    <option value="0">Todos</option>
                    <option value="1">Intraestadual</option>
                    <option value="2">Interestadual</option>
                    <option value="3">Estado Destino Igual Transportador</option>
                    <option value="4">Estado Destino Diferente Transportador</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Tipo CT-e
                </span>
                <select id="selTipoCTe" class="form-control">
                    <option value="0">Normal</option>
                    <option value="1">Complemento</option>
                    <option value="2">Anulacao</option>
                    <option value="3">Substituto</option>
                </select>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Estado Origem:
                </span>
                <select id="selUfInicio" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Estado Destino:
                </span>
                <select id="selUfFim" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Tributação
                </span>
                <select id="selCSTCTe" class="form-control">
                    <option value="">Todos</option>
                    <option value="00">00 - ICMS Normal</option>
                    <option value="20">20 - ICMS com Redução de Base de Cálculo</option>
                    <option value="40">40 - ICMS Isenção</option>
                    <option value="41">41 - ICMS Não Tributado</option>
                    <option value="51">51 - ICMS Diferido</option>
                    <option value="60">60 - ICMS Pagto atr. ao tomador ou 3º previsto para ST</option>
                    <option value="90">90 - ICMS devido à UF de origem da prestação, quando diferente da UF do emitente</option>
                    <option value="91">90 - ICMS Outras Situações</option>
                    <option value="SN">Simples Nacional</option>
                    <option value="M0">Com Imposto</option>
                    <option value="I0">Sem Imposto</option>
                </select>
            </div>
        </div>
    </div>
    <div class="row">
        <b>Tags para observação avançada (Apenas para observação automática): </b>
        <button type="button" id="lnkSerieCTe" class="btn btn-default btn-xs">Serie CTe</button>
        <button type="button" id="lnkNumeroNFe" class="btn btn-default btn-xs">Número NFe</button>
        <button type="button" id="lnkCPFCNPJRemetente" class="btn btn-default btn-xs">CPF/CNPJ do Remetente</button>
        <button type="button" id="lnkNomeRemetente" class="btn btn-default btn-xs">Nome do Remetente</button>
        <button type="button" id="lnkFantasiaRemetente" class="btn btn-default btn-xs">Fantasia do Remetente</button>
        <button type="button" id="lnkCidadeRemetente" class="btn btn-default btn-xs">Cidade do Remetente</button>
        <button type="button" id="lnkEstadoRemetente" class="btn btn-default btn-xs">Estado Remetente</button>
        <button type="button" id="lnkCPFCNPJDestinatario" class="btn btn-default btn-xs">CPF/CNPJ do Destinatario</button>
        <button type="button" id="lnkNomeDestinatario" class="btn btn-default btn-xs">Nome do Destinatario</button>
        <button type="button" id="lnkFantasiaDestinatario" class="btn btn-default btn-xs">Fantasia do Destinatario</button>
        <button type="button" id="lnkCidadeDestinatario" class="btn btn-default btn-xs">Cidade do Destinatario</button>
        <button type="button" id="lnkEstadoDestinatario" class="btn btn-default btn-xs">Estado do Destinatario</button>
        <button type="button" id="lnkCPFCNPJProprietarioVeiculo" class="btn btn-default btn-xs">CPF/CNPJ do Prop. do Veículo</button>
        <button type="button" id="lnkNomeProprietarioVeiculo" class="btn btn-default btn-xs">Nome do Prop. do Veículo</button>
        <button type="button" id="lnkRNTRCProprietario" class="btn btn-default btn-xs">RNTRC do Prop.</button>
        <button type="button" id="lnkPlaca" class="btn btn-default btn-xs">Placa Veículo</button>
        <button type="button" id="lnkRENAVAMVeiculo" class="btn btn-default btn-xs">Renavam Veic.</button>
        <button type="button" id="lnkUFVeiculo" class="btn btn-default btn-xs">UF Veic.</button>
        <button type="button" id="lnkMarcaVeiculo" class="btn btn-default btn-xs">Marca Veic.</button>
        <button type="button" id="lnkPlacasVinculadas" class="btn btn-default btn-xs">Placas Vinculadas</button>
        <button type="button" id="lnkPlacasRenavamVinculadas" class="btn btn-default btn-xs">Placas/Renavam Vinculadas</button>
        <button type="button" id="lnkNomeMotorista" class="btn btn-default btn-xs">Nome Motorista</button>
        <button type="button" id="lnkCPFMotorista" class="btn btn-default btn-xs">CPF Motorista</button>
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Descrição*:
                </span>
                <textarea id="txtObservacao" class="form-control taggedInput2" rows="3"></textarea>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Log das Alterações">Log</abbr>:
                </span>
                <textarea id="txtLog" class="form-control taggedInput" rows="5"></textarea>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnExcluir" class="btn btn-danger" style="display: none;">Excluir</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
