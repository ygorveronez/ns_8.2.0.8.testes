using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Veiculos
{
    public class HistoricoVeiculoVinculoMotorista : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoMotorista>
    {
        public HistoricoVeiculoVinculoMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoMotorista BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoMotorista>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoMotorista BuscarPorVinculo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoMotorista>();
            var result = from obj in query where obj.HistoricoVeiculoVinculo.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoMotorista BuscarMotoristaPorVeiculo(int codigoVeiculo, DateTime dataOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoMotorista>();
            var result = from obj in query where obj.HistoricoVeiculoVinculo.Veiculo.Codigo == codigoVeiculo && obj.HistoricoVeiculoVinculo.DataHora <= dataOperacao select obj;
            return result.OrderByDescending(o => o.HistoricoVeiculoVinculo.DataHora).FirstOrDefault();
        }

    }
}
