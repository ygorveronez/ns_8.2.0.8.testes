using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Transportadores
{
    public class Empresa : ServicoBase
    {
        #region Construtores        

        public Empresa(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        #endregion Construtores

        #region Métodos Privados

        private Dominio.Entidades.Empresa BuscarEmpresaPorCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork);

            Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Empresa empresaEmissora = null;

            if (cargaPedido.Carga.TipoOperacao?.UsarConfiguracaoEmissao ?? false)
                empresaEmissora = cargaPedido.Carga.TipoOperacao.EmpresaEmissora;
            else if (tomador != null)
            {
                if (tomador.NaoUsarConfiguracaoEmissaoGrupo || tomador.GrupoPessoas == null)
                    empresaEmissora = tomador.EmpresaEmissora;
                else
                    empresaEmissora = tomador.GrupoPessoas.EmpresaEmissora;
            }

            if ((empresaEmissora == null) && (cargaPedido.Origem != null))
                empresaEmissora = repositorioEmpresa.BuscarEmpresaEmissoraEstado(cargaPedido.Origem.Estado);

            if (empresaEmissora == null && !(configuracaoPedido?.NaoSubstituirEmpresaNaGeracaoCarga ?? false))
                empresaEmissora = repositorioEmpresa.BuscarPrincipalEmissoraTMS();

            return empresaEmissora;
        }

        private string ObterProximoCodigoAlfanumerico(string codigoAlfanumerido)
        {
            if (string.IsNullOrWhiteSpace(codigoAlfanumerido) || (codigoAlfanumerido == "ZZ"))
                return "AA";

            char primeiraLetra = codigoAlfanumerido[0];
            char segundaLetra = codigoAlfanumerido[1];

            if (segundaLetra == 'Z')
            {
                primeiraLetra = (char)((int)primeiraLetra + 1);
                segundaLetra = 'A';
            }
            else
                segundaLetra = (char)((int)segundaLetra + 1);

            return $"{primeiraLetra}{segundaLetra}";
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public async Task AtualizarCodigoAlfanumericoAsync(Dominio.Entidades.Empresa empresa)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);

            empresa.CodigoAlfanumerico = ObterProximoCodigoAlfanumerico(empresa.CodigoAlfanumerico);

            await repositorioEmpresa.AtualizarAsync(empresa);
        }

        public Dominio.Entidades.Empresa BuscarEmpresaPadraoCarga(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos.FirstOrDefault();

            if (cargaPedido == null)
                return null;

            Dominio.Entidades.Empresa empresaEmissora = BuscarEmpresaPorCargaPedido(cargaPedido);

            return empresaEmissora;
        }

        public List<Dominio.Entidades.Empresa> BuscarEmpresasPorRaizCnpj(Dominio.Entidades.Empresa empresa)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            List<Dominio.Entidades.Empresa> empresas = repositorioEmpresa.BuscarPorRaizCNPJ(empresa.CNPJ.Left(8));

            return empresas;
        }

        #endregion Métodos Públicos
    }
}
