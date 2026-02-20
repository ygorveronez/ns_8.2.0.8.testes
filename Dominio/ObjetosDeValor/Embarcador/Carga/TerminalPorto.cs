namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class TerminalPorto
    {
        public int Codigo { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        public string CodigoTerminal { get; set; }
        public string CodigoDocumento { get; set; }
        public Porto Porto { get; set; }
        public Embarcador.Pessoas.Pessoa Terminal { get; set; }
        public bool Atualizar { get; set; }
        public bool InativarCadastro { get; set; }
        public string CodigoMercante { get; set; }
        public int QuantidadeDiasEnvioDocumentacao { get; set; }
        public string CodigoObservacaoContribuinte { get; set; }
    }
}
