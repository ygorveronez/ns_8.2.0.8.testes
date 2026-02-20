using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Documentos
{
    public class DadosDocsys : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.DadosDocsys>
    {
        public DadosDocsys(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Documentos.DadosDocsys BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DadosDocsys>();
            query = query.Where(o => o.Codigo == codigo);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.DadosDocsys> RegistroDuplicado(string vessel, string voy, string dir, string blading, string BOOKNO, string pol, string pod, string ubli)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DadosDocsys>();
            query = query.Where(o => o.vessel == vessel && o.voy == voy && o.dir == dir && o.blading == blading && o.BOOKNO == BOOKNO && o.pol == pol && o.pod == pod && o.UBLI != ubli);
            return query.ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Documentos.DadosDocsys> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroDadosDocsys filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaDadosDocsys = new ConsultaDadosDocsys().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaDadosDocsys.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Documentos.DadosDocsys)));

            return consultaDadosDocsys.SetTimeout(6000).List<Dominio.Relatorios.Embarcador.DataSource.Documentos.DadosDocsys>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroDadosDocsys filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaDadosDocsys = new ConsultaDadosDocsys().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaDadosDocsys.SetTimeout(6000).UniqueResult<int>();
        }
    }
}
