using System;

namespace Dominio.ObjetosDeValor
{
    public class ObjetoConsulta : IEquatable<Dominio.ObjetosDeValor.ObjetoConsulta>
    {
        public ObjetoConsulta(int codigo, Dominio.Enumeradores.TipoObjetoConsulta tipo)
        {
            this.Codigo = codigo;
            this.Tipo = tipo;
        }

        public int Codigo { get; set; }
        public Dominio.Enumeradores.TipoObjetoConsulta Tipo { get; set; }

        public bool Equals(ObjetoConsulta other)
        {
            if (other == null || other.Codigo != this.Codigo || other.Tipo != this.Tipo)
                return false;
            else
                return true;
        }
    }
}
