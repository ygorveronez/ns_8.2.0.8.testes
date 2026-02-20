using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Dominio.Relatorios.Embarcador.DataSource.Logistica;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Repositorio;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Logistica
{
    public class FilaCarregamentoHistorico : RelatorioBase<FiltroPesquisaFilaCarregamentoVeiculoHistorico, FilaCarregamentoVeiculoHistorico>
    {
        #region Propriedades Privadas

        private readonly Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico repositorioFilaCarregamentoVeiculoHistorico;

        #endregion Propriedades Privadas

        #region Construtores

        public FilaCarregamentoHistorico(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            repositorioFilaCarregamentoVeiculoHistorico = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico(_unitOfWork);
        }

        #endregion Construtores

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<FilaCarregamentoVeiculoHistorico> ConsultarRegistros(FiltroPesquisaFilaCarregamentoVeiculoHistorico filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return repositorioFilaCarregamentoVeiculoHistorico.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaFilaCarregamentoVeiculoHistorico filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return repositorioFilaCarregamentoVeiculoHistorico.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Logistica/FilaCarregamentoHistorico";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaFilaCarregamentoVeiculoHistorico filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.GrupoModeloVeicular repositorioGrupoModeloVeicular = new Repositorio.Embarcador.Cargas.GrupoModeloVeicular(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Embarcador.Logistica.MotivoRetiradaFilaCarregamento repositorioMotivoRetiradaFilaCarregamento = new Repositorio.Embarcador.Logistica.MotivoRetiradaFilaCarregamento(_unitOfWork);
            Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = filtrosPesquisa.CodigoCentroCarregamento > 0 ? repositorioCentroCarregamento.BuscarPorCodigo(filtrosPesquisa.CodigoCentroCarregamento) : null;
            Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular grupoModeloVeicular = filtrosPesquisa.CodigoGrupoModeloVeicularCarga > 0 ? repositorioGrupoModeloVeicular.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoModeloVeicularCarga) : null;
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = filtrosPesquisa.CodigoModeloVeicularCarga > 0 ? repositorioModeloVeicularCarga.BuscarPorCodigo(filtrosPesquisa.CodigoModeloVeicularCarga) : null;
            Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento motivo = filtrosPesquisa.CodigoMotivoRetivadaFilaCarregamento > 0 ? repositorioMotivoRetiradaFilaCarregamento.BuscarPorCodigo(filtrosPesquisa.CodigoMotivoRetivadaFilaCarregamento) : null;
            Dominio.Entidades.Usuario motorista = filtrosPesquisa.CodigoMotorista > 0 ? repositorioMotorista.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista) : null;
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repositorioVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;

            List<Parametro> parametros = new List<Parametro>
            {
                new Parametro("Periodo", filtrosPesquisa.DataInicio, filtrosPesquisa.DataLimite),
                new Parametro("CentroCarregamento", centroCarregamento?.Descricao),
                new Parametro("GrupoModeloVeicularCarga", grupoModeloVeicular?.Descricao),
                new Parametro("ModeloVeicularCarga", modeloVeicularCarga?.Descricao),
                new Parametro("MotivoRetiradaFilaCarregamento", motivo?.Descricao),
                new Parametro("Motorista", motorista?.Nome),
                new Parametro("Veiculo", veiculo?.Placa),
                new Parametro("Tipo", filtrosPesquisa.Tipo.HasValue ? filtrosPesquisa.Tipo.Value.ObterDescricao() : null)
            };

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion Métodos Protegidos Sobrescritos
    }
}
