using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioCTesRedespacho
    {
        #region Propriedades

        public int Codigo { get; set; }

        public int Numero { get; set; }

        public int Serie { get; set; }

        public DateTime DataEmissao { get; set; }

        public string DataEmissaoContrato { get; set; }

        public Enumeradores.TipoServico TipoServico { get; set; }

        public RegimeTributarioCTe RegimeTributarioCTe { get; set; }

        public int CFOP { get; set; }

        public string Transportador { get; set; }

        public string CNPJTransportador { get; set; }

        public string Remetente { get; set; }

        public string CPFCNPJRemetente { get; set; }

        public string UFRemetente { get; set; }

        public string Destinatario { get; set; }

        public string CPFCNPJDestinatario { get; set; }

        public string UFDestinatario { get; set; }

        public string Expedidor { get; set; }

        public string CPFCNPJExpedidor { get; set; }

        public string Recebedor { get; set; }

        public string CPFCNPJRecebedor { get; set; }

        public string Tomador { get; set; }

        public string CPFCNPJTomador { get; set; }

        public string UFOrigemPrestacao { get; set; }

        public string UFTerminoPrestacao { get; set; }

        public string CSTICMS { get; set; }

        public decimal ValorICMS { get; set; }

        public decimal BaseCalculoICMS { get; set; }

        public decimal AliquotaICMS { get; set; }

        public decimal ValorAReceber { get; set; }

        public decimal ValorFrete { get; set; }

        public string NumeroCteAnterior { get; set; }

        public string ChaveCteAnterior { get; set; }

        public string SerieCteAnterior { get; set; }

        public string CodigoProcessoTransporte { get; set; }

        public decimal ValorFreteContrato { get; set; }

        public string Percurso { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public string CSTICMSFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CSTICMS))
                    return "SN";

                return CSTICMS;
            }
        }

        public string NumeroCteAnteriorFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ChaveCteAnterior))
                    return NumeroCteAnterior;

                IEnumerable<string> chaves = ChaveCteAnterior.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());

                IEnumerable<int> numeros = chaves
                    .Where(chave => chave.Length == 44)
                    .Select(chave => Utilidades.Chave.ObterNumero(chave));

                return string.Join(", ", numeros);
            }
        }

        public string SerieCteAnteriorFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ChaveCteAnterior))
                    return SerieCteAnterior;

                IEnumerable<string> chaves = ChaveCteAnterior.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());

                IEnumerable<string> series = chaves
                    .Where(chave => chave.Length == 44)
                    .Select(chave => Utilidades.Chave.ObterSerie(chave));

                return string.Join(", ", series);
            }
        }

        public string DataEmissaoFormatada
        {
            get { return this.DataEmissao != default ? this.DataEmissao.ToDateTimeString() : string.Empty; }
        }

        public string TipoServicoFormatada
        {
            get { return TipoServico.ObterDescricao(); }
        }

        public string RegimeTributarioCTeFormatada
        {
            get { return RegimeTributarioCTe.ObterDescricao(); }
        }

        #endregion Propriedades com Regras
    }
}