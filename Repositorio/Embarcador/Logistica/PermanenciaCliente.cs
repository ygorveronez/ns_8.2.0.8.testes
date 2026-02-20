using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PermanenciaCliente : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente>
    {
        public PermanenciaCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> BuscarPorCliente(Dominio.Entidades.Cliente cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente>();
            query = query.Where(obj => obj.Cliente == cliente);
            return query.OrderBy(obj => obj.DataInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente>();
            query = query.Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga);
            return query.Fetch(obj => obj.Cliente).Fetch(obj => obj.CargaEntrega).OrderBy(obj => obj.DataInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> BuscarPorCargas(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente>();
            query = query.Where(obj => codigosCarga.Contains(obj.CargaEntrega.Carga.Codigo));
            return query.Fetch(obj => obj.Cliente).Fetch(obj => obj.CargaEntrega).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente>();
            query = query.Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> BuscarPorCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente>();
            query = query.Where(obj => obj.CargaEntrega == cargaEntrega);
            return query.OrderBy(obj => obj.DataInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> BuscarPorClienteECargaEntrega(double codigoCliente, int codigoCargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente>();
            query = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega && obj.Cliente.CPF_CNPJ == codigoCliente);
            return query.OrderBy(obj => obj.DataInicio).ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente BuscarAbertaPorClienteECargaEntrega(double codigoCliente, int codigoCargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente>();
            query = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega && obj.Cliente.CPF_CNPJ == codigoCliente && obj.DataFim == null);
            return query.OrderBy(obj => obj.DataInicio).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente BuscarAbertaPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente>();
            query = query.Where(obj => obj.CargaEntrega.Carga == carga && obj.DataFim == null);
            return query.OrderBy(obj => obj.DataInicio).FirstOrDefault();
        }

        public DateTime BuscarDataUltimaSaidaDoClienteECarga(double codigoCliente, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente>();
            query = query.Where(obj => obj.Cliente.CPF_CNPJ == codigoCliente && obj.DataFim != null && obj.CargaEntrega.Carga.Codigo == codigoCarga);
            var result = query.OrderByDescending(obj => obj.DataFim).FirstOrDefault();
            return result?.DataFim ?? DateTime.MinValue;
        }

        public void ExcluirTodosPorCargaEntrega(int codigoCargaEntrega)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE PermanenciaCliente obj WHERE obj.CargaEntrega IN (:cargaEntrega)")
                             .SetInt32("cargaEntrega", codigoCargaEntrega)
                             .ExecuteUpdate();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaCliente> BuscarObjetoDeValorPorCarga(int codigoCarga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaCliente> result = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaCliente>();

            string sql = @"Select
	                            PermanenciaCliente.CEN_CODIGO CodigoCargaEntrega,
	                            PermanenciaCliente.CLI_CGCCPF CodigoCliente,
	                            PermanenciaCliente.PCL_DATA_INICIO DataInicio,
	                            PermanenciaCliente.PCL_DATA_FIM DataFim,
	                            PermanenciaCliente.PCL_TEMPO_SEGUNDOS TempoSegundos
                            From
	                            T_CARGA_ENTREGA CargaEntrega
	                            Join T_PERMANENCIA_CLIENTE PermanenciaCliente On PermanenciaCliente.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                            Where
	                            CargaEntrega.CAR_CODIGO = :codigoCarga
                            Order By
                                CargaEntrega.CEN_CODIGO,
                                PermanenciaCliente.PCL_CODIGO";

            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetParameter("codigoCarga", codigoCarga);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaCliente)));
            return consulta.List<Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaCliente>();
        }

    }
}
