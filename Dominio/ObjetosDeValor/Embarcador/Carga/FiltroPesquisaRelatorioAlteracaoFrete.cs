using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaRelatorioAlteracaoFrete
    {
        #region Atributos Privados

        public bool _codigoFilialInformado;
        public bool _codigoTipoCargaInformado;
        public bool _codigoTipoOperacaoInformado;
        public bool _codigoTransportadorInformado;

        #endregion

        #region Propriedades

        public string CodigoCargaEmbarcador { get; set; }
        public int CodigoModeloVeicularCarga { get; set; }
        public int CodigoOperador { get; set; }
        public List<int> CodigosFilial { get; set; }
        public List<int> CodigosTipoCarga { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public int CodigoVeiculo { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public SituacaoAlteracaoFreteCarga? SituacaoAlteracaoFrete { get; set; }
        public List<SituacaoCarga> Situacoes { get; set; }
        public bool UtilizarPercentualEmRelacaoValorFreteLiquidoCarga { get; set; }

        #endregion

        #region Propriedades com Regras

        public int CodigoTransportador
        {
            get
            {
                if (_codigoTransportadorInformado)
                    return CodigosTransportador.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoTransportadorInformado = true;
                    CodigosTransportador = new List<int>() { value };
                }
            }
        }

        public int CodigoFilial
        {
            get
            {
                if (_codigoFilialInformado)
                    return CodigosFilial.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoFilialInformado = true;
                    CodigosFilial = new List<int>() { value };
                }
            }
        }

        public int CodigoTipoCarga
        {
            get
            {
                if (_codigoTipoCargaInformado)
                    return CodigosTipoCarga.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoTipoCargaInformado = true;
                    CodigosTipoCarga = new List<int>() { value };
                }
            }
        }

        public int CodigoTipoOperacao
        {
            get
            {
                if (_codigoTipoOperacaoInformado)
                    return CodigosTipoOperacao.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoTipoOperacaoInformado = true;
                    CodigosTipoOperacao = new List<int>() { value };
                }
            }
        }

        #endregion
    }
}
