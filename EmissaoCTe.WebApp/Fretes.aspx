<%@ Page Title="Cadastro de Fretes" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Fretes.aspx.cs" Inherits="EmissaoCTe.WebApp.Fretes" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
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
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/priceformat") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {

            BuscarUFs();

            $("#selUFDestino").change(function () {
                BuscarLocalidades($(this).val(), "selCidadeDestino", null);
            });

            $("#txtClienteOrigem").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoClienteOrigem").val("");
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtUnidadeDeMedida").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoUnidadeDeMedida").val("");
                    } else {
                        e.preventDefault();
                    }
                }
            });

            CarregarConsultaDeUnidadesDeMedidas("btnBuscarUnidadeDeMedida", "btnBuscarUnidadeDeMedida", RetornoConsultaUnidadeDeMedida, true, false);
            CarregarConsultadeClientes("btnBuscarClienteOrigem", "btnBuscarClienteOrigem", RetornoConsultaClienteOrigem, true, false);

            $("#txtValorFrete").priceFormat({ prefix: '', centsLimit: 4 });
            $("#txtValorPedagio").priceFormat({ prefix: '' });
            $("#txtValorSeguro").priceFormat({ prefix: '' });
            $("#txtOutrosValores").priceFormat({ prefix: '' });
            $("#txtQuantidadeExcedente").priceFormat({ prefix: '' });
            $("#txtValorExcedente").priceFormat({ prefix: '', centsLimit: 4 });
            $("#txtDataInicial").datepicker();
            $("#txtDataFinal").datepicker();

            $("#txtValorMinimo").priceFormat({ prefix: '' });
            $("#txtValorDescarga").priceFormat({ prefix: '' });
            $("#txtPercentualGris").priceFormat({ prefix: '', centsLimit: 4 });
            $("#txtPercentualAdValorem").priceFormat({ prefix: '', centsLimit: 4 });
            $("#txtPercentualPedagio").priceFormat({ prefix: '', centsLimit: 4 });

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            CarregarConsultaDeFretes("default-search", "default-search", "", RetornoConsultaFrete, true, false);
        });

        function RetornoConsultaFrete(frete) {
            executarRest("/Frete/ObterDetalhes?callback=?", { Codigo: frete.Codigo }, function (r) {
                if (r.Sucesso) {
                    frete = r.Objeto;
                    $("#hddCodigo").val(frete.Codigo);
                    $("#hddCodigoClienteOrigem").val(frete.CPFCNPJClienteOrigem);
                    $("#txtClienteOrigem").val(frete.NomeClienteOrigem);
                    BuscarLocalidades(frete.UFDestino, "selCidadeDestino", frete.CodigoLocalidadeDestino);
                    $("#selUFDestino").val(frete.UFDestino);
                    $("#hddCodigoUnidadeDeMedida").val(frete.CodigoUnidadeMedida);
                    $("#txtUnidadeDeMedida").val(frete.DescricaoUnidadeMedida);
                    $("#txtValorFrete").val(frete.ValorFrete);
                    $("#txtValorPedagio").val(frete.ValorPedagio);
                    $("#txtValorSeguro").val(frete.ValorSeguro);
                    $("#txtOutrosValores").val(frete.OutrosValores);
                    $("#txtDataInicial").val(frete.DataInicio);
                    $("#txtDataFinal").val(frete.DataFim);
                    $("#selStatus").val(frete.Status);
                    $("#txtQuantidadeExcedente").val(frete.QuantidadeExcedente);
                    $("#txtValorExcedente").val(frete.ValorExcedente);
                    $("#selTipoPagamento").val(frete.TipoPagamento);

                    $("#txtValorMinimo").val(frete.ValorMinimo);
                    $("#txtValorDescarga").val(frete.ValorDescarga);
                    $("#txtPercentualGris").val(frete.PercentualGris);
                    $("#txtPercentualAdValorem").val(frete.PercentualAdValorem);

                    $("#selIncluirICMS").val(frete.IncluiICMS);
                    $("#selIncluirPedagioBC").val(frete.IncluirPedagioBC);
                    $("#selIncluirSeguroBC").val(frete.IncluirSeguroBC);
                    $("#selIncluirOutrosBC").val(frete.IncluirOutrosBC);
                    $("#selIncluirDescargaBC").val(frete.IncluirDescargaBC);
                    $("#selIncluirGrisBC").val(frete.IncluirGrisBC);
                    $("#selIncluirAdValoremBC").val(frete.IncluirAdValoremBC);
                    $("#selTipoCliente").val(frete.TipoCliente);
                    $("#selTipoMinimo").val(frete.TipoMinimo);
                    
                    $("#txtPercentualPedagio").val(frete.ValorPedagioPerc);                  

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function RetornoConsultaUnidadeDeMedida(unidade) {
            $("#txtUnidadeDeMedida").val(unidade.CodigoDaUnidade + " - " + unidade.Descricao);
            $("#hddCodigoUnidadeDeMedida").val(unidade.Codigo);
        }

        function BuscarLocalidades(uf, idSelect, codigo) {
            executarRest("/Localidade/BuscarPorUF?callback=?", { UF: uf }, function (r) {
                if (r.Sucesso) {
                    RenderizarLocalidades(r.Objeto, idSelect, codigo);
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }

        function RenderizarLocalidades(localidades, idSelect, codigo) {
            var selLocalidades = document.getElementById(idSelect);
            selLocalidades.options.length = 0;
            for (var i = 0; i < localidades.length; i++) {
                var optn = document.createElement("option");
                optn.text = localidades[i].Descricao;
                optn.value = localidades[i].Codigo;
                if (codigo != null) {
                    if (codigo == localidades[i].Codigo) {
                        optn.setAttribute("selected", "selected");
                    }
                }
                selLocalidades.options.add(optn);
            }
        }

        function BuscarUFs() {
            executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    RenderizarUFs(r.Objeto, "selUFDestino");
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }

        function RenderizarUFs(ufs, idSelect) {
            var selUFs = document.getElementById(idSelect);
            selUFs.options.length = 0;
            var optn = document.createElement("option");
            optn.text = 'Selecione';
            optn.value = '0';
            selUFs.options.add(optn);
            for (var i = 0; i < ufs.length; i++) {
                var optn = document.createElement("option");
                optn.text = ufs[i].Sigla + " - " + ufs[i].Nome;
                optn.value = ufs[i].Sigla;
                selUFs.options.add(optn);
            }
        }

        function RetornoConsultaClienteOrigem(cliente) {
            $("#txtClienteOrigem").val(cliente.CPFCNPJ + " - " + cliente.Nome);
            $("#hddCodigoClienteOrigem").val(cliente.CPFCNPJ);
        }

        function LimparCampos() {
            $("#hddCodigo").val("0");
            $("#hddCodigoClienteOrigem").val("");
            $("#txtClienteOrigem").val("");
            $("#selUFDestino").val($("#selUFDestino option:first").val());
            $("#selCidadeDestino").html("");
            $("#hddCodigoUnidadeDeMedida").val("0");
            $("#txtUnidadeDeMedida").val("");
            $("#txtValorFrete").val("0,00");
            $("#txtValorPedagio").val("0,00");
            $("#txtValorSeguro").val("0,00");
            $("#txtOutrosValores").val("0,00");
            $("#txtDataInicial").val("");
            $("#txtDataFinal").val("");
            $("#selStatus").val($("#selStatus option:first").val());
            $("#txtQuantidadeExcedente").val("0,00");
            $("#txtValorExcedente").val("0,0000");
            $("#selTipoPagamento").val($("#selTipoPagamento option:first").val());

            $("#txtValorMinimo").val("0,00");
            $("#txtValorDescarga").val("0,00");
            $("#txtPercentualGris").val("0,0000");
            $("#txtPercentualAdValorem").val("0,0000");
            $("#txtPercentualPedagio").val("0,0000");

            $("#selIncluirICMS").val($("#selIncluirICMS option:first").val());
            $("#selIncluirPedagioBC").val($("#selIncluirPedagioBC option:first").val());
            $("#selIncluirSeguroBC").val($("#selIncluirSeguroBC option:first").val());
            $("#selIncluirOutrosBC").val($("#selIncluirOutrosBC option:first").val());
            $("#selIncluirDescargaBC").val($("#selIncluirDescargaBC option:first").val());
            $("#selIncluirGrisBC").val($("#selIncluirGrisBC option:first").val());
            $("#selIncluirAdValoremBC").val($("#selIncluirAdValoremBC option:first").val());
            $("#selTipoCliente").val($("#selTipoCliente option:first").val());
            $("#selTipoMinimo").val($("#selTipoMinimo option:first").val());
        }

        function ValidarCampos() {
            var clienteOrigem = $("#hddCodigoClienteOrigem").val();
            var unidadeDeMedida = Globalize.parseInt($("#hddCodigoUnidadeDeMedida").val());

            var valido = true;

            if (clienteOrigem != "") {
                CampoSemErro("#txtClienteOrigem");
            } else {
                CampoComErro("#txtClienteOrigem");
                valido = false;
            }

            if (unidadeDeMedida > 0) {
                CampoSemErro("#txtUnidadeDeMedida");
            } else {
                CampoComErro("#txtUnidadeDeMedida");
                valido = false;
            }

            return valido;
        }

        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    CodigoClienteOrigem: $("#hddCodigoClienteOrigem").val(),
                    CodigoCidadeDestino: $("#selCidadeDestino").val(),
                    CodigoUnidadeMedida: $("#hddCodigoUnidadeDeMedida").val(),
                    ValorFrete: $("#txtValorFrete").val(),
                    ValorPedagio: $("#txtValorPedagio").val(),
                    ValorSeguro: $("#txtValorSeguro").val(),
                    OutrosValores: $("#txtOutrosValores").val(),
                    DataInicial: $("#txtDataInicial").val(),
                    DataFinal: $("#txtDataFinal").val(),
                    Status: $("#selStatus").val(),
                    QuantidadeExcedente: $("#txtQuantidadeExcedente").val(),
                    ValorExcedente: $("#txtValorExcedente").val(),
                    TipoPagamento: $("#selTipoPagamento").val(),

                    ValorMinimo: $("#txtValorMinimo").val(),
                    ValorDescarga: $("#txtValorDescarga").val(),
                    PercentualGris: $("#txtPercentualGris").val(),
                    PercentualAdValorem: $("#txtPercentualAdValorem").val(),
                    IncluirICMS: $("#selIncluirICMS").val(),
                    IncluirPedagioBC: $("#selIncluirPedagioBC").val(),
                    IncluirSeguroBC: $("#selIncluirSeguroBC").val(),
                    IncluirOutrosBC: $("#selIncluirOutrosBC").val(),
                    IncluirDescargaBC: $("#selIncluirDescargaBC").val(),
                    IncluirGrisBC: $("#selIncluirGrisBC").val(),
                    IncluirAdValoremBC: $("#selIncluirAdValoremBC").val(),
                    TipoCliente: $("#selTipoCliente").val(),
                    TipoMinimo: $("#selTipoMinimo").val(),
                    ValorPedagioPerc: $("#txtPercentualPedagio").val()
                };
                executarRest("/Frete/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!");
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoClienteOrigem" value="" />
        <input type="hidden" id="hddCodigoUnidadeDeMedida" value="0" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Fretes por Peso
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Cliente*:
                </span>
                <input type="text" id="txtClienteOrigem" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarClienteOrigem" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Tipo Cliente:
                </span>
                <select id="selTipoCliente" class="form-control">
                    <option value="0">Remetente</option>
                    <option value="1">Expedidor</option>
                    <option value="2">Recebedor</option>
                    <option value="3">Destinatário</option>
                    <option value="4">Tomador</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">UF Destino:
                </span>
                <select id="selUFDestino" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Cidade Destino:
                </span>
                <select id="selCidadeDestino" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Unidade Medida*:
                </span>
                <input type="text" id="txtUnidadeDeMedida" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarUnidadeDeMedida" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor do Frete por Unidade de Medida">Valor Frete</abbr>:
                </span>
                <input type="text" id="txtValorFrete" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor mínimo do Frete sobre a soma de todos componentes">Valor Mínimo</abbr>:
                </span>
                <input type="text" id="txtValorMinimo" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Tipo Minimo:
                </span>
                <select id="selTipoMinimo" class="form-control">
                    <option value="0">Mínimo Garantido</option>
                    <option value="1">Mínimo + Componentes</option>
                </select>
            </div
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor fixo para pedágio">Valor Pedágio</abbr>:
                </span>
                <input type="text" id="txtValorPedagio" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor calculado por unidade para o pedágio">Valor Pedágio Unidade</abbr>:
                </span>
                <input type="text" id="txtPercentualPedagio" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Incluir o valor do pedágio na BC do ICMS">Incluir ped. BC</abbr>:
                </span>
                <select id="selIncluirPedagioBC" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor Seguro:
                </span>
                <input type="text" id="txtValorSeguro" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Incluir o valor do seguro na BC do ICMS">Incluir seg. BC</abbr>:
                </span>
                <select id="selIncluirSeguroBC" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Outros Valores:
                </span>
                <input type="text" id="txtOutrosValores" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Incluir o outros valores na BC do ICMS">Incluir Outros BC</abbr>:
                </span>
                <select id="selIncluirOutrosBC" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor Descarga:
                </span>
                <input type="text" id="txtValorDescarga" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Incluir o descarga na BC do ICMS">Incluir Des. BC</abbr>:
                </span>
                <select id="selIncluirDescargaBC" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Percentual sobre o valor da mercadoria">% Ad Valorem</abbr>:
                </span>
                <input type="text" id="txtPercentualAdValorem" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Incluir valor Ad Valorem  do GRIS na BC do ICMS">Incluir AdValorem BC</abbr>:
                </span>
                <select id="selIncluirAdValoremBC" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Percentual sobre o valor da mercadoria">% Gris</abbr>:
                </span>
                <input type="text" id="txtPercentualGris" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Incluir valor do GRIS na BC do ICMS">Incluir GRIS BC</abbr>:
                </span>
                <select id="selIncluirGrisBC" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Quantidade para Início do Cálculo de Valor Excedente">Qtd. Excedente</abbr>:
                </span>
                <input type="text" id="txtQuantidadeExcedente" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor por Unidade Excedente">Vl. Excedente</abbr>:
                </span>
                <input type="text" id="txtValorExcedente" class="form-control" value="0,000" />
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
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Pgto.*:
                </span>
                <select id="selTipoPagamento" class="form-control">
                    <option value="0">Todos</option>
                    <option value="1">Pago</option>
                    <option value="2">A Pagar</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Incluir ICMS*:
                </span>
                <select id="selIncluirICMS" class="form-control">
                    <option value="0">Não</option>
                    <option value="1">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="A">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>

