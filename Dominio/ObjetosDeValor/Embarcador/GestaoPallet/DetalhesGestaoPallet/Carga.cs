namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.DetalhesGestaoPallet
{
    public class Carga
    {
        public string CodigoCargaEmbarcador { get; set; }

        public CargaDadosSumarizados DadosSumarizados { get; set; }
    }

    public class CargaDadosSumarizados
    {
        public string Origem { get; set; }

        public string Destino { get; set; }
    }
}
