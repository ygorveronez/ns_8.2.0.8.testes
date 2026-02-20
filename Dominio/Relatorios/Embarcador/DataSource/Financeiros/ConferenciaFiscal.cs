using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class ConferenciaFiscal
    {
        #region Propriedades

        public int Codigo { get; set; }
        public DateTime DataEmissaoCIOT { get; set; }
        public string NumeroCIOT { get; set; }
        public string Contratante { get; set; }
        public string CNPJContratante { get; set; }
        public string Contratado { get; set; }
        public string TipoContratado { get; set; }
        public double CPFCNPJContratado { get; set; }
        public RegimeTributario RegimeTributarioTerceiro { get; set; }
        public string UFContratado { get; set; }
        public string MunicipioContratado { get; set; }
        public decimal ValorFreteContratado { get; set; }
        public decimal AliquotaPIS { get; set; }
        public decimal AliquotaCOFINS { get; set; }
        public DateTime DataEmissaoCTe { get; set; }
        public string ChaveCTe { get; set; }
        public int NumeroCTe { get; set; }
        public int SerieCTe { get; set; }
        public TipoCTE Finalidade { get; set; }
        public TipoCTE Forma { get; set; }
        public int CFOPCTe { get; set; }
        public string StatusCTe { get; set; }
        public DateTime DataCancelamentoCTe { get; set; }
        public decimal ValorTotalCTe { get; set; }
        public decimal BaseICMS { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public string CSTICMS { get; set; }
        public string ChaveCTeComplementar { get; set; }
        public string Emitente { get; set; }
        public string CPFCNPJEmitente { get; set; }
        public string UFEmitente { get; set; }
        public string Tomador { get; set; }
        public string CPFCNPJTomador { get; set; }
        public string UFTomador { get; set; }
        public string Remetente { get; set; }
        public string CPFCNPJRemetente { get; set; }
        public string UFRemetente { get; set; }
        public string Expedidor { get; set; }
        public string CPFCNPJExpedidor { get; set; }
        public string UFExpedidor { get; set; }
        public string Destinatario { get; set; }
        public string CPFCNPJDestinatario { get; set; }
        public string UFDestinatario { get; set; }
        public string Recebedor { get; set; }
        public string CPFCNPJRecebedor { get; set; }
        public string UFRecebedor { get; set; }
        public string UFInicioPrestacao { get; set; }
        public string UFFimPrestacao { get; set; }
        public string MunicipioInicioPrestacao { get; set; }
        public string MunicipioFimPrestacao { get; set; }
        public string ProdutoPredominante { get; set; }
        public decimal Quantidade { get; set; }
        public string UnidadeMedida { get; set; }
        public string ChaveNFeReferenciada { get; set; }
        public int NumeroNFeReferenciada { get; set; }
        public DateTime DataEmissaoNFe { get; set; }
        public decimal ValorNFe { get; set; }
        public string CFOPNFe { get; set; }
        public decimal BaseICMSNFe { get; set; }
        public decimal ValorICMSNFe { get; set; }
        public string EmitenteNota { get; set; }
        public string TipoEmitenteNota { get; set; }
        public double CPFCNPJEmitenteNota { get; set; }
        public string DestinatarioNota { get; set; }
        public string TipoDestinatarioNota { get; set; }
        public double CPFCNPJDestinatarioNota { get; set; }

        #endregion

        #region Propriedades com regras

        public string DataEmissaoCIOTFormatada
        { 
            get { return DataEmissaoCIOT != DateTime.MinValue ? DataEmissaoCIOT.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataEmissaoCTeFormatada
        {
            get { return DataEmissaoCTe != DateTime.MinValue ? DataEmissaoCTe.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataCancelamentoCTeFormatada
        {
            get { return DataCancelamentoCTe != DateTime.MinValue ? DataCancelamentoCTe.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataEmissaoNFeFormatada
        {
            get { return DataEmissaoNFe != DateTime.MinValue ? DataEmissaoNFe.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public decimal ValorPIS 
        { 
            get { return Math.Round((AliquotaPIS / 100) * ValorFreteContratado, 2, MidpointRounding.AwayFromZero); } 
        }

        public decimal ValorCOFINS 
        {
            get { return Math.Round((AliquotaCOFINS / 100) * ValorFreteContratado, 2, MidpointRounding.AwayFromZero); } 
        }

        public decimal AliquotaICMSNFe 
        {
            get { return BaseICMSNFe > 0 ? Math.Round((ValorICMSNFe * 100) / BaseICMSNFe, 2, MidpointRounding.AwayFromZero) : 0; } 
        }

        public string CNPJContratanteFormatado 
        {
            get { return CNPJContratante.ObterCpfOuCnpjFormatado(); }
        }

        public string CPFCNPJContratadoFormatado 
        { 
            get { return CPFCNPJContratado.ToString().ObterCpfOuCnpjFormatado(TipoContratado); }
        }

        public string CPFCNPJEmitenteFormatado 
        {
            get { return CPFCNPJEmitente.ObterCpfOuCnpjFormatado(); }
        }

        public string CPFCNPJTomadorFormatado
        {
            get { return CPFCNPJTomador.ObterCpfOuCnpjFormatado(); }
        }

        public string CPFCNPJRemetenteFormatado
        {
            get { return CPFCNPJRemetente.ObterCpfOuCnpjFormatado(); }
        }

        public string CPFCNPJExpedidorFormatado
        {
            get { return CPFCNPJExpedidor.ObterCpfOuCnpjFormatado(); }
        }

        public string CPFCNPJDestinatarioFormatado
        {
            get { return CPFCNPJDestinatario.ObterCpfOuCnpjFormatado(); }
        }

        public string CPFCNPJRecebedorFormatado
        {
            get { return CPFCNPJRecebedor.ObterCpfOuCnpjFormatado(); }
        }

        public string CPFCNPJEmitenteNotaFormatado 
        {
            get { return CPFCNPJEmitenteNota.ToString().ObterCpfOuCnpjFormatado(TipoEmitenteNota); }
        }

        public string CPFCNPJDestinatarioNotaFormatado
        {
            get { return CPFCNPJDestinatarioNota.ToString().ObterCpfOuCnpjFormatado(TipoDestinatarioNota); }
        }

        public string RegimeTributarioTerceiroFormatado 
        { 
            get { return RegimeTributarioTerceiro.ObterDescricao(); }
        }

        public string FinalidadeFormatada 
        { 
            get { return Finalidade.ObterDescricao(); }
        }

        public string FormaFormatada
        {
            get { return Forma.ObterDescricao(); }
        }

        public string DescricaoStatusCTe
        {
            get
            {
                switch (StatusCTe)
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
                        return "Anulado Gerencialmente";
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

        public string DescricaoUnidadeMedida 
        { 
            get {
                switch (this. UnidadeMedida)
                {
                    case "00":
                        return "M3";
                    case "01":
                        return "KG";
                    case "02":
                        return "TON";
                    case "03":
                        return "UN";
                    case "04":
                        return "LT";
                    case "05":
                        return "MMBTU";
                    case "99":
                        return "M3 ST";
                    default:
                        return "KG";
                }
            }
        }
        #endregion
    }
}
