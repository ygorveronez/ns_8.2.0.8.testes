using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using LinqKit;

namespace Repositorio.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoNaturezaOperacao : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>
    {
        #region Construtores
        public ConfiguracaoNaturezaOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos de Consulta

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> FiltrarPorEmpresa(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>();

            var result = from obj in query where obj.Ativo && obj.Empresa.Codigo == empresa select obj;

            return result
                .Fetch(obj => obj.NaturezaDaOperacaoCompra)
                .Fetch(obj => obj.NaturezaDaOperacaoVenda)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> FiltrarPorParticipantes(double remetente, double destinatario, double tomador, int grupoRemetente, int grupoDestinatario, int grupoTomador, int codigoCategoriaDestinatario, int codigoCategoriaRemetente, int codigoCategoriaTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>();
            var predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>();

            var result = from obj in query where obj.Ativo select obj;

            if (remetente > 0)
            {
                if ((grupoRemetente > 0) && (codigoCategoriaRemetente > 0))
                    predicateOr = predicateOr.Or(obj => obj.Remetente.CPF_CNPJ == remetente || obj.GrupoRemetente.Codigo == grupoRemetente || obj.CategoriaRemetente.Codigo == codigoCategoriaRemetente);
                else if (grupoRemetente > 0)
                    predicateOr = predicateOr.Or(obj => obj.Remetente.CPF_CNPJ == remetente || obj.GrupoRemetente.Codigo == grupoRemetente);
                else if (codigoCategoriaRemetente > 0)
                    predicateOr = predicateOr.Or(obj => obj.Remetente.CPF_CNPJ == remetente || obj.CategoriaRemetente.Codigo == codigoCategoriaRemetente);
                else
                    predicateOr = predicateOr.Or(obj => obj.Remetente.CPF_CNPJ == remetente);
            }

            if (destinatario > 0)
            {
                if ((grupoDestinatario > 0) && (codigoCategoriaDestinatario > 0))
                    predicateOr = predicateOr.Or(obj => obj.Destinatario.CPF_CNPJ == destinatario || obj.GrupoDestinatario.Codigo == grupoDestinatario || obj.CategoriaDestinatario.Codigo == codigoCategoriaDestinatario);
                else if (grupoDestinatario > 0)
                    predicateOr = predicateOr.Or(obj => obj.Destinatario.CPF_CNPJ == destinatario || obj.GrupoDestinatario.Codigo == grupoDestinatario);
                else if (codigoCategoriaDestinatario > 0)
                    predicateOr = predicateOr.Or(obj => obj.Destinatario.CPF_CNPJ == destinatario || obj.CategoriaDestinatario.Codigo == codigoCategoriaDestinatario);
                else
                    predicateOr = predicateOr.Or(obj => obj.Destinatario.CPF_CNPJ == destinatario);
            }

            if (tomador > 0)
            {
                if ((grupoTomador > 0) && (codigoCategoriaTomador > 0))
                    predicateOr = predicateOr.Or(obj => obj.Tomador.CPF_CNPJ == tomador || obj.GrupoTomador.Codigo == grupoTomador || obj.CategoriaTomador.Codigo == codigoCategoriaTomador);
                else if (grupoTomador > 0)
                    predicateOr = predicateOr.Or(obj => obj.Tomador.CPF_CNPJ == tomador || obj.GrupoTomador.Codigo == grupoTomador);
                else if (codigoCategoriaTomador > 0)
                    predicateOr = predicateOr.Or(obj => obj.Tomador.CPF_CNPJ == tomador || obj.CategoriaTomador.Codigo == codigoCategoriaTomador);
                else
                    predicateOr = predicateOr.Or(obj => obj.Tomador.CPF_CNPJ == tomador);
            }
            result = result.Where(predicateOr);

            return result
                .Fetch(obj => obj.NaturezaDaOperacaoCompra)
                .Fetch(obj => obj.NaturezaDaOperacaoVenda)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> FiltrarPorGrupoProdutoEmbarcador(int grupoProdutoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>();

            var result = from obj in query where obj.Ativo select obj;

            if (grupoProdutoEmbarcador > 0)
            {
                result = result.Where(obj => obj.GrupoProduto.Codigo == grupoProdutoEmbarcador);
            }

            result = result.Where(obj =>
                obj.Remetente == null &&
                obj.Destinatario == null &&
                obj.Tomador == null &&
                obj.GrupoDestinatario == null &&
                obj.GrupoRemetente == null &&
                obj.GrupoTomador == null &&
                obj.CategoriaDestinatario == null &&
                obj.CategoriaRemetente == null &&
                obj.CategoriaTomador == null &&
                obj.TipoOperacao == null &&
                obj.RotaFrete == null
            );

            return result
                .Fetch(obj => obj.NaturezaDaOperacaoCompra)
                .Fetch(obj => obj.NaturezaDaOperacaoVenda)
                .ToList();
        }


        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> FiltrarPorTipoOperacao(int tipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>();

            var result = from obj in query where obj.Ativo select obj;

            if (tipoOperacao > 0)
            {
                result = result.Where(obj => obj.TipoOperacao.Codigo == tipoOperacao);
            }

            result = result.Where(obj =>
                obj.Remetente == null &&
                obj.Destinatario == null &&
                obj.Tomador == null &&
                obj.GrupoDestinatario == null &&
                obj.GrupoRemetente == null &&
                obj.GrupoTomador == null &&
                obj.CategoriaDestinatario == null &&
                obj.CategoriaRemetente == null &&
                obj.CategoriaTomador == null &&
                obj.GrupoProduto == null &&
                obj.RotaFrete == null
            );

            return result
                .Fetch(obj => obj.NaturezaDaOperacaoCompra)
                .Fetch(obj => obj.NaturezaDaOperacaoVenda)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> FiltrarPorRotaFrete(int rotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>();

            var result = from obj in query where obj.Ativo select obj;

            if (rotaFrete > 0)
            {
                result = result.Where(obj => obj.RotaFrete.Codigo == rotaFrete);
            }

            result = result.Where(obj =>
                obj.Remetente == null &&
                obj.Destinatario == null &&
                obj.Tomador == null &&
                obj.GrupoDestinatario == null &&
                obj.GrupoRemetente == null &&
                obj.GrupoTomador == null &&
                obj.CategoriaDestinatario == null &&
                obj.CategoriaRemetente == null &&
                obj.CategoriaTomador == null &&
                obj.GrupoProduto == null &&
                obj.TipoOperacao == null
            );

            return result
                .Fetch(obj => obj.NaturezaDaOperacaoCompra)
                .Fetch(obj => obj.NaturezaDaOperacaoVenda)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> FiltrarPorAtividade(int atividadeRemetente, int atividadeDestinatario, int atividadeTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>();
            var predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>();

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                            obj.Ativo
                         select obj;

            if (atividadeRemetente > 0)
            {
                predicateOr = predicateOr.Or(obj => obj.AtividadeRemetente.Codigo == atividadeRemetente);
            }

            if (atividadeDestinatario > 0)
            {
                predicateOr = predicateOr.Or(obj => obj.AtividadeDestinatario.Codigo == atividadeDestinatario);
            }

            if (atividadeTomador > 0)
            {
                predicateOr = predicateOr.Or(obj => obj.AtividadeTomador.Codigo == atividadeTomador);
            }

            result = result.Where(obj =>
                obj.Remetente == null &&
                obj.Destinatario == null &&
                obj.Tomador == null &&
                obj.GrupoDestinatario == null &&
                obj.GrupoRemetente == null &&
                obj.GrupoTomador == null &&
                obj.CategoriaDestinatario == null &&
                obj.CategoriaRemetente == null &&
                obj.CategoriaTomador == null &&
                obj.UFDestino == null &&
                obj.RotaFrete == null &&
                obj.GrupoProduto == null &&
                obj.UFOrigem == null
            );

            result = result.Where(predicateOr);

            return result
                .Fetch(obj => obj.NaturezaDaOperacaoCompra)
                .Fetch(obj => obj.NaturezaDaOperacaoVenda)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> FiltrarPorEstados(string ufOrigem, string ufDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>();
            var predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>();

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                            obj.Ativo
                         select obj;


            if (!string.IsNullOrWhiteSpace(ufOrigem))
            {
                predicateOr = predicateOr.Or(obj => (obj.EstadoOrigemDiferente == false && obj.UFOrigem.Sigla == ufOrigem) || (obj.EstadoOrigemDiferente == true && obj.UFOrigem.Sigla != ufOrigem));
            }

            if (!string.IsNullOrWhiteSpace(ufDestino))
            {
                predicateOr = predicateOr.Or(obj => (obj.EstadoDestinoDiferente == false && obj.UFDestino.Sigla == ufOrigem) || (obj.EstadoDestinoDiferente == true && obj.UFDestino.Sigla != ufOrigem));
            }

            result = result.Where(predicateOr);
            result = result.Where(obj =>
                obj.Remetente == null &&
                obj.Destinatario == null &&
                obj.GrupoDestinatario == null &&
                obj.GrupoRemetente == null &&
                obj.GrupoTomador == null &&
                obj.CategoriaDestinatario == null &&
                obj.CategoriaRemetente == null &&
                obj.CategoriaTomador == null &&
                obj.Tomador == null &&
                obj.RotaFrete == null &&
                obj.GrupoProduto == null
            );

            return result
                .Fetch(obj => obj.NaturezaDaOperacaoCompra)
                .Fetch(obj => obj.NaturezaDaOperacaoVenda)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> FiltrarPorEstadosRegraGlobal(string ufOrigem, string ufDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>();

            DateTime dataFiltro = DateTime.Now;

            var result = from obj in query
                         where
                            obj.Ativo
                         select obj;


            if (!string.IsNullOrWhiteSpace(ufOrigem) && string.IsNullOrWhiteSpace(ufDestino))
            {
                if (ufOrigem == ufDestino)
                    result = result.Where(obj => obj.EstadoOrigemIgualUFDestino);
                else
                    result = result.Where(obj => obj.EstadoOrigemDiferenteUFDestino);
            }

            result = result.Where(obj =>
                obj.Remetente == null &&
                obj.Destinatario == null &&
                obj.GrupoDestinatario == null &&
                obj.GrupoRemetente == null &&
                obj.GrupoTomador == null &&
                obj.CategoriaDestinatario == null &&
                obj.CategoriaRemetente == null &&
                obj.CategoriaTomador == null &&
                obj.Tomador == null &&
                obj.RotaFrete == null &&
                obj.GrupoProduto == null
            );

            return result
                .Fetch(obj => obj.NaturezaDaOperacaoCompra)
                .Fetch(obj => obj.NaturezaDaOperacaoVenda)
                .ToList();
        }

        public int ContarRegrasCadastradas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>();
            var result = from obj in query where obj.Ativo select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> FiltrarTodasRegrasCadastradas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>();
            var result = from obj in query where obj.Ativo select obj;

            return result.ToList();
        }


        #endregion

        #region Métodos Publicos

        public Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> Consultar(double remetente, double destinatario, double tomador, int empresa, int grupoRemetente, int grupoDestinatario, int grupoTomador, int tipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int categoriaDestinatario, int categoriaRemetente, int categoriaTomador, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = MontarConsulta(remetente, destinatario, tomador, empresa, grupoRemetente, grupoDestinatario, grupoTomador, tipoOperacao, ativo, categoriaDestinatario, categoriaRemetente, categoriaTomador);
            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(double remente, double destinatario, double tomador, int empresa, int grupoRemetente, int grupoDestinatario, int grupoTomador, int tipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int categoriaDestinatario, int categoriaRemetente, int categoriaTomador)
        {
            var query = MontarConsulta(remente, destinatario, tomador, empresa, grupoRemetente, grupoDestinatario, grupoTomador, tipoOperacao, ativo, categoriaDestinatario, categoriaRemetente, categoriaTomador);
            return query.Count();
        }

        public Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao BuscarPorParametros(double remetente, double destinatario, double tomador, int empresa, int grupoRemetente, int grupoDestinatario, int grupoTomador, int atividadeRemetente, int atividadeDestinatario, int atividadeTomador, int modeloDocumento, int tipoOperacao, int grupoProdutoEmbarcador, string ufOrigem, string ufDestino, bool ufOrigemDiferenteDe, bool ufDestinoDiferenteDe, bool ufOrigemDiferenteDeUFDestino, bool ufOrigemIgualUFDestino, bool estadoEmissorDiferenteUFOrigem, int rotaFrete, double categoriaRemetente, double categoriaDestinatario, double categoriaTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>();

            var result = from obj in query select obj;

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

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);
            else
                result = result.Where(obj => obj.Empresa == null);

            if (tipoOperacao > 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == tipoOperacao);
            else
                result = result.Where(obj => obj.TipoOperacao == null);

            if (rotaFrete > 0)
                result = result.Where(obj => obj.RotaFrete.Codigo == rotaFrete);
            else
                result = result.Where(obj => obj.RotaFrete == null);

            if (grupoProdutoEmbarcador > 0)
                result = result.Where(obj => obj.GrupoProduto.Codigo == grupoProdutoEmbarcador);
            else
                result = result.Where(obj => obj.GrupoProduto == null);

            if (modeloDocumento > 0)
                result = result.Where(obj => obj.ModeloDocumentoFiscal.Codigo == modeloDocumento);
            else
                result = result.Where(obj => obj.ModeloDocumentoFiscal == null);

            if (atividadeDestinatario > 0)
                result = result.Where(obj => obj.AtividadeDestinatario.Codigo == atividadeDestinatario);
            else
                result = result.Where(obj => obj.AtividadeDestinatario == null);

            if (atividadeRemetente > 0)
                result = result.Where(obj => obj.AtividadeRemetente.Codigo == atividadeRemetente);
            else
                result = result.Where(obj => obj.AtividadeRemetente == null);

            if (atividadeTomador > 0)
                result = result.Where(obj => obj.AtividadeTomador.Codigo == atividadeTomador);
            else
                result = result.Where(obj => obj.AtividadeTomador == null);

            if (grupoDestinatario > 0)
                result = result.Where(obj => obj.GrupoDestinatario.Codigo == grupoDestinatario);
            else
                result = result.Where(obj => obj.GrupoDestinatario == null);

            if (grupoRemetente > 0)
                result = result.Where(obj => obj.GrupoRemetente.Codigo == grupoRemetente);
            else
                result = result.Where(obj => obj.GrupoRemetente == null);

            if (grupoTomador > 0)
                result = result.Where(obj => obj.GrupoTomador.Codigo == grupoTomador);
            else
                result = result.Where(obj => obj.GrupoTomador == null);

            if (categoriaRemetente > 0)
                result = result.Where(obj => obj.CategoriaRemetente.Codigo == categoriaRemetente);
            else
                result = result.Where(obj => obj.CategoriaRemetente == null);

            if (categoriaDestinatario > 0)
                result = result.Where(obj => obj.CategoriaDestinatario.Codigo == categoriaDestinatario);
            else
                result = result.Where(obj => obj.CategoriaDestinatario == null);

            if (categoriaTomador > 0)
                result = result.Where(obj => obj.CategoriaTomador.Codigo == categoriaTomador);
            else
                result = result.Where(obj => obj.CategoriaTomador == null);

            if (!string.IsNullOrWhiteSpace(ufOrigem))
                result = result.Where(obj => obj.UFOrigem.Sigla == ufOrigem);
            else
                result = result.Where(obj => obj.UFOrigem == null);

            result = result.Where(obj => obj.EstadoDestinoDiferente == ufDestinoDiferenteDe);

            result = result.Where(obj => obj.EstadoOrigemDiferente == ufOrigemDiferenteDe);

            result = result.Where(obj => obj.EstadoOrigemDiferenteUFDestino == ufOrigemDiferenteDeUFDestino);

            result = result.Where(obj => obj.EstadoOrigemIgualUFDestino == ufOrigemIgualUFDestino);

            result = result.Where(obj => obj.EstadoEmissorDiferenteUFOrigem == estadoEmissorDiferenteUFOrigem);

            if (!string.IsNullOrWhiteSpace(ufDestino))
                result = result.Where(obj => obj.UFDestino.Sigla == ufDestino);

            result = result.Where(obj => obj.Ativo == true);

            return result.FirstOrDefault();
        }

        #endregion


        #region Métodos Privados
        private IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> MontarConsulta(double remetente, double destinatario, double tomador, int empresa, int grupoRemetente, int grupoDestinatario, int grupoTomador, int tipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int categoriaDestinatario, int categoriaRemetente, int categoriaTomador)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>();

            var result = from obj in query select obj;

            if (remetente > 0)
                result = result.Where(obj => obj.Remetente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                result = result.Where(obj => obj.Destinatario.CPF_CNPJ == destinatario);

            if (tomador > 0)
                result = result.Where(obj => obj.Tomador.CPF_CNPJ == tomador);

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);

            if (grupoRemetente > 0)
                result = result.Where(obj => obj.GrupoRemetente.Codigo == grupoRemetente);

            if (grupoDestinatario > 0)
                result = result.Where(obj => obj.GrupoDestinatario.Codigo == grupoDestinatario);

            if (grupoTomador > 0)
                result = result.Where(obj => obj.GrupoTomador.Codigo == grupoTomador);

            if (categoriaDestinatario > 0)
                result = result.Where(obj => obj.CategoriaDestinatario.Codigo == categoriaDestinatario);

            if (categoriaRemetente > 0)
                result = result.Where(obj => obj.CategoriaRemetente.Codigo == categoriaRemetente);

            if (categoriaTomador > 0)
                result = result.Where(obj => obj.CategoriaTomador.Codigo == categoriaTomador);

            if (tipoOperacao > 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == tipoOperacao);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result;
        }

        #endregion

    }
}

