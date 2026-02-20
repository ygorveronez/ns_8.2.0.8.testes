using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.ProcessadorTarefas
{
    [BsonIgnoreExtraElements]
    public class ProcessamentoTarefa
    {
        public ProcessamentoTarefa()
        {
            Id = ObjectId.GenerateNewId().ToString();
            Status = StatusTarefa.Aguardando;
            EtapaAtual = 0;
            CriadoEm = DateTime.UtcNow;
            AtualizadoEm = DateTime.UtcNow;
            Etapas = new List<EtapaInfo>();
            Versao = 1;
        }

        [BsonId]
        public string Id { get; set; }

        public StatusTarefa Status { get; set; }

        public TipoRequest TipoRequest { get; set; }

        public int EtapaAtual { get; set; }

        public string RequestId { get; set; }

        public DateTime CriadoEm { get; set; }

        public DateTime AtualizadoEm { get; set; }

        public int? CodigoIntegradora { get; set; }

        public string JobId { get; set; }

        public List<EtapaInfo> Etapas { get; set; }

        public BsonDocument Resultado { get; set; }

        public int Versao { get; set; }

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
        public string TipoRequestFormatado
        {
            get { return TipoRequest.ObterDescricao(); }
        }

        [BsonIgnore]
        public EtapaInfo ObterEtapaAtual
        {
            get { return Etapas.ElementAtOrDefault(EtapaAtual); }
        }

        [BsonIgnore]
        public string EtapaAtualFormatada
        {
            get { return ObterEtapaAtual?.TipoFormatado ?? string.Empty; }
        }

        [BsonIgnore]
        public string EtapaAtualMensagemFormatada
        {
            get { return ObterEtapaAtual?.Mensagem ?? string.Empty; }
        }

        [BsonIgnore]
        public string CriadoEmFormatado
        {
            get { return CriadoEm.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"); }
        }

        [BsonIgnore]
        public string AtualizadoEmFormatado
        {
            get { return AtualizadoEm.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"); }
        }

        [BsonIgnore]
        public int ProgressoPercentual
        {
            get
            {
                if (Etapas == null || Etapas.Count == 0)
                    return 0;

                int etapasConcluidas = Etapas.Count(e => e.Status == StatusTarefa.Concluida);
                return (int)((double)etapasConcluidas / Etapas.Count * 100);
            }
        }
    }
}

