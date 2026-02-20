using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyEventos
{
    public class ResponseTrizyEventos
    {
        #region Propriedades

        [JsonProperty("notifications")]
        public List<Notificacao> Notificacoes { get; set; }

        #endregion Propriedades

        #region Propriedades Com Regras

        public string Mensagem
        {
            get
            {
                if (Notificacoes != null && Notificacoes.Count > 0)
                    return string.Join(" | ", Notificacoes.ConvertAll(x => x.Mensagem));

                return string.Empty;
            }
        }

        #endregion Propriedades Com Regras
    }

    public class Notificacao
    {
        [JsonProperty("title")]
        public string Titulo { get; set; }

        [JsonProperty("message")]
        public string Mensagem { get; set; }

        [JsonProperty("level")]
        public string Nivel { get; set; }
    }
}

