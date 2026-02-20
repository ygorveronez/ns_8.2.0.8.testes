using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Dominio.Entidades.ProcessadorTarefas
{   
    [BsonIgnoreExtraElements]
    public class RequestSubtarefa
    {
        public RequestSubtarefa()
        {
            Id = ObjectId.GenerateNewId().ToString();
            Status = StatusTarefa.Aguardando;
            Tentativas = 0;
            DataCriacao = DateTime.UtcNow;
        }

        [BsonId]
        public string Id { get; set; }

        public string TarefaId { get; set; }

        public BsonDocument Dados { get; set; }

        public StatusTarefa Status { get; set; }

        public int Ordem { get; set; }

        public int Tentativas { get; set; }

        public DateTime DataCriacao { get; set; }

        public string Mensagem { get; set; }

        public BsonDocument Resultado { get; set; }

        #region Propriedades Formatadas

        [BsonIgnore]
        public string Codigo
        {
            get { return Id; }
        }

        [BsonIgnore]
        public string StatusFormatado
        {
            get { return Status.ObterDescricao(); }
        }

        [BsonIgnore]
        public string DataCriacaoFormatada
        {
            get { return DataCriacao.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"); }
        }

        #endregion
    }
}

