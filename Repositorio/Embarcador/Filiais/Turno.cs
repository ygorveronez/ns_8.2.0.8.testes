using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Filiais
{
    public class Turno : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.Turno>
    {
        #region Construtores

        public Turno(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Filiais.Turno> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var listaTurno = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Turno>();

            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                bool ativo = status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                listaTurno = listaTurno.Where(turno => turno.Ativo == ativo);
            }

            if (!string.IsNullOrWhiteSpace(descricao))
                listaTurno = listaTurno.Where(turno => turno.Descricao.Contains(descricao));

            return listaTurno;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Filiais.Turno> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int codigoFilial, int codigoSetor)
        {
            if ((codigoFilial == 0) || (codigoSetor == 0))
                return Consultar(descricao, status);

            var sqlPesquisa = new System.Text.StringBuilder()
                .Append("select TURNO.TUR_CODIGO Codigo, TURNO.TUR_DESCRICAO Descricao, ")
                .Append("       TURNO.TUR_ATIVO Ativo, TURNO.TUR_OBSERVACAO Observacao ")
                .Append("  from T_FILIAL_SETORES SETORES ")
                .Append("  join T_SETOR_FILIAL SETOR_FILIAL ")
                .Append("    on SETORES.SEF_CODIGO = SETOR_FILIAL.SEF_CODIGO ")
                .Append("  join T_SETOR_FILIAL_TURNO FILIAL_TURNO ")
                .Append("    on SETOR_FILIAL.SEF_CODIGO = FILIAL_TURNO.SEF_CODIGO ")
                .Append("  join T_TURNO TURNO ")
                .Append("    on FILIAL_TURNO.TUR_CODIGO = TURNO.TUR_CODIGO ")
                .Append($"where SETORES.FIL_CODIGO = {codigoFilial} ")
                .Append($"  and SETOR_FILIAL.SET_CODIGO = {codigoSetor} ");

            if (!string.IsNullOrWhiteSpace(descricao))
                sqlPesquisa.Append($" and TURNO.TUR_DESCRICAO LIKE '%{descricao}%' ");

            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                sqlPesquisa.Append($" and TURNO.TUR_ATIVO = {(int)status} ");

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlPesquisa.ToString());

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Entidades.Embarcador.Filiais.Turno)));

            return query.List<Dominio.Entidades.Embarcador.Filiais.Turno>().AsQueryable();
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Filiais.Turno BuscarPorCodigo(int codigo)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Turno>()
                .Where(turno => turno.Codigo == codigo)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.Turno> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int codigoFilial, int codigoSetor, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var listaTurno = Consultar(descricao, status, codigoFilial, codigoSetor);

            if (inicioRegistros > 0)
                listaTurno = listaTurno.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                listaTurno = listaTurno.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar))
                listaTurno = listaTurno.OrderBy(propriedadeOrdenar + (direcaoOrdenacao == "asc" ? " ascending" : " descending"));

            return listaTurno.ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int codigoFilial, int codigoSetor)
        {
            var result = Consultar(descricao, status, codigoFilial, codigoSetor);

            return result.Count();
        }

        #endregion
    }
}
