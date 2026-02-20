using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.NFe
{
    public class Notas : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotas, Dominio.Relatorios.Embarcador.DataSource.NFe.Notas>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.NotaFiscal.NotaFiscal _repositorioNotas;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        #endregion

        #region Construtores

        public Notas(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioNotas = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(_unitOfWork);
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.NFe.Notas> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotas filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioNotas.RelatorioNotas(filtrosPesquisa, parametrosConsulta,propriedadesAgrupamento, _tipoServicoMultisoftware).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotas filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioNotas.ContarRelatorioNotas(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/NFe/Notas";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(_unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Embarcador.Pessoas.CategoriaPessoa repositorioCategoriaPessoa = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);

            List<string> centrosResultado = (filtrosPesquisa.CodigosCentroResultado?.Count ?? 0) > 0 ? repCentroResultado.BuscarDescricaoPorCodigos(filtrosPesquisa.CodigosCentroResultado) : new List<string>();
            Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa categoriaPessoa = filtrosPesquisa.Categoria > 0 ? repositorioCategoriaPessoa.BuscarPorCodigo(filtrosPesquisa.Categoria) : null;


            parametros.Add(new Parametro("NumeroInicial", filtrosPesquisa.NumeroInicial));
            parametros.Add(new Parametro("NumeroFinal", filtrosPesquisa.NumeroFinal));

            parametros.Add(new Parametro("Serie", filtrosPesquisa.Serie));
            parametros.Add(new Parametro("CentroResultado", centrosResultado));

            if (filtrosPesquisa.OperadorLancamentoDocumento > 0)
            {
                Dominio.Entidades.Usuario usuarioOperadorLancamento = repUsuario.BuscarPorCodigo(filtrosPesquisa.OperadorLancamentoDocumento);
                parametros.Add(new Parametro("OperadorLancamento", usuarioOperadorLancamento.Nome, true));
            }
            else
            {
                parametros.Add(new Parametro("OperadorLancamento", false));
            }

            if (filtrosPesquisa.OperadorFinalizouDocumento > 0)
            {
                Dominio.Entidades.Usuario usuarioOperadorFinalizou = repUsuario.BuscarPorCodigo(filtrosPesquisa.OperadorFinalizouDocumento);
                parametros.Add(new Parametro("OperadorFinalizou", usuarioOperadorFinalizou.Nome, true));
            }
            else
            {
                parametros.Add(new Parametro("OperadorFinalizou", false));
            }

            if (filtrosPesquisa.CodigosNaturezaOperacao?.Count > 0)
            {
                List<Dominio.Entidades.NaturezaDaOperacao> naturezas = repNaturezaDaOperacao.BuscarPorIds(filtrosPesquisa.CodigosNaturezaOperacao);
                parametros.Add(new Parametro("Natureza", string.Join(", ", naturezas.Select(o => o.Descricao)), true));
            }
            else
                parametros.Add(new Parametro("Natureza", false));

            //if (codigoModelo > 0)
            if (filtrosPesquisa.CodigosModeloDocumentoFiscal != null && filtrosPesquisa.CodigosModeloDocumentoFiscal.Count > 0)
            {
                List<Dominio.Entidades.ModeloDocumentoFiscal> modelosDocumentosFiscais = filtrosPesquisa.CodigosModeloDocumentoFiscal.Count > 0 ?
                    repModelo.BuscarPorCodigo(filtrosPesquisa.CodigosModeloDocumentoFiscal.ToArray()) : new List<Dominio.Entidades.ModeloDocumentoFiscal>();
                parametros.Add(new Parametro("Modelo", string.Join(", ", from obj in modelosDocumentosFiscais select obj.Descricao), true));
            }
            else
                parametros.Add(new Parametro("Modelo", false));

            if (filtrosPesquisa.CnpjPessoa > 0)
            {
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjPessoa);
                parametros.Add(new Parametro("Cliente", "(" + cliente.CPF_CNPJ_Formatado + ") " + cliente.Nome, true));
            }
            else
                parametros.Add(new Parametro("Cliente", false));

            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("Tipo", filtrosPesquisa.TipoMovimento.ObterDescricao()));
            parametros.Add(new Parametro("Chave", filtrosPesquisa.Chave));
            parametros.Add(new Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar));

            if (filtrosPesquisa.CodigoEmpresa > 0)
                parametros.Add(new Parametro("Filial", repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa).RazaoSocial, true));
            else
                parametros.Add(new Parametro("Filial", false));

            if (filtrosPesquisa.CodigoVeiculo > 0)
                parametros.Add(new Parametro("Veiculo", repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo).Placa, true));
            else
                parametros.Add(new Parametro("Veiculo", false));

            parametros.Add(new Parametro("StatusNota", filtrosPesquisa.StatusNotaEntrada.ObterDescricao()));
            parametros.Add(new Parametro("SituacaoFinanceiraNota", filtrosPesquisa.SituacaoFinanceiraNotaEntrada.ObterDescricao()));

            if (!string.IsNullOrEmpty(filtrosPesquisa.EstadoEmitente) && filtrosPesquisa.EstadoEmitente != "0")
                parametros.Add(new Parametro("EstadoEmitente", filtrosPesquisa.EstadoEmitente, true));
            else
                parametros.Add(new Parametro("EstadoEmitente", false));

            if (filtrosPesquisa.CodigoSegmento > 0)
            {
                Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo segmento = repSegmentoVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoSegmento);
                parametros.Add(new Parametro("Segmento", segmento.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Segmento", false));

            parametros.Add(new Parametro("DataEntrada", filtrosPesquisa.DataEntradaInicial, filtrosPesquisa.DataEntradaFinal));

            if (filtrosPesquisa.CodigoEquipamento > 0)
            {
                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(filtrosPesquisa.CodigoEquipamento);
                parametros.Add(new Parametro("Equipamento", equipamento.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Equipamento", false));

            parametros.Add(new Parametro("Categoria", categoriaPessoa?.Descricao ?? string.Empty));


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