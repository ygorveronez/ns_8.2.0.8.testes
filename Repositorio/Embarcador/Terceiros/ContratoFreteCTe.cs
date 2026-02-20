using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Terceiros
{
    public class ContratoFreteCTe : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe>
    {
        public ContratoFreteCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe BuscarCargaCTe(int codigo, int contrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe>();
            var resut = from obj in query where obj.CargaCTe.Codigo == codigo && obj.ContratoFrete.Codigo == contrato select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe> BuscarPorCargaFreteSubContratacao(int codigoCargaFreteSubContratacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe>();

            var result = from obj in query where obj.ContratoFrete.Codigo == codigoCargaFreteSubContratacao select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe> BuscarPorCargaFreteSubContratacao(int codigoCargaFreteSubContratacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe>();

            var result = from obj in query where obj.ContratoFrete.Codigo == codigoCargaFreteSubContratacao select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarPorCargaFreteSubContratacao(int codigoCargaFreteSubContratacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe>();

            var result = from obj in query where obj.ContratoFrete.Codigo == codigoCargaFreteSubContratacao select obj;

            return result.Count();
        }

        public int ContarPorCargaFreteSubContratacaoPendentesEnvio(int codigoCargaFreteSubContratacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe>();

            var result = from obj in query where obj.ContratoFrete.Codigo == codigoCargaFreteSubContratacao && obj.CTeTerceiro == null select obj;

            return result.Count();
        }
        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe> BuscarPorCargaCTe(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe>();

            var resut = from obj in query where obj.CargaCTe.Codigo == codigo select obj;

            return resut.ToList();
        }
        public void DeletarPorContratoFrete(int codigoContratoFrete)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE ContratoFreteCTe obj WHERE obj.ContratoFrete.Codigo = :codigoContratoFrete")
                                     .SetInt32("codigoContratoFrete", codigoContratoFrete)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE ContratoFreteCTe obj WHERE obj.ContratoFrete.Codigo = :codigoContratoFrete")
                                    .SetInt32("codigoContratoFrete", codigoContratoFrete)
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
                throw ex;
            }
        }
    }
}
