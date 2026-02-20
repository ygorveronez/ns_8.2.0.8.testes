using System;

namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class ChamadoAnalise
    {
        public Usuario Autor { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataRetorno { get; set; }

        public string Observacao { get; set; }
    }
}
