using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Importacao
{
    public class ConfiguracaoImportacao
    {
        public ConfiguracaoImportacao()
        {
            this.Regras = new List<string>();
        }
        /// <summary>
        /// Identificador para salvar campos padroes.Permite alterar a ordem ou o nome da propriedade sem perder a config.salva
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Define se o campo é apenas informação. Caso seja, ele não é inserido na entidade
        /// </summary>
        public bool CampoInformacao { get; set; }

        public string Descricao { get; set; }

        public string Propriedade { get; set; }

        public int Tamanho { get; set; }

        public bool Obrigatorio { get; set; }

        public List<string> Regras { get; set; }

        public bool CampoEntidade { get; set; }
    }
}

