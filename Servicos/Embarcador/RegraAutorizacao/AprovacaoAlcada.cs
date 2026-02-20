using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.RegraAutorizacao
{
    public abstract class AprovacaoAlcada<TOrigem, TRegra, TAprovacao>
        where TOrigem : Dominio.Entidades.EntidadeBase, Dominio.Interfaces.Embarcador.Entidade.IEntidade
        where TRegra : Dominio.Entidades.Embarcador.RegraAutorizacao.RegraAutorizacao
        where TAprovacao : Dominio.Entidades.Embarcador.RegraAutorizacao.AprovacaoAlcada<TOrigem, TRegra>
    {
        #region Atributos

        protected readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public AprovacaoAlcada(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Protegidos

        protected bool ValidarAlcadas<TAlcada, TPropriedade>(IList<TAlcada> alcadas, object valorComparar)
            where TAlcada : Dominio.Entidades.Embarcador.RegraAutorizacao.Alcada<TRegra, TPropriedade>
        {
            if (alcadas.ToList().All(alcada => alcada.IsCondicaoVerdadeira(valorComparar) && alcada.IsJuncaoTodasVerdadeiras()))
                return true;

            if (alcadas.ToList().Any(alcada => alcada.IsCondicaoVerdadeira(valorComparar) && !alcada.IsJuncaoTodasVerdadeiras()))
                return true;

            return false;
        }

        #endregion

        #region Métodos Protegidos Abstratos

        protected abstract void NotificarAprovador(TOrigem origemAprovacao, TAprovacao aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware);

        #endregion

        #region Métodos Públicos

        public bool LiberarProximaPrioridadeAprovacao(TOrigem origemAprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem> repositorioAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem>(_unitOfWork);
            List<TAprovacao> alcadasAprovacao = repositorioAutorizacao.BuscarPendentesBloqueadas(origemAprovacao.Codigo);

            if (alcadasAprovacao.Count > 0)
            {
                int menorPrioridadeAprovacao = alcadasAprovacao.Select(alcada => alcada.RegraAutorizacao.PrioridadeAprovacao).Min();

                foreach (TAprovacao aprovacao in alcadasAprovacao)
                {
                    if (aprovacao.RegraAutorizacao.PrioridadeAprovacao == menorPrioridadeAprovacao)
                    {
                        aprovacao.Bloqueada = false;
                        repositorioAutorizacao.Atualizar(aprovacao);

                        NotificarAprovador(origemAprovacao, aprovacao, tipoServicoMultisoftware);
                    }
                }

                return false;
            }

            return true;
        }

        public virtual void RemoverAprovacao(TOrigem origemAprovacao)
        {
            Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem> repositorioAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem>(_unitOfWork);

            repositorioAutorizacao.DeletarPorOrigemAprovacao(origemAprovacao.Codigo);
        }

        public virtual void RemoverAprovacaoPorCodigo(int codigoOrigemAprovacao)
        {
            Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem> repositorioAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem>(_unitOfWork);

            repositorioAutorizacao.DeletarPorOrigemAprovacao(codigoOrigemAprovacao);
        }

        #endregion
    }
}
