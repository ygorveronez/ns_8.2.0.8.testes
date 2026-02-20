using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Auditoria
{
    public static class Auditoria
    {
        public static void Auditar(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, dynamic entidade, string descricaoAcao, Repositorio.UnitOfWork unitOfWork)
        {
            if (auditado == null)
                return;

            if (string.IsNullOrWhiteSpace(descricaoAcao))
                throw new Dominio.Excecoes.Embarcador.ServicoException("Descrição da ação não informada");

            Auditar(auditado, entidade, alteracoes: null, descricaoAcao: descricaoAcao, unitOfWork: unitOfWork, acao: AcaoBancoDados.Registro);
        }

        public static async Task AuditarAsync(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, dynamic entidade, string descricaoAcao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (auditado == null)
                return;

            if (string.IsNullOrWhiteSpace(descricaoAcao))
                throw new Dominio.Excecoes.Embarcador.ServicoException("Descrição da ação não informada");

            await AuditarAsync(auditado, entidade, alteracoes: null, descricaoAcao: descricaoAcao, unitOfWork: unitOfWork, acao: AcaoBancoDados.Registro, cancellationToken: cancellationToken);
        }

        public static void Auditar(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, dynamic entidade, List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes, string descricaoAcao, Repositorio.UnitOfWork unitOfWork, AcaoBancoDados acao = AcaoBancoDados.Registro)
        {
            Repositorio.Auditoria.HistoricoObjeto repositorioHistoricoObjeto = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork);
            string nomeEntidade = entidade.GetType().Name.Replace("ProxyForFieldInterceptor", "").Replace("Proxy", "");

            Dominio.Entidades.Auditoria.HistoricoObjeto historico = new Dominio.Entidades.Auditoria.HistoricoObjeto
            {
                CodigoObjeto = (long)entidade.Codigo,
                Data = DateTime.Now,
                Objeto = nomeEntidade,
                Acao = acao,
                Descricao = ((string)entidade.Descricao).Left(300),
                DescricaoAcao = string.IsNullOrEmpty(descricaoAcao) ? acao.ObterDescricao() : Utilidades.String.Left(descricaoAcao, 200),
                Empresa = auditado.Empresa,
                Usuario = auditado.Usuario,
                UsuarioMultisoftware = auditado.Usuario?.DescricaoUsuarioInterno.Left(200) ?? string.Empty,
                IP = auditado.IP,
                Integradora = auditado.Integradora,
                TipoAuditado = auditado.TipoAuditado,
                OrigemAuditado = auditado.OrigemAuditado
            };

            if (alteracoes != null)
                historico.Propriedades = alteracoes;

            repositorioHistoricoObjeto.Inserir(historico);
        }

        public static async Task AuditarAsync(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, dynamic entidade, List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes, string descricaoAcao, Repositorio.UnitOfWork unitOfWork, AcaoBancoDados acao = AcaoBancoDados.Registro, CancellationToken cancellationToken = default)
        {
            Repositorio.Auditoria.HistoricoObjeto repositorioHistoricoObjeto = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork, cancellationToken);
            string nomeEntidade = entidade.GetType().Name.Replace("ProxyForFieldInterceptor", "").Replace("Proxy", "");

            Dominio.Entidades.Auditoria.HistoricoObjeto historico = new Dominio.Entidades.Auditoria.HistoricoObjeto
            {
                CodigoObjeto = (long)entidade.Codigo,
                Data = DateTime.Now,
                Objeto = nomeEntidade,
                Acao = acao,
                Descricao = ((string)entidade.Descricao).Left(300),
                DescricaoAcao = string.IsNullOrEmpty(descricaoAcao) ? acao.ObterDescricao() : Utilidades.String.Left(descricaoAcao, 200),
                Empresa = auditado.Empresa,
                Usuario = auditado.Usuario,
                UsuarioMultisoftware = auditado.Usuario?.DescricaoUsuarioInterno.Left(200) ?? string.Empty,
                IP = auditado.IP,
                Integradora = auditado.Integradora,
                TipoAuditado = auditado.TipoAuditado,
                OrigemAuditado = auditado.OrigemAuditado
            };

            if (alteracoes != null)
                historico.Propriedades = alteracoes;

            await repositorioHistoricoObjeto.InserirAsync(historico);
        }

        public static void AuditarComAlteracoesRealizadas(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, dynamic entidade, List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes, string descricaoAcao, Repositorio.UnitOfWork unitOfWork, AcaoBancoDados acao = AcaoBancoDados.Registro)
        {
            if (auditado == null)
                return;

            if ((alteracoes == null) || (alteracoes.Count == 0))
                return;

            Auditar(auditado, entidade, alteracoes, descricaoAcao, unitOfWork, acao);
        }

        public static async Task AuditarComAlteracoesRealizadasAsync(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, dynamic entidade, List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes, string descricaoAcao, Repositorio.UnitOfWork unitOfWork, AcaoBancoDados acao = AcaoBancoDados.Registro, CancellationToken cancellationToken = default)
        {
            if (auditado == null)
                return;

            if ((alteracoes == null) || (alteracoes.Count == 0))
                return;

            await AuditarAsync(auditado, entidade, alteracoes, descricaoAcao, unitOfWork, acao, cancellationToken);
        }

        public static void AuditarConsulta(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string descricaoAcao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            if (repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao()?.AuditarConsultasWebService ?? false)
                Auditar(auditado, new Dominio.ObjetosDeValor.ConsultaPadrao(), descricaoAcao, unitOfWork);
        }

        public static async Task AuditarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string descricaoAcao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken = default)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repositorioConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();

            if (configuracao?.AuditarConsultasWebService ?? false)
                await AuditarAsync(auditado, new Dominio.ObjetosDeValor.ConsultaPadrao(), descricaoAcao, unitOfWork, cancellationToken);
        }

        public static void AuditarSemDadosUsuario(dynamic entidade, string descricaoAcao, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(descricaoAcao))
                throw new Dominio.Excecoes.Embarcador.ServicoException("Descrição da ação não informada");

            Repositorio.Auditoria.HistoricoObjeto repositorioHistoricoObjeto = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork);
            string nomeEntidade = entidade.GetType().Name.Replace("ProxyForFieldInterceptor", "").Replace("Proxy", "");

            Dominio.Entidades.Auditoria.HistoricoObjeto historico = new Dominio.Entidades.Auditoria.HistoricoObjeto
            {
                CodigoObjeto = (long)entidade.Codigo,
                Data = DateTime.Now,
                Objeto = nomeEntidade,
                Acao = AcaoBancoDados.Registro,
                Descricao = ((string)entidade.Descricao).Left(300),
                DescricaoAcao = descricaoAcao,
                Empresa = null,
                Usuario = null,
                IP = string.Empty,
                Integradora = null,
                TipoAuditado = TipoAuditado.Sistema,
                OrigemAuditado = OrigemAuditado.Sistema
            };

            repositorioHistoricoObjeto.Inserir(historico);
        }

        public static void AuditarSemEntidade(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, long codigoEntidade, string nomeEntidade, string descricaoEntidade, string descricaoAcao, Repositorio.UnitOfWork unitOfWork)
        {
            AuditarSemEntidade(auditado, codigoEntidade, nomeEntidade, descricaoEntidade, alteracoes: null, descricaoAcao, unitOfWork);
        }

        public static void AuditarSemEntidade(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, long codigoEntidade, string nomeEntidade, string descricaoEntidade, List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes, string descricaoAcao, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(descricaoAcao))
                throw new Dominio.Excecoes.Embarcador.ServicoException("Descrição da ação não informada");

            Repositorio.Auditoria.HistoricoObjeto repositorioHistoricoObjeto = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork);
            Dominio.Entidades.Auditoria.HistoricoObjeto historico = new Dominio.Entidades.Auditoria.HistoricoObjeto
            {
                CodigoObjeto = codigoEntidade,
                Data = DateTime.Now,
                Objeto = nomeEntidade,
                Acao = AcaoBancoDados.Registro,
                Descricao = descricaoEntidade.Left(300),
                DescricaoAcao = descricaoAcao,
                Empresa = auditado.Empresa,
                Usuario = auditado.Usuario,
                UsuarioMultisoftware = auditado.Usuario?.DescricaoUsuarioInterno.Left(200) ?? string.Empty,
                IP = auditado.IP,
                Integradora = auditado.Integradora,
                TipoAuditado = auditado.TipoAuditado,
                OrigemAuditado = auditado.OrigemAuditado
            };

            if (alteracoes != null)
                historico.Propriedades = alteracoes;

            repositorioHistoricoObjeto.Inserir(historico);
        }

        public static void TrocarAuditoria<T>(T entidadeAtual, T novaEntidade, Repositorio.UnitOfWork unitOfWork) where T : Dominio.Entidades.EntidadeBase
        {
            string nomeEntidade = entidadeAtual.GetType().Name.Replace("ProxyForFieldInterceptor", "").Replace("Proxy", "");
            long codigoEntidadeAtual = Convert.ToInt64(entidadeAtual.GetType().GetProperty("Codigo").GetValue(entidadeAtual));
            long codigoNovaEntidade = Convert.ToInt64(novaEntidade.GetType().GetProperty("Codigo").GetValue(novaEntidade));
            Repositorio.Auditoria.HistoricoObjeto repositorioHistoricoObjeto = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork);

            repositorioHistoricoObjeto.TrocarAuditoria(nomeEntidade, codigoEntidadeAtual, codigoNovaEntidade);
        }

        public static List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> CombinarAlteracoes(List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes)
        {
            // Dicionário para armazenar alterações temporárias por propriedade
            var grupos = new Dictionary<string, List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>>();

            // Agrupa alterações por propriedade
            foreach (var alteracao in alteracoes)
            {
                if (!grupos.ContainsKey(alteracao.Propriedade))
                {
                    grupos[alteracao.Propriedade] = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();
                }
                grupos[alteracao.Propriedade].Add(alteracao);
            }

            var resultado = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            // Processa cada grupo de alterações
            foreach (var grupo in grupos.Values)
            {
                var paresDe = new Queue<string>(grupo.Where(a => !string.IsNullOrEmpty(a.De)).Select(a => a.De));
                var paresPara = new Queue<string>(grupo.Where(a => !string.IsNullOrEmpty(a.Para)).Select(a => a.Para));

                // Processa pares e combina `De` e `Para`
                while (paresDe.Count > 0 || paresPara.Count > 0)
                {
                    var de = paresDe.Count > 0 ? paresDe.Dequeue() : "";
                    var para = paresPara.Count > 0 ? paresPara.Dequeue() : "";

                    resultado.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade
                    {
                        Propriedade = grupo.First().Propriedade,
                        De = de,
                        Para = para
                    });
                }
            }

            return resultado;
        }
    }
}
