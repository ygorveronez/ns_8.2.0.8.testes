using System;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class EAI
    {
        public int sequencia;

        public Dominio.ObjetosDeValor.EDI.EAI.Lote GerarPorLote(Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao loteEDIIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Dominio.ObjetosDeValor.EDI.EAI.Lote EDIlote = new Dominio.ObjetosDeValor.EDI.EAI.Lote();

            Dominio.Entidades.Embarcador.Avarias.Lote lote = loteEDIIntegracao.Lote;
            EDIlote.NumeroDocumento = lote.Numero.ToString();
            EDIlote.CodigoEmitente = lote.Transportador?.CodigoIntegracao ?? string.Empty;
            EDIlote.ContaContabil = lote.MotivoAvaria ?.ContaContabil?.PlanoContabilidade ?? string.Empty;
            EDIlote.DataEmissao = loteEDIIntegracao.DataIntegracao; //lote.DataGeracao;
            EDIlote.DataTransacao = DateTime.Now;
            EDIlote.ValorDesconto = lote.ValorLote;
            EDIlote.ObservacaoDocumento = lote.Transportador?.RazaoSocial ?? string.Empty;

            return EDIlote;
        }
    }
}
