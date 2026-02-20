using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class FilaCarregamentoVeiculoDestino : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoDestino>
    {
        #region Construtores

        public FilaCarregamentoVeiculoDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoDestino> BuscarPorFilaCarregamentoVeiculo(int codigoFilaCarregamentoVeiculo)
        {
            var consultaDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoDestino>()
                .Where(destino => codigoFilaCarregamentoVeiculo == destino.FilaCarregamentoVeiculo.Codigo);

            return consultaDestino.ToList();
        }

        public List<(int CodigoFilaCarregamentoVeiculo, string Destino)> BuscarPorFilasCarregamentoVeiculo(List<int> codigosFilaCarregamentoVeiculo)
        {
            var consultaDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoDestino>()
                .Where(destino => codigosFilaCarregamentoVeiculo.Contains(destino.FilaCarregamentoVeiculo.Codigo));

            return consultaDestino
                .Select(regiaoDestino => ValueTuple.Create(regiaoDestino.FilaCarregamentoVeiculo.Codigo, regiaoDestino.Localidade.Descricao))
                .ToList();
        }

        #endregion Métodos Públicos
    }
}
