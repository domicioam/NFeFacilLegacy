﻿// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace Venda.Impostos.DetalhamentoII
{
    public struct Detalhamento : IDetalhamentoImposto
    {
        public PrincipaisImpostos Tipo => PrincipaisImpostos.II;
    }
}
