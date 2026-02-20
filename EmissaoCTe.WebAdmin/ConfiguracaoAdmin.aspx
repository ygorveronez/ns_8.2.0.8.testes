<%@ Page Title="Configuração Empresa Admin" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ConfiguracaoAdmin.aspx.cs" Inherits="EmissaoCTe.WebAdmin.ConfiguracaoAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .fields .fields-title {
            border-bottom: #cdcdcd solid 1px;
            margin: 0 2px 5px 5px;
        }

            .fields .fields-title h3 {
                font-size: 1.5em;
                padding: 0 0 8px 8px;
                font-family: "Segoe UI", "Frutiger", "Tahoma", "Helvetica", "Helvetica Neue", "Arial", "sans-serif";
                color: #000;
                letter-spacing: -1px;
                font-weight: bold;
            }
    </style>
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/json2.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.priceformat.min.js" type="text/javascript"></script>
    <script defer="defer" type="text/javascript">
        var path = "";
        var countArquivos = 0;
        $(document).ready(function () {
            if (document.location.pathname.split("/").length > 1) {
                var paths = document.location.pathname.split("/");
                for (var i = 0; (paths.length - 1) > i; i++) {
                    if (paths[i] != "") {
                        path += "/" + paths[i];
                    }
                }
            }

            $("#txtDiasParaEmissaoDeCTeAnulacao").mask("9?99", { placeholder: "   " });
            $("#txtDiasParaEmissaoDeCTeComplementar").mask("9?99", { placeholder: "   " });
            $("#txtDiasParaEmissaoDeCTeSubstituicao").mask("9?99", { placeholder: "   " });
            $("#txtPrazoCancelamentoCTe").mask("9?99", { placeholder: "   " });
            $("#txtDiasParaEmissaoDeCTeSubstituicao").mask("9?99", { placeholder: "   " });
            $("#txtSerieCTeFora").mask("9?99");
            $("#txtSerieCTeDentro").mask("9?99");
            $("#txtSerieMDFe").mask("9?99");
            $("#txtValorLimiteFrete").priceFormat({ prefix: '' });

            $("#txtSeguradoraCNPJ").mask("99.999.999/9999-99");

            $("#btnSalvar").click(function () {
                Salvar();
            });

            executarRest("/ConfiguracaoEmpresa/ObterDetalhesEmpresaAdmin?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    RenderizarDetalhes(r.Objeto);
                    $("#divNomeEmpresa").text(" - (" + r.Objeto.NomeFantasia + ")");
                    $("#btnSalvar").show();
                } else {
                    jAlert(r.Erro, "Atenção!");
                    $("#btnSalvar").show();
                }
            });
        });

        function AtualizarVisibilidadeCampos() {
            const integrarAutomaticamente = $("#selIntegrarAutomaticamenteValePedagio").val() === "1";
            const integradoraValePedagio = $("#selIntegradoraValePedagio").val() === "2";

            $("#selIntegradoraValePedagio").closest(".field").toggle(integrarAutomaticamente);
            $("#txtUrlIntegracaoRest").closest(".field").toggle(integrarAutomaticamente && integradoraValePedagio);
        }

        function Salvar() {
            var empresa = {
                DiasParaEmissaoDeCTeAnulacao: $("#txtDiasParaEmissaoDeCTeAnulacao").val(),
                DiasParaEmissaoDeCTeComplementar: $("#txtDiasParaEmissaoDeCTeComplementar").val(),
                DiasParaEmissaoDeCTeSubstituicao: $("#txtDiasParaEmissaoDeCTeSubstituicao").val(),
                PrazoCancelamentoCTe: $("#txtPrazoCancelamentoCTe").val(),
                PrazoCancelamentoMDFe: $("#txtPrazoCancelamentoMDFe").val(),
                ProdutoPredominante: $("#txtProdutoPredominante").val(),
                OutrasCaracteristicas: $("#txtOutrasCaracteristicas").val(),

                SeguradoraCNPJ: $("#txtSeguradoraCNPJ").val(),
                SeguradoraNome: $("#txtSeguradoraNome").val(),
                ResponsavelSeguro: $("#selSeguroResponsavel").val(),
                SeguradoraNApolice: $("#txtSeguradoraNApolice").val(),
                SeguradoraNAverbacao: $("#txtSeguradoraNAverbacao").val(),

                TipoImpressao: $("#selTipoImpressao").val(),
                SerieCTeFora: $("#txtSerieCTeFora").val(),
                SerieCTeDentro: $("#txtSerieCTeDentro").val(),
                SerieMDFe: $("#txtSerieMDFe").val(),
                VersaoCTe: $("#selVersaoCTe").val(),
                VersaoMDFe: $("#selVersaoMDFe").val(),
                TokenIntegracaoCTe: $("#txtTokenIntegracaoCTe").val(),
                ValorLimiteFrete: $("#txtValorLimiteFrete").val(),
                AssinaturaEmail: $("#txtAssinaturaEmail").val(),
                CNPJTransportadorComoCNPJSeguradora: $("#chkCNPJTransportadorComoCNPJSeguradora").prop('checked') ? 1 : 0,
                NumeroApoliceComoNumeroAverbacao: $("#chkNumeroApoliceComoNumeroAverbacao").prop('checked') ? 1 : 0,
                NFSeKeyENotas: $("#txtKeyENotas").val(),
                NFSeURLENotas: $("#txtURLENotas").val(),

                AverbaAutomaticoATM: $("#selAverbaAutomatico").val(),
                CodigoATM: $("#txtCodigoSeguroATM").val(),
                UsuarioATM: $("#txtUsuarioSeguroATM").val(),
                SenhaATM: $("#txtSenhaSeguroATM").val(),
                SeguradoraAverbacao: $("#selSeguradoraAverbacao").val(),
                TokenAverbacao: $("#txtTokenSeguroBradesco").val(),
                AverbarComoEmbarcador: $("#selAverbarComoEmbarcador").val(),
                AverbarMDFe: $("#selAverbarMDFe").val(),

                IntegrarAutomaticamenteValePedagio: $("#selIntegrarAutomaticamenteValePedagio").val(),
                IntegradoraValePedagio: $("#selIntegradoraValePedagio").val(),
                UrlIntegracaoRest: $("#selIntegradoraValePedagio").val() == 2 ? $("#txtUrlIntegracaoRest").val() : "",
                UsuarioValePedagio: $("#txtUsuarioValePedagio").val(),
                SenhaValePedagio: $("#txtSenhaValePedagio").val(),
                TokenValePedagio: $("#txtTokenValePedagio").val(),
                FornecedorValePedagio: $("#txtFornecedorValePedagio").val(),
                ResponsavelValePedagio: $("#txtResponsavelValePedagio").val(),


                TrafegusURL: $("#txtTrafegusURL").val(),
                TrafegusUsuario: $("#txtTrafegusUsuario").val(),
                TrafegusSenha: $("#txtTrafegusSenha").val()

            };

            executarRest("/ConfiguracaoEmpresa/SalvarEmpresaAdmin?callback=?", empresa, function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function RenderizarDetalhes(empresa) {
            $("#txtDiasParaEmissaoDeCTeAnulacao").val(empresa.DiasParaEmissaoDeCTeAnulacao);
            $("#txtDiasParaEmissaoDeCTeComplementar").val(empresa.DiasParaEmissaoDeCTeComplementar);
            $("#txtDiasParaEmissaoDeCTeSubstituicao").val(empresa.DiasParaEmissaoDeCTeSubstituicao);
            $("#txtPrazoCancelamentoCTe").val(empresa.PrazoCancelamentoCTe);
            $("#txtPrazoCancelamentoMDFe").val(empresa.PrazoCancelamentoMDFe);
            $("#txtProdutoPredominante").val(empresa.ProdutoPredominante);
            $("#txtOutrasCaracteristicas").val(empresa.OutrasCaracteristicas);
            $("#selTipoImpressao").val(empresa.TipoImpressao);
            $("#txtSerieCTeFora").val(empresa.SerieCTeFora);
            $("#txtSerieCTeDentro").val(empresa.SerieCTeDentro);
            $("#txtSerieMDFe").val(empresa.SerieMDFe);
            $("#selVersaoCTe").val(empresa.VersaoCTe);
            $("#selVersaoMDFe").val(empresa.VersaoMDFe);
            $("#txtTokenIntegracaoCTe").val(empresa.TokenIntegracaoCTe);
            $("#txtValorLimiteFrete").val(empresa.ValorLimiteFrete);
            $("#txtAssinaturaEmail").val(empresa.AssinaturaEmail);
            $("#chkCNPJTransportadorComoCNPJSeguradora").prop('checked', empresa.CNPJTransportadorComoCNPJSeguradora);
            $("#chkNumeroApoliceComoNumeroAverbacao").prop('checked', empresa.NumeroApoliceComoNumeroAverbacao);
            $("#txtKeyENotas").val(empresa.NFSeKeyENotas);
            $("#txtURLENotas").val(empresa.NFSeURLENotas);

            $("#txtSeguradoraCNPJ").val(empresa.SeguradoraCNPJ).trigger('blur');
            $("#txtSeguradoraNome").val(empresa.SeguradoraNome);
            $("#selSeguroResponsavel").val(empresa.ResponsavelSeguro);
            $("#txtSeguradoraNApolice").val(empresa.SeguradoraNApolice);
            $("#txtSeguradoraNAverbacao").val(empresa.SeguradoraNAverbacao);

            $("#selAverbaAutomatico").val(empresa.AverbaAutomaticoATM);
            $("#txtCodigoSeguroATM").val(empresa.CodigoATM);
            $("#txtUsuarioSeguroATM").val(empresa.UsuarioATM);
            $("#txtSenhaSeguroATM").val(empresa.SenhaATM);
            $("#selSeguradoraAverbacao").val(empresa.SeguradoraAverbacao);
            $("#txtTokenSeguroBradesco").val(empresa.TokenAverbacao);
            $("#selAverbarComoEmbarcador").val(empresa.AverbarComoEmbarcador.toString());
            $("#selAverbarMDFe").val(empresa.AverbarMDFe.toString());

            $("#selIntegrarAutomaticamenteValePedagio").val(empresa.IntegrarAutomaticamenteValePedagio);
            $("#selIntegradoraValePedagio").val(empresa.IntegradoraValePedagio);
            $("#txtUrlIntegracaoRest").val(empresa.UrlIntegracaoRest);
            $("#txtUsuarioValePedagio").val(empresa.UsuarioValePedagio);
            $("#txtSenhaValePedagio").val(empresa.SenhaValePedagio);
            $("#txtTokenValePedagio").val(empresa.TokenValePedagio);
            $("#txtFornecedorValePedagio").val(empresa.FornecedorValePedagio);
            $("#txtResponsavelValePedagio").val(empresa.ResponsavelValePedagio);

            $("#txtTrafegusURL").val(empresa.TrafegusURL);
            $("#txtTrafegusUsuario").val(empresa.TrafegusUsuario);
            $("#txtTrafegusSenha").val(empresa.TrafegusSenha);

            AtualizarVisibilidadeCampos();

            $("#selIntegrarAutomaticamenteValePedagio, #selIntegradoraValePedagio").on("change", AtualizarVisibilidadeCampos);
        }

        function apenasNumeros(string) {
            var numsStr = string.replace(/[^0-9]/g, '');
            return numsStr;
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoPlanoEmissao" value="0" />
        <input type="hidden" id="hddCodigoEmpresaCobradora" value="0" />
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Configuração Empresa Admin
                    <div style="display: inline" id="divNomeEmpresa"></div>
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

                        <div class="label">
                            <label>
                                <b>Configurações emissões:</b>
                            </label>
                        </div>
                        <div class="fields">
                            <div class="fieldzao">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Dias para emissão de CTe de Anulação:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtDiasParaEmissaoDeCTeAnulacao" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Dias para emissão de CTe de Complementar:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtDiasParaEmissaoDeCTeComplementar" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Dias para emissão de CTe de Substituição:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtDiasParaEmissaoDeCTeSubstituicao" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Horas para Cancelamento CT-e:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtPrazoCancelamentoCTe" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Horas para Cancelamento MDF-e:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtPrazoCancelamentoMDFe" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Produto Predominante:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtProdutoPredominante" />
                                    </div>
                                </div>

                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Outras Caracteristicas:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtOutrasCaracteristicas" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Tipo de Impressão:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selTipoImpressao">
                                            <option value="1">Retrato</option>
                                            <option value="2">Paisagem</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Série Padrão CT-e Fora:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtSerieCTeFora" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Série Padrão CT-e Dentro:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtSerieCTeDentro" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Série MDF-e:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtSerieMDFe" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Versão CT-e:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selVersaoCTe">
                                            <option value="2.00">2.00</option>
                                            <option value="3.00">3.00</option>
                                            <option value="4.00">4.00</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Versão MDF-e:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selVersaoMDFe">
                                            <option value="1.00">1.00</option>
                                            <option value="3.00">3.00</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Valor Limite Frete:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtValorLimiteFrete" />
                                    </div>
                                </div>
                            </div>

                            <div class="fields">
                                <br />
                                <br />
                                <div class="field fieldcinco">
                                    <div class="label">
                                        <label>
                                            <b>Token Integração</b>
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTokenIntegracaoCTe" />
                                    </div>
                                </div>
                            </div>
                            <br />
                            <br />
                            <div class="label">
                                <label>
                                    <b>Averbação padrão:</b>
                                </label>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Averbar com CNPJ Embarcador:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selAverbarComoEmbarcador">
                                        <option value="false">Nao</option>
                                        <option value="true">Sim</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fieldum">
                                <div class="label">
                                    <label>
                                        Averba automaticamente:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selAverbaAutomatico">
                                        <option value="0">Não</option>
                                        <option value="1">Sim</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Integradora Averbação:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selSeguradoraAverbacao">
                                        <option value="">Não definida</option>
                                        <option value="A">ATM</option>
                                        <option value="B">Quorum</option>
                                        <option value="P">Porto Seguro</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fieldum">
                                <div class="label">
                                    <label>
                                        Averbar MDFe:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selAverbarMDFe">
                                        <option value="false">Não</option>
                                        <option value="true">Sim</option>
                                    </select>
                                </div>
                            </div>
                            <div class="fields">
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Código Averbação:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtCodigoSeguroATM" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Usuário Averbação:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtUsuarioSeguroATM" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Senha Averbação:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtSenhaSeguroATM" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Token Averbação:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTokenSeguroBradesco" />
                                    </div>
                                </div>
                            </div>
                            <br />
                            <br />
                            <div class="label">
                                <label>
                                    <b>Seguro Padrão MDF-e:</b>
                                </label>
                            </div>
                            <div class="fields">
                                <div class="field fieldtres" style="margin: 5px 0">
                                    <div class="checkbox">
                                        <input type="checkbox" id="chkCNPJTransportadorComoCNPJSeguradora" />
                                        <label for="chkCNPJTransportadorComoCNPJSeguradora">
                                            Usar o CNPJ do transportador como CNPJ da seguradora
                                        </label>
                                    </div>
                                </div>
                                <div class="field fieldtres" style="margin: 5px 0">
                                    <div class="checkbox">
                                        <input type="checkbox" id="chkNumeroApoliceComoNumeroAverbacao">
                                        <label for="chkNumeroApoliceComoNumeroAverbacao">
                                            Usar o Número da Apólice como Número de Averbação
                                        </label>
                                    </div>
                                </div>
                            </div>
                            <div class="fields">
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            CNPJ Seguradora:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtSeguradoraCNPJ" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Nome Seguradora:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtSeguradoraNome" maxlength="30" />
                                    </div>
                                </div>
                            </div>
                            <div class="fields">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Responsável:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selSeguroResponsavel">
                                            <option value="">Não definido</option>
                                            <option value="4">Emitente (Transportador)</option>
                                            <option value="5">Contratante (Tomador do Serviço)</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Nº da Apólice:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtSeguradoraNApolice" maxlength="20" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Nº da Averbação:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtSeguradoraNAverbacao" maxlength="40" />
                                    </div>
                                </div>
                            </div>

                            <br />
                            <br />
                            <div class="label">
                                <label>
                                    <b>Trafegus (SM):</b>
                                </label>
                            </div>
                            <div class="fields">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            URL:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTrafegusURL" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Usuário:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTrafegusUsuario" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Senha:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTrafegusSenha" />
                                    </div>
                                </div>
                            </div>


                            <br />
                            <br />
                            <div class="label">
                                <label>
                                    <b>Vale Pedágio Padrão:</b>
                                </label>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Integrar automaticamente Vale Pedágio:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selIntegrarAutomaticamenteValePedagio">
                                        <option value="0">Nao</option>
                                        <option value="1">Sim</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Integradora Vale Pedágio:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selIntegradoraValePedagio">
                                        <option value="0">Nenhuma</option>
                                        <option value ="1" selected>Target</option>
                                        <option value="2">Sem Parar</option>
                                    </select>
                                </div>
                            </div>
                            <div class ="field fielddois">
                                <div class="label">
                                    <label>
                                        URL integração REST
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtUrlIntegracaoRest"/>
                                </div>
                            </div>
                            <div class="fields">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Usuário Vale Pedágio:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtUsuarioValePedagio" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Senha Vale Pedágio:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtSenhaValePedagio" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Token Vale Pedágio:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTokenValePedagio" />
                                    </div>
                                </div>
                            </div>
                            <div class="fields">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            CNPJ Fornecedor Vale Pedágio:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtFornecedorValePedagio" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            CNPJ Responsável Vale Pedágio:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtResponsavelValePedagio" />
                                    </div>
                                </div>
                            </div>

                            <br />
                            <br />
                            <div class="label">
                                <label>
                                    <b>Integração eNotas (NFSe):</b>
                                </label>
                            </div>
                            <div class="fields">
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Key eNotas (NFSe):
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtKeyENotas" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            URL eNotas (NFSe)
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtURLENotas" />
                                    </div>
                                </div>
                            </div>
                            <br />
                            <br />
                            <div class="fieldzao" style="margin-bottom: 15px;">
                                <div class="field fieldseis">
                                    <div class="label">
                                        <label>
                                            <b>Ass. padrão e-mail:</b>
                                        </label>
                                    </div>
                                    <div class="input">
                                        <textarea id="txtAssinaturaEmail" rows="10" cols="10" style="width: 99.5%" maxlength="2000"></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="buttons" style="margin-left: 5px;">
                                <input type="button" id="btnSalvar" value="Salvar" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    </div>
</asp:Content>
