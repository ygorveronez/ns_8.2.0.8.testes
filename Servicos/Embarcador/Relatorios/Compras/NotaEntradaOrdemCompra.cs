using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Compras
{
    public class NotaEntradaOrdemCompra : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioNotaEntradaOrdemCompra, Dominio.Relatorios.Embarcador.DataSource.Compras.NotaEntradaOrdemCompra>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Compras.OrdemCompra _repositorioNotaEntradaOrdemCompra;

        #endregion

        #region Construtores

        public NotaEntradaOrdemCompra(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioNotaEntradaOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Compras.NotaEntradaOrdemCompra> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioNotaEntradaOrdemCompra filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioNotaEntradaOrdemCompra.ConsultarRelatorioNotaEntradaOrdemCompra(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioNotaEntradaOrdemCompra filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioNotaEntradaOrdemCompra.ContarConsultaRelatorioNotaEntradaOrdemCompra(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Compras/NotaEntradaOrdemCompra";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioNotaEntradaOrdemCompra filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(_unitOfWork);
            Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(_unitOfWork);

            if (filtrosPesquisa.DataEntrada != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEntrada", filtrosPesquisa.DataEntrada.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEntrada", false));

            if (filtrosPesquisa.Nota > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Nota", filtrosPesquisa.Nota.ToString(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Nota", false));

            if (filtrosPesquisa.CodigoOrdem > 0)
            {
                Dominio.Entidades.Embarcador.Compras.OrdemCompra _ordem = repOrdemCompra.BuscarPorCodigo(filtrosPesquisa.CodigoOrdem);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Ordem", _ordem.Numero.ToString(), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Ordem", false));

            if (filtrosPesquisa.Fornecedor > 0)
            {
                Dominio.Entidades.Cliente _fornecedor = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Fornecedor);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fornecedor", _fornecedor.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fornecedor", false));

            if (filtrosPesquisa.CodigoProduto > 0)
            {
                Dominio.Entidades.Produto _produto = repProduto.BuscarPorCodigo(filtrosPesquisa.CodigoProduto);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", _produto.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", false));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataEntradaFormatada")
                return "DataEntrada";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}