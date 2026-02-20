using System.Configuration;

namespace Dominio.ObjetosDeValor.Embarcador.Monitoramento.Configuracao.Integracao
{
    public class ContaElement : ConfigurationElement
    {

        private const string NomeKey = "Nome";
        private const string HabilitadaKey = "Habilitada";
        private const string TipoKey = "Tipo";
        private const string ProtocoloKey = "Protocolo";
        private const string ServidorKey = "Servidor";
        private const string PortaKey = "Porta";
        private const string UsuarioKey = "Usuario";
        private const string SenhaKey = "Senha";
        private const string URIKey = "URI";
        private const string BancoDeDadosKey = "BancoDeDados";
        private const string CharsetKey = "Charset";
        private const string DiretorioKey = "Diretorio";
        private const string ArquivoControleKey = "ArquivoControle";
        private const string ParametrosAdicionaisKey = "ParametrosAdicionais";
        private const string UsaPosicaoFrotaKey = "UsaPosicaoFrota";

        private const string SolicitanteSenhaKey = "SolicitanteSenha";
        private const string SolicitanteIdKey = "SolicitanteId";
        private const string RastreadorIdKey = "RastreadorId";

        private const string BuscarDadosVeiculosKey = "BuscarDadosVeiculos";

        [ConfigurationProperty(NomeKey, IsRequired = true)]
        public string Nome => (string)this[NomeKey];

        [ConfigurationProperty(HabilitadaKey, IsRequired = true, DefaultValue = false)]
        public bool Habilitada => (bool)this[HabilitadaKey];

        [ConfigurationProperty(TipoKey, IsRequired = true)]
        public string Tipo => (string)this[TipoKey];

        [ConfigurationProperty(ProtocoloKey, IsRequired = false)]
        public string Protocolo => (string)this[ProtocoloKey];

        [ConfigurationProperty(ServidorKey, IsRequired = false)]
        public string Servidor => (string)this[ServidorKey];

        [ConfigurationProperty(PortaKey, IsRequired = false)]
        public int Porta => (int)this[PortaKey];

        [ConfigurationProperty(URIKey, IsRequired = false)]
        public string URI => (string)this[URIKey];

        [ConfigurationProperty(UsuarioKey, IsRequired = false)]
        public string Usuario => (string)this[UsuarioKey];

        [ConfigurationProperty(SenhaKey, IsRequired = false)]
        public string Senha => (string)this[SenhaKey];

        [ConfigurationProperty(BancoDeDadosKey, IsRequired = false)]
        public string BancoDeDados => (string)this[BancoDeDadosKey];

        [ConfigurationProperty(CharsetKey, IsRequired = false)]
        public string Charset => (string)this[CharsetKey];

        [ConfigurationProperty(DiretorioKey, IsRequired = false)]
        public string Diretorio => (string)this[DiretorioKey];

        [ConfigurationProperty(ArquivoControleKey, IsRequired = false)]
        public string ArquivoControle => (string)this[ArquivoControleKey];

        [ConfigurationProperty(ParametrosAdicionaisKey, IsRequired = false)]
        public string ParametrosAdicionais => (string)this[ParametrosAdicionaisKey];

        [ConfigurationProperty(SolicitanteSenhaKey, IsRequired = false)]
        public string SoliccitanteSenha => (string)this[SolicitanteSenhaKey];

        [ConfigurationProperty(SolicitanteIdKey, IsRequired = false)]
        public string SolicitanteId => (string)this[SolicitanteIdKey];

        [ConfigurationProperty(RastreadorIdKey, IsRequired = false)]
        public string RastreadorId => (string)this[RastreadorIdKey];

        [ConfigurationProperty(BuscarDadosVeiculosKey, IsRequired = false)]
        public string BuscarDadosVeiculos => (string)this[BuscarDadosVeiculosKey];

        [ConfigurationProperty(UsaPosicaoFrotaKey, IsRequired = false)]
        public bool UsaPosicaoFrota => (bool)this[UsaPosicaoFrotaKey];

    }
}
