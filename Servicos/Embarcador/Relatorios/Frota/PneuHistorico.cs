using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Relatorios.Frota
{
    public class PneuHistorico : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico, Dominio.Relatorios.Embarcador.DataSource.Frota.PneuHistorico>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Frota.PneuHistorico _repositorioPneuHistorico;

        #endregion

        #region Construtores

        public PneuHistorico(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPneuHistorico = new Repositorio.Embarcador.Frota.PneuHistorico(_unitOfWork);
        }

        public PneuHistorico(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, 
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPneuHistorico = new Repositorio.Embarcador.Frota.PneuHistorico(_unitOfWork, cancellationToken);
        }

        #endregion
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuHistorico>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioPneuHistorico.ConsultarRelatorioAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #region

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuHistorico> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioPneuHistorico.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioPneuHistorico.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frota/PneuHistorico";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);

            Dominio.Entidades.Usuario usuarioOperador = filtrosPesquisa.CodigoUsuarioOperador > 0 ? repositorioUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoUsuarioOperador) : null;

            if (filtrosPesquisa.DataInicio.HasValue || filtrosPesquisa.DataLimite.HasValue)
            {
                string periodo = $"{(filtrosPesquisa.DataInicio.HasValue ? $"{filtrosPesquisa.DataInicio.Value.ToString("dd/MM/yyyy")} " : "")}{(filtrosPesquisa.DataLimite.HasValue ? $"até {filtrosPesquisa.DataLimite.Value.ToString("dd/MM/yyyy")}" : "")}";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Periodo", periodo, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Periodo", false));

            if (filtrosPesquisa.CodigoBandaRodagem > 0)
            {
                Repositorio.Embarcador.Frota.BandaRodagemPneu repositorioBandaRodagem = new Repositorio.Embarcador.Frota.BandaRodagemPneu(_unitOfWork);
                Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu BandaRodagem = repositorioBandaRodagem.BuscarPorCodigo(filtrosPesquisa.CodigoBandaRodagem);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("BandaRodagem", BandaRodagem.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("BandaRodagem", false));

            if (filtrosPesquisa.CodigoDimensao > 0)
            {
                Repositorio.Embarcador.Frota.DimensaoPneu repositorioDimensao = new Repositorio.Embarcador.Frota.DimensaoPneu(_unitOfWork);
                Dominio.Entidades.Embarcador.Frota.DimensaoPneu dimensao = repositorioDimensao.BuscarPorCodigo(filtrosPesquisa.CodigoDimensao);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Dimensao", dimensao.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Dimensao", false));

            if (filtrosPesquisa.CodigoMarca > 0)
            {
                Repositorio.Embarcador.Frota.MarcaPneu repositorioMarca = new Repositorio.Embarcador.Frota.MarcaPneu(_unitOfWork);
                Dominio.Entidades.Embarcador.Frota.MarcaPneu marca = repositorioMarca.BuscarPorCodigo(filtrosPesquisa.CodigoMarca);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Marca", marca.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Marca", false));

            if (filtrosPesquisa.CodigoModelo > 0)
            {
                Repositorio.Embarcador.Frota.ModeloPneu repositorioModeloPneu = new Repositorio.Embarcador.Frota.ModeloPneu(_unitOfWork);
                Dominio.Entidades.Embarcador.Frota.ModeloPneu modeloPneu = repositorioModeloPneu.BuscarPorCodigo(filtrosPesquisa.CodigoModelo);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Modelo", modeloPneu.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Modelo", false));

            if (filtrosPesquisa.CodigoPneu > 0)
            {
                Repositorio.Embarcador.Frota.Pneu repositorioPneu = new Repositorio.Embarcador.Frota.Pneu(_unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Pneu pneu = repositorioPneu.BuscarPorCodigo(filtrosPesquisa.CodigoPneu);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pneu", pneu.NumeroFogo, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pneu", false));

            if (filtrosPesquisa.Vida.HasValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Vida", filtrosPesquisa.Vida.Value.ObterDescricao(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Vida", false));

            if (filtrosPesquisa.SomenteSucata)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomenteSucata", "Sim", true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomenteSucata", false));

            if (filtrosPesquisa.CodigoServico > 0)
            {
                Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiuloFrota = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(_unitOfWork);
                Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servicoVeiculo = repServicoVeiuloFrota.BuscarPorCodigo(filtrosPesquisa.CodigoServico);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ServicoVeiculo", servicoVeiculo.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ServicoVeiculo", false));

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo.Descricao, true));
            }
            else
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));
            }

            if (filtrosPesquisa.SituacaoPneu.HasValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoPneu", filtrosPesquisa.SituacaoPneu.Value.ObterDescricao(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoPneu", false));

            if (!string.IsNullOrEmpty(filtrosPesquisa.DTO))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DTO", filtrosPesquisa.DTO, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DTO", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("UsuarioOperador", usuarioOperador?.Nome));

            if(filtrosPesquisa.CodigoAlmoxarifado.Count > 0)
            {
                Repositorio.Embarcador.Frota.Almoxarifado repAlmoxarifado = new Repositorio.Embarcador.Frota.Almoxarifado(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.Almoxarifado> almoxarifados = repAlmoxarifado.BuscarPorCodigos(filtrosPesquisa.CodigoAlmoxarifado);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Almoxarifados", string.Join(", ", almoxarifados.Select(o => o.Descricao)), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Almoxarifados", false));

            if (filtrosPesquisa.CodigoMotivoSucata.Count > 0)
            {
                Repositorio.Embarcador.Frota.MotivoSucateamentoPneu repMotivoSucateamentoPneu = new Repositorio.Embarcador.Frota.MotivoSucateamentoPneu(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu> motivosSucateamentoPneu = repMotivoSucateamentoPneu.BuscarPorCodigos(filtrosPesquisa.CodigoMotivoSucata);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("MotivoSucata", string.Join(", ", motivosSucateamentoPneu.Select(o => o.Descricao)), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("MotivoSucata", false));

            if (filtrosPesquisa.TiposAquisicao.Count > 0)
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoAquisicao", string.Join(", ", filtrosPesquisa.TiposAquisicao.Select(o => o.ObterDescricao())), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoAquisicao", false));
            
            return parametros;

        }
        public virtual void ExecutarPesquisaRelatorio(out List<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuHistorico> listaRegistros, out int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            AtualizarPropriedadesOrdenacaoEAgrupamento(parametrosConsulta);

            totalRegistros = ContarRegistros(filtrosPesquisa, propriedadesAgrupamento);
            listaRegistros = (totalRegistros > 0) ? ConsultarRegistros(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuHistorico>();
        }
        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataFormatada")
                return "Data";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}