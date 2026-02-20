<%@ Page Title="Configuração LS Translog" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="LSTranslogConfiguracao.aspx.cs" Inherits="EmissaoCTe.WebApp.LSTranslogConfiguracao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: System.Web.Optimization.Styles.Render("~/bundle/styles/datetimepicker") %>
        <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/datetimepicker",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/ckeditor",
                           "~/bundle/scripts/ckeditoradapters",
                           "~/bundle/scripts/statecreator") %>
        <script>
            var _ClienteSelecionado;
            var StateClientes;
            $(document).ready(function () {
                // Eventos
                $("#btnSalvar").click(function () {
                    SalvarConfiguracoes();
                });
                $("#btnAdicionar").click(function () {
                    AdicionarClienteSelecionado();
                });

                // Buscas
                CarregarConsultadeClientes("btnBuscarCliente", "btnBuscarCliente", RetornoConsultaCliente, true, false);
                RemoveConsulta("#txtCliente", LimparCampoCliente);

                // Dados Salvos
                BuscarDadosSalvos();

                // Init
                StateClientes = new State({
                    name: "Cliente",
                    id: "Id",
                    render: RenderizarTabelaClientes
                });
                LimparCampos();
            });

            // Metodos
            function SalvarConfiguracoes() {
                var dados = {
                    Usuario: $("#txtUsuario").val(),
                    Senha: $("#txtSenha").val(),
                    Clientes: StateClientes.toJson()
                };

                executarRest("/LSTranslogIntegracao/SalvarConfiguracao", dados, function (r) {
                    if (r.Sucesso)
                        ExibirMensagemSucesso("", "Dados salvos com sucesso");
                    else
                        ExibirMensagemErro(r.Erro, "Atenção!");
                });
            }
            function RetornoConsultaCliente(dados) {
                $("#txtCliente").val(dados.Nome);
                $("#btnAdicionar").show();
                _ClienteSelecionado = dados;
            }
            function LimparCampoCliente() {
                $("#txtCliente").val('');
                _ClienteSelecionado = null;
                $("#btnAdicionar").hide();
            }
            function AdicionarClienteSelecionado() {
                var cliente = {
                    Nome: _ClienteSelecionado.Nome,
                    Tipo: _ClienteSelecionado.Tipo,
                    CPFCNPJ: _ClienteSelecionado.CPFCNPJ.replace(/[^0-9]/g, '')
                };

                // Valida duplicados
                var validaCliente = true;
                StateClientes.get().map(function (cli) {
                    if (cli.CPFCNPJ == cliente.CPFCNPJ)
                        validaCliente = false;
                });

                if (validaCliente) {
                    InsereCliente(cliente);
                    LimparCampoCliente();
                } else
                    ExibirMensagemAlerta("Cliente " + cliente.Nome + " já esta vinculado na lista.", "Cliente já inserido");
            }
            function LimparCampos() {
                $("#btnAdicionar").hide();
                StateClientes.clear();
            }

            function InsereCliente(obj) {
                var obj = $.extend({
                    Id: 0,
                    Nome: "",
                    Tipo: "",
                    CPFCNPJ: "",
                    Excluir: false
                }, obj);

                obj.CPFCNPJ = obj.CPFCNPJ.replace(/[^0-9]/g, '');

                if (obj.Id != 0)
                    StateClientes.update(obj);
                else
                    StateClientes.insert(obj);
            }
            function RenderizarTabelaClientes() {
                var itens = StateClientes.get();
                var $tabela = $("#tblClientesConfiguracao");

                $tabela.find("tbody").html("");

                itens.forEach(function (info) {
                    if (!info.Excluir) {
                        var cnpj = "";

                        if (info.Tipo == "J")
                            cnpj = FormataMascara(info.CPFCNPJ, "##.###.###/####-##");
                        else if (info.Tipo == "F")
                            cnpj = FormataMascara(info.CPFCNPJ, "###.###.###-##");

                        var $row = $("<tr>" +
                            "<td>" + info.Nome + "</td>" +
                            "<td>" + cnpj + "</td>" +
                            "<td><button type='button' class='btn btn-default btn-xs btn-block'>Excluir</button></td>" +
                            "</tr>");

                        $row.on("click", "button", function () {
                            ExcluirCliente(info);
                        });

                        $tabela.find("tbody").append($row);
                    }
                });

                if ($tabela.find("tbody tr").length == 0)
                    $tabela.find("tbody").html("<tr><td colspan='" + $tabela.find("thead th").length + "'>Nenhum registro encontrado.</td></tr>");
            }
            function ExcluirCliente(cliente) {
                StateClientes.remove({ Id: cliente.Id });
            }
            function BuscarDadosSalvos() {
                executarRest("/LSTranslogIntegracao/ObterDadosSalvos", {}, function (r) {
                    if (r.Sucesso) {
                        var data = r.Objeto;

                        $("#txtUsuario").val(data.Usuario)
                        $("#txtSenha").val(data.Senha)
                        StateClientes.set(data.Clientes);
                    }
                    else
                        ExibirMensagemErro(r.Erro, "Atenção!");
                });
            }
        </script>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Configuração de Integração</h2>
    </div>
    <div id="messages-placeholder">
    </div>

    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Usuário:</span>
                <input type="text" id="txtUsuario" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Senha:</span>
                <input type="text" id="txtSenha" class="form-control" />
            </div>
        </div>
    </div>

    <br />

    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6">
            <div class="input-group">
                <span class="input-group-addon">Cliente:</span>
                <input type="text" id="txtCliente" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarCliente" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6">
            <button type="button" id="btnAdicionar" class="btn btn-primary">Adicionar</button>
        </div>
    </div>
    <table id="tblClientesConfiguracao" class="table table-bordered table-condensed table-hover">
        <thead>
            <tr>
                <th width="50%">Cliente</th>
                <th width="30%">CNPJ/CPF</th>
                <th width="20%">Opção</th>
            </tr>
        </thead>
        <tbody>
        </tbody>
    </table>

    <br />
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
</asp:Content>
