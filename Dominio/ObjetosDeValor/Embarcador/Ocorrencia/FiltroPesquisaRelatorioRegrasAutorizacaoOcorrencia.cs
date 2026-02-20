using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public sealed class FiltroPesquisaRelatorioRegrasAutorizacaoOcorrencia
    {
        public int CodigoAprovador { get; set; }

        public DateTime? DataVgenciaInicial { get; set; }

        public DateTime? DataVigenciaLimite { get; set; }

        public string Descricao { get; set; }

        public bool ExibirAlcadas { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa Ativo { get; set; }

        public Enumeradores.EtapaAutorizacaoOcorrencia? EtapaAutorizacao { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<double> CodigosRecebedores { get; set; }
    }
}
