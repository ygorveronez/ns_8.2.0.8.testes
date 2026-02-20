using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class ConsultaMonitoramentoNotificacoesApp
    {
        #region Propriedades 

        public int Codigo { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public int Chamado { get; set; }
        public TipoNotificacaoApp TipoNotificacao { get; set; }
        public DateTime DataEnvio { get; set; }
        public int NumeroTentativas { get; set; }
        public string Retorno { get; set; }
        public SituacaoIntegracao SituacaoIntegracao { get; set; }
        public string CPFMotorista { get; set; }
        public string NomeMotorista { get; set; }
        public string DescricaoTransportador { get; set; }
        public int CodigoIBGE { get; set; }
        public string DescricaoLocalidade { get; set; }
        public int Pais { get; set; }
        public string PaisNome { get; set; }
        public string PaisAbreviacao { get; set; }
        public string EstadoSigla { get; set; }

        #endregion

        #region Propriedades com Regras

        public string TipoNotificacaoDescricao => TipoNotificacao.ObterDescricao();
        public string SituacaoIntegracaoDescricao => SituacaoIntegracao.ObterDescricao();
        public string DT_RowColor => SituacaoIntegracao.ObterCorLinha();
        public string DT_FontColor => SituacaoIntegracao.ObterCorFonte();

        public string Integradora
        {
            get
            {
                return $"Trizy";
            }
        }

        public string Transportador
        {
            get
            {
                string cidadeEstado;

                if (CodigoIBGE != 9999999 || Pais == 0)
                {
                    cidadeEstado = DescricaoLocalidade + " - " + (EstadoSigla ?? "");
                }
                else
                {
                    if (!string.IsNullOrEmpty(PaisAbreviacao))
                        cidadeEstado = DescricaoLocalidade + " - " + PaisAbreviacao;
                    else
                        cidadeEstado = DescricaoLocalidade + " - " + PaisNome;
                }

                return $"{DescricaoTransportador} ({cidadeEstado})";
            }
        }

        public string Motorista
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CPFMotorista))
                    return NomeMotorista;

                string cpfFormatado = CPFMotorista.ObterCpfFormatado();

                return $"{NomeMotorista} {cpfFormatado}";
            }
        }

        public string DataEnvioFormatada
        {
            get { return DataEnvio != DateTime.MinValue ? DataEnvio.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        #endregion
    }
}
