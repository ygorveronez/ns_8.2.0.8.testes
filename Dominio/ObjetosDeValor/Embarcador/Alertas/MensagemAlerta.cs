using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Alertas
{
    public sealed class MensagemAlerta
    {
        public bool Bloquear { get; set; }

        public int Codigo { get; set; }

        public int CodigoEntidade { get; set; }

        public TipoMensagemAlerta Tipo { get; set; }

        public string Titulo { get; set; }

        public bool UtilizarConfirmacao { get; set; }

        public List<string> Mensagens { get; set; }
    }
}
