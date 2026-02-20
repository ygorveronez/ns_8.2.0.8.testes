using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Dominio.Entidades.ProcessadorTarefas
{
    [BsonIgnoreExtraElements]
    public class EtapaInfo
    {
        public EtapaInfo()
        {
            Status = StatusTarefa.Aguardando;
            Tentativas = 0;
        }

        public TipoEtapaTarefa Tipo { get; set; }

        public StatusTarefa Status { get; set; }

        public int Tentativas { get; set; }

        public DateTime? IniciadoEm { get; set; }

        public DateTime? ConcluidoEm { get; set; }

        public string Mensagem { get; set; }

        public long? DuracaoMs { get; set; }

        #region Propriedades Formatadas

        [BsonIgnore]
        public string TipoFormatado
        {
            get { return Tipo.ObterDescricao(); }
        }

        [BsonIgnore]
        public string StatusFormatado
        {
            get { return Status.ObterDescricao(); }
        }

        [BsonIgnore]
        public string IniciadoEmFormatado
        {
            get { return IniciadoEm?.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty; }
        }

        [BsonIgnore]
        public string ConcluidoEmFormatado
        {
            get { return ConcluidoEm?.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty; }
        }

        #endregion
    }
}

