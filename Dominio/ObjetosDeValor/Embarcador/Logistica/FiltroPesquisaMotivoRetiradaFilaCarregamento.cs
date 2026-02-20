namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaMotivoRetiradaFilaCarregamento
    {
        public string Descricao { get; set; }

        public bool? Mobile { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa SituacaoAtivo { get; set; }
    }
}
