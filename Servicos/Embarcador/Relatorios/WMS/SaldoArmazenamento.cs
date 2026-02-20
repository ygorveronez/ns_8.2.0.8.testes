using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.WMS
{
    public class SaldoArmazenamento : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioSaldoArmazenamento, Dominio.Relatorios.Embarcador.DataSource.WMS.SaldoArmazenamento>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.WMS.RecebimentoMercadoria _repositorioSaldoArmazenamento;

        #endregion

        #region Construtores

        public SaldoArmazenamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioSaldoArmazenamento = new Repositorio.Embarcador.WMS.RecebimentoMercadoria(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.WMS.SaldoArmazenamento> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioSaldoArmazenamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioSaldoArmazenamento.RelatorioSaldoArmazenamento(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioSaldoArmazenamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioSaldoArmazenamento.ContarRelatorioSaldoArmazenamento(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/WMS/SaldoArmazenamento";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioSaldoArmazenamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.WMS.Deposito repDeposito = new Repositorio.Embarcador.WMS.Deposito(_unitOfWork);
            Repositorio.Embarcador.WMS.DepositoBloco repDepositoBloco = new Repositorio.Embarcador.WMS.DepositoBloco(_unitOfWork);
            Repositorio.Embarcador.WMS.DepositoPosicao repDepositoPosicao = new Repositorio.Embarcador.WMS.DepositoPosicao(_unitOfWork);
            Repositorio.Embarcador.WMS.DepositoRua repDepositoRua = new Repositorio.Embarcador.WMS.DepositoRua(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);

            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = filtrosPesquisa.CodigoProdutoEmbarcador > 0 ? repProdutoEmbarcador.BuscarPorCodigo(filtrosPesquisa.CodigoProdutoEmbarcador) : null;
            Dominio.Entidades.Embarcador.WMS.Deposito deposito = filtrosPesquisa.CodigoDeposito > 0 ? repDeposito.BuscarPorCodigo(filtrosPesquisa.CodigoDeposito) : null;
            Dominio.Entidades.Embarcador.WMS.DepositoBloco depositoBloco = filtrosPesquisa.CodigoBloco > 0 ? repDepositoBloco.BuscarPorCodigo(filtrosPesquisa.CodigoBloco) : null;
            Dominio.Entidades.Embarcador.WMS.DepositoPosicao depositoPosicao = filtrosPesquisa.CodigoPosicao > 0 ? repDepositoPosicao.BuscarPorCodigo(filtrosPesquisa.CodigoPosicao) : null;
            Dominio.Entidades.Embarcador.WMS.DepositoRua depositoRua = filtrosPesquisa.CodigoRua > 0 ? repDepositoRua.BuscarPorCodigo(filtrosPesquisa.CodigoRua) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoRecebimento", filtrosPesquisa.TipoRecebimento?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ProdutoEmbarcador", produtoEmbarcador?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Deposito", deposito?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Bloco", depositoBloco?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Posicao", depositoPosicao?.Abreviacao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Rua", depositoRua?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoInicial", filtrosPesquisa.DataVencimentoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoFinal", filtrosPesquisa.DataVencimentoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CodigoBarras", filtrosPesquisa.CodigoBarras));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Descricao", filtrosPesquisa.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta.PropriedadeAgrupar));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
