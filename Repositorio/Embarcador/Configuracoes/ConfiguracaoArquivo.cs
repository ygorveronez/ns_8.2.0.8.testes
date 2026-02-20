using Confluent.Kafka;
using NHibernate.Criterion;
using NHibernate.Hql.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoArquivo : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo>
    {
        public ConfiguracaoArquivo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoArquivo BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo>();

            return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoArquivo
            {

                Codigo = o.Codigo,
                CaminhoRelatorios = o.CaminhoRelatorios,
                CaminhoTempArquivosImportacao = o.CaminhoTempArquivosImportacao,
                CaminhoCanhotos = o.CaminhoCanhotos,
                CaminhoCanhotosAvulsos = o.CaminhoCanhotosAvulsos,
                CaminhoXMLNotaFiscalComprovanteEntrega = o.CaminhoXMLNotaFiscalComprovanteEntrega,
                CaminhoArquivosIntegracao = o.CaminhoArquivosIntegracao,
                CaminhoRelatoriosEmbarcador = o.CaminhoRelatoriosEmbarcador,
                CaminhoLogoEmbarcador = o.CaminhoLogoEmbarcador,
                CaminhoDocumentosFiscaisEmbarcador = o.CaminhoDocumentosFiscaisEmbarcador,
                Anexos = o.Anexos,
                CaminhoGeradorRelatorios = o.CaminhoGeradorRelatorios,
                CaminhoArquivosEmpresas = o.CaminhoArquivosEmpresas,
                CaminhoRelatoriosCrystal = o.CaminhoRelatoriosCrystal,
                CaminhoRetornoXMLIntegrador = o.CaminhoRetornoXMLIntegrador,
                CaminhoArquivos = o.CaminhoArquivos,
                CaminhoArquivosIntegracaoEDI = o.CaminhoArquivosIntegracaoEDI,
                CaminhoArquivosImportacaoBoleto = o.CaminhoArquivosImportacaoBoleto,
                CaminhoOcorrencias = o.CaminhoOcorrencias,
                CaminhoOcorrenciasMobiles = o.CaminhoOcorrenciasMobiles,
                CaminhoArquivosImportacaoXMLNotaFiscal = o.CaminhoArquivosImportacaoXMLNotaFiscal,
                CaminhoDestinoXML = o.CaminhoDestinoXML,
                CaminhoCanhotosAntigos = o.CaminhoCanhotosAntigos,
                CaminhoRaiz = o.CaminhoRaiz,
                CaminhoGuia = o.CaminhoGuia,
                CaminhoDanfeSMS = o.CaminhoDanfeSMS,
                CaminhoRaizFTP = o.CaminhoRaizFTP,
                CaminhoDocumentosINPUT = o.CaminhoDocumentosINPUT,
                CaminhoDocumentosOUTPUT = o.CaminhoDocumentosOUTPUT
            }).FirstOrDefault();

        }

        public Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoArquivo BuscarConfiguracaoDebugLocal() // esta função pode ser manipulada para atender debugs locais
        {
            var config = BuscarConfiguracaoPadrao();

            if (config == null)
                config = new Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoArquivo();

            var fields = config.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(f => f.FieldType == typeof(string));

            foreach (var field in fields)
            {
                var valorAtual = (string)field.GetValue(config);
                if (valorAtual == null)
                    valorAtual = "";
                else
                {
                    System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(valorAtual, @"\b[A-Z]:");

                    if (match.Success)
                        valorAtual = valorAtual.Replace(match.Value, "C:");

                    field.SetValue(config, valorAtual);
                }
            }

            var fieldsBool = config.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(f => f.FieldType == typeof(bool) || f.FieldType == typeof(bool?));

            foreach (var field in fieldsBool)
            {
                var valorAtual = (bool?)field.GetValue(config);
                if (valorAtual == null)
                {
                    valorAtual = false;
                    field.SetValue(config, valorAtual);
                }
            }

            return config;
        }
    }
}
