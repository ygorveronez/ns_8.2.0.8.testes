using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class OcorrenciaLote : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote>
    {
        public OcorrenciaLote(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote>();

            int? ultimoNumero = query.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote> Consultar(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrenciaLote filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote> result = Consultar(filtrosPesquisa);

            result = result.Fetch(o => o.TipoOcorrencia);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrenciaLote filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote> result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> ConsultarCarga(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrenciaLoteCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.Carga> result = ConsultarCarga(filtrosPesquisa);

            result = result
                .Fetch(o => o.DadosSumarizados);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsultaCarga(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrenciaLoteCarga filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.Carga> result = ConsultarCarga(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> ObterCargas(bool selecionarTodos, List<int> codigosCargas, Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrenciaLoteCarga filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.Carga> result = ConsultarCarga(filtrosPesquisa);

            if (selecionarTodos)
                result = result.Where(o => !codigosCargas.Contains(o.Codigo));
            else
                result = result.Where(o => codigosCargas.Contains(o.Codigo));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ConsultarOcorrencia(int codigoOcorrenciaLote, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia>()
                .Where(o => o.OcorrenciaLote.Codigo == codigoOcorrenciaLote && o.CargaOcorrencia != null);

            return ObterLista(query.Select(o => o.CargaOcorrencia), parametroConsulta);
        }

        public int ContarConsultaOcorrencia(int codigoOcorrenciaLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia>()
                            .Where(o => o.OcorrenciaLote.Codigo == codigoOcorrenciaLote && o.CargaOcorrencia != null);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote> BuscarOcorrenciaLotePendentesGeracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote>()
                .Where(obj => obj.Situacao == SituacaoOcorrenciaLote.EmGeracao);

            query = query
                .Fetch(o => o.TipoOcorrencia)
                .Fetch(o => o.Usuario);

            return query.ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote> Consultar(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrenciaLote filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal > 0)
                result = result.Where(obj => obj.Numero >= filtrosPesquisa.NumeroInicial && obj.Numero <= filtrosPesquisa.NumeroFinal);
            else if (filtrosPesquisa.NumeroInicial > 0)
                result = result.Where(obj => obj.Numero == filtrosPesquisa.NumeroInicial);
            else if (filtrosPesquisa.NumeroFinal > 0)
                result = result.Where(obj => obj.Numero == filtrosPesquisa.NumeroFinal);

            if (filtrosPesquisa.CodigoTipoOcorrencia > 0)
                result = result.Where(obj => obj.TipoOcorrencia.Codigo == filtrosPesquisa.CodigoTipoOcorrencia);

            if (filtrosPesquisa.Situacao != SituacaoOcorrenciaLote.Todos)
                result = result.Where(obj => obj.Situacao == filtrosPesquisa.Situacao);

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.Carga> ConsultarCarga(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrenciaLoteCarga filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();

            var result = from obj in query select obj;

            var queryOcorrenciaLote = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia>();
            if (filtrosPesquisa.CodigoOcorrenciaLote > 0)
            {
                var resultQueryOcorrenciaLote = from obj in queryOcorrenciaLote where obj.OcorrenciaLote.Codigo == filtrosPesquisa.CodigoOcorrenciaLote select obj;

                return result.Where(o => resultQueryOcorrenciaLote.Where(r => r.Carga.Codigo == o.Codigo).Any());
            }
            else
            {
                var resultQueryOcorrenciaLote = from obj in queryOcorrenciaLote select obj;

                result = result.Where(o => !resultQueryOcorrenciaLote.Where(r => r.Carga.Codigo == o.Codigo).Any());

                result = result.Where(obj => obj.CargaFechada && (obj.SituacaoCarga == SituacaoCarga.EmTransporte || obj.SituacaoCarga == SituacaoCarga.Encerrada));
            }

            if (filtrosPesquisa.DataCriacaoInicial != DateTime.MinValue)
                result = result.Where(obj => obj.DataCriacaoCarga.Date >= filtrosPesquisa.DataCriacaoInicial);

            if (filtrosPesquisa.DataCriacaoFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataCriacaoCarga.Date <= filtrosPesquisa.DataCriacaoFinal);

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                result = result.Where(obj => obj.GrupoPessoaPrincipal.Codigo == filtrosPesquisa.CodigoGrupoPessoas);

            if (filtrosPesquisa.CnpjCpfRemetente > 0)
                result = result.Where(obj => obj.DadosSumarizados.ClientesRemetentes.Any(o => o.CPF_CNPJ == filtrosPesquisa.CnpjCpfRemetente));

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            return result;
        }

        #endregion
    }
}
