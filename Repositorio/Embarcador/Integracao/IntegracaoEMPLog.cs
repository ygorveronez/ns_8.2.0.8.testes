using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Integracao
{
    public class IntegracaoEMPLog : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog>
    {
        public IntegracaoEMPLog(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog BuscarPorCodigo(int codigo)
        {
            var arquivoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return arquivoIntegracao;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAuditoriaEMP filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = MontarConsulta(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAuditoriaEMP filtrosPesquisa)
        {
            var result = MontarConsulta(filtrosPesquisa);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog> MontarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAuditoriaEMP filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog>();

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEnvio.Value.Date >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEnvio.Value.Date <= filtrosPesquisa.DataFinal.Value.Date);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Topic))
                query = query.Where(o => o.Topic.Contains(filtrosPesquisa.Topic));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Booking))
                query = query.Where(o => o.NumeroBooking.Contains(filtrosPesquisa.Booking));

            if (filtrosPesquisa.Justificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim)
                query = query.Where(o => o.Justificativa != string.Empty && o.Justificativa != null);

            if (filtrosPesquisa.Justificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Nao)
                query = query.Where(o => o.Justificativa == string.Empty || o.Justificativa == null);

            if (filtrosPesquisa.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoEMP.NaoInformado)
                query = query.Where(o => o.SituacaoIntegracao == filtrosPesquisa.Status);

            if (filtrosPesquisa.TipoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoEMP.NaoInformado)
                query = query.Where(o => o.TipoIntegracao == filtrosPesquisa.TipoIntegracao);

            return query;
        }
    }
}