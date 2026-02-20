namespace Servicos.WebServiceCarrefour.Conversores.Frete
{
    public sealed class ComponenteFreteConversor
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public ComponenteFreteConversor(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region MMétodos Privados

        private Dominio.ObjetosDeValor.WebServiceCarrefour.Frete.Componente ObterComponente(Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCteComponenteFrete)
        {
            Dominio.ObjetosDeValor.WebServiceCarrefour.Frete.Componente componente = new Dominio.ObjetosDeValor.WebServiceCarrefour.Frete.Componente();

            componente.TipoComponenteFrete = cargaCteComponenteFrete.TipoComponenteFrete;

            if (cargaCteComponenteFrete.ComponenteFrete != null)
            {
                componente.Descricao = cargaCteComponenteFrete.ComponenteFrete.Descricao;
                componente.CodigoIntegracao = cargaCteComponenteFrete.ComponenteFrete.CodigoIntegracao;
            }
            else
                componente.Descricao = cargaCteComponenteFrete.DescricaoComponente;

            return componente;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Frete.ComponenteAdicional Converter(Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCteComponenteFrete)
        {
            if (cargaCteComponenteFrete == null)
                return null;

            Dominio.ObjetosDeValor.WebServiceCarrefour.Frete.ComponenteAdicional componenteAdicional = new Dominio.ObjetosDeValor.WebServiceCarrefour.Frete.ComponenteAdicional();

            componenteAdicional.Componente = ObterComponente(cargaCteComponenteFrete);
            componenteAdicional.ValorComponente = cargaCteComponenteFrete.ValorComponente;
            componenteAdicional.IncluirBaseCalculoICMS = cargaCteComponenteFrete.IncluirBaseCalculoICMS;            

            return componenteAdicional;
        }

        #endregion
    }
}
