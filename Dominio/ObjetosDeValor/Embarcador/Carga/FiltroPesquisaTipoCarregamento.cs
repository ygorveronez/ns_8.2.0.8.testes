namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaTipoCarregamento
    {
        public string Descricao { get; set; }

        public string CodigoIntegracao { get; set; }

        public string Observacao { get; set; }

        public bool? Situacao { get; set; }
    }
}
