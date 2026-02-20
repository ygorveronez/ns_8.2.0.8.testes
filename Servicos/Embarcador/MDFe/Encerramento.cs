using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Servicos.Embarcador.MDFe
{
    public class Encerramento
    {
        public static void EnviarEmailsEncerramentoTransportadores(int codigoClienteAdminMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Repositorio.UnitOfWork unidadeTrabalhoAdminMultisoftware, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unidadeTrabalhoAdminMultisoftware);

            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorClienteETipoProducao(codigoClienteAdminMultisoftware, tipoServico);

            List<int> codigosMDFes = repMDFe.BuscarCodigosParaNotificacao(DateTime.Now.AddDays(-1).Date, DateTime.Now.AddDays(-1).Date);

            foreach (int codigoMDFe in codigosMDFes)
                EnviarEmailEncerramentoTransportador(codigoMDFe, clienteURLAcesso, unidadeDeTrabalho);
        }

        public static void EnviarEmailEncerramentoTransportador(int codigoMDFe, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            if (email == null)
                return;

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

            if (mdfe == null ||
                mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado ||
                mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento ||
                mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado ||
                string.IsNullOrWhiteSpace(mdfe.Empresa.Email))
                return;

            string placa = mdfe.Veiculos.FirstOrDefault()?.Placa ?? string.Empty;
            string urlAcesso = clienteURLAcesso.URLAcesso;

            if (urlAcesso.Contains("192.168.0.125"))
                urlAcesso += "/Embarcador";

            System.Text.StringBuilder stBuilder = new StringBuilder();

            stBuilder.Append("Olá " + mdfe.Empresa.RazaoSocial + ",")
                     .AppendLine()
                     .AppendLine()
                     .Append("O MDF-e número ")
                     .Append(mdfe.Numero)
                     .Append(" do veículo placa ")
                     .Append(placa)
                     .Append(" ainda não foi encerrado.")
                     .AppendLine()
                     .Append("Para encerrar o MDF-e acesse o link: http://" + urlAcesso + "/MDFe/EncerramentoTransportador?x=")
                     .Append(HttpUtility.UrlEncode(Servicos.Criptografia.Criptografar(mdfe.Empresa.CNPJ + mdfe.Codigo.ToString(), "Encerramento##@")))
                     .AppendLine()
                     .AppendLine()
                     .Append("E-mail enviado automaticamente, favor não responder.");

            string titulo = "O MDF-e " + mdfe.Numero + " do veículo " + placa + " está aguardando o encerramento.";

            StringBuilder rodape = new StringBuilder();
            rodape.Append("Atenciosamente,")
                  .AppendLine()
                  .Append("MultiCTe");

            string erro = string.Empty;

            List<string> emails = mdfe.Empresa.Email.Split(';').ToList();

#if DEBUG
            emails = new List<string>() { "willian@multisoftware.com.br" };
#endif

            if (!Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, titulo, stBuilder.ToString(), email.Smtp, out erro, email.DisplayEmail, null, rodape.ToString(), email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeDeTrabalho))
                Servicos.Log.TratarErro(erro);

            mdfe.DataEnvioNotificacao = DateTime.Now.Date;

            repMDFe.Atualizar(mdfe);
        }
    }
}
