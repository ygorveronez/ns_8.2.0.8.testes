using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Escrituracao
{
    public class FreteContabil
    {
        public double CNPJTomador { get; set; }

        public string TipoTomador { get; set; }

        public string NomeTomador { get; set; }

        public string Tomador
        {
            get
            {
                string CNPJ = "";
                if (string.IsNullOrEmpty(TipoTomador))
                    CNPJ = "";
                else if (TipoTomador.Equals("E"))
                    CNPJ = "00.000.000/0000-00";
                else
                    CNPJ = TipoTomador.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJTomador) : String.Format(@"{0:000\.000\.000\-00}", this.CNPJTomador);

                return CNPJ + " - " + NomeTomador;
            }
        }

        public string TipoOperacao { get; set; }

        public string Filial { get; set; }

        public string CNPJEmpresa { get; set; }

        public string RazaoEmpresa { get; set; }

        public string Empresa
        {
            get
            {
                string CNPJ = CNPJEmpresa;
                if (string.IsNullOrWhiteSpace(CNPJEmpresa))
                    CNPJ = "0";

                return String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(CNPJ)) + " - " + RazaoEmpresa;
            }
        }

        public string Origem
        {
            get
            {
                string ufOrigem = UFOrigem;
                string cidadeOrigem = CidadeOrigem;

                if (string.IsNullOrWhiteSpace(cidadeOrigem) || string.IsNullOrWhiteSpace(ufOrigem))
                    return "";

                return cidadeOrigem + " - " + ufOrigem;
            }
        }
        public string CidadeOrigem { get; set; }
        public string UFOrigem { get; set; }
        public string CidadeDestino { get; set; }
        public string UFDestino { get; set; }
        public string Destino
        {
            get
            {
                string ufDestino = UFDestino;
                string cidadeDestino = CidadeDestino;

                if (string.IsNullOrWhiteSpace(cidadeDestino) || string.IsNullOrWhiteSpace(ufDestino))
                    return "";

                return cidadeDestino + " - " + ufDestino;
            }
        }

        public string Carga { get; set; }

        public int Ocorrencia { get; set; }
        public string NumeroOcorrencia
        {
            get
            {
                if (Ocorrencia > 0)
                    return Ocorrencia.ToString();
                else
                    return "";
            }
        }

        public DateTime DataEmissao { get; set; }

        public double CNPJRemetente { get; set; }

        public string TipoRemetente { get; set; }

        public string NomeRemetente { get; set; }

        public int Pagamento { get; set; }

        public int CancelamentoPagamento { get; set; }

        public int Provisao { get; set; }

        public int CancelamentoProvisao { get; set; }

        public string TipoDeContabilizacao
        {
            get
            {
                if (CancelamentoPagamento > 0)
                    return "Cancelamento Pagamento";
                else if (Pagamento > 0)
                    return "Pagamento";
                else if (CancelamentoProvisao > 0)
                    return "Cancelamento Provisão";
                else if (Provisao > 0)
                    return "Provisão";
                else
                    return "";
            }
        }

        public string Remetente
        {
            get
            {
                string CNPJ = "";
                if (string.IsNullOrEmpty(TipoRemetente))
                    CNPJ = "";
                else if (TipoRemetente.Equals("E"))
                    CNPJ = "00.000.000/0000-00";
                else
                    CNPJ = TipoRemetente.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJRemetente) : String.Format(@"{0:000\.000\.000\-00}", this.CNPJRemetente);

                return CNPJ + " - " + NomeRemetente;
            }
        }

        public double CNPJDestinatario { get; set; }

        public string TipoDestinatario { get; set; }

        public string NomeDestinatario { get; set; }

        public string Destinatario
        {
            get
            {
                string CNPJ = "";
                if (string.IsNullOrEmpty(TipoDestinatario))
                    CNPJ = "";
                else if (TipoDestinatario.Equals("E"))
                    CNPJ = "00.000.000/0000-00";
                else
                    CNPJ = TipoDestinatario.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJDestinatario) : String.Format(@"{0:000\.000\.000\-00}", this.CNPJDestinatario);

                return CNPJ + " - " + NomeDestinatario;
            }
        }

        public int Numero { get; set; }

        public int Serie { get; set; }

        public string Modelo { get; set; }

        public int NumeroCTe { get; set; }

        public int SerieCTe { get; set; }

        public DateTime DataLancamento { get; set; }
        public DateTime DataLancamentoCancelamentoProvisao { get; set; }
        public string AnoLancamento
        {
            get
            {
                return DataLancamento.ToString("yyyy");
            }
        }
        public string MesLancamento
        {
            get
            {
                return DataLancamento.ToString("MM");
            }
        }

        public string CodigoCentroResultado { get; set; }

        public string CentroResultado { get; set; }

        public string CodigoPlanoConta { get; set; }

        public string PlanoConta { get; set; }

        public decimal _ValorLancamento { get; set; }

        public decimal ValorLancamento
        {
            get
            {
                return (_TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Debito ? -1 : 1) * _ValorLancamento;
            }
        }

        public decimal Credito
        {
            get
            {
                return _TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Credito ? _ValorLancamento : 0;
            }
        }

        public decimal Debito
        {
            get
            {
                return _TipoContabilizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Debito ? _ValorLancamento : 0;
            }
        }

        public TipoContabilizacao _TipoContabilizacao { get; set; }

        public string TipoContabilizacao
        {
            get
            {
                return _TipoContabilizacao.ObterDescricao();
            }
        }

        public decimal _ICMS { get; set; }

        public decimal ICMS
        {
            get
            {
                return CST != "60" ? _ICMS : 0;
            }
        }

        public decimal ICMSRetido
        {
            get
            {
                return CST == "60" ? _ICMS : 0;
            }
        }

        public decimal _ValorISS { get; set; }

        public decimal ValorISSRetido { get; set; }

        public decimal ValorISS
        {
            get
            {
                return _ValorISS - ValorISSRetido;
            }
        }

        public decimal Aliquota { get; set; }

        public decimal AliquotaISS { get; set; }

        public string CST { get; set; }

        public string MotivoCancelamento { get; set; }

        public string TipoCarga { get; set; }

        public string ModeloVeicularCarga { get; set; }

        public string Veiculo { get; set; }

        public decimal PesoCTe { get; set; }

        public string RemetenteCTe { get; set; }
        public string DestinatarioCTe { get; set; }
        public string NumeroVP { get; set; }
        private decimal ValorVP { get; set; }
        public string ValorVPFormatado
        {
            get
            {
                return ValorVP.ToString("N2") ?? string.Empty;
            }
        }

        public string Expedidor { get; set; }
        public string Recebedor { get; set; }
        public string TipoOcorrencia { get; set; }
        private DateTime DataCriacaoCarga { get; set; }
        public string DataCriacaoCargaFormatada
        {
            get
            {
                return (DataCriacaoCarga != DateTime.MinValue) ? DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm") : string.Empty;
            }
        }
        private DateTime DataEmissaoCTe { get; set; }
        public string DataEmissaoCTeFormatada
        {
            get
            {
                return (DataEmissaoCTe != DateTime.MinValue) ? DataEmissaoCTe.ToString("dd/MM/yyyy HH:mm") : string.Empty;
            }
        }
        private string CNPJTransportador { get; set; }
        public string CNPJTransportadorFormatado
        {
            get
            {
                return CNPJTransportador.ObterCnpjFormatado();
            }
        }
        public string CNPJRemetenteFormatado
        {
            get
            {
                string CNPJ = "";
                if (string.IsNullOrEmpty(TipoRemetente))
                    CNPJ = "";
                else if (TipoRemetente.Equals("E"))
                    CNPJ = "00.000.000/0000-00";
                else
                    CNPJ = TipoRemetente.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJRemetente) : String.Format(@"{0:000\.000\.000\-00}", this.CNPJRemetente);

                return CNPJ;
            }
        }

        public string CNPJDestinatarioFormatado
        {
            get
            {
                string CNPJ = "";
                if (string.IsNullOrEmpty(TipoDestinatario))
                    CNPJ = "";
                else if (TipoDestinatario.Equals("E"))
                    CNPJ = "00.000.000/0000-00";
                else
                    CNPJ = TipoDestinatario.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJDestinatario) : String.Format(@"{0:000\.000\.000\-00}", this.CNPJDestinatario);

                return CNPJ;
            }
        }
        private decimal KMCarga { get; set; }
        public string KMCargaFormatado
        {
            get
            {
                return String.Format("{0:n1} Km", KMCarga);
            }
        }

        public decimal PesoNF { get; set; }
        public decimal PesoCarga { get; set; }
        public decimal KmRodado { get; set; }
    }
}
