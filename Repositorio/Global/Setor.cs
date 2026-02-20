using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;


namespace Repositorio
{
    public class Setor : RepositorioBase<Dominio.Entidades.Setor>, Dominio.Interfaces.Repositorios.Setor
    {
        #region Construtores

        public Setor(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Setor(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Setor> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, List<int> setores = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Setor>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status == "A");
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => o.Status == "I");
            if (setores != null && setores.Count > 0)
                result = result.Where(o => setores.Contains(o.Codigo));

            return result;
        }

        private IQueryable<Dominio.Entidades.Setor> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int codigoFilial, List<int> setores = null)
        {
            if (codigoFilial == 0)
                return Consultar(descricao, status, setores);

            var sqlPesquisa = new System.Text.StringBuilder()
                .Append("select SETOR.SET_CODIGO Codigo, SETOR.SET_DESCRICAO Descricao, SETOR.SET_STATUS Status ")
                .Append("  from T_FILIAL_SETORES SETORES ")
                .Append("  join T_SETOR_FILIAL SETOR_FILIAL ")
                .Append("    on SETORES.SEF_CODIGO = SETOR_FILIAL.SEF_CODIGO ")
                .Append("  join T_SETOR SETOR ")
                .Append("    on SETOR_FILIAL.SET_CODIGO = SETOR.SET_CODIGO ")
                .Append($"where SETORES.FIL_CODIGO = {codigoFilial} ");

            if (!string.IsNullOrWhiteSpace(descricao))
                sqlPesquisa.Append($" and SETOR.SET_DESCRICAO LIKE '%{descricao}%' ");

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                sqlPesquisa.Append($" and SETOR.SET_STATUS = 'A' ");
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                sqlPesquisa.Append($" and SETOR.SET_STATUS = 'I' ");

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlPesquisa.ToString());

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Entidades.Setor)));

            return query.List<Dominio.Entidades.Setor>().AsQueryable();
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Setor BuscarPorCodigo(int codigo)
        {
            var consultaSetor = this.SessionNHiBernate.Query<Dominio.Entidades.Setor>()
                .Where(setor => setor.Codigo == codigo);

            return consultaSetor.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Setor> BuscarPorCodigoAsync(int codigo)
        {
            var consultaSetor = this.SessionNHiBernate.Query<Dominio.Entidades.Setor>()
                .Where(setor => setor.Codigo == codigo);

            return await consultaSetor.FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Setor> BuscarPorCodigos(List<int> codigos)
        {
            var consultaSetor = this.SessionNHiBernate.Query<Dominio.Entidades.Setor>()
                .Where(setor => codigos.Contains(setor.Codigo));

            return consultaSetor.ToList();
        }

        public List<Dominio.Entidades.Setor> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int codigoFilial, List<int> setores, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(descricao, status, codigoFilial, setores);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int codigoFilial, List<int> setores = null)
        {
            var result = Consultar(descricao, status, codigoFilial, setores);

            return result.Count();
        }

        public Dominio.Entidades.Setor BuscarPorCarga(int codigoCarga)
        {
            var consultaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>()
                .Where(carga => carga.Codigo == codigoCarga);

            return consultaCarga.Select(o => o.Filial.SetorAtendimento).FirstOrDefault();
        }

        public Dominio.Entidades.Setor BuscarPorDescricao(string descricao) {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Setor>()
                   .Where(x => x.Descricao.ToLower().Equals(descricao));

            return query.FirstOrDefault();
        }

        #endregion
    }
}
