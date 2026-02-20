using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]

    public class Financeiro : LongRunningProcessBase<Financeiro>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            BloquearFinanceiroCliente(unitOfWork);
            new Servicos.Embarcador.Integracao.IntegracaoTitulo(unitOfWork).VerificarIntegracoesPendentesTitulo();
            new Servicos.Embarcador.Integracao.IntegracaoDocumentoEntrada(unitOfWork).VerificarIntegracoesPendentesDocumentoEntrada();
        }

        public override bool CanRun()
        {
            return _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS;
        }

        private void BloquearFinanceiroCliente(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro repConfiguracaoBloqueioFinanceiro = new Repositorio.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);

            if (repConfiguracaoBloqueioFinanceiro.ContemConfiguracaoAtiva())
            {
                List<int> codigosGrupoPessoa = repConfiguracaoBloqueioFinanceiro.BuscarCodigoGrupoPessoa();
                if (codigosGrupoPessoa != null && codigosGrupoPessoa.Count > 0)
                {
                    foreach (var codigoGrupoPessoa in codigosGrupoPessoa)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro configuracaoBloqueioFinanceiro = repConfiguracaoBloqueioFinanceiro.BuscarConfiguracaoBloqueioPadrao(codigoGrupoPessoa);

                        DateTime dataVencimento = DateTime.Now.Date.AddDays(-configuracaoBloqueioFinanceiro.QuantidadeDiasAtrasoPagamento);

                        List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulosEmAtraso = repTitulo.TitulosAReceberEmAtraso(dataVencimento, codigoGrupoPessoa);
                        if (titulosEmAtraso != null && titulosEmAtraso.Count > 0)
                        {
                            List<Dominio.Entidades.Cliente> clientes = titulosEmAtraso.Select(o => o.Pessoa).Distinct().ToList();
                            List<Dominio.Entidades.Cliente> clientesBloqueados = new List<Dominio.Entidades.Cliente>();

                            foreach (Dominio.Entidades.Cliente cliente in clientes)
                            {
                                if (!cliente.SituacaoFinanceira.HasValue || cliente.SituacaoFinanceira == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFinanceira.Liberada)
                                {
                                    if (cliente.DataAlteracaoSituacaoFinanceira.HasValue && cliente.DataAlteracaoSituacaoFinanceira.Value.AddDays(configuracaoBloqueioFinanceiro.QuantidadeDiasNovoBloqueio) > DateTime.Now)
                                        continue;

                                    List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulosDoCliente = titulosEmAtraso.Where(o => o.Pessoa.CPF_CNPJ == cliente.CPF_CNPJ).ToList();
                                    if (cliente.TitulosBloqueioFinanceiro == null)
                                        cliente.TitulosBloqueioFinanceiro = new List<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
                                    else
                                        cliente.TitulosBloqueioFinanceiro.Clear();

                                    foreach (Dominio.Entidades.Embarcador.Financeiro.Titulo titulo in titulosDoCliente)
                                        cliente.TitulosBloqueioFinanceiro.Add(titulo);

                                    cliente.SituacaoFinanceira = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFinanceira.Bloqueada;
                                    cliente.DataUltimaAtualizacao = DateTime.Now;
                                    cliente.Integrado = false;
                                    repCliente.Atualizar(cliente);

                                    if (cliente.GrupoPessoas != null)
                                    {
                                        cliente.GrupoPessoas.SituacaoFinanceira = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFinanceira.Bloqueada;
                                        repGrupoPessoas.Atualizar(cliente.GrupoPessoas);
                                    }

                                    clientesBloqueados.Add(cliente);
                                }
                            }

                            if (clientesBloqueados.Count > 0)
                                EnviarEmailClientesBloqueados(clientesBloqueados, unidadeDeTrabalho);
                        }
                    }
                }
            }
        }

        private void EnviarEmailClientesBloqueados(List<Dominio.Entidades.Cliente> clientesBloqueados, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            if (email == null)
                return;

            foreach (Dominio.Entidades.Cliente cliente in clientesBloqueados)
            {
                List<string> emails = new List<string>();

                string assunto = "Bloqueio de Financeiro";
                string mensagemEmail = "Olá<br/><br/>Você foi bloqueado automaticamente devido ter títulos vencidos.";
                mensagemEmail += "<br/><br/>E-mail enviado automaticamente. Por favor, não responda.";

                if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                    mensagemEmail += "<br/>" + "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");

                string mensagemErro = "Erro ao enviar e-mail";

                //E-mails
                if (!string.IsNullOrWhiteSpace(cliente.Email))
                    emails.AddRange(cliente.Email.Split(';').ToList());

                for (int a = 0; a < cliente.Emails.Count; a++)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail outroEmail = cliente.Emails[a];
                    if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A")
                        emails.Add(outroEmail.Email);
                }

                emails = emails.Distinct().ToList();
                if (emails.Count > 0)
                {
                    Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails.Count == 1 ? emails[0] : null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail,
                        null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeDeTrabalho);
                }
            }
        }
    }
}