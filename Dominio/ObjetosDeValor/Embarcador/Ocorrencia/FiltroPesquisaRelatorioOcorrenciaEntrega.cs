using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public sealed class FiltroPesquisaRelatorioOcorrenciaEntrega
    {
        public DateTime? DataCriacaoInicial { get; set;  }
        public DateTime? DataCriacaoFinal { get; set; }
        public List<SituacaoOcorrencia> SituacoesOcorrencia { get; set; }
        public List<SituacaoOcorrencia> SituacoesCancelamento { get; set; }
        public List<int> CodigosOcorrencia { get; set; }
        public List<int> CodigosGrupoOcorrencia { get; set; }
        public int NumeroOcorrenciaFinal { get; set; }
        public int NumeroOcorrenciaInicial { get; set; }
        public int TipoPessoa { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public string CargaAgrupada { get; set; }
        public List<int> CodigosFilial { get; set; }
        public int CodigoVeiculo { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public int CodigoMotorista { get; set; }
        public List<int> CodigoSolicitante { get; set; }
        public int CodigoOperador { get; set; }
        public int CodigoTransportadorChamado { get; set; }
        public List<int> TiposOperacaoCarga { get; set; }
        public int CodigoRecebedor { get; set; }
        public List<int> CodigosTransportadorCarga { get; set; }
        public double CpfCnpjPessoa { get; set; }


    }
}
