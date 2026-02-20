using Azure.Storage.Blobs.Models;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.CTe
{
    public sealed class CTeSubstituto
    {
        public int Codigo { get; set; }
        public int Protocolo { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        public int NumeroOcorrencia { get; set; }
        public int ProtocoloOcorrencia { get; set; }
        public string CPFResponsavel { get; set; }
        public string NumeroCargaEmbarcador { get; set; }
        public string NomeResponsavel { get; set; }
        public int ProtocoloCarga {  get; set; }
        public string Chave { get; set; }
        public int Cfop { get; set; }
        public string DataEmissao { get; set; }
        public bool Lotacao { get; set; }
        public string Modelo { get; set; }
        public int Numero { get; set; }
        public string NumeroControle { get; set; }
        public string Pdf { get;set; }
        public int Serie { get; set; }
        public SituacaoCTeSefaz SituacaoCTeSefaz { get; set; }
        public TipoCTE TipoCte { get; set; }
        public Dominio.Enumeradores.TipoServico TipoServico { get; set; }
        public TipoTomador TipoTomador { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal AliquotaICMS { get; set; }
        public string Cst {  get; set; }
        public bool IncluirICMSBC { get; set; }
        public string ObservacaoCTe { get; set; }
        public decimal PercentualInclusaoBC { get; set; }
        public bool SimplesNacional { get; set; }
        public decimal ValorBaseCalculoICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorTotalAReceber { get;set; }
        public decimal ValorPrestacaoServico { get; set; }
        public string DataAutorizacao { get; set; }
        public string DataEmbarque { get; set; }
        public string DataPreviaVencimento {  get; set; }
        public decimal ValorTotalMercadoria { get; set; }
        public string VersaoCTE { get; set; }
        public string MotivoCancelamento { get; set; }
        public string NumeroNFSePrefeitura { get; set; }
        public string ProtocoloAutorizacao { get; set; }
    }    
}
