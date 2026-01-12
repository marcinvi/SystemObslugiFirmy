using System;

namespace Reklamacje_Dane.Allegro.Issues
{
    public static class IssueDecisionHelper
    {
        /// <summary>
        /// Okno decyzyjne dot. zwrotu: 3 dni od otwarcia (lub DecisionDueDate z API),
        /// z tolerancją +1 dzień (jeśli termin wypada "do dziś", wciąż pozwalamy).
        /// </summary>
        public static bool IsReturnDecisionWindowOpen(
            DateTime openedUtc,
            DateTime? decisionDueUtcNullable = null,
            DateTime? nowUtc = null)
        {
            var now = nowUtc ?? DateTime.UtcNow;
            var baseDue = decisionDueUtcNullable ?? openedUtc.AddDays(3);

            // +1 dzień tolerancji
            var lastMoment = baseDue.AddDays(1);

            return now <= lastMoment;
        }
    }
}
