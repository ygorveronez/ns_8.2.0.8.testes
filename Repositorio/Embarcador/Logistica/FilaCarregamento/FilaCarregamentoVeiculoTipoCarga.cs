using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class FilaCarregamentoVeiculoTipoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoTipoCarga>
    {
        #region Construtores

        public FilaCarregamentoVeiculoTipoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoTipoCarga> BuscarPorFilaCarregamentoVeiculo(int codigoFilaCarregamentoVeiculo)
        {
            var consultaDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoTipoCarga>()
                .Where(destino => codigoFilaCarregamentoVeiculo == destino.FilaCarregamentoVeiculo.Codigo);

            return consultaDestino.ToList();
        }

        public List<(int CodigoFilaCarregamentoVeiculo, string TipoCarga)> BuscarPorFilasCarregamentoVeiculo(List<int> codigosFilaCarregamentoVeiculo)
        {
            var consultaTipoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoTipoCarga>()
                .Where(tipoCarga => codigosFilaCarregamentoVeiculo.Contains(tipoCarga.FilaCarregamentoVeiculo.Codigo));

            return consultaTipoCarga
                .Select(tipoCarga => ValueTuple.Create(tipoCarga.FilaCarregamentoVeiculo.Codigo, tipoCarga.TipoCarga.Descricao))
                .ToList();
        }

        #endregion Métodos Públicos
    }
}
