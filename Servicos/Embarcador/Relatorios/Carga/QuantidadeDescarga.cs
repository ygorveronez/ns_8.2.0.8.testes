using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class QuantidadeDescarga : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidadeDescarga, Dominio.Relatorios.Embarcador.DataSource.Cargas.QuantidadeDescarga.QuantidadeDescarga>
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Repositorio.Embarcador.Cargas.Carga _repositorioCarga;

        #endregion

        #region Construtores

        public QuantidadeDescarga(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        #endregion Métodos Privados

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.QuantidadeDescarga.QuantidadeDescarga> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidadeDescarga filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            filtrosPesquisa.TempoSemPosicaoParaVeiculoPerderSinal = ObterConfiguracaoEmbarcador().TempoSemPosicaoParaVeiculoPerderSinal;

            return _repositorioCarga.ConsultarRelatorioQuantidadeDescarregamento(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }



        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidadeDescarga filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            filtrosPesquisa.TempoSemPosicaoParaVeiculoPerderSinal = ObterConfiguracaoEmbarcador().TempoSemPosicaoParaVeiculoPerderSinal;

            return _repositorioCarga.ContarConsultaRelatorioQuantidadeDescarregamento(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/QuantidadeDescarga";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidadeDescarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Logistica.CentroDescarregamento repCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeiculo = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);


            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeiculo = filtrosPesquisa.CodigoModeloVeiculo > 0 ? repModeloVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoModeloVeiculo) : new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga();
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCarga = filtrosPesquisa.CodigosTipoCarga.Count > 0 ? repTipoCarga.BuscarPorCodigos(filtrosPesquisa.CodigosTipoCarga) : null;
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = filtrosPesquisa.CodigosTipoOperacao.Count > 0 ? repTipoOperacao.BuscarPorCodigos(filtrosPesquisa.CodigosTipoOperacao) : null;
            Dominio.Entidades.RotaFrete rota = filtrosPesquisa.CodigoRota > 0 ? repRotaFrete.BuscarPorCodigo(filtrosPesquisa.CodigoRota) : null;
            Dominio.Entidades.Cliente destinatario = filtrosPesquisa.CpfCnpjDestinatario > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjDestinatario) : null;
            //Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = filtrosPesquisa.CodigoCentroDescarregamento > 0 ? repCentroDescarregamento.BuscarPorCodigo(filtrosPesquisa.CodigoCentroDescarregamento) : new Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento();
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = filtrosPesquisa.CodigosFilial.Count > 0 ? repFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFilial) : null;
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoTransportador > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador) : null;
            Dominio.Entidades.Usuario operador = filtrosPesquisa.CodigoOperador > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoOperador) : null;

            List<string> descricaoCentroDescarregamento = new List<string>();
            if (filtrosPesquisa.CodigosCentroDescarregamento.Count > 0)
            {
                foreach(var codigoCentroDescarregamento in filtrosPesquisa.CodigosCentroDescarregamento)
                {
                    Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repCentroDescarregamento.BuscarPorCodigo(codigoCentroDescarregamento);

                    if (centroDescarregamento != null)
                        descricaoCentroDescarregamento.Add(centroDescarregamento.Descricao);
                }
            }

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial?.ToString("dd/MM/yyyy") ?? "", true));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal?.ToString("dd/MM/yyyy") ?? "", true));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.Situacao?.Count > 0 ? string.Join(", ", filtrosPesquisa.Situacao.Select(o => o.ObterDescricao()).ToList()) : ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa != null ? empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Operador", operador?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", modeloVeiculo.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TiposCarga", tiposCarga != null && tiposCarga.Count > 0 ? string.Join(", ", tiposCarga.Select(o => o.Descricao)) : ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TiposOperacao", tiposOperacao != null && tiposOperacao.Count > 0 ? string.Join(", ", tiposOperacao.Select(o => o.Descricao)) : ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Rota", rota?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", destinatario?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroDescarregamento", string.Join(", ",descricaoCentroDescarregamento)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filiais", filiais != null && filiais.Count > 0 ? string.Join(", ", filiais.Select(o => o.Descricao)) : ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo?.Placa));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}