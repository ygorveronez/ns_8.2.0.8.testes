using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Usuarios.Colaborador
{
    public class ColaboradorSituacaoLancamentoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao>
    {
        public ColaboradorSituacaoLancamentoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao> BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao>();
            var result = from obj in query where obj.SituacaoIntegracao == situacaoIntegracao select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao BuscarPorCodigoSituacao(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao>()
                .Where(o => o.ColaboradorLancamento.Codigo == codigo && o.SituacaoIntegracao == situacaoIntegracao);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao> Consultar(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consulta = Consultar(codigo, situacao);

            return ObterLista(consulta, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacaoIntegracao)
        {
            var consulta = Consultar(codigo, situacaoIntegracao);

            return consulta.Count();
        }

        public Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return query;
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao> Consultar(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao>().Where(o => o.ColaboradorLancamento.Codigo == codigo);

            if (situacao.HasValue)
                consulta = consulta.Where(o => o.SituacaoIntegracao == situacao);

            return consulta;
        }

        #endregion
    }
}
