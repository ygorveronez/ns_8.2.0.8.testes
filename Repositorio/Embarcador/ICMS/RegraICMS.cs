using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using LinqKit;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.ICMS
{
    public class RegraICMS : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.ICMS.RegraICMS>
    {
        #region Construtores

        public RegraICMS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos de Consulta

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> BuscarRegrasParaAlerta()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();
            query = query.Where(obj => obj.Ativo == true && obj.VigenciaFim.HasValue && (((TipoRegraICMS?)obj.Tipo).HasValue == false || obj.Tipo == TipoRegraICMS.Ativa));

            var queryAlerta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Alerta>();
            queryAlerta = queryAlerta.Where(obj => obj.TelaAlerta == ControleAlertaTela.Veiculo);
            query = query.Where(o => !queryAlerta.Any(c => c.CodigoEntidade == o.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> BuscarTodosAtivas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            var result = from obj in query where obj.Ativo && (((TipoRegraICMS?)obj.Tipo).HasValue == false || obj.Tipo == TipoRegraICMS.Ativa) select obj;

            return result
                //.Fetch(obj => obj.TipoOperacao)
                .Fetch(obj => obj.CFOP)
                //.Fetch(obj => obj.ProdutoEmbarcador)
                .ToList();
        }

        [Obsolete("Método descontinuado para utilizar a lista em memória no serviço RegrasCalculoImpostos", error: true)]
        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarPorEmpresa(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                          obj.Ativo
                          && obj.Empresa.Codigo == empresa
                          && (obj.VigenciaInicio == null || obj.VigenciaInicio <= dataFiltro.Date)
                          && (obj.VigenciaFim == null || obj.VigenciaFim >= dataFiltro.Date)
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarPorEmpresa(int empresa, IQueryable<Dominio.Entidades.Embarcador.ICMS.RegraICMS> query)
        {

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                          obj.Ativo
                          && obj.Empresa != null && obj.Empresa.Codigo == empresa
                          && (obj.VigenciaInicio == null || obj.VigenciaInicio <= dataFiltro.Date)
                          && (obj.VigenciaFim == null || obj.VigenciaFim >= dataFiltro.Date)
                         select obj;

            return result.ToList();
        }

        [Obsolete("Método descontinuado para utilizar a lista em memória no serviço RegrasCalculoImpostos", error: true)]
        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarPorSetorEmpresa(string setorEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                          obj.Ativo
                          && obj.SetorEmpresa == setorEmpresa
                          && (obj.VigenciaInicio == null || obj.VigenciaInicio <= dataFiltro.Date)
                          && (obj.VigenciaFim == null || obj.VigenciaFim >= dataFiltro.Date)
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarPorSetorEmpresa(string setorEmpresa, IQueryable<Dominio.Entidades.Embarcador.ICMS.RegraICMS> query)
        {

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                          obj.Ativo
                          && obj.SetorEmpresa != null && obj.SetorEmpresa == setorEmpresa
                          && (obj.VigenciaInicio == null || obj.VigenciaInicio <= dataFiltro.Date)
                          && (obj.VigenciaFim == null || obj.VigenciaFim >= dataFiltro.Date)
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarPorNumeroProposta(string numeroProposta, IQueryable<Dominio.Entidades.Embarcador.ICMS.RegraICMS> query)
        {

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                          obj.Ativo
                          && obj.NumeroProposta != null && obj.NumeroProposta == numeroProposta
                          && (obj.VigenciaInicio == null || obj.VigenciaInicio <= dataFiltro.Date)
                          && (obj.VigenciaFim == null || obj.VigenciaFim >= dataFiltro.Date)
                         select obj;

            return result.ToList();
        }

        [Obsolete("Método descontinuado para utilizar a lista em memória no serviço RegrasCalculoImpostos", error: true)]
        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarPorParticipantes(double remetente, double destinatario, double tomador, int grupoRemetente, int grupoDestinatario, int grupoTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();
            var predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                            obj.Ativo
                            && (obj.VigenciaInicio == null || obj.VigenciaInicio <= dataFiltro.Date)
                            && (obj.VigenciaFim == null || obj.VigenciaFim >= dataFiltro.Date)
                         select obj;

            if (remetente > 0)
            {
                if (grupoRemetente > 0)
                    predicateOr = predicateOr.Or(obj => obj.Remetente.CPF_CNPJ == remetente || obj.GrupoRemetente.Codigo == grupoRemetente);
                else
                    predicateOr = predicateOr.Or(obj => obj.Remetente.CPF_CNPJ == remetente);
            }

            if (destinatario > 0)
            {
                if (grupoDestinatario > 0)
                    predicateOr = predicateOr.Or(obj => obj.Destinatario.CPF_CNPJ == destinatario || obj.GrupoDestinatario.Codigo == grupoDestinatario);
                else
                    predicateOr = predicateOr.Or(obj => obj.Destinatario.CPF_CNPJ == destinatario);
            }

            if (tomador > 0)
            {
                if (grupoTomador > 0)
                    predicateOr = predicateOr.Or(obj => obj.Tomador.CPF_CNPJ == tomador || obj.GrupoTomador.Codigo == grupoTomador);
                else
                    predicateOr = predicateOr.Or(obj => obj.Tomador.CPF_CNPJ == tomador);
            }
            result = result.Where(predicateOr);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarPorParticipantes(double remetente, double destinatario, double tomador, int grupoRemetente, int grupoDestinatario, int grupoTomador, IQueryable<Dominio.Entidades.Embarcador.ICMS.RegraICMS> query)
        {
            var predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                            obj.Ativo
                            && (obj.VigenciaInicio == null || obj.VigenciaInicio <= dataFiltro.Date)
                            && (obj.VigenciaFim == null || obj.VigenciaFim >= dataFiltro.Date)
                         select obj;

            if (remetente > 0)
            {
                if (grupoRemetente > 0)
                    predicateOr = predicateOr.Or(obj => (obj.Remetente != null && obj.Remetente.CPF_CNPJ == remetente) || (obj.GrupoRemetente != null && obj.GrupoRemetente.Codigo == grupoRemetente));
                else
                    predicateOr = predicateOr.Or(obj => (obj.Remetente != null && obj.Remetente.CPF_CNPJ == remetente));
            }

            if (destinatario > 0)
            {
                if (grupoDestinatario > 0)
                    predicateOr = predicateOr.Or(obj => (obj.Destinatario != null && obj.Destinatario.CPF_CNPJ == destinatario) || (obj.GrupoDestinatario != null && obj.GrupoDestinatario.Codigo == grupoDestinatario));
                else
                    predicateOr = predicateOr.Or(obj => (obj.Destinatario != null && obj.Destinatario.CPF_CNPJ == destinatario));
            }

            if (tomador > 0)
            {
                if (grupoTomador > 0)
                    predicateOr = predicateOr.Or(obj => (obj.Tomador != null && obj.Tomador.CPF_CNPJ == tomador) || (obj.GrupoTomador != null && obj.GrupoTomador.Codigo == grupoTomador));
                else
                    predicateOr = predicateOr.Or(obj => (obj.Tomador != null && obj.Tomador.CPF_CNPJ == tomador));
            }

            result = result.Where(predicateOr);

            return result.ToList();
        }

        [Obsolete("Método descontinuado para utilizar a lista em memória no serviço RegrasCalculoImpostos", error: true)]
        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarPorEstados(string ufEmitente, string ufOrigem, string ufDestino, string ufTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();
            var predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                            obj.Ativo
                            && (obj.VigenciaInicio == null || obj.VigenciaInicio <= dataFiltro.Date)
                            && (obj.VigenciaFim == null || obj.VigenciaFim >= dataFiltro.Date)
                         select obj;

            if (!string.IsNullOrWhiteSpace(ufEmitente))
            {
                predicateOr = predicateOr.Or(obj => obj.UFEmitente.Sigla == ufEmitente);
                predicateOr = predicateOr.Or(obj => obj.UFEmitenteDiferente.Sigla != ufEmitente);
            }

            if (!string.IsNullOrWhiteSpace(ufOrigem))
            {
                predicateOr = predicateOr.Or(obj => (obj.EstadoOrigemDiferente == false && obj.UFOrigem.Sigla == ufOrigem) || (obj.EstadoOrigemDiferente == true && obj.UFOrigem.Sigla != ufOrigem));
            }

            if (!string.IsNullOrWhiteSpace(ufDestino))
            {
                predicateOr = predicateOr.Or(obj => (obj.EstadoDestinoDiferente == false && obj.UFDestino.Sigla == ufDestino) || (obj.EstadoDestinoDiferente == true && obj.UFDestino.Sigla != ufDestino));
            }

            if (!string.IsNullOrWhiteSpace(ufTomador))
            {
                predicateOr = predicateOr.Or(obj => obj.UFTomador.Sigla == ufTomador);
            }

            result = result.Where(predicateOr);
            result = result.Where(obj => obj.Remetente == null && obj.Destinatario == null && obj.Tomador == null && obj.GrupoDestinatario == null
            && obj.GrupoRemetente == null && obj.GrupoTomador == null);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarPorEstados(string ufEmitente, string ufOrigem, string ufDestino, string ufTomador, IQueryable<Dominio.Entidades.Embarcador.ICMS.RegraICMS> query)
        {
            var predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                            obj.Ativo
                            && (obj.VigenciaInicio == null || obj.VigenciaInicio <= dataFiltro.Date)
                            && (obj.VigenciaFim == null || obj.VigenciaFim >= dataFiltro.Date)
                         select obj;

            if (!string.IsNullOrWhiteSpace(ufEmitente))
            {
                predicateOr = predicateOr.Or(obj => (obj.UFEmitente != null && obj.UFEmitente.Sigla == ufEmitente));
                predicateOr = predicateOr.Or(obj => (obj.UFEmitenteDiferente != null && obj.UFEmitenteDiferente.Sigla != ufEmitente));
            }

            if (!string.IsNullOrWhiteSpace(ufOrigem))
                predicateOr = predicateOr.Or(obj => (obj.EstadoOrigemDiferente == false && obj.UFOrigem != null && obj.UFOrigem.Sigla == ufOrigem) || (obj.EstadoOrigemDiferente == true && obj.UFOrigem != null && obj.UFOrigem.Sigla != ufOrigem));

            if (!string.IsNullOrWhiteSpace(ufDestino))
                predicateOr = predicateOr.Or(obj => (obj.EstadoDestinoDiferente == false && obj.UFDestino != null && obj.UFDestino.Sigla == ufDestino) || (obj.EstadoDestinoDiferente == true && obj.UFDestino != null && obj.UFDestino.Sigla != ufDestino));

            if (!string.IsNullOrWhiteSpace(ufTomador))
                predicateOr = predicateOr.Or(obj => (!obj.EstadoTomadorDiferente && obj.UFTomador != null && obj.UFTomador.Sigla == ufTomador) || (obj.EstadoTomadorDiferente && obj.UFTomador != null && obj.UFTomador.Sigla != ufTomador));


            result = result.Where(predicateOr);
            result = result.Where(obj => obj.Remetente == null &&
                                         obj.Destinatario == null &&
                                         obj.Tomador == null &&
                                         obj.GrupoDestinatario == null &&
                                         obj.GrupoRemetente == null &&
                                         obj.GrupoTomador == null);

            return result.ToList();
        }

        [Obsolete("Método descontinuado para utilizar a lista em memória no serviço RegrasCalculoImpostos", error: true)]
        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarPorAtividade(int atividadeRemetente, int atividadeDestinatario, int atividadeTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();
            var predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                            obj.Ativo
                            && (obj.VigenciaInicio == null || obj.VigenciaInicio <= dataFiltro.Date)
                            && (obj.VigenciaFim == null || obj.VigenciaFim >= dataFiltro.Date)
                         select obj;

            if (atividadeRemetente > 0)
                predicateOr = predicateOr.Or(obj => obj.AtividadeRemetente.Codigo == atividadeRemetente);

            if (atividadeDestinatario > 0)
                predicateOr = predicateOr.Or(obj => obj.AtividadeDestinatario.Codigo == atividadeDestinatario);

            if (atividadeTomador > 0)
                predicateOr = predicateOr.Or(regra => (regra.AtividadeTomadorDiferente && regra.AtividadeTomador.Codigo != atividadeTomador) || (!regra.AtividadeTomadorDiferente && regra.AtividadeTomador.Codigo == atividadeTomador));

            result = result.Where(obj => obj.Remetente == null && obj.Destinatario == null && obj.Tomador == null &&
                                         obj.GrupoDestinatario == null && obj.GrupoRemetente == null && obj.GrupoTomador == null &&
                                         obj.UFDestino == null && obj.UFOrigem == null && obj.UFTomador == null && obj.UFEmitente == null);

            result = result.Where(predicateOr);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarPorAtividade(int atividadeRemetente, int atividadeDestinatario, int atividadeTomador, IQueryable<Dominio.Entidades.Embarcador.ICMS.RegraICMS> query)
        {
            var predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                            obj.Ativo
                            && (obj.VigenciaInicio == null || obj.VigenciaInicio <= dataFiltro.Date)
                            && (obj.VigenciaFim == null || obj.VigenciaFim >= dataFiltro.Date)
                         select obj;

            if (atividadeRemetente > 0)
                predicateOr = predicateOr.Or(obj => obj.AtividadeRemetente != null && obj.AtividadeRemetente.Codigo == atividadeRemetente);

            if (atividadeDestinatario > 0)
                predicateOr = predicateOr.Or(obj => obj.AtividadeDestinatario != null && obj.AtividadeDestinatario.Codigo == atividadeDestinatario);

            if (atividadeTomador > 0)
                predicateOr = predicateOr.Or(regra => (regra.AtividadeTomadorDiferente && regra.AtividadeTomador != null && regra.AtividadeTomador.Codigo != atividadeTomador) || (!regra.AtividadeTomadorDiferente && regra.AtividadeTomador != null && regra.AtividadeTomador.Codigo == atividadeTomador));

            result = result.Where(obj => obj.Remetente == null && obj.Destinatario == null && obj.Tomador == null &&
                                         obj.GrupoDestinatario == null && obj.GrupoRemetente == null && obj.GrupoTomador == null &&
                                         obj.UFDestino == null && obj.UFOrigem == null && obj.UFTomador == null && obj.UFEmitente == null);

            result = result.Where(predicateOr);

            return result.ToList();
        }

        [Obsolete("Método descontinuado para utilizar a lista em memória no serviço RegrasCalculoImpostos", error: true)]
        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarPorProdutoEmbarcador(int produtoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                            obj.Ativo
                            && (obj.VigenciaInicio == null || obj.VigenciaInicio <= dataFiltro.Date)
                            && (obj.VigenciaFim == null || obj.VigenciaFim >= dataFiltro.Date)
                         select obj;

            if (produtoEmbarcador > 0)
            {
                //result = result.Where(obj => obj.ProdutoEmbarcador.Codigo == produtoEmbarcador);
                result = result.Where(obj => obj.ProdutosEmbarcador != null && obj.ProdutosEmbarcador.Any(o => o.Codigo == produtoEmbarcador));
            }

            result = result.Where(obj => obj.Remetente == null && obj.Destinatario == null && obj.Tomador == null
            && obj.GrupoDestinatario == null && obj.GrupoRemetente == null && obj.GrupoTomador == null && obj.UFDestino == null
            && obj.UFOrigem == null && obj.UFTomador == null && obj.UFEmitente == null);


            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarPorProdutoEmbarcador(int produtoEmbarcador, IQueryable<Dominio.Entidades.Embarcador.ICMS.RegraICMS> query)
        {

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                            obj.Ativo
                            && (obj.VigenciaInicio == null || obj.VigenciaInicio <= dataFiltro.Date)
                            && (obj.VigenciaFim == null || obj.VigenciaFim >= dataFiltro.Date)
                         select obj;

            if (produtoEmbarcador > 0)
            {
                //result = result.Where(obj => obj.ProdutoEmbarcador != null && obj.ProdutoEmbarcador.Codigo == produtoEmbarcador);
                result = result.Where(obj => obj.ProdutosEmbarcador != null && obj.ProdutosEmbarcador.Any(o => o.Codigo == produtoEmbarcador));
            }

            result = result.Where(obj => obj.Remetente == null && obj.Destinatario == null && obj.Tomador == null
            && obj.GrupoDestinatario == null && obj.GrupoRemetente == null && obj.GrupoTomador == null && obj.UFDestino == null
            && obj.UFOrigem == null && obj.UFTomador == null && obj.UFEmitente == null);


            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarPorTipoOperacao(int codigoTipoOperacao, IQueryable<Dominio.Entidades.Embarcador.ICMS.RegraICMS> query)
        {

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                            obj.Ativo
                            && (obj.VigenciaInicio == null || obj.VigenciaInicio <= dataFiltro.Date)
                            && (obj.VigenciaFim == null || obj.VigenciaFim >= dataFiltro.Date)
                            //&& obj.TipoOperacao != null && obj.TipoOperacao.Codigo == codigoTipoOperacao
                            && (obj.TiposOperacao != null && obj.TiposOperacao.Any(o => o.Codigo == codigoTipoOperacao))
                         select obj;

            return result.ToList();
        }

        public int ContarRegrasCadastradas(int codigoAtividade, string ufEmissor, string ufOrigem, string ufDestino, string ufTomador, DateTime? dataFiltro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();
            var result = from obj in query where obj.Ativo && (((TipoRegraICMS?)obj.Tipo).HasValue == false || obj.Tipo == TipoRegraICMS.Ativa) select obj;

            if (codigoAtividade > 0)
                result = result.Where(o => o.AtividadeTomador.Codigo == codigoAtividade);

            if (dataFiltro.HasValue)
                result = result.Where(o => (o.VigenciaInicio == null || o.VigenciaInicio <= dataFiltro.Value.Date) && (o.VigenciaFim == null || o.VigenciaFim >= dataFiltro.Value.Date));

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarTodasRegrasCadastradas(int codigoAtividade, string ufEmissor, string ufOrigem, string ufDestino, string ufTomador, DateTime? dataFiltro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            var result = from obj in query where obj.Ativo && (((TipoRegraICMS?)obj.Tipo).HasValue == false || obj.Tipo == TipoRegraICMS.Ativa) select obj;

            if (codigoAtividade > 0)
                result = result.Where(o => o.AtividadeTomador.Codigo == codigoAtividade);

            if (dataFiltro.HasValue)
                result = result.Where(o => (o.VigenciaInicio == null || o.VigenciaInicio <= dataFiltro.Value.Date) && (o.VigenciaFim == null || o.VigenciaFim >= dataFiltro.Value.Date));

            return result.OrderByDescending(o => o.Empresa.Codigo).ToList();
        }

        public int ContarRegrasCadastradas(int codigoEmpresa, DateTime? dataFiltro, int codigoEmpresaPai)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();
            var result = from obj in query where obj.Ativo && (((TipoRegraICMS?)obj.Tipo).HasValue == false || obj.Tipo == TipoRegraICMS.Ativa) select obj;

            if (codigoEmpresa > 0 && codigoEmpresaPai == 0)
                result = result.Where(o => (o.Empresa.Codigo.Equals(codigoEmpresa) || o.Empresa == null));
            else if (codigoEmpresa > 0 && codigoEmpresaPai > 0)
                result = result.Where(o => (o.Empresa.Codigo.Equals(codigoEmpresa) || o.Empresa == null || o.Empresa.Codigo.Equals(codigoEmpresaPai)));
            else if (codigoEmpresa == 0 && codigoEmpresaPai > 0)
                result = result.Where(o => (o.Empresa == null || o.Empresa.Codigo.Equals(codigoEmpresaPai)));

            if (dataFiltro.HasValue)
                result = result.Where(o => (o.VigenciaInicio == null || o.VigenciaInicio <= dataFiltro.Value.Date) && (o.VigenciaFim == null || o.VigenciaFim >= dataFiltro.Value.Date));

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarTodasRegrasCadastradas(int codigoEmpresa, DateTime? dataFiltro, int codigoEmpresaPai)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            var result = from obj in query where obj.Ativo && (((TipoRegraICMS?)obj.Tipo).HasValue == false || obj.Tipo == TipoRegraICMS.Ativa) select obj;

            if (codigoEmpresa > 0 && codigoEmpresaPai == 0)
                result = result.Where(o => (o.Empresa.Codigo.Equals(codigoEmpresa) || o.Empresa == null));
            else if (codigoEmpresa > 0 && codigoEmpresaPai > 0)
                result = result.Where(o => (o.Empresa.Codigo.Equals(codigoEmpresa) || o.Empresa == null || o.Empresa.Codigo.Equals(codigoEmpresaPai)));
            else if (codigoEmpresa == 0 && codigoEmpresaPai > 0)
                result = result.Where(o => (o.Empresa == null || o.Empresa.Codigo.Equals(codigoEmpresaPai)));

            if (dataFiltro.HasValue)
                result = result.Where(o => (o.VigenciaInicio == null || o.VigenciaInicio <= dataFiltro.Value.Date) && (o.VigenciaFim == null || o.VigenciaFim >= dataFiltro.Value.Date));

            return result.OrderByDescending(o => o.Empresa.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> FiltrarTodasRegrasCadastradas(int codigoEmpresa, DateTime? dataFiltro, int codigoEmpresaPai, IQueryable<Dominio.Entidades.Embarcador.ICMS.RegraICMS> query)
        {
            var result = from obj in query where obj.Ativo select obj;

            if (codigoEmpresa > 0 && codigoEmpresaPai == 0)
                result = result.Where(o => (o.Empresa != null && o.Empresa.Codigo.Equals(codigoEmpresa)) || o.Empresa == null);
            else if (codigoEmpresa > 0 && codigoEmpresaPai > 0)
                result = result.Where(o => (o.Empresa != null && o.Empresa.Codigo.Equals(codigoEmpresa)) || o.Empresa == null || (o.Empresa != null && o.Empresa.Codigo.Equals(codigoEmpresaPai)));
            else if (codigoEmpresa == 0 && codigoEmpresaPai > 0)
                result = result.Where(o => (o.Empresa == null || (o.Empresa != null && o.Empresa.Codigo.Equals(codigoEmpresaPai))));

            if (dataFiltro.HasValue)
                result = result.Where(o => (o.VigenciaInicio == null || o.VigenciaInicio <= dataFiltro.Value.Date) && (o.VigenciaFim == null || o.VigenciaFim >= dataFiltro.Value.Date));

            return result.OrderByDescending(o => o.Empresa != null).ToList();
        }

        #endregion

        #region Métodos Publicos

        public Dominio.Entidades.Embarcador.ICMS.RegraICMS BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.ICMS.RegraICMS BuscarPorCodigoEmpresa(int codigo, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == empresa select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.ICMS.RegraICMS BuscarPorAprovacaoPendente(int codigo)
        {
            var consultaRegraICMS = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>()
                .Where(o =>
                    o.RegraOriginaria.Codigo == codigo &&
                    (o.SituacaoAlteracao == SituacaoAlteracaoRegraICMS.AguardandoAprovacao || o.SituacaoAlteracao == SituacaoAlteracaoRegraICMS.SemRegraAprovacao)
                );

            return consultaRegraICMS.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> BuscarSemRegraAprovacaoPorCodigos(List<int> codigosRegraICMS)
        {
            var consultaRegraICMS = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>()
                .Where(o => codigosRegraICMS.Contains(o.Codigo) && o.SituacaoAlteracao == SituacaoAlteracaoRegraICMS.SemRegraAprovacao);

            return consultaRegraICMS.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> Consultar(double remetente, double destinatario, double tomador, int empresa, int grupoRemetente, int grupoDestinatario, int grupoTomador, string ufEmitente, string ufEmitenteDiferenteDe, string ufOrigem, string ufDestino, string ufTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, DateTime? dataInicio, DateTime? dataFim, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaAlteracaoRegraICMS = MontarConsulta(remetente, destinatario, tomador, empresa, grupoRemetente, grupoDestinatario, grupoTomador, ufEmitente, ufEmitenteDiferenteDe, ufOrigem, ufDestino, ufTomador, ativo, dataInicio, dataFim, descricao);

            consultaAlteracaoRegraICMS = consultaAlteracaoRegraICMS
                .Fetch(o => o.UFEmitente)
                .Fetch(o => o.UFDestino)
                .Fetch(o => o.UFOrigem)
                .Fetch(o => o.UFTomador)
                .Fetch(o => o.Destinatario)
                .Fetch(o => o.Remetente)
                .Fetch(o => o.Tomador)
                .Fetch(o => o.GrupoDestinatario)
                .Fetch(o => o.GrupoRemetente)
                .Fetch(o => o.GrupoTomador);

            return consultaAlteracaoRegraICMS.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(double remente, double destinatario, double tomador, int empresa, int grupoRemetente, int grupoDestinatario, int grupoTomador, string ufEmitente, string ufEmitenteDiferenteDe, string ufOrigem, string ufDestino, string ufTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, DateTime? dataInicio, DateTime? dataFim, string descricao)
        {
            var consultaAlteracaoRegraICMS = MontarConsulta(remente, destinatario, tomador, empresa, grupoRemetente, grupoDestinatario, grupoTomador, ufEmitente, ufEmitenteDiferenteDe, ufOrigem, ufDestino, ufTomador, ativo, dataInicio, dataFim, descricao);

            return consultaAlteracaoRegraICMS.Count();
        }

        public Dominio.Entidades.Embarcador.ICMS.RegraICMS BuscarPorParametros(bool somenteSimplesNacional, double remetente, double destinatario, double tomador, int empresa, int grupoRemetente, int grupoDestinatario, int grupoTomador, string ufEmitente, string ufEmitenteDiferenteDe, string ufOrigem, string ufDestino, string ufTomador, int atividadeRemetente, int atividadeDestinatario, int atividadeTomador, int produtoEmbarcador, DateTime? vigenciaInicio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario? regimeTributarioTomador, bool regimeTributarioTomadorDiferente, bool atividadeTomadorDiferente, string setorEmpresa, int codigoTipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal? tipoModal, Dominio.Enumeradores.TipoServico? tipoServico, Dominio.Enumeradores.TipoPagamentoRegraICMS? tipoPagamento, string numeroProposta, bool estadoDestinoDiferente, bool estadoOrigemDiferente, bool estadoTomadorDiferente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            var result = from obj in query where (((TipoRegraICMS?)obj.Tipo).HasValue == false || obj.Tipo == TipoRegraICMS.Ativa) select obj;

            result = result.Where(obj => obj.SomenteOptanteSimplesNacional == somenteSimplesNacional);

            result = result.Where(obj => obj.EstadoDestinoDiferente == estadoDestinoDiferente);
            result = result.Where(obj => obj.EstadoOrigemDiferente == estadoOrigemDiferente);
            result = result.Where(obj => obj.EstadoTomadorDiferente == estadoTomadorDiferente);

            if (remetente > 0)
                result = result.Where(obj => obj.Remetente.CPF_CNPJ == remetente);
            else
                result = result.Where(obj => obj.Remetente == null);

            if (destinatario > 0)
                result = result.Where(obj => obj.Destinatario.CPF_CNPJ == destinatario);
            else
                result = result.Where(obj => obj.Destinatario == null);

            if (tomador > 0)
                result = result.Where(obj => obj.Tomador.CPF_CNPJ == tomador);
            else
                result = result.Where(obj => obj.Tomador == null);

            if (!string.IsNullOrWhiteSpace(setorEmpresa))
                result = result.Where(obj => obj.SetorEmpresa == setorEmpresa);
            else
                result = result.Where(obj => obj.SetorEmpresa == null || obj.SetorEmpresa == "");

            if (!string.IsNullOrWhiteSpace(numeroProposta))
                result = result.Where(obj => obj.NumeroProposta == numeroProposta);
            else
                result = result.Where(obj => obj.NumeroProposta == null || obj.NumeroProposta == "");

            if (atividadeTomador > 0)
                result = result.Where(obj => obj.AtividadeTomador.Codigo == atividadeTomador && obj.AtividadeTomadorDiferente == atividadeTomadorDiferente);

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);
            else
                result = result.Where(obj => obj.Empresa == null);

            if (!string.IsNullOrWhiteSpace(ufEmitenteDiferenteDe))
                result = result.Where(obj => obj.UFEmitenteDiferente.Sigla == ufEmitenteDiferenteDe);
            else
                result = result.Where(obj => obj.UFEmitenteDiferente == null);

            if (atividadeDestinatario > 0)
                result = result.Where(obj => obj.AtividadeDestinatario.Codigo == atividadeDestinatario);
            else
                result = result.Where(obj => obj.AtividadeDestinatario == null);

            if (atividadeRemetente > 0)
                result = result.Where(obj => obj.AtividadeRemetente.Codigo == atividadeRemetente);
            else
                result = result.Where(obj => obj.AtividadeRemetente == null);

            if (grupoRemetente > 0)
                result = result.Where(obj => obj.GrupoRemetente.Codigo == grupoRemetente);
            else
                result = result.Where(obj => obj.GrupoRemetente == null);

            if (grupoDestinatario > 0)
                result = result.Where(obj => obj.GrupoDestinatario.Codigo == grupoDestinatario);
            else
                result = result.Where(obj => obj.GrupoDestinatario == null);

            if (grupoTomador > 0)
                result = result.Where(obj => obj.GrupoTomador.Codigo == grupoTomador);
            else
                result = result.Where(obj => obj.GrupoTomador == null);

            if (produtoEmbarcador > 0)
                result = result.Where(obj => obj.ProdutosEmbarcador != null && obj.ProdutosEmbarcador.Any(o => o.Codigo == produtoEmbarcador));  //result = result.Where(obj => obj.ProdutoEmbarcador.Codigo == produtoEmbarcador);
            else
                result = result.Where(obj => obj.ProdutosEmbarcador.Count == 0); //result = result.Where(obj => obj.ProdutoEmbarcador == null);

            if (!string.IsNullOrWhiteSpace(ufOrigem))
                result = result.Where(obj => obj.UFOrigem.Sigla == ufOrigem);
            else
                result = result.Where(obj => obj.UFOrigem == null);

            if (!string.IsNullOrWhiteSpace(ufDestino))
                result = result.Where(obj => obj.UFDestino.Sigla == ufDestino);
            else
                result = result.Where(obj => obj.UFDestino == null);

            if (!string.IsNullOrWhiteSpace(ufTomador))
                result = result.Where(obj => obj.UFTomador.Sigla == ufTomador);
            else
                result = result.Where(obj => obj.UFTomador == null);

            if (vigenciaInicio.HasValue)
                result = result.Where(obj => obj.VigenciaFim == null || obj.VigenciaFim > vigenciaInicio.Value);

            if (regimeTributarioTomador.HasValue)
                result = result.Where(o => o.RegimeTributarioTomador == regimeTributarioTomador.Value && o.RegimeTributarioTomadorDiferente == regimeTributarioTomadorDiferente);

            if (tipoModal.HasValue)
                result = result.Where(o => o.TipoModal == tipoModal.Value);

            if (tipoServico.HasValue)
                result = result.Where(o => o.TipoServico == tipoServico.Value);

            if (tipoPagamento.HasValue)
                result = result.Where(o => o.TipoPagamento == tipoPagamento.Value);

            if (codigoTipoOperacao > 0)
                result = result.Where(obj => obj.TiposOperacao != null && obj.TiposOperacao.Any(o => o.Codigo == codigoTipoOperacao));  //result = result.Where(obj => obj.TipoOperacao.Codigo == codigoTipoOperacao);
            else
                result = result.Where(obj => obj.TiposOperacao.Count == 0); //result = result.Where(obj => obj.TipoOperacao == null);

            result = result.Where(obj => obj.Ativo == true);

            return result.FirstOrDefault();
        }

        // Consulta MultiCTe
        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> ConsultarMultiCTe(int codigoEmpresa, string ufEmitete, string ufOrigem, string ufDestino, string ufTomador, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && (((TipoRegraICMS?)obj.Tipo).HasValue == false || obj.Tipo == TipoRegraICMS.Ativa) select obj;

            if (!string.IsNullOrWhiteSpace(ufEmitete))
                result = result.Where(o => o.UFEmitente.Sigla == ufEmitete);

            if (!string.IsNullOrWhiteSpace(ufOrigem))
                result = result.Where(o => o.UFOrigem.Sigla == ufOrigem);

            if (!string.IsNullOrWhiteSpace(ufDestino))
                result = result.Where(o => o.UFDestino.Sigla == ufDestino);

            if (!string.IsNullOrWhiteSpace(ufTomador))
                result = result.Where(o => o.UFTomador.Sigla == ufTomador);

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaMultiCTe(int codigoEmpresa, string ufEmitete, string ufOrigem, string ufDestino, string ufTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && (((TipoRegraICMS?)obj.Tipo).HasValue == false || obj.Tipo == TipoRegraICMS.Ativa) select obj;

            if (!string.IsNullOrWhiteSpace(ufEmitete))
                result = result.Where(o => o.UFEmitente.Sigla == ufEmitete);

            if (!string.IsNullOrWhiteSpace(ufOrigem))
                result = result.Where(o => o.UFOrigem.Sigla == ufOrigem);

            if (!string.IsNullOrWhiteSpace(ufDestino))
                result = result.Where(o => o.UFDestino.Sigla == ufDestino);

            if (!string.IsNullOrWhiteSpace(ufTomador))
                result = result.Where(o => o.UFTomador.Sigla == ufTomador);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> BuscarPorVencimentoAlertar(Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlerta filtrosPesquisa)
        {
            DateTime dataAtual = DateTime.Now.Date;
            DateTime dataVencimentoAnttAlertar = dataAtual.AddDays(filtrosPesquisa.DiasAlertarAntesVencimento);

            var consultaRegraICMS = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>()
                .Where(obj => obj.Ativo == true && obj.VigenciaFim.HasValue && (((TipoRegraICMS?)obj.Tipo).HasValue == false || obj.Tipo == TipoRegraICMS.Ativa));

            if (!filtrosPesquisa.AlertarAposVencimento)
                consultaRegraICMS = consultaRegraICMS.Where(o => o.VigenciaFim >= dataAtual);

            if (filtrosPesquisa.DiasAlertarAntesVencimento > 0)
            {
                consultaRegraICMS = consultaRegraICMS.Where(o =>
                    o.VigenciaFim.HasValue &&
                    o.VigenciaFim.Value >= dataAtual &&
                    o.VigenciaFim.Value <= dataAtual.AddDays(filtrosPesquisa.DiasAlertarAntesVencimento));
            }

            if (filtrosPesquisa.DiasRepetirAlerta > 0)
            {
                DateTime dataVencimentoRepetirAlerta = dataAtual.AddDays(-filtrosPesquisa.DiasRepetirAlerta);

                consultaRegraICMS = consultaRegraICMS.Where(o => (o.DataUltimoAlertaVencimento == null) || (o.DataUltimoAlertaVencimento <= dataVencimentoRepetirAlerta));
            }
            else
                consultaRegraICMS = consultaRegraICMS.Where(o => o.DataUltimoAlertaVencimento == null);

            return consultaRegraICMS.ToList();
        }

        #endregion

        #region Métodos Públicos - Relatorios

        public IList<Dominio.Relatorios.Embarcador.DataSource.ICMS.RegraICMS> ConsultarRelatorioRegraICMS(Dominio.ObjetosDeValor.Embarcador.ICMS.FiltroPesquisaRelatorioRegraICMS filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaRegraICMS().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.ICMS.RegraICMS)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.ICMS.RegraICMS>();
        }

        public int ContarConsultaRelatorioRegraICMS(Dominio.ObjetosDeValor.Embarcador.ICMS.FiltroPesquisaRelatorioRegraICMS filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaRegraICMS().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.ICMS.RegraICMS> MontarConsulta(double remetente, double destinatario, double tomador, int empresa, int grupoRemetente, int grupoDestinatario, int grupoTomador, string ufEmitente, string ufEmitenteDiferenteDe, string ufOrigem, string ufDestino, string ufTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, DateTime? dataInicio, DateTime? dataFim, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            var result = from obj in query where (((TipoRegraICMS?)obj.Tipo).HasValue == false || obj.Tipo == TipoRegraICMS.Ativa) select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (remetente > 0)
                result = result.Where(obj => obj.Remetente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                result = result.Where(obj => obj.Destinatario.CPF_CNPJ == destinatario);

            if (tomador > 0)
                result = result.Where(obj => obj.Tomador.CPF_CNPJ == tomador);

            if (grupoRemetente > 0)
                result = result.Where(obj => obj.GrupoRemetente.Codigo == grupoRemetente);

            if (grupoDestinatario > 0)
                result = result.Where(obj => obj.GrupoDestinatario.Codigo == grupoDestinatario);

            if (grupoTomador > 0)
                result = result.Where(obj => obj.GrupoTomador.Codigo == grupoTomador);

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);

            if (!string.IsNullOrWhiteSpace(ufEmitente))
                result = result.Where(obj => obj.UFEmitente.Sigla == ufEmitente);

            if (!string.IsNullOrWhiteSpace(ufOrigem))
                result = result.Where(obj => obj.UFOrigem.Sigla == ufOrigem);

            if (!string.IsNullOrWhiteSpace(ufDestino))
                result = result.Where(obj => obj.UFDestino.Sigla == ufDestino);

            if (!string.IsNullOrWhiteSpace(ufTomador))
                result = result.Where(obj => obj.UFTomador.Sigla == ufTomador);

            if (!string.IsNullOrWhiteSpace(ufEmitenteDiferenteDe))
                result = result.Where(obj => obj.UFEmitenteDiferente.Sigla == ufEmitenteDiferenteDe);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (dataInicio.HasValue)
                result = result.Where(obj => (obj.VigenciaInicio >= dataInicio || obj.VigenciaFim >= dataInicio));

            if (dataFim.HasValue)
                result = result.Where(obj => (obj.VigenciaFim <= dataFim || obj.VigenciaFim <= dataFim));

            return result;
        }

        #endregion
    }
}
