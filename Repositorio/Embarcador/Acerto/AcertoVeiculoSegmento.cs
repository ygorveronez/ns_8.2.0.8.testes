using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoVeiculoSegmento : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento>
    {
        public AcertoVeiculoSegmento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento> BuscarPorAcerto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento BuscarPorAcertoEVeiculo(int codigoAcerto, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.Veiculo.Codigo == codigoVeiculo select obj;
            return result.FirstOrDefault();
        }

        public void DeletarPorAcerto(int codigoAcerto)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM AcertoVeiculoSegmento c WHERE c.AcertoViagem.Codigo = :codigoAcerto").SetInt32("codigoAcerto", codigoAcerto).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM AcertoVeiculoSegmento c WHERE c.AcertoViagem.Codigo = :codigoAcerto").SetInt32("codigoAcerto", codigoAcerto).ExecuteUpdate();

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
