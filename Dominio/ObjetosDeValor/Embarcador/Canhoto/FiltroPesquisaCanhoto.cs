using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Canhoto
{
    public class FiltroPesquisaCanhoto
    {
        public int CodigoCTe { get; set; }
        public List<int> CodigosConhecimentos { get; set; }
        public List<int> CodigosCanhotos { get; set; }
        public TipoCanhoto? TipoCanhoto { get; set; }
        public SituacaoCanhoto? Situacao { get; set; }
        public List<SituacaoCanhoto> Situacoes { get; set; }
        public SituacaoDigitalizacaoCanhoto? SituacaoDigitalizacaoCanhoto { get; set; }
        public List<SituacaoDigitalizacaoCanhoto> SituacoesDigitalizacaoCanhoto { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada? TipoNotaFiscalIntegrada { get; set; }
        public List<TipoRejeicaoPelaIA> TipoRejeicaoPelaIA { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? TipoSituacaoIA { get; set; }
        public List<int> CodigosCargaEmbarcador { get; set; }
        public int Carga { get; set; }
        public int Transportador { get; set; }
        public int Motorista { get; set; }
        public int NumeroNFe { get; set; }
        public int NumeroCanhoto { get; set; }
        public double Pessoa { get; set; }
        public int GrupoPessoa { get; set; }
        public int Numero { get; set; }
        public string Chave { get; set; }
        public List<int> Numeros { get; set; }
        public CanhotoOrigemDigitalizacao OrigemDigitalizacao { get; set; }
        public int Filial
        {
            set { if (value > 0) Filiais = new List<int>() { value }; }
        }
        public List<int> Filiais { get; set; }

        public int Empresa { get; set; }
        public List<int> Empresas { get; set; }
        public double Terceiro { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public DateTime? DataInicioDigitalizacao { get; set; }
        public DateTime? DataFimDigitalizacao { get; set; }
        public DateTime? DataCriacaoCargaInicial { get; set; }
        public DateTime? DataCriacaoCargaFinal { get; set; }
        public int CodigoLocalArmazenamento { get; set; }
        public int Pacote { get; set; }
        public int Posicao { get; set; }
        public int Serie { get; set; }
        public int? Malote { get; set; }
        public bool SemMalote { get; set; }
        public bool ObrigatorioFilial { get; set; }
        public List<int> TiposCarga { get; set; }
        public List<int> TiposOperacao { get; set; }
        public double Recebedor { get; set; }
        public List<double> Recebedores { get; set; }
        public List<double> Destinatario { get; set; }
        public bool BaixarCanhotoAposAprovacaoDigitalizacao { get; set; }
        public DateTime? DataEmissaoCTeInicial { get; set; }
        public DateTime? DataEmissaoCTeFinal { get; set; }
        public SituacaoPgtoCanhoto? SituacaoPgtoCanhoto { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
        public int Usuario { get; set; }
        public DateTime? DataInicioEnvio { get; set; }
        public DateTime? DataFimEnvio { get; set; }
        public TipoLocalPrestacao TipoLocalPrestacao { get; set; }
        public int CodigoTransportadorLogado { get; set; }
        public List<int> CodigosVeiculo { get; set; }
        public bool FiltrarCargasPorParteDoNumero { get; set; }
        public string PlacaVeiculoResponsavelEntrega { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto? SituacaoHistorico { get; set; }
        public DateTime? DataInicialHistorico { get; set; }
        public DateTime? DataFinalHistorico { get; set; }
        public List<SituacaoCarga> SituacoesCarga { get; set; }

        public List<SituacaoCargaMercante> SituacoesCargaMercante { get; set; }

        public int CodigoLocalidadeOrigem { get; set; }

        public int CodigoLocalidadeDestino { get; set; }

        public double CnpjExpedidor { get; set; }

        public string NumeroDocumentoOriginario { get; set; }

        public int CodigoGrupoPessoaTomador { get; set; }

        public List<SituacaoNotaFiscal> SituacoesNotaFiscal { get; set; }

        public int CodigoMalote { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public StatusViagemControleEntrega? SituacaoViagem { get; set; }

        public bool ExibirCanhotosSemVinculoComCarga { get; set; }

        public List<double> ClienteComplementar { get; set; }

        public bool? DigitalizacaoIntegrada { get; set; }

        public string EscritorioVendas { get; set; }

        public string Matriz { get; set; }

        public bool? ValidacaoCanhoto { get; set; }
    }
}
