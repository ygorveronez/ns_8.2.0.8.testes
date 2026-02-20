using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Integracao
{
    public class IntegracaoEMPLogRecebimento : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLogRecebimento>
    {
        public IntegracaoEMPLogRecebimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLogRecebimento BuscarPorCodigo(int codigo)
        {
            var arquivoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLogRecebimento>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return arquivoIntegracao;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLogRecebimento> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAuditoriaEMP filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = MontarConsulta(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAuditoriaEMP filtrosPesquisa)
        {
            var result = MontarConsulta(filtrosPesquisa);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLogRecebimento> MontarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAuditoriaEMP filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLogRecebimento>();

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataRecebimento.Value.Date >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataRecebimento.Value.Date <= filtrosPesquisa.DataFinal.Value.Date);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Topic))
                query = query.Where(o => o.Topic.Contains(filtrosPesquisa.Topic));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Booking))
                query = query.Where(o => o.NumeroBooking.Contains(filtrosPesquisa.Booking));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Customer))
                query = query.Where(o => o.CustomerCode.Contains(filtrosPesquisa.Customer));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Schedule))
                query = query.Where(o => o.ScheduleViagemNavio.Contains(filtrosPesquisa.Schedule));

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
