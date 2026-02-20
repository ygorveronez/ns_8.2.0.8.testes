using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega
{
    public class Entrega
    {
        public int Codigo;

        public DateTime Data;

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario;

        public List<Dominio.ObjetosDeValor.Embarcador.Pessoas.JanelaDescarregamento> JanelaDescarregamento;
        
        public string Senha;

        public string Endereco;

        public decimal Peso { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega Situacao;

        public int Ordem;
    }
}
