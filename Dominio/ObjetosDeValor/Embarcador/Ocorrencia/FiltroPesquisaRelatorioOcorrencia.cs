using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public sealed class FiltroPesquisaRelatorioOcorrencia
    {
        public string CodigoCargaEmbarcador { get; set; }

        public List<int> CodigosFilial { get; set; }

        public int CodigoGrupoPessoas { get; set; }

        public string CargaAgrupada { get; set; }

        public int CodigoMotorista { get; set; }

        public List<int> CodigosOcorrencia { get; set; }

        public int CodigoRecebedor { get; set; }

        public int CodigoResponsavelChamado { get; set; }

        public List<int> CodigoSolicitante { get; set; }

        public int CodigoTransportadorChamado { get; set; }

        public int CodigoVeiculo { get; set; }

        public List<int> CodigosTransportadorCarga { get; set; }

        public double CpfCnpjPessoa { get; set; }

        public DateTime? DataCancelamentoFinal { get; set; }

        public DateTime? DataCancelamentoInicial { get; set; }

        public DateTime? DataOcorrenciaFinal { get; set; }

        public DateTime? DataOcorrenciaInicial { get; set; }

        public DateTime? DataSolicitacaoFinal { get; set; }

        public DateTime? DataSolicitacaoInicial { get; set; }

        public int NumeroCTeOriginal { get; set; }

        public int NumeroCTeGerado { get; set; }

        public int NumeroNotaFiscal { get; set; }

        public string NumeroOcorrenciaCliente { get; set; }

        public int NumeroOcorrenciaFinal { get; set; }

        public int NumeroOcorrenciaInicial { get; set; }

        public List<SituacaoOcorrencia> SituacoesCancelamento { get; set; }

        public List<SituacaoOcorrencia> SituacoesOcorrencia { get; set; }

        public decimal ValorFinal { get; set; }

        public decimal ValorInicial { get; set; }

        public List<int> TiposOperacaoCarga { get; set; }

        public TipoDocumentoCreditoDebito TipoDocumentoCreditoDebito { get; set; }

        public Dominio.Enumeradores.TipoDocumento TipoDocumentoEmissao { get; set; }

        public bool? OcorrenciaEstadia { get; set; }

        public List<TipoCargaEntrega> EtapaEstadia { get; set; }

        public List<double> Recebedores { get; set; }

        public List<int> CodigoGrupoOcorrencia { get; set; }
    }
}
