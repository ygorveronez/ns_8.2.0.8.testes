using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;

namespace Repositorio
{
    public class InformacaoCargaCTE : RepositorioBase<Dominio.Entidades.InformacaoCargaCTE>, Dominio.Interfaces.Repositorios.InformacaoCargaCTE
    {
        public InformacaoCargaCTE(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.InformacaoCargaCTE> BuscarPorCTe(int CodigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            var result = from obj in query where obj.CTE.Codigo == CodigoCTe select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.InformacaoCargaCTE> BuscarPorCTe(IEnumerable<int> codigosCTes)
        {
            IQueryable<Dominio.Entidades.InformacaoCargaCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            
            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = codigosCTes.Count() / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.InformacaoCargaCTE> informacoesCargaCTeRetornar = new List<Dominio.Entidades.InformacaoCargaCTE>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                informacoesCargaCTeRetornar.AddRange(query.Where(o => codigosCTes.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.CTE.Codigo)).Fetch(o => o.CTE).ToList());

            return informacoesCargaCTeRetornar;
        }

        public List<Dominio.Entidades.InformacaoCargaCTE> BuscarPorCTe(int codigoEmpresa, int CodigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            var result = from obj in query where obj.CTE.Codigo == CodigoCTe && obj.CTE.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToList();
        }

        public Dominio.Entidades.InformacaoCargaCTE BuscarPorCTeUnidade(int CodigoCTe, string unidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            var result = from obj in query where obj.CTE.Codigo == CodigoCTe && obj.UnidadeMedida == unidade select obj;
            return result.FirstOrDefault();
        }

        public decimal BuscarPorChaveCTeUnidade(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.Chave == chave select obj;
            return result.Sum(o => (decimal?)o.Peso) ?? 0m;
        }

        public decimal BuscarPorChaveCTeUnidade(List<string> chaves, string unidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            var result = from obj in query where chaves.Contains(obj.CTE.Chave) && obj.UnidadeMedida == unidade select obj;
            return result.Sum(o => (decimal?)o.Quantidade) ?? 0m;
        }

        public List<Dominio.Entidades.InformacaoCargaCTE> BuscarPorCTe(int codigoEmpresa, int[] CodigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            var result = from obj in query where CodigoCTe.Contains(obj.CTE.Codigo) && obj.CTE.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.InformacaoCargaCTE> BuscarPorCTe(int[] CodigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            var result = from obj in query where CodigoCTe.Contains(obj.CTE.Codigo) select obj;
            return result.ToList();
        }

        public int ContarPorCTe(int codigoEmpresa, int CodigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            var result = from obj in query where obj.CTE.Codigo == CodigoCTe && obj.CTE.Empresa.Codigo == codigoEmpresa select obj.Codigo;
            return result.Count();
        }

        public int ContarPorCTe(int CodigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            var result = from obj in query where obj.CTE.Codigo == CodigoCTe select obj.Codigo;
            return result.Count();
        }

        public Dominio.Entidades.InformacaoCargaCTE BuscarPorCTeECodigo(int codigoCTe, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe && obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public decimal ObterPesoTotal(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            var result = (from obj in query where obj.CTE.Codigo == codigoCTe select (decimal?)obj.Quantidade).Sum() ?? 0m;
            return result;
        }

        public decimal ObterPesoKg(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            var pesoKgTotal = (from obj in query where obj.CTE.Codigo == codigoCTe && obj.UnidadeMedida == "01" select (decimal?)obj.Quantidade).Sum() ?? 0m;

            return pesoKgTotal;
        }

        public decimal ObterPesoTon(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            var pesoKgTotal = (from obj in query where obj.CTE.Codigo == codigoCTe && obj.UnidadeMedida == "02" select (decimal?)obj.Quantidade).Sum() ?? 0m;

            return pesoKgTotal;
        }

        public decimal ObterPesoKg(List<int> codigosCTes)
        {
            IQueryable<Dominio.Entidades.InformacaoCargaCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();

            query = query.Where(obj => codigosCTes.Contains(obj.CTE.Codigo) && obj.UnidadeMedida == "01");

            return query.Sum(o => (decimal?)o.Quantidade) ?? 0m;
        }

        public int ObterQuantidadeUnitaria(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            var quantidade = (from obj in query where obj.CTE.Codigo == codigoCTe && obj.UnidadeMedida == "03" select (int?)obj.Quantidade).Sum() ?? 0;

            return quantidade;
        }

        public string ObterUnidade(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            var result = (from obj in query where obj.CTE.Codigo == codigoCTe select obj.UnidadeMedida).FirstOrDefault();
            return result;
        }

        public decimal ObterPesoTotal(int[] codigoCTes, Dominio.Enumeradores.UnidadeMedidaMDFe unidadeMedida)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();
            decimal result = 0;
            if (unidadeMedida == Dominio.Enumeradores.UnidadeMedidaMDFe.KG)
                result = (from obj in query where codigoCTes.Contains(obj.CTE.Codigo) && obj.UnidadeMedida == "01" select (decimal?)obj.Quantidade).Sum() ?? 0m;
            else if (unidadeMedida == Dominio.Enumeradores.UnidadeMedidaMDFe.TON)
                result = (from obj in query where codigoCTes.Contains(obj.CTE.Codigo) && obj.UnidadeMedida == "02" select (decimal?)obj.Quantidade).Sum() ?? 0m;
            return result;
        }

        public List<Dominio.Entidades.InformacaoCargaCTE> ObterSumarizadosPorCTes(IEnumerable<int> codigoCTes)
        {
            IQueryable<Dominio.Entidades.InformacaoCargaCTE> query = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();

            query = query.Where(o => codigoCTes.Contains(o.CTE.Codigo));

            return query.
                GroupBy(o => new
                {
                    o.UnidadeMedida,
                    o.Tipo
                }).
                Select(o => new Dominio.Entidades.InformacaoCargaCTE()
                {
                    Tipo = o.Key.Tipo,
                    UnidadeMedida = o.Key.UnidadeMedida,
                    Quantidade = o.Sum(c => c.Quantidade)
                }).ToList();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE InformacaoCargaCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE InformacaoCargaCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
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
