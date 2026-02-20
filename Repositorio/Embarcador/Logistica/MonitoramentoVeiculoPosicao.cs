using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoVeiculoPosicao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao>
    {

        #region Atributos públicos

        public MonitoramentoVeiculoPosicao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos públicos

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao>();
            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao> BuscarTodosPorMonitoramento(int codigoMonitoramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao>();
            var result = from obj in query select obj;
            result = result.Where(ent => ent.MonitoramentoVeiculo.Monitoramento.Codigo == codigoMonitoramento);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao> BuscarTodosPorVeiculo(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao>();
            var result = from obj in query select obj;
            result = result.Where(ent => ent.MonitoramentoVeiculo.Veiculo.Codigo == codigoVeiculo);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao> BuscarTodosPorMonitoramentoVeiculo(int codigoMonitoramento, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao>();
            var result = from obj in query select obj;
            result = result.Where(ent => ent.MonitoramentoVeiculo.Monitoramento.Codigo == codigoMonitoramento && ent.MonitoramentoVeiculo.Veiculo.Codigo == codigoVeiculo);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao> BuscarAbertoPorMonitoramento(int codigoMonitoramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao>();
            var result = from obj in query select obj;
            result = result.Where(ent => ent.MonitoramentoVeiculo.Veiculo.Codigo == codigoMonitoramento && ent.MonitoramentoVeiculo.DataFim == null);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Posicao> BuscarPorMonitoramentoEVeiculoDataInicialeFinal(int codigoMonitoramento, int codigoVeiculo, DateTime dataInicial, DateTime dataFinal, string direcaoOrdenar = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao>();

            var result = from obj in query select obj;
            result = result.Where(ent => (ent.MonitoramentoVeiculo.Monitoramento.Codigo == codigoMonitoramento && ent.MonitoramentoVeiculo.Veiculo.Codigo == codigoVeiculo && ent.Posicao.DataVeiculo >= dataInicial && ent.Posicao.DataVeiculo <= dataFinal && ent.Posicao.Processar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado));
            result = result.OrderBy(ent => ent.Posicao.DataVeiculo);
            return result.Select(ent => ent.Posicao).ToList();

        }

        public bool ExistePosicoesPorMonitoramento(int codigoMonitoramento, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao>();
            var result = from obj in query select obj;
            result = result.Where(ent => ent.MonitoramentoVeiculo.Monitoramento.Codigo == codigoMonitoramento && ent.MonitoramentoVeiculo.Veiculo.Codigo == codigoVeiculo);
            return result.Count() > 0;
        }

        #endregion

    }
}
