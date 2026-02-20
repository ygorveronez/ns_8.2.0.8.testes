using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Canhotos
{
    public class MotivoInconsistenciaDigitacao : RepositorioBase<Dominio.Entidades.Embarcador.Canhotos.MotivoInconsistenciaDigitacao>
    {
        #region Construtores

        public MotivoInconsistenciaDigitacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Canhotos.MotivoInconsistenciaDigitacao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaMotivoInconsistenciaDigitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.MotivoInconsistenciaDigitacao>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaMotivoInconsistenciaDigitacao = consultaMotivoInconsistenciaDigitacao.Where(o => o.Descricao.Contains(descricao));

            if (situacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaMotivoInconsistenciaDigitacao = consultaMotivoInconsistenciaDigitacao.Where(o => o.Ativo);

            if (situacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaMotivoInconsistenciaDigitacao = consultaMotivoInconsistenciaDigitacao.Where(o => !o.Ativo);

            return consultaMotivoInconsistenciaDigitacao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Canhotos.MotivoInconsistenciaDigitacao BuscarPorCodigo(int codigo)
        {
            var motivoInconsistenciaDigitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.MotivoInconsistenciaDigitacao>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return motivoInconsistenciaDigitacao;
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.MotivoInconsistenciaDigitacao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMotivoInconsistenciaDigitacao = Consultar(descricao, situacaoAtivo);

            return ObterLista(consultaMotivoInconsistenciaDigitacao, parametrosConsulta);
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaMotivoInconsistenciaDigitacao = Consultar(descricao, situacaoAtivo);

            return consultaMotivoInconsistenciaDigitacao.Count();
        }

        #endregion
    }
}
