using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using NHibernate.Linq;

namespace Repositorio.Embarcador.ConfiguracaoContabil
{
    public class DocumentoContabil : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>
    {
        public DocumentoContabil(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> BuscarPorDocumentoProvisao(int codigoDocumentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>();
            query = query.Where(obj => obj.DocumentoProvisao.Codigo == codigoDocumentoProvisao);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> BuscarPorDocumentoFaturamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>();
            query = query.Where(obj => obj.DocumentoFaturamento.Codigo == codigo);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> ConsultarPorChave(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>();
            var result = from o in query where o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoContabil.Consolidado select o;

            if (!string.IsNullOrWhiteSpace(chave))
                result = result.Where(o => o.XMLNotaFiscal.Chave == chave);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> BuscarPorProvisao(int codigoProvisao)
        {
            var consultaDocumentoContabil = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>()
                .Where(documento => documento.Provisao.Codigo == codigoProvisao && documento.CancelamentoProvisao == null);

            consultaDocumentoContabil = consultaDocumentoContabil
                .Fetch(documento => documento.DocumentoProvisao)
                .Fetch(documento => documento.XMLNotaFiscal)
                .Fetch(documento => documento.Stage);

            return consultaDocumentoContabil.ToList();
        }

        public List<(int codigoProvisao, decimal valorContabilizacao)> BuscarTotalReceberPorProvisoes(List<int> codigosDocumentosProvisao)
        {
            var consultaDocumentoContabil = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>()
                .Where(documento => codigosDocumentosProvisao.Contains(documento.DocumentoProvisao.Codigo) && documento.CancelamentoProvisao == null && documento.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.TotalReceber);


            return consultaDocumentoContabil.Select(o => ValueTuple.Create(o.DocumentoProvisao.Codigo, o.ValorContabilizacao)).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> BuscarCTesParaGeracaoEDIPorPagamento(int codigoPagamento, bool lotePagamentoLiberado)
        {
            var query = _BuscarCTesParaGeracaoEDIPorPagamento(codigoPagamento, lotePagamentoLiberado);

            return query
                .GroupBy(obj => new
                {
                    CodigoPlanoConta = obj.PlanoConta.PlanoContabilidade,
                    DescricaoPlanoConta = obj.PlanoConta.Descricao,
                    CentroCusto = obj.CentroResultado.PlanoContabilidade,
                    obj.TipoContabilizacao,
                    obj.TipoContaContabil,
                    obj.CTe.Numero,
                    CodigoCTe = obj.CTe.Codigo,
                    Serie = obj.CTe.Serie.Numero,
                    TipoTomador = obj.CTe.TomadorPagador.Cliente.Tipo,
                    CNPJCPFTomador = obj.Tomador.CPF_CNPJ,
                    CodigoIntegracaoTomador = obj.CTe.TomadorPagador.Cliente.CodigoIntegracao,
                    GrupoTomador = obj.CTe.TomadorPagador.Cliente.GrupoPessoas.Descricao,
                    TipoRemetente = obj.CTe.Remetente.Cliente.Tipo,
                    CNPJCPFRemetente = obj.CTe.Remetente.Cliente.CPF_CNPJ,
                    CodigoIntegracaoRemetente = obj.CTe.Remetente.Cliente.CodigoIntegracao,
                    TipoDestinatario = obj.CTe.Destinatario.Cliente.Tipo,
                    CPFCNPJDestinatario = obj.CTe.Destinatario.Cliente.CPF_CNPJ,
                    CodigoIntegracaoDestinatario = obj.CTe.Destinatario.Cliente.CodigoIntegracao,
                    CodigoIntegracaoEmpresa = obj.CTe.Empresa.CodigoIntegracao,
                    DataEmissao = obj.DataEmissaoCTe,
                    obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao,
                    CNPJEmpresa = obj.CTe.Empresa.CNPJ,
                    NumeroModelo = obj.CTe.ModeloDocumentoFiscal.Numero,
                    obj.CTe.Titulo.DataVencimento,
                    NumeroCarga = obj.Carga.CodigoCargaEmbarcador,
                    obj.CargaOcorrencia.NumeroOcorrencia,
                    obj.CTe.CST,
                    obj.CTe.ValorAReceber,
                    obj.CTe.BaseCalculoICMS,
                    obj.CTe.BaseCalculoISS,
                    obj.CTe.AliquotaICMS,
                    obj.CTe.AliquotaISS,
                    obj.CTe.ValorICMS,
                    obj.CTe.ValorISS,
                    TipoOperacao = obj.TipoOperacao.Descricao,
                    TipoOcorrencia = obj.CargaOcorrencia.TipoOcorrencia.Descricao,
                    CodigoCarga = obj.Carga.Codigo,
                    PrefixoOcorrenciaOutrosDocumentos = obj.CargaOcorrencia.TipoOcorrencia.PrefixoFaturamentoOutrosModelos
                })
                .Select(obj => new Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil
                {
                    CodigoContaContabil = obj.Key.CodigoPlanoConta,
                    DescricaoContaContabil = obj.Key.DescricaoPlanoConta,
                    ValorContabilizacao = obj.Sum(dc => dc.ValorContabilizacao),
                    CodigoCentroResultado = obj.Key.CentroCusto,
                    TipoContabilizacao = obj.Key.TipoContabilizacao,
                    Numero = obj.Key.Numero,
                    CodigoCTe = obj.Key.CodigoCTe,
                    Serie = obj.Key.Serie,
                    CNPJCPFTomador = obj.Key.CNPJCPFTomador,
                    CNPJDestinatario = obj.Key.CPFCNPJDestinatario,
                    CNPJEmpresa = obj.Key.CNPJEmpresa,
                    DataEmissao = obj.Key.DataEmissao,
                    GrupoTomador = obj.Key.GrupoTomador,
                    CodigoDestinatario = obj.Key.CodigoIntegracaoDestinatario,
                    CodigoEmpresa = obj.Key.CodigoIntegracaoEmpresa,
                    TipoContaContabil = obj.Key.TipoContaContabil,
                    CodigoTomador = obj.Key.CodigoIntegracaoTomador,
                    TipoDocumentoEmissao = obj.Key.TipoDocumentoEmissao,
                    TipoTomador = obj.Key.TipoTomador,
                    TipoDestinatario = obj.Key.TipoDestinatario,
                    NumeroModelo = obj.Key.NumeroModelo,
                    DataVencimento = obj.Key.DataVencimento,
                    NumeroCarga = obj.Key.NumeroCarga,
                    NumeroOcorrencia = obj.Key.NumeroOcorrencia,
                    CST = obj.Key.CST,
                    ValorAReceber = obj.Key.ValorAReceber,
                    BaseCalculoICMS = obj.Key.BaseCalculoICMS,
                    BaseCalculoIIS = obj.Key.BaseCalculoISS,
                    AliquotaICMS = obj.Key.AliquotaICMS,
                    AliquotaISS = obj.Key.AliquotaISS,
                    ValorICMS = obj.Key.ValorICMS,
                    ValorISS = obj.Key.ValorISS,
                    TipoRemetente = obj.Key.TipoRemetente,
                    CNPJRemetente = obj.Key.CNPJCPFRemetente,
                    CodigoRemetente = obj.Key.CodigoIntegracaoRemetente,
                    TipoOperacao = obj.Key.TipoOperacao,
                    TipoOcorrencia = obj.Key.TipoOcorrencia,
                    CodigoCarga = obj.Key.CodigoCarga,
                    PrefixoOcorrenciaOutrosDocumentos = obj.Key.PrefixoOcorrenciaOutrosDocumentos
                })
                .ToList();
        }

        public int ContarCTesParaGeracaoEDIPorPagamento(int codigoPagamento, bool lotePagamentoLiberado)
        {
            var query = _BuscarCTesParaGeracaoEDIPorPagamento(codigoPagamento, lotePagamentoLiberado);

            return query.Count();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> BuscarNFSParaGeracaoEDIPorPagamento(int codigoPagamento, bool lotePagamentoLiberado)
        {
            var query = _BuscarNFSParaGeracaoEDIPorPagamento(codigoPagamento, lotePagamentoLiberado);

            return query
                .GroupBy(obj => new
                {
                    CodigoPlanoConta = obj.PlanoConta.PlanoContabilidade,
                    DescricaoPlanoConta = obj.PlanoConta.Descricao,
                    CentroCusto = obj.CentroResultado.PlanoContabilidade,
                    obj.TipoContabilizacao,
                    obj.TipoContaContabil,
                    obj.CTe.Numero,
                    CodigoCTe = obj.CTe.Codigo,
                    Serie = obj.CTe.Serie.Numero,
                    TipoTomador = obj.CTe.TomadorPagador.Cliente.Tipo,
                    GrupoTomador = obj.CTe.TomadorPagador.Cliente.GrupoPessoas.Descricao,
                    CNPJCPFTomador = obj.Tomador.CPF_CNPJ,
                    CodigoIntegracaoTomador = obj.CTe.TomadorPagador.Cliente.CodigoIntegracao,
                    TipoRemetente = obj.CTe.Remetente.Cliente.Tipo,
                    CNPJCPFRemetente = obj.CTe.Remetente.Cliente.CPF_CNPJ,
                    CodigoIntegracaoRemetente = obj.CTe.Remetente.Cliente.CodigoIntegracao,
                    TipoDestinatario = obj.CTe.Destinatario.Cliente.Tipo,
                    CPFCNPJDestinatario = obj.CTe.Destinatario.Cliente.CPF_CNPJ,
                    CodigoIntegracaoDestinatario = obj.CTe.Destinatario.Cliente.CodigoIntegracao,
                    CodigoIntegracaoEmpresa = obj.CTe.Empresa.CodigoIntegracao,
                    DataEmissao = obj.DataEmissaoCTe,
                    obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao,
                    CNPJEmpresa = obj.CTe.Empresa.CNPJ,
                    NumeroModelo = obj.CTe.ModeloDocumentoFiscal.Numero,
                    obj.CTe.Titulo.DataVencimento,
                    obj.CTe.CST,
                    obj.CTe.ValorAReceber,
                    obj.CTe.BaseCalculoICMS,
                    obj.CTe.BaseCalculoISS,
                    obj.CTe.AliquotaICMS,
                    obj.CTe.AliquotaISS,
                    obj.CTe.ValorICMS,
                    obj.CTe.ValorISS,
                    PrefixoOcorrenciaOutrosDocumentos = obj.CargaOcorrencia.TipoOcorrencia.PrefixoFaturamentoOutrosModelos
                })
                .Select(obj => new Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil
                {
                    CodigoContaContabil = obj.Key.CodigoPlanoConta,
                    DescricaoContaContabil = obj.Key.DescricaoPlanoConta,
                    ValorContabilizacao = obj.Sum(dc => dc.ValorContabilizacao),
                    CodigoCentroResultado = obj.Key.CentroCusto,
                    TipoContabilizacao = obj.Key.TipoContabilizacao,
                    Numero = obj.Key.Numero,
                    CodigoCTe = obj.Key.CodigoCTe,
                    Serie = obj.Key.Serie,
                    CNPJCPFTomador = obj.Key.CNPJCPFTomador,
                    CNPJDestinatario = obj.Key.CPFCNPJDestinatario,
                    CNPJEmpresa = obj.Key.CNPJEmpresa,
                    DataEmissao = obj.Key.DataEmissao,
                    GrupoTomador = obj.Key.GrupoTomador,
                    CodigoDestinatario = obj.Key.CodigoIntegracaoDestinatario,
                    CodigoEmpresa = obj.Key.CodigoIntegracaoEmpresa,
                    TipoContaContabil = obj.Key.TipoContaContabil,
                    CodigoTomador = obj.Key.CodigoIntegracaoTomador,
                    TipoDocumentoEmissao = obj.Key.TipoDocumentoEmissao,
                    TipoTomador = obj.Key.TipoTomador,
                    TipoDestinatario = obj.Key.TipoDestinatario,
                    NumeroModelo = obj.Key.NumeroModelo,
                    DataVencimento = obj.Key.DataVencimento,
                    CST = obj.Key.CST,
                    ValorAReceber = obj.Key.ValorAReceber,
                    BaseCalculoICMS = obj.Key.BaseCalculoICMS,
                    BaseCalculoIIS = obj.Key.BaseCalculoISS,
                    AliquotaICMS = obj.Key.AliquotaICMS,
                    AliquotaISS = obj.Key.AliquotaISS,
                    ValorICMS = obj.Key.ValorICMS,
                    ValorISS = obj.Key.ValorISS,
                    TipoRemetente = obj.Key.TipoRemetente,
                    CNPJRemetente = obj.Key.CNPJCPFRemetente,
                    CodigoRemetente = obj.Key.CodigoIntegracaoRemetente,
                    PrefixoOcorrenciaOutrosDocumentos = obj.Key.PrefixoOcorrenciaOutrosDocumentos
                })
                .ToList();
        }

        public int ContarNFSParaGeracaoEDIPorPagamento(int codigoPagamento, bool lotePagamentoLiberado)
        {
            var query = _BuscarNFSParaGeracaoEDIPorPagamento(codigoPagamento, lotePagamentoLiberado);

            //var cuzinho = (from obj in query
            //               group obj by new
            //               {
            //                   CodigoPlanoConta = obj.PlanoConta.PlanoContabilidade,
            //                   DescricaoPlanoConta = obj.PlanoConta.Descricao,
            //                   CentroCusto = obj.CentroResultado.PlanoContabilidade,
            //                   obj.TipoContabilizacao,
            //                   obj.TipoContaContabil,
            //                   obj.CTe.Numero,
            //                   CodigoCTe = obj.CTe.Codigo,
            //                   Serie = obj.CTe.Serie.Numero,
            //                   TipoTomador = obj.CTe.TomadorPagador.Cliente.Tipo,
            //                   CNPJCPFTomador = obj.Tomador.CPF_CNPJ,
            //                   CodigoIntegracaoTomador = obj.CTe.TomadorPagador.Cliente.CodigoIntegracao,
            //                   TipoRemetente = obj.CTe.Remetente.Cliente.Tipo,
            //                   CNPJCPFRemetente = obj.CTe.Remetente.Cliente.CPF_CNPJ,
            //                   CodigoIntegracaoRemetente = obj.CTe.Remetente.Cliente.CodigoIntegracao,
            //                   TipoDestinatario = obj.CTe.Destinatario.Cliente.Tipo,
            //                   CPFCNPJDestinatario = obj.CTe.Destinatario.Cliente.CPF_CNPJ,
            //                   CodigoIntegracaoDestinatario = obj.CTe.Destinatario.Cliente.CodigoIntegracao,
            //                   CodigoIntegracaoEmpresa = obj.CTe.Empresa.CodigoIntegracao,
            //                   DataEmissao = obj.DataEmissaoCTe,
            //                   obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao,
            //                   CNPJEmpresa = obj.CTe.Empresa.CNPJ,
            //                   NumeroModelo = obj.CTe.ModeloDocumentoFiscal.Numero,
            //                   obj.CTe.Titulo.DataVencimento,
            //                   obj.CTe.CST,
            //                   obj.CTe.ValorAReceber,
            //                   obj.CTe.BaseCalculoICMS,
            //                   obj.CTe.BaseCalculoISS,
            //                   obj.CTe.AliquotaICMS,
            //                   obj.CTe.AliquotaISS,
            //                   obj.CTe.ValorICMS,
            //                   obj.CTe.ValorISS
            //               } into grupo select grupo)
            //               .Count();

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> BuscarPorPagamento(int codigoPagamento, bool lotePagamentoLiberado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>();

            if (lotePagamentoLiberado)
                query = query.Where(obj => obj.PagamentoLiberado.Codigo == codigoPagamento);
            else
                query = query.Where(obj => obj.Pagamento.Codigo == codigoPagamento);

            return query.Fetch(obj => obj.DocumentoFaturamento).ToList();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> BuscarPorStage(int codigoStage)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>();

            query = query.Where(obj => obj.Stage.Codigo == codigoStage && obj.Provisao != null);

            return query.Fetch(obj => obj.DocumentoFaturamento).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> BuscarSumarizadoPorPagamento(int codigoPagamento, bool lotePagamentoLiberado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>();

            if (lotePagamentoLiberado)
                query = query.Where(obj => obj.PagamentoLiberado.Codigo == codigoPagamento);
            else
                query = query.Where(obj => obj.Pagamento.Codigo == codigoPagamento);

            return query.GroupBy(obj => new
            {
                CodigoPlanoConta = obj.PlanoConta.PlanoContabilidade,
                DescricaoPlanoConta = obj.PlanoConta.Descricao,
                CentroCusto = obj.CentroResultado.PlanoContabilidade,
                TipoContabilizacao = obj.TipoContabilizacao
            }).Select(obj => new Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil
            {
                CodigoContaContabil = obj.Key.CodigoPlanoConta,
                DescricaoContaContabil = obj.Key.DescricaoPlanoConta,
                ValorContabilizacao = obj.Sum(dc => dc.ValorContabilizacao),
                CodigoCentroResultado = obj.Key.CentroCusto,
                TipoContabilizacao = obj.Key.TipoContabilizacao
            }).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> BuscarParaGeracaoEDIPorCancelamentoProvisao(int codigoCancelamentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>();

            query = query.Where(obj => obj.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao);

            return query.GroupBy(obj => new
            {
                CodigoPlanoConta = obj.PlanoConta.PlanoContabilidade,
                DescricaoPlanoConta = obj.PlanoConta.Descricao,
                CentroCusto = obj.CentroResultado.PlanoContabilidade,
                obj.TipoContabilizacao,
                CNPJCPFTomador = obj.Tomador.CPF_CNPJ,
                obj.DocumentoProvisao.CST,
                obj.TipoContaContabil

            }).Select(obj => new Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil
            {
                CodigoContaContabil = obj.Key.CodigoPlanoConta,
                DescricaoContaContabil = obj.Key.DescricaoPlanoConta,
                ValorContabilizacao = obj.Sum(dc => dc.ValorContabilizacao),
                CodigoCentroResultado = obj.Key.CentroCusto,
                TipoContabilizacao = obj.Key.TipoContabilizacao,
                CST = obj.Key.CST,
                CNPJCPFTomador = obj.Key.CNPJCPFTomador,
                TipoContaContabil = obj.Key.TipoContaContabil
            }).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> BuscarParaGeracaoEDIPorProvisao(int codigoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>();

            query = query.Where(obj => obj.Provisao.Codigo == codigoProvisao && obj.CancelamentoProvisao == null);

            return query.GroupBy(obj => new
            {
                CodigoPlanoConta = obj.PlanoConta.PlanoContabilidade,
                DescricaoPlanoConta = obj.PlanoConta.Descricao,
                CentroCusto = obj.CentroResultado.PlanoContabilidade,
                CST = obj.DocumentoProvisao.CST,
                obj.TipoContabilizacao,
                CNPJCPFTomador = obj.Tomador.CPF_CNPJ,
                obj.TipoContaContabil

            }).Select(obj => new Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil
            {
                CodigoContaContabil = obj.Key.CodigoPlanoConta,
                DescricaoContaContabil = obj.Key.DescricaoPlanoConta,
                ValorContabilizacao = obj.Sum(dc => dc.ValorContabilizacao),
                CodigoCentroResultado = obj.Key.CentroCusto,
                TipoContabilizacao = obj.Key.TipoContabilizacao,
                CST = obj.Key.CST,
                CNPJCPFTomador = obj.Key.CNPJCPFTomador,
                TipoContaContabil = obj.Key.TipoContaContabil
            }).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> BuscarSumarizadoPorProvisao(int codigoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>();

            query = query.Where(obj => obj.Provisao.Codigo == codigoProvisao && obj.CancelamentoProvisao == null);

            return query.GroupBy(obj => new
            {
                CodigoPlanoConta = obj.PlanoConta.PlanoContabilidade,
                DescricaoPlanoConta = obj.PlanoConta.Descricao,
                CentroCusto = obj.CentroResultado.PlanoContabilidade,
                TipoContabilizacao = obj.TipoContabilizacao
            }).Select(obj => new Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil
            {
                CodigoContaContabil = obj.Key.CodigoPlanoConta,
                DescricaoContaContabil = obj.Key.DescricaoPlanoConta,
                ValorContabilizacao = obj.Sum(dc => dc.ValorContabilizacao),
                CodigoCentroResultado = obj.Key.CentroCusto,
                TipoContabilizacao = obj.Key.TipoContabilizacao
            }).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> BuscarSumarizadoPorCancelamentoPagamento(int codigoCancelamentoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>();

            query = query.Where(obj => obj.CancelamentoPagamento.Codigo == codigoCancelamentoPagamento);

            return query.GroupBy(obj => new
            {
                CodigoPlanoConta = obj.PlanoConta.PlanoContabilidade,
                DescricaoPlanoConta = obj.PlanoConta.Descricao,
                CentroCusto = obj.CentroResultado.PlanoContabilidade,
                TipoContabilizacao = obj.TipoContabilizacao
            }).Select(obj => new Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil
            {
                CodigoContaContabil = obj.Key.CodigoPlanoConta,
                DescricaoContaContabil = obj.Key.DescricaoPlanoConta,
                ValorContabilizacao = obj.Sum(dc => dc.ValorContabilizacao),
                CodigoCentroResultado = obj.Key.CentroCusto,
                TipoContabilizacao = obj.Key.TipoContabilizacao
            }).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> BuscarSumarizadoPorCancelamentoProvisao(int codigoCancelamentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>();

            query = query.Where(obj => obj.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao);

            return query.GroupBy(obj => new
            {
                CodigoPlanoConta = obj.PlanoConta.PlanoContabilidade,
                DescricaoPlanoConta = obj.PlanoConta.Descricao,
                CentroCusto = obj.CentroResultado.PlanoContabilidade,
                TipoContabilizacao = obj.TipoContabilizacao
            }).Select(obj => new Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil
            {
                CodigoContaContabil = obj.Key.CodigoPlanoConta,
                DescricaoContaContabil = obj.Key.DescricaoPlanoConta,
                ValorContabilizacao = obj.Sum(dc => dc.ValorContabilizacao),
                CodigoCentroResultado = obj.Key.CentroCusto,
                TipoContabilizacao = obj.Key.TipoContabilizacao
            }).ToList();
        }

        public bool ExisteComStage(int codigoProvisao)
        {
            var consultaDocumentoContabil = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>()
                .Where(documento => documento.Provisao.Codigo == codigoProvisao && documento.CancelamentoProvisao == null && documento.Stage != null);

            return consultaDocumentoContabil.Count() > 0;
        }

        public bool ExisteSemImpostoValorAgregado(int codigoProvisao)
        {
            var consultaDocumentoContabil = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>()
                .Where(documento => documento.Provisao.Codigo == codigoProvisao && documento.CancelamentoProvisao == null && documento.ImpostoValorAgregado == null);

            return consultaDocumentoContabil.Count() > 0;
        }

        public void ExcluirTodosPorProvisao(int codigoProvisao)
        {
            UnitOfWork.Sessao
                .CreateQuery("delete DocumentoContabil documento where documento.Provisao.Codigo = :codigoProvisao")
                .SetInt32("codigoProvisao", codigoProvisao)
                .ExecuteUpdate();
        }

        public void ExcluirTodosPorProvisaoEStage(int codigoProvisao, int codigoStage)
        {
            UnitOfWork.Sessao
                .CreateQuery("delete DocumentoContabil documento where documento.Provisao.Codigo = :codigoProvisao and documento.Stage.Codigo = :codigoStage")
                .SetInt32("codigoProvisao", codigoProvisao)
                .SetInt32("codigoStage", codigoStage)
                .ExecuteUpdate();
        }

        public void ExcluirTodosPorDocumentoProvisao(int codigoDocumentoProvisao)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE DocumentoContabil obj WHERE obj.DocumentoProvisao.Codigo = :codigoDocumentoProvisao")
                             .SetInt32("codigoDocumentoProvisao", codigoDocumentoProvisao)
                             .ExecuteUpdate();
        }

        public void ExcluirTodosPorDocumentoProvisaoReferencia(int codigoDocumentoProvisao)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE DocumentoContabil obj WHERE obj.DocumentoProvisaoReferencia.Codigo = :codigoDocumentoProvisao")
                             .SetInt32("codigoDocumentoProvisao", codigoDocumentoProvisao)
                             .ExecuteUpdate();
        }

        public void ExcluirTodosPorPagamento(int pagamento)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE DocumentoContabil obj WHERE obj.Pagamento.Codigo = :Pagamento")
                             .SetInt32("Pagamento", pagamento)
                             .ExecuteUpdate();
        }

        public void LiberarPorPagamento(int pagamento)
        {
            UnitOfWork.Sessao.CreateQuery("update DocumentoContabil obj set obj.PagamentoLiberado = NULL WHERE obj.PagamentoLiberado.Codigo = :PagamentoLiberado")
                             .SetInt32("PagamentoLiberado", pagamento)
                             .ExecuteUpdate();
        }

        public void ExcluirTodosPorCancelamentoPagamento(int codigo)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE DocumentoContabil obj WHERE obj.CancelamentoPagamento.Codigo = :CancelamentoPagamento")
                             .SetInt32("CancelamentoPagamento", codigo)
                             .ExecuteUpdate();
        }

        public void SetarReferenciaProvisaoNulaPorCarga(int carga)
        {
            string hql = "update DocumentoContabil obj set obj.DocumentoProvisaoReferencia = NULL where obj.Carga= :Carga ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Carga", carga);
            query.ExecuteUpdate();
        }

        public void SetarReferenciaProvisaoNulaPorOcorrencia(int ocorrencia)
        {
            string hql = "update DocumentoContabil obj set obj.DocumentoProvisaoReferencia = NULL where obj.CargaOcorrencia= :CargaOcorrencia ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("CargaOcorrencia", ocorrencia);
            query.ExecuteUpdate();
        }

        public void ConfirmarMovimentoPorProvisao(int provisao, DateTime dataLancamento)
        {
            string hql = "update DocumentoContabil obj set obj.Situacao= :Situacao, obj.DataLancamento= :DataLancamento where obj.Provisao= :Provisao ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Provisao", provisao);
            query.SetDateTime("DataLancamento", dataLancamento);
            query.SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoContabil.Consolidado);
            query.ExecuteUpdate();
        }

        public void ConfirmarMovimentoPorPagamento(int pagamento)
        {
            string hql = "update DocumentoContabil obj set obj.Situacao= :Situacao where obj.Pagamento= :Pagamento ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Pagamento", pagamento);
            query.SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoContabil.Consolidado);
            query.ExecuteUpdate();
        }

        public void ConfirmarMovimentoPorCancelamentoPagamento(int codigo)
        {
            string hql = "update DocumentoContabil obj set obj.Situacao= :Situacao where obj.CancelamentoPagamento= :CancelamentoPagamento ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("CancelamentoPagamento", codigo);
            query.SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoContabil.Consolidado);
            query.ExecuteUpdate();
        }

        public void SetarPagamentoLiberacaoDocumentoContabil(int documentoFaturamento, int pagamentoLiberado)
        {
            string hql = "update DocumentoContabil obj set obj.PagamentoLiberado= :PagamentoLiberado where obj.DocumentoFaturamento= :DocumentoFaturamento ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("DocumentoFaturamento", documentoFaturamento);
            query.SetInt32("PagamentoLiberado", pagamentoLiberado);
            query.ExecuteUpdate();
        }

        public void SetarPagamentosLiberacaoDocumentoContabil(List<int> documentoFaturamento, int pagamentoLiberado)
        {
            string hql = "update DocumentoContabil obj set obj.PagamentoLiberado= :PagamentoLiberado where obj.DocumentoFaturamento IN (:DocumentoFaturamento) ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetParameterList("DocumentoFaturamento", documentoFaturamento);
            query.SetInt32("PagamentoLiberado", pagamentoLiberado);
            query.ExecuteUpdate();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.ConfiguracaoContabil.FreteContabil> ConsultarRelatorioFreteContabil(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaFreteContabil filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFreteContabil = new ConsultaFreteContabil().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaFreteContabil.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.ConfiguracaoContabil.FreteContabil)));

            return consultaFreteContabil.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.ConfiguracaoContabil.FreteContabil>();
        }

        public int ContarConsultaRelatorioFreteContabil(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaFreteContabil filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaFreteContabil = new ConsultaFreteContabil().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaFreteContabil.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> _BuscarCTesParaGeracaoEDIPorPagamento(int codigoPagamento, bool lotePagamentoLiberado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>();

            query = query.Where(obj => obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe);

            if (lotePagamentoLiberado)
                query = query.Where(obj => obj.PagamentoLiberado.Codigo == codigoPagamento);
            else
                query = query.Where(obj => obj.Pagamento.Codigo == codigoPagamento);

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> _BuscarNFSParaGeracaoEDIPorPagamento(int codigoPagamento, bool lotePagamentoLiberado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>();

            query = query.Where(obj => obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe);

            if (lotePagamentoLiberado)
                query = query.Where(obj => obj.PagamentoLiberado.Codigo == codigoPagamento);
            else
                query = query.Where(obj => obj.Pagamento.Codigo == codigoPagamento);

            return query;
        }

        #endregion
    }
}
