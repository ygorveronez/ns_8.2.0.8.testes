using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class Pneu : RepositorioBase<Dominio.Entidades.Pneu>, Dominio.Interfaces.Repositorios.Pneu
    {
        public Pneu(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Pneu BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pneu>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Pneu BuscarPorCodigoEVeiculo(int codigo, int codigoVeiculo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pneu>();
            var result = from obj in query where obj.Codigo == codigo && obj.Veiculo.Codigo == codigoVeiculo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Pneu BuscarPorEixoEVeiculo(int codigoEixo, int codigoVeiculo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pneu>();
            var result = from obj in query where obj.Eixo.Codigo == codigoEixo && obj.Veiculo.Codigo == codigoVeiculo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Pneu> ConsultarParaManutencao(int codigoEmpresa, string serie, string tipoStatus, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pneu>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.StatusPneu.Tipo.Equals(tipoStatus) && obj.Status.Equals("A") select obj;

            if (!string.IsNullOrWhiteSpace(serie))
                result = result.Where(o => o.Serie == serie);

            return result.OrderBy(o => o.Serie).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaParaManutencao(int codigoEmpresa, string serie, string tipoStatus)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pneu>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.StatusPneu.Tipo.Equals(tipoStatus) && obj.Status.Equals("A") select obj;

            if (!string.IsNullOrWhiteSpace(serie))
                result = result.Where(o => o.Serie == serie);

            return result.Count();
        }

        public IList<Dominio.Entidades.Pneu> Consultar(int codigoEmpresa, string marcaPneu, string modeloPneu, string dimensaoPneu, string statusPneu, string status, string serie, int inicioRegistros, int maximoRegistros, string[] tipoStatusPneu = null, int codigoVeiculo = 0)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Pneu>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.CreateAlias("StatusPneu", "status");
            if (!string.IsNullOrWhiteSpace(marcaPneu))
            {
                criteria.CreateAlias("MarcaPneu", "marca");
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("marca.Descricao", marcaPneu, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(modeloPneu))
            {
                criteria.CreateAlias("ModeloPneu", "modelo");
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("modelo.Descricao", modeloPneu, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(dimensaoPneu))
            {
                criteria.CreateAlias("DimensaoPneu", "dimensao");
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("dimensao.Descricao", dimensaoPneu, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(serie))
            {
                criteria.Add(NHibernate.Criterion.Restrictions.Eq("Serie", serie));
            }
            if (!string.IsNullOrWhiteSpace(statusPneu))
            {
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("status.Descricao", statusPneu, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (tipoStatusPneu != null && tipoStatusPneu.Count() > 0)
            {
                criteria.Add(NHibernate.Criterion.Restrictions.In("status.Tipo", tipoStatusPneu));
            }
            if (codigoVeiculo > 0)
            {
                criteria.Add(NHibernate.Criterion.Restrictions.Eq("Veiculo.Codigo", codigoVeiculo));
            }
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<Dominio.Entidades.Pneu>();
        }

        public int ContarConsulta(int codigoEmpresa, string marcaPneu, string modeloPneu, string dimensaoPneu, string statusPneu, string status, string serie, string[] tipoStatusPneu = null, int codigoVeiculo = 0)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Pneu>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.CreateAlias("StatusPneu", "status");
            if (!string.IsNullOrWhiteSpace(marcaPneu))
            {
                criteria.CreateAlias("MarcaPneu", "marca");
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("marca.Descricao", marcaPneu, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(modeloPneu))
            {
                criteria.CreateAlias("ModeloPneu", "modelo");
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("modelo.Descricao", modeloPneu, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(dimensaoPneu))
            {
                criteria.CreateAlias("DimensaoPneu", "dimensao");
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("dimensao.Descricao", dimensaoPneu, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(serie))
            {
                criteria.Add(NHibernate.Criterion.Restrictions.Eq("Serie", serie));
            }
            if (!string.IsNullOrWhiteSpace(statusPneu))
            {
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("status.Descricao", statusPneu, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (tipoStatusPneu != null && tipoStatusPneu.Count() > 0)
            {
                criteria.Add(NHibernate.Criterion.Restrictions.In("status.Tipo", tipoStatusPneu));
            }
            if (codigoVeiculo > 0)
            {
                criteria.Add(NHibernate.Criterion.Restrictions.Eq("Veiculo.Codigo", codigoVeiculo));
            }
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public IList<Dominio.Entidades.Pneu> ConsultarPorHistorico(int codigoEmpresa, string marcaPneu, string modeloPneu, string dimensaoPneu, string statusPneu, int inicioRegistros, int maximoRegistros, int codigoVeiculo = 0, string tipoHistorico = "")
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Pneu>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Status", "A"));
            criteria.CreateAlias("StatusPneu", "status");
            criteria.CreateAlias("ModeloPneu", "modelo");
            if (!string.IsNullOrWhiteSpace(marcaPneu))
            {
                criteria.CreateAlias("MarcaPneu", "marca");
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("marca.Descricao", marcaPneu, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(modeloPneu))
            {
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("modelo.Descricao", modeloPneu, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(dimensaoPneu))
            {
                criteria.CreateAlias("DimensaoPneu", "dimensao");
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("dimensao.Descricao", dimensaoPneu, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(statusPneu))
            {
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("status.Descricao", statusPneu, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (tipoHistorico.Equals("E"))
            {
                criteria.Add(NHibernate.Criterion.Restrictions.IsNull("Veiculo"));
                criteria.Add(NHibernate.Criterion.Restrictions.In("status.Tipo", new string[] { "A", "S" }));
            }
            else if (tipoHistorico.Equals("S"))
            {
                criteria.Add(NHibernate.Criterion.Restrictions.Eq("Veiculo.Codigo", codigoVeiculo));
                criteria.Add(NHibernate.Criterion.Restrictions.In("status.Tipo", new string[] { "E"}));
            }
            else
            {
                criteria.Add(NHibernate.Criterion.Restrictions.Eq("Codigo", 0));
            }
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<Dominio.Entidades.Pneu>();
        }

        public int ContarConsultaPorHistorico(int codigoEmpresa, string marcaPneu, string modeloPneu, string dimensaoPneu, string statusPneu, int codigoVeiculo = 0, string tipoHistorico = "")
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Pneu>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.CreateAlias("StatusPneu", "status");
            if (!string.IsNullOrWhiteSpace(marcaPneu))
            {
                criteria.CreateAlias("MarcaPneu", "marca");
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("marca.Descricao", marcaPneu, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(modeloPneu))
            {
                criteria.CreateAlias("ModeloPneu", "modelo");
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("modelo.Descricao", modeloPneu, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(dimensaoPneu))
            {
                criteria.CreateAlias("DimensaoPneu", "dimensao");
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("dimensao.Descricao", dimensaoPneu, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(statusPneu))
            {
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("status.Descricao", statusPneu, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (tipoHistorico.Equals("E"))
            {
                criteria.Add(NHibernate.Criterion.Restrictions.IsNull("Veiculo"));
                criteria.Add(NHibernate.Criterion.Restrictions.In("status.Tipo", new string[] { "A", "S" }));
            }
            else if (tipoHistorico.Equals("S"))
            {
                criteria.Add(NHibernate.Criterion.Restrictions.Eq("Veiculo.Codigo", codigoVeiculo));
                criteria.Add(NHibernate.Criterion.Restrictions.In("status.Tipo", new string[] { "E" }));
            }
            criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", "A"));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public int ContarPorStatus(int codigoStatus, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pneu>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.StatusPneu.Codigo == codigoStatus select obj.Codigo;
            return result.Count<int>();
        }

        public List<Dominio.Entidades.Pneu> BuscarPorEixosEVeiculo(List<int> listaIdEixos, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pneu>();
            var result = from obj in query where listaIdEixos.Contains(obj.Eixo.Codigo) && obj.Veiculo.Codigo == codigoVeiculo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Pneu> BuscarPorVeiculo(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pneu>();

            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo select obj;

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioHistoricosPneusMapa> RelatorioMapa(int codigoEmpresa, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pneu>();
            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa && obj.Veiculo.Codigo == codigoVeiculo && obj.Status.Equals("A")
                         select new
                         {
                             Dianteira = obj.Eixo.Dianteiro,
                             Interna_Externa = obj.Eixo.Interno_Externo,
                             Ordem = obj.Eixo.OrdemEixo,
                             Posicao = obj.Eixo.Posicao,
                             SeriePneu = obj.Serie,
                             Tipo = obj.Eixo.Tipo
                         };

            var listaMapaPneu = result.ToList();
            var listaOrdem = (from obj in listaMapaPneu select obj.Ordem).Distinct().ToList();
            var listaMapa = new List<Dominio.ObjetosDeValor.Relatorios.RelatorioHistoricosPneusMapa>();
            foreach (int ordem in listaOrdem)
            {
                var listaMapasOrdem = (from obj in listaMapaPneu where obj.Ordem == ordem select obj).ToList();
                var eixo = new Dominio.ObjetosDeValor.Relatorios.RelatorioHistoricosPneusMapa()
                {
                    Dianteiro = (from mapa in listaMapasOrdem where mapa.Dianteira == true select mapa.Dianteira).FirstOrDefault(),
                    DireitoInterno = (from mapa in listaMapasOrdem where (mapa.Tipo == "D" || mapa.Tipo == "S") && mapa.Posicao == "D" && mapa.Interna_Externa == "I" select mapa.SeriePneu).FirstOrDefault(),
                    DireitoExterno = (from mapa in listaMapasOrdem where (mapa.Tipo == "D" || mapa.Tipo == "S") && mapa.Posicao == "D" && mapa.Interna_Externa == "E" select mapa.SeriePneu).FirstOrDefault(),
                    Estepe = (from mapa in listaMapasOrdem where mapa.Tipo == "E" select mapa.SeriePneu).FirstOrDefault(),
                    EsquerdoInterno = (from mapa in listaMapasOrdem where (mapa.Tipo == "D" || mapa.Tipo == "S") && mapa.Posicao == "E" && mapa.Interna_Externa == "I" select mapa.SeriePneu).FirstOrDefault(),
                    EsquerdoExterno = (from mapa in listaMapasOrdem where (mapa.Tipo == "D" || mapa.Tipo == "S") && mapa.Posicao == "E" && mapa.Interna_Externa == "E" select mapa.SeriePneu).FirstOrDefault(),
                    Ordem = ordem
                };
                listaMapa.Add(eixo);
            }

            return listaMapa;
        }
    }
}
