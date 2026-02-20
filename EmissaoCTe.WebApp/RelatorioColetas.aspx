<%@ Page Title="Relatório Coletas" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioColetas.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioColetas" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker") %>
        <%: Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/filedownload",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datepicker") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        var ParametrosPesquisa = {
            DataInicial: '',
            DataFinal: '',
            DataEntrega: '',
            Remetente: '',
            Destinatario: '',
            Tomador: '',
            Origem: 0,
            Destino: 0,
            Veiculo: 0,
            Motorista: 0,
            TipoColeta: 0,
            TipoCarga: 0
        };
        $(document).ready(function () {
            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").mask("99/99/9999");
            $("#txtDataEntrega").mask("99/99/9999");
            $("#txtVeiculo").mask("*******");

            $("#txtDataInicial").datepicker();
            $("#txtDataFinal").datepicker();
            $("#txtDataEntrega").datepicker();

            $("#txtCPFCNPJRemetente").mask("99999999999?999", { placeholder: "              " });
            $("#txtCPFCNPJDestinatario").mask("99999999999?999", { placeholder: "              " });
            $("#txtCPFCNPJTomador").mask("99999999999?999", { placeholder: "              " });

            $("#btnGerarRelatorio").click(function () {
                DownloadRelatorio();
            });

            CarregarConsultadeClientes("btnBuscarRemetente", "btnBuscarRemetente", RetornoConsultaCliente, true, false);
            CarregarConsultadeClientes("btnBuscarDestinatario", "btnBuscarDestinatario", RetornoConsultaCliente, true, false);
            CarregarConsultadeClientes("btnBuscarTomador", "btnBuscarTomador", RetornoConsultaCliente, true, false);

            CarregarConsultaDeLocalidades("btnBuscarOrigem", "btnBuscarOrigem", RetornoLocalidade, true, false);
            CarregarConsultaDeLocalidades("btnBuscarDestino", "btnBuscarDestino", RetornoLocalidade, true, false);

            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculo, true, false);
            CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);

            CarregarConsultaDeTiposDeColetas("btnBuscarTipoColeta", "btnBuscarTipoColeta", "A", RetornoConsultaTipoColeta, true, false);
            CarregarConsultaDeTiposDeCargas("btnBuscarTipoCarga", "btnBuscarTipoCarga", "A", RetornoConsultaTipoCarga, true, false);

            /**
             * Quando o nome do cliente é modificado, o cliente é limpo
             * O parametro de relatorio é limpo
             */
            RemoveConsulta($("#txtRemetente, #txtDestinatario, #txtTomador"), function ($this) {
                var tipoCliente = $this.parents('.grupo-cliente').data('cliente');
                $this.val("");
                ParametrosPesquisa[tipoCliente] = "";
                $("#txtCPFCNPJ" + tipoCliente).val("");
            });

            /**
             * Quando o cnpj do cliente é modificado, o nome cliente é limpo
             * O parametro de relatorio é limpo
             * Pois quando sair do campo, vai ser feita a pesquisa de cliente
             */
            $("#txtCPFCNPJRemetente, #txtCPFCNPJDestinatario, #txtCPFCNPJTomador")
            .keydown(function (e) {
                if (e.ctrlKey) return;
                var $this = $(this);
                var tipoCliente = $this.parents('.grupo-cliente').data('cliente');

                $("#txt" + tipoCliente).val("");
                ParametrosPesquisa[tipoCliente] = "";
            })
            .focusout(function () {
                var $this = $(this);
                var tipoCliente = $this.parents('.grupo-cliente').data('cliente');

                var cpfCnpj = $this.val().replace(/[^0-9]/g, '');

                if (cpfCnpj.length == 14) {
                    executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                        if (r.Sucesso) {
                            if (r.Objeto != null) {
                                ParametrosPesquisa[tipoCliente] = cpfCnpj;
                                $("#txt" + tipoCliente).val(r.Objeto.Nome);
                            } else {
                                ExibirMensagemAlerta(tipoCliente + " não encontrado.", "Atenção!");
                            }
                        } else {
                            ExibirMensagemErro(r.Erro, "Erro!");
                        }
                    });
                }
            });

            /**
             * Busca Veiculo automatico
             */
            $("#txtVeiculo")
            .keydown(function (e) {
                if (e.ctrlKey) return;

                ParametrosPesquisa.Veiculo = 0;
            })
            .change(function () {
                var $this = $(this);
                var placa = $this.val();

                if (placa.length == 7) {
                    executarRest("/Veiculo/BuscarPorPlacaSimples?callback=?", { Placa: placa }, function (r) {
                        if (r.Sucesso) {
                            if (r.Objeto != null) {
                                ParametrosPesquisa.Veiculo = r.Objeto.Codigo;
                            } else {
                                ExibirMensagemAlerta("Veículo não encontrado.", "Atenção!");
                                $this.val('');
                                ParametrosPesquisa.Veiculo = 0;
                            }
                        } else {
                            ExibirMensagemErro(r.Erro, "Erro!");
                            $this.val('');
                            ParametrosPesquisa.Veiculo = 0;
                        }
                    });
                }
            });

            RemoveConsulta($("#txtDestino, #txtOrigem"), function ($this) {
                var loc = $this.attr('id').replace('txt', '');
                $this.val("");
                ParametrosPesquisa[loc] = 0;
            });

            RemoveConsulta($("#txtMotorista"), function ($this) {
                $this.val("");
                ParametrosPesquisa.Motorista = 0;
            });

            RemoveConsulta($("#txtTipoColeta"), function ($this) {
                $this.val("");
                ParametrosPesquisa.TipoColeta = 0;
            });

            RemoveConsulta($("#txtTipoCarga"), function ($this) {
                $this.val("");
                ParametrosPesquisa.TipoCarga = 0;
            });
        });

        function DownloadRelatorio() {
            var dados = {
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                DataEntrega: $("#txtDataEntrega").val(),
                Remetente: ParametrosPesquisa.Remetente,
                Destinatario: ParametrosPesquisa.Destinatario,
                Tomador: ParametrosPesquisa.Tomador,
                Origem: ParametrosPesquisa.Origem,
                Destino: ParametrosPesquisa.Destino,
                Veiculo: ParametrosPesquisa.Veiculo,
                Motorista: ParametrosPesquisa.Motorista,
                TipoColeta: ParametrosPesquisa.TipoColeta,
                TipoCarga: ParametrosPesquisa.TipoCarga,
                Requisitante: $("#selRequisitante").val(),
                TipoPagamento: $("#selTipoPagamento").val(),
                Situacao: $("#selSituacao").val(),
            };

            executarDownload("/Coleta/DownloadRelatorio", dados);
        }

        function RetornoConsultaCliente(cliente, e) {
            var $btn = $(e.target);
            var tipoCliente = $btn.parents('.grupo-cliente').data('cliente');

            ParametrosPesquisa[tipoCliente] = cliente.CPFCNPJ.replace(/[^0-9]/g, '');

            var $cnpj = $("#txtCPFCNPJ" + tipoCliente).val(cliente.CPFCNPJ);
            var $nome = $("#txt" + tipoCliente).val(cliente.Nome);
        }

        function RetornoLocalidade(obj, e) {
            var $btn = $(e.target);
            var loc = $btn.data('localidade');

            $("#txt" + loc).val(obj.Descricao);
            ParametrosPesquisa[loc] = obj.Codigo;
        }

        function RetornoConsultaVeiculo(veiculo) {
            ParametrosPesquisa.Veiculo = veiculo.Codigo;
            $("#txtVeiculo").val(FormataMascara(veiculo.Placa, "###-####"));
        }
        function RetornoConsultaMotorista(motorista) {
            var cpf = motorista.CPFCNPJ.replace(/[^0-9]/g, '');

            if (cpf.length == 11)
                cpf = FormataMascara(cpf, "###.###.###-##") + " - ";
            else
                cpf = "";

            $("#txtMotorista").val(cpf + motorista.Nome);
            ParametrosPesquisa.Motorista = motorista.Codigo;
        }
        function RetornoConsultaTipoColeta(tipoColeta) {
            $("#txtTipoColeta").val(tipoColeta.Descricao);
            ParametrosPesquisa.TipoColeta = tipoColeta.Codigo;
        }
        function RetornoConsultaTipoCarga(tipoCarga) {
            $("#txtTipoCarga").val(tipoCarga.Descricao);
            ParametrosPesquisa.TipoCarga = tipoCarga.Codigo;
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Coletas
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data inicial da coleta">Dt. Inicial</abbr>*:
                </span>
                <input type="text" id="txtDataInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data final da coleta">Dt. Final</abbr>*:
                </span>
                <input type="text" id="txtDataFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data prevista da entrega">Dt. Entrega</abbr>*:
                </span>
                <input type="text" id="txtDataEntrega" class="form-control" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="grupo-cliente" data-cliente="Remetente">
            <div class="col-xs-12 col-sm-5 col-md-5 col-lg-4">
                <div class="input-group">
                    <span class="input-group-addon">CPF/CNPJ Rem.:
                    </span>
                    <input type="text" id="txtCPFCNPJRemetente" class="form-control" />
                </div>
            </div>
            <div class="col-xs-12 col-sm-7 col-md-7 col-lg-8">
                <div class="input-group">
                    <span class="input-group-addon">Nome Rem.:
                    </span>
                    <input type="text" id="txtRemetente" class="form-control" />
                    <span class="input-group-btn">
                        <button type="button" id="btnBuscarRemetente" class="btn btn-primary">Buscar</button>
                    </span>
                </div>
            </div>
        </div>
        <div class="grupo-cliente" data-cliente="Destinatario">
            <div class="col-xs-12 col-sm-5 col-md-5 col-lg-4">
                <div class="input-group">
                    <span class="input-group-addon">CPF/CNPJ Dest.:
                    </span>
                    <input type="text" id="txtCPFCNPJDestinatario" class="form-control" />
                </div>
            </div>
            <div class="col-xs-12 col-sm-7 col-md-7 col-lg-8">
                <div class="input-group">
                    <span class="input-group-addon">Nome Dest.:
                    </span>
                    <input type="text" id="txtDestinatario" class="form-control" />
                    <span class="input-group-btn">
                        <button type="button" id="btnBuscarDestinatario" class="btn btn-primary">Buscar</button>
                    </span>
                </div>
            </div>
        </div>
        <div class="grupo-cliente" data-cliente="Tomador">
            <div class="col-xs-12 col-sm-5 col-md-5 col-lg-4">
                <div class="input-group">
                    <span class="input-group-addon">CPF/CNPJ Toma.:
                    </span>
                    <input type="text" id="txtCPFCNPJTomador" class="form-control" />
                </div>
            </div>
            <div class="col-xs-12 col-sm-7 col-md-7 col-lg-8">
                <div class="input-group">
                    <span class="input-group-addon">Nome Toma.:
                    </span>
                    <input type="text" id="txtTomador" class="form-control" />
                    <span class="input-group-btn">
                        <button type="button" id="btnBuscarTomador" class="btn btn-primary">Buscar</button>
                    </span>
                </div>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Localidade de início da prestação">Origem</abbr>*:
                </span>
                <input type="text" id="txtOrigem" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarOrigem" class="btn btn-primary" data-localidade="Origem">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Localidade de término da prestação">Destino</abbr>*:
                </span>
                <input type="text" id="txtDestino" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarDestino" class="btn btn-primary" data-localidade="Destino">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    Veículo:
                </span>
                <input type="text" id="txtVeiculo" class="form-control text-uppercase" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    Motorista:
                </span>
                <input type="text" id="txtMotorista" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Tipo de coleta">Tp. Coleta</abbr>:
                </span>
                <input type="text" id="txtTipoColeta" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTipoColeta" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Tipo da carga transportada">Tp. Carga</abbr>:
                </span>
                <input type="text" id="txtTipoCarga" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTipoCarga" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Requisitante:
                </span>
                <select id="selRequisitante" class="form-control">
                    <option value="">Todos</option>
                    <option value="0">Remetente</option>
                    <option value="1">Destinatário</option>
                    <option value="2">Outros</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Tipo do Pagamento:
                </span>
                <select id="selTipoPagamento" class="form-control">
                    <option value="">Todos</option>
                    <option value="0">Pago</option>
                    <option value="1">A pagar</option>
                    <option value="2">Outros</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Situação da coleta:
                </span>
                <select id="selSituacao" class="form-control">
                    <option value="">Todos</option>
                    <option value="1">Aberta</option>
                    <option value="2">Cancelada</option>
                    <option value="3">Finalizada</option>
                </select>
            </div>
        </div>
    </div>

    <button type="button" id="btnGerarRelatorio" style="margin-top: 15px;" class="btn btn-primary">Gerar Relatório</button>
</asp:Content>

