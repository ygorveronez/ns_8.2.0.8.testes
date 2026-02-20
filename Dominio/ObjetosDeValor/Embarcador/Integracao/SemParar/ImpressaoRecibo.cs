namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar
{
    public class ImpressaoRecibo
    {
        public string NumeroVale { get; set; }

        public string Tipo { get; set; }

        public string Emissor { get; set; }

        public string Embarcador { get; set; }

        public string CNPJEmbarcador { get; set; }

        public string Transportador { get; set; }

        public string CNPJTransportador { get; set; }

        public string DataConfirmacao { get; set; }

        public string DataExpiracao { get; set; }

        public string DataViagem { get; set; }

        public string VeiculoCategoria { get; set; }

        public string Rota { get; set; }

        public decimal Total { get; set; }

        public string Observacao1 { get; set; }
        public string Observacao2 { get; set; }
        public string Observacao3 { get; set; }
        public string Observacao4 { get; set; }
        public string Observacao5 { get; set; }
        public string Observacao6 { get; set; }
    }
}
