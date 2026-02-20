using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.ProcessadorTarefas
{
    public class TarefaIntegracao : Integracao
    {
        public TarefaIntegracao()
        {
            Id = ObjectId.GenerateNewId().ToString();
            DataCriacao = DateTime.UtcNow;
            Tentativas = 0;
            SituacaoIntegracao = ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            Arquivos = new List<ArquivoIntegracao>();
        }

        #region Propriedades

        public string Id { get; set; }

        public string IdTarefa { get; set; }

        public List<ArquivoIntegracao> Arquivos { get; set; }

        public DateTime DataCriacao { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public string Codigo
        {
            get { return Id; }
        }

        public string DataCriacaoFormatada
        {
            get { return DataCriacao.ToLocalTime().ToDateTimeString(showSeconds: false); }
        }

        #endregion Propriedades com Regras
    }
}
