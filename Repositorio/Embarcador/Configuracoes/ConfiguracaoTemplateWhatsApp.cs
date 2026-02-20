using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoTemplateWhatsApp : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp>
    {
        public ConfiguracaoTemplateWhatsApp(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp Buscar()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp>();

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp BuscarPorNomeTemplate(string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp>()
                .Where(p => p.Nome == nome);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp> BuscarPorTipoTemplate(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTemplateWhatsApp tipoTemplate)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp>()
                .Where(p => p.TipoTemplate == tipoTemplate);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp> BuscarTemplatesPendentesAprovacao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp>()
                .Where(p => p.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTemplateWhatsApp.AguardandoAprovacao);

            return query.ToList();
        }
        #endregion
    }
}
