using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    public class ComponentePrestacaoCTE : RepositorioBase<Dominio.Entidades.ComponentePrestacaoCTE>, Dominio.Interfaces.Repositorios.ComponentePrestacaoCTE
    {
        public ComponentePrestacaoCTE(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.ComponentePrestacaoCTE> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>();

            var retorno = from obj in query where obj.CTE.Codigo == codigoCTe select obj;

            return retorno.ToList();
        }

        public List<Dominio.Entidades.ComponentePrestacaoCTE> BuscarPorCTe(IEnumerable<int> codigosCTes)
        {
            IQueryable<Dominio.Entidades.ComponentePrestacaoCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = codigosCTes.Count() / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.ComponentePrestacaoCTE> componentesCTeRetornar = new List<Dominio.Entidades.ComponentePrestacaoCTE>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                componentesCTeRetornar.AddRange(query.Where(o => codigosCTes.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.CTE.Codigo)).Fetch(o => o.CTE).ToList());

            return componentesCTeRetornar;
        }

        public decimal ObterSomaValoresComponentesPorCTe(IEnumerable<int> codigosCTEs, string[] tipoValoresIgnorar = null)
        {
            try
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>();
                var queryFiltrada = query.Where(obj => codigosCTEs.Contains(obj.CTE.Codigo));

                if (tipoValoresIgnorar != null && tipoValoresIgnorar.Length > 0)
                {
                    var tiposIgnorarLower = tipoValoresIgnorar.Select(t => t.ToLower()).ToArray();
                    queryFiltrada = queryFiltrada.Where(obj => !tiposIgnorarLower.Contains(obj.NomeCTe.ToLower()));
                }
            
                return queryFiltrada.Sum(obj => obj.Valor);
            }
            catch (InvalidOperationException)
            {
                return 0m;
            }
        }

        public List<Dominio.Entidades.ComponentePrestacaoCTE> BuscarPorCTeDescricao(int codigoCTe, string descricaoComponente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>();

            var retorno = from obj in query where obj.CTE.Codigo == codigoCTe && obj.NomeCTe.Equals(descricaoComponente) select obj;

            return retorno.ToList();
        }

        public List<Dominio.Entidades.ComponentePrestacaoCTE> BuscarPorCTeTipo(int codigoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>();

            var retorno = from obj in query where obj.CTE.Codigo == codigoCTe && obj.ComponenteFrete != null && (obj.ComponenteFrete.TipoComponenteFrete == tipoComponente || obj.ComponenteFrete.TipoComponenteFreteDOCCOB == tipoComponente) select obj;

            return retorno.ToList();
        }

        public decimal BuscarValorPorCTeTipo(int codigoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>();

            var retorno = from obj in query where obj.CTE.Codigo == codigoCTe && obj.ComponenteFrete != null && (obj.ComponenteFrete.TipoComponenteFrete == tipoComponente || obj.ComponenteFrete.TipoComponenteFreteDOCCOB == tipoComponente) select obj;

            return retorno?.Sum(c => (decimal?)c.Valor) ?? 0m;
        }

        public Dominio.Entidades.ComponentePrestacaoCTE BuscarPrimeiroPorCTeDescricao(int codigoCTe, string descricaoComponente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>();

            var retorno = from obj in query where obj.CTE.Codigo == codigoCTe && obj.NomeCTe.Equals(descricaoComponente) select obj;

            return retorno.FirstOrDefault();
        }

        public List<Dominio.Entidades.ComponentePrestacaoCTE> BuscarPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>();
            var retorno = from obj in query where obj.CTE.Codigo == codigoCTe && obj.CTE.Empresa.Codigo == codigoEmpresa select obj;
            return retorno.OrderBy(o => o.NomeCTe).ToList();
        }

        public List<Dominio.Entidades.ComponentePrestacaoCTE> BuscarInformadosPorCTe(int codigoCTe)
        {
            var consultaComponentePrestacaoCTE = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>()
                .Where(o => o.CTE.Codigo == codigoCTe && !o.NomeCTe.Contains("FRETE VALOR") && !o.NomeCTe.Contains("IMPOSTOS"));

            return consultaComponentePrestacaoCTE.ToList();
        }

        public List<Dominio.Entidades.ComponentePrestacaoCTE> BuscarInformadosPorCTes(List<int> codigosCTe, bool verificarComponenteFreteValorComOutraDescricao)
        {
            List<Dominio.Entidades.ComponentePrestacaoCTE> componentesPrestacaoCTE = new List<Dominio.Entidades.ComponentePrestacaoCTE>();
            int limiteRegistros = 1000;
            int inicio = 0;

            while (inicio < codigosCTe?.Count)
            {
                List<int> codigosCTePaginado = codigosCTe.Skip(inicio).Take(limiteRegistros).ToList();
                var consultaComponentePrestacaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>()
                    .Where(o => codigosCTePaginado.Contains(o.CTE.Codigo));

                if (!verificarComponenteFreteValorComOutraDescricao)
                    consultaComponentePrestacaoCTe = consultaComponentePrestacaoCTe.Where(o => !o.NomeCTe.Contains("FRETE VALOR") && !o.NomeCTe.Contains("IMPOSTOS"));

                componentesPrestacaoCTE.AddRange(consultaComponentePrestacaoCTe.Fetch(o => o.CTE).ToList());

                inicio += limiteRegistros;
            }

            return componentesPrestacaoCTE;
        }

        public List<Dominio.Entidades.ComponentePrestacaoCTE> BuscarInformadosPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>();
            var retorno = from obj in query where obj.CTE.Codigo == codigoCTe && obj.CTE.Empresa.Codigo == codigoEmpresa && !obj.NomeCTe.Contains("FRETE VALOR") && !obj.NomeCTe.Contains("IMPOSTOS") select obj;
            return retorno.ToList();
        }

        public List<Dominio.Entidades.ComponentePrestacaoCTE> BuscarPorCTe(int codigoEmpresa, int[] codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>();
            var retorno = from obj in query where codigoCTe.Contains(obj.CTE.Codigo) && obj.CTE.Empresa.Codigo == codigoEmpresa select obj;
            return retorno.ToList();
        }

        public List<Dominio.Entidades.ComponentePrestacaoCTE> BuscarPorCTe(int[] codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>();
            var retorno = from obj in query where codigoCTe.Contains(obj.CTE.Codigo) select obj;
            return retorno.ToList();
        }

        public Dominio.Entidades.ComponentePrestacaoCTE BuscarPorCodigoECTe(int codigoCTe, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>();
            var retorno = from obj in query where obj.CTE.Codigo == codigoCTe && obj.Codigo == codigo select obj;
            return retorno.FirstOrDefault();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE ComponentePrestacaoCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE ComponentePrestacaoCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
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
