using AdminMultisoftware.Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoLoteEscrituracao : ServicoBase
    {
        #region Métodos Públicos

        public IntegracaoLoteEscrituracao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IntegracaoLoteEscrituracao(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken)
        {
        }

        public void IniciarIntegracoesComDocumentos(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao loteEscrituracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipos = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>();
            tipos.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Digibee);
            tipos.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil);

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repTipoIntegracao.BuscarPorTipos(tipos, null);
            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                switch (tipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Digibee:
                        Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();
                        if (!string.IsNullOrWhiteSpace(integracao?.URLIntegracaoEscrituracaoCTeDigibee))
                            AdcionarLoteEscrituracaoParaIntegracao(loteEscrituracao, tipoIntegracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil:
                        AdcionarLoteEscrituracaoParaIntegracao(loteEscrituracao, tipoIntegracao);
                        break;
                    default:
                        break;
                }
            }
        }

        public void AdcionarLoteEscrituracaoParaIntegracao(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao loteEscrituracao, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);

            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao repLoteEscrituracaoIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao loteEscrituracaoIntegracao = repLoteEscrituracaoIntegracao.BuscarPorLoteEscrituracaoETipoIntegracao(loteEscrituracao.Codigo, tipoIntegracao.Tipo);

            if (loteEscrituracaoIntegracao == null)
            {
                loteEscrituracaoIntegracao = new Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao();
                loteEscrituracaoIntegracao.LoteEscrituracao = loteEscrituracao;
                loteEscrituracaoIntegracao.DataIntegracao = DateTime.Now;
                loteEscrituracaoIntegracao.NumeroTentativas = 0;
                loteEscrituracaoIntegracao.ProblemaIntegracao = "";
                loteEscrituracaoIntegracao.TipoIntegracao = tipoIntegracao;
                loteEscrituracaoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                repLoteEscrituracaoIntegracao.Inserir(loteEscrituracaoIntegracao);
            }
        }

        public void IniciarIntegracoesComEDI(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao loteEscrituracao, Repositorio.UnitOfWork unitOfWork)
        {
            IntegracaoEDI.AdicionarEDIParaIntegracao(loteEscrituracao, unitOfWork);
        }

        public async Task VerificarIntegracoesPendentesAsync()
        {
            IntegracaoEDI servicoIntegracaoEDI = new IntegracaoEDI(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao> loteEscrituracaos = await servicoIntegracaoEDI.VerificarIntegracoesPendentesEscrituracaoAsync();
            loteEscrituracaos = loteEscrituracaos.Distinct().ToList();


            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadraoAsync();

            foreach (Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao loteEscrituracao in loteEscrituracaos)
            {
                AtualizarSituacaoEscrituracaoIntegracao(loteEscrituracao, configuracao, _unitOfWork, _unitOfWork.StringConexao, _tipoServicoMultisoftware);
            }
        }

        public static void AtualizarSituacaoEscrituracaoIntegracao(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao loteEscrituracao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao repEscrituracaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.LoteEscrituracao repLoteEscrituracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracao(unitOfWork);

            loteEscrituracao = repLoteEscrituracao.BuscarPorCodigo(loteEscrituracao.Codigo);

            if (loteEscrituracao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao.AgIntegracao ||
                loteEscrituracao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao.FalhaIntegracao)
            {
                if (repEscrituracaoEDIIntegracao.ContarPorLoteEscrituracao(loteEscrituracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                {
                    loteEscrituracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao.FalhaIntegracao;
                    repLoteEscrituracao.Atualizar(loteEscrituracao);
                }
                else if (repEscrituracaoEDIIntegracao.ContarPorLoteEscrituracao(loteEscrituracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                {
                    loteEscrituracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao.FalhaIntegracao;
                    repLoteEscrituracao.Atualizar(loteEscrituracao);
                }
                else
                {
                    loteEscrituracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao.Finalizado;
                    repLoteEscrituracao.Atualizar(loteEscrituracao);
                }
            }
        }

        public static bool VerificarSePossuiIntegracao(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao loteEscrituracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao repEscrituracaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao(unidadeDeTrabalho);

            if (repEscrituracaoEDIIntegracao.ContarPorLoteEscrituracaoESituacaoDiff(loteEscrituracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                return true;

            return false;
        }

        #endregion
    }
}
