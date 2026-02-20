using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega
{
    public class Parada
    {
        public int Codigo { get; set; }
        public DateTime? Data { get; set; }
        public DateTime? DataInicioEntrega { get; set; }
        public DateTime? DataConfirmacao { get; set; }
        public DateTime? DataInicioCarregamento { get; set; }
        public DateTime? DataTerminoCarregamento { get; set; }
        public DateTime? DataInicioDescarga { get; set; }
        public DateTime? DataTerminoDescarga { get; set; }
        public DateTime? EntradaPropriedade { get; set; }
        public DateTime? SaidaPropriedade { get; set; }
        public DateTime? DataProgramadaColeta { get; set; }
        public DateTime? DataProgramadaDescarga { get; set; }
        public string TempoProgramadoColeta { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Cliente { get; set; }
        public string Senha { get; set; }
        public string Endereco { get; set; }
        public decimal Peso { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega Situacao;
        public int Ordem { get; set; }
        public bool Coleta { get; set; }
        public TipoCargaEntrega Tipo { get; set; }
        public bool PossuiReentrega { get; set; }
        public bool DevolucaoParcial { get; set; }
        public bool DiferencaDevolucao { get; set; }
        public bool ColetaAdicional { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList> CheckList { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido> Pedidos { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColeta MotivoRejeicaoColeta { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao MotivoDevolucao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRetificacaoColeta MotivoRetificacaoColeta { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Nota> Notas { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto> Canhotos { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> Produtos { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Atendimento> Atendimentos { get; set; }
        public string JanelaDescarga { get; set; }
        public string NotasFiscais { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.DadosRecebedor DadosRecebedor;
        public DateTime? DataConfirmacaoChegada { get; set; }
        public string ObservacoesPedidos { get; set; }
    }
}
