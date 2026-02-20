using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Global
{
    public class EmpresaIntelipostTipoOcorrencia : RepositorioBase<Dominio.Entidades.EmpresaIntelipostTipoOcorrencia>
    {
        public EmpresaIntelipostTipoOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.EmpresaIntelipostTipoOcorrencia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntelipostTipoOcorrencia>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();

        }

        public Dominio.Entidades.EmpresaIntelipostTipoOcorrencia BuscarPorEmpresaECodigoIntegracao(int codigoEmpresa, string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntelipostTipoOcorrencia>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Empresa.Codigo == codigoEmpresa && ent.CodigoIntegracao == codigoIntegracao);

            return result.FirstOrDefault();
        }


        public Dominio.Entidades.EmpresaIntelipostTipoOcorrencia BuscarPorTipoOcorrencia(int codigoEmpresa, int codigoTipoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntelipostTipoOcorrencia>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.TipoOcorrencia.Codigo == codigoTipoOcorrencia && ent.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }


        private IQueryable<Dominio.Entidades.EmpresaIntelipostTipoOcorrencia> Consultar(string descricao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntelipostTipoOcorrencia>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(descricao));


            return consulta;
        }

        public List<Dominio.Entidades.EmpresaIntelipostTipoOcorrencia> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(descricao);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(string descricao)
        {
            var consulta = Consultar(descricao);

            return consulta.Count();
        }


        public void DeletarPorEmpresa(int codigoEmpresa)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM EmpresaIntelipostTipoOcorrencia c WHERE c.Empresa.Codigo = :codigoEmpresa").SetInt32("codigoEmpresa", codigoEmpresa).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM EmpresaIntelipostTipoOcorrencia c WHERE c.Empresa.Codigo = :codigoEmpresa").SetInt32("codigoEmpresa", codigoEmpresa).ExecuteUpdate();

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





