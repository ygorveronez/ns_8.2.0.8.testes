using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoModeloEmail : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail>
    {
        public ConfiguracaoModeloEmail(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail> BuscarPorTipoModeloEmail(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloEmail tipoModeloEmail)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail> consultaModelosEmail = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail>();
            consultaModelosEmail = consultaModelosEmail.Where(modelo => modelo.TipoModeloEmail == tipoModeloEmail && modelo.Ativo);
            return consultaModelosEmail.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail> BuscarPorTipoGatilhoNotificacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGatilhoNotificacao tipoGatilhoNotificacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail> consultaModelosEmail = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail>();
            consultaModelosEmail = consultaModelosEmail.Where(modelo => modelo.TipoGatilhoNotificacao == tipoGatilhoNotificacao && modelo.Ativo);
            return consultaModelosEmail.DistinctBy(o => o.TipoEnviarPara).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail> Consultar(Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoModeloEmail filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail> result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoModeloEmail filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail> result = Consultar(filtrosPesquisa);

            return result.Count();
        }
        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail> Consultar(Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoModeloEmail filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail> consultaModelosEmail = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail>();

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                consultaModelosEmail = consultaModelosEmail.Where(modelo => modelo.Descricao == filtrosPesquisa.Descricao);

            if (filtrosPesquisa.Ativo != null)
                consultaModelosEmail = consultaModelosEmail.Where(modelo => modelo.Ativo == filtrosPesquisa.Ativo);

            if (filtrosPesquisa.Tipo != null)
                consultaModelosEmail = consultaModelosEmail.Where(modelo => modelo.TipoModeloEmail == filtrosPesquisa.Tipo);

            return from obj in consultaModelosEmail select obj;
        }

        #endregion
    }
}
