using AdminMultisoftware.Repositorio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 43200000)]

    public class EnvioEmailAvisoVencimentoCobranca : LongRunningProcessBase<EnvioEmailAvisoVencimentoCobranca>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificaAvisoVencimentosPendenteEnvio(unitOfWork);
            VerificaCobrancaPendenteEnvio(unitOfWork);
        }

        private void VerificaCobrancaPendenteEnvio(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca repositorioConfiguracaoEnvioEmailCobranca = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca configuracaoEnvioEmailCobranca = repositorioConfiguracaoEnvioEmailCobranca.BuscarConfiguracaoPadrao();
            if (configuracaoEnvioEmailCobranca.CobrancaEnvarEmail && !string.IsNullOrWhiteSpace(configuracaoEnvioEmailCobranca.CobrancaMensagem))
            {
                IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.CobrancaPendenteEnviarEmail> cobrancaPendentesEnvio = repTitulo.BuscarCobrancaPendenteEnvio();
                foreach (var cobranca in cobrancaPendentesEnvio)
                {
                    unitOfWork.Start();
                    try
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(cobranca.Codigo);
                        List<string> emails = obterEmails(titulo);
                        if (emails.Count > 0)
                        {
                            bool enviou = EnviarEmail(titulo, configuracaoEnvioEmailCobranca.CobrancaAssunto, configuracaoEnvioEmailCobranca.CobrancaMensagem, emails, unitOfWork);
                            if (enviou)
                            {                               
                                titulo.DataEnvioEmailCobranca = DateTime.Now;
                                repTitulo.Atualizar(titulo);
                                Servicos.Auditoria.Auditoria.Auditar(_auditado, titulo, null, "Enviou de e-mail de cobrança automaticamente.", unitOfWork);
                               
                            }
                        }
                        else {
                            titulo.DataEnvioEmailCobranca = DateTime.Now;
                            repTitulo.Atualizar(titulo);
                            Servicos.Auditoria.Auditoria.Auditar(_auditado, titulo, null, "E-mail de cobrança não enviado, pois o cliente não possui nenhum e-mail cadastrado.", unitOfWork);
                        }
                        unitOfWork.CommitChanges();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        unitOfWork.Rollback();
                    }
                    unitOfWork.FlushAndClear();
                }
            }
        }

        private void VerificaAvisoVencimentosPendenteEnvio(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca repositorioConfiguracaoEnvioEmailCobranca = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);


            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca configuracaoEnvioEmailCobranca = repositorioConfiguracaoEnvioEmailCobranca.BuscarConfiguracaoPadrao();

            if (configuracaoEnvioEmailCobranca.AvisoVencimetoEnvarEmail && !string.IsNullOrWhiteSpace(configuracaoEnvioEmailCobranca.AvisoVencimetoMensagem)) {
                IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.CobrancaPendenteEnviarEmail> avisoPendentesEnvio = repTitulo.BuscarAvisoVencimentoPendenteEnvio();
                if (avisoPendentesEnvio != null && avisoPendentesEnvio.Count > 0)
                {
                    foreach (var avisoVencimento in avisoPendentesEnvio)
                    {
                        try
                        {
                            unitOfWork.Start();
                            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(avisoVencimento.Codigo);
                            List<string> emails = obterEmails(titulo);
                            if (emails.Count > 0)
                            {
                                bool enviou = EnviarEmail(titulo, configuracaoEnvioEmailCobranca.AvisoVencimetoAssunto, configuracaoEnvioEmailCobranca.AvisoVencimetoMensagem, emails, unitOfWork);
                                if (enviou)
                                {

                                    titulo.DataEnvioEmailAvisoVencimento = DateTime.Now;
                                    repTitulo.Atualizar(titulo);
                                    Servicos.Auditoria.Auditoria.Auditar(_auditado, titulo, null, "Enviou de e-mail de aviso de vencimento automaticamente.", unitOfWork);

                                }
                            }
                            else {
                                titulo.DataEnvioEmailAvisoVencimento = DateTime.Now;
                                repTitulo.Atualizar(titulo);
                                Servicos.Auditoria.Auditoria.Auditar(_auditado, titulo, null, "E-mail de aviso de vencimento não enviado, pois o cliente não possui nenhum e-mail cadastrado.", unitOfWork);
                            }
                            unitOfWork.CommitChanges();
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            Servicos.Log.TratarErro(ex);                            
                        }                     

                        unitOfWork.FlushAndClear();
                    }
                }
            }

        }

        private bool EnviarEmail(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, String assunto, String mensaem, List<string> emails, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();


            if (email != null && titulo != null)
            {               

                if (string.IsNullOrWhiteSpace(assunto))
                {
                    assunto = "Aviso de vencimento";
                }
                if (!string.IsNullOrWhiteSpace(assunto))
                {
                    assunto = assunto.Replace("#CodigoTitulo", Utilidades.String.OnlyNumbers(titulo?.Codigo.ToString() ?? ""));
                    assunto = assunto.Replace("#DataVencimento", titulo?.DataVencimento?.ToString("dd/MM/yyyy") ?? "");
                    assunto = assunto.Replace("#DataEmissao", titulo?.DataEmissao?.ToString("dd/MM/yyyy") ?? "");
                    assunto = assunto.Replace("#Fatura", Utilidades.String.OnlyNumbers(titulo.FaturaParcela?.Fatura?.Numero.ToString() ?? ""));
                    assunto = assunto.Replace("#EmpresaRazao", titulo?.Empresa?.RazaoSocial ?? "");
                    assunto = assunto.Replace("#EmpresaCNPJ", titulo?.Empresa?.CNPJ_Formatado ?? "");
                    assunto = assunto.Replace("#PessoaRazao", titulo?.Pessoa?.Nome ?? "");
                    assunto = assunto.Replace("#PessoaCNPJ", titulo?.Pessoa?.CPF_CNPJ_Formatado ?? "");
                    assunto = assunto.Replace("#Documentos", string.Join(", ", titulo.Documentos.Select(f => f.NumeroDocumento).Distinct().ToList()));
                }
                if (!string.IsNullOrWhiteSpace(mensaem))
                {
                    mensaem = mensaem.Replace("#CodigoTitulo", Utilidades.String.OnlyNumbers(titulo?.Codigo.ToString() ?? ""));
                    mensaem = mensaem.Replace("#DataVencimento", titulo?.DataVencimento?.ToString("dd/MM/yyyy") ?? "");
                    mensaem = mensaem.Replace("#DataEmissao", titulo?.DataEmissao?.ToString("dd/MM/yyyy") ?? "");
                    mensaem = mensaem.Replace("#Fatura", Utilidades.String.OnlyNumbers(titulo.FaturaParcela?.Fatura?.Numero.ToString() ?? ""));
                    mensaem = mensaem.Replace("#Situacao", titulo?.StatusTitulo.ObterDescricao() ?? "");
                    mensaem = mensaem.Replace("#ValorOriginal", $"R$ {titulo.ValorOriginal:N2}" ?? "");
                    mensaem = mensaem.Replace("#ValorPendente", $"R$ {titulo.ValorPendente:N2}" ?? "");
                    mensaem = mensaem.Replace("#ValorPago", $"R$ {titulo.ValorPago:N2}" ?? "");
                    mensaem = mensaem.Replace("#Desconto", $"R$ {titulo.Desconto:N2}" ?? "");
                    mensaem = mensaem.Replace("#Acrescimo", $"R$ {titulo.Acrescimo:N2}" ?? "");
                    mensaem = mensaem.Replace("#Saldo", "R$" + (titulo.StatusTitulo == StatusTitulo.Quitada ? 0.ToString("n2") : (titulo.ValorOriginal - titulo.Desconto + titulo.Acrescimo).ToString("n2")));
                    mensaem = mensaem.Replace("#EmpresaRazao", titulo?.Empresa?.RazaoSocial ?? "");
                    mensaem = mensaem.Replace("#EmpresaCNPJ", titulo?.Empresa?.CNPJ_Formatado ?? "");
                    mensaem = mensaem.Replace("#PessoaRazao", titulo?.Pessoa?.Nome ?? "");
                    mensaem = mensaem.Replace("#PessoaCNPJ", titulo?.Pessoa?.CPF_CNPJ_Formatado ?? "");
                    mensaem = mensaem.Replace("#FormaTitulo", titulo?.FormaTitulo.ObterDescricao() ?? "");
                    mensaem = mensaem.Replace("#Documentos", string.Join(", ", titulo.Documentos.Select(f => f.NumeroDocumento).Distinct().ToList()));
                    mensaem = mensaem.Replace("#ObservacaoFatura", titulo.Observacao);
                    mensaem = mensaem.Replace("#QuebraLinha", "<br/>");

                    string tabelaCorpo = "";
                    if (mensaem.Contains("#Tabela"))
                    {
                        tabelaCorpo = "<script src='https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js'></script>";
                        tabelaCorpo += "<script src='http://www.developerdan.com/table-to-json/javascripts/jquery.tabletojson.min.js'></script>";
                        tabelaCorpo += "<table style='width:100%; align='center'; border='1';>";
                        tabelaCorpo += "<tr>";
                        tabelaCorpo += "<th>Documento</th>";
                        tabelaCorpo += "<th>Valor Documento</th>";

                        tabelaCorpo += "<th>Fatura</th>";
                        tabelaCorpo += "</tr>";

                        foreach (var documento in titulo.Documentos)
                        {
                            tabelaCorpo += "<tr>";
                            tabelaCorpo += "<td>" + documento.NumeroDocumento + "</td>";
                            tabelaCorpo += "<td>" + (documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.CTe ? documento.CTe.ValorAReceber : documento.Carga.ValorFreteAPagar).ToString("n2");
                            tabelaCorpo += "<td>" + Utilidades.String.OnlyNumbers(titulo.FaturaParcela?.Fatura?.Numero.ToString() ?? "") + "</td>";
                            tabelaCorpo += "</tr>";
                        }
                        tabelaCorpo += "</table>";
                    }

                    mensaem = mensaem.Replace("#Tabela", tabelaCorpo);


                }

                mensaem += "<br/><br/>By Multisoftware";
                string mensagemErro = "Erro ao enviar e-mail";               
                if (emails.Count > 0)
                {
                    bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails.Count == 1 ? emails[0] : null, emails.ToArray(), null, assunto, mensaem, email.Smtp, out mensagemErro, email.DisplayEmail,
                      null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork);

                    return sucesso;
                }
                else if (emails.Count == 0) {
                    return true;
                }                
            }
            return false;
        }

        private List<string> obterEmails(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo)
        {
            List<string> emails = new List<string>();
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail> tiposPermitidos = new List<TipoEmail> {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Financeiro
            };
            if (titulo.Pessoa != null)
            {
                if (titulo.Pessoa.Emails.Count > 0)
                {
                    foreach (var emailCliente in titulo.Pessoa.Emails)
                    {
                        if (tiposPermitidos.Contains(emailCliente.TipoEmail))
                        {
                            emails.Add(emailCliente.Email);
                        }
                    }
                }

                if (titulo.Pessoa.Email != null && tiposPermitidos.Contains(titulo.Pessoa.TipoEmail))
                {
                    emails.Add(titulo.Pessoa.Email);
                }
            }
            return emails.Distinct().ToList();
        }
    }
}