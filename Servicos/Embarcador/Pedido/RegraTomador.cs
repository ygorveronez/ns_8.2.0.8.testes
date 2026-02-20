using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Pedido
{
    public class RegraTomador
    {
        readonly Repositorio.UnitOfWork _unitOfWork;

        #region Contrutor

        public RegraTomador(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion


        public static Dominio.Entidades.Embarcador.Pedidos.RegraTomador BuscarRegraTomador(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {

            Dominio.Entidades.Embarcador.Pedidos.RegraTomador regraTomador = null;

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Repositorio.Embarcador.Pedidos.RegraTomador repRegraTomador = new Repositorio.Embarcador.Pedidos.RegraTomador(unitOfWork);

                if (repRegraTomador.PossuiRegras())
                {
                    Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                    bool origemFilial = repFilial.VerificarPorCNPJ(cargaPedido.Pedido.Remetente.CPF_CNPJ_SemFormato);
                    bool destinoFilial = repFilial.VerificarPorCNPJ(cargaPedido.Pedido?.Destinatario?.CPF_CNPJ_SemFormato);

                    List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> regrasParticipantes = repRegraTomador.BuscarPorRemetenteDestinatario(cargaPedido.Pedido.Remetente.CPF_CNPJ, cargaPedido.Pedido?.Destinatario?.CPF_CNPJ ?? 0);
                    if (regrasParticipantes.Count > 0)
                        regraTomador = RetornarRegraValida(regrasParticipantes, cargaPedido.Pedido.Remetente, cargaPedido.Pedido?.Destinatario, origemFilial, destinoFilial);

                    if (regraTomador == null)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> regrasRemetente = repRegraTomador.BuscarPorRemetenteDestinatario(cargaPedido.Pedido.Remetente.CPF_CNPJ, 0);
                        if (regrasRemetente.Count > 0)
                            regraTomador = RetornarRegraValida(regrasRemetente, cargaPedido.Pedido.Remetente, cargaPedido.Pedido?.Destinatario, origemFilial, destinoFilial);
                    }

                    if (regraTomador == null)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> regrasDestinatario = repRegraTomador.BuscarPorRemetenteDestinatario(0, cargaPedido.Pedido?.Destinatario?.CPF_CNPJ ?? 0);
                        if (regrasDestinatario.Count > 0)
                            regraTomador = RetornarRegraValida(regrasDestinatario, cargaPedido.Pedido.Remetente, cargaPedido.Pedido?.Destinatario, origemFilial, destinoFilial);
                    }

                    if (regraTomador == null)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> regrasFiliais = repRegraTomador.BuscarPorFilialNaoFilial();
                        if (regrasFiliais.Count > 0)
                            regraTomador = RetornarRegraValida(regrasFiliais, cargaPedido.Pedido.Remetente, cargaPedido.Pedido?.Destinatario, origemFilial, destinoFilial);
                    }

                }
            }
            return regraTomador;
        }

        public async Task<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> BuscarRegraTomadorAsync(Dominio.ObjetosDeValor.Embarcador.Regras.FiltroRegraTomador filtroRegraTomador)
        {
            if (filtroRegraTomador.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return null;

            Dominio.Entidades.Embarcador.Pedidos.RegraTomador regraTomador = null;

            Repositorio.Embarcador.Pedidos.RegraTomador repositorioRegraTomador = new Repositorio.Embarcador.Pedidos.RegraTomador(_unitOfWork);

            if (filtroRegraTomador.PossuiRegra || await repositorioRegraTomador.PossuiRegrasAsync())
            {

                bool origemFilial = await VerificarPorCNPJAsync(filtroRegraTomador.CargaPedido.Pedido.Remetente.CPF_CNPJ_SemFormato, filtroRegraTomador.Filiais, filtroRegraTomador.PossuiRegra);
                bool destinoFilial = await VerificarPorCNPJAsync(filtroRegraTomador.CargaPedido.Pedido?.Destinatario?.CPF_CNPJ_SemFormato, filtroRegraTomador.Filiais, filtroRegraTomador.PossuiRegra);

                List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> regrasParticipantes = await BuscarPorRemetenteDestinatarioAsync(filtroRegraTomador.CargaPedido.Pedido.Remetente.CPF_CNPJ, filtroRegraTomador.CargaPedido.Pedido?.Destinatario?.CPF_CNPJ ?? 0, filtroRegraTomador.RegrasTomadores, filtroRegraTomador.PossuiRegra);
                if (regrasParticipantes.Count > 0)
                    regraTomador = RetornarRegraValida(regrasParticipantes, filtroRegraTomador.CargaPedido.Pedido.Remetente, filtroRegraTomador.CargaPedido.Pedido?.Destinatario, origemFilial, destinoFilial);

                if (regraTomador == null)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> regrasRemetente = await BuscarPorRemetenteDestinatarioAsync(filtroRegraTomador.CargaPedido.Pedido.Remetente.CPF_CNPJ, 0, filtroRegraTomador.RegrasTomadores, filtroRegraTomador.PossuiRegra);
                    if (regrasRemetente.Count > 0)
                        regraTomador = RetornarRegraValida(regrasRemetente, filtroRegraTomador.CargaPedido.Pedido.Remetente, filtroRegraTomador.CargaPedido.Pedido?.Destinatario, origemFilial, destinoFilial);
                }

                if (regraTomador == null)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> regrasDestinatario = await BuscarPorRemetenteDestinatarioAsync(0, filtroRegraTomador.CargaPedido.Pedido?.Destinatario?.CPF_CNPJ ?? 0, filtroRegraTomador.RegrasTomadores, filtroRegraTomador.PossuiRegra);
                    if (regrasDestinatario.Count > 0)
                        regraTomador = RetornarRegraValida(regrasDestinatario, filtroRegraTomador.CargaPedido.Pedido.Remetente, filtroRegraTomador.CargaPedido.Pedido?.Destinatario, origemFilial, destinoFilial);
                }

                if (regraTomador == null)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> regrasFiliais = null;

                    if (filtroRegraTomador.RegrasTomadoresSemTomador?.Count > 0 || filtroRegraTomador.PossuiRegra)
                        regrasFiliais = filtroRegraTomador.RegrasTomadoresSemTomador;
                    else
                        regrasFiliais = await repositorioRegraTomador.BuscarPorFilialNaoFilialAsync();

                    if (regrasFiliais.Count > 0)
                        regraTomador = RetornarRegraValida(regrasFiliais, filtroRegraTomador.CargaPedido.Pedido.Remetente, filtroRegraTomador.CargaPedido.Pedido?.Destinatario, origemFilial, destinoFilial);
                }

            }

            return regraTomador;
        }

        private async Task<bool> VerificarPorCNPJAsync(string cnpj, List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = null, bool buscadoAnteriormente = false)
        {
            if (filiais?.Count > 0 || buscadoAnteriormente)
                return filiais.Exists(x => x.CNPJ == cnpj);

            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

            return await repositorioFilial.VerificarPorCNPJAsync(cnpj);
        }

        private async Task<List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>> BuscarPorRemetenteDestinatarioAsync(double remetente, double destinatario, List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> regrasTomadores = null, bool buscadoAnteriormente = false)
        {
            if (regrasTomadores?.Count > 0 || buscadoAnteriormente)
            {

                if (remetente > 0)
                    regrasTomadores = regrasTomadores.Where(obj => obj.Remetente != null && obj.Remetente.CPF_CNPJ == remetente).ToList();

                if (destinatario > 0)
                    regrasTomadores = regrasTomadores.Where(obj => obj.Destinatario != null && obj.Destinatario.CPF_CNPJ == destinatario).ToList();

                if (regrasTomadores.Any())
                    return regrasTomadores;

                return new List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador>();
            }

            Repositorio.Embarcador.Pedidos.RegraTomador repositorioRegraTomador = new Repositorio.Embarcador.Pedidos.RegraTomador(_unitOfWork);

            return await repositorioRegraTomador.BuscarPorRemetenteDestinatarioAsync(remetente, destinatario);
        }

        private static Dominio.Entidades.Embarcador.Pedidos.RegraTomador RetornarRegraValida(List<Dominio.Entidades.Embarcador.Pedidos.RegraTomador> regras, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, bool origemFilial, bool destinoFilial)
        {
            Dominio.Entidades.Embarcador.Pedidos.RegraTomador regraValida = null;
            foreach (Dominio.Entidades.Embarcador.Pedidos.RegraTomador regra in regras)
            {
                bool valido = true;
                if ((regra.Destinatario != null) && (destinatario != null))
                {
                    if (regra.Destinatario.CPF_CNPJ != destinatario.CPF_CNPJ)
                    {
                        valido = false;
                    }
                }
                else
                {
                    if (regra.DestinoFilial != destinoFilial)
                        valido = false;
                }

                if (regra.Remetente != null)
                {
                    if (regra.Remetente.CPF_CNPJ != remetente.CPF_CNPJ)
                    {
                        valido = false;
                    }
                }
                else
                {
                    if (regra.OrigemFilial != origemFilial)
                        valido = false;
                }

                if (valido)
                {
                    regraValida = regra;
                    break;
                }

            }
            return regraValida;
        }

    }
}
