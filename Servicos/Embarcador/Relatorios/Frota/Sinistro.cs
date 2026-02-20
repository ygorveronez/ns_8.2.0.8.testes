using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Threading;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Frota
{
    public class Sinistro : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioSinistro, Dominio.Relatorios.Embarcador.DataSource.Frota.Sinistro>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Frota.SinistroDados _repositorioSinistro;

        #endregion

        #region Construtores

        public Sinistro(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioSinistro = new Repositorio.Embarcador.Frota.SinistroDados(_unitOfWork);
        }

        public Sinistro(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioSinistro = new Repositorio.Embarcador.Frota.SinistroDados(_unitOfWork, cancellationToken);
        }


        #endregion

        #region
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Sinistro>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioSinistro filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioSinistro.ConsultarRelatorioSinistroAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList meotodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.Sinistro> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioSinistro filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioSinistro.ConsultarRelatorioSinistro(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioSinistro filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioSinistro.ContarConsultaRelatorioSinistro(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frota/Sinistro";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioSinistro filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Localidade repCidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Embarcador.Frota.TipoSinistro repTipoSinistro = new Repositorio.Embarcador.Frota.TipoSinistro(_unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(_unitOfWork);



            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo);
            Dominio.Entidades.Veiculo veiculoReboque = repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculoReboque);
            Dominio.Entidades.Localidade cidade = filtrosPesquisa.CodigoCidade > 0 ? repCidade.BuscarPorCodigo(filtrosPesquisa.CodigoCidade) : null;
            Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro tipoSinistro = repTipoSinistro.BuscarPorCodigo(filtrosPesquisa.CodigoTipoSinistro);
            Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista);

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroSinistro", filtrosPesquisa.NumeroSinistro));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CausadorSinistro", filtrosPesquisa.CausadorSinistro != CausadorSinistro.Todos ? filtrosPesquisa.CausadorSinistro?.ObterDescricao() : ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoSinistro", tipoSinistro?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroBoletimOcorrencia", filtrosPesquisa.NumeroBoletimOcorrencia));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataSinistroInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataSinistroFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Cidade", cidade?.Codigo));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo?.Placa_Formatada));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Reboque", veiculoReboque?.Placa_Formatada));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", motorista?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroOrdemServico", filtrosPesquisa.NumeroOrdemServico));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("IndicacaoPagador", filtrosPesquisa.IndicacaoPagador?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoSinistro", filtrosPesquisa.SituacaoSinistro?.ObterDescricao()));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataFormatada")
                return "Data";

            if (propriedadeOrdenarOuAgrupar == "DataHoraInclusaoFormatada")
                return "DataHoraInclusao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}