using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi
{
    public sealed class IntegracaoCTe
	{
		[JsonProperty(PropertyName = "numero_cte", Order = 1)]
		public int NumeroCTe { get; set; }

		[JsonProperty(PropertyName = "serie_cte", Order = 2)]
		public int SerieCTe { get; set; }

		[JsonProperty(PropertyName = "data_emissao", Order = 3)]
		public string DataEmissao { get; set; }

		[JsonProperty(PropertyName = "codigo_cfop", Order = 4)]
		public int CodigoCFOP { get; set; }

		[JsonProperty(PropertyName = "valor_total", Order = 5)]
		public decimal ValorTotal { get; set; }

		[JsonProperty(PropertyName = "al_icms", Order = 6)]
		public decimal AliquotaICMS { get; set; }

		[JsonProperty(PropertyName = "bs_icms", Order = 7)]
		public decimal BaseCalculoICMS { get; set; }

		[JsonProperty(PropertyName = "vl_icms", Order = 8)]
		public decimal ValorICMS { get; set; }

		[JsonProperty(PropertyName = "cnpj_emissor", Order = 9)]
		public string CNPJEmissor { get; set; }

		[JsonProperty(PropertyName = "nome_emitente", Order = 10)]
		public string NomeEmitente { get; set; }

		[JsonProperty(PropertyName = "cnpj_tomador", Order = 11)]
		public string CPFCNPJTomador { get; set; }

		[JsonProperty(PropertyName = "nome_tomador", Order = 12)]
		public string NomeTomador { get; set; }

		[JsonProperty(PropertyName = "remetente", Order = 13)]
		public string CPFCNPJRemetente { get; set; }

		[JsonProperty(PropertyName = "nome_remetente", Order = 14)]
		public string NomeRemetente { get; set; }

		[JsonProperty(PropertyName = "destinatario", Order = 15)]
		public string CPFCNPJDestinatario { get; set; }

		[JsonProperty(PropertyName = "nome_destinatario", Order = 16)]
		public string NomeDestinatario { get; set; }

		[JsonProperty(PropertyName = "expedidor", Order = 17)]
		public string CPFCNPJExpedidor { get; set; }

		[JsonProperty(PropertyName = "nome_expedidor", Order = 18)]
		public string NomeExpedidor { get; set; }

		[JsonProperty(PropertyName = "recebedor", Order = 19)]
		public string CPFCNPJRecebedor { get; set; }

		[JsonProperty(PropertyName = "nome_recebedor", Order = 20)]
		public string NomeRecebedor { get; set; }

		[JsonProperty(PropertyName = "chave_cte", Order = 21)]
		public string ChaveCTe { get; set; }

		[JsonProperty(PropertyName = "tomador_cte", Order = 22)]
		public int TomadorCTe { get; set; }

		[JsonProperty(PropertyName = "tipo_documento", Order = 23)]
		public int TipoDocumento { get; set; }

		[JsonProperty(PropertyName = "regiao_emissor_documento", Order = 24)]
		public int RegiaoEmissorDocumento { get; set; }

		[JsonProperty(PropertyName = "uf_inicio_prestacao", Order = 25)]
		public string UFInicioPrestacao { get; set; }

		[JsonProperty(PropertyName = "codigo_domicilio_origem", Order = 26)]
		public string CodigoDomicilioOrigem { get; set; }

		[JsonProperty(PropertyName = "nome_domicilio_origem", Order = 27)]
		public string NomeDomicilioOrigem { get; set; }

		[JsonProperty(PropertyName = "uf_fim_de_prestacao", Order = 28)]
		public string UFFimPrestacao { get; set; }

		[JsonProperty(PropertyName = "codigo_domicilio_destino", Order = 29)]
		public string CodigoDomicilioDestino { get; set; }

		[JsonProperty(PropertyName = "nome_domicilio_destino", Order = 30)]
		public string NomeDomicilioDestino { get; set; }

		[JsonProperty(PropertyName = "uf_emitente", Order = 31)]
		public string UFEmitente { get; set; }

		[JsonProperty(PropertyName = "modo_transporte", Order = 32)]
		public int ModoTransporte { get; set; }

		[JsonProperty(PropertyName = "status_cte_sefaz", Order = 33)]
		public string StatusCTeSefaz { get; set; }

        [JsonProperty(PropertyName = "centro_custo", Order = 33)]
        public int CentroCusto { get; set; }
    }
}
