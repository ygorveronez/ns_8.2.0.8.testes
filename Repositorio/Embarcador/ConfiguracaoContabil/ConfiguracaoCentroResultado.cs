using LinqKit;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoCentroResultado : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>
    {
        #region Construtores
        public ConfiguracaoCentroResultado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos de Consulta

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> BuscarTodosAtivas()
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>();

            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> result = from obj in query where obj.Ativo select obj;

            return result
                .Fetch(obj => obj.CentroResultadoContabilizacao)
                .Fetch(obj => obj.CentroResultadoEscrituracao)
                .ToList();
        }



        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> FiltrarPorEmpresa(int empresa, IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> query)
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> result = from obj in query where obj.Ativo && obj.Empresa != null && obj.Empresa.Codigo == empresa select obj;

            return result.OrderByDescending(obj => obj.TipoOperacao != null).ToList();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> FiltrarPorParticipantesEmpresa(int empresa, double remetente, double destinatario, double expedidor, double recebedor, double tomador, int grupoRemetente, int grupoDestinatario, int grupoTomador, int codigoCategoriaDestinatario, int codigoCategoriaRemetente, int codigoCategoriaTomador, IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> query)
        {
            System.Linq.Expressions.Expression<System.Func<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado, bool>> predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>();

            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> result = from obj in query where obj.Ativo && obj.Empresa != null && obj.Empresa.Codigo == empresa select obj;

            if (remetente > 0)
            {
                if ((grupoRemetente > 0) && (codigoCategoriaRemetente > 0))
                    predicateOr = predicateOr.Or(obj => (obj.Remetente != null && obj.Remetente.CPF_CNPJ == remetente) || (obj.GrupoRemetente != null && obj.GrupoRemetente.Codigo == grupoRemetente) || (obj.CategoriaRemetente != null && obj.CategoriaRemetente.Codigo == codigoCategoriaRemetente));
                else if (grupoRemetente > 0)
                    predicateOr = predicateOr.Or(obj => (obj.Remetente != null && obj.Remetente.CPF_CNPJ == remetente) || (obj.GrupoRemetente != null && obj.GrupoRemetente.Codigo == grupoRemetente));
                else if (codigoCategoriaRemetente > 0)
                    predicateOr = predicateOr.Or(obj => (obj.Remetente != null && obj.Remetente.CPF_CNPJ == remetente) || (obj.CategoriaRemetente != null && obj.CategoriaRemetente.Codigo == codigoCategoriaRemetente));
                else
                    predicateOr = predicateOr.Or(obj => obj.Remetente != null && obj.Remetente.CPF_CNPJ == remetente);
            }

            if (destinatario > 0)
            {
                if ((grupoDestinatario > 0) && (codigoCategoriaDestinatario > 0))
                    predicateOr = predicateOr.Or(obj => (obj.Destinatario != null && obj.Destinatario.CPF_CNPJ == destinatario) || (obj.GrupoDestinatario != null && obj.GrupoDestinatario.Codigo == grupoDestinatario) || (obj.CategoriaDestinatario != null && obj.CategoriaDestinatario.Codigo == codigoCategoriaDestinatario));
                else if (grupoDestinatario > 0)
                    predicateOr = predicateOr.Or(obj => (obj.Destinatario != null && obj.Destinatario.CPF_CNPJ == destinatario) || (obj.GrupoDestinatario != null && obj.GrupoDestinatario.Codigo == grupoDestinatario));
                else if (codigoCategoriaDestinatario > 0)
                    predicateOr = predicateOr.Or(obj => (obj.Destinatario != null && obj.Destinatario.CPF_CNPJ == destinatario) || (obj.CategoriaDestinatario != null && obj.CategoriaDestinatario.Codigo == codigoCategoriaDestinatario));
                else
                    predicateOr = predicateOr.Or(obj => obj.Destinatario != null && obj.Destinatario.CPF_CNPJ == destinatario);
            }

            if (tomador > 0)
            {
                if ((grupoTomador > 0) && (codigoCategoriaTomador > 0))
                    predicateOr = predicateOr.Or(obj => (obj.Tomador != null && obj.Tomador.CPF_CNPJ == tomador) || (obj.GrupoTomador != null && obj.GrupoTomador.Codigo == grupoTomador) || (obj.CategoriaTomador != null && obj.CategoriaTomador.Codigo == codigoCategoriaTomador));
                else if (grupoTomador > 0)
                    predicateOr = predicateOr.Or(obj => (obj.Tomador != null && obj.Tomador.CPF_CNPJ == tomador) || (obj.GrupoTomador != null && obj.GrupoTomador.Codigo == grupoTomador));
                else if (codigoCategoriaTomador > 0)
                    predicateOr = predicateOr.Or(obj => (obj.Tomador != null && obj.Tomador.CPF_CNPJ == tomador) || (obj.CategoriaTomador != null && obj.CategoriaTomador.Codigo == codigoCategoriaTomador));
                else
                    predicateOr = predicateOr.Or(obj => obj.Tomador != null && obj.Tomador.CPF_CNPJ == tomador);
            }

            if (expedidor > 0)
                predicateOr = predicateOr.Or(obj => obj.Expedidor != null && obj.Expedidor.CPF_CNPJ == expedidor);

            if (recebedor > 0)
                predicateOr = predicateOr.Or(obj => obj.Recebedor != null && obj.Recebedor.CPF_CNPJ == recebedor);

            result = result.Where(predicateOr);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> FiltrarPorParticipantes(double remetente, double destinatario, double expedidor, double recebedor, double tomador, int grupoRemetente, int grupoDestinatario, int grupoTomador, int codigoCategoriaDestinatario, int codigoCategoriaRemetente, int codigoCategoriaTomador, IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> query)
        {
            System.Linq.Expressions.Expression<System.Func<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado, bool>> predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>();

            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> result = from obj in query where obj.Ativo select obj;

            if (remetente > 0)
            {
                if ((grupoRemetente > 0) && (codigoCategoriaRemetente > 0))
                    predicateOr = predicateOr.Or(obj => (obj.Remetente != null && obj.Remetente.CPF_CNPJ == remetente) || (obj.GrupoRemetente != null && obj.GrupoRemetente.Codigo == grupoRemetente) || (obj.CategoriaRemetente != null && obj.CategoriaRemetente.Codigo == codigoCategoriaRemetente));
                else if (grupoRemetente > 0)
                    predicateOr = predicateOr.Or(obj => (obj.Remetente != null && obj.Remetente.CPF_CNPJ == remetente) || (obj.GrupoRemetente != null && obj.GrupoRemetente.Codigo == grupoRemetente));
                else if (codigoCategoriaRemetente > 0)
                    predicateOr = predicateOr.Or(obj => (obj.Remetente != null && obj.Remetente.CPF_CNPJ == remetente) || (obj.CategoriaRemetente != null && obj.CategoriaRemetente.Codigo == codigoCategoriaRemetente));
                else
                    predicateOr = predicateOr.Or(obj => obj.Remetente != null && obj.Remetente.CPF_CNPJ == remetente);
            }

            if (destinatario > 0)
            {
                if ((grupoDestinatario > 0) && (codigoCategoriaDestinatario > 0))
                    predicateOr = predicateOr.Or(obj => (obj.Destinatario != null && obj.Destinatario.CPF_CNPJ == destinatario) || (obj.GrupoDestinatario != null && obj.GrupoDestinatario.Codigo == grupoDestinatario) || (obj.CategoriaDestinatario != null && obj.CategoriaDestinatario.Codigo == codigoCategoriaDestinatario));
                else if (grupoDestinatario > 0)
                    predicateOr = predicateOr.Or(obj => (obj.Destinatario != null && obj.Destinatario.CPF_CNPJ == destinatario) || (obj.GrupoDestinatario != null && obj.GrupoDestinatario.Codigo == grupoDestinatario));
                else if (codigoCategoriaDestinatario > 0)
                    predicateOr = predicateOr.Or(obj => (obj.Destinatario != null && obj.Destinatario.CPF_CNPJ == destinatario) || (obj.CategoriaDestinatario != null && obj.CategoriaDestinatario.Codigo == codigoCategoriaDestinatario));
                else
                    predicateOr = predicateOr.Or(obj => obj.Destinatario != null && obj.Destinatario.CPF_CNPJ == destinatario);
            }

            if (tomador > 0)
            {
                if ((grupoTomador > 0) && (codigoCategoriaTomador > 0))
                    predicateOr = predicateOr.Or(obj => (obj.Tomador != null && obj.Tomador.CPF_CNPJ == tomador) || (obj.GrupoTomador != null && obj.GrupoTomador.Codigo == grupoTomador) || (obj.CategoriaTomador != null && obj.CategoriaTomador.Codigo == codigoCategoriaTomador));
                else if (grupoTomador > 0)
                    predicateOr = predicateOr.Or(obj => (obj.Tomador != null && obj.Tomador.CPF_CNPJ == tomador) || (obj.GrupoTomador != null && obj.GrupoTomador.Codigo == grupoTomador));
                else if (codigoCategoriaTomador > 0)
                    predicateOr = predicateOr.Or(obj => (obj.Tomador != null && obj.Tomador.CPF_CNPJ == tomador) || (obj.CategoriaTomador != null && obj.CategoriaTomador.Codigo == codigoCategoriaTomador));
                else
                    predicateOr = predicateOr.Or(obj => obj.Tomador != null && obj.Tomador.CPF_CNPJ == tomador);
            }

            if (expedidor > 0)
                predicateOr = predicateOr.Or(obj => obj.Expedidor != null && obj.Expedidor.CPF_CNPJ == expedidor);

            if (recebedor > 0)
                predicateOr = predicateOr.Or(obj => obj.Recebedor != null && obj.Recebedor.CPF_CNPJ == recebedor);

            result = result.Where(predicateOr);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> FiltrarPorGrupoProdutoEmbarcador(int grupoProdutoEmbarcador, IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> query)
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> result = from obj in query where obj.Ativo select obj;

            if (grupoProdutoEmbarcador > 0)
            {
                result = result.Where(obj => obj.GrupoProduto != null && obj.GrupoProduto.Codigo == grupoProdutoEmbarcador);
            }

            result = result.Where(obj =>
                obj.Remetente == null &&
                obj.Destinatario == null &&
                obj.Expedidor == null &&
                obj.Recebedor == null &&
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

            return result.ToList();
        }



        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> FiltrarPorTipoOcorrencia(int tipoOcorrencia, IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> query)
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> result = from obj in query where obj.Ativo select obj;

            if (tipoOcorrencia > 0)
            {
                result = result.Where(obj => obj.TipoOcorrencia != null && obj.TipoOcorrencia.Codigo == tipoOcorrencia);
            }

            result = result.Where(obj =>
                obj.Remetente == null &&
                obj.Destinatario == null &&
                obj.Expedidor == null &&
                obj.Recebedor == null &&
                obj.Tomador == null &&
                obj.GrupoDestinatario == null &&
                obj.GrupoRemetente == null &&
                obj.GrupoTomador == null &&
                obj.CategoriaDestinatario == null &&
                obj.CategoriaRemetente == null &&
                obj.CategoriaTomador == null &&
                obj.GrupoProduto == null &&
                obj.GrupoProduto == null &&
                obj.TipoOperacao == null
            );

            return result.ToList();
        }


        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> FiltrarPorTipoOperacao(int tipoOperacao, IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> query)
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> result = from obj in query where obj.Ativo select obj;

            if (tipoOperacao > 0)
            {
                result = result.Where(obj => obj.TipoOperacao != null && obj.TipoOperacao.Codigo == tipoOperacao);
            }

            result = result.Where(obj =>
                obj.Remetente == null &&
                obj.Destinatario == null &&
                obj.Expedidor == null &&
                obj.Recebedor == null &&
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

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> FiltrarPorRotaFrete(int rotaFrete, IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> query)
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> result = from obj in query where obj.Ativo select obj;

            if (rotaFrete > 0)
            {
                result = result.Where(obj => obj.RotaFrete != null && obj.RotaFrete.Codigo == rotaFrete);
            }

            result = result.Where(obj =>
                obj.Remetente == null &&
                obj.Destinatario == null &&
                obj.Expedidor == null &&
                obj.Recebedor == null &&
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

            return result.ToList();
        }

        //public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> FiltrarPorEmpresa(int empresa)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>();

        //    var result = from obj in query where obj.Ativo && obj.Empresa.Codigo == empresa select obj;

        //    return result.OrderByDescending(obj => obj.TipoOperacao).ToList();
        //}

        //public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> FiltrarPorParticipantesEmpresa(int empresa, double remetente, double destinatario, double tomador, int grupoRemetente, int grupoDestinatario, int grupoTomador, int codigoCategoriaDestinatario, int codigoCategoriaRemetente, int codigoCategoriaTomador)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>();
        //    var predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>();

        //    var result = from obj in query where obj.Ativo && obj.Empresa.Codigo == empresa select obj;

        //    if (remetente > 0)
        //    {
        //        if ((grupoRemetente > 0) && (codigoCategoriaRemetente > 0))
        //            predicateOr = predicateOr.Or(obj => obj.Remetente.CPF_CNPJ == remetente || obj.GrupoRemetente.Codigo == grupoRemetente || obj.CategoriaRemetente.Codigo == codigoCategoriaRemetente);
        //        else if (grupoRemetente > 0)
        //            predicateOr = predicateOr.Or(obj => obj.Remetente.CPF_CNPJ == remetente || obj.GrupoRemetente.Codigo == grupoRemetente);
        //        else if (codigoCategoriaRemetente > 0)
        //            predicateOr = predicateOr.Or(obj => obj.Remetente.CPF_CNPJ == remetente || obj.CategoriaRemetente.Codigo == codigoCategoriaRemetente);
        //        else
        //            predicateOr = predicateOr.Or(obj => obj.Remetente.CPF_CNPJ == remetente);
        //    }

        //    if (destinatario > 0)
        //    {
        //        if ((grupoDestinatario > 0) && (codigoCategoriaDestinatario > 0))
        //            predicateOr = predicateOr.Or(obj => obj.Destinatario.CPF_CNPJ == destinatario || obj.GrupoDestinatario.Codigo == grupoDestinatario || obj.CategoriaDestinatario.Codigo == codigoCategoriaDestinatario);
        //        else if (grupoDestinatario > 0)
        //            predicateOr = predicateOr.Or(obj => obj.Destinatario.CPF_CNPJ == destinatario || obj.GrupoDestinatario.Codigo == grupoDestinatario);
        //        else if (codigoCategoriaDestinatario > 0)
        //            predicateOr = predicateOr.Or(obj => obj.Destinatario.CPF_CNPJ == destinatario || obj.CategoriaDestinatario.Codigo == codigoCategoriaDestinatario);
        //        else
        //            predicateOr = predicateOr.Or(obj => obj.Destinatario.CPF_CNPJ == destinatario);
        //    }

        //    if (tomador > 0)
        //    {
        //        if ((grupoTomador > 0) && (codigoCategoriaTomador > 0))
        //            predicateOr = predicateOr.Or(obj => obj.Tomador.CPF_CNPJ == tomador || obj.GrupoTomador.Codigo == grupoTomador || obj.CategoriaTomador.Codigo == codigoCategoriaTomador);
        //        else if (grupoTomador > 0)
        //            predicateOr = predicateOr.Or(obj => obj.Tomador.CPF_CNPJ == tomador || obj.GrupoTomador.Codigo == grupoTomador);
        //        else if (codigoCategoriaTomador > 0)
        //            predicateOr = predicateOr.Or(obj => obj.Tomador.CPF_CNPJ == tomador || obj.CategoriaTomador.Codigo == codigoCategoriaTomador);
        //        else
        //            predicateOr = predicateOr.Or(obj => obj.Tomador.CPF_CNPJ == tomador);
        //    }

        //    result = result.Where(predicateOr);

        //    return result.ToList();
        //}

        //public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> FiltrarPorParticipantes(double remetente, double destinatario, double tomador, int grupoRemetente, int grupoDestinatario, int grupoTomador, int codigoCategoriaDestinatario, int codigoCategoriaRemetente, int codigoCategoriaTomador)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>();
        //    var predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>();

        //    var result = from obj in query where obj.Ativo select obj;

        //    if (remetente > 0)
        //    {
        //        if ((grupoRemetente > 0) && (codigoCategoriaRemetente > 0))
        //            predicateOr = predicateOr.Or(obj => obj.Remetente.CPF_CNPJ == remetente || obj.GrupoRemetente.Codigo == grupoRemetente || obj.CategoriaRemetente.Codigo == codigoCategoriaRemetente);
        //        else if (grupoRemetente > 0)
        //            predicateOr = predicateOr.Or(obj => obj.Remetente.CPF_CNPJ == remetente || obj.GrupoRemetente.Codigo == grupoRemetente);
        //        else if (codigoCategoriaRemetente > 0)
        //            predicateOr = predicateOr.Or(obj => obj.Remetente.CPF_CNPJ == remetente || obj.CategoriaRemetente.Codigo == codigoCategoriaRemetente);
        //        else
        //            predicateOr = predicateOr.Or(obj => obj.Remetente.CPF_CNPJ == remetente);
        //    }

        //    if (destinatario > 0)
        //    {
        //        if ((grupoDestinatario > 0) && (codigoCategoriaDestinatario > 0))
        //            predicateOr = predicateOr.Or(obj => obj.Destinatario.CPF_CNPJ == destinatario || obj.GrupoDestinatario.Codigo == grupoDestinatario || obj.CategoriaDestinatario.Codigo == codigoCategoriaDestinatario);
        //        else if (grupoDestinatario > 0)
        //            predicateOr = predicateOr.Or(obj => obj.Destinatario.CPF_CNPJ == destinatario || obj.GrupoDestinatario.Codigo == grupoDestinatario);
        //        else if (codigoCategoriaDestinatario > 0)
        //            predicateOr = predicateOr.Or(obj => obj.Destinatario.CPF_CNPJ == destinatario || obj.CategoriaDestinatario.Codigo == codigoCategoriaDestinatario);
        //        else
        //            predicateOr = predicateOr.Or(obj => obj.Destinatario.CPF_CNPJ == destinatario);
        //    }

        //    if (tomador > 0)
        //    {
        //        if ((grupoTomador > 0) && (codigoCategoriaTomador > 0))
        //            predicateOr = predicateOr.Or(obj => obj.Tomador.CPF_CNPJ == tomador || obj.GrupoTomador.Codigo == grupoTomador || obj.CategoriaTomador.Codigo == codigoCategoriaTomador);
        //        else if (grupoTomador > 0)
        //            predicateOr = predicateOr.Or(obj => obj.Tomador.CPF_CNPJ == tomador || obj.GrupoTomador.Codigo == grupoTomador);
        //        else if (codigoCategoriaTomador > 0)
        //            predicateOr = predicateOr.Or(obj => obj.Tomador.CPF_CNPJ == tomador || obj.CategoriaTomador.Codigo == codigoCategoriaTomador);
        //        else
        //            predicateOr = predicateOr.Or(obj => obj.Tomador.CPF_CNPJ == tomador);
        //    }

        //    result = result.Where(predicateOr);

        //    return result.ToList();
        //}

        //public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> FiltrarPorGrupoProdutoEmbarcador(int grupoProdutoEmbarcador)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>();

        //    var result = from obj in query where obj.Ativo select obj;

        //    if (grupoProdutoEmbarcador > 0)
        //    {
        //        result = result.Where(obj => obj.GrupoProduto.Codigo == grupoProdutoEmbarcador);
        //    }

        //    result = result.Where(obj =>
        //        obj.Remetente == null &&
        //        obj.Destinatario == null &&
        //        obj.Tomador == null &&
        //        obj.GrupoDestinatario == null &&
        //        obj.GrupoRemetente == null &&
        //        obj.GrupoTomador == null &&
        //        obj.CategoriaDestinatario == null &&
        //        obj.CategoriaRemetente == null &&
        //        obj.CategoriaTomador == null &&
        //        obj.TipoOperacao == null &&
        //        obj.RotaFrete == null
        //    );

        //    return result.ToList();
        //}


        //public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> FiltrarPorTipoOperacao(int tipoOperacao)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>();

        //    var result = from obj in query where obj.Ativo select obj;

        //    if (tipoOperacao > 0)
        //    {
        //        result = result.Where(obj => obj.TipoOperacao.Codigo == tipoOperacao);
        //    }

        //    result = result.Where(obj =>
        //        obj.Remetente == null &&
        //        obj.Destinatario == null &&
        //        obj.Tomador == null &&
        //        obj.GrupoDestinatario == null &&
        //        obj.GrupoRemetente == null &&
        //        obj.GrupoTomador == null &&
        //        obj.CategoriaDestinatario == null &&
        //        obj.CategoriaRemetente == null &&
        //        obj.CategoriaTomador == null &&
        //        obj.GrupoProduto == null &&
        //        obj.RotaFrete == null
        //    );

        //    return result.ToList();
        //}

        //public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> FiltrarPorRotaFrete(int rotaFrete)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>();

        //    var result = from obj in query where obj.Ativo select obj;

        //    if (rotaFrete > 0)
        //    {
        //        result = result.Where(obj => obj.RotaFrete.Codigo == rotaFrete);
        //    }

        //    result = result.Where(obj =>
        //        obj.Remetente == null &&
        //        obj.Destinatario == null &&
        //        obj.Tomador == null &&
        //        obj.GrupoDestinatario == null &&
        //        obj.GrupoRemetente == null &&
        //        obj.GrupoTomador == null &&
        //        obj.CategoriaDestinatario == null &&
        //        obj.CategoriaRemetente == null &&
        //        obj.CategoriaTomador == null &&
        //        obj.GrupoProduto == null &&
        //        obj.TipoOperacao == null
        //    );

        //    return result.ToList();
        //}


        public int ContarRegrasCadastradas()
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>();
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> result = from obj in query where obj.Ativo select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> FiltrarTodasRegrasCadastradas()
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>();
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> result = from obj in query where obj.Ativo select obj;

            return result.ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.ConfiguracaoContabil.ConfiguracaoCentroResultado> ConsultarRelatorioConfiguracaoCentroResultado(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaConfiguracaoCentroResultado filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            NHibernate.ISQLQuery consultaQuantidade = new ConsultaConfiguracaoCentroResultado().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaQuantidade.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.ConfiguracaoContabil.ConfiguracaoCentroResultado)));

            return consultaQuantidade.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.ConfiguracaoContabil.ConfiguracaoCentroResultado>();
        }

        public int ContarConsultaRelatorioConfiguracaoCentroResultado(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaConfiguracaoCentroResultado filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            NHibernate.ISQLQuery consultaQuantidade = new ConsultaConfiguracaoCentroResultado().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaQuantidade.SetTimeout(600).UniqueResult<int>();
        }


        #endregion

        #region Métodos Publicos

        public Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>();
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> Consultar(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaCentroResultado filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> query = MontarConsulta(filtrosPesquisa);
            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .Fetch(obj => obj.TipoOperacao)
                .Fetch(obj => obj.GrupoTomador)
                .Fetch(obj => obj.CentroResultadoEscrituracao)
                .Fetch(obj => obj.CentroResultadoContabilizacao)
                .Fetch(obj => obj.CategoriaDestinatario)
                .Fetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Tomador)
                .ThenFetch(obj => obj.Localidade)
                .ToList();

        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaCentroResultado filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> query = MontarConsulta(filtrosPesquisa);
            return query.Count();
        }

        public Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado BuscarPorParametros(Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado)
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>();

            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> result = from obj in query select obj;

            if (configuracaoCentroResultado.Remetente != null)
                result = result.Where(obj => obj.Remetente == configuracaoCentroResultado.Remetente);
            else
                result = result.Where(obj => obj.Remetente == null);

            if (configuracaoCentroResultado.Destinatario != null)
                result = result.Where(obj => obj.Destinatario == configuracaoCentroResultado.Destinatario);
            else
                result = result.Where(obj => obj.Destinatario == null);

            if (configuracaoCentroResultado.Expedidor != null)
                result = result.Where(obj => obj.Expedidor == configuracaoCentroResultado.Expedidor);
            else
                result = result.Where(obj => obj.Expedidor == null);

            if (configuracaoCentroResultado.Recebedor != null)
                result = result.Where(obj => obj.Recebedor == configuracaoCentroResultado.Recebedor);
            else
                result = result.Where(obj => obj.Recebedor == null);

            if (configuracaoCentroResultado.Tomador != null)
                result = result.Where(obj => obj.Tomador == configuracaoCentroResultado.Tomador);
            else
                result = result.Where(obj => obj.Tomador == null);

            if (configuracaoCentroResultado.Empresa != null)
                result = result.Where(obj => obj.Empresa == configuracaoCentroResultado.Empresa);
            else
                result = result.Where(obj => obj.Empresa == null);

            if (configuracaoCentroResultado.TipoOperacao != null)
                result = result.Where(obj => obj.TipoOperacao == configuracaoCentroResultado.TipoOperacao);
            else
                result = result.Where(obj => obj.TipoOperacao == null);

            if (configuracaoCentroResultado.TipoOcorrencia != null)
                result = result.Where(obj => obj.TipoOcorrencia == configuracaoCentroResultado.TipoOcorrencia);
            else
                result = result.Where(obj => obj.TipoOcorrencia == null);

            if (configuracaoCentroResultado.RotaFrete != null)
                result = result.Where(obj => obj.RotaFrete == configuracaoCentroResultado.RotaFrete);
            else
                result = result.Where(obj => obj.RotaFrete == null);

            if (configuracaoCentroResultado.GrupoProduto != null)
                result = result.Where(obj => obj.GrupoProduto == configuracaoCentroResultado.GrupoProduto);
            else
                result = result.Where(obj => obj.GrupoProduto == null);

            if (configuracaoCentroResultado.GrupoRemetente != null)
                result = result.Where(obj => obj.GrupoRemetente == configuracaoCentroResultado.GrupoRemetente);
            else
                result = result.Where(obj => obj.GrupoRemetente == null);

            if (configuracaoCentroResultado.GrupoDestinatario != null)
                result = result.Where(obj => obj.GrupoDestinatario == configuracaoCentroResultado.GrupoDestinatario);
            else
                result = result.Where(obj => obj.GrupoDestinatario == null);

            if (configuracaoCentroResultado.GrupoTomador != null)
                result = result.Where(obj => obj.GrupoTomador == configuracaoCentroResultado.GrupoTomador);
            else
                result = result.Where(obj => obj.GrupoTomador == null);

            if (configuracaoCentroResultado.CategoriaRemetente != null)
                result = result.Where(obj => obj.CategoriaRemetente == configuracaoCentroResultado.CategoriaRemetente);
            else
                result = result.Where(obj => obj.CategoriaRemetente == null);

            if (configuracaoCentroResultado.CategoriaDestinatario != null)
                result = result.Where(obj => obj.CategoriaDestinatario == configuracaoCentroResultado.CategoriaDestinatario);
            else
                result = result.Where(obj => obj.CategoriaDestinatario == null);

            if (configuracaoCentroResultado.CategoriaTomador != null)
                result = result.Where(obj => obj.CategoriaTomador == configuracaoCentroResultado.CategoriaTomador);
            else
                result = result.Where(obj => obj.CategoriaTomador == null);

            if (configuracaoCentroResultado.Filial != null)
                result = result.Where(obj => obj.Filial == configuracaoCentroResultado.Filial);
            else
                result = result.Where(obj => obj.Filial == null);

            if (configuracaoCentroResultado.Origem != null)
                result = result.Where(obj => obj.Origem == configuracaoCentroResultado.Origem);
            else
                result = result.Where(obj => obj.Origem == null);

            result = result.Where(obj => obj.Ativo);

            return result.FirstOrDefault();
        }

        #endregion


        #region Métodos Privados
        private IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> MontarConsulta(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaCentroResultado filtrosPesquisa)
        {

            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado>();

            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> result = from obj in query select obj;

            if (filtrosPesquisa.Remetente > 0)
                result = result.Where(obj => obj.Remetente.CPF_CNPJ == filtrosPesquisa.Remetente);

            if (filtrosPesquisa.Destinatario > 0)
                result = result.Where(obj => obj.Destinatario.CPF_CNPJ == filtrosPesquisa.Destinatario);

            if (filtrosPesquisa.Tomador > 0)
                result = result.Where(obj => obj.Tomador.CPF_CNPJ == filtrosPesquisa.Tomador);

            if (filtrosPesquisa.GrupoRemetente > 0)
                result = result.Where(obj => obj.GrupoRemetente.Codigo == filtrosPesquisa.GrupoRemetente);

            if (filtrosPesquisa.GrupoDestinatario > 0)
                result = result.Where(obj => obj.GrupoDestinatario.Codigo == filtrosPesquisa.GrupoDestinatario);

            if (filtrosPesquisa.GrupoTomador > 0)
                result = result.Where(obj => obj.GrupoTomador.Codigo == filtrosPesquisa.GrupoTomador);

            if (filtrosPesquisa.CategoriaDestinatario > 0)
                result = result.Where(obj => obj.CategoriaDestinatario.Codigo == filtrosPesquisa.CategoriaDestinatario);

            if (filtrosPesquisa.CategoriaRemetente > 0)
                result = result.Where(obj => obj.CategoriaRemetente.Codigo == filtrosPesquisa.CategoriaRemetente);

            if (filtrosPesquisa.CategoriaTomador > 0)
                result = result.Where(obj => obj.CategoriaTomador.Codigo == filtrosPesquisa.CategoriaTomador);

            if (filtrosPesquisa.Empresas?.Count() > 0)
                result = result.Where(obj => filtrosPesquisa.Empresas.Contains(obj.Empresa.Codigo));

            if (filtrosPesquisa.TipoOperacao > 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == filtrosPesquisa.TipoOperacao);

            if (filtrosPesquisa.TipoOcorrencia > 0)
                result = result.Where(obj => obj.TipoOcorrencia.Codigo == filtrosPesquisa.TipoOcorrencia);

            if (filtrosPesquisa.CentroResultado > 0)
                result = result.Where(obj => (obj.CentroResultadoContabilizacao.Codigo == filtrosPesquisa.CentroResultado || obj.CentroResultadoEscrituracao.Codigo == filtrosPesquisa.CentroResultado));

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (filtrosPesquisa.Expedidor > 0)
                result = result.Where(obj => obj.Expedidor.CPF_CNPJ == filtrosPesquisa.Expedidor);

            if (filtrosPesquisa.Recebedor > 0)
                result = result.Where(obj => obj.Recebedor.CPF_CNPJ == filtrosPesquisa.Recebedor);

            return result;
        }

        #endregion

    }
}
