using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoPamcard : LongRunningProcessBase<IntegracaoPamcard>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            GerarIntegracoesPamcardPagamentoMotoristas(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
            GerarIntegracoesAdicionaisPagamentoMotoristas(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
        }

        public void GerarIntegracoesPamcardPagamentoMotoristas(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unidadeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            TipoIntegracaoPagamentoMotorista[] tipos = new TipoIntegracaoPagamentoMotorista[]
            {
                TipoIntegracaoPagamentoMotorista.PamCard,
                TipoIntegracaoPagamentoMotorista.PagBem,
                TipoIntegracaoPagamentoMotorista.PamCardCorporativo,
                TipoIntegracaoPagamentoMotorista.Email,
                TipoIntegracaoPagamentoMotorista.Target,
                TipoIntegracaoPagamentoMotorista.Extratta,
                TipoIntegracaoPagamentoMotorista.RepomFrete
            };

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };

            List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio> listaIntegracao = repPagamentoMotoristaIntegracaoEnvio.BuscarIntegracoesPendenteDeEnvio(tipos);
            for (var i = 0; i < listaIntegracao.Count; i++)
            {
                var integracao = listaIntegracao[i];
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotoristaTMS = repPagamentoMotoristaTMS.BuscarPorCodigo(integracao.PagamentoMotoristaTMS.Codigo);

                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.Data = DateTime.Now;
                integracao.NumeroTentativas += 1;

                if (pagamentoMotoristaTMS.Carga != null)
                {
                    pagamentoMotoristaTMS.Carga.AgIntegracaoPagamentoMotorista = false;
                    repCarga.Atualizar(pagamentoMotoristaTMS.Carga);
                }

                try
                {
                    TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotoristaTMS.PagamentoMotoristaTipo.TipoIntegracaoPagamentoMotorista;

                    if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PagBem)
                    {
                        Servicos.Embarcador.CIOT.Pagbem svcPagbem = new Servicos.Embarcador.CIOT.Pagbem();
                        svcPagbem.RealizarPagamentoMotorista(integracao.PagamentoMotoristaTMS, auditado, tipoServicoMultisoftware, unidadeTrabalho, out string mensagem);
                    }
                    else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCard)
                    {
                        Servicos.Embarcador.PagamentoMotorista.PamCard svcPamCard = new Servicos.Embarcador.PagamentoMotorista.PamCard(unidadeTrabalho);
                        svcPamCard.EmitirPagamentoMotorista(pagamentoMotoristaTMS.Codigo, unidadeTrabalho, tipoServicoMultisoftware, auditado);
                    }
                    else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCardCorporativo)
                    {
                        Servicos.Embarcador.PagamentoMotorista.PamCard svcPamCard = new Servicos.Embarcador.PagamentoMotorista.PamCard(unidadeTrabalho);
                        svcPamCard.EmitirPagamentoMotoristaPamcardCorporativo(pagamentoMotoristaTMS.Codigo, unidadeTrabalho, tipoServicoMultisoftware, auditado);
                    }
                    else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Email)
                    {
                        Servicos.Embarcador.PagamentoMotorista.IntegracaoEmailPagamentoMotorista svcIntegracaoEmail = new Servicos.Embarcador.PagamentoMotorista.IntegracaoEmailPagamentoMotorista(unidadeTrabalho);
                        svcIntegracaoEmail.EnviarEmailPagamentoMotorista(pagamentoMotoristaTMS.Codigo, unidadeTrabalho, tipoServicoMultisoftware, auditado);
                    }
                    else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Target)
                    {
                        Servicos.Embarcador.PagamentoMotorista.Target svcTarget = new Servicos.Embarcador.PagamentoMotorista.Target(unidadeTrabalho);
                        svcTarget.EmitirPagamentoMotorista(pagamentoMotoristaTMS.Codigo, tipoServicoMultisoftware, auditado);
                    }
                    else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Extratta)
                    {
                        Servicos.Embarcador.CIOT.Extratta svcExtratta = new Servicos.Embarcador.CIOT.Extratta();
                        svcExtratta.EmitirPagamentoMotorista(pagamentoMotoristaTMS.Codigo, unidadeTrabalho, tipoServicoMultisoftware, auditado);
                    }
                    else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.RepomFrete)
                    {
                        Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete svcRepomFrete = new Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete();
                        svcRepomFrete.EmitirPagamentoMotorista(pagamentoMotoristaTMS.Codigo, unidadeTrabalho, tipoServicoMultisoftware, auditado);
                    }
                }
                catch (ServicoException excecao)
                {
                    integracao.Retorno = excecao.Message;
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    pagamentoMotoristaTMS.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.FalhaIntegracao;
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    integracao.Retorno = excecao.Message;
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    pagamentoMotoristaTMS.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.FalhaIntegracao;
                }

                #region Verificar integração pendente
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio> listaIntegracaoPendente = repPagamentoMotoristaIntegracaoEnvio.BuscarIntegracoesPendenteDeEnvio(pagamentoMotoristaTMS.Codigo, null);

                if (listaIntegracaoPendente.Count == 0)
                    pagamentoMotoristaTMS.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;
                #endregion

                repPagamentoMotoristaTMS.Atualizar(pagamentoMotoristaTMS);
                repPagamentoMotoristaIntegracaoEnvio.Atualizar(integracao);

                if (configuracaoTMS.ConfirmarPagamentoMotoristaAutomaticamente && pagamentoMotoristaTMS.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.Finalizada)
                {
                    string msgRetorno = "";
                    if (!Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamentoMotoristaTMS.Codigo, configuracaoTMS.TipoMovimentoPagamentoMotorista, auditado, pagamentoMotoristaTMS.Usuario, unidadeTrabalho, unidadeTrabalho.StringConexao, tipoServicoMultisoftware))
                        Servicos.Log.TratarErro($"Ocorreu uma falha ao confirmar o pagamento do motorista código {pagamentoMotoristaTMS.Codigo}: {msgRetorno}");
                }
            }
        }

        public void GerarIntegracoesAdicionaisPagamentoMotoristas(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unidadeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            TipoIntegracaoPagamentoMotorista[] tipos = new TipoIntegracaoPagamentoMotorista[]
            {
                TipoIntegracaoPagamentoMotorista.KMM
            };

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };

            List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio> listaIntegracao = repPagamentoMotoristaIntegracaoEnvio.BuscarIntegracoesPendenteDeEnvioAdicionais(tipos);
            for (var i = 0; i < listaIntegracao.Count; i++)
            {
                var integracao = listaIntegracao[i];
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotoristaTMS = repPagamentoMotoristaTMS.BuscarPorCodigo(integracao.PagamentoMotoristaTMS.Codigo);

                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.Data = DateTime.Now;
                integracao.NumeroTentativas += 1;

                try
                {
                    TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = integracao.TipoIntegracaoPagamentoMotorista;

                    if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.KMM)
                    {
                        Servicos.Embarcador.Integracao.KMM.IntegracaoKMM svcKMM = new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unidadeTrabalho);
                        svcKMM.IntegrarPagamentoMotorista(pagamentoMotoristaTMS.Codigo, unidadeTrabalho, tipoServicoMultisoftware, auditado);
                    }
                }
                catch (ServicoException excecao)
                {
                    integracao.Retorno = excecao.Message;
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    integracao.Retorno = excecao.Message;
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                repPagamentoMotoristaIntegracaoEnvio.Atualizar(integracao);

                #region Verificar integração pendente
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio> listaIntegracaoPendente = repPagamentoMotoristaIntegracaoEnvio.BuscarIntegracoesPendenteDeEnvio(pagamentoMotoristaTMS.Codigo, null);

                if (listaIntegracaoPendente.Count == 0)
                {
                    pagamentoMotoristaTMS = repPagamentoMotoristaTMS.BuscarPorCodigo(pagamentoMotoristaTMS.Codigo);
                    pagamentoMotoristaTMS.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;
                    repPagamentoMotoristaTMS.Atualizar(pagamentoMotoristaTMS);

                    if (configuracaoTMS.ConfirmarPagamentoMotoristaAutomaticamente)
                    {
                        string msgRetorno = "";
                        if (!Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamentoMotoristaTMS.Codigo, configuracaoTMS.TipoMovimentoPagamentoMotorista, auditado, pagamentoMotoristaTMS.Usuario, unidadeTrabalho, unidadeTrabalho.StringConexao, tipoServicoMultisoftware))
                            Servicos.Log.TratarErro($"Ocorreu uma falha ao confirmar o pagamento do motorista código {pagamentoMotoristaTMS.Codigo}: {msgRetorno}");
                    }
                }
                #endregion
            }
        }
    }
}