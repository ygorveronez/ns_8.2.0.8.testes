using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FiltroPesquisaRelatorioQuantidadeDescarga
    {
        public int CodigoCentroDescarregamento { get; set; }

        public int CodigoModeloVeiculo { get; set; }

        public int CodigoOperador { get; set; }

        public int CodigoRota { get; set; }

        public int CodigoTransportador { get; set; }

        public int CodigoVeiculo { get; set; }

        public List<int> CodigosFilial { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public double CpfCnpjDestinatario { get; set; }

        public DateTime? DataFinal { get; set; }

        public DateTime? DataInicial { get; set; }

        public List<SituacaoCargaJanelaDescarregamento> Situacao { get; set; }

        public List<int> CodigosCentroDescarregamento { get; set; }

        public int TempoSemPosicaoParaVeiculoPerderSinal { get; set; }
    }
}
