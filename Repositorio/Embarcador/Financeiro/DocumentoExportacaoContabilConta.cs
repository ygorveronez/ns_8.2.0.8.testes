using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class DocumentoExportacaoContabilConta : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabilConta>
    {
        public DocumentoExportacaoContabilConta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ExportacaoContabilizacao> BuscarDadosParaExportacao(int codigoLoteContabilizacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabilConta> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabilConta>();

            query = query.Where(o => o.DocumentoExportacaoContabil.LoteContabilizacao.Codigo == codigoLoteContabilizacao);

            return query.GroupBy(o => new
            {
                CodigoContratoFrete = (int?)o.DocumentoExportacaoContabil.ContratoFrete.NumeroContrato, //realmente é o número, pois não pode ir dois contratos com o mesmo número no arquivo (caso cancele um e emita novamente com o mesmo número)
                CodigoCTe = (int?)o.DocumentoExportacaoContabil.CTe.Codigo,
                CNPJEmpresa = o.DocumentoExportacaoContabil.Empresa.CNPJ,
                CodigoCentroResultado = o.CodigoCentroResultado,
                CodigoCentroResultadoCadastro = o.CentroResultado.PlanoContabilidade,
                CodigoCentroResultadoEmpresa = o.DocumentoExportacaoContabil.Empresa.CodigoCentroCusto,
                CodigoContaContabil = o.ContaContabil,
                CodigoContaContabilCadastro = o.PlanoConta.PlanoContabilidade,
                CodigoIntegracaoEmpresa = o.DocumentoExportacaoContabil.Empresa.CodigoIntegracao,
                CodigoIntegracaoTomador = o.DocumentoExportacaoContabil.Tomador.CodigoIntegracao,
                CPFCNPJTomador = o.DocumentoExportacaoContabil.Tomador.CPF_CNPJ,
                DataEmissao = o.DocumentoExportacaoContabil.DataEmissao.Date,
                DataMovimento = o.DocumentoExportacaoContabil.MovimentoFinanceiro.DataMovimento.Date,
                DebitoCredito = o.Tipo,
                Numero = o.DocumentoExportacaoContabil.Numero,
                TipoDocumentoEmissao = (Dominio.Enumeradores.TipoDocumento?)o.DocumentoExportacaoContabil.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao,
                TipoMovimentoExportacao = o.DocumentoExportacaoContabil.TipoMovimento,
                TipoTomador = o.DocumentoExportacaoContabil.Tomador.Tipo,
                TipoDocumento = o.DocumentoExportacaoContabil.TipoDocumento,
                SerieCTe = (int?)o.DocumentoExportacaoContabil.CTe.Serie.Numero,
                TomadorFazParteGrupoEconomico = (bool?)o.DocumentoExportacaoContabil.Tomador.FazParteGrupoEconomico
            }).Select(o => new Dominio.ObjetosDeValor.Embarcador.Financeiro.ExportacaoContabilizacao()
            {
                CodigoContratoFrete = o.Key.CodigoContratoFrete,
                CodigoCTe = o.Key.CodigoCTe,
                CNPJEmpresa = o.Key.CNPJEmpresa,
                CodigoCentroResultado = o.Key.CodigoCentroResultado,
                CodigoCentroResultadoCadastro = o.Key.CodigoCentroResultadoCadastro,
                CodigoCentroResultadoEmpresa = o.Key.CodigoCentroResultadoEmpresa,
                CodigoContaContabil = o.Key.CodigoContaContabil,
                CodigoContaContabilCadastro = o.Key.CodigoContaContabilCadastro,
                CodigoIntegracaoEmpresa = o.Key.CodigoIntegracaoEmpresa,
                CodigoIntegracaoTomador = o.Key.CodigoIntegracaoTomador,
                CPFCNPJTomador = o.Key.CPFCNPJTomador,
                DataEmissao = o.Key.DataEmissao,
                DataMovimento = o.Key.DataMovimento,
                DebitoCredito = o.Key.DebitoCredito,
                Numero = o.Key.Numero,
                TipoDocumento = o.Key.TipoDocumento,
                TipoDocumentoEmissao = o.Key.TipoDocumentoEmissao,
                TipoMovimentoExportacao = o.Key.TipoMovimentoExportacao,
                TipoTomador = o.Key.TipoTomador,
                Valor = o.Sum(x => x.DocumentoExportacaoContabil.Valor),
                SerieCTe = o.Key.SerieCTe,
                TomadorFazParteGrupoEconomico = o.Key.TomadorFazParteGrupoEconomico ?? false
            }).ToList();

            //return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Financeiro.ExportacaoContabilizacao()
            //{
            //    CodigoCTe = o.DocumentoExportacaoContabil.CTe.Codigo,
            //    CNPJEmpresa = o.DocumentoExportacaoContabil.Empresa.CNPJ,
            //    CodigoCentroResultado = o.CodigoCentroResultado,
            //    CodigoCentroResultadoCadastro = o.CentroResultado.PlanoContabilidade,
            //    CodigoCentroResultadoEmpresa = o.DocumentoExportacaoContabil.Empresa.CodigoCentroCusto,
            //    CodigoContaContabil = o.ContaContabil,
            //    CodigoContaContabilCadastro = o.PlanoConta.PlanoContabilidade,
            //    CodigoIntegracaoEmpresa = o.DocumentoExportacaoContabil.Empresa.CodigoIntegracao,
            //    CodigoIntegracaoTomador = o.DocumentoExportacaoContabil.Tomador.CodigoIntegracao,
            //    CPFCNPJTomador = o.DocumentoExportacaoContabil.Tomador.CPF_CNPJ,
            //    DataEmissao = o.DocumentoExportacaoContabil.DataEmissao,
            //    DataMovimento = o.DocumentoExportacaoContabil.MovimentoFinanceiro.DataMovimento.Date,
            //    DebitoCredito = o.Tipo,
            //    Numero = o.DocumentoExportacaoContabil.Numero,
            //    TipoDocumentoEmissao = o.DocumentoExportacaoContabil.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao,
            //    TipoMovimentoExportacao = o.DocumentoExportacaoContabil.TipoMovimento,
            //    TipoTomador = o.DocumentoExportacaoContabil.Tomador.Tipo,
            //    Valor = o.DocumentoExportacaoContabil.Valor,
            //    TipoDocumento = o.DocumentoExportacaoContabil.TipoDocumento
            //}).OrderBy(o => o.TipoMovimentoExportacao).ToList();
        }

    }
}
