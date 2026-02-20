using MongoDB.Bson;
using System;

namespace Dominio.Entidades.ProcessadorTarefas
{
    public class ArquivoIntegracao
    {
        public ArquivoIntegracao()
        {
            Identifcador = ObjectId.GenerateNewId().ToString();
            DataCriacao = DateTime.UtcNow;
        }

        #region Propriedades

        public string Identifcador { get; set; }

        public BsonDocument ArquivoRequisicao { get; set; }

        public BsonDocument ArquivoResposta { get; set; }

        public string Tipo { get; set; }

        public DateTime DataCriacao { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public string Codigo
        {
            get { return Identifcador; }
        }

        public string DataCriacaoFormatada
        {
            get { return DataCriacao.ToLocalTime().ToDateTimeString(showSeconds: false); }
        }

        #endregion Propriedades com Regras
    }
}
