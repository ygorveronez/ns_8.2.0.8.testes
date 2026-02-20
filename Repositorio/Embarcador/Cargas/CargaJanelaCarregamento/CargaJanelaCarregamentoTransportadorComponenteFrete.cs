using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaCarregamentoTransportadorComponenteFrete : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete>
    {
        public CargaJanelaCarregamentoTransportadorComponenteFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete> BuscarPorCargaJanelaCarregamentoTransportador(int cargaJanelaCarregamentoTransportador)
        {
            var consultaComponenteFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete>()
                .Where(o => o.CargaJanelaCarregamentoTransportador.Codigo == cargaJanelaCarregamentoTransportador);

            return consultaComponenteFrete.ToList();
        }
        
        public List<int> BuscarPorCargaJanelaCarregamentoTransportadorPercentuais(int cargaJanelaCarregamentoTransportador)
        {
            var consultaComponenteFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete>()
                .Where(o => o.CargaJanelaCarregamentoTransportador.Codigo == cargaJanelaCarregamentoTransportador && o.Percentual > 0);

            return consultaComponenteFrete
                .Select(o => o.Codigo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete> BuscarPorCargasJanelaCarregamentoTransportador(List<int> cargasJanelaCarregamentoTransportador)
        {
            var consultaComponenteFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete>()
                .Where(o => cargasJanelaCarregamentoTransportador.Contains(o.CargaJanelaCarregamentoTransportador.Codigo));

            return consultaComponenteFrete.ToList();
        }

        public decimal BuscarValorPorCargaJanelaCarregamentoTransportador(int cargaJanelaCarregamentoTransportador)
        {
            var consultaComponenteFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete>()
                .Where(o => o.CargaJanelaCarregamentoTransportador.Codigo == cargaJanelaCarregamentoTransportador);

            return consultaComponenteFrete.Sum(o => (decimal?)o.ValorComponente) ?? 0m;
        }

        public void DeletarPorCargaJanelaCarregamentoTransportador(int cargaJanelaCarregamentoTransportador)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery("delete CargaJanelaCarregamentoTransportadorComponenteFrete where CargaJanelaCarregamentoTransportador.Codigo = :codigo")
                    .SetInt32("codigo", cargaJanelaCarregamentoTransportador)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                        .CreateQuery("delete CargaJanelaCarregamentoTransportadorComponenteFrete where CargaJanelaCarregamentoTransportador.Codigo = :codigo")
                        .SetInt32("codigo", cargaJanelaCarregamentoTransportador)
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
    }
}