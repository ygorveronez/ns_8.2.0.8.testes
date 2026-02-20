using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class AbastecimentoTicketLog : RepositorioBase<Dominio.Entidades.AbastecimentoTicketLog>
    {
        public AbastecimentoTicketLog(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public AbastecimentoTicketLog(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }


        #region Métodos Públicos

        public Dominio.Entidades.AbastecimentoTicketLog BuscarPorCodigoEAcertoDeViagem(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AbastecimentoTicketLog>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }


        public bool VerificarSeJaExisteAbastecimentoImportacaoWS(string codigoTransacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AbastecimentoTicketLog>();

            int codTransacao = codigoTransacao.ToInt();

            return query.Where(o => (o.CodigoTransacao == codTransacao)).Any();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frotas.AbastecimentoTicketLog> ConsultarRelatorioAbastecimentoTicketLog(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimentoTicketLog filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaAbastecimentoTicketLog = new Repositorio.Embarcador.TicketLog.Consulta.ConsultaAbastecimentoTicketLog().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaAbastecimentoTicketLog.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frotas.AbastecimentoTicketLog)));

            return consultaAbastecimentoTicketLog.SetTimeout(1200).List<Dominio.Relatorios.Embarcador.DataSource.Frotas.AbastecimentoTicketLog>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frotas.AbastecimentoTicketLog>> ConsultarRelatorioAbastecimentoTicketLogAsync(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimentoTicketLog filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaAbastecimentoTicketLog = new Repositorio.Embarcador.TicketLog.Consulta.ConsultaAbastecimentoTicketLog().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaAbastecimentoTicketLog.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frotas.AbastecimentoTicketLog)));

            return await consultaAbastecimentoTicketLog.SetTimeout(1200).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frotas.AbastecimentoTicketLog>();
        }

        public int ContarConsultaRelatorioAbastecimentoTicketLog(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimentoTicketLog filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaAbastecimentoTicketLog = new Repositorio.Embarcador.TicketLog.Consulta.ConsultaAbastecimentoTicketLog().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaAbastecimentoTicketLog.SetTimeout(1200).UniqueResult<int>();
        }

        #endregion
    }
}
