using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Chamado
{
    public sealed class ConfiguracaoTempoChamado
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public ConfiguracaoTempoChamado(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado ObterConfiguracaoPorCriterios(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Embarcador.Filiais.Filial filial)
        {
            return ObterConfiguracaoPorCriterios(cliente?.CPF_CNPJ ?? 0d, tipoOperacao?.Codigo ?? 0, filial?.Codigo ?? 0);
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado ObterConfiguracaoPorCriterios(double cnpjCpfCliente, int codigoTipoOperacao, int codigoFilial)
        {
            Repositorio.Embarcador.Chamados.ConfiguracaoTempoChamado repConfiguracaoTempoChamado = new Repositorio.Embarcador.Chamados.ConfiguracaoTempoChamado(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado> configuracoesFiltradas = new List<Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado>();
            List<Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado> configuracoes = repConfiguracaoTempoChamado.BuscarPorClienteTipoOperacaoFilial(cnpjCpfCliente, codigoTipoOperacao, codigoFilial);

            // 1 - Com Cliente, Tipo Operação e Filial
            configuracoesFiltradas = (from o in configuracoes where o.Cliente != null && o.TipoOperacao != null && o.Filial != null select o).ToList();
            if (configuracoesFiltradas.Count() > 0)
                return configuracoesFiltradas.FirstOrDefault();

            // 2 - Com Cliente e Tipo Operação
            configuracoesFiltradas = (from o in configuracoes where o.Cliente != null && o.TipoOperacao != null select o).ToList();
            if (configuracoesFiltradas.Count() > 0)
                return configuracoesFiltradas.FirstOrDefault();

            // 3 - Com Cliente e Filial
            configuracoesFiltradas = (from o in configuracoes where o.Cliente != null && o.Filial != null select o).ToList();
            if (configuracoesFiltradas.Count() > 0)
                return configuracoesFiltradas.FirstOrDefault();

            // 4 - Com Cliente
            configuracoesFiltradas = (from o in configuracoes where o.Cliente != null select o).ToList();
            if (configuracoesFiltradas.Count() > 0)
                return configuracoesFiltradas.FirstOrDefault();

            // 5 - Com Tipo Operação
            configuracoesFiltradas = (from o in configuracoes where o.TipoOperacao != null select o).ToList();
            if (configuracoesFiltradas.Count() > 0)
                return configuracoesFiltradas.FirstOrDefault();

            // 6 - Com Filial
            configuracoesFiltradas = (from o in configuracoes where o.Filial != null select o).ToList();
            if (configuracoesFiltradas.Count() > 0)
                return configuracoesFiltradas.FirstOrDefault();

            return configuracoes.FirstOrDefault();
        }

        #endregion
    }
}
