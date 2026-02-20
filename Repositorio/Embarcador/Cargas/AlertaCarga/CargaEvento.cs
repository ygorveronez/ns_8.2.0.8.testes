using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.AlertaCarga
{
    public class CargaEvento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento>
    {
        #region Métodos públicos

        public CargaEvento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();

        }

        public List<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento> BuscarTodosEmAberto()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto);

            return result.ToList();

        }

        public List<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento>();
            var result = from obj in query select obj;
            result = result.Where(obj => obj.Carga.Codigo == carga);
            return result.ToList();
        }


        public Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento BuscarUltimoEmAbertoPorCarga(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga tipoAlertaCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto && ent.Carga.Codigo == carga && ent.TipoAlerta == tipoAlertaCarga);
            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento BuscarAlertaPorCargaChamado(int carga, int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto && ent.Chamado.Codigo == codigoChamado && ent.Carga.Codigo == carga);
            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento> BuscarPorCargaETipoAlerta(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga tipoAlerta, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento>();
            var result = from obj in query select obj;
            result = result.Where(obj => obj.Carga.Codigo == codigoCarga && obj.TipoAlerta == tipoAlerta).OrderBy(obj => obj.DataEvento);
            return ObterLista(result, parametrosConsulta);
        }


        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> BuscarUltimoAlertaPendenteObjetoDeValorPorTiposDeAlertaeCarga(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga> tiposAlerta)
        {
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga>();
            if (tiposAlerta != null && tiposAlerta.Count > 0)
            {

                string sql = $@"
                        select
	                        Alerta.ALC_CODIGO Codigo,
	                        Alerta.CAR_CODIGO CodigoCarga,
	                        Alerta.VEI_CODIGO CodigoVeiculo,
	                        Alerta.ALC_DATA_CADASTRO DataCadastro,
	                        Alerta.ALC_DATA_EVENTO Data,
	                        Alerta.ALC_TIPO TipoAlerta,
	                        Alerta.ALC_STATUS Status
                        from 
	                        T_CARGA_EVENTO Alerta
                        join
	                        (
		                        select 
			                        max(alc_codigo) ALC_CODIGO
		                        from 
			                        T_CARGA_EVENTO 
		                        group by 
			                        alc_codigo, car_codigo, vei_codigo
	                        ) UltimoAlerta on Alerta.ALC_CODIGO = UltimoAlerta.alc_codigo
                        where
                            Alerta.ALC_STATUS = 0
                            and Alerta.ALC_TIPO in ({string.Join(",", tiposAlerta.Select(t => (int)t).ToArray())})";

                var query = this.SessionNHiBernate.CreateSQLQuery(sql);
                query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga)));
                alertas = query.List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga>();
            }

            return alertas;
        }

        #endregion
    }
}
