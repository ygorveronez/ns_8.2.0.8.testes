using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Compras
{
    public class OrdemCompra : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioOrdemCompra, Dominio.Relatorios.Embarcador.DataSource.Compras.RelatorioOrdemCompra>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Compras.OrdemCompra _repositorioOrdemCompra;

        #endregion

        #region Construtores

        public OrdemCompra(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Compras.RelatorioOrdemCompra> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioOrdemCompra filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioOrdemCompra.ConsultarRelatorioOrdemCompra(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioOrdemCompra filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioOrdemCompra.ContarConsultaRelatorioOrdemCompra(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Compras/OrdemCompra";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioOrdemCompra filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Dominio.Relatorios.Embarcador.DataSource.Compras.RelatorioOrdemCompra aux = new Dominio.Relatorios.Embarcador.DataSource.Compras.RelatorioOrdemCompra();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataGeracao", filtrosPesquisa.DataGeracaoInicio, filtrosPesquisa.DataGeracaoFim));


            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPrevisao", filtrosPesquisa.DataPrevisaoInicio, filtrosPesquisa.DataPrevisaoFim));


            if (filtrosPesquisa.NumeroInicial > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroInicial", filtrosPesquisa.NumeroInicial.ToString(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroInicial", false));

            if (filtrosPesquisa.NumeroFinal > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroFinal", filtrosPesquisa.NumeroFinal.ToString(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroFinal", false));

            if (filtrosPesquisa.Produto > 0)
            {
                Dominio.Entidades.Produto _produto = repProduto.BuscarPorCodigo(filtrosPesquisa.Produto);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", _produto.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", false));

            if (filtrosPesquisa.Operador > 0)
            {
                Dominio.Entidades.Usuario _operador = repUsuario.BuscarPorCodigo(filtrosPesquisa.Operador);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Operador", _operador.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Operador", false));

            if (filtrosPesquisa.Fornecedor > 0)
            {
                Dominio.Entidades.Cliente _fornecedor = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Fornecedor);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fornecedor", _fornecedor.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fornecedor", false));

            if (filtrosPesquisa.Transportador > 0)
            {
                Dominio.Entidades.Cliente _transportador = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Transportador);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", _transportador.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

            if (filtrosPesquisa.Situacao > 0)
            {
                aux.Situacao = filtrosPesquisa.Situacao;
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", aux.SituacaoDescricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", false));


            if (filtrosPesquisa.Veiculo > 0)
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(filtrosPesquisa.Veiculo);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo.Placa, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "SituacaoDescricao")
                return "Situacao";

            if (propriedadeOrdenarOuAgrupar == "DataGeracaoInicioFormatada")
                return "DataGeracaoInicio";
            if (propriedadeOrdenarOuAgrupar == "DataGeracaoFimFormatada")
                return "DataGeracaoFim";

            if (propriedadeOrdenarOuAgrupar == "DataPrevisaoInicioFormatada")
                return "DataInicioPrevisao";
            if (propriedadeOrdenarOuAgrupar == "DataPrevisaoFimFormatada")
                return "DataFimPrevisao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}