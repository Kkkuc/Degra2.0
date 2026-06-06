set SERVEROUTPUT ON;

CREATE OR REPLACE PROCEDURE GenerateMonthlyStats(
    p_year IN NUMBER,
    p_month IN NUMBER
) IS
    CURSOR c_stats IS
        SELECT "TableName", "Operation", COUNT(*) as OpsCount
        FROM "Logs"
        WHERE EXTRACT(YEAR FROM "ChangedAt") = p_year
          AND EXTRACT(MONTH FROM "ChangedAt") = p_month
        GROUP BY "TableName", "Operation"
        ORDER BY OpsCount DESC, "TableName";

    v_total_operations NUMBER := 0;
BEGIN
    DBMS_OUTPUT.PUT_LINE('=========================================');
    DBMS_OUTPUT.PUT_LINE(' STATYSTYKI AUDYTU ZA: ' || TO_CHAR(p_month, 'FM00') || '/' || p_year);
    DBMS_OUTPUT.PUT_LINE('=========================================');
    
    FOR r_stat IN c_stats LOOP
        DBMS_OUTPUT.PUT_LINE(
            'Tabela: ' || RPAD(r_stat."TableName", 20) || 
            ' | Operacja: ' || RPAD(r_stat."Operation", 10) || 
            ' | Ilosc: ' || r_stat.OpsCount
        );
        
        v_total_operations := v_total_operations + r_stat.OpsCount;
    END LOOP;

    DBMS_OUTPUT.PUT_LINE('-----------------------------------------');
    IF v_total_operations = 0 THEN
        DBMS_OUTPUT.PUT_LINE('Brak operacji w wybranym miesiacu.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('SUMA WSZYSTKICH ZMIAN W BAZIE: ' || v_total_operations);
    END IF;
    DBMS_OUTPUT.PUT_LINE('=========================================');
    
EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Wystąpił błąd podczas generowania raportu: ' || SQLERRM);
END GenerateMonthlyStats;
/

BEGIN
    GENERATEMONTHLYSTATS(2026,5);
end;
/
--testy
insert into "Students"("Id", "StudentID", "FirstName", "LastName") values (3, 22334, 'Karol', 'Siemano');