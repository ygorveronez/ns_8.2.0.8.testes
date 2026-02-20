using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public class FiltroRegiaoBidding
    {
        public string Descricao { get; set; }
        public int Codigo { get; set; }
    }

    public class FiltroRegiaoBiddingComparer : IEqualityComparer<Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroRegiaoBidding>
    {
        public bool Equals(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroRegiaoBidding x, Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroRegiaoBidding y)
        {
            return x?.Codigo == y?.Codigo && x?.Descricao == y?.Descricao;
        }

        public int GetHashCode(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroRegiaoBidding obj)
        {
            if (obj == null) return 0;
            int hashDescricao = obj.Descricao?.GetHashCode() ?? 0;
            int hashCodigo = obj.Codigo.GetHashCode();
            return hashDescricao ^ hashCodigo;
        }
    }
}
