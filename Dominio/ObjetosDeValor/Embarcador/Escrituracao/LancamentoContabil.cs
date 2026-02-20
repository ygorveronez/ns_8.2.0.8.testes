using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class LancamentoContabil
    {
        public LancamentoContabil(string titulo, IList<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentos, List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> dados)
        {
            string viagens = documentos != null ? String.Join(", ", (from o in documentos where o.Carga != null && !string.IsNullOrWhiteSpace(o.Carga.CodigoCargaEmbarcador) select o.Carga.CodigoCargaEmbarcador).Distinct()) : "";

            this.Descricao = titulo + (!string.IsNullOrWhiteSpace(viagens) ? " - Viagens " + viagens: "");
            this.ProcessaDados(dados);
        }

        public LancamentoContabil(string titulo, IList<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentos, List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> dados)
        {
            string viagens = documentos != null ? String.Join(", ", (from o in documentos where o.Carga != null && !string.IsNullOrWhiteSpace(o.Carga.CodigoCargaEmbarcador) select o.Carga.CodigoCargaEmbarcador).Distinct()) : "";

            this.Descricao = titulo + (!string.IsNullOrWhiteSpace(viagens) ? " - Viagens " + viagens : "");
            this.ProcessaDados(dados);
        }

        public string Descricao { get; set; }

        public List<LancamentoContabilDetalhe> Lancamentos { get; set; }

        private void ProcessaDados(List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> dados)
        {
            this.Lancamentos = (from o in dados
                                select new LancamentoContabilDetalhe()
                                {
                                    TipoContaContabil = o.TipoContaContabil,
                                    ContaContabil = o.PlanoConta?.BuscarDescricao ?? string.Empty,
                                    CentroCusto = o.CentroResultado?.PlanoContabilidade ?? string.Empty,
                                    TipoContabilizacao = o.TipoContabilizacao,
                                    Valor = o.ValorContabilizacao,
                                    DataLancamento = o.DataEmissao.ToString("dd/MM/yyyy"),
                                }).ToList();
        }
    }
}
