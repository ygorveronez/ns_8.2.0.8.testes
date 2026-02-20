using System.Linq;

namespace Servicos.Embarcador.NFSe
{
    public class NFSe : ServicoBase
    {        
        public NFSe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe BuscarConfiguracaoEmissaoNFSe(int codigoEmpresa, int localidade, string ufTomador, int grupoTomador, int localidadeTomador, int codigoTipoOperacao, double clienteTomador, int codigoTipoOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.ISS.RegrasCalculoISS regrasCalculoImpostos = Servicos.Embarcador.ISS.RegrasCalculoISS.GetInstance(unitOfWork);
            IQueryable<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe> queryListaRegras = regrasCalculoImpostos.ObterRegrasISS().AsQueryable();

            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);

            Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = null;

            if (codigoTipoOcorrencia > 0) {

                if (codigoTipoOperacao > 0)
                {
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, ufTomador, grupoTomador, localidadeTomador, codigoTipoOperacao, clienteTomador, codigoTipoOcorrencia, queryListaRegras);

                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, ufTomador, 0, 0, codigoTipoOperacao, 0, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, "", grupoTomador, 0, codigoTipoOperacao, 0, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, "", 0, 0, codigoTipoOperacao, clienteTomador, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, 0, ufTomador, 0, 0, codigoTipoOperacao, 0, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, 0, ufTomador, grupoTomador, 0, codigoTipoOperacao, 0, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, 0, ufTomador, 0, 0, codigoTipoOperacao, clienteTomador, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, "", 0, 0, codigoTipoOperacao, 0, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, "", 0, localidadeTomador, codigoTipoOperacao, 0, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, 0, "", 0, 0, codigoTipoOperacao, 0, codigoTipoOcorrencia, queryListaRegras);
                }

                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, ufTomador, grupoTomador, localidadeTomador, 0, 0, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, ufTomador, 0, localidadeTomador, 0, clienteTomador, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, ufTomador, 0, 0, 0, 0, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, "", grupoTomador, 0, 0, 0, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, "", 0, 0, 0, clienteTomador, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, 0, ufTomador, 0, 0, 0, 0, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, 0, ufTomador, grupoTomador, 0, 0, 0, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, 0, ufTomador, 0, 0, 0, clienteTomador, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, "", 0, 0, 0, 0, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, "", 0, localidadeTomador, 0, 0, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, 0, "", 0, 0, 0, 0, codigoTipoOcorrencia, queryListaRegras);
            }


            if (codigoTipoOperacao > 0)
            {
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, ufTomador, grupoTomador, localidadeTomador, codigoTipoOperacao, clienteTomador, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, ufTomador, 0, 0, codigoTipoOperacao, 0, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, "", grupoTomador, 0, codigoTipoOperacao, 0, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, "", 0, 0, codigoTipoOperacao, clienteTomador, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, 0, ufTomador, 0, 0, codigoTipoOperacao, 0, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, 0, ufTomador, grupoTomador, 0, codigoTipoOperacao, 0, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, 0, ufTomador, 0, 0, codigoTipoOperacao, clienteTomador, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, "", 0, 0, codigoTipoOperacao, 0, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, "", 0, localidadeTomador, codigoTipoOperacao, 0, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, 0, "", 0, 0, codigoTipoOperacao, 0, 0, queryListaRegras);
            }

            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, ufTomador, grupoTomador, localidadeTomador, 0, 0, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, ufTomador, 0, localidadeTomador, 0, clienteTomador, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, ufTomador, 0, 0, 0, 0, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, "", grupoTomador, 0, 0, 0, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, "", 0, 0, 0, clienteTomador, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, 0, ufTomador, 0, 0, 0, 0, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, 0, ufTomador, grupoTomador, 0, 0, 0, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, 0, ufTomador, 0, 0, 0, clienteTomador, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, "", 0, 0, 0, 0, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, localidade, "", 0, localidadeTomador, 0, 0, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(codigoEmpresa, 0, "", 0, 0, 0, 0, 0, queryListaRegras);

            return transportadorConfiguracaoNFSe;
        }

        public Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe BuscarConfiguracaoEmissaoNFSePorLocalidadeEmpresa(int localidade, string ufTomador, int grupoTomador, int localidadeTomador, int codigoTipoOperacao, double clienteTomador, int codigoTipoOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
            Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = null;
            Servicos.Embarcador.ISS.RegrasCalculoISS regrasCalculoImpostos = Servicos.Embarcador.ISS.RegrasCalculoISS.GetInstance(unitOfWork);
            IQueryable<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe> queryListaRegras = regrasCalculoImpostos.ObterRegrasISS().AsQueryable();

            if (codigoTipoOcorrencia > 0) {
                if (codigoTipoOperacao > 0)
                {
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, ufTomador, grupoTomador, localidadeTomador, codigoTipoOperacao, clienteTomador, codigoTipoOcorrencia, queryListaRegras);

                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, ufTomador, 0, 0, codigoTipoOperacao, 0, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", grupoTomador, 0, codigoTipoOperacao, 0, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", 0, 0, codigoTipoOperacao, clienteTomador, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, 0, ufTomador, 0, 0, codigoTipoOperacao, 0, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, 0, ufTomador, grupoTomador, 0, codigoTipoOperacao, 0, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, 0, ufTomador, 0, 0, codigoTipoOperacao, clienteTomador, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", 0, 0, codigoTipoOperacao, 0, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", 0, localidadeTomador, codigoTipoOperacao, 0, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", 0, 0, codigoTipoOperacao, 0, codigoTipoOcorrencia, queryListaRegras);
                    if (transportadorConfiguracaoNFSe == null)
                        transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, 0, "", 0, 0, codigoTipoOperacao, 0, codigoTipoOcorrencia, queryListaRegras);
                }

                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, ufTomador, grupoTomador, localidadeTomador, 0, 0, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, ufTomador, 0, localidadeTomador, 0, clienteTomador, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, ufTomador, 0, 0, 0, 0, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", grupoTomador, 0, 0, 0, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", 0, 0, 0, clienteTomador, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, 0, ufTomador, 0, 0, 0, 0, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, 0, ufTomador, grupoTomador, 0, 0, 0, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, 0, ufTomador, 0, 0, 0, clienteTomador, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", 0, 0, 0, 0, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", 0, localidadeTomador, 0, 0, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", 0, 0, 0, 0, codigoTipoOcorrencia, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, 0, "", 0, 0, 0, 0, codigoTipoOcorrencia, queryListaRegras);

            }

            if (codigoTipoOperacao > 0)
            {
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, ufTomador, grupoTomador, localidadeTomador, codigoTipoOperacao, clienteTomador, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, ufTomador, 0, 0, codigoTipoOperacao, 0, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", grupoTomador, 0, codigoTipoOperacao, 0, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", 0, 0, codigoTipoOperacao, clienteTomador, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, 0, ufTomador, 0, 0, codigoTipoOperacao, 0, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, 0, ufTomador, grupoTomador, 0, codigoTipoOperacao, 0, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, 0, ufTomador, 0, 0, codigoTipoOperacao, clienteTomador, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", 0, 0, codigoTipoOperacao, 0, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", 0, localidadeTomador, codigoTipoOperacao, 0, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", 0, 0, codigoTipoOperacao, 0, 0, queryListaRegras);
                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, 0, "", 0, 0, codigoTipoOperacao, 0, 0, queryListaRegras);
            }

            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, ufTomador, grupoTomador, localidadeTomador, 0, 0, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, ufTomador, 0, localidadeTomador, 0, clienteTomador, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, ufTomador, 0, 0, 0, 0, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", grupoTomador, 0, 0, 0, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", 0, 0, 0, clienteTomador, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, 0, ufTomador, 0, 0, 0, 0, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, 0, ufTomador, grupoTomador, 0, 0, 0, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, 0, ufTomador, 0, 0, 0, clienteTomador, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", 0, 0, 0, 0, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, localidade, "", 0, localidadeTomador, 0, 0, 0, queryListaRegras);
            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorLocalidadeEmpresa(localidade, 0, "", 0, 0, 0, 0, 0, queryListaRegras);

            return transportadorConfiguracaoNFSe;
        }


        public Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe BuscarPorLocalidadeEmpresaServico(int localidade, int empresa, int servico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
            Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = null;
            Servicos.Embarcador.ISS.RegrasCalculoISS regrasCalculoImpostos = Servicos.Embarcador.ISS.RegrasCalculoISS.GetInstance(unitOfWork);
            IQueryable<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe> queryListaRegras = regrasCalculoImpostos.ObterRegrasISS().AsQueryable();


            transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaLocalidadeServico(localidade, empresa, servico, queryListaRegras);

            return transportadorConfiguracaoNFSe;
        }

    }
}
