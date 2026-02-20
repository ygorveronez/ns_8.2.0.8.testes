using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public class RejeitarEntregaParametros
    {
        #region Atributos

        public int codigoCargaEntrega;
        public int codigoMotivo;
        public DateTime data;
        public Logistica.WayPoint wayPoint;
        public Logistica.WayPoint wayPointDescarga;
        public Entidades.Usuario usuario;
        public int motivoRetificacao;
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware;
        public AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware;
        public string observacao;
        public Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao;
        public bool devolucaoParcial;
        public List<Mobile.ControleEntrega.CargaEntregaNotaFiscal> notasFiscais;
        public List<Mobile.ControleEntrega.Produto> produtos;
        public int motivoFalhaGTA;
        public bool apenasRegistrar;
        public Mobile.Cargas.DadosRecebedor dadosRecebedor;
        public bool permitirEntregarMaisTarde;
        public bool atendimentoRegistradoPeloMotorista;
        public List<SituacaoNotaFiscal> situacoesNotasFiscaisNaoAtualizar;
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega OrigemSituacaoEntrega;
        public int quantidadeImagens;
        public List<string> imagens;
        public decimal valorChamado;
        public int codigoCarga;

        #endregion Atributos

        #region Atributos - Carga e Descarga

        public DateTime? dataInicioCarregamento;
        public DateTime? dataTerminoCarregamento;
        public DateTime? dataInicioDescarga;
        public DateTime? dataTerminoDescarga;
        public DateTime? dataPrevisaoEntrega;

        #endregion Atributos - Carga e Descarga

        #region Atributos - Confirmação de Chegada

        public DateTime? dataConfirmacaoChegada;
        public Logistica.WayPoint wayPointConfirmacaoChegada;

        #endregion Atributos - Confirmação de Chegada
    }
}
