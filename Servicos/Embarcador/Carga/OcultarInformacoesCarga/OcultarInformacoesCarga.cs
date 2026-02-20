namespace Servicos.Embarcador.Carga.OcultarInformacoesCarga
{
    public class OcultarInformacoesCarga
    {
        Repositorio.UnitOfWork _unitOfWork;

        public OcultarInformacoesCarga(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public bool PossuiOcultarInformacoesCarga(int codigoUsuario)
        {
            Repositorio.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga repOcultar = new Repositorio.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);

            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigoUsuario);
            return repOcultar.PossuiOcultarInformacoesCarga(usuario.Codigo, usuario.PerfilAcesso?.Codigo ?? 0);
        }

        public Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ObterOcultarInformacoesCarga(int codigoUsuario)
        {
            Repositorio.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga repOcultar = new Repositorio.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);

            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigoUsuario);
            return repOcultar.BuscarOcultarInformacoesPorUsuarioEPerfil(usuario.Codigo, usuario.PerfilAcesso?.Codigo ?? 0);
        }

        public object ValidarOcultarValor(Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ocultarInformacoesCarga, Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga.TipoValorOcultarInformacoesCarga tipoValor, object valorOriginal)
        {
            bool possuiRestricao = PossuiRestricaoPorTipo(ocultarInformacoesCarga, tipoValor);

            if (possuiRestricao)
                return "";
            else
                return valorOriginal;
        }

        public decimal ValidarOcultarValor(Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ocultarInformacoesCarga, Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga.TipoValorOcultarInformacoesCarga tipoValor, decimal valorOriginal)
        {
            bool possuiRestricao = PossuiRestricaoPorTipo(ocultarInformacoesCarga, tipoValor);

            if (possuiRestricao)
                return 0;
            else
                return valorOriginal;
        }

        private bool PossuiRestricaoPorTipo(Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ocultarInformacoesCarga, Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga.TipoValorOcultarInformacoesCarga tipoValor)
        {
            switch (tipoValor)
            {
                case Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga.TipoValorOcultarInformacoesCarga.ValorFrete:
                    return ocultarInformacoesCarga.ValorFrete;
                case Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga.TipoValorOcultarInformacoesCarga.Rota:
                    return ocultarInformacoesCarga.Rota;
                case Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga.TipoValorOcultarInformacoesCarga.ValorNotaFiscal:
                    return ocultarInformacoesCarga.ValorNotaFiscal;
                case Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga.TipoValorOcultarInformacoesCarga.ValorProdutos:
                    return ocultarInformacoesCarga.ValorProduto;
                default:
                    return false;
            }
        }
    }
}
