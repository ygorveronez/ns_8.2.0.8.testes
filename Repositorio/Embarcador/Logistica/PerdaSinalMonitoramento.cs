using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PerdaSinalMonitoramento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PerdaSinalMonitoramento>
    {
        public PerdaSinalMonitoramento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.PerdaSinalMonitoramento BuscarUltimoPorMonitoramentoEmAberto(int codigoMonitoramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PerdaSinalMonitoramento>();
            query = query.Where(obj => obj.Monitoramento.Codigo == codigoMonitoramento && obj.DataFim == null);
            return query.OrderByDescending(obj => obj.DataInicio).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PerdaSinalMonitoramento> BuscarAlertasPerdaSinalPorveiculoSemDataRetroativa(int codigoVeiculo)
        {
            //buscar alertas que nao possuem posicao retroativa de pelo menos 4 dias atras;
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PerdaSinalMonitoramento>();
            query = query.Where(obj => obj.Veiculo.Codigo == codigoVeiculo && obj.AlertaPossuiPosicaoRetroativa == false && obj.DataInicio >= DateTime.Now.AddDays(-3));
            return query.OrderByDescending(obj => obj.DataInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PerdaSinalMonitoramento> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PerdaSinalMonitoramento>()
                .Where(obj => obj.AlertaMonitor.CargaEntrega.Carga.Codigo == codigoCarga);

            return query.ToList();
        }
    }
}
