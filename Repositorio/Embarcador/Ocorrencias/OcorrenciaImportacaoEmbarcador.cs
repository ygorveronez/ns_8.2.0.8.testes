using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class OcorrenciaImportacaoEmbarcador : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador>
    {
        #region Metodos Publicos

        public OcorrenciaImportacaoEmbarcador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador BuscarPorProtocoloEGrupoPessoas(int protocolo, int codigoGrupoPessoas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador>();

            query = query.Where(o => o.Protocolo == protocolo && o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador BuscarPorProtocoloCancelamentoEGrupoPessoas(int protocolo, int codigoGrupoPessoas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador>();

            query = query.Where(o => o.ProtocoloCancelamento == protocolo && o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.FirstOrDefault();
        }

        public List<long> BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador situacao, int quantidadeRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador>();

            query = query.Where(o => o.Situacao == situacao);

            return query.OrderBy(o => o.Codigo).Select(o => o.Codigo).Take(quantidadeRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador.FiltroPesquisaOcorrenciaIntegracaoEmbarcador filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador> query = ObterQueryConsulta(filtro, parametroConsulta);

            return query.OrderBy(o => o.Codigo).Skip(parametroConsulta.InicioRegistros).Take(parametroConsulta.LimiteRegistros).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador.FiltroPesquisaOcorrenciaIntegracaoEmbarcador filtro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador> query = ObterQueryConsulta(filtro);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador BuscarPorProtocolo(int protocolo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador>();

            query = query.Where(o => o.Protocolo == protocolo);

            return query.FirstOrDefault();
        }


        #endregion

        #region Metodos Privados
        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador> ObterQueryConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador.FiltroPesquisaOcorrenciaIntegracaoEmbarcador filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador>();


            if (filtro.CodigoEmpresa != null && filtro.CodigoEmpresa.Count() > 0)
                query = query.Where(o => filtro.CodigoEmpresa.Contains(o.Empresa.Codigo));

            if (filtro.Situacao != null && filtro.Situacao.Count() > 0)
                query = query.Where(o => filtro.Situacao.Contains(o.Situacao));

            if (!string.IsNullOrWhiteSpace(filtro.NumeroOcorrencia))
                query = query.Where(o => o.CargaOcorrencia.NumeroOcorrencia == int.Parse(filtro.NumeroOcorrencia));

            if (!string.IsNullOrWhiteSpace(filtro.NumeroOcorrenciaEmbarcador))
                query = query.Where(o => o.NumeroOcorrenciaEmbarcador == int.Parse(filtro.NumeroOcorrenciaEmbarcador));

            if (filtro.DataInicialOcorrencia.HasValue)
                query = query.Where(o => o.DataOcorrencia >= filtro.DataInicialOcorrencia.Value);

            if (filtro.DataFinalOcorrencia.HasValue)
                query = query.Where(o => o.DataOcorrencia < filtro.DataInicialOcorrencia.Value.AddDays(1));

            if (filtro.CodigoGrupoPessoa != null && filtro.CodigoGrupoPessoa.Count() > 0)
                query = query.Where(o => filtro.CodigoGrupoPessoa.Contains(o.GrupoPessoas.Codigo));

            return query;
        }
        #endregion

    }
}
