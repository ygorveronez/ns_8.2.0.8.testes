using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CentroCarregamento
    {
        #region Atributos Protegidos Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public CentroCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Logistica.CentroCarregamento ObterCentroCarregamentoPorFilialContrato(int codigoTransportador)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> contratos = repositorioContratoFreteTransportador.BuscarAtivoPorTransportador(codigoTransportador);

            foreach (Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato in contratos)
            {
                foreach (Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFilial contratoFilial in contrato.Filiais)
                {
                    Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(contratoFilial.Filial.Codigo);

                    if (centroCarregamento != null)
                        return centroCarregamento;
                }
            }

            return null;
        }

        private string RenderizarEmail(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, int codigoCliente, string stringConexaoAdmin)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);

            StringBuilder sb = new StringBuilder();

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Usuario motorista = carregamento.Motoristas.FirstOrDefault();
            Dominio.Entidades.Usuario usuarioAgendamento = carregamento.UsuarioAgendamento;
            Dominio.Entidades.Cliente clienteFilial = repCliente.BuscarPorCPFCNPJ(double.Parse(carregamento.Filial.CNPJ));
            Dominio.Entidades.Cliente destinatario = repositorioCarregamentoPedido.BuscarPrimeiroDestinatarioPedido(carregamento.Codigo);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repCentroCarregamento.BuscarPorFilial(carregamento.Filial.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repositorioCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);

            string data = carregamento.DataCarregamentoCarga?.ToDateString() ?? "";
            string hora = carregamento.DataCarregamentoCarga?.ToTimeString() ?? "";
            string enderecoLogoEmpresa = ObterEnderecoLogoCliente(tipoServicoMultisoftware, codigoCliente, stringConexaoAdmin);

            string ObservacaoHTML = !string.IsNullOrWhiteSpace(centroCarregamento.ObservacaoRetira) ? centroCarregamento.ObservacaoRetira : Localization.Resources.Pedidos.RetiradaProduto.ObservacaoHTML;

            sb.Append("<!DOCTYPE HTML PUBLIC '-//W3C//DTD XHTML 1.0 Transitional //EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>");
            sb.Append("<html xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>");
            sb.Append("<head>");
            sb.Append("<!--[if gte mso 9]>");
            sb.Append("<xml>");
            sb.Append("  <o:OfficeDocumentSettings>");
            sb.Append("    <o:AllowPNG/>");
            sb.Append("    <o:PixelsPerInch>96</o:PixelsPerInch>");
            sb.Append("  </o:OfficeDocumentSettings>");
            sb.Append("</xml>");
            sb.Append("<![endif]-->");
            sb.Append("  <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>");
            sb.Append("  <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            sb.Append("  <meta name='x-apple-disable-message-reformatting'>");
            sb.Append("  <!--[if !mso]><!--><meta http-equiv='X-UA-Compatible' content='IE=edge'><!--<![endif]-->");
            sb.Append("  <title></title>");
            sb.Append("  ");
            sb.Append("    <style type='text/css'>");

            sb.Append("@media only screen and (min-width: 520px) {");
            sb.Append("  .u-row {");
            sb.Append("    width: 500px !important;");
            sb.Append("  }");
            sb.Append("  .u-row .u-col {");
            sb.Append("    vertical-align: top;");
            sb.Append("  }");
            sb.Append("  .u-row .u-col-25 {");
            sb.Append("    width: 125px !important;");
            sb.Append("  }");
            sb.Append("  .u-row .u-col-50 {");
            sb.Append("    width: 250px !important;");
            sb.Append("  }");
            sb.Append("  .u-row .u-col-100 {");
            sb.Append("    width: 500px !important;");
            sb.Append("  }");
            sb.Append("}");
            sb.Append("@media (max-width: 520px) {");
            sb.Append("  .u-row-container {");
            sb.Append("    max-width: 100% !important;");
            sb.Append("    padding-left: 0px !important;");
            sb.Append("    padding-right: 0px !important;");
            sb.Append("  }");
            sb.Append("  .u-row .u-col {");
            sb.Append("    min-width: 320px !important;");
            sb.Append("    max-width: 100% !important;");
            sb.Append("    display: block !important;");
            sb.Append("  }");
            sb.Append("  .u-row {");
            sb.Append("    width: calc(100% - 40px) !important;");
            sb.Append("  }");
            sb.Append("  .u-col {");
            sb.Append("    width: 100% !important;");
            sb.Append("  }");
            sb.Append("  .u-col > div {");
            sb.Append("    margin: 0 auto;");
            sb.Append("  }");
            sb.Append("}");
            sb.Append("body {");
            sb.Append("  margin: 0;");
            sb.Append("  padding: 0;");
            sb.Append("}");
            sb.Append("table,");
            sb.Append("tr,");
            sb.Append("td {");
            sb.Append("  vertical-align: top;");
            sb.Append("  border-collapse: collapse;");
            sb.Append("}");
            sb.Append("p {");
            sb.Append("  margin: 0;");
            sb.Append("}");
            sb.Append(".ie-container table,");
            sb.Append(".mso-container table {");
            sb.Append("  table-layout: fixed;");
            sb.Append("}");
            sb.Append("* {");
            sb.Append("  line-height: inherit;");
            sb.Append("}");
            sb.Append("a[x-apple-data-detectors='true'] {");
            sb.Append("  color: inherit !important;");
            sb.Append("  text-decoration: none !important;");
            sb.Append("}");

            sb.Append("</style>");
            sb.Append("  ");
            sb.Append("  ");
            sb.Append("</head>");
            sb.Append("<body class='clean-body' style='margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #e7e7e7'>");

            sb.Append("  <!--[if IE]><div class='ie-container'><![endif]-->");
            sb.Append("  <!--[if mso]><div class='mso-container'><![endif]-->");
            sb.Append("  <table style='border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #e7e7e7;width:100%' cellpadding='0' cellspacing='0'>");
            sb.Append("  <tbody>");
            sb.Append("  <tr style='vertical-align: top'>");
            sb.Append("    <td style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>");
            sb.Append("    <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td align='center' style='background-color: #e7e7e7;'><![endif]-->");
            sb.Append("    ");
            sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
            sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
            sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
            sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
            sb.Append("      ");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='500' style='width: 500px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("<table width='100%' cellpadding='0' cellspacing='0' border='0'>");
            sb.Append("  <tr>");
            sb.Append("    <td style='padding-right: 0px;padding-left: 0px;' align='center'>");
            sb.Append("      ");
            sb.Append($"      <img src='{enderecoLogoEmpresa}' alt='Logo' title='Logo' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 100%;max-width: 200px;' width='480'/>");
            sb.Append("      ");
            sb.Append("    </td>");
            sb.Append("  </tr>");
            sb.Append("</table>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
            sb.Append("    </div>");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
            sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
            sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
            sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
            sb.Append("      ");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='250' style='background-color: #f2f2f2;width: 250px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-50' style='max-width: 320px;min-width: 250px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='background-color: #f2f2f2;width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("  <div style='color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>");
            sb.Append($"    <p style='font-size: 14px; line-height: 140%;'>{Localization.Resources.Pedidos.RetiradaProduto.Data}: {data}</p>");
            sb.Append($"    <p style='font-size: 14px; line-height: 140%;'>{Localization.Resources.Pedidos.RetiradaProduto.Hora}: {hora}</p>");
            sb.Append("  </div>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='250' style='background-color: #f2f2f2;width: 250px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-50' style='max-width: 320px;min-width: 250px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='background-color: #f2f2f2;width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("  <div style='color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>");
            sb.Append($"    <p style='font-size: 14px; line-height: 140%;'>{Localization.Resources.Pedidos.RetiradaProduto.Carregamento} N&ordm; {carregamento.NumeroCarregamento}</p>");
            sb.Append("<p style='font-size: 14px; line-height: 140%;'>&nbsp;</p>");
            sb.Append("  </div>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
            sb.Append("    </div>");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
            sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
            sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
            sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
            sb.Append("      ");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='500' style='width: 500px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("  <div style='color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>");
            sb.Append($"    <p style='font-size: 14px; line-height: 140%;'>{Localization.Resources.Pedidos.RetiradaProduto.Agendamento} N&ordm; {carregamento.NumeroCarregamento}</p>");
            sb.Append($"<p style='font-size: 14px; line-height: 140%;'>{Localization.Resources.Pedidos.RetiradaProduto.LocalExpedicaoHTML}: {clienteFilial?.EnderecoCompleto}<br />{clienteFilial?.Telefone1}</p>");
            sb.Append("  </div>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
            sb.Append("    </div>");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
            sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
            sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
            sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
            sb.Append("      ");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='500' style='background-color: #fddc8b;width: 500px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='background-color: #fddc8b;width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("  <h1 style='margin: 0px; color: #000000; line-height: 140%; text-align: center; word-wrap: break-word; font-weight: normal; font-family: arial,helvetica,sans-serif; font-size: 22px;'>");
            sb.Append($"    {Localization.Resources.Pedidos.RetiradaProduto.AtencaoHTML}");
            sb.Append("  </h1>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
            sb.Append("    </div>");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
            sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
            sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
            sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
            sb.Append("      ");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='500' style='background-color: #f66053;width: 500px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='background-color: #f66053;width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("  <div style='color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>");

            if (configuracaoPedido.ModeloEmailAgendamentoPedido == ModeloEmailAgendamentoPedido.Modelo1)
                sb.Append($"    <p style='font-size: 14px; line-height: 140%;'>{Localization.Resources.Pedidos.RetiradaProduto.Aviso1HTML}.</p>");

            sb.Append($"    <p style='font-size: 14px; line-height: 140%;'>{Localization.Resources.Pedidos.RetiradaProduto.Aviso2HTML}.</p>");
            sb.Append($"    <p style='font-size: 14px; line-height: 140%;'>{Localization.Resources.Pedidos.RetiradaProduto.Aviso3HTML}.</p>");
            sb.Append($"    <p style='font-size: 14px; line-height: 140%;'>{Localization.Resources.Pedidos.RetiradaProduto.Aviso4HTML}.</p>");
            sb.Append($"    <p style='font-size: 14px; line-height: 140%;'>{Localization.Resources.Pedidos.RetiradaProduto.Aviso5HTML}.</p>");
            //sb.Append($"    <p style='font-size: 14px; line-height: 140%;'>{motorista.Nome}, deve chegar ao CD &agrave;s {hora} do dia {data}</p>");
            sb.Append("  </div>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
            sb.Append("    </div>");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
            sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
            sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
            sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
            sb.Append("      ");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='500' style='background-color: #f2f2f2;width: 500px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='background-color: #f2f2f2;width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("  <h4 style='margin: 0px; color: #000000; line-height: 140%; text-align: left; word-wrap: break-word; font-weight: normal; font-family: arial,helvetica,sans-serif; font-size: 12px;'>");
            sb.Append($"    <strong>{Localization.Resources.Pedidos.RetiradaProduto.AgendamentoEfetuadoPor}:</strong>");
            sb.Append("  </h4>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
            sb.Append("    </div>");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
            sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
            sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
            sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
            sb.Append("      ");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='500' style='width: 500px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("  <div style='color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>");
            sb.Append($"    <p style='font-size: 14px; line-height: 140%;'>{Localization.Resources.Pedidos.RetiradaProduto.Nome}:{usuarioAgendamento?.Nome}<br />SAP: {usuarioAgendamento?.CodigoIntegracao}<br />Email:{usuarioAgendamento?.Email}</p>");
            sb.Append("  </div>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
            sb.Append("    </div>");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
            sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
            sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
            sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
            sb.Append("      ");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='500' style='background-color: #f2f2f2;width: 500px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='background-color: #f2f2f2;width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("  <h4 style='margin: 0px; color: #000000; line-height: 140%; text-align: left; word-wrap: break-word; font-weight: normal; font-family: arial,helvetica,sans-serif; font-size: 12px;'>");
            sb.Append($"    <strong>{Localization.Resources.Pedidos.RetiradaProduto.InformacoesClienteHTML}:</strong>");
            sb.Append("  </h4>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
            sb.Append("    </div>");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
            sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
            sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
            sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
            sb.Append("      ");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='500' style='width: 500px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("  <div style='color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>");
            sb.Append("    <p style='font-size: 14px; line-height: 140%;'>");
            sb.Append($"        {Localization.Resources.Pedidos.RetiradaProduto.Nome}: {destinatario.Nome}");
            sb.Append($"        <br /> SAP: {destinatario.CodigoIntegracao}");

            if (configuracaoPedido.ModeloEmailAgendamentoPedido == ModeloEmailAgendamentoPedido.Modelo2)
                sb.Append($"        <br /> {Localization.Resources.Pedidos.RetiradaProduto.Destino}: {destinatario?.Localidade?.DescricaoCidadeEstado ?? string.Empty}");

            sb.Append("     </p>");
            sb.Append("  </div>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
            sb.Append("    </div>");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
            sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
            sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
            sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
            sb.Append("      ");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='500' style='background-color: #f2f2f2;width: 500px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='background-color: #f2f2f2;width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("  <h4 style='margin: 0px; color: #000000; line-height: 140%; text-align: left; word-wrap: break-word; font-weight: normal; font-family: arial,helvetica,sans-serif; font-size: 12px;'>");
            sb.Append($"    <strong>{Localization.Resources.Pedidos.RetiradaProduto.InformacoesTransportadoraHTML}:</strong>");
            sb.Append("  </h4>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
            sb.Append("    </div>");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
            sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
            sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
            sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
            sb.Append("      ");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='500' style='width: 500px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("  <div style='color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>");
            sb.Append($@"    <p style='font-size: 14px; line-height: 140%;'>{Localization.Resources.Pedidos.RetiradaProduto.Nome}: {ObterNomeTransportadora(carregamento)}</p>");
            sb.Append($@"<p style='font-size: 14px; line-height: 140%;'>{Localization.Resources.Pedidos.RetiradaProduto.TipoCaminhaoHTML}: {carregamento.ModeloVeicularCarga.Descricao}<br />{Localization.Resources.Pedidos.RetiradaProduto.Placa}: {carregamento.PlacaVeiculo}");
            sb.Append($@"  <br />{Localization.Resources.Pedidos.RetiradaProduto.Motorista}: {motorista.Nome}<br />{Localization.Resources.Pedidos.RetiradaProduto.Documento}: {motorista.CPF_Formatado}</p>");
            sb.Append("  </div>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
            sb.Append("    </div>");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
            sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
            sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
            sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
            sb.Append("      ");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='500' style='background-color: #f2f2f2;width: 500px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='background-color: #f2f2f2;width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("  <h4 style='margin: 0px; color: #000000; line-height: 140%; text-align: left; word-wrap: break-word; font-weight: normal; font-family: arial,helvetica,sans-serif; font-size: 12px;'>");
            sb.Append($"    <strong>{Localization.Resources.Pedidos.RetiradaProduto.Pedidos}</strong>");
            sb.Append("  </h4>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
            sb.Append("    </div>");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
            sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
            sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
            sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
            sb.Append("      ");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='250' style='width: 250px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-50' style='max-width: 320px;min-width: 250px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("  <div style='color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>");
            sb.Append($"    <p style='font-size: 14px; line-height: 140%;'><strong>{Localization.Resources.Pedidos.RetiradaProduto.Pedido}</strong></p>");
            sb.Append("  </div>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='125' style='width: 125px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-25' style='max-width: 160px;min-width: 125px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("  <div style='color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>");
            sb.Append("    <p style='font-size: 14px; line-height: 140%;'><strong>Peso</strong></p>");
            sb.Append("  </div>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");

            if (configuracaoPedido.ModeloEmailAgendamentoPedido == ModeloEmailAgendamentoPedido.Modelo2)
            {
                sb.Append("<!--[if (mso)|(IE)]><td align='center' width='125' style='width: 125px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
                sb.Append("<div class='u-col u-col-25' style='max-width: 160px;min-width: 125px;display: table-cell;vertical-align: top;'>");
                sb.Append("  <div style='width: 100% !important;'>");
                sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
                sb.Append("  ");
                sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
                sb.Append("  <tbody>");
                sb.Append("    <tr>");
                sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
                sb.Append("        ");
                sb.Append("  <div style='color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>");
                sb.Append("    <p style='font-size: 14px; line-height: 140%;'><strong>M²</strong></p>");
                sb.Append("  </div>");
                sb.Append("      </td>");
                sb.Append("    </tr>");
                sb.Append("  </tbody>");
                sb.Append("</table>");
                sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
                sb.Append("  </div>");
                sb.Append("</div>");
                sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            }

            sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
            sb.Append("    </div>");
            sb.Append("  </div>");
            sb.Append("</div>");

            foreach (var pedido in carregamentoPedidos)
            {
                sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
                sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
                sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
                sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
                sb.Append("      ");
                sb.Append("<!--[if (mso)|(IE)]><td align='center' width='250' style='width: 250px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
                sb.Append("<div class='u-col u-col-50' style='max-width: 320px;min-width: 250px;display: table-cell;vertical-align: top;'>");
                sb.Append("  <div style='width: 100% !important;'>");
                sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
                sb.Append("  ");
                sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
                sb.Append("  <tbody>");
                sb.Append("    <tr>");
                sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
                sb.Append("        ");
                sb.Append("  <div style='color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>");
                sb.Append($"    <p style='font-size: 14px; line-height: 140%;'>{pedido.Pedido.NumeroPedidoEmbarcador}</p>");
                sb.Append("  </div>");
                sb.Append("      </td>");
                sb.Append("    </tr>");
                sb.Append("  </tbody>");
                sb.Append("</table>");
                sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
                sb.Append("  </div>");
                sb.Append("</div>");
                sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
                sb.Append("<!--[if (mso)|(IE)]><td align='center' width='125' style='width: 125px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
                sb.Append("<div class='u-col u-col-25' style='max-width: 160px;min-width: 125px;display: table-cell;vertical-align: top;'>");
                sb.Append("  <div style='width: 100% !important;'>");
                sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
                sb.Append("  ");
                sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
                sb.Append("  <tbody>");
                sb.Append("    <tr>");
                sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
                sb.Append("        ");
                sb.Append("  <div style='color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>");
                sb.Append($"    <p style='font-size: 14px; line-height: 140%;'>{pedido.Peso.ToString("n2")}</p>");
                sb.Append("  </div>");
                sb.Append("      </td>");
                sb.Append("    </tr>");
                sb.Append("  </tbody>");
                sb.Append("</table>");
                sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
                sb.Append("  </div>");
                sb.Append("</div>");
                sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");

                if (configuracaoPedido.ModeloEmailAgendamentoPedido == ModeloEmailAgendamentoPedido.Modelo2)
                {
                    sb.Append("<!--[if (mso)|(IE)]><td align='center' width='125' style='width: 125px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
                    sb.Append("<div class='u-col u-col-25' style='max-width: 160px;min-width: 125px;display: table-cell;vertical-align: top;'>");
                    sb.Append("  <div style='width: 100% !important;'>");
                    sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
                    sb.Append("  ");
                    sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
                    sb.Append("  <tbody>");
                    sb.Append("    <tr>");
                    sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
                    sb.Append("        ");
                    sb.Append("  <div style='color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>");
                    sb.Append($"    <p style='font-size: 14px; line-height: 140%;'>{pedido.Pedido.CubagemTotal.ToString("n2")}</p>");
                    sb.Append("  </div>");
                    sb.Append("      </td>");
                    sb.Append("    </tr>");
                    sb.Append("  </tbody>");
                    sb.Append("</table>");
                    sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
                    sb.Append("  </div>");
                    sb.Append("</div>");
                    sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
                }

                sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
                sb.Append("    </div>");
                sb.Append("  </div>");
                sb.Append("</div>");
            }

            // Inicio totalizadores
            decimal totalPesoPedido = carregamentoPedidos.Sum(o => o.Peso);
            decimal totalCubagemPedido = carregamentoPedidos.Sum(p => p.Pedido.CubagemTotal);

            sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
            sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
            sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
            sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
            sb.Append("      ");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='250' style='width: 250px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-50' style='max-width: 320px;min-width: 250px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='right'>");
            sb.Append("        ");
            sb.Append("  <div style='color: #000000; line-height: 140%; text-align: right; word-wrap: break-word;'>");
            sb.Append($"    <p style='font-size: 14px; line-height: 140%;'><strong>Total</strong></p>");
            sb.Append("  </div>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='125' style='width: 125px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-25' style='max-width: 160px;min-width: 125px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif; border-top: 1px solid;' align='left'>");
            sb.Append("        ");
            sb.Append("  <div style='color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>");
            sb.Append($"    <p style='font-size: 14px; line-height: 140%;'><strong>{totalPesoPedido.ToString("n2")}</strong></p>");
            sb.Append("  </div>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");

            if (configuracaoPedido.ModeloEmailAgendamentoPedido == ModeloEmailAgendamentoPedido.Modelo2)
            {
                sb.Append("<!--[if (mso)|(IE)]><td align='center' width='125' style='width: 125px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
                sb.Append("<div class='u-col u-col-25' style='max-width: 160px;min-width: 125px;display: table-cell;vertical-align: top;'>");
                sb.Append("  <div style='width: 100% !important;'>");
                sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
                sb.Append("  ");
                sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
                sb.Append("  <tbody>");
                sb.Append("    <tr>");
                sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif; border-top: 1px solid;' align='left'>");
                sb.Append("        ");
                sb.Append("  <div style='color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>");
                sb.Append($"    <p style='font-size: 14px; line-height: 140%;'><strong>{totalCubagemPedido.ToString("n2")}</strong></p>");
                sb.Append("  </div>");
                sb.Append("      </td>");
                sb.Append("    </tr>");
                sb.Append("  </tbody>");
                sb.Append("</table>");
                sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
                sb.Append("  </div>");
                sb.Append("</div>");
                sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            }

            sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
            sb.Append("    </div>");
            sb.Append("  </div>");
            sb.Append("</div>");
            // Fim totalizadores


            sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
            sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
            sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
            sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
            sb.Append("      ");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='500' style='width: 500px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='width: 100% !important;background-color: #f2f2f2;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("  <h4 style='margin: 0px; color: #000000; line-height: 140%; text-align: left; word-wrap: break-word; font-weight: normal; font-family: arial,helvetica,sans-serif; font-size: 12px;'>");
            sb.Append($"    <strong>{Localization.Resources.Pedidos.RetiradaProduto.ObservacoesHTML}:</strong>");
            sb.Append("  </h4>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
            sb.Append("    </div>");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<div class='u-row-container' style='padding: 0px;background-color: transparent'>");
            sb.Append("  <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>");
            sb.Append("    <div style='border-collapse: collapse;display: table;width: 100%;background-color: transparent;'>");
            sb.Append("      <!--[if (mso)|(IE)]><table width='100%' cellpadding='0' cellspacing='0' border='0'><tr><td style='padding: 0px;background-color: transparent;' align='center'><table cellpadding='0' cellspacing='0' border='0' style='width:500px;'><tr style='background-color: transparent;'><![endif]-->");
            sb.Append("      ");
            sb.Append("<!--[if (mso)|(IE)]><td align='center' width='500' style='width: 500px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;' valign='top'><![endif]-->");
            sb.Append("<div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>");
            sb.Append("  <div style='width: 100% !important;'>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--><div style='padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'><!--<![endif]-->");
            sb.Append("  ");
            sb.Append("<table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>");
            sb.Append("  <tbody>");
            sb.Append("    <tr>");
            sb.Append("      <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>");
            sb.Append("        ");
            sb.Append("  <div style='color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>");
            sb.Append($"    <p style='font-size: 14px; line-height: 140%; text-align: left;'>{ObservacaoHTML}.</p>");
            sb.Append("  </div>");
            sb.Append("      </td>");
            sb.Append("    </tr>");
            sb.Append("  </tbody>");
            sb.Append("</table>");
            sb.Append("  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("<!--[if (mso)|(IE)]></td><![endif]-->");
            sb.Append("      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->");
            sb.Append("    </div>");
            sb.Append("  </div>");
            sb.Append("</div>");
            sb.Append("    <!--[if (mso)|(IE)]></td></tr></table><![endif]-->");
            sb.Append("    </td>");
            sb.Append("  </tr>");
            sb.Append("  </tbody>");
            sb.Append("  </table>");
            sb.Append("  <!--[if mso]></div><![endif]-->");
            sb.Append("  <!--[if IE]></div><![endif]-->");

            sb.Append("</body>");
            sb.Append("</html>");

            return sb.ToString();
        }

        private string ObterEnderecoLogoCliente(TipoServicoMultisoftware tipoServicoMultisoftware, int codigoCliente, string stringConexaoAdmin)
        {
            AdminMultisoftware.Repositorio.UnitOfWork adminMultisoftwareUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(stringConexaoAdmin);

            try
            {
                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(adminMultisoftwareUnitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorClienteETipo(codigoCliente, tipoServicoMultisoftware);

                return clienteURLAcesso.Cliente.Logo;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return "";
            }
            finally
            {
                adminMultisoftwareUnitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamento ObterCentroCarregamentoPorVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorVeiculo(codigoCentroCarregamento: 0, codigoVeiculo: veiculo.Codigo);

            if ((centroCarregamento == null) && (veiculo.Empresa != null))
                centroCarregamento = ObterCentroCarregamentoPorFilialContrato(veiculo.Empresa.Codigo);

            return centroCarregamento;
        }

        public void EnviarNotificacaoConfirmacaoCarga(int codigoCarregamento, TipoServicoMultisoftware tipoServicoMultisoftware, int codigoCliente, string stringConexaoAdmin, Repositorio.UnitOfWork unitOfWork)
        {
            //#if DEBUG
            //            return;
            //#endif

            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repCarregamento.BuscarPorCodigoComFetch(codigoCarregamento);

            try
            {
                string subject = $" {Localization.Resources.Pedidos.RetiradaProduto.AssuntoNotificacaoConfirmacaoCarga}";
                string body = RenderizarEmail(carregamento, unitOfWork, tipoServicoMultisoftware, codigoCliente, stringConexaoAdmin);
                string emails = "";

                if (!string.IsNullOrEmpty(carregamento.EmailNotificacao))
                    emails = carregamento.EmailNotificacao;

                string[] splitEmail = emails.Split(';');
                string email = carregamento?.UsuarioAgendamento?.Email;
                List<string> cc = new List<string>();

                if (splitEmail.Length > 1)
                {
                    for (int i = 1; i < splitEmail.Length; i++)
                        cc.Add(splitEmail[i]);
                }
                else
                    cc.Add(carregamento.EmailNotificacao);

                if (!Servicos.Email.EnviarEmailAutenticado(email, subject, body, unitOfWork, out string msg, "", null, null, cc.ToArray()))
                    Servicos.Log.TratarErro("Falha ao enviar notificação de confirmação de agendamento: " + msg);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        public string ObterNomeTransportadora(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            if (!string.IsNullOrWhiteSpace(carregamento.NomeTransportadora))
                return carregamento.NomeTransportadora;

            return carregamento.Empresa?.RazaoSocial ?? "";
        }

        #endregion
    }
}
