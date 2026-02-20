namespace Servicos.Embarcador.Frete
{
    public class ComponenteFrete : ServicoBase
    {
        public ComponenteFrete(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional ConverterObjetoComponentePedido(Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete)
        {
            if (cargaPedidoComponenteFrete != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteAdicional = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();

                componenteAdicional.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                componenteAdicional.Componente.TipoComponenteFrete = cargaPedidoComponenteFrete.TipoComponenteFrete;

                if (cargaPedidoComponenteFrete.ComponenteFrete != null)
                {
                    componenteAdicional.Componente.Descricao = cargaPedidoComponenteFrete.ComponenteFrete.Descricao;
                    componenteAdicional.Componente.CodigoIntegracao = cargaPedidoComponenteFrete.ComponenteFrete.CodigoIntegracao;
                }
                else
                    componenteAdicional.Componente.Descricao = cargaPedidoComponenteFrete.DescricaoComponente;

                componenteAdicional.ValorComponente = cargaPedidoComponenteFrete.ValorComponente;
                componenteAdicional.IncluirBaseCalculoICMS = cargaPedidoComponenteFrete.IncluirBaseCalculoICMS;

                return componenteAdicional;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional ConverterObjetoComponenteCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCteComponenteFrete)
        {
            if (cargaCteComponenteFrete != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteAdicional = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();

                componenteAdicional.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                componenteAdicional.Componente.TipoComponenteFrete = cargaCteComponenteFrete.TipoComponenteFrete;

                if (cargaCteComponenteFrete.ComponenteFrete != null)
                {
                    componenteAdicional.Componente.Descricao = cargaCteComponenteFrete.ComponenteFrete.Descricao;
                    componenteAdicional.Componente.CodigoIntegracao = cargaCteComponenteFrete.ComponenteFrete.CodigoIntegracao;
                }
                else
                    componenteAdicional.Componente.Descricao = cargaCteComponenteFrete.DescricaoComponente;

                componenteAdicional.ValorComponente = cargaCteComponenteFrete.ValorComponente;
                componenteAdicional.IncluirBaseCalculoICMS = cargaCteComponenteFrete.IncluirBaseCalculoICMS;

                return componenteAdicional;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional ConverterObjetoComponentePrestacaoCTe(Dominio.Entidades.ComponentePrestacaoCTE componentePrestacaoCTE)
        {
            if (componentePrestacaoCTE == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteAdicional = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();

            componenteAdicional.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();

            if (componentePrestacaoCTE.ComponenteFrete != null)
            {

                componenteAdicional.Componente.TipoComponenteFrete = componentePrestacaoCTE.ComponenteFrete.TipoComponenteFrete;
                componenteAdicional.Componente.Descricao = componentePrestacaoCTE.ComponenteFrete.Descricao;
                componenteAdicional.Componente.CodigoIntegracao = componentePrestacaoCTE.ComponenteFrete.CodigoIntegracao;
            }
            else
            {
                componenteAdicional.Componente.Descricao = componentePrestacaoCTE.NomeCTe ?? string.Empty;
            }

            componenteAdicional.ValorComponente = componentePrestacaoCTE.Valor;
            componenteAdicional.IncluirBaseCalculoICMS = componentePrestacaoCTE.IncluiNaBaseDeCalculoDoICMS;

            return componenteAdicional;
        }
    }
}
