using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Frota;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Frota
{
    public class PneuCustoEstoque : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneuCustoEstoque, Dominio.Relatorios.Embarcador.DataSource.Frota.PneuCustoEstoque>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Frota.Pneu _repositorioPneuCustoEstoque;

        #endregion

        #region Construtores

        public PneuCustoEstoque(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPneuCustoEstoque = new Repositorio.Embarcador.Frota.Pneu(_unitOfWork);
        }

        public PneuCustoEstoque(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, 
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPneuCustoEstoque = new Repositorio.Embarcador.Frota.Pneu(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuCustoEstoque>> ConsultarRegistrosAsync(FiltroPesquisaRelatorioPneuCustoEstoque filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return await _repositorioPneuCustoEstoque.ConsultarRelatorioPneuCustoEstoqueAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion


        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuCustoEstoque> ConsultarRegistros(FiltroPesquisaRelatorioPneuCustoEstoque filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioPneuCustoEstoque.ConsultarRelatorioPneuCustoEstoque(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioPneuCustoEstoque filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioPneuCustoEstoque.ContarConsultaRelatorioPneuCustoEstoque(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frota/PneuCustoEstoque";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioPneuCustoEstoque filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Frota.Almoxarifado repAlmoxarifado = new Repositorio.Embarcador.Frota.Almoxarifado(_unitOfWork);
            Repositorio.Embarcador.Frota.BandaRodagemPneu repBandaRodagem = new Repositorio.Embarcador.Frota.BandaRodagemPneu(_unitOfWork);
            Repositorio.Embarcador.Frota.DimensaoPneu repDimensao = new Repositorio.Embarcador.Frota.DimensaoPneu(_unitOfWork);
            Repositorio.Embarcador.Frota.MarcaPneu repMarca = new Repositorio.Embarcador.Frota.MarcaPneu(_unitOfWork);
            Repositorio.Embarcador.Frota.ModeloPneu repModelo = new Repositorio.Embarcador.Frota.ModeloPneu(_unitOfWork);
            Repositorio.Embarcador.Frota.Pneu repPneu = new Repositorio.Embarcador.Frota.Pneu(_unitOfWork);
            Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiuloFrota = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            Dominio.Entidades.Embarcador.Frota.Almoxarifado almoxarifado = filtrosPesquisa.CodigoAlmoxarifado > 0 ? repAlmoxarifado.BuscarPorCodigo(filtrosPesquisa.CodigoAlmoxarifado) : null;
            Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu bandaRodagemPneu = filtrosPesquisa.CodigoBandaRodagem > 0 ? repBandaRodagem.BuscarPorCodigo(filtrosPesquisa.CodigoBandaRodagem) : null;
            Dominio.Entidades.Embarcador.Frota.DimensaoPneu dimensaoPneu = filtrosPesquisa.CodigoDimensao > 0 ? repDimensao.BuscarPorCodigo(filtrosPesquisa.CodigoDimensao) : null;
            Dominio.Entidades.Embarcador.Frota.MarcaPneu marcaPneu = filtrosPesquisa.CodigoMarca > 0 ? repMarca.BuscarPorCodigo(filtrosPesquisa.CodigoMarca) : null;
            Dominio.Entidades.Embarcador.Frota.ModeloPneu modeloPneu = filtrosPesquisa.CodigoModelo > 0 ? repModelo.BuscarPorCodigo(filtrosPesquisa.CodigoModelo) : null;
            Dominio.Entidades.Embarcador.Frota.Pneu pneu = filtrosPesquisa.CodigoPneu > 0 ? repPneu.BuscarPorCodigo(filtrosPesquisa.CodigoPneu) : null;
            Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servicoVeiculoFrota = filtrosPesquisa.CodigoServicoVeiculoFrota > 0 ? repServicoVeiuloFrota.BuscarPorCodigo(filtrosPesquisa.CodigoServicoVeiculoFrota) : null;
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;

            parametros.Add(new Parametro("Vida", filtrosPesquisa.Vida?.ObterDescricao()));
            parametros.Add(new Parametro("Almoxarifado", almoxarifado?.Descricao));
            parametros.Add(new Parametro("DataAquisicao", filtrosPesquisa.DataAquisicaoInicial, filtrosPesquisa.DataAquisicaoFinal));
            parametros.Add(new Parametro("BandaRodagem", bandaRodagemPneu?.Descricao));
            parametros.Add(new Parametro("Dimensao", dimensaoPneu?.Descricao));
            parametros.Add(new Parametro("Marca", marcaPneu?.Descricao));
            parametros.Add(new Parametro("Modelo", modeloPneu?.Descricao));
            parametros.Add(new Parametro("Pneu", pneu?.Descricao));
            parametros.Add(new Parametro("ServicoVeiculoFrota", servicoVeiculoFrota?.Descricao));
            parametros.Add(new Parametro("Veiculo", veiculo?.Descricao));
            parametros.Add(new Parametro("Situacao", string.Join(", ", filtrosPesquisa.Situacao.Select(o => o.ObterDescricao()))));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "EstadoAtualPneuDescricao")
                return propriedadeOrdenarOuAgrupar.Replace("Descricao", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}
