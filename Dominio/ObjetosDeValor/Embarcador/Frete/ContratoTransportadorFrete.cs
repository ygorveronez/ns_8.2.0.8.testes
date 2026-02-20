using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class ContratoTransportadorFrete : IEquatable<ContratoTransportadorFrete>
    {
        #region Propriedades

        public int CodigoContrato { get; set; }

        public int CodigoTransportador { get; set; }

        public List<int> CodigosTabelaFrete { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosFiliais { get; set; }

        public List<int> CodigosModelosVeiculares { get; set; }

        public List<int> CodigosCanalEntrega { get; set; }

        #endregion Propriedades

        #region Métodos Públicos

        public bool Equals(ContratoTransportadorFrete other)
        {
            bool transportadorIgual = this.CodigoTransportador == other.CodigoTransportador;
            bool existeFilial = this.CodigosFiliais.Count > 0 ? this.CodigosFiliais.Any(f => other.CodigosFiliais.Contains(f)) : true;
            bool existeCanaisEntrega = this.CodigosCanalEntrega.Count > 0 ? this.CodigosCanalEntrega.Any(c => other.CodigosCanalEntrega.Contains(c)) : true;
            bool existeTabelaFrete = this.CodigosTabelaFrete.Count > 0 ? this.CodigosTabelaFrete.Any(c => other.CodigosTabelaFrete.Contains(c)) : true;
            bool existeModeloVeicular = this.CodigosModelosVeiculares.Any(c => other.CodigosModelosVeiculares.Contains(c));
            bool existeTipoCarga = this.CodigosTipoCarga.Count > 0 ? this.CodigosTipoCarga.Any(c => other.CodigosTipoCarga.Contains(c)) : true;
            bool matchContrato = transportadorIgual && existeFilial && existeCanaisEntrega && existeTabelaFrete && existeModeloVeicular && existeTipoCarga;

            return matchContrato;
        }

        public int ObterQuantidadeFiltrosCompativeis(ContratoTransportadorFrete other)
        {
            int quantidadeItensCompativeis = 0;

            if (this.CodigoTransportador == other.CodigoTransportador)
                quantidadeItensCompativeis++;

            if ((this.CodigosTabelaFrete.Count > 0) && this.CodigosTabelaFrete.Any(t => other.CodigosTabelaFrete.Contains(t)))
                quantidadeItensCompativeis++;

            if ((this.CodigosFiliais.Count > 0) && this.CodigosFiliais.Any(e => other.CodigosFiliais.Contains(e)))
                quantidadeItensCompativeis++;

            if ((this.CodigosTipoCarga.Count > 0) && this.CodigosTipoCarga.Any(e => other.CodigosTipoCarga.Contains(e)))
                quantidadeItensCompativeis++;

            if ((this.CodigosModelosVeiculares.Count > 0) && this.CodigosModelosVeiculares.Any(e => other.CodigosModelosVeiculares.Contains(e)))
                quantidadeItensCompativeis++;

            if ((this.CodigosCanalEntrega.Count > 0) && this.CodigosCanalEntrega.Any(c => other.CodigosCanalEntrega.Contains(c)))
                quantidadeItensCompativeis++;

            return quantidadeItensCompativeis;
        }

        #endregion Métodos Públicos
    }
}