using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.PagamentoMotorista
{
    public class IntegracaoEmailPagamentoMotorista : ServicoBase
    {        
        public IntegracaoEmailPagamentoMotorista(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public bool EnviarEmailPagamentoMotorista(int codigoPagamento, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento = repPagamentoMotorista.BuscarPorCodigo(codigoPagamento);
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio = repPagamentoMotoristaIntegracaoEnvio.BuscarPorPagamento(codigoPagamento);

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            var assuntoEmail = substituirTags(pagamento.PagamentoMotoristaTipo.AssuntoEmail, pagamento);
            var corpoEmail = substituirTags(pagamento.PagamentoMotoristaTipo.CorpoEmail, pagamento);

            corpoEmail += "<br/><br/>By Multisoftware";

            string mensagemErro = "Erro ao enviar e-mail";
            List<string> emails = new List<string>();
            if (pagamento.PagamentoMotoristaTipo?.Pessoa?.Email != null && pagamento.PagamentoMotoristaTipo?.Pessoa?.Email != "")
            {
                emails.Add(pagamento.PagamentoMotoristaTipo?.Pessoa?.Email);
            }

            if (emails.Count > 0)
            {
                bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails.Count == 1 ? emails[0] : null, emails.ToArray(), null, assuntoEmail, corpoEmail, email.Smtp, out mensagemErro, email.DisplayEmail,
                    null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeDeTrabalho);

                if (!sucesso)
                    pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                else
                {
                    pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                }

                repPagamentoMotoristaIntegracaoEnvio.Atualizar(pagamentoEnvio);
            }
            else
            {
                pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoEnvio.Retorno = "Nenhum e-mail encontrado para envio da integração!";
                repPagamentoMotoristaIntegracaoEnvio.Atualizar(pagamentoEnvio);
            }

            return true;
        }

        private string substituirTags(string texto, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento)
        {
            texto = texto.Replace("#Fornecedor#", pagamento.PagamentoMotoristaTipo?.Pessoa?.Nome ?? "");
            texto = texto.Replace("#Valor#", pagamento.Valor.ToString() ?? "");
            texto = texto.Replace("#Motorista#", pagamento.Motorista?.Nome ?? "");
            texto = texto.Replace("#Usuario#", pagamento.Usuario?.Nome ?? "");
            texto = texto.Replace("#Numero#", pagamento.Numero.ToString() ?? "");
            texto = texto.Replace("#QuebraLinha#", "<br/>");

            return texto;
        }

    }
}
