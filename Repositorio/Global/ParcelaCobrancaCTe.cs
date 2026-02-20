using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ParcelaCobrancaCTe : RepositorioBase<Dominio.Entidades.ParcelaCobrancaCTe>, Dominio.Interfaces.Repositorios.ParcelaCobrancaCTe
    {
        public ParcelaCobrancaCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ParcelaCobrancaCTe BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();
            var result = from obj in query where obj.Codigo == codigo && obj.Cobranca.CTe.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ParcelaCobrancaCTe> BuscarPorListaDeCodigo(int codigoEmpresa, List<int> listaCodigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();
            var result = from obj in query where listaCodigos.Contains(obj.Codigo) && obj.Cobranca.CTe.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToList();
        }

        public Dominio.Entidades.ParcelaCobrancaCTe BuscarPorCodigoECobranca(int codigo, int codigoCobranca)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();
            var result = from obj in query where obj.Codigo == codigo && obj.Cobranca.Codigo == codigoCobranca select obj;
            return result.FirstOrDefault();
        }

        public int ContarPorCobranca(int codigoCobranca)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();
            var result = from obj in query where obj.Cobranca.Codigo == codigoCobranca select obj.Codigo;
            return result.Count();
        }

        public List<Dominio.Entidades.ParcelaCobrancaCTe> BuscarPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();
            var result = from obj in query where obj.Cobranca.CTe.Codigo == codigoCTe && obj.Cobranca.CTe.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.ParcelaCobrancaCTe> Consultar(int codigoEmpresa, int codigoCTe, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.StatusDuplicata status, double cpfCnpjCliente, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();
            var result = from obj in query where obj.Cobranca.CTe.Empresa.Codigo == codigoEmpresa && obj.Status == status && obj.Cobranca.CTe.Status.Equals("A") select obj;

            if (codigoCTe > 0)
                result = result.Where(o => o.Cobranca.CTe.Codigo == codigoCTe);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataVencimento >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataVencimento < dataFinal.AddDays(1).Date);

            if (cpfCnpjCliente > 0)
                result = result.Where(o => o.Cobranca.Cliente.CPF_CNPJ == cpfCnpjCliente);

            return result.OrderBy(o => o.Cobranca.Numero)
                         .ThenBy(o => o.Numero)
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)
                         .ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int codigoCTe, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.StatusDuplicata status, double cpfCnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();
            var result = from obj in query where obj.Cobranca.CTe.Empresa.Codigo == codigoEmpresa && obj.Status == status && obj.Cobranca.CTe.Status.Equals("A") select obj;

            if (codigoCTe > 0)
                result = result.Where(o => o.Cobranca.CTe.Codigo == codigoCTe);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataVencimento >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataVencimento < dataFinal.AddDays(1).Date);

            if (cpfCnpjCliente > 0)
                result = result.Where(o => o.Cobranca.Cliente.CPF_CNPJ == cpfCnpjCliente);

            return result.Count();
        }

        public int ContarParcelasDoCTe(int codigoCTe, Dominio.Enumeradores.StatusDuplicata status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaCobrancaCTe>();

            var result = from obj in query where obj.Status == status && obj.Cobranca.CTe.Codigo == codigoCTe select obj.Codigo;

            return result.Count();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE ParcelaCobrancaCTe obj WHERE obj.Cobranca.Codigo IN (SELECT cob.Codigo FROM CobrancaCTe cob WHERE cob.CTe.Codigo = :codigoCTe)")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE ParcelaCobrancaCTe obj WHERE obj.Codigo IN (SELECT cob.Codigo FROM CobrancaCTe cob WHERE cob.CTe.Codigo = :codigoCTe)")
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
