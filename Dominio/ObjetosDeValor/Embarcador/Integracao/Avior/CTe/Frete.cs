namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe
{
    public class Frete
    {
        public string cnpj_filial { get; set; }
        public string numero_frete { get; set; }
        public string numero_cte { get; set; }
        public string valor_total_frete { get; set; }
        public string valor_liquido { get; set; }
        public string valor_bruto { get; set; }
        public string prc_icms { get; set; }
        public string valor_icms { get; set; }
        public string valor_pedagio { get; set; }
        public string descricao_tipos_produtos { get; set; }
        public string peso_carga { get; set; }
        public string valor_mercadoria { get; set; }
        public string valor_descarga { get; set; }
        public string qtde_volume_transportado { get; set; }
        public string[] ordens_coletas { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe.Cliente Cliente { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe.Cliente Entrega { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe.Cliente Remetente { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe.Cliente Destinatario { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe.Cliente Tomador { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe.NFe Nfe { get; set; }
    }
}
