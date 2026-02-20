using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class TipoCargaModeloVeicular : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular>
    {
        public TipoCargaModeloVeicular(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public TipoCargaModeloVeicular(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> ConsultarPorTipoCarga(int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular>();

            var result = from obj in query select obj;
            result = result.Where(tmv => tmv.TipoDeCarga.Codigo == codigoTipoCarga);

            return result
               .Fetch(tmv => tmv.ModeloVeicularCarga)
               .OrderBy(tmv => tmv.Posicao).ToList();

        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> ConsultarPorTipoCargaeModeloVeicularAtivo(int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular>();

            var result = from obj in query select obj;
            result = result.Where(tmv => tmv.TipoDeCarga.Codigo == codigoTipoCarga && tmv.TipoDeCarga.Ativo == true && tmv.ModeloVeicularCarga.Ativo == true);

            return result.OrderBy(tmv => tmv.Posicao).ToList();

        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> ConsultarPorTipoCargaeModeloVeicularPaletizadoAtivo(int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular>();

            var result = from obj in query select obj;
            result = result.Where(tmv => tmv.TipoDeCarga.Codigo == codigoTipoCarga && tmv.TipoDeCarga.Ativo && tmv.ModeloVeicularCarga.Ativo && tmv.ModeloVeicularCarga.VeiculoPaletizado);

            return result.OrderBy(tmv => tmv.Posicao).ToList();

        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular>> ConsultarPorTipoCargaeModeloVeicularPaletizadoAtivoAsync(int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular>();

            var result = from obj in query select obj;
            result = result.Where(tmv => tmv.TipoDeCarga.Codigo == codigoTipoCarga && tmv.TipoDeCarga.Ativo && tmv.ModeloVeicularCarga.Ativo && tmv.ModeloVeicularCarga.VeiculoPaletizado);

            return result.OrderBy(tmv => tmv.Posicao).ToListAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular ConsultarPrimeiroPorTipoDeCarga(int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular>();

            var result = from obj in query select obj;
            result = result.Where(tmv => tmv.TipoDeCarga.Codigo == codigoTipoCarga);

            return result.OrderBy(tmv => tmv.Posicao).FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular ConsultarPorModeloVeicular(int codigoTipoCarga, int codigoModeloVeicular)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular>();

            var result = from obj in query select obj;
            result = result.Where(tmv => tmv.TipoDeCarga.Codigo == codigoTipoCarga && tmv.ModeloVeicularCarga.Codigo == codigoModeloVeicular);
            return result.FirstOrDefault();

        }


        public List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> Consultar(int codigoTipoCarga, string descricao, decimal? capacidadePesoTransporte, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular>();

            var result = from obj in query
                         select obj;

            result = result.Where(tmv => tmv.TipoDeCarga.Codigo == codigoTipoCarga);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(mvc => mvc.ModeloVeicularCarga.Descricao.Contains(descricao));

            if (capacidadePesoTransporte.HasValue)
                result = result.Where(mvc => (mvc.ModeloVeicularCarga.CapacidadePesoTransporte + mvc.ModeloVeicularCarga.ToleranciaPesoExtra) >= capacidadePesoTransporte.Value
                    && (mvc.ModeloVeicularCarga.CapacidadePesoTransporte - mvc.ModeloVeicularCarga.ToleranciaPesoMenor) <= capacidadePesoTransporte.Value);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoTipoCarga, string descricao, decimal? capacidadePesoTransporte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular>();

            var result = from obj in query
                         select obj;
            result = result.Where(tmv => tmv.TipoDeCarga.Codigo == codigoTipoCarga);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(mvc => mvc.ModeloVeicularCarga.Descricao.Contains(descricao));

            if (capacidadePesoTransporte.HasValue)
                result = result.Where(mvc => (mvc.ModeloVeicularCarga.CapacidadePesoTransporte + mvc.ModeloVeicularCarga.ToleranciaPesoExtra) >= capacidadePesoTransporte.Value
                    && (mvc.ModeloVeicularCarga.CapacidadePesoTransporte - mvc.ModeloVeicularCarga.ToleranciaPesoMenor) <= capacidadePesoTransporte.Value);

            return result.Count();
        }

        public void DeletarPorTipoCarga(int codigoTipoCarga)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE TipoCargaModeloVeicular obj WHERE obj.TipoDeCarga.Codigo = :codigoTipoCarga")
                                     .SetInt32("codigoTipoCarga", codigoTipoCarga)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE TipoCargaModeloVeicular obj WHERE obj.TipoDeCarga.Codigo = :codigoTipoCarga")
                                .SetInt32("codigoTipoCarga", codigoTipoCarga)
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
