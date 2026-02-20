using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Documentos
{
    public class DadosDocsys
    {
        public string Viagem { get; set; }
        public string PortoOrigem { get; set; }
        public string PortoDestino { get; set; }
        public string TrackingStatus
        {
            get
            {
                return "Y";
            }
        }
        public DateTime TrackingData { get; set; }
        public string TrackingDataFormatado
        {
            get
            {
                return this.TrackingData > DateTime.MinValue ? this.TrackingData.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string Booking { get; set; }
        public string NumeroControle { get; set; }
        public Dominio.Enumeradores.TipoCTE TipoCTE { get; set; }
        public string TipoCTeFormatado
        {
            get
            {
                switch (this.TipoCTE)
                {
                    case Dominio.Enumeradores.TipoCTE.Anulacao:
                        return "Anulação";
                    case Dominio.Enumeradores.TipoCTE.Complemento:
                        return "Complemento";
                    case Dominio.Enumeradores.TipoCTE.Normal:
                        return "Normal";
                    case Dominio.Enumeradores.TipoCTE.Substituto:
                        return "Substituto";
                    default:
                        return "";
                }
            }
        }
        public decimal ValorReceber { get; set; }
        public decimal ValorPrestacao { get; set; }
        public DateTime PrevisaoSaida { get; set; }
        public string PrevisaoSaidaFormatado
        {
            get
            {
                return this.PrevisaoSaida > DateTime.MinValue ? this.PrevisaoSaida.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public DateTime Emissao { get; set; }
        public string EmissaoFormatado
        {
            get
            {
                return this.Emissao > DateTime.MinValue ? this.Emissao.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string VoucherNO { get; set; }
        public DateTime VoucherDate { get; set; }
        public string VoucherDateFormatado
        {
            get
            {
                return this.VoucherDate > DateTime.MinValue ? this.VoucherDate.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public DateTime DACS_transf { get; set; }
        public string DACS_transfFormatado
        {
            get
            {
                return this.DACS_transf > DateTime.MinValue ? this.DACS_transf.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string UBLI { get; set; }
        public DateTime DataInclusao { get; set; }
        public string DataInclusaoFormatado
        {
            get
            {
                return this.DataInclusao > DateTime.MinValue ? this.DataInclusao.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string CorrCode { get; set; }
        public string BLVersion { get; set; }
        public string FlgDocsys
        {
            get
            {
                return "OK";
            }
        }
        public string TipoOperacao { get; set; }
        public string Status { get; set; }
        public string StatusFormatado
        {
            get
            {
                switch (this.Status)
                {
                    case "P":
                        return "Pendente";
                    case "E":
                        return "Enviado";
                    case "R":
                        return "Rejeição";
                    case "A":
                        return "Autorizado";
                    case "C":
                        return "Cancelado";
                    case "I":
                        return "Inutilizado";
                    case "D":
                        return "Denegado";
                    case "S":
                        return "Em Digitação";
                    case "K":
                        return "Em Cancelamento";
                    case "L":
                        return "Em Inutilização";
                    case "Z":
                        return "ANULADO";
                    case "X":
                        return "Aguardando Assinatura";
                    case "V":
                        return "Aguardando Assinatura Cancelamento";
                    case "B":
                        return "Aguardando Assinatura Inutilização";
                    case "M":
                        return "Aguardando Emissão e-mail";
                    case "F":
                        return "Contingência FSDA";
                    case "Q":
                        return "Contingência EPEC";
                    case "Y":
                        return "Aguardando Finalizar Carga Integração";
                    default:
                        return string.Empty;
                }
            }
        }
        public string Tomador { get; set; }
        public bool PossuiCartaCorrecao { get; set; }
        public string PossuiCartaCorrecaoFormatado
        {
            get
            {
                return this.PossuiCartaCorrecao == true ? "SIM" : "NÃO";
            }
        }
        public bool FoiAnulado { get; set; }
        public string FoiAnuladoFormatado
        {
            get
            {
                return this.FoiAnulado == true ? "SIM" : "NÃO";
            }
        }
        public bool PossuiComplemento { get; set; }
        public string PossuiComplementoFormatado
        {
            get
            {
                return this.PossuiComplemento == true ? "SIM" : "NÃO";
            }
        }
        public int FoiSubstituido { get; set; }
        public string FoiSubstituidoFormatado
        {
            get
            {
                return this.FoiSubstituido > 0 ? "SIM" : "NÃO";
            }
        }
        public DateTime DataEmissaoFatura { get; set; }
        public string DataEmissaoFaturaFormatado
        {
            get
            {
                return this.DataEmissaoFatura > DateTime.MinValue ? this.DataEmissaoFatura.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public int FoiFaturado { get; set; }
        public string FoiFaturadoFormatado
        {
            get
            {
                return this.FoiFaturado > 0 ? "SIM" : "NÃO";
            }
        }
        public int DiaEmissao
        {
            get
            {
                return this.Emissao > DateTime.MinValue ? this.Emissao.Day : 0;
            }
        }
        public int AnoEmissao
        {
            get
            {
                return this.Emissao > DateTime.MinValue ? this.Emissao.Year : 0;
            }
        }
        public string CorteEmissao
        {
            get
            {
                return this.CortePrevisaoSaida == "Desconsiderar" ? "Desconsiderar" : ((DateTime.Now - this.Emissao).Days < 1) ? "Desconsiderar" : "Considerar";
            }
        }
        public string ViagemBookingCTe
        {
            get
            {
                return this.Viagem + this.Booking + this.NumeroControle;
            }
        }
        public string StatusDocsys
        {
            get
            {
                if (this.ConsiderarDesconsiderar == "Desconsiderar" || this.CanceladosAnulados == "Desconsiderar")
                    return "Desconsiderar";
                if ((this.Status == "A" && this.FoiAnulado && this.FlgDocsys != "Pendente" && this.CorrCode != "CRN" && this.BLVersion != "") || (this.Status != "A" && this.FlgDocsys != "Pendente" && this.CorrCode != "CRN" && this.BLVersion != ""))
                    return "Necessário ajuste no Docsys";
                else if (this.Status == "A" && this.FlgDocsys != "Pendente" && this.CorrCode == "CRN" && this.BLVersion != "" && !this.FoiAnulado)
                    return "Cancelado no docsys - verificar";
                else if (this.DACS_transf > DateTime.MinValue)
                    return "Dacs Transfer - OK";
                else if (this.FlgDocsys == "OK")
                    return "Dacs Transfer - Pendente";
                else if (this.FlgDocsys == "Pendente" && this.FoiFaturado <= 0)
                    return "Faturamento - Pendente";
                else if (this.FlgDocsys == "Pendente")
                    return "Docsys - Pendente";
                else
                    return "0";
            }
        }
        public string CortePrevisaoSaida
        {
            get
            {
                return ((DateTime.Now.Date - this.PrevisaoSaida.Date).Days < 1) ? "Desconsiderar" : "Considerar";
            }
        }
        public string SaidaNavio
        {
            get
            {
                if (this.PrevisaoSaida <= DateTime.MinValue)
                    return "";
                else if ((DateTime.Now - this.PrevisaoSaida.Date).Days > 3)
                    return "D - 4 ou mais";
                else if ((DateTime.Now - this.PrevisaoSaida.Date).Days < -1)
                    return "Navio ainda não saiu";
                else
                    return "D - " + (DateTime.Now - this.PrevisaoSaida.Date).Days.ToString("n0");
            }
        }
        public string TipoEmissao
        {
            get
            {
                if (this.Tomador.ToUpper().Contains("HAMBURG SUDAMERIKANISCHE"))
                    return "Feeder - HSUD";
                else if (string.IsNullOrWhiteSpace(this.TipoOperacao))
                    return "SVM";
                else if (this.TipoOperacao.ToUpper().Contains("FEEDER"))
                    return "Feeder";
                else
                    return "Cabotagem";
            }
        }
        public int IBGEOrigem { get; set; }
        public int IBGEDestino { get; set; }
        public string EstadoOrigem { get; set; }
        public string EstadoDestino { get; set; }
        public string EstadosIguais
        {
            get
            {
                return this.IBGEOrigem == this.IBGEDestino ? "S" : "N";
            }
        }
        public bool Duplicado { get; set; }
        public string DuplicadoFormatado
        {
            get
            {
                return this.Duplicado == true ? "S" : "N";
            }
        }
        public string ConsiderarDesconsiderar
        {
            get
            {
                if (this.Status != "A" && this.FlgDocsys == "Pendente")
                    return "Desconsiderar";
                else if (this.TipoCTE == Dominio.Enumeradores.TipoCTE.Anulacao || this.CortePrevisaoSaida == "Desconsiderar")
                    return "Desconsiderar";
                else
                    return "Considerar";
            }
        }
        public string CanceladosAnulados
        {
            get
            {
                if (this.Status != "A" && this.FlgDocsys != "Pendente" && this.CorrCode != "CRN" && this.BLVersion != "")
                    return "Necessário ajuste no Docsys";
                else
                    return "OK";
            }
        }
        public int MesSaidaNavio
        {
            get
            {
                return this.PrevisaoSaida > DateTime.MinValue ? this.PrevisaoSaida.Month : 0;
            }
        }
        public int AnoSaidaNavio
        {
            get
            {
                return this.PrevisaoSaida > DateTime.MinValue ? this.PrevisaoSaida.Year : 0;
            }
        }
        public string Meta
        {
            get
            {
                if (this.ConsiderarDesconsiderar == "Desconsiderar")
                    return "Desconsiderar";
                else if (this.PrevisaoSaida <= DateTime.MinValue)
                    return "";
                else if ((this.DACS_transf.Date - this.PrevisaoSaida.Date).Days > 2)
                    return "Fora do Prazo";
                else
                    return "No Prazo";
            }
        }
        public string MesContabil
        {
            get
            {
                if (this.ConsiderarDesconsiderar == "Desconsiderar")
                    return "Desconsiderar";
                else if (this.DACS_transf == DateTime.MinValue || this.PrevisaoSaida == DateTime.MinValue)
                    return "Pendente";
                else if (this.PrevisaoSaida.Day < 27 && this.DACS_transf.Day < 27 && this.PrevisaoSaida.Month == this.DACS_transf.Month)
                    return "Sim";
                else if (this.PrevisaoSaida.Day > 27 && (((this.PrevisaoSaida.Month + 1) == this.DACS_transf.Month) || (this.PrevisaoSaida.Month == this.DACS_transf.Month)))
                    return "Lançamento do final do mês - OK";
                else if (this.Meta == "No prazo")
                    return "Não - Dentro do Prazo";
                else
                    return "Não - Fora do Prazo";
            }
        }
    }
}
