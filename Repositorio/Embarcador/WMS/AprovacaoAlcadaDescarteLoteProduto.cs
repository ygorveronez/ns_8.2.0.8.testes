using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.WMS
{
    public class AprovacaoAlcadaDescarteLoteProduto : RepositorioBase<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto>
    {
        public AprovacaoAlcadaDescarteLoteProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool VerificarSePodeAprovar(int codigoDescarte, int codigoRegra, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto > ();
            var resut = from obj in query
                        where
                            obj.Codigo == codigoRegra
                            && obj.Descarte.Codigo == codigoDescarte
                            && obj.Usuario.Codigo == codigoUsuario
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente
                        select obj;

            return resut.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto> ConsultarAutorizacoesPorDescarte(int codigo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto>();
            var result = from obj in query where obj.Descarte.Codigo == codigo select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaAutorizacoesPorDescarte(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto>();
            var result = from obj in query where obj.Descarte.Codigo == codigo select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto> BuscarPorDescarteUsuarioSituacao(int codigo, int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto>();
            var result = from obj in query
                         where
                            obj.Descarte.Codigo == codigo &&
                            obj.Usuario.Codigo == usuario &&
                            obj.Situacao == situacao
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto> BuscarPorDescarteEUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto>();
            var result = from obj in query where obj.Descarte.Codigo == codigo select obj;

            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            return result.ToList();
        }

        public int ContarRejeitadas(int codigoDescarte, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto>();
            var resut = from obj in query
                        where
                            obj.Descarte.Codigo == codigoDescarte
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada
                            && (obj.RegraDescarte.Codigo == codigoRegra || obj.RegraDescarte == null)
                        select obj;

            return resut.Count();
        }

        public int ContarPendentes(int codigoDescarte, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto>();
            var resut = from obj in query
                        where
                            obj.Descarte.Codigo == codigoDescarte
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente
                            && (obj.RegraDescarte.Codigo == codigoRegra || obj.RegraDescarte == null)
                        select obj;

            return resut.Count();
        }

        public int ContarAprovacoesSolicitacao(int codigoDescarte, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto>();
            var resut = from obj in query
                        where
                            obj.Descarte.Codigo == codigoDescarte
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada
                            && (obj.RegraDescarte.Codigo == codigoRegra || obj.RegraDescarte == null)
                        select obj;

            return resut.Count();
        }

        public List<Dominio.Entidades.Embarcador.WMS.RegraDescarte> BuscarRegrasDescarte(int codigoDescarte)
        {
            var queryGroup = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.RegraDescarte>();

            var resultGroup = from obj in queryGroup where obj.Descarte.Codigo == codigoDescarte select obj.RegraDescarte;
            var result = from obj in query where resultGroup.Contains(obj) select obj;

            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador> _Consultar(int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador? situacao, string numero, int produto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador>();
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto>();

            var resultAutorizacao = from obj in queryAutorizacao select obj.Descarte;
            var result = from obj in query where resultAutorizacao.Contains(obj) select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(numero))
                result = result.Where(obj => obj.Lote.Numero == numero);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date <= dataFinal);

            if (situacao.HasValue)
                result = result.Where(obj => obj.Situacao == situacao.Value);

            if (produto > 0)
                result = result.Where(obj => obj.Lote.ProdutoEmbarcador.Codigo == produto);

            // Filtros da autorizacao
            if (usuario > 0)
                result = result.Where(obj => obj.DescartesAutorizacoes.Any(aut => aut.Usuario.Codigo == usuario));

            // O filtro que mais influencia é o de situacao da ocorrencia, pois AgAprovacao, AgAutorizacaoEmissao e AutorizacaoPendente tem o mesmo comportamento
            bool situacaoPendentes = situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.AgAprovacao;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacaoAutorizacaoPendente = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente;

            if (situacaoPendentes)
                result = result.Where(obj => obj.DescartesAutorizacoes.Any(aut => usuario > 0 ? (aut.Usuario.Codigo == usuario && aut.Situacao == situacaoAutorizacaoPendente) : (aut.Situacao == situacaoAutorizacaoPendente)));


            return result;
        }

        public List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador> Consultar(int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador? situacao, string numero, int produto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(usuario, dataInicial, dataFinal, situacao, numero, produto);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador? situacao, string numero, int produto)
        {
            var result = _Consultar(usuario, dataInicial, dataFinal, situacao, numero, produto);

            return result.Count();
        }
    }
}
