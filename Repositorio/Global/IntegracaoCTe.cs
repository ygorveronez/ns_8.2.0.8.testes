using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Repositorio
{
    public class IntegracaoCTe : RepositorioBase<Dominio.Entidades.IntegracaoCTe>, Dominio.Interfaces.Repositorios.IntegracaoCTe
    {
        public IntegracaoCTe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public IntegracaoCTe(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.IntegracaoCTe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.IntegracaoCTe BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.IntegracaoCTe> BuscarIntegracoesFinalizadora(string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();

            var result = from obj in query where obj.FinalizarCarga.Equals("true") && obj.FinalizouCarga == false select obj;

            if (!string.IsNullOrWhiteSpace(statusCTe))
                result = result.Where(o => o.CTe.Status.Equals(statusCTe));

            return result.ToList();
        }

        public List<Dominio.Entidades.IntegracaoCTe> BuscarIntegracoesPendentes(int numeroCarga, int numeroUnidade, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();

            var result = from obj in query
                         where
                            obj.Tipo == Dominio.Enumeradores.TipoIntegracao.Emissao &&
                            obj.NumeroDaCarga == numeroCarga &&
                            obj.NumeroDaUnidade == numeroUnidade &&
                            obj.FinalizouCarga == false
                         select obj;

            if (!string.IsNullOrWhiteSpace(statusCTe))
                result = result.Where(o => o.CTe.Status.Equals(statusCTe));

            return result.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPendentes(int numeroCarga, int numeroUnidade, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();

            var result = from obj in query
                         where
                            obj.Tipo == Dominio.Enumeradores.TipoIntegracao.Emissao &&
                            obj.NumeroDaCarga == numeroCarga &&
                            obj.NumeroDaUnidade == numeroUnidade &&
                            obj.FinalizouCarga == false
                         select obj.CTe;

            if (!string.IsNullOrWhiteSpace(statusCTe))
                result = result.Where(o => o.Status.Equals(statusCTe));

            return result.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorCarga(int codigoEmpresa, int numeroDaUnidade, int numeroDaCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
            var result = from obj in query where obj.Tipo == Dominio.Enumeradores.TipoIntegracao.Emissao && obj.NumeroDaUnidade == numeroDaUnidade && obj.NumeroDaCarga == numeroDaCarga select obj.CTe;

            if (!string.IsNullOrWhiteSpace(statusCTe))
                result = result.Where(o => o.Status.Equals(statusCTe));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.ToList();

        }

        public Dominio.Entidades.IntegracaoCTe BuscarPorNumeroDaNotaFiscal(string cnpjEmitente, string numeroNotaFiscal, string serieNotaFiscal, Dominio.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.IntegracaoCTe>();

            criteria.CreateAlias("CTe", "cte");
            criteria.CreateAlias("cte.Empresa", "empresa");

            criteria.Add(Restrictions.Eq("empresa.CNPJ", cnpjEmitente));
            criteria.Add(Restrictions.Eq("Tipo", tipoIntegracao));

            var subCriteriaNF = NHibernate.Criterion.DetachedCriteria.ForEntityName("DocumentosCTE");

            subCriteriaNF.CreateAlias("CTE", "cteNF");
            subCriteriaNF.CreateAlias("cteNF.Empresa", "empresaNF");

            subCriteriaNF.Add(Restrictions.Eq("Numero", numeroNotaFiscal));
            subCriteriaNF.Add(Restrictions.Eq("Serie", serieNotaFiscal));
            subCriteriaNF.Add(Restrictions.Eq("empresaNF.CNPJ", cnpjEmitente));

            subCriteriaNF.SetProjection(Projections.Property("cteNF.Codigo"));

            criteria.Add(Subqueries.PropertyIn("cte.Codigo", subCriteriaNF));

            criteria.AddOrder(Order.Desc("Codigo"));

            criteria.SetMaxResults(1);

            return criteria.UniqueResult<Dominio.Entidades.IntegracaoCTe>();
        }

        public List<Dominio.Entidades.IntegracaoCTe> BuscarPorCTeETipo(int codigoCTe, Dominio.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();

            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;

            if (tipoIntegracao != Dominio.Enumeradores.TipoIntegracao.Todos)
                result = result.Where(o => o.Tipo == tipoIntegracao);

            return result.ToList();
        }

        public Dominio.Entidades.IntegracaoCTe BuscarPrimeiroPorCTeETipo(int codigoCTe, Dominio.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();

            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;

            if (tipoIntegracao != Dominio.Enumeradores.TipoIntegracao.Todos)
                result = result.Where(o => o.Tipo == tipoIntegracao);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.IntegracaoCTe BuscarPorCTeTipoStatus(int codigoCTe, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, Dominio.Enumeradores.StatusIntegracao status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();

            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.Status == status select obj;

            if (tipoIntegracao != Dominio.Enumeradores.TipoIntegracao.Todos)
                result = result.Where(o => o.Tipo == tipoIntegracao);

            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.IntegracaoCTe> BuscarPorCTeETipo(int codigoCTe, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, string cnpjEmpresaPai, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();

            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.Tipo == tipoIntegracao && obj.CTe.Empresa.EmpresaPai.CNPJ.Equals(cnpjEmpresaPai) && obj.CTe.Status.Equals(statusCTe) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.IntegracaoCTe> Buscar(int codigoEmpresaPai, int numeroCarga, int numeroUnidade, int quantidadeRegistros, Dominio.Enumeradores.StatusIntegracao statusIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
            var result = from obj in query where obj.CTe.Empresa.EmpresaPai.Codigo == codigoEmpresaPai && obj.Status == statusIntegracao select obj;

            if (numeroCarga > 0)
                result = result.Where(o => o.NumeroDaCarga == numeroCarga);

            if (numeroUnidade > 0)
                result = result.Where(o => o.NumeroDaUnidade == numeroUnidade);

            return result.OrderBy(o => o.CTe.Status).ThenBy(o => o.CTe.Codigo).Take(quantidadeRegistros).ToList();
        }

        public List<int> BuscarPendentesIntegracao(int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
            var result = from obj in query where obj.Tipo == Dominio.Enumeradores.TipoIntegracao.Emissao && obj.GerouCargaEmbarcador == false && obj.CTe.Status == "A" select obj;

            //return result.OrderBy(o => o.CTe.Codigo).Select(o => o.Codigo).ToList();
            if (maximoRegistros > 0)
                return result.OrderBy(o => o.CTe.Codigo).Select(o => o.Codigo).Take(maximoRegistros).ToList();
            else
                return result.OrderBy(o => o.CTe.Codigo).Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarIntegracaoCTesPendentes(int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
            var result = from obj in query where obj.Tipo == Dominio.Enumeradores.TipoIntegracao.Emissao && obj.Status == Dominio.Enumeradores.StatusIntegracao.AguardandoGeracaoCTe select obj;

            return result.OrderBy(o => o.Tentativas).ThenBy(o => o.Codigo).DistinctBy(o => o.CTe.Empresa.Codigo).Select(o => o.Codigo).Take(maximoRegistros).ToList();
        }

        public List<(int Codigo, int EmpresaCodigo)> BuscarIntegracaoCTesPendentesCodigos(int maximoRegistros)
        {
            return SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>()
                .Where(o => o.Tipo == Dominio.Enumeradores.TipoIntegracao.Emissao && o.Status == Dominio.Enumeradores.StatusIntegracao.AguardandoGeracaoCTe)
                .OrderBy(o => o.Tentativas)
                .ThenBy(o => o.Codigo)
                .Take(maximoRegistros)
                .Select(o => new { o.Codigo, EmpresaCodigo = o.CTe.Empresa.Codigo })
                .AsEnumerable()
                .Select(x => (x.Codigo, x.EmpresaCodigo))
                .ToList();
        }

        public Dictionary<int, int> BuscarEmpresaPorIntegracaoCodigos(IEnumerable<int> codigos)
        {
            return SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>()
                .Where(x => codigos.Contains(x.Codigo))
                .Select(x => new { x.Codigo, EmpresaId = x.CTe.Empresa.Codigo })
                .ToList()
                .ToDictionary(x => x.Codigo, x => x.EmpresaId);
        }

        public List<Dominio.Entidades.IntegracaoCTe> BuscarIntegracoesSemCarga()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
            var result = from obj in query where obj.Tipo == Dominio.Enumeradores.TipoIntegracao.Emissao && obj.GerouCargaEmbarcador == true && obj.CTe.Status == "A" select obj;

            var queryCargasCTes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            result = result.Where(o => !(from obj in queryCargasCTes where obj.CTe != null select obj.CTe.Codigo).Contains(o.CTe.Codigo));

            return result.OrderBy(o => o.CTe.Codigo).ToList();
        }

        public List<Dominio.Entidades.IntegracaoCTe> Buscar(int codigoEmpresa, int numeroCarga, int numeroUnidade, Dominio.Enumeradores.TipoIntegracao tipo, Dominio.Enumeradores.StatusIntegracao[] status, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();

            var result = from obj in query where obj.CTe.Empresa.Codigo == codigoEmpresa && status.Contains(obj.Status) && obj.Tipo == tipo && obj.CTe.Status.Equals(statusCTe) select obj;

            if (numeroCarga > 0)
                result = result.Where(o => o.NumeroDaCarga == numeroCarga);

            if (numeroUnidade > 0)
                result = result.Where(o => o.NumeroDaUnidade == numeroUnidade);

            return result.ToList();
        }

        public Dominio.Entidades.IntegracaoCTe BuscarPrimeiroRegistro(int codigoEmpresaPai, int numeroCarga, int numeroUnidade, Dominio.Enumeradores.TipoIntegracao tipo, Dominio.Enumeradores.StatusIntegracao[] status, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();

            var result = from obj in query where obj.CTe.Empresa.EmpresaPai.Codigo == codigoEmpresaPai && status.Contains(obj.Status) && obj.Tipo == tipo && obj.CTe.Status.Equals(statusCTe) select obj;

            if (numeroCarga > 0)
                result = result.Where(o => o.NumeroDaCarga == numeroCarga);

            if (numeroUnidade > 0)
                result = result.Where(o => o.NumeroDaUnidade == numeroUnidade);

            return result.FirstOrDefault();
        }

        public List<int> BuscarPorCargaPendentes(int numeroCarga, int numeroUnidade, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();
            var result = from obj in query
                         where obj.Tipo == Dominio.Enumeradores.TipoIntegracao.Emissao &&
                               obj.GerouCargaEmbarcador == false &&
                               obj.NumeroDaCarga == numeroCarga &&
                               obj.NumeroDaUnidade == numeroUnidade
                         select obj;

            if (maximoRegistros > 0)
                return result.OrderBy(o => o.CTe.Codigo).Select(o => o.Codigo).Take(maximoRegistros).ToList();
            else
                return result.OrderBy(o => o.CTe.Codigo).Select(o => o.Codigo).ToList();
        }


        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE IntegracaoCTe obj WHERE obj.CTe.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE IntegracaoCTe obj WHERE obj.CTe.Codigo = :codigoCTe")
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

    }
}
