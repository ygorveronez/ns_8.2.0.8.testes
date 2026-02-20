using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.TorreControle
{
    public class RegraQualidadeMonitoramento : RepositorioBase<Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento>
    {
        public RegraQualidadeMonitoramento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento> BuscarRegrasAtivas()
        {
            IQueryable<Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento>();
            query = query.Where(ent => ent.Ativo);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento> BuscarRegrasAtivasPorTipoRegra(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraQualidadeMonitoramento tipoRegra)
        {
            IQueryable<Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento>();
            query = query.Where(ent => ent.Ativo && ent.TipoRegraQualidadeMonitoramento == tipoRegra);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento> Consultar(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRegraQualidadeMonitoramento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(filtrosPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRegraQualidadeMonitoramento filtrosPesquisa)
        {
            var consulta = Consultar(filtrosPesquisa);

            return consulta.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento> Consultar(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRegraQualidadeMonitoramento filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento>();
            var consultaRegraQualidadeMonitoramento = from obj in query select obj;

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                consultaRegraQualidadeMonitoramento = consultaRegraQualidadeMonitoramento.Where(regra => regra.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.TipoRegra != null)
                consultaRegraQualidadeMonitoramento = consultaRegraQualidadeMonitoramento.Where(regra => regra.TipoRegraQualidadeMonitoramento == filtrosPesquisa.TipoRegra);

            if (filtrosPesquisa.Ativo != null)
                consultaRegraQualidadeMonitoramento = consultaRegraQualidadeMonitoramento.Where(regra => regra.Ativo == filtrosPesquisa.Ativo);


            return consultaRegraQualidadeMonitoramento;
        }
    }
}
