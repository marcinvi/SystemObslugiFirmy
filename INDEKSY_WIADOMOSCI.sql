-- ============================================
-- OPTYMALIZACJA WIADOMOŚCI - SUPER PROSTY SKRYPT
-- Wykonaj w MySQL Workbench lub phpMyAdmin
-- ============================================

-- 1. Indeks na DisputeId (NAJWAŻNIEJSZY!)
CREATE INDEX IF NOT EXISTS idx_disputeid 
ON AllegroChatMessages(DisputeId);

-- 2. Indeks na DisputeId + CreatedAt (dla sortowania)
CREATE INDEX IF NOT EXISTS idx_disputeid_created 
ON AllegroChatMessages(DisputeId, CreatedAt DESC);

-- 3. Indeks na CreatedAt (dla wyszukiwania ostatnich)
CREATE INDEX IF NOT EXISTS idx_created 
ON AllegroChatMessages(CreatedAt DESC);

-- 4. Indeks na AllegroDisputes
CREATE INDEX IF NOT EXISTS idx_allegrodisputes_accountid 
ON AllegroDisputes(AllegroAccountId);

-- 5. Indeks na ComplaintId
CREATE INDEX IF NOT EXISTS idx_allegrodisputes_complaintid 
ON AllegroDisputes(ComplaintId);

-- ============================================
-- SPRAWDŹ CZY DZIAŁA:
-- ============================================

-- To zapytanie powinno wykonać się < 100ms:
EXPLAIN SELECT 
    AD.DisputeId,
    AD.BuyerLogin,
    AD.AllegroAccountId,
    AD.HasNewMessages,
    AA.AccountName,
    Z.NrZgloszenia,
    (SELECT MessageText FROM AllegroChatMessages 
     WHERE DisputeId = AD.DisputeId 
     ORDER BY CreatedAt DESC LIMIT 1) as LastMessage,
    (SELECT CreatedAt FROM AllegroChatMessages 
     WHERE DisputeId = AD.DisputeId 
     ORDER BY CreatedAt DESC LIMIT 1) as LastDate
FROM AllegroDisputes AD
JOIN AllegroAccounts AA ON AD.AllegroAccountId = AA.Id
LEFT JOIN Zgloszenia Z ON AD.ComplaintId = Z.Id
WHERE AD.DisputeId IN (SELECT DISTINCT DisputeId FROM AllegroChatMessages)
ORDER BY LastDate DESC
LIMIT 500;

-- ============================================
-- GOTOWE!
-- ============================================
