using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class TipoSeparacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoSeparacao>
    {
        #region Construtores

        public TipoSeparacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.TipoSeparacao> MontarQuery(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string codigoTipoSeparacaoEmbarcador = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoSeparacao>();
            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                result = result.Where(x => x.Ativo == (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false));

            if (!string.IsNullOrWhiteSpace(codigoTipoSeparacaoEmbarcador))
                result = result.Where(obj => obj.CodigoTipoSeparacaoEmbarcador.Contains(codigoTipoSeparacaoEmbarcador));

            return result;
        }

        #endregion

        #region Métodos públicos

        public List<Dominio.Entidades.Embarcador.Cargas.TipoSeparacao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, string codigoTipoSeparacaoEmbarcador = "")
        {
            var query = MontarQuery(descricao, ativo, codigoTipoSeparacaoEmbarcador);
            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string codigoTipoSeparacaoEmbarcador = "")
        {
            var query = MontarQuery(descricao, ativo, codigoTipoSeparacaoEmbarcador);
            return query.Count();
        }
        
        public Dominio.Entidades.Embarcador.Cargas.TipoSeparacao BuscarPadrao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoSeparacao>();
            var result = from obj in query where obj.PadraoMontagemCarregamentoAuto == true select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoSeparacao BuscarPorCodigoEmbarcador(string codigoTipoSeparacaoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoSeparacao>();
            var result = query.Where(x => x.CodigoTipoSeparacaoEmbarcador == codigoTipoSeparacaoEmbarcador);
            return result.FirstOrDefault();
        }

        #endregion

    }
}
