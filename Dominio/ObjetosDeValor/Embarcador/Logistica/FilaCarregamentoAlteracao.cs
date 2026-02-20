using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FilaCarregamentoAlteracao
    {
        public HashSet<int> CentrosCarregamento { get; private set; }

        public HashSet<int> Filiais { get; private set; }

        public HashSet<int> GruposModelosVeicularesCarga { get; private set; }

        public HashSet<int> ModelosVeicularesCarga { get; private set; }

        public bool RecarregarInformacoes { get; private set; }

        public HashSet<Enumeradores.TipoFilaCarregamentoAlteracao> Tipos { get; private set; }

        public FilaCarregamentoAlteracao() : this (recarregarInformacoes: false) { }

        public FilaCarregamentoAlteracao(bool recarregarInformacoes)
        {
            CentrosCarregamento = new HashSet<int>();
            Filiais = new HashSet<int>();
            GruposModelosVeicularesCarga = new HashSet<int>();
            ModelosVeicularesCarga = new HashSet<int>();
            RecarregarInformacoes = recarregarInformacoes;
            Tipos = new HashSet<Enumeradores.TipoFilaCarregamentoAlteracao>();
        }
    }
}
