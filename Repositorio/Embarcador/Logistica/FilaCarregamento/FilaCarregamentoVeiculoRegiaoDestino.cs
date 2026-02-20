using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class FilaCarregamentoVeiculoRegiaoDestino : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoRegiaoDestino>
    {
        #region Construtores

        public FilaCarregamentoVeiculoRegiaoDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoRegiaoDestino> BuscarPorFilaCarregamentoVeiculo(int codigoFilaCarregamentoVeiculo)
        {
            var consultaDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoRegiaoDestino>()
                .Where(destino => codigoFilaCarregamentoVeiculo == destino.FilaCarregamentoVeiculo.Codigo);

            return consultaDestino.ToList();
        }

        public List<(int CodigoFilaCarregamentoVeiculo, string Regiao)> BuscarPorFilasCarregamentoVeiculo(List<int> codigosFilaCarregamentoVeiculo)
        {
            var consultaRegiaoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoRegiaoDestino>()
                .Where(regiaoDestino => codigosFilaCarregamentoVeiculo.Contains(regiaoDestino.FilaCarregamentoVeiculo.Codigo));

            return consultaRegiaoDestino
                .Select(regiaoDestino => ValueTuple.Create(regiaoDestino.FilaCarregamentoVeiculo.Codigo, regiaoDestino.Regiao.Descricao))
                .ToList();
        }

        #endregion Métodos Públicos
    }
}
