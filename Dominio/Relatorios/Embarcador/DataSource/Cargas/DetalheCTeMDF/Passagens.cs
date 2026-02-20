using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas
{
    public class Passagem : IEquatable<Passagem>
    {
        public int Posicao { get; set; }
        public string Sigla { get; set; }

        public bool Equals(Passagem parPassagem)
        {
            return this.Posicao == parPassagem.Posicao;
        }

        public override int GetHashCode()
        {
            return this.Posicao.GetHashCode();
        }

    }
}
