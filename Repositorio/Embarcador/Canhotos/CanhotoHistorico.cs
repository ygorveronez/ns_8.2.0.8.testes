using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;


namespace Repositorio.Embarcador.Canhotos
{
    public class CanhotoHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico>
    {
        public CanhotoHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CanhotoHistorico(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico> BuscarPorCanhoto(int canhoto)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico>();

            query = query.Where(o => o.Canhoto.Codigo == canhoto);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico> BuscarPorCanhotos(List<int> canhotos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico>();

            query = query.Where(o => canhotos.Contains(o.Canhoto.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico> Consultar(int canhoto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico>();

            var result = from obj in query where obj.Canhoto.Codigo == canhoto select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico> Consultar(int canhoto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico>();

            var result = from obj in query where obj.Canhoto.Codigo == canhoto select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros)
                .Fetch(obj => obj.Usuario)
                .Fetch(o => o.LocalArmazenamentoCanhoto)
                .Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int canhoto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico>();

            var result = from obj in query where obj.Canhoto.Codigo == canhoto select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico> ConsultarPorCTe(int codigoCTe, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico>();

            var result = from obj in query select obj;
            result = result.Where(obj => obj.Canhoto.XMLNotaFiscal.CTEs.Any(o => o.Codigo == codigoCTe));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros)
                .Fetch(obj => obj.Usuario)
                .Fetch(o => o.LocalArmazenamentoCanhoto)
                .Take(maximoRegistros).ToList();
        }

        public int ContarConsultaPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico>();

            var result = from obj in query select obj;
            result = result.Where(obj => obj.Canhoto.XMLNotaFiscal.CTEs.Any(o => o.Codigo == codigoCTe));

            return result.Count();
        }

        #region Relatorios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.HistoricoMovimentacaoCanhoto> ConsultarRelatorioHistoricoMovimentacaoCanhoto(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaHistoricoMovimentacaoCanhoto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propiedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaHistoricoCanhoto = new ConsultaHistoricoMovimentacaoCanhoto().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propiedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaHistoricoCanhoto.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.HistoricoMovimentacaoCanhoto)));

            return consultaHistoricoCanhoto.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.HistoricoMovimentacaoCanhoto>();

        }

        public int ContarConsultaRelatorioHistoricoMovimentacaoCanhoto(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaHistoricoMovimentacaoCanhoto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propiedades)
        {
            var consultaHistoricoCanhoto = new ConsultaHistoricoMovimentacaoCanhoto().ObterSqlContarPesquisa(filtrosPesquisa, propiedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaHistoricoCanhoto.SetTimeout(600).UniqueResult<int>();

        }
        #endregion



    }
}
