using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Terceiros
{
    public class AprovacaoAlcadaContratoFrete : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>
    {
        public AprovacaoAlcadaContratoFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool VerificarSePodeAprovar(int codigoContrato, int codigoRegra, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();
            var resut = from obj in query
                        where
                            obj.Codigo == codigoRegra
                            && obj.ContratoFrete.Codigo == codigoContrato
                            && obj.Usuario.Codigo == codigoUsuario
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente
                        select obj;

            return resut.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete> ConsultarAutorizacoesPorContrato(int codigo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();
            var result = from obj in query where obj.ContratoFrete.Codigo == codigo select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaAutorizacoesPorContrato(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();
            var result = from obj in query where obj.ContratoFrete.Codigo == codigo select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete> BuscarPorContratoUsuarioSituacao(int codigo, int usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == codigo &&
                            obj.Usuario.Codigo == usuario &&
                            obj.Situacao == situacao
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete> BuscarPorContratoEUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();
            var result = from obj in query where obj.ContratoFrete.Codigo == codigo select obj;

            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            return result.ToList();
        }

        public int ContarRejeitadas(int codigoContrato, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();
            var resut = from obj in query
                        where
                            obj.ContratoFrete.Codigo == codigoContrato
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada
                            && (obj.RegraContratoFreteTerceiro.Codigo == codigoRegra || obj.RegraContratoFreteTerceiro == null)
                        select obj;

            return resut.Count();
        }

        public int ContarPendentes(int codigoContrato, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();
            var resut = from obj in query
                        where
                            obj.ContratoFrete.Codigo == codigoContrato
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente
                            && (obj.RegraContratoFreteTerceiro.Codigo == codigoRegra || obj.RegraContratoFreteTerceiro == null)
                        select obj;

            return resut.Count();
        }

        public int ContarAprovacoesSolicitacao(int codigoContrato, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();
            var resut = from obj in query
                        where
                            obj.ContratoFrete.Codigo == codigoContrato
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada
                            && (obj.RegraContratoFreteTerceiro.Codigo == codigoRegra || obj.RegraContratoFreteTerceiro == null)
                        select obj;

            return resut.Count();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> BuscarRegrasContrato(int codigoContrato)
        {
            var queryGroup = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro>();

            var resultGroup = from obj in queryGroup where obj.ContratoFrete.Codigo == codigoContrato select obj.RegraContratoFreteTerceiro;
            var result = from obj in query where resultGroup.Contains(obj) select obj;

            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> _Consultar(int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete? situacao, int numero, int carga, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();

            var resultAutorizacao = from obj in queryAutorizacao select obj.ContratoFrete;
            var result = from obj in query where resultAutorizacao.Contains(obj) select obj;

            // Filtros
            if (numero > 0)
                result = result.Where(obj => obj.NumeroContrato == numero);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissaoContrato.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissaoContrato.Date <= dataFinal);

            if (situacao.HasValue)
                result = result.Where(obj => obj.SituacaoContratoFrete == situacao.Value);

            if (empresa > 0)
                result = result.Where(obj => obj.TransportadorTerceiro.Codigo == empresa);

            if (carga > 0)
                result = result.Where(obj => obj.Carga.Codigo == carga);

            // Filtros da autorizacao
            if (usuario > 0)
                result = result.Where(obj => obj.ContratoAutorizacoes.Any(aut => aut.Usuario.Codigo == usuario));

            // Filtros da autorizacao
            if (usuario > 0)
                result = result.Where(obj => obj.ContratoAutorizacoes.Any(aut => aut.Usuario.Codigo == usuario));

            // O filtro que mais influencia é o de situacao da ocorrencia, pois AgAprovacao, AgAutorizacaoEmissao e AutorizacaoPendente tem o mesmo comportamento
            bool situacaoPendentes = situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.AgAprovacao;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacaoAutorizacaoPendente = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente;

            if (situacaoPendentes)
                result = result.Where(obj => obj.ContratoAutorizacoes.Any(aut => usuario > 0 ? (aut.Usuario.Codigo == usuario && aut.Situacao == situacaoAutorizacaoPendente) : (aut.Situacao == situacaoAutorizacaoPendente)));


            return result;
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> Consultar(int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete? situacao, int numero, int carga, int empresa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(usuario, dataInicial, dataFinal, situacao, numero, carga, empresa);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete? situacao, int numero, int carga, int empresa)
        {
            var result = _Consultar(usuario, dataInicial, dataFinal, situacao, numero, carga, empresa);

            return result.Count();
        }

        public int ContarAprovacoesNecessarias(int codigo)
        {
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro>();

            var resutAutorizacao = from aut in queryAutorizacao where aut.ContratoFrete.Codigo == codigo select aut.RegraContratoFreteTerceiro;
            var resut = from obj in query where resutAutorizacao.Contains(obj) select obj;

            return resut.Sum(o => (int?)o.NumeroAprovadores) ?? 0;
        }

        public int ContarAprovacoes(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();
            var resut = from obj in query
                        where
                            obj.ContratoFrete.Codigo == codigo
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada
                        select obj;

            return resut.Count();
        }

        public int ContarReprovacoes(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();
            var resut = from obj in query
                        where
                            obj.ContratoFrete.Codigo == codigo
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada
                        select obj;

            return resut.Count();
        }
    }
}
