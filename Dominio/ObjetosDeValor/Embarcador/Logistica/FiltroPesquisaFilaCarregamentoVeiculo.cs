using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaFilaCarregamentoVeiculo
    {
        #region Campos Privados

        private bool _codigoTransportadorInformado;

        #endregion Campos Privados

        #region Propriedades

        public int CodigoCentroCarregamento { get; set; }

        public int CodigoFilaCarregamento { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoGrupoModeloVeicularCarga { get; set; }

        public List<int> CodigosCarga { get; set; }

        public List<int> CodigosConfiguracaoProgramacaoCarga { get; set; }

        public List<int> CodigosModeloVeicularCarga { get; set; }

        public List<int> CodigosDestino { get; set; }

        public List<int> CodigosRegiaoDestino { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public List<int> CodigosTransportador { get; set; }

        public List<int> CodigosVeiculo { get; set; }

        public double CodigoProprietarioVeiculo { get; set; }

        public DateTime? DataProgramadaFinal { get; set; }

        public DateTime? DataProgramadaInicial { get; set; }

        public List<string> SiglasEstadoDestino { get; set; }

        public List<Enumeradores.SituacaoFilaCarregamentoVeiculoPesquisa> SituacaoPesquisa { get; set; }

        public List<Enumeradores.SituacaoFilaCarregamentoVeiculo> Situacoes { get; set; }

        public Enumeradores.TipoFilaCarregamentoVeiculo? Tipo { get; set; }

        #endregion Propriedades

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

        #endregion Propriedades com Regras
    }
}
