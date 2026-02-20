using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Relatorios.Veiculos
{
    public class Equipamento : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioEquipamento, Dominio.Relatorios.Embarcador.DataSource.Veiculos.Equipamento>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Veiculos.Equipamento _repositorioVeiculos;

        #endregion

        #region Construtores

        public Equipamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioVeiculos = new Repositorio.Embarcador.Veiculos.Equipamento(_unitOfWork);
        }

        public Equipamento(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioVeiculos = new Repositorio.Embarcador.Veiculos.Equipamento(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Equipamento>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioEquipamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioVeiculos.ConsultarRelatorioEquipamentoAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList meotodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Equipamento> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioEquipamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioVeiculos.ConsultarRelatorioEquipamento(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioEquipamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioVeiculos.ContarConsultaRelatorioEquipamento(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Veiculos/Equipamento";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioEquipamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Veiculos.MarcaEquipamento repMarcaEquipamento = new Repositorio.Embarcador.Veiculos.MarcaEquipamento(_unitOfWork);
            Repositorio.Embarcador.Veiculos.ModeloEquipamento repModeloEquipamento = new Repositorio.Embarcador.Veiculos.ModeloEquipamento(_unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            Dominio.Entidades.Embarcador.Veiculos.MarcaEquipamento marcaEquipamento = filtrosPesquisa.CodigoMarca > 0 ? repMarcaEquipamento.BuscarPorCodigo(filtrosPesquisa.CodigoMarca) : null;
            Dominio.Entidades.Embarcador.Veiculos.ModeloEquipamento modeloEquipamento = filtrosPesquisa.CodigoModelo > 0 ? repModeloEquipamento.BuscarPorCodigo(filtrosPesquisa.CodigoModelo) : null;
            List<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo> segmentos = filtrosPesquisa.CodigosSegmento.Count > 0 ? repSegmentoVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigosSegmento.ToArray()) : new List<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo>();
            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = filtrosPesquisa.CodigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(filtrosPesquisa.CodigoCentroResultado) : null;
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAquisicao", filtrosPesquisa.DataAquisicaoInicial + " " + filtrosPesquisa.DataAquisicaoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Marca", marcaEquipamento?.Descricao ?? ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Modelo", modeloEquipamento?.Descricao ?? ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Segmento", string.Join(", ", from obj in segmentos select obj.Descricao)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AnoFabricacao", filtrosPesquisa.AnoFabricacao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Ativo", filtrosPesquisa.Ativo.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar ?? ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo?.Descricao ?? ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultado", centroResultado?.Descricao ?? ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Neokohm", filtrosPesquisa.Neokohm != 9 ? filtrosPesquisa.Neokohm == 1 ? "Sim" : "Não" : string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataAquisicaoFormatada")
                return "DataAquisicao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}