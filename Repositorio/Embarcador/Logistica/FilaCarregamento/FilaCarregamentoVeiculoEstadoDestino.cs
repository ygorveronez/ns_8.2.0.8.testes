using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class FilaCarregamentoVeiculoEstadoDestino : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino>
    {
        #region Construtores

        public FilaCarregamentoVeiculoEstadoDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino> BuscarPorFilaCarregamentoVeiculo(int codigoFilaCarregamentoVeiculo)
        {
            var consultaDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino>()
                .Where(destino => codigoFilaCarregamentoVeiculo == destino.FilaCarregamentoVeiculo.Codigo);

            return consultaDestino.ToList();
        }

        public List<(int CodigoFilaCarregamentoVeiculo, string Estado)> BuscarPorFilasCarregamentoVeiculo(List<int> codigosFilaCarregamentoVeiculo)
        {
            var consultaEstadoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino>()
                .Where(estadoDestino => codigosFilaCarregamentoVeiculo.Contains(estadoDestino.FilaCarregamentoVeiculo.Codigo));

            return consultaEstadoDestino
                .Select(regiaoDestino => ValueTuple.Create(regiaoDestino.FilaCarregamentoVeiculo.Codigo, regiaoDestino.Estado.Nome))
                .ToList();
        }

        #endregion Métodos Públicos
    }
}
