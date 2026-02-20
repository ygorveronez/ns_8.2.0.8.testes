using System;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega
{
    public class Coleta
    {
        public int Codigo;

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Remetente;

        public DateTime Data;
        public decimal Peso { get; set; }

        public string Senha;

        public string Endereco;

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega Situacao;

        public int Ordem;
    }
}
