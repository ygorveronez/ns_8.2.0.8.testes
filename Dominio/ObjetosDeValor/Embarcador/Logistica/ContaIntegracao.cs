using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class ContaIntegracao
    {
        public string Nome { get; set; }
        public bool Habilitada { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao TipoIntegracao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComunicacaoIntegracao TipoComunicacaoIntegracao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.Protocolo Protocolo { get; set; }
        public string Servidor { get; set; }
        public int Porta { get; set; }
        public string URI { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string BancoDeDados { get; set; }
        public string Diretorio { get; set; }
        public string ArquivoControle { get; set; }
        public string ParametrosAdicionais { get; set; }
        public List<KeyValuePair<string, string>> ListaParametrosAdicionais { get; set; }

        /// <summary>
        /// Propriedades utilizadas para integrações com a NSTech
        /// </summary>
        public string RastreadorId { get; set; }
        public string SolicitanteId { get; set; }
        public string SolicitanteSenha { get; set; }

        /// <summary>
        /// Propriedade utilizada na integracao com OnixSat
        /// </summary>
        public bool BuscarDadosVeiculos { get; set; } = false;

        /// <summary>
        /// Propriedade utilizada na integracao com OpenTech
        /// </summary>
        public bool UsaPosicaoFrota { get; set; } = false;
    }
}
