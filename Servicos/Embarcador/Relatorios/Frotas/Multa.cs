using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Frotas
{
    public class Multa : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMulta, Dominio.Relatorios.Embarcador.DataSource.Frota.Infracao>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Frota.Infracao _repositorioMulta;

        #endregion

        #region Construtores

        public Multa(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMulta = new Repositorio.Embarcador.Frota.Infracao(_unitOfWork);
        }

        public Multa(
    Repositorio.UnitOfWork unitOfWork,
    AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
    AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
    CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMulta = new Repositorio.Embarcador.Frota.Infracao(_unitOfWork, cancellationToken);
        }



        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Infracao>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMulta filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioMulta.ConsultarRelatorioInfracaoAsync(propriedadesAgrupamento, filtrosPesquisa, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList meotodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.Infracao> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMulta filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioMulta.ConsultarRelatorioInfracao(propriedadesAgrupamento, filtrosPesquisa, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMulta filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioMulta.ContarConsultaRelatorioInfracao(propriedadesAgrupamento, filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frotas/Multa";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMulta filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Frota.Infracao repInfracao = new Repositorio.Embarcador.Frota.Infracao(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Frota.TipoInfracao repTipoInfracao = new Repositorio.Embarcador.Frota.TipoInfracao(_unitOfWork);


            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo.Placa, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoTipoInfracao))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoTipoInfracao", ((TipoInfracaoTransito)filtrosPesquisa.TipoTipoInfracao.ToInt()).ObterDescricao(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoTipoInfracao", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NivelInfracao))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NivelInfracao", ((NivelInfracaoTransito)filtrosPesquisa.NivelInfracao.ToInt()).ObterDescricao(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NivelInfracao", false));

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", false));

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroAtuacao))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroInfracao", filtrosPesquisa.NumeroAtuacao, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroInfracao", false));

            if (filtrosPesquisa.NumeroMulta > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroMulta", filtrosPesquisa.NumeroMulta.ToString("n0"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroMulta", false));

            if ((int)filtrosPesquisa.PagoPor > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PagoPor", filtrosPesquisa.PagoPor.ObterDescricao(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PagoPor", false));

            if (filtrosPesquisa.CodigoCidade > 0)
            {
                Dominio.Entidades.Localidade obj = repLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoCidade);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Cidade", obj.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Cidade", false));

            if (filtrosPesquisa.CodigoTipoInfracao > 0)
            {
                Dominio.Entidades.Embarcador.Frota.TipoInfracao obj = repTipoInfracao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoInfracao);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoInfracao", obj.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoInfracao", false));

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                Dominio.Entidades.Usuario obj = repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", obj.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

            if (filtrosPesquisa.CnpjPessoa > 0)
            {
                Dominio.Entidades.Cliente obj = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjPessoa);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", obj.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", false));

            if (filtrosPesquisa.DataVencimentoInicial != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoInicial", filtrosPesquisa.DataVencimentoInicial.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoInicial", false));

            if (filtrosPesquisa.DataVencimentoFinal != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoFinal", filtrosPesquisa.DataVencimentoFinal.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoFinal", false));

            if (filtrosPesquisa.CodigoTitulo > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Titulo", filtrosPesquisa.CodigoTitulo.ToString("n0"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Titulo", false));

            if ((int)filtrosPesquisa.StatusMulta > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusMulta", filtrosPesquisa.StatusMulta.ObterDescricao(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusMulta", false));

            if (filtrosPesquisa.DataVencimentoInicialPagar != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoInicialPagar", filtrosPesquisa.DataVencimentoInicialPagar.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoInicialPagar", false));

            if (filtrosPesquisa.DataVencimentoFinalPagar != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoFinalPagar", filtrosPesquisa.DataVencimentoFinalPagar.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoFinalPagar", false));

            if (filtrosPesquisa.CnpjFornecedorPagar > 0)
            {
                Dominio.Entidades.Cliente obj = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjFornecedorPagar);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("FornecedorPagar", obj.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("FornecedorPagar", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLimiteInfracao", filtrosPesquisa.DataLimiteInicial, filtrosPesquisa.DataLimiteFinal, false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoOcorrenciaInfracao))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOcorrenciaInfracao", ((TipoOcorrenciaInfracao)filtrosPesquisa.TipoOcorrenciaInfracao.ToInt()).ObterDescricao(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOcorrenciaInfracao", false));


            if (filtrosPesquisa.DataLancamentoInicial != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLancamentoInicial", filtrosPesquisa.DataLancamentoInicial.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLancamentoInicial", false));

            if (filtrosPesquisa.DataLancamentoFinal != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLancamentoFinal", filtrosPesquisa.DataLancamentoFinal.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLancamentoFinal", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoMotorista", filtrosPesquisa.TipoMotorista.ObterDescricao()));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "Descricao")
                return "Descricao";

            if (propriedadeOrdenarOuAgrupar == "DataLancamentoFormatada")
                return "DataLancamento";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}