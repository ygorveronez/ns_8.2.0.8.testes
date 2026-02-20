using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    public class VeiculoCTE : RepositorioBase<Dominio.Entidades.VeiculoCTE>, Dominio.Interfaces.Repositorios.VeiculoCTE
    {
        public VeiculoCTE(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.VeiculoCTE BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.VeiculoCTE BuscarPorCodigoECTe(int codigo, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();
            var result = from obj in query where obj.Codigo == codigo && obj.CTE.Codigo == codigoCTe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.VeiculoCTE> BuscarPorCTe(int codigoEmpresa, int codigoCTe, string tipoVeiculo = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();
            var result = from obj in query where obj.CTE.Empresa.Codigo == codigoEmpresa && obj.CTE.Codigo == codigoCTe select obj;

            if (!string.IsNullOrWhiteSpace(tipoVeiculo))
                result = result.Where(o => o.Veiculo.TipoVeiculo.Equals(tipoVeiculo));

            return result.OrderBy(o => o.Codigo).ToList();
        }


        public List<Dominio.Entidades.VeiculoCTE> BuscarPorCTe(int codigoCTe, string tipoVeiculo = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>()
             .Fetch(o => o.CTE)
             .Fetch(o => o.Veiculo)
             .ThenFetch(obj => obj.TipoDoVeiculo);
            var result = query.Where(obj => obj.CTE.Codigo == codigoCTe);

            if (!string.IsNullOrWhiteSpace(tipoVeiculo))
                result = result.Where(o => o.Veiculo.TipoVeiculo == tipoVeiculo);

            return result.OrderBy(o => o.Codigo).ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.VeiculoDACTE> BuscarParaDACTE(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();
            var result = from obj in query
                         where obj.CTE.Empresa.Codigo == codigoEmpresa && obj.CTE.Codigo == codigoCTe
                         select new Dominio.ObjetosDeValor.Relatorios.VeiculoDACTE()
                         {
                             Codigo = obj.Codigo,
                             Placa = obj.Placa,
                             RNTRC = obj.Proprietario.RNTRC,
                             SiglaUF = obj.Estado.Sigla,
                             TipoVeiculo = obj.TipoVeiculo
                         };

            return result.ToList();
        }

        public List<Dominio.Entidades.VeiculoCTE> BuscarPorCTe(int codigoEmpresa, int[] codigosCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();
            var result = from obj in query where obj.CTE.Empresa.Codigo == codigoEmpresa && codigosCTes.Contains(obj.CTE.Codigo) select obj;

            return result.ToList();
        }

        public int ContarPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();
            var result = from obj in query where obj.CTE.Empresa.Codigo == codigoEmpresa && obj.CTE.Codigo == codigoCTe select obj.Codigo;
            return result.Count();
        }

        public int ContarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe select obj.Codigo;
            return result.Count();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE VeiculoCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE VeiculoCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
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

        public void DeletarPorCargaPedido(int codigoCargaPedido)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE VeiculoCTE obj WHERE obj.CTE IN (SELECT cargaPedidoDocumentoCTe.CTe FROM CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe WHERE cargaPedidoDocumentoCTe.CargaPedido.Codigo = :codigoCargaPedido)")
                                     .SetInt32("codigoCargaPedido", codigoCargaPedido)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE VeiculoCTE obj WHERE obj.CTE IN (SELECT cargaPedidoDocumentoCTe.CTe FROM CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe WHERE cargaPedidoDocumentoCTe.CargaPedido.Codigo = :codigoCargaPedido)")
                                     .SetInt32("codigoCargaPedido", codigoCargaPedido)
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

        public List<int> BuscarVeiculosSemValores()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

            var result = from obj in query where obj.Veiculo != null && (obj.Placa == null || obj.Placa == "") select obj.Codigo;

            return result.ToList();
        }

        public List<Dominio.Entidades.VeiculoCTE> BuscarPorCTe(int[] codigosCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

            var retorno = from obj in query where codigosCTes.Contains(obj.CTE.Codigo) select obj;

            return retorno.ToList();
        }
    }
}
