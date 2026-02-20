using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class Atendimento
    {
        public int Protocolo { get; set; }

        public int ProtocoloCarga { get; set; }

        public int CodigoEntrega { get; set; }

        public int Numero { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataFinalizacao { get; set; }

        public string Situacao { get; set; }

        public string Observacao { get; set; }

        public MotivoAtendimento Motivo { get; set; }

        public AtendimentoDevolucao Devolucao { get; set; }

        public Cliente Cliente { get; set; }

        public Usuario Responsavel { get; set; }

        public Setor SetorResponsavel { get; set; }

        public DateTime? DataEstorno { get; set; }

        public Usuario UsuarioEstorno { get; set; }

        public List<int> ProtocolosOcorrencias { get; set; }

        public List<ChamadoAnalise> Analises { get; set; }
    }
}
