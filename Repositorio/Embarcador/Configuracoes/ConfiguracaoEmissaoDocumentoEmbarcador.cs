using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoEmissaoDocumentoEmbarcador : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador>
    {
        #region Construtores

        public ConfiguracaoEmissaoDocumentoEmbarcador(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public ConfiguracaoEmissaoDocumentoEmbarcador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador> Consultar(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltrosPesquisaConfiguracaoEmissaoDocumentoEmbarcador filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltrosPesquisaConfiguracaoEmissaoDocumentoEmbarcador filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public bool RegistroDuplicado(double cfpcnpjCliente, int codigoTipoOperacao, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador>();
            var result = from obj in query where obj.Codigo != codigo && obj.Cliente.CPF_CNPJ == cfpcnpjCliente && obj.TipoOperacao.Codigo == codigoTipoOperacao select obj;
            return result.Count() > 0;
        }

        public bool ExistePorClienteETipoOperacao(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador> CacheConfiguracaoEmissaoDocumentoEmbarcador)
        {
            if (CacheConfiguracaoEmissaoDocumentoEmbarcador != null)
            {
                return CacheConfiguracaoEmissaoDocumentoEmbarcador.Where(o => o.Cliente == cliente && o.TipoOperacao == tipoOperacao).Select(o => o.Codigo).Any();
            }
            else
            {
                IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador>();
                query = query.Where(o => o.Cliente == cliente && o.TipoOperacao == tipoOperacao);
                return query.Select(o => o.Codigo).Any();
            }
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador> BuscaConfiguracaoEmissaoDocumentoEmbarcadorPorClientesDaCarga(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador> configuracoesEmissaoDocumentoEmbarcador = new List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador>();
            List<Dominio.Entidades.Cliente> tomadores = cargaPedidos.Select(cargaPedido => cargaPedido.ObterTomador()).Where(tomador => tomador != null).ToList();

            if (tomadores == null && tomadores.Count() == 0)
                return null;

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = tomadores.Count / quantidadeRegistrosConsultarPorVez;

            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                configuracoesEmissaoDocumentoEmbarcador.AddRange(query.Where(o => tomadores.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.Cliente)).ToList());

            return configuracoesEmissaoDocumentoEmbarcador;
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador> Consultar(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltrosPesquisaConfiguracaoEmissaoDocumentoEmbarcador filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.CPFCNPJCliente > 0)
                result = result.Where(obj => obj.Cliente.CPF_CNPJ == filtrosPesquisa.CPFCNPJCliente);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            return result;
        }

        #endregion
    }
}