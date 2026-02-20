<%@ Page Title="Cadastro de Frete Fracionado por Valor" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="FreteFracionadoValor.aspx.cs" Inherits="EmissaoCTe.WebApp.FreteFracionadoValor" %>

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
            CarregarConsultadeClientes("btnBuscarClienteOrigem", "btnBuscarClienteOrigem", RetornoConsultaClienteOrigem, true, false);

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

            $("#txtValorDe").priceFormat({ prefix: '', centsLimit: 4 });
            $("#txtValorAte").priceFormat({ prefix: '', centsLimit: 4 });
            $("#txtValorFrete").priceFormat({ prefix: '', centsLimit: 2 });
            $("#txtValorExcedente").priceFormat({ prefix: '', centsLimit: 2 });
            $("#txtPercentualAdValorem").priceFormat({ prefix: '', centsLimit: 2 });
            $("#txtValorPedagio").priceFormat({ prefix: '', centsLimit: 2 });
            $("#txtValorTAS").priceFormat({ prefix: '', centsLimit: 2 });
            $("#txtPercentualGris").priceFormat({ prefix: '', centsLimit: 2 });
            $("#txtValorMinimoAdValorem").priceFormat({ prefix: '', centsLimit: 2 });
            $("#txtValorMinimoGris").priceFormat({ prefix: '', centsLimit: 2 });
            $("#chkTodasCidadesDoEstado").prop('checked', false);

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            CarregarConsultaDeFreteFracionadoValor("default-search", "default-search", "", RetornoConsultaFrete, true, false);
        });

        function RetornoConsultaFrete(frete) {
            executarRest("/FreteFracionadoValor/obterdetalhes?callback=?", { Codigo: frete.Codigo }, function (r) {
                if (r.Sucesso) {
                    frete = r.Objeto;
                    $("#hddCodigo").val(r.Objeto.Codigo);
                    $("#hddCodigoClienteOrigem").val(r.Objeto.CPFCNPJClienteOrigem);
                    $("#txtClienteOrigem").val(frete.NomeClienteOrigem);
                    BuscarLocalidades(r.Objeto.UFDestino, "selCidadeDestino", r.Objeto.CodigoLocalidadeDestino);
                    $("#selUFDestino").val(r.Objeto.UFDestino);

                    $("#txtValorDe").val(r.Objeto.ValorDe);
                    $("#txtValorAte").val(r.Objeto.ValorAte);
                    $("#txtValorFrete").val(r.Objeto.ValorFrete);
                    $("#txtValorExcedente").val(r.Objeto.ValorExcedente);
                    $("#txtPercentualAdValorem").val(r.Objeto.PercentualAdValorem);
                    $("#txtValorPedagio").val(r.Objeto.ValorPedagio);
                    $("#txtValorTAS").val(r.Objeto.ValorTAS);
                    $("#txtPercentualGris").val(r.Objeto.PercentualGris);
                    $("#txtValorMinimoGris").val(r.Objeto.ValorMinimoGris);
                    $("#txtValorMinimoAdValorem").val(r.Objeto.ValorMinimoAdValorem);

                    $("#selStatus").val(r.Objeto.Status);
                    $("#selIncluirICMS").val(r.Objeto.IncluirICMS);
                    $("#selTipoCliente").val(r.Objeto.TipoCliente);
                    $("#selTipoValor").val(r.Objeto.TipoValor);

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
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
            $("#txtValorDe").val("0,0000");
            $("#txtValorAte").val("0,0000");
            $("#txtValorFrete").val("0,00");
            $("#txtValorExcedente").val("0,00");
            $("#txtPercentualAdValorem").val("0,00");
            $("#txtValorPedagio").val("0,00");
            $("#txtValorTAS").val("0,00");
            $("#txtPercentualGris").val("0,00");
            $("#txtValorMinimoGris").val("0,00");
            $("#txtValorMinimoAdValorem").val("0,00");

            $("#selTipoValor").val($("#selTipoValor option:first").val());
            $("#selTipoCliente").val($("#selTipoCliente option:first").val());
        }

        function ValidarCampos() {
            var clienteOrigem = $("#hddCodigoClienteOrigem").val();
            var codigoCidadeDestino = $("#selCidadeDestino").val()

            var valido = true;

            //if (clienteOrigem != "") {
            //    CampoSemErro("#txtClienteOrigem");
            //} else {
            //    CampoComErro("#txtClienteOrigem");
            //    valido = false;
            //}

            if (codigoCidadeDestino != "" && codigoCidadeDestino != null) {
                CampoSemErro("#selCidadeDestino");
            } else {
                CampoComErro("#selCidadeDestino");
                valido = false;
            }

            return valido;
        }

        function Salvar() {

            if ($("#chkTodasCidadesDoEstado").prop('checked')) {
                jConfirm("Valores da tabela vão ser replicado para todos municipios do estado destino selecionado, deseja continuar? ", "Atenção", function (ret) {
                    if (ret) {
                        if (ValidarCampos()) {
                            var dados = {
                                Codigo: $("#hddCodigo").val(),
                                CodigoClienteOrigem: $("#hddCodigoClienteOrigem").val(),
                                CodigoCidadeDestino: $("#selCidadeDestino").val(),
                                ValorDe: $("#txtValorDe").val(),
                                ValorAte: $("#txtValorAte").val(),
                                ValorFrete: $("#txtValorFrete").val(),
                                ValorPedagio: $("#txtValorPedagio").val(),
                                ValorTAS: $("#txtValorTAS").val(),
                                ValorExcedente: $("#txtValorExcedente").val(),
                                PercentualGris: $("#txtPercentualGris").val(),
                                PercentualAdValorem: $("#txtPercentualAdValorem").val(),
                                Status: $("#selStatus").val(),
                                IncluirICMS: $("#selIncluirICMS").val(),
                                TipoCliente: $("#selTipoCliente").val(),
                                ValorMinimoGris: $("#txtValorMinimoGris").val(),
                                ValorMinimoAdValorem: $("#txtValorMinimoAdValorem").val(),
                                TipoValor: $("#selTipoValor").val(),
                                TodasCidadesDoEstado: $("#chkTodasCidadesDoEstado").prop('checked')
                            };
                            executarRest("/FreteFracionadoValor/Salvar?callback=?", dados, function (r) {
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
                });
            }
            else {
                if (ValidarCampos()) {
                    var dados = {
                        Codigo: $("#hddCodigo").val(),
                        CodigoClienteOrigem: $("#hddCodigoClienteOrigem").val(),
                        CodigoCidadeDestino: $("#selCidadeDestino").val(),
                        ValorDe: $("#txtValorDe").val(),
                        ValorAte: $("#txtValorAte").val(),
                        ValorFrete: $("#txtValorFrete").val(),
                        ValorPedagio: $("#txtValorPedagio").val(),
                        ValorTAS: $("#txtValorTAS").val(),
                        ValorExcedente: $("#txtValorExcedente").val(),
                        PercentualGris: $("#txtPercentualGris").val(),
                        PercentualAdValorem: $("#txtPercentualAdValorem").val(),
                        Status: $("#selStatus").val(),
                        IncluirICMS: $("#selIncluirICMS").val(),
                        TipoCliente: $("#selTipoCliente").val(),
                        ValorMinimoGris: $("#txtValorMinimoGris").val(),
                        ValorMinimoAdValorem: $("#txtValorMinimoAdValorem").val(),
                        TipoValor: $("#selTipoValor").val(),
                        TodasCidadesDoEstado: $("#chkTodasCidadesDoEstado").prop('checked')
                    };
                    executarRest("/FreteFracionadoValor/Salvar?callback=?", dados, function (r) {
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
        }
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoClienteOrigem" value="" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Frete Fracionado por Valor
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
                <span class="input-group-addon">Cliente:
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
                <span class="input-group-addon">UF Destino*:
                </span>
                <select id="selUFDestino" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Cidade Destino*:
                </span>
                <select id="selCidadeDestino" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <div class="checkbox">
                    <label>
                        <input type="checkbox" id="chkTodasCidadesDoEstado" />
                        Replicar tabela para todas cidades do estado destino selecionado
                    </label>
                </div>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor mercadoria inicial">Valor De*</abbr>:
                </span>
                <input type="text" id="txtValorDe" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor mercadoria final">Valor Até*</abbr>:
                </span>
                <input type="text" id="txtValorAte" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Frete*:
                </span>
                <select id="selTipoValor" class="form-control">
                    <option value="V">Valor</option>
                    <option value="P">% Valor merc.</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Quanto tipo for Percentual será calculado sobre o valor da mercadoria, quando for Valor será o Valor do Frete">Frete*</abbr>:
                </span>
                <input type="text" id="txtValorFrete" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor Excedente apenas quando Tipo Frete for Valor">Valor Excedente</abbr>:
                </span>
                <input type="text" id="txtValorExcedente" class="form-control" value="0,00" />
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
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor do Frete">Valor Min. Ad Valorem</abbr>:
                </span>
                <input type="text" id="txtValorMinimoAdValorem" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor Pedágio:
                </span>
                <input type="text" id="txtValorPedagio" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor TAS:
                </span>
                <input type="text" id="txtValorTAS" class="form-control" value="0,00" />
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
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor do Frete">Valor Min. Gris</abbr>:
                </span>
                <input type="text" id="txtValorMinimoGris" class="form-control" value="0,00" />
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
