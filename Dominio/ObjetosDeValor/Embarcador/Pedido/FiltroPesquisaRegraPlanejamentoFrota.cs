using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class FiltroPesquisaRegraPlanejamentoFrota
    {
        #region Atributos



        #endregion

        #region Propriedades

        public int NumeroSequencial { get; set; }

        public string Descricao { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa? Ativo { get; set; }

        public DateTime? VigenciaInicial { get; set; }

        public DateTime? VigenciaFinal { get; set; }

        public int GrupoPessoa { get; set; }

        public int CidadeOrigem { get; set; }

        public String EstadoOrigem { get; set; }

        public int CidadeDestino { get; set; }

        public String EstadoDestino { get; set; }

        public List<int> TiposOperacao { get; set; }

        public List<int> TipoCarga { get; set; }

        public List<int> CentroResultdo { get; set; }

        public List<int> ModeloVeicular { get; set; }

        public List<int> NivelCooperado { get; set; }
   

        #endregion

        #region Propriedades com Regras



        #endregion
    }
}