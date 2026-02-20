using NHibernate.Linq;
using Repositorio.Embarcador.CTe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class ContainerCTE : RepositorioBase<Dominio.Entidades.ContainerCTE>, Dominio.Interfaces.Repositorios.ContainerCTE
    {
        public ContainerCTE(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ContainerCTE(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.ContainerCTE> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ContainerCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.ContainerCTE>> BuscarPorCTeAsync(int codigoCTe,CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ContainerCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe select obj;

            return result.ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.ContainerCTE> BuscarPorContainers(List<int> codigosContainers, int codigoCarga)
        {
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(c => c.Carga.Codigo == codigoCarga);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ContainerCTE>();
            query = query.Where(obj => codigosContainers.Contains(obj.Container.Codigo));

            query = query.Where(c => queryCargaCTe.Any(a => a.CTe == c.CTE));

            return query.ToList();
        }

        public List<Dominio.Entidades.ContainerCTE> BuscarPorContainersEChaveAcesso(List<int> codigosContainers, List<string> chavesAcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ContainerCTE>();
            var result = query.Where(obj => codigosContainers.Contains(obj.Container.Codigo) && chavesAcesso.Contains(obj.CTE.Chave));
            return result.ToList();
        }

        public Dominio.Entidades.ContainerCTE BuscarPorContainerENota(int codigoContainer, string chaveNota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento>();
            var result = from obj in query where obj.ContainerCTE.Container.Codigo == codigoContainer && obj.Chave == chaveNota select obj;
            return result.Select(c => c.ContainerCTE)?.FirstOrDefault() ?? null;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Container> BuscarContainerPorCTe(List<int> codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ContainerCTE>();
            var result = from obj in query where codigoCTe.Contains(obj.CTE.Codigo) select obj;
            return result.Select(c => c.Container).Distinct().ToList();
        }

        public List<int> BuscarCodigoContainerPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ContainerCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe select obj;
            return result.Where(c => c.Container != null).Select(c => c.Container.Codigo).Distinct().ToList();
        }

        public int BuscarQuantidadeCTeContainer(int codigoContainer, string numeroBooking)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ContainerCTE>();
            query = query.Where(obj => obj.Container.Codigo == codigoContainer && obj.CTE.NumeroBooking == numeroBooking && (obj.CTE.Status == "A" || obj.CTE.Status == "P" || obj.CTE.Status == "E" || obj.CTE.Status == "S"));

            return query.Select(c => c.CTE).Distinct().Count();
        }

        public decimal BuscarPesoCTeContainer(string chaveCTe, string numeroContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ContainerCTE>();
            query = query.Where(obj => obj.CTE.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal && obj.CTE.Status == "A" && obj.CTE.NumeroBooking == numeroContainer);

            var queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ContainerCTE>();
            queryCTe = queryCTe.Where(obj => obj.CTE.Chave == chaveCTe);

            query = query.Where(obj => queryCTe.Any(c => c.Container.Codigo == obj.Container.Codigo));

            return query.Count() > 0 ? query?.Sum(o => o.CTE.Peso) ?? 0m : 0m;
        }

        public decimal BuscarPesoCTeContainer(string chaveCTe)
        {
            var queryInfoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            queryInfoCarga = queryInfoCarga.Where(c => c.UnidadeMedida == "01" && c.CTE.Chave == chaveCTe);

            return queryInfoCarga?.Sum(o => o.Quantidade) ?? 0m;
        }

        public decimal BuscarPesoNotasPorCTe(List<string> chaveCTe, string numeroContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento>();
            query = query.Where(obj => chaveCTe.Contains(obj.ContainerCTE.CTE.Chave));

            if (!string.IsNullOrWhiteSpace(numeroContainer))
                query = query.Where(obj => obj.ContainerCTE.Container.Numero == numeroContainer);

            var queryNotas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            queryNotas = queryNotas.Where(o => query.Any(p => p.Chave == o.Chave));

            return queryNotas.Count() > 0 ? queryNotas?.Sum(o => o.Peso) ?? 0m : 0m;
        }

        public decimal BuscarMetrosCubicosNotasPorCTe(List<string> chaveCTe, string numeroContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento>();
            query = query.Where(obj => chaveCTe.Contains(obj.ContainerCTE.CTE.Chave));

            if (!string.IsNullOrWhiteSpace(numeroContainer))
                query = query.Where(obj => obj.ContainerCTE.Container.Numero == numeroContainer);

            var queryNotas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            queryNotas = queryNotas.Where(o => query.Any(p => p.Chave == o.Chave));

            return queryNotas.Count() > 0 ? queryNotas?.Sum(o => o.MetrosCubicos) ?? 0m : 0m;
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE ContainerCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE ContainerCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
                                    .SetInt32("codigoCTe", codigoCTe)
                                    .ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }

        #endregion

        #region Relatório de Containers

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.Container> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioContainer filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaContainer().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.Container)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.Container>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioContainer filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consulta = new ConsultaContainer().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.NFeCTeContainer> ConsultarRelatorioNFeCTeContainer(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioNFeCTeContainer filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaNFeCTeContainer().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.NFeCTeContainer)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.NFeCTeContainer>();
        }

        public int ContarConsultaRelatorioNFeCTeContainer(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioNFeCTeContainer filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consulta = new ConsultaNFeCTeContainer().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
