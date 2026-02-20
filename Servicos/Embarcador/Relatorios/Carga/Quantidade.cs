using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class Quantidade : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidade, Dominio.Relatorios.Embarcador.DataSource.Cargas.Quantidade.Quantidade>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.Carga _repositorioCarga;

        #endregion

        #region Construtores

        public Quantidade(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        }

        #endregion
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Quantidade.Quantidade> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidade filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioCarga.ConsultarRelatorioQuantidade(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidade filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCarga.ContarConsultaRelatorioQuantidade(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/Quantidade";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidade filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.Tipo.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial?.ToString("dd/MM/yyyy")));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal?.ToString("dd/MM/yyyy")));

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
                Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", $"{empresa.CNPJ_Formatado} - {empresa.RazaoSocial}", true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

            if (filtrosPesquisa.CodigoOperador > 0)
            {
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
                Dominio.Entidades.Usuario operador = repositorioUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoOperador);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Operador", operador.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Operador", false));

            if (filtrosPesquisa.CodigosCentroCarregamento.Count > 0)
            {
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);
                List<string> descricaoCentrosCarregamentos = new List<string>();

                foreach (var codigoCentroCarregamento in filtrosPesquisa.CodigosCentroCarregamento)
                {
                    Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento);

                    if (centroCarregamento != null)
                        descricaoCentrosCarregamentos.Add(centroCarregamento.Descricao);
                }

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroCarregamento", string.Join(", ", descricaoCentrosCarregamentos), true)); ;
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroCarregamento", false));

            if (filtrosPesquisa.CpfCnpjDestinatario > 0d)
            {
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
                Dominio.Entidades.Cliente destinatario = repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjDestinatario);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", $"{destinatario.CPF_CNPJ_Formatado} - {destinatario.Nome}", true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", false));

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
            {
                if (filtrosPesquisa.CodigosTipoCarga.Count == 1)
                {
                    Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repositorioTipoCarga.BuscarPorCodigo(filtrosPesquisa.CodigosTipoCarga.FirstOrDefault());

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", tipoCarga.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", "Múltiplos registros selecionados", true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", false));

            if (filtrosPesquisa.CodigoModeloVeiculo > 0)
            {
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeiculo = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeiculo = repositorioModeloVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoModeloVeiculo);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", modeloVeiculo.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", false));

            if (filtrosPesquisa.CodigoRota > 0)
            {
                Repositorio.RotaFrete repositorioRota = new Repositorio.RotaFrete(_unitOfWork);
                Dominio.Entidades.RotaFrete rota = repositorioRota.BuscarPorCodigo(filtrosPesquisa.CodigoRota);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Rota", rota.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Rota", false));

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo.Placa, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
            {
                if (filtrosPesquisa.CodigosFilial.Count > 0)
                {
                    Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo(filtrosPesquisa.CodigosFilial.FirstOrDefault());

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", filial.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", "Múltiplos registros selecionados", true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }
    }
}
