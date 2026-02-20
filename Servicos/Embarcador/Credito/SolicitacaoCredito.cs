using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Credito
{
    public class SolicitacaoCredito : ServicoBase
    {        
        public SolicitacaoCredito(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public void ExtornarSolicitacaoCredito(Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito solicitacaoCredito, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Notificacao.Notificacao(StringConexao, null, tipoServicoMultisoftware, "");

            Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);
            Repositorio.Embarcador.Creditos.SolicitacaoCredito repSolicitacaoCredito = new Repositorio.Embarcador.Creditos.SolicitacaoCredito(unitOfWork);
            Servicos.Embarcador.Credito.CreditoMovimentacao serCreditoMovimentacao = new Credito.CreditoMovimentacao(unitOfWork);

            solicitacaoCredito.SituacaoSolicitacaoCredito = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Estornado;
            repSolicitacaoCredito.Atualizar(solicitacaoCredito);

            string nota = "";
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                nota = string.Format(Localization.Resources.Credito.SolicitacaoCredito.SolicitacaoComplementoCargaCancelada, solicitacaoCredito.Carga.CodigoCargaEmbarcador);
            else
                nota = string.Format(Localization.Resources.Credito.SolicitacaoCredito.CreditoLiberadoParaCargaExtornado, solicitacaoCredito.ValorLiberado.ToString("n2"), solicitacaoCredito.Solicitante.Nome, solicitacaoCredito.Carga.CodigoCargaEmbarcador);

            serNotificacao.GerarNotificacao(solicitacaoCredito.Solicitado, solicitacaoCredito.Solicitante, solicitacaoCredito.Codigo, "Creditos/CreditoLiberacao", nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.estornado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
            
            List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> creditosUtilizadosDestino = repCreditoDisponivelUtilizado.BuscarPorCreditoSolicitacaoCredito(solicitacaoCredito.Codigo);
            serCreditoMovimentacao.ExtornarCreditos(creditosUtilizadosDestino, tipoServicoMultisoftware, unitOfWork);   
        }

        public Dominio.ObjetosDeValor.Embarcador.Creditos.SolicitacaoCreditoGerada GerarSolicitacaoCredito(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario solicitante, Dominio.Entidades.Usuario solicitado, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, decimal valorSolicitacao, string motivo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Notificacao.Notificacao(StringConexao, null, tipoServicoMultisoftware, "");

            Servicos.Embarcador.Credito.ControleSaldo serControleSaldo = new ControleSaldo(unitOfWork);
            var retornoSaldo =  serControleSaldo.BuscarInformacoesSaldoCredito(solicitante, unitOfWork);

            Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);
            Repositorio.Embarcador.Creditos.SolicitacaoCredito repSolicitacaoCredito = new Repositorio.Embarcador.Creditos.SolicitacaoCredito(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Creditos.SolicitacaoCreditoGerada solicitacaoCreditoGerada = new Dominio.ObjetosDeValor.Embarcador.Creditos.SolicitacaoCreditoGerada();
            if (solicitado != null)
            {
                if (!string.IsNullOrWhiteSpace(motivo))
                {
                    if (valorSolicitacao > 0)
                    {

                        if (retornoSaldo.Saldo <= 0)
                        {
                            Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito hierarquia = repHierarquiaSolicitacaoCredito.BuscarPorCreditorRecebedor(solicitado.Codigo, solicitante.Codigo);

                            if (hierarquia != null || solicitado.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Terceiro || solicitado.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao)
                            {
                                Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito solicitacaoCredito = new Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito();
                                solicitacaoCredito.Carga = carga;
                                solicitacaoCredito.ComponenteFrete = componenteFrete;
                                solicitacaoCredito.DataSolicitacao = DateTime.Now;
                                solicitacaoCredito.MotivoSolicitacao = motivo;
                                solicitacaoCredito.RetornoSolicitacao = "";
                                solicitacaoCredito.Solicitado = solicitado;
                                solicitacaoCredito.Solicitante = solicitante;
                                solicitacaoCredito.ValorSolicitado = valorSolicitacao;
                                solicitacaoCredito.SituacaoSolicitacaoCredito = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.AgLiberacao;
                                repSolicitacaoCredito.Inserir(solicitacaoCredito);
                                solicitacaoCreditoGerada.SolicitacaoCredito = solicitacaoCredito;
                                solicitacaoCreditoGerada.GerouSolicitacao = true;

                                string nota = "";
                                if (solicitado.TipoAcesso != Dominio.Enumeradores.TipoAcesso.Terceiro && solicitado.TipoAcesso != Dominio.Enumeradores.TipoAcesso.Emissao)
                                {
                                    nota = string.Format(Localization.Resources.Credito.SolicitacaoCredito.SolicitouLiberacaoCreditoParaCarga, solicitante.Nome, valorSolicitacao.ToString("n2"), carga.CodigoCargaEmbarcador);
                                    serNotificacao.GerarNotificacao(solicitado, solicitante, solicitacaoCredito.Codigo, "Creditos/CreditoLiberacao", nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
                                }
                            }
                            else
                            {
                                solicitacaoCreditoGerada.MensagemRetorno = "Você não possui permissão para solicitar crédito deste creditor (" + solicitado.Nome + ").";
                            }
                        }
                        else
                        {
                            solicitacaoCreditoGerada.MensagemRetorno = "Você possui um saldo (" + retornoSaldo.Saldo.ToString("n2") + "), não utilizado, por favor verifique seu crédito.";
                        }
                    }
                    else
                    {
                        solicitacaoCreditoGerada.MensagemRetorno = "O valor solicitado deve ser maior que zero.";
                    }
                }
                else
                {
                    solicitacaoCreditoGerada.MensagemRetorno = "É obrigatório informar um motivo.";
                }
            }
            else
            {
                solicitacaoCreditoGerada.MensagemRetorno = "Não foi possível localizar para quem o crédito foi solicitado";
            }

            return solicitacaoCreditoGerada;

        }
    }
}
